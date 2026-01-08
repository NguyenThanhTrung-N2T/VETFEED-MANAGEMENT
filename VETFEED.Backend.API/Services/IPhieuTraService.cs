using VETFEED.Backend.API.DTOs.PhieuTra;

namespace VETFEED.Backend.API.Services
{
    public interface IPhieuTraService
    {
        // lay danh sach phieu tra
        Task<IEnumerable<PhieuTraListResponse>> GetDanhSachPhieuTraAsync();


    }
}
