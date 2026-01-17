using VETFEED.Backend.API.DTOs.BaoCao;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class BaoCaoService : IBaoCaoService
    {
        private readonly IBaoCaoRepository _baoCaoRepository;

        public BaoCaoService(IBaoCaoRepository baoCaoRepository)
        {
            _baoCaoRepository = baoCaoRepository;
        }

        // doanh thu từ ngày from đến ngày to
        public async Task<DoanhThuPhanTichResponse> GetDoanhThuPhanTichAsync(DateTime from, DateTime to)
        {
            return await _baoCaoRepository.GetDoanhThuPhanTichAsync(from, to);
        }
    }
}
