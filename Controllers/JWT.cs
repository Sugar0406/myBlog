
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;


public class JwtService
{
    public string GenerateToken(User user, IConfiguration config)
    {

        // 建立放入token的資料
        // ClaimTypes :Name, Email, Role, NameIdentifier...
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // 主鍵
            new Claim(ClaimTypes.Name, user.Username),                    // 顯示名稱
            new Claim(ClaimTypes.Email, user.Email),                      // Email
        };

        // 產生加密Key，並把string key轉成 byte[]
        // 因為 HMAC 加密只能吃 binary data
        var jwtKey = config.GetValue<string>("Jwt:Key") ?? throw new Exception("Jwt Key is missing!");
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        );

        // 設定簽名方式為 HMAC-SHA256
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 建立 JWT Token
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}



