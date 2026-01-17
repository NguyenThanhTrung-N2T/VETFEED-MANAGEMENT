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

        //Lấy phân tích doanh thu theo khoảng thời gian
        public async Task<DoanhThuPhanTichResponse> GetDoanhThuPhanTichAsync(DateTime from, DateTime to)
        {
            return await _baoCaoRepository.GetDoanhThuPhanTichAsync(from, to);
        }

        // Lấy danh sách chi tiết đơn hàng doanh thu với phân trang
        public async Task<DoanhThuDonHangResponse> GetDoanhThuDonHangAsync(DateTime from, DateTime to, int page, int limit)
        {
            return await _baoCaoRepository.GetDoanhThuDonHangAsync(from, to, page, limit);
        }

        // Lấy phân tích lợi nhuận theo khoảng thời gian
        public async Task<LoiNhuanPhanTichResponse> GetLoiNhuanPhanTichAsync(DateTime from, DateTime to)
        {
            return await _baoCaoRepository.GetLoiNhuanPhanTichAsync(from, to);
        }

        // Lấy danh sách lợi nhuận theo sản phẩm với phân trang và sắp xếp
        public async Task<LoiNhuanSanPhamResponse> GetLoiNhuanSanPhamAsync(DateTime from, DateTime to, int page, int limit, string sortBy, string order)
        {
            return await _baoCaoRepository.GetLoiNhuanSanPhamAsync(from, to, page, limit, sortBy, order);
        }
    }
}
