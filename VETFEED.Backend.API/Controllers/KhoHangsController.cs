using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VETFEED.Backend.API.Services;

namespace VETFEED.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhoHangsController : ControllerBase
    {
        private readonly IKhoHangService _khoHangService;
        public KhoHangsController(IKhoHangService khoHangService)
        {
            _khoHangService = khoHangService;
        }

        // GET: api/khohangs  : lấy tất cả kho hàng 
        [HttpGet]
        public async Task<IActionResult> GetAllKhoHangsAsync()
        {
            // lấy tất cả kho hàng từ service
            var khoHangs = await _khoHangService.GetAllKhoHangsAsync();
            // trả về client 
            return Ok(khoHangs);
        }
    }
}
