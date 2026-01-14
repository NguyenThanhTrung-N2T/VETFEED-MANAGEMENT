using VETFEED.Backend.API.DTOs.CongNo;

namespace VETFEED.Backend.API.Services
{
    public interface ICongNoService
    {
        // lay cong no tong hop cua tat ca doi tuong
        Task<List<CongNoTongHopResponse>> GetTongHopCongNoAsync();

        // lay lịch su cong no cua doi tuong theo ma doi tuong
        Task<List<CongNoHistoryResponse>> GetCongNoHistoryAsync(Guid maDoiTuong);

        // tạo công nợ mới
        Task CreateCongNoAsync(CreateCongNoRequest request);
    }
}
