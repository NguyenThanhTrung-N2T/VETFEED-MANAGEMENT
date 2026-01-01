using VETFEED.Backend.API.DTOs.KhoHang;

namespace VETFEED.Backend.API.Services
{
    public interface IKhoHangService
    {
        // lấy tất cả kho hàng 
        Task<IEnumerable<KhoHangResponse>?> GetAllKhoHangsAsync();
    }
}
