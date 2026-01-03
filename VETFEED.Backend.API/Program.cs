using VETFEED.Backend.API.Data;
using Microsoft.EntityFrameworkCore;
using VETFEED.Backend.API.Repositories;
using VETFEED.Backend.API.Services;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using VETFEED.Backend.API.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IKhoHangRepository, KhoHangRepository>();
builder.Services.AddScoped<IKhoHangService, KhoHangService>();
builder.Services.AddScoped<ISanPhamRepository, SanPhamRepository>();
builder.Services.AddScoped<ISanPhamService, SanPhamService>();

builder.Services.AddScoped<IKhachHangRepository, KhachHangRepository>();
builder.Services.AddScoped<IKhachHangService, KhachHangService>();

builder.Services.AddScoped<IGiaBanRepository, GiaBanRepository>();
builder.Services.AddScoped<IGiaBanService, GiaBanService>();
builder.Services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
builder.Services.AddScoped<ITaiKhoanService, TaiKhoanService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "VetFeed API",
        Version = "v1"
    });

    // Thêm cấu hình JWT Bearer cho Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Nhập JWT token vào đây (ví dụ: Bearer {token})",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<VetFeedManagementContext>(options => options.UseSqlServer(connectionString));

// Đăng ký Authentication với JWT
builder.Services.AddAuthentication(options => 
{ 
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
})
.AddJwtBearer(options => 
{ 
    var jwtSettings = builder.Configuration.GetSection("Jwt"); 
    var key = jwtSettings["Key"];
    
    if (string.IsNullOrEmpty(key))
        throw new InvalidOperationException("JWT Key không được cấu hình!");

    options.TokenValidationParameters = new TokenValidationParameters 
    { 
        ValidateIssuer = true, 
        ValidateAudience = true, 
        ValidateLifetime = true, 
        ValidateIssuerSigningKey = true, 
        ValidIssuer = jwtSettings["Issuer"], 
        ValidAudience = jwtSettings["Audience"], 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ClockSkew = TimeSpan.Zero
    }; 

    options.Events = new JwtBearerEvents 
    { 
        OnMessageReceived = context => 
        { 
            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                context.Token = authorizationHeader.Substring("Bearer ".Length).Trim();
            }
            else if (context.Request.Cookies.ContainsKey("AccessToken"))
            {
                context.Token = context.Request.Cookies["AccessToken"];
            }
            return Task.CompletedTask;
        }
    }; 
});

var app = builder.Build();

// Kiểm tra kết nối và log ra console
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<VetFeedManagementContext>();
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine("🔎 ConnectionString = " + cs);

    try
    {
        await dbContext.Database.OpenConnectionAsync();
        Console.WriteLine("✅ Kết nối database thành công!");
        await dbContext.Database.CloseConnectionAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Lỗi kết nối database (chi tiết): " + ex.Message);
        if (ex.InnerException != null)
            Console.WriteLine("❌ Inner: " + ex.InnerException.Message);
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
