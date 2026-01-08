using VETFEED.Backend.API.DTOs.PhieuTra;

namespace VETFEED.Backend.API.Repositories
{
    public interface IPhieuTraRepository
    {
        // lay danh sach phieu tra
        Task<IEnumerable<PhieuTraListResponse>> GetDanhSachPhieuTraAsync();


    }
}
