using VETFEED.Backend.API.DTOs.TonKho;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class TonKhoService : ITonKhoService
    {
        private readonly ITonKhoRepository _tonKhoRepo;
        public TonKhoService(ITonKhoRepository repo)
        {
            _tonKhoRepo = repo;
        }

        // lay danh sach ton kho theo cac kho 
        public async Task<IEnumerable<TonKhoResponse>> GetListTonKhoByKhoAsync()
        {
            return await _tonKhoRepo.GetListTonKhoByKhoAsync();
        }

        // cap nhat so luong ton kho 
        public async Task<bool> UpdateTonKhoAsync(Guid maKho, Guid maLo, decimal soLuong)
        {
            return await _tonKhoRepo.UpdateTonKhoAsync(maKho, maLo, soLuong);
        }

    }
}
