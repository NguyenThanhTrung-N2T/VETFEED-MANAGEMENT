using VETFEED.Backend.API.DTOs.KhoHang;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class KhoHangService : IKhoHangService
    {
        private readonly IKhoHangRepository _khoHangRepo;
        public KhoHangService(IKhoHangRepository khoHangsRepo)
        {
            _khoHangRepo = khoHangsRepo;
        }

        // lấy tất cả kho hàng 
        public async Task<IEnumerable<KhoHangResponse>> GetAllKhoHangsAsync()
        {
            // lấy tất cả kho hàng từ db 
            var khoHangs = await _khoHangRepo.GetAllKhoHangsAsync();
            // trả về danh sách kho hàng 
            return khoHangs;
        }

        // lấy kho hàng theo mã kho 
        public async Task<KhoHangResponse?> GetKhoHangByIdAsync(Guid maKho)
        {
            // lấy kho hàng theo mã kho 
            var khoHang = await _khoHangRepo.GetKhoHangByIdAsync(maKho);
            if(khoHang == null)
            {
                return null;
            }
            // trả về kho hàng
            return khoHang;
        }

        // thêm kho hàng 
        public async Task<KhoHangResponse> AddKhoHangAsync(CreateKhoHangRequest dto)
        {
            try
            {
                // thêm kho hàng vào db
                return await _khoHangRepo.AddKhoHangAsync(dto);
            } catch (Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi thêm kho hàng !", ex);
            }
        }

        // cập nhật thông tin kho hàng 
        public async Task<KhoHangResponse?> UpdateKhoHangAsync(Guid maKho, UpdateKhoHangRequest dto)
        {
            try
            {
                // kiểm tra kho hàng tồn tại 
                var isExist = await _khoHangRepo.IsKhoHangExist(maKho);
                if (!isExist)
                {
                    return null;
                }
                // cập nhật thông tin kho
                return await _khoHangRepo.UpdateKhoHangAsync(maKho, dto);
            } catch ( Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi cập nhật thông tin kho hàng !", ex);
            }
        }

        // Xóa kho hàng
        public async Task<bool> DeleteKhoHangAsync(Guid maKho)
        {
            try
            {
                // kiểm tra kho hàng tồn tại 
                var isExist = await _khoHangRepo.IsKhoHangExist(maKho);
                if (!isExist)
                {
                    return false;
                }

                // xóa kho hàng trong db 
                return await _khoHangRepo.DeleteKhoHangAsync(maKho);
            } catch (Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi xóa kho hàng !", ex);
            }
        }


        // tìm kiếm kho hàng theo thuộc tính và từ khóa 
        public async Task<IEnumerable<KhoHangResponse>> SearchKhoHangAsync(SearchKhoHangRequest dto)
        {
            // lấy danh sách tìm kiếm 
            var khoHangs = await _khoHangRepo.SearchKhoHangAsync(dto);
            // trả về danh sách kho hàng
            return khoHangs;
        }
    }
}
