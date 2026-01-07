using VETFEED.Backend.API.DTOs.CTChuyenKho;
using VETFEED.Backend.API.DTOs.PhieuChuyenKho;

namespace VETFEED.Backend.API.Services
{
    public interface IPhieuChuyenKhoService
    {
        // lay danh sach phieu chuyen kho 
        Task<IEnumerable<PhieuChuyenKhoResponse>> GetDanhSachPhieuChuyenKhoAsync();

        // lay chi tiet phieu chuyen kho
        Task<ChiTietPhieuChuyenKhoResponse?> GetChiTietPhieuChuyenKhoAsync(Guid maCK);

        // them phieu chuyen kho
        Task<ChiTietPhieuChuyenKhoResponse> AddPhieuChuyenKhoAsync(PhieuChuyenKhoRequest request);

        // cap nhat phieu chuyen kho 
        Task<ChiTietPhieuChuyenKhoResponse?> UpdatePhieuChuyenKhoAsync(UpdatePhieuChuyenKhoRequest request);

        // cap nhat trang thai chuyen kho
        Task<ChiTietPhieuChuyenKhoResponse?> UpdateTrangThaiChiTietAsync(Guid maCTCK, UpdateTrangThaiCTChuyenKho request);

        // xoa phieu chuyen kho 
        Task<bool> XoaPhieuChuyenKhoAsync(Guid maCK);
    }

}
