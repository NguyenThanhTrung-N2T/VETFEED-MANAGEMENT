using VETFEED.Backend.API.DTOs.TonKho;

namespace VETFEED.Backend.API.Repositories
{
    public interface ITonKhoRepository
    {
        // lay danh sanh ton kho trong tung kho
        Task<IEnumerable<TonKhoResponse>> GetListTonKhoByKhoAsync();

        // cap nhat so luong ton kho 
        Task<bool> UpdateTonKhoAsync(Guid maKho, Guid maLo, decimal soLuong);
    }
}
