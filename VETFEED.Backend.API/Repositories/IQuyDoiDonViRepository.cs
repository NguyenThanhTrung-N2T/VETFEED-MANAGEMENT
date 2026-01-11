using VETFEED.Backend.API.Models;

namespace VETFEED.Backend.API.Repositories
{
    public interface IQuyDoiDonViRepository
    {
        /// <summary>
        /// Lấy danh sách quy đổi đơn vị theo mã sản phẩm
        /// </summary>
        Task<IEnumerable<QuyDoiDonVi>> GetByMaSPAsync(Guid maSP);
        
        /// <summary>
        /// Lấy tỷ lệ quy đổi theo mã sản phẩm và đơn vị nhập
        /// Trả về TyLe nếu tìm thấy, null nếu không có
        /// </summary>
        Task<decimal?> GetTyLeByMaSPAndDonViNhapAsync(Guid maSP, string donViNhap);

        /// <summary>Lấy 1 cấu hình quy đổi theo mã</summary>
        Task<QuyDoiDonVi?> GetByIdAsync(Guid maQD);

        /// <summary>Thêm mới cấu hình quy đổi</summary>
        Task<QuyDoiDonVi> AddAsync(QuyDoiDonVi entity);

        /// <summary>Cập nhật cấu hình quy đổi</summary>
        Task<QuyDoiDonVi?> UpdateAsync(QuyDoiDonVi entity);

        /// <summary>Xoá cấu hình quy đổi</summary>
        Task<bool> DeleteAsync(Guid maQD);

        /// <summary>Kiểm tra trùng đơn vị nhập cho 1 sản phẩm</summary>
        Task<bool> ExistsDonViNhapAsync(Guid maSP, string donViNhap, Guid? excludeMaQD = null);
    }
}
