using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using myBlog.Models;

namespace myBlog.Controllers;


public class mainController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}