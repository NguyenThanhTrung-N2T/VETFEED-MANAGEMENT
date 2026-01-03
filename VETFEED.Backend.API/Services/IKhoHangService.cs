using VETFEED.Backend.API.DTOs.KhoHang;

namespace VETFEED.Backend.API.Services
{
    public interface IKhoHangService
    {
        // lấy tất cả kho hàng 
        Task<IEnumerable<KhoHangResponse>> GetAllKhoHangsAsync();

        // lấy kho hàng theo mã kho 
        Task<KhoHangResponse?> GetKhoHangByIdAsync(Guid maKho);

        // thêm kho hàng 
        Task<KhoHangResponse> AddKhoHangAsync(CreateKhoHangRequest dto);

        // cập nhật kho hàng 
        Task<KhoHangResponse?> UpdateKhoHangAsync(Guid maKho, UpdateKhoHangRequest dto);

        // Xóa kho hàng 
        Task<bool> DeleteKhoHangAsync(Guid maKho);

        // tìm kiếm kho hàng theo từ khóa và thuộc tính 
        Task<IEnumerable<KhoHangResponse>> SearchKhoHangAsync(SearchKhoHangRequest dto);
    }
}
