-- Nếu DB đã tồn tại thì xóa
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'VETFEED_MANAGEMENT')
BEGIN
    ALTER DATABASE VETFEED_MANAGEMENT SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE VETFEED_MANAGEMENT;
END
GO

-- Tạo lại DB
CREATE DATABASE VETFEED_MANAGEMENT;
GO

USE VETFEED_MANAGEMENT;
GO

-- =========================================
-- Bảng TaiKhoan
-- Quản lý tài khoản
-- =========================================
CREATE TABLE TaiKhoan (
    MaTK UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),   -- Khóa chính
    Email NVARCHAR(255) NOT NULL UNIQUE,                 -- Email đăng nhập
    PasswordHash NVARCHAR(255) NOT NULL,                 -- Mật khẩu đã hash bằng bcrypt
    Role NVARCHAR(20) NOT NULL                           -- Vai trò
        CHECK (Role IN ('QUAN_LY','NHAN_VIEN')),
    TrangThai NVARCHAR(20) NOT NULL DEFAULT 'HOAT_DONG'  -- Trạng thái tài khoản
        CHECK (TrangThai IN ('HOAT_DONG','KHOA')),
	HoTen NVARCHAR(100) NOT NULL,                        -- Họ tên người dùng 
	SoDienThoai NVARCHAR(15) NUll,                       -- Số điện thoại 
	AnhDaiDien NVARCHAR(500) NULL,
    NgayTao DATETIME2 DEFAULT SYSDATETIME()
);

-- =========================================
-- Bảng KhoHang
-- Quản lý các kho vật lý trong hệ thống
-- =========================================
CREATE TABLE KhoHang (
    MaKho UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),   -- Khóa chính kho
    MaKhoCode NVARCHAR(50) NOT NULL UNIQUE,               -- Mã kho hiển thị (KHO01, KHO_HCM...)
    TenKho NVARCHAR(255) NOT NULL,                         -- Tên kho
    DiaChi NVARCHAR(500) NOT NULL,                         -- Địa chỉ kho
    TrangThai NVARCHAR(20) NOT NULL                        -- Trạng thái hoạt động kho
        CHECK (TrangThai IN ('HOAT_DONG','NGUNG_HOAT_DONG')),
    GhiChu NVARCHAR(500),                                 -- Ghi chú thêm
    NgayTao DATETIME2 DEFAULT SYSDATETIME()                -- Ngày tạo kho
);

-- =========================================
-- Bảng NhaCungCap
-- Quản lý các nhà cung cấp hàng hóa
-- =========================================
CREATE TABLE NhaCungCap (
    MaNCC UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),   -- Khóa chính NCC
    MaNCCCode NVARCHAR(50) NOT NULL UNIQUE,               -- Mã NCC
    TenNCC NVARCHAR(255) NOT NULL,                         -- Tên NCC
    SoDienThoai NVARCHAR(20) NOT NULL UNIQUE,              -- SĐT liên hệ
    DiaChi NVARCHAR(500),                                 -- Địa chỉ NCC
    TrangThai NVARCHAR(20)                                -- Trạng thái hoạt động
        CHECK (TrangThai IN ('HOAT_DONG','NGUNG_HOAT_DONG')),
    GhiChu NVARCHAR(500),                                 -- Ghi chú
    NgayTao DATETIME2 DEFAULT SYSDATETIME()                -- Ngày tạo
);

-- =========================================
-- Bảng KhachHang
-- Quản lý thông tin khách hàng và hạn mức công nợ
-- =========================================
CREATE TABLE KhachHang (
    MaKH UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),    -- Khóa chính KH
    MaKHCode NVARCHAR(50) NOT NULL UNIQUE,                -- Mã khách hàng
    TenKH NVARCHAR(255) NOT NULL,                          -- Tên khách hàng
    SoDienThoai NVARCHAR(20) UNIQUE,                       -- SĐT khách hàng
    DiaChi NVARCHAR(500),                                 -- Địa chỉ
    LoaiKhachHang NVARCHAR(20)                             -- Phân loại KH
        CHECK (LoaiKhachHang IN ('CA_NHAN','TRANG_TRAI','DAI_LY')),
    HanMucCongNo DECIMAL(18,2),                            -- Hạn mức công nợ tối đa
    TongMua DECIMAL(18,2) NOT NULL DEFAULT 0               -- Tổng giá trị đã mua
        CHECK (TongMua >= 0),
    CongNoHienTai DECIMAL(18,2) NOT NULL DEFAULT 0,        -- Công nợ hiện tại (cache)
    TrangThai NVARCHAR(20)                                 -- Trạng thái KH
        CHECK (TrangThai IN ('HOAT_DONG','KHOA')),
    GhiChu NVARCHAR(500),                                  -- Ghi chú
    NgayTao DATETIME2 DEFAULT SYSDATETIME()                -- Ngày tạo KH
);

-- =========================================
-- Bảng SanPham
-- Danh mục sản phẩm (thuốc thú y / TĂCN)
-- =========================================
CREATE TABLE SanPham (
    MaSP UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),    -- Khóa chính SP
    MaSPCode NVARCHAR(50) NOT NULL UNIQUE,                -- Mã sản phẩm
    TenSP NVARCHAR(255) NOT NULL,                          -- Tên sản phẩm
    LoaiSanPham NVARCHAR(30)                               -- Phân loại SP
        CHECK (LoaiSanPham IN ('THUOC_THU_Y','THUC_AN_CHAN_NUOI')),
    DonViTinh NVARCHAR(20),                                -- Đơn vị tính
    GhiChu NVARCHAR(500),                                  -- Ghi chú
    NgayTao DATETIME2 DEFAULT SYSDATETIME()                -- Ngày tạo
);

-- =========================================
-- Bảng QuyDoiDonVi
-- Quy đổi các đơn vị của từng loại sản phẩm
-- =========================================
CREATE TABLE QuyDoiDonVi (
    MaQD UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MaSP UNIQUEIDENTIFIER NOT NULL,       -- Sản phẩm
    DonViNhap NVARCHAR(50) NOT NULL,      -- Đơn vị nhập (Thùng/Hộp)
    TyLe DECIMAL(18,2) NOT NULL,          -- 1 DonViNhap = TyLe DonViTinh (đơn vị chuẩn)  ( 1 thùng = 12 hộp )

    CONSTRAINT FK_QD_SP FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP),
    CONSTRAINT UQ_QD UNIQUE (MaSP, DonViNhap) -- tránh trùng đơn vị nhập cho cùng SP
);


-- =========================================
-- Bảng NhaCungCapSanPham
-- Lưu danh sách sản phẩm mà mỗi nhà cung cấp có thể cung cấp
-- =========================================
CREATE TABLE NhaCungCapSanPham (
    MaNCSP UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    -- Khóa chính

    MaNCC UNIQUEIDENTIFIER NOT NULL,
    -- Nhà cung cấp

    MaSP UNIQUEIDENTIFIER NOT NULL,
    -- Sản phẩm có thể cung cấp

    GiaNhapMacDinh DECIMAL(18,2) NULL CHECK (GiaNhapMacDinh >= 0),
    -- Giá nhập tham khảo (không bắt buộc, chỉ để gợi ý)

    TrangThai NVARCHAR(20) NOT NULL DEFAULT 'HOAT_DONG'
        CHECK (TrangThai IN ('HOAT_DONG','NGUNG_HOAT_DONG')),
    -- NCC còn cung cấp SP này hay không

    GhiChu NVARCHAR(500),

    NgayTao DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_NCSP_NCC FOREIGN KEY (MaNCC)
        REFERENCES NhaCungCap(MaNCC),

    CONSTRAINT FK_NCSP_SP FOREIGN KEY (MaSP)
        REFERENCES SanPham(MaSP),

    CONSTRAINT UQ_NCSP UNIQUE (MaNCC, MaSP)
    -- 1 NCC không được khai báo trùng 1 SP
);
CREATE INDEX IDX_NCSP_NCC ON NhaCungCapSanPham(MaNCC);
CREATE INDEX IDX_NCSP_SP ON NhaCungCapSanPham(MaSP);

-- =========================
-- BẢNG GIÁ BÁN SẢN PHẨM (THEO THỜI GIAN) -- Khi code logic phải check kỹ để không bị overlap khi chèn giá
-- =========================
CREATE TABLE GiaBan (
    MaGia UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), 
    -- Khóa chính giá bán

    MaSP UNIQUEIDENTIFIER NOT NULL,                   
    -- Sản phẩm áp dụng giá

    DonGiaBan DECIMAL(18,2) NOT NULL CHECK (DonGiaBan >= 0),
    -- Giá bán tại thời điểm hiệu lực

    TuNgay  DATETIME2  NOT NULL,   --datetime                          
    -- Ngày bắt đầu áp dụng giá

    DenNgay  DATETIME2  NULL,   --datetime                             
    -- Ngày kết thúc áp dụng giá (NULL = còn hiệu lực)

    GhiChu NVARCHAR(500),                             
    -- Ghi chú (lý do đổi giá, khuyến mãi...)

    NgayTao DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_GiaBan_SP FOREIGN KEY (MaSP)
        REFERENCES SanPham(MaSP),

    CONSTRAINT CK_GiaBan_ThoiGian 
        CHECK (DenNgay IS NULL OR DenNgay >= TuNgay)
);
CREATE INDEX IDX_GiaBan_MaSP ON GiaBan(MaSP);
CREATE INDEX IDX_GiaBan_ThoiGian ON GiaBan(MaSP, TuNgay, DenNgay);


-- =========================================
-- Bảng LoHang
-- Quản lý lô sản phẩm theo hạn sử dụng
-- =========================================
CREATE TABLE LoHang (
    MaLo UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),    -- Khóa chính lô
    MaLoCode NVARCHAR(100) NOT NULL,                      -- Mã lô (do NCC cung cấp)
    MaSP UNIQUEIDENTIFIER NOT NULL,                       -- Sản phẩm thuộc lô
    NgaySanXuat DATE,                                     -- Ngày sản xuất
    HanSuDung DATE NOT NULL,                               -- Hạn sử dụng

	-- kiểm tra hạn sử dụng 
	CONSTRAINT CK_LoHang_HSD CHECK (HanSuDung >= NgaySanXuat OR NgaySanXuat IS NULL),

    CONSTRAINT FK_LoHang_SP FOREIGN KEY (MaSP)
        REFERENCES SanPham(MaSP),

    CONSTRAINT UQ_LoHang UNIQUE (MaSP, MaLoCode)           -- Mỗi SP không trùng mã lô
);

-- Index hỗ trợ truy vấn theo hạn sử dụng
CREATE INDEX IDX_LoHang_HSD ON LoHang(HanSuDung);

-- Index hỗ trợ join theo sản phẩm
CREATE INDEX IDX_LoHang_MaSP ON LoHang(MaSP);


-- =========================================
-- Bảng TonKho
-- Quản lý tồn kho theo kho + lô
-- =========================================
CREATE TABLE TonKho (
    MaTonKho UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- Khóa chính
    MaKho UNIQUEIDENTIFIER NOT NULL,                        -- Kho
    MaLo UNIQUEIDENTIFIER NOT NULL,                         -- Lô hàng
    SoLuong DECIMAL(18,2) NOT NULL                          -- Số lượng tồn
        CHECK (SoLuong >= 0),
    NgayCapNhat DATETIME2 NOT NULL DEFAULT SYSDATETIME(),   -- Lần cập nhật gần nhất

    CONSTRAINT FK_TonKho_Kho FOREIGN KEY (MaKho)
        REFERENCES KhoHang(MaKho),

    CONSTRAINT FK_TonKho_Lo FOREIGN KEY (MaLo)
        REFERENCES LoHang(MaLo),

    CONSTRAINT UQ_TonKho UNIQUE (MaKho, MaLo)               -- Mỗi kho + lô chỉ 1 dòng
);

-- Index truy vấn tồn kho theo kho
CREATE INDEX IDX_TonKho_MaKho ON TonKho(MaKho);

-- Index truy vấn tồn kho theo lô
CREATE INDEX IDX_TonKho_MaLo ON TonKho(MaLo);


-- =========================
-- PHIẾU NHẬP HÀNG
-- =========================
CREATE TABLE PhieuNhap (
    MaPN UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- Khóa chính phiếu nhập
    MaPNCode NVARCHAR(50) NOT NULL UNIQUE,             -- Mã phiếu nhập hiển thị cho người dùng
    MaNCC UNIQUEIDENTIFIER NOT NULL,                   -- Nhà cung cấp
    MaKho UNIQUEIDENTIFIER NOT NULL,                   -- Kho nhập hàng
    ThanhTien DECIMAL(18,2) NOT NULL DEFAULT 0,        -- Tổng tiền phiếu nhập
    TrangThai NVARCHAR(20)
        CHECK (TrangThai IN ('DA_DAT','DA_NHAN','DA_HUY')), -- Trạng thái nghiệp vụ
    GhiChu NVARCHAR(500),                              -- Ghi chú thêm
    NgayCapNhat DATETIME2 NOT NULL DEFAULT SYSDATETIME(), -- Thời điểm cập nhật gần nhất

    CONSTRAINT FK_PN_NCC FOREIGN KEY (MaNCC)
        REFERENCES NhaCungCap(MaNCC),

    CONSTRAINT FK_PN_Kho FOREIGN KEY (MaKho)
        REFERENCES KhoHang(MaKho)
);

-- Truy vấn theo thời gian
CREATE INDEX IDX_PN_Ngay ON PhieuNhap(NgayCapNhat);

-- Truy vấn theo nhà cung cấp
CREATE INDEX IDX_PN_NCC ON PhieuNhap(MaNCC);


-- =========================
-- CHI TIẾT PHIẾU NHẬP
-- =========================
CREATE TABLE CTPhieuNhap (
    MaCTPN UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- Khóa chính
    MaPN UNIQUEIDENTIFIER NOT NULL,                      -- Phiếu nhập cha
    MaLo UNIQUEIDENTIFIER NOT NULL,                      -- Lô hàng
    SoLuong DECIMAL(18,2) NOT NULL CHECK (SoLuong > 0),  -- Số lượng nhập
    DonGia DECIMAL(18,2),                                -- Đơn giá nhập

    CONSTRAINT FK_CTPN_PN FOREIGN KEY (MaPN)
        REFERENCES PhieuNhap(MaPN)
        ON DELETE CASCADE, -- Xóa phiếu → xóa chi tiết

    CONSTRAINT FK_CTPN_Lo FOREIGN KEY (MaLo)
        REFERENCES LoHang(MaLo),

    CONSTRAINT UQ_CTPN UNIQUE (MaPN, MaLo) -- 1 lô chỉ xuất hiện 1 lần trong phiếu
);

CREATE INDEX IDX_CTPN_PN ON CTPhieuNhap(MaPN);
CREATE INDEX IDX_CTPN_MaLo ON CTPhieuNhap(MaLo);


-- =========================
-- PHIẾU BÁN HÀNG
-- =========================
CREATE TABLE PhieuBan (
    MaPB UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- Khóa chính
    MaPBCode NVARCHAR(50) NOT NULL UNIQUE,             -- Mã phiếu bán
    MaKH UNIQUEIDENTIFIER NOT NULL,                    -- Khách hàng
    NgayBan DATETIME2 NOT NULL DEFAULT SYSDATETIME(),  -- Ngày bán

    TongTienHang DECIMAL(18,2) NOT NULL DEFAULT 0,     -- Tổng tiền hàng
    ChietKhauPhanTram DECIMAL(5,2) NOT NULL DEFAULT 0, -- % chiết khấu
    TienChietKhau DECIMAL(18,2) NOT NULL DEFAULT 0,    -- Tiền chiết khấu
    ThanhTien DECIMAL(18,2) NOT NULL DEFAULT 0,        -- Tổng thanh toán

    HinhThucThanhToan NVARCHAR(20)
        CHECK (HinhThucThanhToan IN ('TIEN_MAT','CHUYEN_KHOAN','CONG_NO')),

    TrangThaiThanhToan NVARCHAR(30)
        CHECK (TrangThaiThanhToan IN ('CHUA_THANH_TOAN','DA_THANH_TOAN')),

    TienCoc DECIMAL(18,2) NOT NULL DEFAULT 0,          -- Tiền cọc
    TienNo DECIMAL(18,2) NOT NULL DEFAULT 0,           -- Số tiền còn nợ
    HanTra DATE,                                       -- Hạn thanh toán
    GhiChu NVARCHAR(MAX),

    CONSTRAINT FK_PB_KH FOREIGN KEY (MaKH)
        REFERENCES KhachHang(MaKH)
);

CREATE INDEX IDX_PB_Ngay ON PhieuBan(NgayBan);
CREATE INDEX IDX_PB_KH ON PhieuBan(MaKH);



-- =========================
-- CHI TIẾT PHIẾU BÁN
-- =========================
CREATE TABLE CTPhieuBan (
    MaCTPB UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),

    MaPB UNIQUEIDENTIFIER NOT NULL, -- Phiếu bán
    MaKho UNIQUEIDENTIFIER NOT NULL, -- Kho xuất
    MaLo UNIQUEIDENTIFIER NOT NULL,  -- Lô xuất

    SoLuong DECIMAL(18,2) NOT NULL CHECK (SoLuong > 0),
    DonGia DECIMAL(18,2) NOT NULL CHECK (DonGia >= 0),

    GhiChu NVARCHAR(500),

    CONSTRAINT FK_CTPB_PB FOREIGN KEY (MaPB)
        REFERENCES PhieuBan(MaPB)
        ON DELETE CASCADE,

    CONSTRAINT FK_CTPB_Kho FOREIGN KEY (MaKho)
        REFERENCES KhoHang(MaKho),

    CONSTRAINT FK_CTPB_Lo FOREIGN KEY (MaLo)
        REFERENCES LoHang(MaLo),

    CONSTRAINT UQ_CTPB UNIQUE (MaPB, MaKho, MaLo)
);

CREATE INDEX IDX_CTPB_PB ON CTPhieuBan (MaPB);
CREATE INDEX IDX_CTPB_MaKho ON CTPhieuBan(MaKho);
CREATE INDEX IDX_CTPB_MaLo ON CTPhieuBan(MaLo);



-- =========================
-- PHIẾU CHUYỂN KHO
-- =========================
CREATE TABLE PhieuChuyenKho (
    MaCK UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- Phiếu chuyển kho
    MaCKCode NVARCHAR(50) NOT NULL UNIQUE,             -- Mã hiển thị

    NgayLap DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    MaKhoXuat UNIQUEIDENTIFIER NOT NULL, -- Kho xuất
    MaKhoNhan UNIQUEIDENTIFIER NOT NULL, -- Kho nhận

    GhiChu NVARCHAR(500),

    CONSTRAINT FK_PCK_KhoXuat FOREIGN KEY (MaKhoXuat)
        REFERENCES KhoHang(MaKho),

    CONSTRAINT FK_PCK_KhoNhan FOREIGN KEY (MaKhoNhan)
        REFERENCES KhoHang(MaKho),

    CONSTRAINT CK_PCK_Kho CHECK (MaKhoXuat <> MaKhoNhan)
);

CREATE INDEX IDX_PCK_NgayLap ON PhieuChuyenKho(NgayLap);
CREATE INDEX IDX_PCK_KhoXuat ON PhieuChuyenKho(MaKhoXuat);
CREATE INDEX IDX_PCK_KhoNhan ON PhieuChuyenKho(MaKhoNhan);

-- =========================
-- CHI TIẾT PHIẾU CHUYỂN KHO
-- =========================
CREATE TABLE CTPhieuChuyenKho (
    MaCTCK UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),

    MaCK UNIQUEIDENTIFIER NOT NULL, -- Phiếu chuyển
    MaLo UNIQUEIDENTIFIER NOT NULL, -- Lô chuyển

    SoLuongChuyen DECIMAL(18,2) NOT NULL CHECK (SoLuongChuyen > 0),

    TrangThai NVARCHAR(20)
        CHECK (TrangThai IN ('TAO','DANG_CHUYEN','DA_NHAN')),

    GhiChu NVARCHAR(500),

    CONSTRAINT FK_CTCK_CK FOREIGN KEY (MaCK)
        REFERENCES PhieuChuyenKho(MaCK)
        ON DELETE CASCADE,

    CONSTRAINT FK_CTCK_Lo FOREIGN KEY (MaLo)
        REFERENCES LoHang(MaLo),

    CONSTRAINT UQ_CTCK UNIQUE (MaCK, MaLo)
);


-- =========================
-- PHIẾU TRẢ HÀNG (KHÁCH TRẢ)
-- =========================
CREATE TABLE PhieuTra (
    MaPT UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	MaPTCode NVARCHAR(50) NOT NULL UNIQUE,
    NgayTra DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    MaPB UNIQUEIDENTIFIER NOT NULL, -- Phiếu bán gốc
    MaKH UNIQUEIDENTIFIER NOT NULL, -- Khách hàng
    LyDo NVARCHAR(500),              -- Lý do trả
    ThanhTien DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (ThanhTien >= 0),
    HinhThucHoanTien NVARCHAR(20)
        CHECK (HinhThucHoanTien IN ('TIEN_MAT','CHUYEN_KHOAN')),

    CONSTRAINT FK_PT_PB FOREIGN KEY (MaPB)
        REFERENCES PhieuBan(MaPB),

    CONSTRAINT FK_PT_KH FOREIGN KEY (MaKH)
        REFERENCES KhachHang(MaKH)
);

CREATE INDEX IDX_PT_NgayTra ON PhieuTra(NgayTra);
CREATE INDEX IDX_PT_PB ON PhieuTra(MaPB);
CREATE INDEX IDX_PT_KH ON PhieuTra(MaKH);


-- =========================
-- CHI TIẾT PHIẾU TRẢ HÀNG
-- =========================
CREATE TABLE CTPhieuTra (
    MaCTPT UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),

    MaPT UNIQUEIDENTIFIER NOT NULL, -- Phiếu trả
    MaLo UNIQUEIDENTIFIER NOT NULL, -- Lô trả

    SoLuongTra DECIMAL(18,2) NOT NULL CHECK (SoLuongTra > 0),

    DonGiaHoan DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (DonGiaHoan >= 0),

    GhiChu NVARCHAR(500),

    CONSTRAINT FK_CTPT_PT FOREIGN KEY (MaPT)
        REFERENCES PhieuTra(MaPT)
        ON DELETE CASCADE,

    CONSTRAINT FK_CTPT_Lo FOREIGN KEY (MaLo)
        REFERENCES LoHang(MaLo),

    CONSTRAINT UQ_CTPT UNIQUE (MaPT, MaLo)
);

CREATE INDEX IDX_CTPT_MaPT ON CTPhieuTra(MaPT);
CREATE INDEX IDX_CTPT_MaLo ON CTPhieuTra(MaLo);



-- =========================================
-- Bảng CongNo
-- Ghi nhận các phát sinh công nợ (tăng / giảm)
-- Mỗi dòng là một nghiệp vụ độc lập
-- Tổng công nợ = SUM(SoTien) theo từng đối tượng
-- =========================================
CREATE TABLE CongNo (
    MaCongNo UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- Khóa chính

    LoaiDoiTuong NVARCHAR(20) NOT NULL                      -- Loại đối tượng công nợ
        CHECK (LoaiDoiTuong IN ('KHACH_HANG','NHA_CUNG_CAP')),

    MaDoiTuong UNIQUEIDENTIFIER NOT NULL,                  -- MaKH hoặc MaNCC

    MaPhieu UNIQUEIDENTIFIER NULL,                          -- Phiếu liên quan (PB/PN/PT...)

    SoTien DECIMAL(18,2) NOT NULL                           -- Số tiền phát sinh
        CHECK (SoTien <> 0),                                -- >0 tăng nợ, <0 giảm nợ

    NgayPhatSinh DATETIME2 NOT NULL DEFAULT SYSDATETIME(),  -- Ngày phát sinh

    HanThanhToan DATE NULL,                                 -- Hạn thanh toán (nợ tăng)

    GhiChu NVARCHAR(500)                                    -- Ghi chú
);

-- Index truy vấn công nợ theo đối tượng
CREATE INDEX IDX_CongNo_DoiTuong
ON CongNo (LoaiDoiTuong, MaDoiTuong);

-- Index phục vụ truy vấn nợ quá hạn
CREATE INDEX IDX_CongNo_HanThanhToan
ON CongNo (HanThanhToan);

-- Index truy vấn theo thời gian
CREATE INDEX IDX_CongNo_Ngay
ON CongNo (NgayPhatSinh);

-- Index truy vấn theo phiếu
CREATE INDEX IDX_CongNo_MaPhieu
ON CongNo (MaPhieu);





-- ===== Kho hàng =====
INSERT INTO KhoHang (MaKho, MaKhoCode, TenKho, DiaChi, TrangThai)
VALUES
(NEWID(), 'KHO01', N'Kho Thủ Đức', N'123 Xa lộ Hà Nội, TP.HCM', 'HOAT_DONG'),
(NEWID(), 'KHO02', N'Kho Tân Phú', N'456 Lũy Bán Bích, TP.HCM', 'HOAT_DONG'),
(NEWID(), 'KHO03', N'Kho Củ Chi', N'789 Tỉnh lộ 8, TP.HCM', 'HOAT_DONG');

-- ===== Nhà cung cấp =====
INSERT INTO NhaCungCap (MaNCC, MaNCCCode, TenNCC, SoDienThoai, DiaChi, TrangThai)
VALUES
(NEWID(), 'NCC01', N'Công ty Dược ABC', '0909123456', N'Quận 1, TP.HCM', 'HOAT_DONG'),
(NEWID(), 'NCC02', N'Công ty Thức ăn XYZ', '0909234567', N'Quận 5, TP.HCM', 'HOAT_DONG');

-- ===== Khách hàng =====
INSERT INTO KhachHang (MaKH, MaKHCode, TenKH, SoDienThoai, DiaChi, LoaiKhachHang, HanMucCongNo, TrangThai)
VALUES
(NEWID(), 'KH01', N'Trại heo Bình Dương', '0912345678', N'Bình Dương', 'TRANG_TRAI', 50000000, 'HOAT_DONG'),
(NEWID(), 'KH02', N'Đại lý thuốc thú y A', '0912345679', N'Quận 9, TP.HCM', 'DAI_LY', 20000000, 'HOAT_DONG');

-- ===== Sản phẩm =====
INSERT INTO SanPham (MaSP, MaSPCode, TenSP, LoaiSanPham, DonViTinh)
VALUES
(NEWID(), 'SP01', N'Antibiotic', 'THUOC_THU_Y', N'Vỉ'),
(NEWID(), 'SP02', N'Peptide', 'THUOC_THU_Y', N'Vỉ'),
(NEWID(), 'SP03', N'Cám heo', 'THUC_AN_CHAN_NUOI', N'Kg');

-- ===== Quy đổi đơn vị =====
INSERT INTO QuyDoiDonVi (MaQD, MaSP, DonViNhap, TyLe)
VALUES
(NEWID(), (SELECT MaSP FROM SanPham WHERE MaSPCode='SP01'), 'Thùng', 50),
(NEWID(), (SELECT MaSP FROM SanPham WHERE MaSPCode='SP01'), 'Hộp', 5),
(NEWID(), (SELECT MaSP FROM SanPham WHERE MaSPCode='SP02'), 'Thùng', 40);

-- ===== Lô hàng =====
INSERT INTO LoHang (MaLo, MaLoCode, MaSP, NgaySanXuat, HanSuDung)
VALUES
(NEWID(), 'LO01', (SELECT MaSP FROM SanPham WHERE MaSPCode='SP01'), '2025-01-01', '2026-01-01'),
(NEWID(), 'LO02', (SELECT MaSP FROM SanPham WHERE MaSPCode='SP02'), '2025-02-01', '2026-02-01'),
(NEWID(), 'LO03', (SELECT MaSP FROM SanPham WHERE MaSPCode='SP03'), '2025-03-01', '2026-03-01');

-- ===== Phiếu nhập + chi tiết =====
INSERT INTO PhieuNhap (MaPN, MaPNCode, MaNCC, MaKho, ThanhTien, TrangThai)
VALUES
(NEWID(), 'PN1001', (SELECT MaNCC FROM NhaCungCap WHERE MaNCCCode='NCC01'), (SELECT MaKho FROM KhoHang WHERE MaKhoCode='KHO01'), 1000000, 'DA_NHAN');

INSERT INTO CTPhieuNhap (MaCTPN, MaPN, MaLo, SoLuong, DonGia)
VALUES
(NEWID(), (SELECT MaPN FROM PhieuNhap WHERE MaPNCode='PN1001'), (SELECT MaLo FROM LoHang WHERE MaLoCode='LO01'), 100, 20000),
(NEWID(), (SELECT MaPN FROM PhieuNhap WHERE MaPNCode='PN1001'), (SELECT MaLo FROM LoHang WHERE MaLoCode='LO02'), 50, 30000);

-- ===== Tồn kho =====
INSERT INTO TonKho (MaTonKho, MaKho, MaLo, SoLuong)
VALUES
(NEWID(), (SELECT MaKho FROM KhoHang WHERE MaKhoCode='KHO01'), (SELECT MaLo FROM LoHang WHERE MaLoCode='LO01'), 100),
(NEWID(), (SELECT MaKho FROM KhoHang WHERE MaKhoCode='KHO01'), (SELECT MaLo FROM LoHang WHERE MaLoCode='LO02'), 50);

-- ===== Phiếu bán + chi tiết =====
INSERT INTO PhieuBan (MaPB, MaPBCode, MaKH, TongTienHang, ThanhTien, HinhThucThanhToan, TrangThaiThanhToan)
VALUES
(NEWID(), 'PB2001', (SELECT MaKH FROM KhachHang WHERE MaKHCode='KH01'), 200000, 200000, 'TIEN_MAT', 'DA_THANH_TOAN');

INSERT INTO CTPhieuBan (MaCTPB, MaPB, MaKho, MaLo, SoLuong, DonGia)
VALUES
(NEWID(), (SELECT MaPB FROM PhieuBan WHERE MaPBCode='PB2001'), (SELECT MaKho FROM KhoHang WHERE MaKhoCode='KHO01'), (SELECT MaLo FROM LoHang WHERE MaLoCode='LO01'), 10, 20000);

-- ===== Phiếu chuyển kho + chi tiết =====
INSERT INTO PhieuChuyenKho (MaCK, MaCKCode, NgayLap, MaKhoXuat, MaKhoNhan, GhiChu)
VALUES
(NEWID(), 'CK3001', '2025-08-11',
 (SELECT MaKho FROM KhoHang WHERE MaKhoCode='KHO01'),
 (SELECT MaKho FROM KhoHang WHERE MaKhoCode='KHO02'),
 N'Chuyển sang Tân Phú');

INSERT INTO CTPhieuChuyenKho (MaCTCK, MaCK, MaLo, SoLuongChuyen, TrangThai, GhiChu)
VALUES
(NEWID(),
 (SELECT MaCK FROM PhieuChuyenKho WHERE MaCKCode='CK3001'),
 (SELECT MaLo FROM LoHang WHERE MaLoCode='LO01'),
 20, 'DA_NHAN', N'Đã nhận tại Tân Phú');

-- ===== Phiếu trả hàng + chi tiết =====
INSERT INTO PhieuTra (MaPT, MaPTCode, NgayTra, MaPB, MaKH, LyDo, ThanhTien, HinhThucHoanTien)
VALUES
(NEWID(), 'PT4001', '2025-09-01',
 (SELECT MaPB FROM PhieuBan WHERE MaPBCode='PB2001'),
 (SELECT MaKH FROM KhachHang WHERE MaKHCode='KH01'),
 N'Hàng lỗi', 40000, 'TIEN_MAT');

INSERT INTO CTPhieuTra (MaCTPT, MaPT, MaLo, SoLuongTra, DonGiaHoan)
VALUES
(NEWID(),
 (SELECT MaPT FROM PhieuTra WHERE MaPTCode='PT4001'),
 (SELECT MaLo FROM LoHang WHERE MaLoCode='LO01'),
 2, 20000);

-- ===== Công nợ =====
INSERT INTO CongNo (MaCongNo, LoaiDoiTuong, MaDoiTuong, MaPhieu, SoTien, NgayPhatSinh, GhiChu)
VALUES
(NEWID(), 'KHACH_HANG', (SELECT MaKH FROM KhachHang WHERE MaKHCode='KH01'),
 (SELECT MaPB FROM PhieuBan WHERE MaPBCode='PB2001'),
 200000, GETDATE(), N'Phát sinh công nợ từ phiếu bán PB2001');
