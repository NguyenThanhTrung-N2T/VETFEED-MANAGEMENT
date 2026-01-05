using VETFEED.Backend.API.DTOs.CTChuyenKho;
using VETFEED.Backend.API.DTOs.PhieuChuyenKho;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.Repositories
{
    public interface IPhieuChuyenKhoRepository
    {
        // lay danh sach phieu chuyen kho 
        Task<IEnumerable<PhieuChuyenKhoResponse>> GetDanhSachPhieuChuyenKhoAsync();

        // lay chi tiet phieu chuyen kho 
        Task<ChiTietPhieuChuyenKhoResponse?> GetChiTietPhieuChuyenKhoAsync(Guid maCK);

        // tao phieu chuyen kho 
        Task<ChiTietPhieuChuyenKhoResponse> AddPhieuChuyenKhoAsync(PhieuChuyenKhoRequest request);

        // update phieu chuyen kho
        Task<ChiTietPhieuChuyenKhoResponse?> UpdatePhieuChuyenKhoAsync(UpdatePhieuChuyenKhoRequest request);

        // cap nhat trang thai chi tiet chuyen kho
        Task<ChiTietPhieuChuyenKhoResponse?> UpdateTrangThaiChiTietAsync(Guid maCTCK, UpdateTrangThaiCTChuyenKho request);

        // xoa phieu chuyen kho
        Task<bool> XoaPhieuChuyenKhoAsync(Guid maCK);
    }
}
