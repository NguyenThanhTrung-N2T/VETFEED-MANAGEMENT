using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.QuyDoiDonVi;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)] 
    public class QuyDoiDonVisController : ControllerBase
    {
        private readonly IQuyDoiDonViService _service;

        public QuyDoiDonVisController(IQuyDoiDonViService service)
        {
            _service = service;
        }

        /// <summary>Danh sách cấu hình quy đổi của 1 sản phẩm</summary>
        [HttpGet("by-product/{maSP:guid}")]
        public async Task<IActionResult> GetByProduct(Guid maSP)
        {
            try
            {
                var result = await _service.GetByMaSPAsync(maSP);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Thêm 1 cấu hình quy đổi cho sản phẩm</summary>
        [HttpPost("by-product/{maSP:guid}")]
        public async Task<IActionResult> Create(Guid maSP, [FromBody] QuyDoiDonViCreateRequest request)
        {
            try
            {
                var created = await _service.CreateAsync(maSP, request);
                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Cập nhật 1 cấu hình quy đổi</summary>
        [HttpPut("{maQD:guid}")]
        public async Task<IActionResult> Update(Guid maQD, [FromBody] QuyDoiDonViUpdateRequest request)
        {
            try
            {
                var updated = await _service.UpdateAsync(maQD, request);
                if (updated == null) return NotFound("Không tìm thấy cấu hình quy đổi.");
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Xoá cấu hình quy đổi</summary>
        [HttpDelete("{maQD:guid}")]
        public async Task<IActionResult> Delete(Guid maQD)
        {
            var ok = await _service.DeleteAsync(maQD);
            if (!ok) return NotFound("Không tìm thấy cấu hình quy đổi.");
            return Ok("Xoá quy đổi đơn vị thành công.");
        }

        /// <summary>Lấy danh sách đơn vị nhập + đơn vị cơ sở của sản phẩm</summary>
        [HttpGet("units/{maSP:guid}")]
        public async Task<IActionResult> GetUnits(Guid maSP)
        {
            try
            {
                var result = await _service.GetUnitsForProductAsync(maSP);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
