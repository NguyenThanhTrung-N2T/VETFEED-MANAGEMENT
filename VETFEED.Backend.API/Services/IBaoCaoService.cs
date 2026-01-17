using VETFEED.Backend.API.DTOs.BaoCao;

namespace VETFEED.Backend.API.Services
{
    public interface IBaoCaoService
    {
        // doanh thu từ ngày from đến ngày to
        Task<DoanhThuPhanTichResponse> GetDoanhThuPhanTichAsync(DateTime from, DateTime to);
    }
}
