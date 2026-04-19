using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using myBlog.Models;

namespace myBlog.Controllers;

// MVC Routing 匹配的是類別名稱（Class Name）
// MVC 會去掉 Controller 後綴 ，所以 HomeController 的路徑是 /Home -> URL 比對不分大小寫， 所以 /Home = /home
[NonController]   // 用來禁止 HomeController route
public class HomeController : Controller
{
    public IActionResult Index()     
    {
        return View(); // 會去找 /Views/{ControllerName}/{ActionName}.cshtml。 假設找不到的話，會再去找Shared(共用頁面）/Views/Shared/{ActionName}.cshtml
        //  也可以寫出完整路徑 return View("Privacy"); -> 等同 Views/Home/Privacy.cshtml。  或指定其他Controller的View  return View("~/Views/Other/Test.cshtml");
    }

    // MVC 會「自動把 Action 方法當成可路由目標」
    // controller + action 是「動態匹配」
    // {controller}/{action} 只要 Controller 裡有這個方法，就可以當 URL
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
