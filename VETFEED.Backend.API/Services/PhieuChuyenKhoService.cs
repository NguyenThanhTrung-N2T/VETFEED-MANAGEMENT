using System.Reflection.Metadata.Ecma335;
using VETFEED.Backend.API.DTOs.PhieuChuyenKho;
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

            } catch(Exception ex)
            {
                throw new Exception("Xảy ra lỗi khi tạo phiếu chuyển kho !", ex);
            }
        }
    }

}
