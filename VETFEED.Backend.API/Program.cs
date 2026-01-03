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
builder.Services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
builder.Services.AddScoped<ITaiKhoanService, TaiKhoanService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
            else if (context.Request.Cookies.ContainsKey("jwt"))
            {
                context.Token = context.Request.Cookies["jwt"];
            }
            return Task.CompletedTask;
        }
    }; 
});

var app = builder.Build();

// Kiểm tra kết nối database
using (var scope = app.Services.CreateScope()) 
{ 
    var dbContext = scope.ServiceProvider.GetRequiredService<VetFeedManagementContext>(); 
    try 
    { 
        if (dbContext.Database.CanConnect()) 
            Console.WriteLine("✅ Database connected successfully!"); 
        else 
            Console.WriteLine("❌ Cannot connect to database."); 
    } 
    catch (Exception ex) 
    { 
        Console.WriteLine($"❌ Database connection error: {ex.Message}"); 
    } 
}

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
