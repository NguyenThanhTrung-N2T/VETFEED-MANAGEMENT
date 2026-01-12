using VETFEED.Backend.API.DTOs.CongNo;

namespace VETFEED.Backend.API.Services
{
    public interface ICongNoService
    {
        // lay cong no tong hop cua tat ca doi tuong
        Task<List<CongNoTongHopResponse>> GetTongHopCongNoAsync();
    }
}
