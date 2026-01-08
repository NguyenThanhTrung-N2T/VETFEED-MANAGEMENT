using VETFEED.Backend.API.DTOs.TonKho;

namespace VETFEED.Backend.API.Services
{
    public interface ITonKhoService
    {
        // lay danh sach ton kho theo cac kho
        Task<IEnumerable<TonKhoResponse>> GetListTonKhoByKhoAsync();

        // cap nhat so luong ton kho 
        Task<bool> UpdateTonKhoAsync(Guid maKho, Guid maLo, decimal soLuong);

        // kiem tra ton kho du so luong 
        Task<bool> IsTonKhoEnough(Guid MaKho, Guid MaLo, decimal SoLuongChuyen);

        // kiem tra ton kho theo lo o tat ca cac kho
        Task<bool> IsTonKhoEnoughAllKhoAsync(Guid maLo, decimal soLuongCan , string DonViTinh);
    }
}
