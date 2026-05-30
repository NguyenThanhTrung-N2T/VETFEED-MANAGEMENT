using Microsoft.Identity.Client;
using System.Reflection.Metadata.Ecma335;
using VETFEED.Backend.API.DTOs.CTChuyenKho;
using VETFEED.Backend.API.DTOs.PhieuChuyenKho;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class PhieuChuyenKhoService : IPhieuChuyenKhoService
    {
        private readonly IPhieuChuyenKhoRepository _repository;
        private readonly IKhoHangRepository _khoHangRepo;
        public PhieuChuyenKhoService(IPhieuChuyenKhoRepository repository, IKhoHangRepository khoHangRepo)
        {
            _repository = repository;
            _khoHangRepo = khoHangRepo;
        }

        // lay danh sach phieu chuyen kho
        public async Task<IEnumerable<PhieuChuyenKhoResponse>> GetDanhSachPhieuChuyenKhoAsync()
        {
            return await _repository.GetDanhSachPhieuChuyenKhoAsync();
        }

        // lay chi tiet phieu chuyen kho 
        public async Task<ChiTietPhieuChuyenKhoResponse?> GetChiTietPhieuChuyenKhoAsync(Guid maCK)
        {
            return await _repository.GetChiTietPhieuChuyenKhoAsync(maCK);
        }

        // them phieu chuyen kho
        public async Task<ChiTietPhieuChuyenKhoResponse> AddPhieuChuyenKhoAsync(PhieuChuyenKhoRequest request)
        {
            try
            {
                // kiểm tra tồn tại kho hàng 
                var isExistKhoXuat = await _khoHangRepo.IsKhoHangExist(request.MaKhoXuat);
                if (!isExistKhoXuat)
                {
                    throw new Exception("Kho xuất không tồn tại !");
                }
                // kiểm tra tồn tại kho nhận 
                var isExistKhoNhan = await _khoHangRepo.IsKhoHangExist(request.MaKhoNhan);
                if (!isExistKhoNhan)
                {
                    throw new Exception("Kho nhận không tồn tại !");
                }

                // them phieu chuyen kho
                return await _repository.AddPhieuChuyenKhoAsync(request);

            }
            catch (Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi tạo phiếu chuyển kho !", ex);
            }
        }

        // cap nhat phieu chuyen kho 
        public async Task<ChiTietPhieuChuyenKhoResponse?> UpdatePhieuChuyenKhoAsync(UpdatePhieuChuyenKhoRequest request)
        {
            try
            {
                // cap nhat phieu chuyen kho
                return await _repository.UpdatePhieuChuyenKhoAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi cập nhật phiếu chuyển kho !", ex);
            }
        }

        // cap nhat trang thai chi tiet chuyen kho 
        public async Task<ChiTietPhieuChuyenKhoResponse?> UpdateTrangThaiChiTietAsync(Guid maCTCK, UpdateTrangThaiCTChuyenKho request)
        {
            try
            {
                return await _repository.UpdateTrangThaiChiTietAsync(maCTCK, request);
            }
            catch (Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi cập nhật chi tiết chuyển kho !", ex);
            }
        }

        // xoa phieu chuyen kho 
        public async Task<bool> XoaPhieuChuyenKhoAsync(Guid maCK)
        {
            try
            {
                return await _repository.XoaPhieuChuyenKhoAsync(maCK);
            }
            catch (Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi xóa phiếu chuyển kho !", ex);
            }
        }
    }

}
