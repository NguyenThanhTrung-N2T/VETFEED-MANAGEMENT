using VETFEED.Backend.API.DTOs.PhieuChuyenKho;

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
    }
}
