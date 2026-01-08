using VETFEED.Backend.API.DTOs.PhieuTra;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class PhieuTraService : IPhieuTraService
    {
        private readonly IPhieuTraRepository _phieuTraRepository;
        public PhieuTraService(IPhieuTraRepository phieuTraRepository)
        {
            _phieuTraRepository = phieuTraRepository;
        }

        // lay danh sach phieu tra
        public async Task<IEnumerable<PhieuTraListResponse>> GetDanhSachPhieuTraAsync() 
        { 
            return await _phieuTraRepository.GetDanhSachPhieuTraAsync(); 
        }
    }
}
