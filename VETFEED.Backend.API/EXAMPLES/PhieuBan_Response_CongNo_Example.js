/**
 * SCENARIO 2: BÁN B?NG CÔNG N? - T?O CÔNG N? T? ??NG
 * 
 * REQUEST:
 * POST /api/phieubans
 */
{
  "maKH": "550e8400-e29b-41d4-a716-446655440000",
  "ngayBan": "2024-01-15",
  "chieTKhauPhanTram": 10,        // Chi?t kh?u 10%
  "hinhThucThanhToan": "CONG_NO", // BÁN B?NG CÔNG N?
  "tienCoc": 200000,              // Khách c?c tr??c 200.000
  "hanTra": "2024-02-15",         // H?n tr? là 15/2/2024
  "ghiChu": "Khách hàng VIP",
  "danhSachChiTiet": [
    {
      "maLo": "lo-001",
      "soLuong": 2,
      "donViBan": "Thùng",
      "donGia": 600000,
      "ghiChu": "N??c mía"
    }
  ]
}

/**
 * RESPONSE:
 * Status: 201 Created
 */
{
  "maPB": "550e8400-e29b-41d4-a716-446655440002",
  "maPBCode": "PB00000002",
  "maKH": "550e8400-e29b-41d4-a716-446655440000",
  "tenKhachHang": "Công ty ABC",
  "ngayBan": "2024-01-15T00:00:00",
  
  // ===== TÍNH TOÁN TI?N V?I CHI?T KH?U =====
  "tongTienHang": 1200000,        // 2 Thùng × 600.000 = 1.200.000
  "chieTKhauPhanTram": 10,        // Chi?t kh?u 10%
  "tienChietKhau": 120000,        // 1.200.000 × 10% = 120.000
  "thanhTien": 1080000,           // 1.200.000 - 120.000 = 1.080.000
  "hinhThucThanhToan": "CONG_NO",
  "trangThaiThanhToan": "CHUA_THANH_TOAN", // ?? CH?A THANH TOÁN - CÓ N?
  "tienCoc": 200000,              // C?c 200.000
  "tienNo": 880000,               // 1.080.000 - 200.000 = 880.000 VND N?
  "hanTra": "2024-02-15T00:00:00",
  "ghiChu": "Khách hàng VIP",
  
  // ===== CHI TI?T PHI?U BÁN (GI?NG NH? TRÊN) =====
  "danhSachChiTiet": [
    {
      "maCTPB": "550e8400-e29b-41d4-a716-446655440020",
      "maKho": "kho-a-guid-123",
      "tenKho": "Kho A",
      "maLo": "lo-001",
      "maLoCode": "LH000001",
      "tenSanPham": "N??c mía",
      "donViCoSo": "Chai",
      "soLuong": 20,
      "donViBan": "Thùng",
      "donGia": 600000,
      "soLuongQuyDoi": 20,
      "giaVonCoSo": 25000,
      "thanhTienVon": 500000,
      "hanSuDung": "2024-12-31T00:00:00",
      "ghiChu": "N??c mía"
    },
    {
      "maCTPB": "550e8400-e29b-41d4-a716-446655440021",
      "maKho": "kho-b-guid-456",
      "tenKho": "Kho B",
      "maLo": "lo-001",
      "maLoCode": "LH000001",
      "tenSanPham": "N??c mía",
      "donViCoSo": "Chai",
      "soLuong": 28,
      "donViBan": "Thùng",
      "donGia": 600000,
      "soLuongQuyDoi": 28,
      "giaVonCoSo": 20000,
      "thanhTienVon": 560000,
      "hanSuDung": "2024-12-31T00:00:00",
      "ghiChu": "N??c mía"
    }
  ]
}

/**
 * ===== BACKEND T? ??NG T?O CÔNG N? =====
 * 
 * VÌ HinhThucThanhToan = CONG_NO VÀ TienNo = 880.000
 * 
 * T?o record CongNo:
 * {
 *   "maCongNo": "550e8400-e29b-41d4-a716-446655440099",
 *   "loaiDoiTuong": "KHACH_HANG",      // ? Lo?i công n?
 *   "maDoiTuong": "550e8400-...",       // ? MaKH
 *   "maPhieu": "550e8400-...-0002",    // ? MaPB (liên k?t)
 *   "soTien": 880000,                   // ? S? ti?n n?
 *   "ngayPhatSinh": "2024-01-15T...",  // ? Hôm nay
 *   "hanThanhToan": "2024-02-15T...",  // ? HanTra
 *   "ghiChu": "Công n? t? phi?u bán PB00000002"
 * }
 * 
 * C?p nh?t KhachHang:
 * - KhachHang.CongNoHienTai += 880.000
 * - Ví d?: Tr??c 500.000 ? Sau 1.380.000
 */

/**
 * ===== FLOW LOGIC CHI TI?T =====
 * 
 * 1. User g?i request bán 2 thùng (48 chai)
 * 
 * 2. Ki?m tra t?n kho:
 *    - Tìm t?t c? TonKho có MaLo = lo-001
 *    - Kho A: 20 chai
 *    - Kho B: 40 chai
 *    - T?ng: 60 chai ?
 * 
 * 3. S?p x?p: ?u tiên l?y t? kho có ít hàng tr??c
 *    - Kho A (20) ? Kho B (40)
 * 
 * 4. T?o chi ti?t phi?u bán:
 *    Dòng 1: Kho A - 20 chai (h?t t?n kho Kho A)
 *    Dòng 2: Kho B - 28 chai (20 + 28 = 48)
 * 
 * 5. Tr? t?n kho:
 *    - Kho A: 20 - 20 = 0
 *    - Kho B: 40 - 28 = 12
 * 
 * 6. Tính ti?n:
 *    - TongTienHang = 1.200.000
 *    - TienChietKhau = 120.000
 *    - ThanhTien = 1.080.000
 *    - TienNo = 880.000 (1.080.000 - 200.000 c?c)
 * 
 * 7. T?O CÔNG N? (vì HinhThucThanhToan = CONG_NO):
 *    - Insert CongNo record
 *    - Update KhachHang.CongNoHienTai
 * 
 * 8. Tr? v? response v?i 2 chi ti?t phi?u bán
 */
