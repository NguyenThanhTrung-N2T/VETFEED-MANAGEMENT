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
    }
}
