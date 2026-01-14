using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.NhaCungCap;
using VETFEED.Backend.API.DTOs.NhaCungCapSanPham;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class NhaCungCapService : INhaCungCapService
    {
        private readonly VetFeedManagementContext _context;
        private readonly INhaCungCapRepository _repo;
        private readonly INhaCungCapSanPhamRepository _sanPhamRepo;

        public NhaCungCapService(
            VetFeedManagementContext context,
            INhaCungCapRepository repo, 
            INhaCungCapSanPhamRepository sanPhamRepo)
        {
            _context = context;
            _repo = repo;
            _sanPhamRepo = sanPhamRepo;
        }

        public async Task<IEnumerable<NhaCungCapResponse>> GetAllNhaCungCapsAsync()
        {
            return await _repo.GetAllNhaCungCapsAsync();
        }

        public async Task<NhaCungCapDetailedResponse?> GetNhaCungCapByIdAsync(Guid id)
        {
            var nhaCungCap = await _repo.GetNhaCungCapByIdAsync(id);
            if (nhaCungCap == null) return null;

            var sanPhams = await _sanPhamRepo.GetByNhaCungCapAsync(id);

            return new NhaCungCapDetailedResponse
            {
                MaNCC = nhaCungCap.MaNCC,
                MaNCCCode = nhaCungCap.MaNCCCode,
                TenNCC = nhaCungCap.TenNCC,
                SoDienThoai = nhaCungCap.SoDienThoai,
                DiaChi = nhaCungCap.DiaChi,
                TrangThai = nhaCungCap.TrangThai,
                GhiChu = nhaCungCap.GhiChu,
                NgayTao = nhaCungCap.NgayTao,
                SanPhams = sanPhams.ToList()
            };
        }

        /// <summary>
        /// Tạo mới nhà cung cấp kèm danh sách sản phẩm (nếu có)
        /// </summary>
        public async Task<NhaCungCapDetailedResponse> AddNhaCungCapAsync(NhaCungCapCreateRequest request)
        {
            // Validate dữ liệu nhà cung cấp
            if (string.IsNullOrWhiteSpace(request.TenNCC))
                throw new ArgumentException("Tên nhà cung cấp không được để trống.");

            if (!Enum.TryParse<TrangThaiNhaCungCapEnum>(request.TrangThai, true, out _))
                throw new ArgumentException("TrangThai không hợp lệ. Chỉ nhận: HOAT_DONG | NGUNG_HOAT_DONG.");

            // Validate danh sách sản phẩm (nếu có)
            if (request.SanPhams != null && request.SanPhams.Count > 0)
            {
                for (int i = 0; i < request.SanPhams.Count; i++)
                {
                    var sp = request.SanPhams[i];
                    if (sp.MaSP == Guid.Empty)
                        throw new ArgumentException($"Sản phẩm thứ {i + 1}: Mã sản phẩm không được để trống.");

                    if (!string.IsNullOrWhiteSpace(sp.TrangThai) &&
                        !Enum.TryParse<TrangThaiNhaCungCapSanPhamEnum>(sp.TrangThai, true, out _))
                        throw new ArgumentException($"Sản phẩm thứ {i + 1}: TrangThai không hợp lệ. Chỉ nhận: HOAT_DONG | NGUNG_HOAT_DONG.");
                }
            }

            // Sử dụng transaction để đảm bảo tính nhất quán
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Tạo nhà cung cấp
                var basicRequest = new NhaCungCapRequest
                {
                    TenNCC = request.TenNCC,
                    SoDienThoai = request.SoDienThoai,
                    DiaChi = request.DiaChi,
                    TrangThai = request.TrangThai,
                    GhiChu = request.GhiChu
                };
                var createdNCC = await _repo.AddNhaCungCapAsync(basicRequest);

                // 2. Tạo danh sách sản phẩm (nếu có)
                if (request.SanPhams != null && request.SanPhams.Count > 0)
                {
                    foreach (var sp in request.SanPhams)
                    {
                        var sanPhamRequest = new NhaCungCapSanPhamRequest
                        {
                            MaNCC = createdNCC.MaNCC,
                            MaSP = sp.MaSP,
                            GiaNhapMacDinh = sp.GiaNhapMacDinh,
                            TrangThai = sp.TrangThai ?? "HOAT_DONG",
                            GhiChu = sp.GhiChu
                        };
                        await _sanPhamRepo.AddNhaCungCapSanPhamAsync(sanPhamRequest);
                    }
                }

                await transaction.CommitAsync();

                // 3. Lấy và trả về thông tin chi tiết
                return (await GetNhaCungCapByIdAsync(createdNCC.MaNCC))!;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /* Update nhà cung cấp
           Logic xử lý SanPhams:
           - NCCSP trong DB nhưng KHÔNG có trong request (theo MaNCSP): sẽ bị XÓA
           - NCCSP trong request với MaNCSP = null/Guid.Empty: THÊM MỚI
           - NCCSP trong request với MaNCSP có giá trị: CẬP NHẬT
         */
        public async Task<NhaCungCapDetailedResponse?> UpdateNhaCungCapAsync(Guid id, NhaCungCapUpdateRequest request)
        {
            // Validate dữ liệu nhà cung cấp
            if (string.IsNullOrWhiteSpace(request.TenNCC))
                throw new ArgumentException("Tên nhà cung cấp không được để trống.");

            if (!Enum.TryParse<TrangThaiNhaCungCapEnum>(request.TrangThai, true, out _))
                throw new ArgumentException("TrangThai không hợp lệ. Chỉ nhận: HOAT_DONG | NGUNG_HOAT_DONG.");

            // Kiểm tra NCC có tồn tại không
            var existingNCC = await _repo.GetNhaCungCapByIdAsync(id);
            if (existingNCC == null)
                return null;

            // Validate danh sách sản phẩm (nếu có)
            if (request.SanPhams != null && request.SanPhams.Count > 0)
            {
                for (int i = 0; i < request.SanPhams.Count; i++)
                {
                    var sp = request.SanPhams[i];
                    if (sp.MaSP == Guid.Empty)
                        throw new ArgumentException($"Sản phẩm thứ {i + 1}: Mã sản phẩm không được để trống.");

                    if (!string.IsNullOrWhiteSpace(sp.TrangThai) &&
                        !Enum.TryParse<TrangThaiNhaCungCapSanPhamEnum>(sp.TrangThai, true, out _))
                        throw new ArgumentException($"Sản phẩm thứ {i + 1}: TrangThai không hợp lệ. Chỉ nhận: HOAT_DONG | NGUNG_HOAT_DONG.");
                }
            }

            // Sử dụng transaction để đảm bảo tính nhất quán
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Cập nhật thông tin nhà cung cấp
                var basicRequest = new NhaCungCapRequest
                {
                    TenNCC = request.TenNCC,
                    SoDienThoai = request.SoDienThoai,
                    DiaChi = request.DiaChi,
                    TrangThai = request.TrangThai,
                    GhiChu = request.GhiChu
                };
                await _repo.UpdateNhaCungCapAsync(id, basicRequest);

                // 2. Xử lý danh sách sản phẩm (nếu có)
                if (request.SanPhams != null)
                {
                    // Lấy danh sách NCCSP hiện có trong DB
                    var existingNCCSPs = await _sanPhamRepo.GetEntitiesByNhaCungCapAsync(id);
                    var existingMaNCCSPs = existingNCCSPs.Select(sp => sp.MaNCSP).ToHashSet();

                    // Lọc ra các MaNCSP trong request (bỏ qua null và Guid.Empty vì đó là NCCSP mới)
                    var requestMaNCCSPs = request.SanPhams
                        .Where(sp => sp.MaNCSP.HasValue && sp.MaNCSP.Value != Guid.Empty)
                        .Select(sp => sp.MaNCSP!.Value)
                        .ToHashSet();

                    // XÓA: NCCSP trong DB nhưng KHÔNG có trong request
                    foreach (var existingNCSP in existingNCCSPs)
                    {
                        if (!requestMaNCCSPs.Contains(existingNCSP.MaNCSP))
                        {
                            await _sanPhamRepo.DeleteNhaCungCapSanPhamAsync(existingNCSP.MaNCSP);
                        }
                    }

                    // THÊM MỚI hoặc CẬP NHẬT
                    for (int i = 0; i < request.SanPhams.Count; i++)
                    {
                        var sp = request.SanPhams[i];
                        var hasMaNCSP = sp.MaNCSP.HasValue && sp.MaNCSP.Value != Guid.Empty;

                        var sanPhamRequest = new NhaCungCapSanPhamRequest
                        {
                            MaNCC = id,
                            MaSP = sp.MaSP,
                            GiaNhapMacDinh = sp.GiaNhapMacDinh,
                            TrangThai = sp.TrangThai ?? "HOAT_DONG",
                            GhiChu = sp.GhiChu
                        };

                        if (hasMaNCSP)
                        {
                            // CẬP NHẬT: MaNCSP có giá trị
                            // Kiểm tra MaNCSP có tồn tại trong DB không
                            if (!existingMaNCCSPs.Contains(sp.MaNCSP!.Value))
                                throw new ArgumentException($"Sản phẩm thứ {i + 1}: Không tìm thấy liên kết NCC-SP với mã {sp.MaNCSP}.");
                            
                            await _sanPhamRepo.UpdateNhaCungCapSanPhamAsync(sp.MaNCSP!.Value, sanPhamRequest);
                        }
                        else
                        {
                            // THÊM MỚI: MaNCSP = null/Empty
                            await _sanPhamRepo.AddNhaCungCapSanPhamAsync(sanPhamRequest);
                        }
                    }
                }

                await transaction.CommitAsync();

                // 3. Lấy và trả về thông tin chi tiết sau khi cập nhật
                return await GetNhaCungCapByIdAsync(id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Xóa nhà cung cấp và cascade xóa tất cả NCC sản phẩm liên quan
        /// </summary>
        public async Task<bool> DeleteNhaCungCapAsync(Guid id)
        {
            // Kiểm tra NCC có tồn tại không
            var existingNCC = await _repo.GetNhaCungCapByIdAsync(id);
            if (existingNCC == null)
                return false;

            // Sử dụng transaction để đảm bảo tính nhất quán
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Cascade xóa tất cả NCC sản phẩm
                await _sanPhamRepo.DeleteByNhaCungCapAsync(id);

                // 2. Xóa nhà cung cấp
                var result = await _repo.DeleteNhaCungCapAsync(id);

                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
