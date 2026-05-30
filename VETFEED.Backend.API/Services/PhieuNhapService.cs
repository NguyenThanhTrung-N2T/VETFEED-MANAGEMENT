using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Data;
using VETFEED.Backend.API.DTOs.CTPhieuNhap;
using VETFEED.Backend.API.DTOs.LoHang;
using VETFEED.Backend.API.DTOs.PhieuNhap;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class PhieuNhapService : IPhieuNhapService
    {
        private readonly VetFeedManagementContext _context;
        private readonly IPhieuNhapRepository _phieuNhapRepo;
        private readonly ICTPhieuNhapRepository _ctPhieuNhapRepo;
        private readonly ILoHangRepository _loHangRepo;
        private readonly ITonKhoRepository _tonKhoRepo;
        private readonly IQuyDoiDonViRepository _quyDoiDonViRepo;
        private readonly IKhoHangRepository _khoHangRepo;


        public PhieuNhapService(
            VetFeedManagementContext context,
            IPhieuNhapRepository phieuNhapRepo,
            ICTPhieuNhapRepository ctPhieuNhapRepo,
            ILoHangRepository loHangRepo,
            ITonKhoRepository tonKhoRepo,
            IQuyDoiDonViRepository quyDoiDonViRepo,
            IKhoHangRepository khoHangRepo)
        {
            _context = context;
            _phieuNhapRepo = phieuNhapRepo;
            _ctPhieuNhapRepo = ctPhieuNhapRepo;
            _loHangRepo = loHangRepo;
            _tonKhoRepo = tonKhoRepo;
            _quyDoiDonViRepo = quyDoiDonViRepo;
            _khoHangRepo = khoHangRepo;
        }

        /// <summary>
        /// Validate đơn vị nhập có tồn tại trong QuyDoiDonVi cho sản phẩm không
        /// </summary>
        private async Task ValidateDonViNhapAsync(Guid maSP, string? donViNhap)
        {
            if (string.IsNullOrEmpty(donViNhap))
                return; // Không cần validate nếu không có đơn vị nhập

            // Kiểm tra đơn vị nhập có tồn tại không
            var tyLe = await _quyDoiDonViRepo.GetTyLeByMaSPAndDonViNhapAsync(maSP, donViNhap);
            if (tyLe == null)
            {
                // Lấy danh sách đơn vị nhập hợp lệ
                var quyDoiList = await _quyDoiDonViRepo.GetByMaSPAsync(maSP);
                var validUnits = quyDoiList
                    .Where(q => !string.IsNullOrEmpty(q.DonViNhap))
                    .Select(q => q.DonViNhap)
                    .Distinct()
                    .ToList();

                if (validUnits.Any())
                {
                    throw new ArgumentException($"Đơn vị nhập '{donViNhap}' không tồn tại. Đơn vị nhập chỉ chấp nhận: {string.Join(", ", validUnits)}");
                }
                else
                {
                    throw new ArgumentException($"Đơn vị nhập '{donViNhap}' không tồn tại. Sản phẩm này chưa có cấu hình quy đổi đơn vị.");
                }
            }
        }

        /// <summary>
        /// Validate sản phẩm không ở trạng thái Không hoạt động
        /// </summary>
        private async Task ValidateSanPhamTrangThaiAsync(Guid maSP, int soThuTu)
        {
            var sanPham = await _context.SanPhams.FirstOrDefaultAsync(sp => sp.MaSP == maSP);
            if (sanPham == null)
                throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: Không tìm thấy sản phẩm.");

            if (sanPham.TrangThai == TrangThaiSanPhamEnum.KhongHoatDong)
                throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: Sản phẩm '{sanPham.TenSP}' đang ở trạng thái Không hoạt động, không thể nhập hàng.");
        }

        // Lấy tất cả phiếu nhập
        public async Task<IEnumerable<PhieuNhapResponse>> GetAllPhieuNhapsAsync()
        {
            return await _phieuNhapRepo.GetAllPhieuNhapsAsync();
        }

        // Lấy phiếu nhập theo ID (bao gồm chi tiết)
        public async Task<PhieuNhapDetailedResponse?> GetPhieuNhapByIdAsync(Guid id)
        {
            return await _phieuNhapRepo.GetPhieuNhapByIdAsync(id);
        }

        // Tạo phiếu nhập mới với danh sách chi tiết
        public async Task<PhieuNhapDetailedResponse> CreatePhieuNhapAsync(PhieuNhapCreateRequest request)
        {
            // Validation trước khi bắt đầu transaction
            if (request.MaNCC == Guid.Empty)
                throw new ArgumentException("Mã nhà cung cấp không hợp lệ.");
            if (request.MaKho == Guid.Empty)
                throw new ArgumentException("Mã kho không hợp lệ.");

            // Kiểm tra NCC không bị soft delete
            var nhaCungCap = await _context.NhaCungCaps.FirstOrDefaultAsync(n => n.MaNCC == request.MaNCC);
            if (nhaCungCap == null || nhaCungCap.IsDeleted)
                throw new ArgumentException("Nhà cung cấp không tồn tại hoặc đã bị xóa.");

            if (request.DanhSachChiTiet == null || !request.DanhSachChiTiet.Any())
                throw new ArgumentException("Danh sách chi tiết phiếu nhập không được để trống.");

            var khohang = await _khoHangRepo.GetKhoHangByIdAsync(request.MaKho);
            if (khohang == null)
                throw new ArgumentException("Kho hàng không tồn tại.");
            if (khohang.TrangThai == TrangThaiKhoEnum.NGUNG_HOAT_DONG.ToString())
                throw new ArgumentException("Kho hàng đang ở trạng thái Không hoạt động, không thể nhập hàng vào kho này.");

            // Sử dụng transaction để đảm bảo tính nhất quán dữ liệu
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Tạo phiếu nhập với trạng thái DA_DAT
                var phieuNhapRequest = new PhieuNhapRequest
                {
                    MaNCC = request.MaNCC,
                    MaKho = request.MaKho,
                    TrangThai = TrangThaiPhieuNhapEnum.DA_DAT.ToString(),
                    GhiChu = request.GhiChu
                };
                var phieuNhap = await _phieuNhapRepo.AddPhieuNhapAsync(phieuNhapRequest);

                // 2. Tạo từng chi tiết (lô hàng + CTPN)
                var danhSachChiTiet = new List<CTPhieuNhapResponse>();
                if (request.DanhSachChiTiet != null && request.DanhSachChiTiet.Any())
                {
                    var chiTietList = request.DanhSachChiTiet.ToList();
                    for (int i = 0; i < chiTietList.Count; i++)
                    {
                        var chiTiet = chiTietList[i];
                        var soThuTu = i + 1; // Số thứ tự 1-based cho user

                        // Validate: SoLuong phải lớn hơn 0
                        if (chiTiet.SoLuong <= 0)
                            throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: Số lượng phải lớn hơn 0.");

                        // Validate: DonGia không được âm
                        if (chiTiet.DonGia.HasValue && chiTiet.DonGia.Value < 0)
                            throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: Đơn giá không được âm.");

                        // Validate: HanSuDung phải trong tương lai
                        if (chiTiet.HanSuDung <= DateTime.Now)
                            throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: Hạn sử dụng phải là ngày trong tương lai.");

                        // Validate: NgaySanXuat < HanSuDung (nếu có NSX)
                        if (chiTiet.NgaySanXuat.HasValue && chiTiet.NgaySanXuat.Value >= chiTiet.HanSuDung)
                            throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: Ngày sản xuất phải trước hạn sử dụng.");

                        // Validate đơn vị nhập với thông báo lỗi rõ ràng
                        try
                        {
                            await ValidateDonViNhapAsync(chiTiet.MaSP, chiTiet.DonViNhap);
                        }
                        catch (ArgumentException ex)
                        {
                            throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: {ex.Message}");
                        }

                        // Validate sản phẩm không ở trạng thái Không hoạt động
                        await ValidateSanPhamTrangThaiAsync(chiTiet.MaSP, soThuTu);

                        // Tạo lô hàng mới
                        var loHangRequest = new LoHangRequest
                        {
                            MaSP = chiTiet.MaSP,
                            NgaySanXuat = chiTiet.NgaySanXuat,
                            HanSuDung = chiTiet.HanSuDung
                        };
                        var loHang = await _loHangRepo.AddLoHangAsync(loHangRequest);

                        // Tạo chi tiết phiếu nhập (DonGia = 0 mặc định)
                        var ctEntity = await _ctPhieuNhapRepo.AddCTPhieuNhapAsync(
                            phieuNhap.MaPN,
                            loHang.MaLo,
                            chiTiet.SoLuong,
                            (chiTiet.DonGia != null) ? chiTiet.DonGia.Value : 0,
                            chiTiet.DonViNhap,
                            0, // SoLuongQuyDoi - sẽ được tính khi nhận hàng
                            0  // DonGiaCoSo - sẽ được tính khi nhận hàng
                        );

                        danhSachChiTiet.Add(new CTPhieuNhapResponse
                        {
                            MaCTPN = ctEntity.MaCTPN,
                            MaPN = ctEntity.MaPN,
                            MaLo = ctEntity.MaLo,
                            MaLoCode = loHang.MaLoCode,
                            TenSP = loHang.TenSP,
                            NgaySanXuat = loHang.NgaySanXuat,
                            HanSuDung = loHang.HanSuDung,
                            SoLuong = ctEntity.SoLuong,
                            DonGia = ctEntity.DonGia,
                            DonViNhap = ctEntity.DonViNhap,
                            SoLuongQuyDoi = ctEntity.SoLuongQuyDoi,
                            DonGiaCoSo = ctEntity.DonGiaCoSo
                        });
                    }
                }

                // Commit transaction sau khi tất cả thao tác thành công
                await transaction.CommitAsync();

                // 3. Trả về response chi tiết
                return new PhieuNhapDetailedResponse
                {
                    MaPN = phieuNhap.MaPN,
                    MaPNCode = phieuNhap.MaPNCode,
                    MaNCC = phieuNhap.MaNCC,
                    TenNCC = phieuNhap.TenNCC,
                    MaKho = phieuNhap.MaKho,
                    TenKho = phieuNhap.TenKho,
                    ThanhTien = 0, // DonGia = 0 nên ThanhTien = 0
                    TrangThai = phieuNhap.TrangThai,
                    GhiChu = phieuNhap.GhiChu,
                    NgayCapNhat = phieuNhap.NgayCapNhat,
                    DanhSachChiTiet = danhSachChiTiet
                };
            }
            catch
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw;
            }
        }

        /* Update phiếu nhập
           Logic: 
           Nếu trạng thái DA_NHAN Thì không thể chuyển lại trạng thái khác
           Nếu trạng thái DA_HUY Thì có thể chuyển sang DA_DAT nhưng KHÔNG được chuyển sang DA_NHAN
           Nếu trạng thái DA_DAT Thì có thể chuyển sang DA_NHAN hoặc DA_HUY
           Nếu update trạng thái thành DA_NHAN thì cập nhật tiền, tồn kho, tính tổng cho từng CTPN
           Đảm bảo khi chuyển trạng thái thành đã nhập thì phải có Đơn giá của sản phẩm (để tính tổng tiền) Error message: "Cần ghi đơn giá khi nhận hàng!"
           Quy đổi trường SoLuong dựa vào bảng QuyDoiDonVi để lưu tồn kho theo đơn vị bán hàng
           
           CTPN trong request:
           - Nếu MaCTPN = Guid.Empty: tạo mới CTPN (yêu cầu MaSP, HanSuDung)
           - Nếu MaCTPN != Guid.Empty: cập nhật CTPN đã có
           - CTPN trong DB nhưng không có trong request: sẽ bị xóa
         */
        public async Task<PhieuNhapDetailedResponse> UpdatePhieuNhapAsync(Guid id, PhieuNhapUpdateRequest request)
        {
            // 1. Lấy phiếu nhập hiện tại
            var phieuNhap = await _phieuNhapRepo.GetPhieuNhapEntityByIdAsync(id);
            if (phieuNhap == null)
                throw new ArgumentException("Không tìm thấy phiếu nhập.");
            // 2. Validate mã nhà cung cấp và mã kho
            if (request.MaNCC == Guid.Empty)
                throw new ArgumentException("Mã nhà cung cấp không hợp lệ.");
            if (request.MaKho == Guid.Empty)
                throw new ArgumentException("Mã kho không hợp lệ.");

            // Kiểm tra NCC không bị soft delete
            var nhaCungCap = await _context.NhaCungCaps.FirstOrDefaultAsync(n => n.MaNCC == request.MaNCC);
            if (nhaCungCap == null || nhaCungCap.IsDeleted)
                throw new ArgumentException("Nhà cung cấp không tồn tại hoặc đã bị xóa.");

            // Kiểm tra kho hàng tồn tại và trạng thái
            var khohang = await _khoHangRepo.GetKhoHangByIdAsync(request.MaKho);
            if (khohang == null)
                throw new ArgumentException("Kho hàng không tồn tại.");
            if (khohang.TrangThai == TrangThaiKhoEnum.NGUNG_HOAT_DONG.ToString())
                throw new ArgumentException("Kho hàng đang ở trạng thái Không hoạt động, không thể nhập hàng vào kho này.");

            if (request.DanhSachChiTiet == null || !request.DanhSachChiTiet.Any())
                throw new ArgumentException("Danh sách chi tiết phiếu nhập không được để trống.");

            // 3. Validate và parse trạng thái mới
            var currentStatus = phieuNhap.TrangThai;
            TrangThaiPhieuNhapEnum newStatus;

            if (currentStatus == TrangThaiPhieuNhapEnum.DA_NHAN)
                throw new ArgumentException("Không thể cập nhật phiếu nhập đã nhận.");
            if (!string.IsNullOrEmpty(request.TrangThai))
            {
                if (!Enum.TryParse<TrangThaiPhieuNhapEnum>(request.TrangThai, true, out newStatus))
                    throw new ArgumentException("TrangThai không hợp lệ. Chỉ nhận: DA_DAT | DA_NHAN | DA_HUY.");
            }
            else
            {
                newStatus = currentStatus; // Giữ nguyên trạng thái cũ nếu không cung cấp
            }

            // DA_HUY có thể chuyển sang DA_DAT nhưng KHÔNG được chuyển sang DA_NHAN
            if (currentStatus == TrangThaiPhieuNhapEnum.DA_HUY && newStatus == TrangThaiPhieuNhapEnum.DA_NHAN)
                throw new InvalidOperationException("Phiếu nhập đã hủy không thể chuyển sang trạng thái Đã nhận.");

            // Sử dụng transaction để đảm bảo tính nhất quán dữ liệu
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                // 4. So sánh danh sách chi tiết trong DB với request
                var existingCTPNs = await _ctPhieuNhapRepo.GetCTPhieuNhapEntitiesByMaPNAsync(id);
                var existingMaCTPNs = existingCTPNs.Select(ct => ct.MaCTPN).ToHashSet();

                // Lọc ra các MaCTPN trong request (bỏ qua Guid.Empty vì đó là CTPN mới)
                var requestMaCTPNs = request.DanhSachChiTiet?
                    .Where(ct => ct.MaCTPN != Guid.Empty)
                    .Select(ct => ct.MaCTPN)
                    .ToHashSet() ?? new HashSet<Guid>();

                // Xóa những CTPN không có trong request
                foreach (var existingCTPN in existingCTPNs)
                {
                    if (!requestMaCTPNs.Contains(existingCTPN.MaCTPN))
                    {
                        // Xóa CTPN
                        await _ctPhieuNhapRepo.DeleteCTPhieuNhapAsync(existingCTPN.MaCTPN);

                        // Xóa LoHang liên quan
                        await _loHangRepo.DeleteLoHangAsync(existingCTPN.MaLo);
                    }
                }

                // 5. Xử lý từng chi tiết: THÊM MỚI hoặc CẬP NHẬT
                if (request.DanhSachChiTiet != null && request.DanhSachChiTiet.Any())
                {
                    var ctUpdateList = request.DanhSachChiTiet.ToList();
                    for (int i = 0; i < ctUpdateList.Count; i++)
                    {
                        var ctUpdate = ctUpdateList[i];
                        var soThuTu = i + 1; // Số thứ tự 1-based cho user

                        if (ctUpdate.MaCTPN == Guid.Empty)
                        {
                            // ===== THÊM MỚI CTPN =====
                            // Validate: MaSP bắt buộc khi tạo mới
                            if (ctUpdate.MaSP == null || ctUpdate.MaSP == Guid.Empty)
                                throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: Mã sản phẩm là bắt buộc khi thêm mới chi tiết phiếu nhập.");

                            // Validate: HanSuDung bắt buộc khi tạo mới
                            if (!ctUpdate.HanSuDung.HasValue)
                                throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: Hạn sử dụng là bắt buộc khi thêm mới chi tiết phiếu nhập.");

                            // Validate: SoLuong phải lớn hơn 0
                            if (ctUpdate.SoLuong <= 0)
                                throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: Số lượng phải lớn hơn 0.");

                            // Validate: DonGia không được âm
                            if (ctUpdate.DonGia.HasValue && ctUpdate.DonGia.Value < 0)
                                throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: Đơn giá không được âm.");

                            // Validate: HanSuDung phải trong tương lai
                            if (ctUpdate.HanSuDung.Value <= DateTime.Now)
                                throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: Hạn sử dụng phải là ngày trong tương lai.");

                            // Validate: NgaySanXuat < HanSuDung
                            if (ctUpdate.NgaySanXuat.HasValue && ctUpdate.NgaySanXuat.Value >= ctUpdate.HanSuDung.Value)
                                throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: Ngày sản xuất phải trước hạn sử dụng.");

                            // Validate đơn vị nhập với thông báo lỗi rõ ràng
                            try
                            {
                                await ValidateDonViNhapAsync(ctUpdate.MaSP.Value, ctUpdate.DonViNhap);
                            }
                            catch (ArgumentException ex)
                            {
                                throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: {ex.Message}");
                            }

                            // Validate sản phẩm không ở trạng thái Không hoạt động
                            await ValidateSanPhamTrangThaiAsync(ctUpdate.MaSP.Value, soThuTu);

                            // Tạo lô hàng mới
                            var loHangRequest = new LoHangRequest
                            {
                                MaSP = ctUpdate.MaSP.Value,
                                NgaySanXuat = ctUpdate.NgaySanXuat,
                                HanSuDung = ctUpdate.HanSuDung.Value
                            };
                            var loHang = await _loHangRepo.AddLoHangAsync(loHangRequest);

                            // Tạo chi tiết phiếu nhập mới
                            await _ctPhieuNhapRepo.AddCTPhieuNhapAsync(
                                id, // MaPN của phiếu nhập hiện tại
                                loHang.MaLo,
                                ctUpdate.SoLuong,
                                ctUpdate.DonGia ?? 0,
                                ctUpdate.DonViNhap,
                                0, // SoLuongQuyDoi - sẽ được tính khi nhận hàng
                                0  // DonGiaCoSo - sẽ được tính khi nhận hàng
                            );
                        }
                        else
                        {
                            // ===== CẬP NHẬT CTPN ĐÃ CÓ =====
                            // Kiểm tra CTPN có tồn tại trong DB không
                            if (!existingMaCTPNs.Contains(ctUpdate.MaCTPN))
                                throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: Không tìm thấy chi tiết phiếu nhập với mã {ctUpdate.MaCTPN}.");

                            // Validate đơn vị nhập - cần lấy MaSP từ LoHang
                            var existingCTPN = existingCTPNs.FirstOrDefault(ct => ct.MaCTPN == ctUpdate.MaCTPN);
                            if (existingCTPN != null)
                            {
                                var loHangInfo = await _loHangRepo.GetLoHangEntityByIdAsync(existingCTPN.MaLo);
                                if (loHangInfo != null)
                                {
                                    try
                                    {
                                        await ValidateDonViNhapAsync(loHangInfo.MaSP, ctUpdate.DonViNhap);
                                    }
                                    catch (ArgumentException ex)
                                    {
                                        throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: {ex.Message}");
                                    }
                                }
                            }

                            // Validate: SoLuong phải lớn hơn 0
                            if (ctUpdate.SoLuong <= 0)
                                throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: Số lượng phải lớn hơn 0.");

                            // Validate: DonGia không được âm
                            if (ctUpdate.DonGia.HasValue && ctUpdate.DonGia.Value < 0)
                                throw new ArgumentException($"Chi tiết phiếu số {soThuTu} có lỗi: Đơn giá không được âm.");

                            // Cập nhật SoLuong, DonGia, DonViNhap cho CTPN
                            var updated = await _ctPhieuNhapRepo.UpdateCTPhieuNhapAsync(
                                ctUpdate.MaCTPN,
                                ctUpdate.SoLuong,
                                ctUpdate.DonGia ?? 0,
                                ctUpdate.DonViNhap
                            );

                            if (!updated)
                                throw new ArgumentException($"Không thể cập nhật chi tiết phiếu nhập với mã {ctUpdate.MaCTPN}.");

                            // Cập nhật NgaySanXuat, HanSuDung cho LoHang (nếu được cung cấp)
                            if (ctUpdate.HanSuDung.HasValue)
                            {
                                // Validate: HanSuDung phải trong tương lai
                                if (ctUpdate.HanSuDung.Value <= DateTime.Now)
                                    throw new ArgumentException("Hạn sử dụng phải là ngày trong tương lai.");

                                // Validate: NgaySanXuat < HanSuDung (nếu có NSX)
                                if (ctUpdate.NgaySanXuat.HasValue && ctUpdate.NgaySanXuat.Value >= ctUpdate.HanSuDung.Value)
                                    throw new ArgumentException("Ngày sản xuất phải trước hạn sử dụng.");

                                // Lấy CTPN để lấy MaLo
                                var ctpn = await _ctPhieuNhapRepo.GetCTPhieuNhapByIdAsync(ctUpdate.MaCTPN);
                                if (ctpn == null)
                                    throw new ArgumentException($"Không tìm thấy chi tiết phiếu nhập với mã {ctUpdate.MaCTPN}.");

                                // Cập nhật LoHang
                                var loUpdated = await _loHangRepo.UpdateLoHangDatesAsync(
                                    ctpn.MaLo,
                                    ctUpdate.NgaySanXuat,
                                    ctUpdate.HanSuDung.Value
                                );

                                if (!loUpdated)
                                    throw new ArgumentException($"Không tìm thấy lô hàng với mã {ctpn.MaLo}.");
                            }
                        }
                    }
                }

                // 6. Xử lý khi chuyển sang DA_NHAN: tính SoLuongQuyDoi, DonGiaCoSo và cập nhật tồn kho
                if (newStatus == TrangThaiPhieuNhapEnum.DA_NHAN && currentStatus == TrangThaiPhieuNhapEnum.DA_DAT)
                {
                    // Lấy lại danh sách chi tiết đã cập nhật
                    var danhSachCTPN = await _ctPhieuNhapRepo.GetCTPhieuNhapEntitiesByMaPNAsync(id);

                    foreach (var ct in danhSachCTPN)
                    {
                        // Kiểm tra DonGia bắt buộc
                        if (ct.DonGia == null || ct.DonGia <= 0)
                            throw new InvalidOperationException("Cần ghi đơn giá khi nhận hàng!");

                        // Lấy thông tin lô hàng để lấy MaSP
                        var loHang = await _loHangRepo.GetLoHangEntityByIdAsync(ct.MaLo);
                        if (loHang == null)
                            throw new ArgumentException($"Không tìm thấy lô hàng với mã {ct.MaLo}.");

                        // Quy đổi số lượng theo đơn vị bán hàng
                        // Nếu không có quy đổi, mặc định TyLe = 1
                        decimal tyLe = 1;

                        // Lấy tỷ lệ theo DonViNhap đã chọn khi nhập phiếu
                        if (!string.IsNullOrEmpty(ct.DonViNhap))
                        {
                            var tyLeResult = await _quyDoiDonViRepo.GetTyLeByMaSPAndDonViNhapAsync(loHang.MaSP, ct.DonViNhap);
                            if (tyLeResult.HasValue)
                            {
                                tyLe = tyLeResult.Value;
                            }
                        }

                        // Số lượng quy đổi về đơn vị cơ sở = SoLuong nhập * TyLe
                        decimal soLuongQuyDoi = ct.SoLuong * tyLe;

                        // Giá vốn theo đơn vị cơ sở = DonGia / TyLe (backend tính)
                        decimal donGiaCoSo = tyLe > 0 ? ct.DonGia.Value / tyLe : ct.DonGia.Value;

                        // Cập nhật lại CTPN với SoLuongQuyDoi và DonGiaCoSo
                        await _ctPhieuNhapRepo.UpdateCTPhieuNhapAsync(
                            ct.MaCTPN,
                            ct.SoLuong,
                            ct.DonGia.Value,
                            ct.DonViNhap,
                            soLuongQuyDoi,
                            donGiaCoSo
                        );

                        // Cập nhật tồn kho (thêm mới hoặc cộng thêm) - sử dụng số lượng quy đổi và giá vốn cơ sở
                        await _tonKhoRepo.AddOrUpdateTonKhoAsync(request.MaKho, ct.MaLo, soLuongQuyDoi, donGiaCoSo);
                    }
                }


                // 7. Cập nhật phiếu nhập (ThanhTien từ FE và TrangThai)
                await _phieuNhapRepo.UpdatePhieuNhapThanhTienAndTrangThaiAsync(id, request.ThanhTien, newStatus);

                // 8. Cập nhật thông tin khác (MaNCC, MaKho, GhiChu) nếu cần
                var updateRequest = new PhieuNhapRequest
                {
                    MaNCC = request.MaNCC,
                    MaKho = request.MaKho,
                    TrangThai = newStatus.ToString(),
                    GhiChu = request.GhiChu
                };
                await _phieuNhapRepo.UpdatePhieuNhapAsync(id, updateRequest);

                // Commit transaction sau khi tất cả thao tác thành công
                await transaction.CommitAsync();

                // 9. Trả về response chi tiết
                var result = await _phieuNhapRepo.GetPhieuNhapByIdAsync(id);
                if (result == null)
                    throw new InvalidOperationException("Không thể lấy thông tin phiếu nhập sau khi cập nhật.");

                return result;
            }
            catch
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw;
            }
        }

        /* Delete phiếu nhập
           Logic:
           Nếu trạng thái là DA_NHAN Thì không thể xóa
           Nếu trạng thái là DA_DAT hoặc DA_HUY Thì xóa phiếu nhập, CTPN và LoHang
         */
        public async Task<bool> DeletePhieuNhapAsync(Guid id)
        {
            // 1. Lấy phiếu nhập
            var phieuNhap = await _phieuNhapRepo.GetPhieuNhapEntityByIdAsync(id);
            if (phieuNhap == null)
                throw new ArgumentException("Không tìm thấy phiếu nhập.");

            // 2. Kiểm tra trạng thái
            if (phieuNhap.TrangThai == TrangThaiPhieuNhapEnum.DA_NHAN)
                throw new InvalidOperationException("Phiếu nhập đã nhận không thể xóa.");

            // Sử dụng transaction để đảm bảo tính nhất quán dữ liệu
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 3. Batch xóa tất cả LoHang liên quan
                var danhSachCTPN = await _ctPhieuNhapRepo.GetCTPhieuNhapEntitiesByMaPNAsync(id);
                var maLoList = danhSachCTPN.Select(ct => ct.MaLo).ToList();
                if (maLoList.Any())
                {
                    await _loHangRepo.DeleteLoHangsByIdsAsync(maLoList);
                }

                // 4. Batch xóa tất cả CTPhieuNhap (không có cascade delete)
                await _ctPhieuNhapRepo.DeleteCTPhieuNhapsByMaPNAsync(id);

                // 5. Xóa phiếu nhập
                var result = await _phieuNhapRepo.DeletePhieuNhapAsync(id);

                // Commit transaction sau khi tất cả thao tác thành công
                await transaction.CommitAsync();

                return result;
            }
            catch
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}


