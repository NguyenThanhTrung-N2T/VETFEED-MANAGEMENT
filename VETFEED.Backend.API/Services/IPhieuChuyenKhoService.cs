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
    }

}
