using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.BaoCao;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaoCaosController : ControllerBase
    {
        private readonly IBaoCaoService _baoCaoService;

        public BaoCaosController(IBaoCaoService baoCaoService)
        {
            _baoCaoService = baoCaoService;
        }


        //GET api/baocaos/doanhthu/phantich?from=2026-01-09&to=2026-01-16 : Lấy phân tích doanh thu theo khoảng thời gian
        [HttpGet("doanhthu/phantich")]
        [ProducesResponseType(typeof(DoanhThuPhanTichResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<DoanhThuPhanTichResponse>> GetDoanhThuPhanTich(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            try
            {
                var result = await _baoCaoService.GetDoanhThuPhanTichAsync(from, to);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //GET api/baocaos/doanhthu/donhang?from=2026-01-09&to=2026-01-16&page=1&limit=20 : Lấy danh sách chi tiết đơn hàng doanh thu với phân trang
        [HttpGet("doanhthu/donhang")]
        [ProducesResponseType(typeof(DoanhThuDonHangResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<DoanhThuDonHangResponse>> GetDoanhThuDonHang(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            try
            {
                // Validate pagination parameters
                if (page < 1) page = 1;
                if (limit < 1) limit = 20;
                if (limit > 100) limit = 100; // Max limit to prevent performance issues

                var result = await _baoCaoService.GetDoanhThuDonHangAsync(from, to, page, limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //GET api/baocaos/loinhuan/phantich?from=2026-01-09&to=2026-01-16 : Lấy phân tích lợi nhuận theo khoảng thời gian
        [HttpGet("loinhuan/phantich")]
        [ProducesResponseType(typeof(LoiNhuanPhanTichResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<LoiNhuanPhanTichResponse>> GetLoiNhuanPhanTich(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            try
            {
                var result = await _baoCaoService.GetLoiNhuanPhanTichAsync(from, to);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //GET api/baocaos/loinhuan/sanpham?from=2026-01-09&to=2026-01-16&page=1&limit=20&sort_by=profit&order=desc : Lấy danh sách lợi nhuận theo sản phẩm
        [HttpGet("loinhuan/sanpham")]
        [ProducesResponseType(typeof(LoiNhuanSanPhamResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<LoiNhuanSanPhamResponse>> GetLoiNhuanSanPham(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20,
            [FromQuery] string sort_by = "profit",
            [FromQuery] string order = "desc")
        {
            try
            {
                // Kiểm tra tham số phân trang
                if (page < 1) page = 1;
                if (limit < 1) limit = 20;
                if (limit > 100) limit = 100;

                // Kiểm tra tham số sắp xếp
                var validSortFields = new[] { "profit", "revenue", "quantity", "margin" };
                if (!validSortFields.Contains(sort_by.ToLower()))
                    sort_by = "profit";

                // Kiểm tra tham số thứ tự
                if (order.ToLower() != "asc" && order.ToLower() != "desc")
                    order = "desc";

                var result = await _baoCaoService.GetLoiNhuanSanPhamAsync(from, to, page, limit, sort_by, order);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //GET api/baocaos/tonkho/phantich?maKho={guid} : Lấy phân tích tồn kho theo kho
        [HttpGet("tonkho/phantich")]
        [ProducesResponseType(typeof(TonKhoPhanTichResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<TonKhoPhanTichResponse>> GetTonKhoPhanTich([FromQuery] Guid maKho)
        {
            try
            {
                var result = await _baoCaoService.GetTonKhoPhanTichAsync(maKho);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
