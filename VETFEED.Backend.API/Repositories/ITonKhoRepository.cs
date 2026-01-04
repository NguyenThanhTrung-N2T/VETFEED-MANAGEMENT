using VETFEED.Backend.API.DTOs.TonKho;

namespace VETFEED.Backend.API.Repositories
{
    public interface ITonKhoRepository
    {
        // lay danh sanh ton kho trong tung kho
        Task<IEnumerable<TonKhoResponse>> GetListTonKhoByKhoAsync();

        // cap nhat so luong ton kho 
        Task<bool> UpdateTonKhoAsync(Guid maKho, Guid maLo, decimal soLuong);

        // kiem tra ton kho 
        Task<bool> IsExistTonKho(Guid MaKho, Guid MaLo);

        // Kiem tra ton kho co du so luong hay khong
        Task<bool> IsTonKhoEnough(Guid MaKho, Guid MaLo, decimal SoLuongChuyen);

        
    }
}
