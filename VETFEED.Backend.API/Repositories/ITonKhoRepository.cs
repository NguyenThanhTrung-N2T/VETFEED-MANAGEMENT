using VETFEED.Backend.API.DTOs.TonKho;

namespace VETFEED.Backend.API.Repositories
{
    public interface ITonKhoRepository
    {
        // lay danh sanh ton kho trong tung kho
        Task<IEnumerable<TonKhoResponse>> GetListTonKhoByKhoAsync();

        // cap nhat so luong ton kho 
        Task<bool> UpdateTonKhoAsync(Guid maKho, Guid maLo, decimal soLuong);

        // tao moi ban ghi ton kho
        Task<bool> AddTonKhoAsync(Guid maKho, Guid maLo, decimal soLuong);

        // them hoac cap nhat ton kho (neu ton tai thi cong them so luong)
        Task<bool> AddOrUpdateTonKhoAsync(Guid maKho, Guid maLo, decimal soLuong);
    }
}
