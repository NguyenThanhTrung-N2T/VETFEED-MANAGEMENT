using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.DTOs.PhieuTra;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhieuTrasController : ControllerBase
    {
        private readonly IPhieuTraService _service;
        public PhieuTrasController(IPhieuTraService service)
        {
            _service = service;
        }

        // GET : api/phieutras : lay danh sach phieu tra
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PhieuTraListResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDanhSachPhieuTra()
        {
            // lay danh sach phieu tra 
            var result = await _service.GetDanhSachPhieuTraAsync();
            // tra ve client
            return Ok(result);
        }
    }
}
