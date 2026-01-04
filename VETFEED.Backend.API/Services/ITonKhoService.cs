using VETFEED.Backend.API.DTOs.TonKho;

namespace VETFEED.Backend.API.Services
{
    public interface ITonKhoService
    {
        // lay danh sach ton kho theo cac kho
        Task<IEnumerable<TonKhoResponse>> GetListTonKhoByKhoAsync();

        // cap nhat so luong ton kho 
        Task<bool> UpdateTonKhoAsync(Guid maKho, Guid maLo, decimal soLuong);
    }
}
