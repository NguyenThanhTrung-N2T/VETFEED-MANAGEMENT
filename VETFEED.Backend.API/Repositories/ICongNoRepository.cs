using VETFEED.Backend.API.DTOs.CongNo;

namespace VETFEED.Backend.API.Repositories
{
    public interface ICongNoRepository
    {
        // lay cong no tong hop cua tat ca doi tuong
        Task<List<CongNoTongHopResponse>> GetTongHopCongNoAsync();
    }
}
