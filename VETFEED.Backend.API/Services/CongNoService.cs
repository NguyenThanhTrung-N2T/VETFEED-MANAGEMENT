using VETFEED.Backend.API.DTOs.CongNo;
using VETFEED.Backend.API.Repositories;
namespace VETFEED.Backend.API.Services
{
    public class CongNoService : ICongNoService
    {
        private readonly ICongNoRepository _congNoRepository;
        public CongNoService(ICongNoRepository congNoRepository)
        {
            _congNoRepository = congNoRepository;
        }

        // lay cong no tong hop cua tat ca doi tuong
        public async Task<List<CongNoTongHopResponse>> GetTongHopCongNoAsync()
        {
            return await _congNoRepository.GetTongHopCongNoAsync();
        }
    }
}
