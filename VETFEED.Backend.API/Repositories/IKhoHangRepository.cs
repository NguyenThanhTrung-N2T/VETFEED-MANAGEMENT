using VETFEED.Backend.API.DTOs.KhoHang;

namespace VETFEED.Backend.API.Repositories
{
    public interface IKhoHangRepository
    {
        // lấy tất cả kho hàng
        Task<IEnumerable<KhoHangResponse>> GetAllKhoHangsAsync();

        // lấy kho hàng theo mã  kho 
        Task<KhoHangResponse?> GetKhoHangByIdAsync(Guid maKho);

        // Thêm kho hàng
        Task<KhoHangResponse> AddKhoHangAsync(CreateKhoHangRequest dto);

        // Tìm kho hàng theo mã kho
        Task<bool> IsKhoHangExist(Guid maKho);

        // Cập nhật thông tin kho hàng 
        Task<KhoHangResponse> UpdateKhoHangAsync(Guid maKho, UpdateKhoHangRequest dto);

        // Xóa kho hàng 
        Task<bool> DeleteKhoHangAsync(Guid maKho);

        // Tìm kiếm kho hàng theo thuộc tính và từ khóa 
        Task<IEnumerable<KhoHangResponse>> SearchKhoHangAsync(SearchKhoHangRequest dto);
    }
}
