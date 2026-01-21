using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.PhieuBan;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Utils;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.Repositories
{
    public class PhieuBanRepository : IPhieuBanRepository
    {
        private readonly VetFeedManagementContext _context;

        public PhieuBanRepository(VetFeedManagementContext context)
        {
            _context = context;
        }

        // lay danh sach phieu ban
        public async Task<IEnumerable<PhieuBanResponse>> GetDanhSachPhieuBanAsync()
        {
            var result = await _context.PhieuBans
                .Include(pb => pb.KhachHang)
                .Select(pb => new PhieuBanResponse
                {
                    MaPB = pb.MaPB,
                    MaPBCode = pb.MaPBCode,
                    MaKH = pb.MaKH,
                    TenKhachHang = pb.KhachHang!.TenKH,
                    NgayBan = pb.NgayBan,
                    TongTienHang = pb.TongTienHang,
                    ChietKhauPhanTram = pb.ChietKhauPhanTram,
                    TienChietKhau = pb.TienChietKhau,
                    ThanhTien = pb.ThanhTien,
                    HinhThucThanhToan = pb.HinhThucThanhToan.ToString(),
                    TrangThaiThanhToan = pb.TrangThaiThanhToan.ToString(),
                    TienCoc = pb.TienCoc,
                    TienNo = pb.TienNo,
                    HanTra = pb.HanTra,
                    GhiChu = pb.GhiChu
                })
                .OrderByDescending(pb => pb.NgayBan)
                .ToListAsync();

            return result;
        }

        // lay chi tiet phieu ban
        public async Task<PhieuBanDetailResponse?> GetChiTietPhieuBanAsync(Guid maPB)
        {
            var phieuBan = await _context.PhieuBans
                .Include(pb => pb.KhachHang)
                .FirstOrDefaultAsync(pb => pb.MaPB == maPB);

            if (phieuBan == null)
                return null;

            // Lấy toàn bộ chi tiết phiếu bán
            var chiTietGoc = await _context.CTPhieuBans
                .Where(ct => ct.MaPB == maPB)
                .Include(ct => ct.KhoHang)
                .Include(ct => ct.LoHang)
                    .ThenInclude(lh => lh!.SanPham)
                .ToListAsync();

            // Gộp theo MaLo
            var chiTietGop = chiTietGoc
                .GroupBy(ct => ct.MaLo)
                .Select(group =>
                {
                    var dauTien = group.First();
                    var sanPham = dauTien.LoHang!.SanPham!;
                    var donViBan = dauTien.DonViBan;

                    // Lấy tỷ lệ quy đổi từ đơn vị bán sang đơn vị cơ sở
                    var tyLe = _context.QuyDoiDonVis
                        .Where(qd => qd.MaSP == sanPham.MaSP && qd.DonViNhap == donViBan)
                        .Select(qd => qd.TyLe)
                        .FirstOrDefault();

                    var tyLeDung = tyLe > 0 ? tyLe : 1;

                    return new ChiTietPhieuBanResponse
                    {
                        MaCTPB = dauTien.MaCTPB, // giữ ID của chi tiết đầu tiên
                        MaLo = group.Key,
                        MaLoCode = dauTien.LoHang.MaLoCode,
                        TenSanPham = sanPham.TenSP,
                        DonViCoSo = sanPham.DonViCoSo,
                        DonViBan = donViBan,
                        DonGia = dauTien.DonGia,
                        HanSuDung = dauTien.LoHang.HanSuDung,
                        // Tổng số lượng quy đổi (đơn vị cơ sở)
                        SoLuongQuyDoi = group.Sum(x => x.SoLuongQuyDoi),
                        // Quy đổi ngược ra đơn vị bán để hiển thị
                        SoLuong = Math.Round(group.Sum(x => x.SoLuongQuyDoi) / tyLeDung, 2),
                        GiaVonCoSo = group.Average(x => x.GiaVonCoSo),
                        ThanhTienVon = group.Sum(x => x.ThanhTienVon),
                        GhiChu = string.Join(" | ", group.Select(x => x.GhiChu).Where(x => !string.IsNullOrEmpty(x)))
                    };
                })
                .ToList();

            return new PhieuBanDetailResponse
            {
                MaPB = phieuBan.MaPB,
                MaPBCode = phieuBan.MaPBCode,
                MaKH = phieuBan.MaKH,
                TenKhachHang = phieuBan.KhachHang!.TenKH,
                NgayBan = phieuBan.NgayBan,
                TongTienHang = phieuBan.TongTienHang,
                ChietKhauPhanTram = phieuBan.ChietKhauPhanTram,
                TienChietKhau = phieuBan.TienChietKhau,
                ThanhTien = phieuBan.ThanhTien,
                HinhThucThanhToan = phieuBan.HinhThucThanhToan.ToString(),
                TrangThaiThanhToan = phieuBan.TrangThaiThanhToan.ToString(),
                TienCoc = phieuBan.TienCoc,
                TienNo = phieuBan.TienNo,
                HanTra = phieuBan.HanTra,
                GhiChu = phieuBan.GhiChu,
                DanhSachChiTiet = chiTietGop
            };
        }


        // tao phieu ban
        public async Task<PhieuBanDetailResponse> CreatePhieuBanAsync(CreatePhieuBanRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // kiem tra khach hang
                var khachHang = await _context.KhachHangs.FindAsync(request.MaKH)
                    ?? throw new Exception("Khách hàng không tồn tại!");

                // KIỂM TRA TRẠNG THÁI KHÁCH HÀNG
                if (khachHang.TrangThai == TrangThaiKhachHangEnum.KHOA)
                {
                    throw new Exception($"Khách hàng '{khachHang.TenKH}' đã bị khóa, không thể tạo phiếu bán!");
                }

                if (request.HinhThucThanhToan != HinhThucThanhToanEnum.CONG_NO)
                {
                    if (request.TienCoc > 0)
                        throw new Exception("Thanh toán tiền mặt/chuyển khoản không được có tiền cọc");
                }
                else
                {
                    if (request.HanTra == null)
                        throw new Exception("Bán công nợ bắt buộc phải có hạn trả");
                    
                    // KIỂM TRA NGÀY BÁN PHẢI NHỎ HƠN HẠN TRẢ
                    if (request.NgayBan.Date >= request.HanTra.Value.Date)
                    {
                        throw new Exception("Ngày bán phải nhỏ hơn hạn trả!");
                    }
                }


                if (request.HinhThucThanhToan == HinhThucThanhToanEnum.CONG_NO)
                {
                    var tongTienHangTam = request.DanhSachChiTiet!
                        .Sum(ct => ct.SoLuong * ct.DonGia);

                    var tienChietKhauTam = tongTienHangTam * (request.ChietKhauPhanTram / 100);
                    var thanhTienTam = tongTienHangTam - tienChietKhauTam;
                    var tienNoTam = thanhTienTam - request.TienCoc;

                    var congNoMoi = khachHang.CongNoHienTai + tienNoTam;
                    if (congNoMoi > khachHang.HanMucCongNo)
                        throw new Exception("Khách hàng vượt hạn mức công nợ!");
                }

                var chiTietPhieuBanList = new List<CTPhieuBan>();
                decimal tongTienHang = 0;

                var chiTietTheoLo = request.DanhSachChiTiet!
                    .GroupBy(x => x.MaLo)
                    .ToList();

                foreach (var group in chiTietTheoLo)
                {
                    var maLo = group.Key;
                    var soLuongCanBan = group.Sum(x => x.SoLuong);

                    var loHang = await _context.LoHangs
                        .Include(lh => lh.SanPham)
                        .FirstOrDefaultAsync(lh => lh.MaLo == maLo)
                        ?? throw new Exception("Lô hàng không tồn tại");

                    // Lấy tồn kho theo lô (nhiều kho)
                    var tonKhoList = await _context.TonKhos
                        .Where(tk => tk.MaLo == maLo)
                        .OrderBy(tk => tk.SoLuongCoSo)
                        .ToListAsync();

                    var tongTonKho = tonKhoList.Sum(tk => tk.SoLuongCoSo);
                    if (tongTonKho < soLuongCanBan)
                        throw new Exception($"Lô {loHang.MaLoCode} không đủ tồn kho");

                    var soLuongConLai = soLuongCanBan;
                    var chiTietRequest = group.First();

                    tongTienHang += soLuongCanBan * chiTietRequest.DonGia;

                    foreach (var tonKho in tonKhoList)
                    {
                        if (soLuongConLai <= 0) break;

                        var layTuKho = Math.Min(soLuongConLai, tonKho.SoLuongCoSo);

                        chiTietPhieuBanList.Add(new CTPhieuBan
                        {
                            MaCTPB = Guid.NewGuid(),
                            MaKho = tonKho.MaKho,
                            MaLo = maLo,
                            SoLuong = layTuKho,
                            DonGia = chiTietRequest.DonGia,
                            DonViBan = loHang.SanPham!.DonViCoSo,
                            SoLuongQuyDoi = layTuKho,
                            GiaVonCoSo = tonKho.GiaVonBinhQuan,
                            ThanhTienVon = layTuKho * tonKho.GiaVonBinhQuan,
                            GhiChu = chiTietRequest.GhiChu
                        });

                        tonKho.SoLuongCoSo -= layTuKho;
                        soLuongConLai -= layTuKho;
                    }
                }


                var tienChietKhau = tongTienHang * (request.ChietKhauPhanTram / 100);
                var thanhTien = tongTienHang - tienChietKhau;

                decimal tienNo = 0;
                if (request.HinhThucThanhToan == HinhThucThanhToanEnum.CONG_NO)
                {
                    tienNo = thanhTien - request.TienCoc;
                }


                var trangThaiThanhToan =
                    request.HinhThucThanhToan == HinhThucThanhToanEnum.CONG_NO
                    ? TrangThaiThanhToanEnum.CHUA_THANH_TOAN
                    : TrangThaiThanhToanEnum.DA_THANH_TOAN;

                var phieuBan = new PhieuBan
                {
                    MaPBCode = await CodeGenerator.GeneratePhieuBanCodeAsync(_context),
                    MaKH = request.MaKH,
                    NgayBan = request.NgayBan,
                    TongTienHang = tongTienHang,
                    ChietKhauPhanTram = request.ChietKhauPhanTram,
                    TienChietKhau = tienChietKhau,
                    ThanhTien = thanhTien,
                    HinhThucThanhToan = request.HinhThucThanhToan,
                    TrangThaiThanhToan = trangThaiThanhToan,
                    TienCoc = request.TienCoc,
                    TienNo = tienNo,
                    HanTra = request.HanTra,
                    GhiChu = request.GhiChu
                };

                _context.PhieuBans.Add(phieuBan);
                await _context.SaveChangesAsync();

                chiTietPhieuBanList.ForEach(ct => ct.MaPB = phieuBan.MaPB);
                _context.CTPhieuBans.AddRange(chiTietPhieuBanList);


                if (request.HinhThucThanhToan == HinhThucThanhToanEnum.CONG_NO && tienNo > 0)
                {
                    _context.CongNos.Add(new CongNo
                    {
                        MaCongNo = Guid.NewGuid(),
                        LoaiDoiTuong = LoaiDoiTuongCongNoEnum.KHACH_HANG,
                        MaDoiTuong = request.MaKH,
                        MaPhieu = phieuBan.MaPB,
                        SoTien = tienNo,
                        NgayPhatSinh = DateTime.Now,
                        HanThanhToan = request.HanTra,
                        GhiChu = $"Công nợ từ phiếu bán {phieuBan.MaPBCode}"
                    });

                    khachHang.CongNoHienTai += tienNo;
                }

                khachHang.TongMua += thanhTien;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetChiTietPhieuBanAsync(phieuBan.MaPB)
                    ?? throw new Exception("Lỗi tạo phiếu bán");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        // Xóa phieuu bán
        public async Task<bool> DeletePhieuBanAsync(Guid maPB)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //  Lấy phiếu bán + chi tiết + khách hàng
                var phieuBan = await _context.PhieuBans
                    .Include(pb => pb.KhachHang)
                    .Include(pb => pb.CTPhieuBans)
                    .FirstOrDefaultAsync(pb => pb.MaPB == maPB);

                if (phieuBan == null)
                    throw new InvalidOperationException("Phiếu bán không tồn tại.");

                // Không cho xóa nếu đã có phiếu trả
                bool hasReturn = await _context.PhieuTras
                    .AnyAsync(pt => pt.MaPB == maPB);

                if (hasReturn)
                    throw new InvalidOperationException("Phiếu bán đã có phiếu trả, không thể xóa.");

                // ROLLBACK TỒN KHO (CHỈ ĐƠN VỊ CƠ SỞ)
                foreach (var ct in phieuBan.CTPhieuBans!)
                {
                    var tonKho = await _context.TonKhos
                        .FirstOrDefaultAsync(tk =>
                            tk.MaKho == ct.MaKho &&
                            tk.MaLo == ct.MaLo);

                    if (tonKho == null)
                    {
                        // Trường hợp hiếm: tồn kho đã bị xóa
                        tonKho = new TonKho
                        {
                            MaTonKho = Guid.NewGuid(),
                            MaKho = ct.MaKho,
                            MaLo = ct.MaLo,
                            SoLuongCoSo = 0,
                            GiaVonBinhQuan = ct.GiaVonCoSo
                        };
                        _context.TonKhos.Add(tonKho);
                    }

                    // Hoàn lại đúng số lượng cơ sở đã xuất
                    tonKho.SoLuongCoSo += ct.SoLuong;
                }

                //  ROLLBACK CÔNG NỢ (NẾU BÁN CÔNG NỢ)
                if (phieuBan.HinhThucThanhToan == HinhThucThanhToanEnum.CONG_NO)
                {
                    var congNos = await _context.CongNos
                        .Where(cn =>
                            cn.MaPhieu == phieuBan.MaPB &&
                            cn.LoaiDoiTuong == LoaiDoiTuongCongNoEnum.KHACH_HANG)
                        .ToListAsync();

                    decimal tongRollback = congNos.Sum(cn => cn.SoTien);

                    phieuBan.KhachHang!.CongNoHienTai -= tongRollback;
                    if (phieuBan.KhachHang.CongNoHienTai < 0)
                        phieuBan.KhachHang.CongNoHienTai = 0;

                    if (congNos.Any())
                        _context.CongNos.RemoveRange(congNos);
                }

                // ROLLBACK TỔNG MUA
                phieuBan.KhachHang!.TongMua -= phieuBan.ThanhTien;
                if (phieuBan.KhachHang.TongMua < 0)
                    phieuBan.KhachHang.TongMua = 0;

                // XÓA DỮ LIỆU
                _context.CTPhieuBans.RemoveRange(phieuBan.CTPhieuBans);
                _context.PhieuBans.Remove(phieuBan);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        // lay lich su mua hang cua khach hang 
        public async Task<KhachHangPhieuBanResponse?> GetPhieuBanByKhachHangAsync(Guid maKH)
        {
            var khachHang = await _context.KhachHangs
                .Where(kh => kh.MaKH == maKH)
                .Select(kh => new KhachHangPhieuBanResponse
                {
                    MaKH = kh.MaKH,
                    MaKHCode = kh.MaKHCode!,
                    TenKH = kh.TenKH!,
                    SoDienThoai = kh.SoDienThoai,
                    DiaChi = kh.DiaChi,
                    LoaiKhachHang = kh.LoaiKhachHang.ToString(),
                    TongMua = kh.TongMua,
                    CongNoHienTai = kh.CongNoHienTai,
                    HanMucCongNo = kh.HanMucCongNo
                })
                .FirstOrDefaultAsync();

            if (khachHang == null)
                return null;

            khachHang.DanhSachPhieuBan = await _context.PhieuBans
                .Where(pb => pb.MaKH == maKH)
                .OrderByDescending(pb => pb.NgayBan)
                .Select(pb => new PhieuBanListResponse
                {
                    MaPB = pb.MaPB,
                    MaPBCode = pb.MaPBCode!,
                    NgayBan = pb.NgayBan,

                    TongTienHang = pb.TongTienHang,
                    TienChietKhau = pb.TienChietKhau,
                    ThanhTien = pb.ThanhTien,

                    HinhThucThanhToan = pb.HinhThucThanhToan!.ToString(),
                    TrangThaiThanhToan = pb.TrangThaiThanhToan!.ToString(),
                    TienNo = pb.TienNo,
                    GhiChu = pb.GhiChu
                })
                .ToListAsync();

            return khachHang;
        }


    }
}
