using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace myBlog.Controllers;

public class LoginController : Controller
{

    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public LoginController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public IActionResult LoginForm()
    {
        return View("LoginForm");
    }


    public async Task<IActionResult> Login (LoginFormModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json( new { success = false, message = "表單驗證失敗" } );
        }


        var userfile = _db.users.FirstOrDefault(u=> u.Email==model.Useremail);
        if(userfile == null)
        {
            return Json(new {success = false, message = "查無帳號" });
        }
        else
        {
            if(BCrypt.Net.BCrypt.Verify(model.Password, userfile.PasswordHash))
            {
                JwtService jwt = new JwtService();
                string JwtToken = jwt.GenerateToken(userfile, _config);


                // 將JWT寫入Cookie
                Response.Cookies.Append("jwt", JwtToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(24)
                });

                return Json(new{success=true, message="登入成功"});
            }
            else
            {
                return Json(new {success = false, message = "密碼錯誤" });
            }
        }
    }
}
