using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using VETFEED.Backend.API.DTOs.TaiKhoan;
using VETFEED.Backend.API.Enums;
using Xunit;

namespace VETFEED.Backend.Tests;

public class UtilityTests
{
    [Fact]
    public void JwtService_GenerateToken_ShouldReturnValidToken()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?> {
            {"Jwt:Key", "SUPER_SECRET_KEY_FOR_TESTING_123456789_LONG_ENOUGH_KEY"},
            {"Jwt:Issuer", "VETFEED"},
            {"Jwt:Audience", "VETFEED.Client"},
            {"Jwt:ExpireMinutes", "60"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var jwtService = new JwtService(configuration);
        var userResponse = new TaiKhoanResponse
        {
            MaTK = Guid.NewGuid(),
            Email = "test@vetfeed.com",
            Role = RoleEnum.QUAN_LY
        };

        // Act
        var token = jwtService.GenerateToken(userResponse);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }
}
