using VETFEED.Backend.API.DTOs.TonKho;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class TonKhoService : ITonKhoService
    {
        private readonly ITonKhoRepository _tonKhoRepo;
        private readonly ILoHangRepository _loHangRepo;
        public TonKhoService(ITonKhoRepository repo, ILoHangRepository loHangRepo)
        {
            _tonKhoRepo = repo;
            _loHangRepo = loHangRepo;
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

        // kiem tra ton kho du so luong 
        public async Task<bool> IsTonKhoEnough(Guid MaKho, Guid MaLo, decimal SoLuongChuyen)
        {
            // kiem tra lo hang 
            var iExist = await _loHangRepo.IsLoHangExist(MaLo);
            if (!iExist)
            {
                throw new Exception("Lô hàng không tồn tại !");
            }

            // kiem tra ton kho 
            var isTonKhoExist = await _tonKhoRepo.IsExistTonKho(MaKho, MaLo);
            if (!isTonKhoExist)
            {
                throw new Exception("Trong kho không lưu trữ lô hàng này !");
            }

            // kiem tra du ton kho khoong
            var isEnough = await _tonKhoRepo.IsTonKhoEnough(MaKho, MaLo, SoLuongChuyen);
            if (isEnough)
            {
                return true;
            }
            return false;
        }

        // kiem tra ton kho theo lo tai tat ca cac kho
        public async Task<bool> IsTonKhoEnoughAllKhoAsync(Guid maLo, decimal soLuongCan)
        {
            return await _tonKhoRepo.KiemTraTonKhoTheoLoAsync(maLo, soLuongCan);
        }
    }
}
