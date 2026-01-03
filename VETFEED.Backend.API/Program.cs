using VETFEED.Backend.API.Data;
using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Repositories;
using VETFEED.Backend.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// thêm các repo và service 
builder.Services.AddScoped<IKhoHangRepository, KhoHangRepository>();
builder.Services.AddScoped<IKhoHangService, KhoHangService>();

// NhaCungCap
builder.Services.AddScoped<INhaCungCapRepository, NhaCungCapRepository>();
builder.Services.AddScoped<INhaCungCapService, NhaCungCapService>();

// NhaCungCapSanPham
builder.Services.AddScoped<INhaCungCapSanPhamRepository, NhaCungCapSanPhamRepository>();
builder.Services.AddScoped<INhaCungCapSanPhamService, NhaCungCapSanPhamService>();

// LoHang
builder.Services.AddScoped<ILoHangRepository, LoHangRepository>();
builder.Services.AddScoped<ILoHangService, LoHangService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Lấy connect string  
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký DbContext với SQL Server
builder.Services.AddDbContext<VetFeedManagementContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

// Kiểm tra kết nối và log ra console
using (var scope = app.Services.CreateScope()) 
{ 
    var dbContext = scope.ServiceProvider.GetRequiredService<VetFeedManagementContext>(); 
    try 
    { 
        if (dbContext.Database.CanConnect()) 
        { 
            Console.WriteLine("✅ Kết nối database thành công!"); 
        } 
        else 
        { 
            Console.WriteLine("❌ Không thể kết nối database."); 
        } 
    } catch (Exception ex) 
    { 
        Console.WriteLine($"❌ Lỗi kết nối database: {ex.Message}"); 
    } 
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
