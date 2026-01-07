using VETFEED.Backend.API.DTOs.CTPhieuNhap;
using VETFEED.Backend.API.DTOs.LoHang;
using VETFEED.Backend.API.DTOs.PhieuNhap;
using VETFEED.Backend.API.Enums;
using VETFEED.Backend.API.Repositories;

namespace VETFEED.Backend.API.Services
{
    public class PhieuNhapService : IPhieuNhapService
    {
        private readonly IPhieuNhapRepository _phieuNhapRepo;
        private readonly ICTPhieuNhapRepository _ctPhieuNhapRepo;
        private readonly ILoHangRepository _loHangRepo;
        private readonly ITonKhoRepository _tonKhoRepo;
        private readonly IQuyDoiDonViRepository _quyDoiDonViRepo;

        public PhieuNhapService(
            IPhieuNhapRepository phieuNhapRepo,
            ICTPhieuNhapRepository ctPhieuNhapRepo,
            ILoHangRepository loHangRepo,
            ITonKhoRepository tonKhoRepo,
            IQuyDoiDonViRepository quyDoiDonViRepo)
        {
            _phieuNhapRepo = phieuNhapRepo;
            _ctPhieuNhapRepo = ctPhieuNhapRepo;
            _loHangRepo = loHangRepo;
            _tonKhoRepo = tonKhoRepo;
            _quyDoiDonViRepo = quyDoiDonViRepo;
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
            // Validation
            if (request.MaNCC == Guid.Empty)
                throw new ArgumentException("Mã nhà cung cấp không hợp lệ.");
            if (request.MaKho == Guid.Empty)
                throw new ArgumentException("Mã kho không hợp lệ.");
            if (request.DanhSachChiTiet == null || !request.DanhSachChiTiet.Any())
                throw new ArgumentException("Danh sách chi tiết phiếu nhập không được để trống.");

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
                foreach (var chiTiet in request.DanhSachChiTiet)
                {
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
                        (chiTiet.DonGia != null) ? chiTiet.DonGia.Value : 0
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
                        DonGia = ctEntity.DonGia
                    });
                }
            }

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

        /* Update phiếu nhập
           Logic: 
           Nếu trạng thái DA_NHAN Thì không thể chuyển lại trạng thái khác
           Nếu trạng thái DA_HUY Thì không thể chuyển lại trạng thái khác
           Nếu trạng thái DA_DAT Thì có thể chuyển sang DA_NHAN hoặc DA_HUY
           Nếu update trạng thái thành DA_NHAN thì cập nhật tiền, tồn kho, tính tổng cho từng CTPN
           Đảm bảo khi chuyển trạng thái thành đã nhập thì phải có Đơn giá của sản phẩm (để tính tổng tiền) Error message: "Cần ghi đơn giá khi nhận hàng!"
           Quy đổi trường SoLuong dựa vào bảng QuyDoiDonVi để lưu tồn kho theo đơn vị bán hàng
           Nếu trạng thái là DA_HUY thì không làm gì
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
            if (request.DanhSachChiTiet == null || !request.DanhSachChiTiet.Any())
                throw new ArgumentException("Danh sách chi tiết phiếu nhập không được để trống.");

            // 3. Validate và parse trạng thái mới
            var currentStatus = phieuNhap.TrangThai;
            TrangThaiPhieuNhapEnum newStatus;

            if (!string.IsNullOrEmpty(request.TrangThai))
            {
                if (!Enum.TryParse<TrangThaiPhieuNhapEnum>(request.TrangThai, true, out newStatus))
                    throw new ArgumentException("TrangThai không hợp lệ. Chỉ nhận: DA_DAT | DA_NHAN | DA_HUY.");
            }
            else
            {
                newStatus = currentStatus; // Giữ nguyên trạng thái cũ nếu không cung cấp
            }

            // Không cho phép thay đổi từ DA_NHAN hoặc DA_HUY
            if (currentStatus == TrangThaiPhieuNhapEnum.DA_NHAN)
                throw new InvalidOperationException("Phiếu nhập đã nhận không thể chuyển sang trạng thái khác.");
            if (currentStatus == TrangThaiPhieuNhapEnum.DA_HUY)
                throw new InvalidOperationException("Phiếu nhập đã hủy không thể chuyển sang trạng thái khác.");

            // 4. So sánh danh sách chi tiết trong DB với request, xóa những chi tiết không có trong request
            var existingCTPNs = await _ctPhieuNhapRepo.GetCTPhieuNhapEntitiesByMaPNAsync(id);
            var requestMaCTPNs = request.DanhSachChiTiet?.Select(ct => ct.MaCTPN).ToHashSet() ?? new HashSet<Guid>();

            foreach (var existingCTPN in existingCTPNs)
            {
                // Nếu chi tiết không có trong danh sách gửi về => xóa
                if (!requestMaCTPNs.Contains(existingCTPN.MaCTPN))
                {
                    // Xóa CTPN
                    await _ctPhieuNhapRepo.DeleteCTPhieuNhapAsync(existingCTPN.MaCTPN);

                    // Xóa LoHang liên quan (vì LoHang được tạo khi tạo CTPN và chỉ dùng cho CTPN này)
                    await _loHangRepo.DeleteLoHangAsync(existingCTPN.MaLo);
                }
            }

            // 5. Cập nhật thông tin chi tiết phiếu nhập (SoLuong, DonGia, NgaySanXuat, HanSuDung)
            if (request.DanhSachChiTiet != null && request.DanhSachChiTiet.Any())
            {
                foreach (var ctUpdate in request.DanhSachChiTiet)
                {
                    if (ctUpdate.MaCTPN == Guid.Empty)
                        throw new ArgumentException("Mã chi tiết phiếu nhập không hợp lệ.");

                    // Cập nhật SoLuong, DonGia cho CTPN
                    var updated = await _ctPhieuNhapRepo.UpdateCTPhieuNhapAsync(
                        ctUpdate.MaCTPN,
                        ctUpdate.SoLuong,
                        ctUpdate.DonGia ?? 0
                    );

                    if (!updated)
                        throw new ArgumentException($"Không tìm thấy chi tiết phiếu nhập với mã {ctUpdate.MaCTPN}.");

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

            // 6. Xử lý khi chuyển sang DA_NHAN
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
                    var quyDoiList = await _quyDoiDonViRepo.GetByMaSPAsync(loHang.MaSP);
                    decimal tyLe = 1;
                    
                    // Lấy tỷ lệ đầu tiên nếu có
                    var quyDoi = quyDoiList.FirstOrDefault();
                    if (quyDoi != null)
                    {
                        tyLe = quyDoi.TyLe;
                    }

                    // SoLuong trong tồn kho = SoLuong nhập * TyLe
                    decimal soLuongTonKho = ct.SoLuong * tyLe;

                    // Cập nhật tồn kho (thêm mới hoặc cộng thêm)
                    await _tonKhoRepo.AddOrUpdateTonKhoAsync(request.MaKho, ct.MaLo, soLuongTonKho);
                }
            }
            
            // 6. Nếu chuyển sang DA_HUY thì không làm gì với tồn kho
            // (chỉ cập nhật trạng thái)

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

            // 9. Trả về response chi tiết
            var result = await _phieuNhapRepo.GetPhieuNhapByIdAsync(id);
            if (result == null)
                throw new InvalidOperationException("Không thể lấy thông tin phiếu nhập sau khi cập nhật.");
            
            return result;
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
            return await _phieuNhapRepo.DeletePhieuNhapAsync(id);
        }

    }
}


