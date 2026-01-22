using VETFEED.Backend.API.DTOs.CongNo;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Repositories;
namespace VETFEED.Backend.API.Services
{
    public class CongNoService : ICongNoService
    {
        private readonly ICongNoRepository _congNoRepository;
        public CongNoService(ICongNoRepository congNoRepository)
        {
            _congNoRepository = congNoRepository;
        }

        // lay cong no tong hop cua tat ca doi tuong
        public async Task<List<CongNoTongHopResponse>> GetTongHopCongNoAsync()
        {
            return await _congNoRepository.GetTongHopCongNoAsync();
        }

        // lay lịch su cong no cua doi tuong theo ma doi tuong
        public async Task<List<CongNoHistoryResponse>> GetCongNoHistoryAsync(Guid maDoiTuong)
        {
            var raws = await _congNoRepository.GetCongNoHistoryAsync(maDoiTuong);

            var result = new List<CongNoHistoryResponse>();
            decimal soDu = 0;

            foreach (var r in raws)
            {
                decimal phatSinhNo = 0;
                decimal daThanhToan = 0;

                if (r.SoTien > 0)
                {
                    phatSinhNo = r.SoTien;
                    soDu += r.SoTien;
                }
                else
                {
                    daThanhToan = Math.Abs(r.SoTien);
                    soDu -= daThanhToan;
                }

                result.Add(new CongNoHistoryResponse
                {
                    Ngay = r.NgayPhatSinh,
                    LoaiPhieu = r.LoaiPhieu,
                    MaPhieu = r.MaPhieuCode ?? "",
                    GhiChu = r.GhiChu ?? "",
                    PhatSinhNo = phatSinhNo,
                    DaThanhToan = daThanhToan,
                    SoDuSau = soDu
                });
            }

            return result;
        }

        // tạo công nợ mới 
        public async Task CreateCongNoAsync(CreateCongNoRequest request)
        {
            if (request.SoTien == 0)
                throw new Exception("Số tiền không hợp lệ");

            bool isTangNo = request.SoTien > 0;

            if (isTangNo && request.HanThanhToan == null)
                throw new Exception("Tăng công nợ bắt buộc có hạn thanh toán");

            // XÁC ĐỊNH ĐỐI TƯỢNG
            var khachHang = await _congNoRepository.GetKhachHangByIdAsync(request.MaDoiTuong);
            var nhaCungCap = await _congNoRepository.GetNhaCungCapByIdAsync(request.MaDoiTuong);

            bool isKhachHang = khachHang != null;
            bool isNhaCungCap = nhaCungCap != null;

            if (!isKhachHang && !isNhaCungCap)
                throw new Exception("Đối tượng không tồn tại");

            // TĂNG CÔNG NỢ
            if (isTangNo)
            {
                //check hạn mức (CHỈ KH)
                if (isKhachHang)
                {
                    var congNoSauTang = khachHang!.CongNoHienTai + request.SoTien;
                    if (congNoSauTang > khachHang.HanMucCongNo)
                        throw new Exception("Vượt hạn mức công nợ");

                    khachHang.CongNoHienTai = congNoSauTang;
                }

                Guid? maPhieu = null;

                if (!string.IsNullOrEmpty(request.MaPhieuCode))
                {
                    if (isKhachHang)
                    {
                        var phieuBan = await _congNoRepository.GetPhieuBanByCodeAsync(request.MaPhieuCode);
                        if (phieuBan == null)
                            throw new Exception("Phiếu bán không tồn tại");

                        maPhieu = phieuBan.MaPB;
                    }
                    else // NCC
                    {
                        var phieuNhap = await _congNoRepository.GetPhieuNhapByCodeAsync(request.MaPhieuCode);
                        if (phieuNhap == null)
                            throw new Exception("Phiếu nhập không tồn tại");

                        maPhieu = phieuNhap.MaPN;
                    }
                }

                await _congNoRepository.AddCongNoAsync(new CongNo
                {
                    MaCongNo = Guid.NewGuid(),
                    MaDoiTuong = request.MaDoiTuong,
                    MaPhieu = maPhieu,
                    LoaiDoiTuong = isKhachHang ? LoaiDoiTuongCongNoEnum.KHACH_HANG : LoaiDoiTuongCongNoEnum.NHA_CUNG_CAP,
                    SoTien = request.SoTien,
                    NgayPhatSinh = request.NgayPhatSinh,
                    HanThanhToan = request.HanThanhToan,
                    GhiChu = request.GhiChu
                });
            }

            //GIẢM CÔNG NỢ
            else
            {
                decimal soTienCanTra = Math.Abs(request.SoTien);
                decimal tongThucTeDaTra = 0;

                // Trả theo phiếu
                if (!string.IsNullOrEmpty(request.MaPhieuCode))
                {
                    Guid maPhieu;

                    if (isKhachHang)
                    {
                        var phieuBan = await _congNoRepository.GetPhieuBanByCodeAsync(request.MaPhieuCode);
                        if (phieuBan == null)
                            throw new Exception("Phiếu bán không tồn tại");

                        maPhieu = phieuBan.MaPB;

                        var noConLai = await _congNoRepository.GetTongCongNoTheoPhieuAsync(maPhieu);
                        if (soTienCanTra > noConLai)
                            throw new Exception("Số tiền trả vượt quá số nợ của phiếu");

                        // Lưu giao dịch thanh toán TRƯỚC
                        await _congNoRepository.AddCongNoAsync(new CongNo
                        {
                            MaCongNo = Guid.NewGuid(),
                            MaDoiTuong = request.MaDoiTuong,
                            MaPhieu = maPhieu,
                            SoTien = -soTienCanTra,
                            LoaiDoiTuong = LoaiDoiTuongCongNoEnum.KHACH_HANG,
                            NgayPhatSinh = request.NgayPhatSinh,
                            GhiChu = request.GhiChu
                        });

                        // SAU ĐÓ tính lại nợ còn lại
                        var noConLaiSauKhiTra = await _congNoRepository.GetTongCongNoTheoPhieuAsync(maPhieu);
                        
                        // Nếu đã trả hết nợ thì cập nhật trạng thái
                        if (noConLaiSauKhiTra <= 0.01m && phieuBan.TrangThaiThanhToan == TrangThaiThanhToanEnum.CHUA_THANH_TOAN)
                        {
                            phieuBan.TrangThaiThanhToan = TrangThaiThanhToanEnum.DA_THANH_TOAN;
                        }
                    }
                    else // NCC
                    {
                        var phieuNhap = await _congNoRepository.GetPhieuNhapByCodeAsync(request.MaPhieuCode);
                        if (phieuNhap == null)
                            throw new Exception("Phiếu nhập không tồn tại");

                        maPhieu = phieuNhap.MaPN;

                        var noConLai = await _congNoRepository.GetTongCongNoTheoPhieuAsync(maPhieu);
                        if (soTienCanTra > noConLai)
                            throw new Exception("Số tiền trả vượt quá số nợ của phiếu");

                        // Lưu giao dịch thanh toán
                        await _congNoRepository.AddCongNoAsync(new CongNo
                        {
                            MaCongNo = Guid.NewGuid(),
                            MaDoiTuong = request.MaDoiTuong,
                            MaPhieu = maPhieu,
                            SoTien = -soTienCanTra,
                            LoaiDoiTuong = LoaiDoiTuongCongNoEnum.NHA_CUNG_CAP,
                            NgayPhatSinh = request.NgayPhatSinh,
                            GhiChu = request.GhiChu
                        });
                    }

                    tongThucTeDaTra = soTienCanTra;
                }
                // Trả không theo phiếu
                else
                {
                    var congNoList = await _congNoRepository.GetCongNoChuaTatToanAsync(request.MaDoiTuong);

                    var tongNoConLai = congNoList.Sum(x => x.SoTien);
                    if (soTienCanTra > tongNoConLai)
                        throw new Exception("Số tiền trả vượt quá tổng công nợ");

                    foreach (var cn in congNoList)
                    {
                        if (soTienCanTra <= 0) break;

                        var soTru = Math.Min(cn.SoTien, soTienCanTra);

                        await _congNoRepository.AddCongNoAsync(new CongNo
                        {
                            MaCongNo = Guid.NewGuid(),
                            MaDoiTuong = request.MaDoiTuong,
                            MaPhieu = cn.MaPhieu,
                            SoTien = -soTru,
                            LoaiDoiTuong = isKhachHang ? LoaiDoiTuongCongNoEnum.KHACH_HANG : LoaiDoiTuongCongNoEnum.NHA_CUNG_CAP,
                            NgayPhatSinh = request.NgayPhatSinh,
                            GhiChu = request.GhiChu
                        });

                        soTienCanTra -= soTru;
                        tongThucTeDaTra += soTru;
                        
                        // KIỂM TRA VÀ CẬP NHẬT TRẠNG THÁI PHIẾU NẾU ĐÃ TRẢ HẾT
                        if (cn.MaPhieu.HasValue)
                        {
                            // Tính lại tổng công nợ của phiếu SAU KHI đã thêm bản ghi thanh toán
                            var noConLaiCuaPhieu = await _congNoRepository.GetTongCongNoTheoPhieuAsync(cn.MaPhieu.Value);
                            
                            // Nếu đã trả hết nợ của phiếu này (nợ còn lại <= 0)
                            if (noConLaiCuaPhieu <= 0.01m) // Dùng epsilon để tránh lỗi làm tròn
                            {
                                if (isKhachHang)
                                {
                                    var phieuBan = await _congNoRepository.GetPhieuBanByIdAsync(cn.MaPhieu.Value);
                                    // Chỉ cập nhật nếu phiếu ban đầu thanh toán bằng công nợ
                                    if (phieuBan != null && phieuBan.TrangThaiThanhToan == TrangThaiThanhToanEnum.CHUA_THANH_TOAN)
                                    {
                                        phieuBan.TrangThaiThanhToan = TrangThaiThanhToanEnum.DA_THANH_TOAN;
                                    }
                                }
                            }
                        }
                    }
                }

                // cập nhật công nợ hiện tại (CHỈ KH)
                if (isKhachHang && tongThucTeDaTra > 0)
                {
                    khachHang!.CongNoHienTai -= tongThucTeDaTra;
                    if (khachHang.CongNoHienTai < 0)
                        khachHang.CongNoHienTai = 0;
                }
            }

            await _congNoRepository.SaveChangesAsync();
        }



    }
}
