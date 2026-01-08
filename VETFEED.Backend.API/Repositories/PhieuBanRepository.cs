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
                // Kiểm tra khách hàng tồn tại
                var khachHang = await _context.KhachHangs.FindAsync(request.MaKH);
                if (khachHang == null)
                    throw new Exception("Khách hàng không tồn tại!");

                // Kiểm tra hạn mức công nợ TRƯỚC khi tạo phiếu
                if (request.HinhThucThanhToan == HinhThucThanhToanEnum.CONG_NO)
                {
                    // Tính tạm thành tiền để kiểm tra
                    var tongTienHangTam = decimal.Zero;
                    
                    // Gom nhóm chi tiết theo lô để tính tạm tiền
                    var chiTietTheoLoTam = request.DanhSachChiTiet!
                        .GroupBy(ct => ct.MaLo)
                        .ToList();

                    foreach (var groupLoHang in chiTietTheoLoTam)
                    {
                        var tongSoLuongCanBan = groupLoHang.Sum(ct => ct.SoLuong);
                        var chiTietRequest = request.DanhSachChiTiet!.First(ct => ct.MaLo == groupLoHang.Key);
                        var thanhTienSanPham = tongSoLuongCanBan * chiTietRequest.DonGia;
                        tongTienHangTam += thanhTienSanPham;
                    }

                    // Tính tạm thành tiền sau chiết khấu
                    var tienChietKhauTam = tongTienHangTam * (request.ChietKhauPhanTram / 100);
                    var thanhTienTam = tongTienHangTam - tienChietKhauTam;
                    var tienNoTam = thanhTienTam - request.TienCoc;

                    // Kiểm tra hạn mức công nợ
                    var congNoMoi = khachHang.CongNoHienTai + tienNoTam;
                    if (congNoMoi > khachHang.HanMucCongNo)
                    {
                        var congNoVuotMuc = congNoMoi - khachHang.HanMucCongNo;
                        throw new Exception($"Khách hàng sẽ vượt hạn mức công nợ! " +
                            $"Công nợ hiện tại: {khachHang.CongNoHienTai:N0}đ, " +
                            $"Sẽ nợ thêm: {tienNoTam:N0}đ, " +
                            $"Tổng sẽ nợ: {congNoMoi:N0}đ, " +
                            $"Hạn mức: {khachHang.HanMucCongNo:N0}đ, " +
                            $"Vượt mức: {congNoVuotMuc:N0}đ");
                    }
                }

                //  Xử lý tồn kho theo từng lô
                var chiTietPhieuBanList = new List<CTPhieuBan>();
                var tongTienHang = decimal.Zero;

                // Gom nhóm chi tiết theo lô để dễ xử lý
                var chiTietTheoLo = request.DanhSachChiTiet!
                    .GroupBy(ct => ct.MaLo)
                    .ToList();

                foreach (var groupLoHang in chiTietTheoLo)
                {
                    var maLo = groupLoHang.Key;
                    var tongSoLuongCanBan = groupLoHang.Sum(ct => ct.SoLuong);

                    // Lấy thông tin lô hàng
                    var loHang = await _context.LoHangs
                        .Include(lh => lh.SanPham)
                        .FirstOrDefaultAsync(lh => lh.MaLo == maLo);

                    if (loHang == null)
                        throw new Exception($"Lô hàng {maLo} không tồn tại!");

                    // Tính quy đổi để kiểm tra tồn kho đúng
                    // Lấy quy đổi từ request (có thể là Thùng, Hộp, v.v.)
                    var chiTietRequestDauTien = request.DanhSachChiTiet!.First(ct => ct.MaLo == maLo);
                    var quyDoiKiemTra = await _context.QuyDoiDonVis
                        .FirstOrDefaultAsync(qd => qd.MaSP == loHang.MaSP && qd.DonViNhap == chiTietRequestDauTien.DonViBan);

                    //Tính số lượng cần bán theo đơn vị cơ sở
                    decimal tongSoLuongCanBanCoSo = tongSoLuongCanBan;
                    
                    // Nếu đơn vị bán = đơn vị cơ sở, không cần quy đổi
                    // Nếu đơn vị bán ≠ đơn vị cơ sở, tính quy đổi
                    if (quyDoiKiemTra != null)
                    {
                        // Có quy đổi → Nhân với TyLe
                        tongSoLuongCanBanCoSo = tongSoLuongCanBan * quyDoiKiemTra.TyLe;
                    }
                    else if (chiTietRequestDauTien.DonViBan != loHang.SanPham!.DonViCoSo)
                    {
                        // Không có quy đổi nhưng đơn vị bán ≠ đơn vị cơ sở
                        throw new Exception($"Sản phẩm {loHang.SanPham.TenSP}: Không tìm được quy đổi từ {chiTietRequestDauTien.DonViBan} sang {loHang.SanPham.DonViCoSo}!");
                    }

                    //Kiểm tra tồn kho từ tất cả các kho
                    var tonKhoTheoKho = await _context.TonKhos
                        .Where(tk => tk.MaLo == maLo)
                        .Include(tk => tk.KhoHang)
                        .OrderBy(tk => tk.SoLuongCoSo)
                        .ToListAsync();

                    // Kiểm tra tổng tồn kho đủ không
                    var tongTonKho = tonKhoTheoKho.Sum(tk => tk.SoLuongCoSo);
                    if (tongTonKho < tongSoLuongCanBanCoSo)
                        throw new Exception($"Lô {loHang.MaLoCode}: Tồn kho không đủ! Cần {tongSoLuongCanBanCoSo} {loHang.SanPham!.DonViCoSo}, tồn {tongTonKho} {loHang.SanPham.DonViCoSo}");

                    // Tạo chi tiết phiếu bán từ các kho khác nhau
                    var soLuongConLai = tongSoLuongCanBanCoSo;

                    foreach (var tonKho in tonKhoTheoKho)
                    {
                        if (soLuongConLai <= 0)
                            break;

                        // Lấy số lượng từ kho này (theo đơn vị cơ sở)
                        var soLuongLayTuKhoNay = Math.Min(soLuongConLai, tonKho.SoLuongCoSo);

                        // Lấy thông tin quy đổi đơn vị từ request
                        var chiTietRequest = request.DanhSachChiTiet!.First(ct => ct.MaLo == maLo);

                        // Tính thành tiền dựa trên SỐ LƯỢNG REQUEST (tròn)
                        var thanhTienSanPham = tongSoLuongCanBan * chiTietRequest.DonGia;
                        
                        var giaVonCoSo = tonKho.GiaVonBinhQuan;
                        var thanhTienVon = soLuongLayTuKhoNay * giaVonCoSo;

                        // Tạo chi tiết phiếu bán
                        var chiTietPhieuBan = new CTPhieuBan
                        {
                            MaCTPB = Guid.NewGuid(),
                            MaKho = tonKho.MaKho,
                            MaLo = maLo,
                            SoLuong = tongSoLuongCanBan,
                            DonGia = chiTietRequest.DonGia,
                            DonViBan = chiTietRequest.DonViBan,
                            SoLuongQuyDoi = soLuongLayTuKhoNay,
                            GiaVonCoSo = giaVonCoSo,
                            ThanhTienVon = thanhTienVon,
                            GhiChu = chiTietRequest.GhiChu
                        };

                        chiTietPhieuBanList.Add(chiTietPhieuBan);
                        
                        //Chỉ cộng tiền lần đầu (khi lấy từ kho đầu tiên)
                        if (chiTietPhieuBanList.Count == 1 || 
                            (chiTietPhieuBanList.Count > 1 && chiTietPhieuBanList[^2].MaLo != maLo))
                        {
                            tongTienHang += thanhTienSanPham;
                        }
                        
                        soLuongConLai -= soLuongLayTuKhoNay;

                        // Trừ tồn kho (trừ theo số lượng đơn vị cơ sở)
                        tonKho.SoLuongCoSo -= soLuongLayTuKhoNay;
                    }
                }

                // Tính chiết khấu và thành tiền
                var tienChietKhau = tongTienHang * (request.ChietKhauPhanTram / 100);
                var thanhTien = tongTienHang - tienChietKhau;
                var tienNo = thanhTien - request.TienCoc;

                // Xác định trạng thái thanh toán
                var trangThaiThanhToan = request.HinhThucThanhToan == HinhThucThanhToanEnum.CONG_NO 
                    ? TrangThaiThanhToanEnum.CHUA_THANH_TOAN 
                    : TrangThaiThanhToanEnum.DA_THANH_TOAN;

                // Tạo phiếu bán
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

                // ADD PHIẾU BÁN TRƯỚC
                _context.PhieuBans.Add(phieuBan);
                await _context.SaveChangesAsync();

                // Thêm chi tiết vào phiếu (SAU khi phiếu đã có MaPB)
                foreach (var chiTiet in chiTietPhieuBanList)
                {
                    chiTiet.MaPB = phieuBan.MaPB;
                }

                _context.CTPhieuBans.AddRange(chiTietPhieuBanList);

                // Nếu thanh toán bằng công nợ, tạo record công nợ
                if (request.HinhThucThanhToan == HinhThucThanhToanEnum.CHUYEN_KHOAN || request.HinhThucThanhToan == HinhThucThanhToanEnum.CONG_NO)
                {
                    if (tienNo > 0)
                    {
                        var congNo = new CongNo
                        {
                            MaCongNo = Guid.NewGuid(),
                            LoaiDoiTuong = LoaiDoiTuongCongNoEnum.KHACH_HANG,
                            MaDoiTuong = request.MaKH,
                            MaPhieu = phieuBan.MaPB,
                            SoTien = tienNo,
                            NgayPhatSinh = DateTime.Now,
                            HanThanhToan = request.HanTra,
                            GhiChu = $"Công nợ từ phiếu bán {phieuBan.MaPBCode}"
                        };

                        _context.CongNos.Add(congNo);

                        //Cập nhật công nợ hiện tại của khách hàng
                        khachHang.CongNoHienTai += tienNo;
                    }
                }

                //Cập nhật tổng mua của khách hàng (dù thanh toán bằng cách nào)
                khachHang.TongMua += thanhTien;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Trả về chi tiết phiếu bán
                return await GetChiTietPhieuBanAsync(phieuBan.MaPB) ?? throw new Exception("Lỗi khi tạo phiếu bán!");
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
                // Lấy phiếu bán + chi tiết + khách hàng
                var phieuBan = await _context.PhieuBans
                    .Include(pb => pb.KhachHang)
                    .Include(pb => pb.CTPhieuBans)
                    .FirstOrDefaultAsync(pb => pb.MaPB == maPB);

                if (phieuBan == null)
                    throw new InvalidOperationException("Phiếu bán không tồn tại.");

                // Kiểm tra phiếu trả hàng liên quan
                var hasReturns = await _context.PhieuTras
                    .AnyAsync(pt => pt.MaPB == maPB);
                if (hasReturns)
                    throw new InvalidOperationException("Phiếu bán đã có phiếu trả liên quan, không thể xóa.");

                //  Rollback tồn kho
                foreach (var ct in phieuBan.CTPhieuBans!)
                {
                    var tonKho = await _context.TonKhos
                        .FirstOrDefaultAsync(tk => tk.MaKho == ct.MaKho && tk.MaLo == ct.MaLo);

                    if (tonKho == null)
                    {
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

                    tonKho.SoLuongCoSo += ct.SoLuongQuyDoi; // cộng trả lại số lượng đã xuất
                }

                // rollback cong no va tong mua cua khach hang
                if (phieuBan.HinhThucThanhToan == HinhThucThanhToanEnum.CONG_NO)
                {
                    // lay cong no
                    var congNos = await _context.CongNos
                        .Where(cn => cn.MaPhieu == phieuBan.MaPB
                                  && cn.LoaiDoiTuong == LoaiDoiTuongCongNoEnum.KHACH_HANG)
                        .ToListAsync();

                    // Tổng số tiền công nợ cần rollback
                    var totalDebtRollback = congNos.Sum(cn => cn.SoTien);

                    // Rollback công nợ hiện tại của khách hàng
                    phieuBan.KhachHang!.CongNoHienTai -= totalDebtRollback;
                    if (phieuBan.KhachHang!.CongNoHienTai < 0)
                        phieuBan.KhachHang!.CongNoHienTai = 0;

                    // Xóa record công nợ của phiếu bán trong bảng CongNos
                    if (congNos.Count > 0)
                        _context.CongNos.RemoveRange(congNos);
                }

                // Rollback tổng mua của khách hàng
                phieuBan.KhachHang!.TongMua -= phieuBan.ThanhTien;
                if (phieuBan.KhachHang!.TongMua < 0)
                    phieuBan.KhachHang!.TongMua = 0;


                // Xóa chi tiết và phiếu bán
                _context.CTPhieuBans.RemoveRange(phieuBan.CTPhieuBans);
                _context.PhieuBans.Remove(phieuBan);

                // Lưu thay đổi
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true; // ✅ báo thành công
            }
            catch
            {
                await transaction.RollbackAsync();
                return false; // ✅ báo thất bại
            }
        }


    }
}
