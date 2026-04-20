using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using myBlog.Models;
using BCrypt.Net;

namespace myBlog.Controllers;


// 一定要public， ASP.NET Core MVC才會識別到並將其加到路由中
public class RegisterController : Controller
{
    // 宣告一個資料庫操作工具 _db，這個 Controller 專用的資料庫工具
    // AppDbContext 定義在 AppDbContext.cs
    private readonly AppDbContext _db;

    // constructor
    // 當 Controller 被建立時，ASP.NET Core 會自動把 AppDbContext塞進來
    public RegisterController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult RegisterForm()
    {
        return View();
    }


    // async 允許寫 await 的語法糖
    // Task 非同步結果容器 代表這個方法會回傳一個未來的結果
    // IActionResult 回傳 HTTP結果（View / Json / Redirect）
    // 引用定義好的RegisterFormModel
    [HttpPost]
    public async Task<IActionResult> Register(RegisterFormModel model)
    {
        // 如果表單資料有錯(驗證失敗)，就不要往下做，直接回到畫面
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "表單驗證失敗" });
        }


        String fileName = Guid.NewGuid() + Path.GetExtension(model.Avatar!.FileName);
        String path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/avatar", fileName);

        // upload avatar to path
        using (var stream = new FileStream(path, FileMode.Create))
        {
            await model.Avatar.CopyToAsync(stream);
        }

        // 先檢查email和name不重複
        if (_db.users.Any(x => x.Email == model.Email))
        {
            return Json(new { success = false, message = "Email 已被使用" });
        }

        // 檢查 Username
        if (_db.users.Any(x => x.Username == model.Username))
        {
            return Json(new { success = false, message = "帳號名稱已被使用" });
        }

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = model.Username,
            Email = model.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            AvatarPath = "/uploads/avatar/" + fileName
        };

        //寫入資料庫
        _db.users.Add(user);
        await _db.SaveChangesAsync();





        return Json(new
        {
            success = true,
            message = $"註冊成功！帳號：{model.Username}"
        });
    }
}