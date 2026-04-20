using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using myBlog.Models;
using System.Security.Claims;

namespace myBlog.Controllers;


public class MainController : Controller
{
    private readonly AppDbContext _db;
    public MainController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return View(null);
        }
        else
        {
            // 從目前登入的使用者資訊（Claims）裡拿 UserId
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //  out -> 該方法除了回傳 bool，還會「額外丟一個結果回來」
            if (!Guid.TryParse(userId, out var guid))
            {
                return View(null);
            }

            var user = _db.users.FirstOrDefault(x => x.UserId == guid);
            return View(user);
        }

    }
}