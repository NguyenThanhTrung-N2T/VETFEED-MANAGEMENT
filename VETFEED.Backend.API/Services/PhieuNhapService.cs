using VETFEED.Backend.API.DTOs.CTPhieuNhap;
using VETFEED.Backend.API.DTOs.LoHang;
using VETFEED.Backend.API.DTOs.PhieuNhap;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class PhieuNhapService : IPhieuNhapService
    {
        private readonly IPhieuNhapRepository _phieuNhapRepo;
        private readonly ICTPhieuNhapRepository _ctPhieuNhapRepo;
        private readonly ILoHangRepository _loHangRepo;

        public PhieuNhapService(
            IPhieuNhapRepository phieuNhapRepo,
            ICTPhieuNhapRepository ctPhieuNhapRepo,
            ILoHangRepository loHangRepo)
        {
            _phieuNhapRepo = phieuNhapRepo;
            _ctPhieuNhapRepo = ctPhieuNhapRepo;
            _loHangRepo = loHangRepo;
        }

        // Lấy tất cả phiếu nhập
        public async Task<IEnumerable<PhieuNhapResponse>> GetAllPhieuNhapsAsync()
        {
            return await _phieuNhapRepo.GetAllPhieuNhapsAsync();
        }

        // Lấy phiếu nhập theo ID (bao gồm chi tiết)
        public async Task<PhieuNhapDetailedResponse?> GetPhieuNhapByIdAsync(Guid id)
        {
            return await _phieuNhapRepo.GetPhieuNhapByIdAsync(id);
        }

        // Tạo phiếu nhập mới với danh sách chi tiết
        public async Task<PhieuNhapDetailedResponse> CreatePhieuNhapAsync(PhieuNhapCreateRequest request)
        {
            // Validation
            if (request.MaNCC == Guid.Empty)
                throw new ArgumentException("Mã nhà cung cấp không hợp lệ.");
            if (request.MaKho == Guid.Empty)
                throw new ArgumentException("Mã kho không hợp lệ.");

            // 1. Tạo phiếu nhập với trạng thái DA_DAT
            var phieuNhapRequest = new PhieuNhapRequest
            {
                MaNCC = request.MaNCC,
                MaKho = request.MaKho,
                TrangThai = TrangThaiPhieuNhapEnum.DA_DAT.ToString(),
                GhiChu = request.GhiChu
            };
            var phieuNhap = await _phieuNhapRepo.AddPhieuNhapAsync(phieuNhapRequest);

            // 2. Tạo từng chi tiết (lô hàng + CTPN)
            var danhSachChiTiet = new List<CTPhieuNhapResponse>();
            if (request.DanhSachChiTiet != null && request.DanhSachChiTiet.Any())
            {
                foreach (var chiTiet in request.DanhSachChiTiet)
                {
                    // Tạo lô hàng mới
                    var loHangRequest = new LoHangRequest
                    {
                        MaSP = chiTiet.MaSP,
                        NgaySanXuat = chiTiet.NgaySanXuat,
                        HanSuDung = chiTiet.HanSuDung
                    };
                    var loHang = await _loHangRepo.AddLoHangAsync(loHangRequest);

                    // Tạo chi tiết phiếu nhập (DonGia = 0 mặc định)
                    var ctEntity = await _ctPhieuNhapRepo.AddCTPhieuNhapAsync(
                        phieuNhap.MaPN,
                        loHang.MaLo,
                        chiTiet.SoLuong,
                        (chiTiet.DonGia != null) ? chiTiet.DonGia.Value : 0
                    );

                    danhSachChiTiet.Add(new CTPhieuNhapResponse
                    {
                        MaCTPN = ctEntity.MaCTPN,
                        MaPN = ctEntity.MaPN,
                        MaLo = ctEntity.MaLo,
                        MaLoCode = loHang.MaLoCode,
                        TenSP = loHang.TenSP,
                        NgaySanXuat = loHang.NgaySanXuat,
                        HanSuDung = loHang.HanSuDung,
                        SoLuong = ctEntity.SoLuong,
                        DonGia = ctEntity.DonGia
                    });
                }
            }

            // 3. Trả về response chi tiết
            return new PhieuNhapDetailedResponse
            {
                MaPN = phieuNhap.MaPN,
                MaPNCode = phieuNhap.MaPNCode,
                MaNCC = phieuNhap.MaNCC,
                TenNCC = phieuNhap.TenNCC,
                MaKho = phieuNhap.MaKho,
                TenKho = phieuNhap.TenKho,
                ThanhTien = 0, // DonGia = 0 nên ThanhTien = 0
                TrangThai = phieuNhap.TrangThai,
                GhiChu = phieuNhap.GhiChu,
                NgayCapNhat = phieuNhap.NgayCapNhat,
                DanhSachChiTiet = danhSachChiTiet
            };
        }
    }
}
