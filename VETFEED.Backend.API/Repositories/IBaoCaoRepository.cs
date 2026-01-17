using VETFEED.Backend.API.DTOs.BaoCao;

namespace VETFEED.Backend.API.Repositories
{
    public interface IBaoCaoRepository
    {
        // doanh thu từ ngày from đến ngày to
        Task<DoanhThuPhanTichResponse> GetDoanhThuPhanTichAsync(DateTime from, DateTime to);
    }
}
