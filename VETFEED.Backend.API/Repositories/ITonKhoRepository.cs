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

        // lay ton kho theo ma kho va ma lo 
        Task<TonKhoChiTietResponse?> GetTonKhoAsync(Guid MaKho, Guid MaLo);

        // them ton kho 
        Task<TonKhoChiTietResponse> AddTonKhoAsync(Guid MaKho, Guid MaLo, decimal soLuong);

        // tang ton kho 
        Task<bool> IncreaseTonKhoAsync(Guid MaKho, Guid MaLo, decimal soLuong);

        // giam ton kho 
        Task<bool> DecreaseTonKhoAsync(Guid MaKho, Guid MaLo, decimal soLuong);

        // kiem tra ton kho theo lo tai cac kho
        Task<bool> IsTonKhoEnoughAllKhoAsync(Guid maLo, decimal soLuongCan);



        // tao moi ban ghi ton kho
        //Task<bool> AddTonKhoAsync(Guid maKho, Guid maLo, decimal soLuong);

        // them hoac cap nhat ton kho (neu ton tai thi cong them so luong)
        Task<bool> AddOrUpdateTonKhoAsync(Guid maKho, Guid maLo, decimal soLuong);
    }
}
