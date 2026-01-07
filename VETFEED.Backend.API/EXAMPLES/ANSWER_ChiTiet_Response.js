/**
 * TÓM T?T - CÂU H?I: "N?U 1 THÙNG LÀ 24 CHAI, BÁN 2 THÙNG CÓ 2 KHO, 
 * 1 KHO 20 CHAI KHO CÒN L?I 40 THÌ PHI?U BÁN TR? V? RESPONSE NH? NÀO?"
 * 
 * ============================================================================
 * RESPONSE TR? V?:
 * ============================================================================
 * 
 * HTTP Status: 201 Created
 * 
 * Body:
 * {
 *   "maPB": "550e8400-e29b-41d4-a716-446655440000",
 *   "maPBCode": "PB00000001",
 *   "maKH": "550e8400-e29b-41d4-a716-446655440000",
 *   "tenKhachHang": "Khách hàng A",
 *   "ngayBan": "2024-01-15T00:00:00",
 *   "tongTienHang": 1200000,
 *   "chieTKhauPhanTram": 0,
 *   "tienChietKhau": 0,
 *   "thanhTien": 1200000,
 *   "hinhThucThanhToan": "TIEN_MAT",
 *   "trangThaiThanhToan": "DA_THANH_TOAN",
 *   "tienCoc": 0,
 *   "tienNo": 0,
 *   "hanTra": null,
 *   "ghiChu": null,
 *   "danhSachChiTiet": [
 *     {
 *       // ===== CHI TI?T 1: L?Y T? KHO A =====
 *       "maCTPB": "550e8400-...-0001",
 *       "maKho": "kho-a-guid",
 *       "tenKho": "Kho A",
 *       "maLo": "lo-001",
 *       "maLoCode": "LH000001",
 *       "tenSanPham": "N??c mía",
 *       "donViCoSo": "Chai",
 *       "soLuong": 20,                // ? L?Y 20 CHAI T? KHO A
 *       "donViBan": "Thùng",
 *       "donGia": 600000,
 *       "soLuongQuyDoi": 20,
 *       "giaVonCoSo": 25000,
 *       "thanhTienVon": 500000,
 *       "hanSuDung": "2024-12-31T00:00:00",
 *       "ghiChu": null
 *     },
 *     {
 *       // ===== CHI TI?T 2: L?Y T? KHO B =====
 *       "maCTPB": "550e8400-...-0002",
 *       "maKho": "kho-b-guid",
 *       "tenKho": "Kho B",
 *       "maLo": "lo-001",
 *       "maLoCode": "LH000001",
 *       "tenSanPham": "N??c mía",
 *       "donViCoSo": "Chai",
 *       "soLuong": 28,                // ? L?Y 28 CHAI T? KHO B
 *       "donViBan": "Thùng",
 *       "donGia": 600000,
 *       "soLuongQuyDoi": 28,
 *       "giaVonCoSo": 20000,
 *       "thanhTienVon": 560000,
 *       "hanSuDung": "2024-12-31T00:00:00",
 *       "ghiChu": null
 *     }
 *   ]
 * }
 * 
 * ============================================================================
 * GI?I THÍCH:
 * ============================================================================
 * 
 * 1. REQUEST:
 *    - Bán 2 Thùng (= 2 × 24 = 48 Chai)
 *    - Lô: lo-001
 *    - ??n giá: 600.000/Thùng
 * 
 * 2. LOGIC X? LÝ T?NKHO:
 *    - Tìm t?t c? kho có lô lo-001:
 *      * Kho A: 20 Chai
 *      * Kho B: 40 Chai
 *      * T?ng: 60 Chai ? ??
 * 
 * 3. PHÂN B? T? CÁC KHO:
 *    - C?n: 48 Chai
 *    - Kho A: L?y min(48, 20) = 20 Chai
 *    - Còn l?i: 48 - 20 = 28 Chai
 *    - Kho B: L?y min(28, 40) = 28 Chai
 * 
 * 4. RESPONSE CÓ 2 CHI TI?T:
 *    - Chi ti?t 1: MaKho = Kho A, SoLuong = 20
 *    - Chi ti?t 2: MaKho = Kho B, SoLuong = 28
 * 
 * 5. C?P NH?T T?N KHO:
 *    - Kho A: 20 - 20 = 0 Chai
 *    - Kho B: 40 - 28 = 12 Chai
 * 
 * 6. TÍNH TOÁN TI?N:
 *    - T?ng ti?n hàng: 1.200.000 (2 Thùng × 600.000/Thùng)
 *    - Chi?t kh?u: 0
 *    - Thành ti?n: 1.200.000
 *    - Tính n?:
 *      * N?u ti?n m?t ? TienNo = 0 ? DA_THANH_TOAN
 *      * N?u công n? ? TienNo = 1.200.000 - TienCoc ? CHUA_THANH_TOAN
 * 
 * 7. T?O CÔNG N? (n?u c?n):
 *    - Ch? t?o n?u HinhThucThanhToan = CONG_NO ho?c CHUYEN_KHOAN
 *    - Và TienNo > 0
 *    - C?p nh?t KhachHang.CongNoHienTai
 * 
 * ============================================================================
 * CÂU H?I PH?: THAM S? TRONG RESPONSE NGH?A LÀ GÌ?
 * ============================================================================
 * 
 * soLuong (trong chi ti?t)      = S? l??ng theo ??N V? C? S? (Chai)
 *                                 = S? Chai l?y t? kho này
 * donViBan                       = ??n v? bán (Thùng - t? request)
 * soLuongQuyDoi                  = S? l??ng quy ??i (v?n = soLuong)
 * giaVonCoSo                     = Giá v?n bình quân per ??n v? c? s?
 * thanhTienVon                   = soLuong × giaVonCoSo
 * 
 * ============================================================================
 */
