using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Models;
using VETFEED.Backend.API.Enums;

namespace VETFEED.Backend.API.Data
{
    public class VetFeedManagementContext : DbContext
    {
        public VetFeedManagementContext(DbContextOptions<VetFeedManagementContext> options) : base(options) { }

        // DbSet cho tất cả các bảng
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<KhoHang> KhoHangs { get; set; }
        public DbSet<NhaCungCap> NhaCungCaps { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<NhaCungCapSanPham> NhaCungCapSanPhams { get; set; }
        public DbSet<GiaBan> GiaBans { get; set; }
        public DbSet<LoHang> LoHangs { get; set; }
        public DbSet<TonKho> TonKhos { get; set; }
        public DbSet<PhieuNhap> PhieuNhaps { get; set; }
        public DbSet<CTPhieuNhap> CTPhieuNhaps { get; set; }
        public DbSet<PhieuBan> PhieuBans { get; set; }
        public DbSet<CTPhieuBan> CTPhieuBans { get; set; }
        public DbSet<PhieuChuyenKho> PhieuChuyenKhos { get; set; }
        public DbSet<CTPhieuChuyenKho> CTPhieuChuyenKhos { get; set; }
        public DbSet<PhieuTra> PhieuTras { get; set; }
        public DbSet<CTPhieuTra> CTPhieuTras { get; set; }
        public DbSet<CongNo> CongNos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map tên bảng đúng với DB
            modelBuilder.Entity<TaiKhoan>().ToTable("TaiKhoan"); 
            modelBuilder.Entity<KhoHang>().ToTable("KhoHang"); 
            modelBuilder.Entity<NhaCungCap>().ToTable("NhaCungCap"); 
            modelBuilder.Entity<KhachHang>().ToTable("KhachHang"); 
            modelBuilder.Entity<SanPham>().ToTable("SanPham"); 
            modelBuilder.Entity<NhaCungCapSanPham>().ToTable("NhaCungCapSanPham"); 
            modelBuilder.Entity<GiaBan>().ToTable("GiaBan"); 
            modelBuilder.Entity<LoHang>().ToTable("LoHang"); 
            modelBuilder.Entity<TonKho>().ToTable("TonKho"); 
            modelBuilder.Entity<PhieuNhap>().ToTable("PhieuNhap"); 
            modelBuilder.Entity<CTPhieuNhap>().ToTable("CTPhieuNhap"); 
            modelBuilder.Entity<PhieuBan>().ToTable("PhieuBan"); 
            modelBuilder.Entity<CTPhieuBan>().ToTable("CTPhieuBan"); 
            modelBuilder.Entity<PhieuChuyenKho>().ToTable("PhieuChuyenKho");
            modelBuilder.Entity<CTPhieuChuyenKho>().ToTable("CTPhieuChuyenKho"); 
            modelBuilder.Entity<PhieuTra>().ToTable("PhieuTra"); 
            modelBuilder.Entity<CTPhieuTra>().ToTable("CTPhieuTra"); 
            modelBuilder.Entity<CongNo>().ToTable("CongNo");

            // ===== Enum mapping (lưu dưới dạng string) =====
            modelBuilder.Entity<TaiKhoan>()
                .Property(t => t.Role)
                .HasConversion<string>();

            modelBuilder.Entity<TaiKhoan>()
                .Property(t => t.TrangThai)
                .HasConversion<string>();

            modelBuilder.Entity<KhoHang>()
                .Property(k => k.TrangThai)
                .HasConversion<string>();

            modelBuilder.Entity<NhaCungCap>()
                .Property(n => n.TrangThai)
                .HasConversion<string>();

            modelBuilder.Entity<KhachHang>()
                .Property(k => k.LoaiKhachHang)
                .HasConversion<string>();

            modelBuilder.Entity<KhachHang>()
                .Property(k => k.TrangThai)
                .HasConversion<string>();

            modelBuilder.Entity<SanPham>()
                .Property(sp => sp.LoaiSanPham)
                .HasConversion<string>();

            modelBuilder.Entity<NhaCungCapSanPham>()
                .Property(ncsp => ncsp.TrangThai)
                .HasConversion<string>();

            modelBuilder.Entity<PhieuNhap>()
                .Property(pn => pn.TrangThai)
                .HasConversion<string>();

            modelBuilder.Entity<PhieuBan>()
                .Property(pb => pb.HinhThucThanhToan)
                .HasConversion<string>();

            modelBuilder.Entity<PhieuBan>()
                .Property(pb => pb.TrangThaiThanhToan)
                .HasConversion<string>();

            modelBuilder.Entity<CTPhieuChuyenKho>()
                .Property(ct => ct.TrangThai)
                .HasConversion<string>();

            modelBuilder.Entity<PhieuTra>()
                .Property(pt => pt.HinhThucHoanTien)
                .HasConversion<string>();

            modelBuilder.Entity<CongNo>()
                .Property(cn => cn.LoaiDoiTuong)
                .HasConversion<string>();

            // ===== Khóa ngoại & quan hệ =====
            modelBuilder.Entity<NhaCungCapSanPham>()
                .HasOne(ncsp => ncsp.NhaCungCap)
                .WithMany(ncc => ncc.NhaCungCapSanPhams)
                .HasForeignKey(ncsp => ncsp.MaNCC);

            modelBuilder.Entity<NhaCungCapSanPham>()
                .HasOne(ncsp => ncsp.SanPham)
                .WithMany(sp => sp.NhaCungCapSanPhams)
                .HasForeignKey(ncsp => ncsp.MaSP);

            modelBuilder.Entity<GiaBan>()
                .HasOne(gb => gb.SanPham)
                .WithMany(sp => sp.GiaBans)
                .HasForeignKey(gb => gb.MaSP);

            modelBuilder.Entity<LoHang>()
                .HasOne(lh => lh.SanPham)
                .WithMany(sp => sp.LoHangs)
                .HasForeignKey(lh => lh.MaSP);

            modelBuilder.Entity<TonKho>()
                .HasOne(tk => tk.KhoHang)
                .WithMany(k => k.TonKhos)
                .HasForeignKey(tk => tk.MaKho);

            modelBuilder.Entity<TonKho>()
                .HasOne(tk => tk.LoHang)
                .WithMany(lh => lh.TonKhos)
                .HasForeignKey(tk => tk.MaLo);

            modelBuilder.Entity<PhieuNhap>()
                .HasOne(pn => pn.NhaCungCap)
                .WithMany(ncc => ncc.PhieuNhaps)
                .HasForeignKey(pn => pn.MaNCC);

            modelBuilder.Entity<PhieuNhap>()
                .HasOne(pn => pn.KhoHang)
                .WithMany(k => k.PhieuNhaps)
                .HasForeignKey(pn => pn.MaKho);

            modelBuilder.Entity<CTPhieuNhap>()
                .HasOne(ct => ct.PhieuNhap)
                .WithMany(pn => pn.CTPhieuNhaps)
                .HasForeignKey(ct => ct.MaPN);

            modelBuilder.Entity<CTPhieuNhap>()
                .HasOne(ct => ct.LoHang)
                .WithMany(lh => lh.CTPhieuNhaps)
                .HasForeignKey(ct => ct.MaLo);

            modelBuilder.Entity<PhieuBan>()
                .HasOne(pb => pb.KhachHang)
                .WithMany(kh => kh.PhieuBans)
                .HasForeignKey(pb => pb.MaKH);

            modelBuilder.Entity<CTPhieuBan>()
                .HasOne(ct => ct.PhieuBan)
                .WithMany(pb => pb.CTPhieuBans)
                .HasForeignKey(ct => ct.MaPB);

            modelBuilder.Entity<CTPhieuBan>()
                .HasOne(ct => ct.KhoHang)
                .WithMany(kh => kh.CTPhieuBans)
                .HasForeignKey(ct => ct.MaKho);

            modelBuilder.Entity<CTPhieuBan>()
                .HasOne(ct => ct.LoHang)
                .WithMany(lh => lh.CTPhieuBans)
                .HasForeignKey(ct => ct.MaLo);

            modelBuilder.Entity<PhieuChuyenKho>()
                .HasOne(pck => pck.KhoXuat)
                .WithMany(kh => kh.PhieuChuyenKhoXuats)
                .HasForeignKey(pck => pck.MaKhoXuat);

            modelBuilder.Entity<PhieuChuyenKho>()
                .HasOne(pck => pck.KhoNhan)
                .WithMany(kh => kh.PhieuChuyenKhoNhans)
                .HasForeignKey(pck => pck.MaKhoNhan);

            modelBuilder.Entity<CTPhieuChuyenKho>()
                .HasOne(ct => ct.PhieuChuyenKho)
                .WithMany(pck => pck.CTPhieuChuyenKhos)
                .HasForeignKey(ct => ct.MaCK);

            modelBuilder.Entity<CTPhieuChuyenKho>()
                .HasOne(ct => ct.LoHang)
                .WithMany(lh => lh.CTPhieuChuyenKhos)
                .HasForeignKey(ct => ct.MaLo);

            modelBuilder.Entity<PhieuTra>()
                .HasOne(pt => pt.PhieuBan)
                .WithMany(pb => pb.PhieuTras)
                .HasForeignKey(pt => pt.MaPB);

            modelBuilder.Entity<PhieuTra>()
                .HasOne(pt => pt.KhachHang)
                .WithMany(kh => kh.PhieuTras)
                .HasForeignKey(pt => pt.MaKH);

            modelBuilder.Entity<CTPhieuTra>()
                .HasOne(ct => ct.PhieuTra)
                .WithMany(pt => pt.CTPhieuTras)
                .HasForeignKey(ct => ct.MaPT);

            modelBuilder.Entity<CTPhieuTra>()
                .HasOne(ct => ct.LoHang)
                .WithMany(lh => lh.CTPhieuTras)
                .HasForeignKey(ct => ct.MaLo);

            // CongNo: liên kết tới KH hoặc NCC (tùy LoaiDoiTuong)
            // Không thể tạo FK trực tiếp vì polymorphic, xử lý logic ở service layer
        }
    }
}
