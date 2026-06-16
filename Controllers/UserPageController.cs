using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

public class UserPageController : Controller
{

    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _webHostEnvironment;


    public UserPageController(AppDbContext db, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;
        _webHostEnvironment = webHostEnvironment;

        // Console.WriteLine(_webHostEnvironment.WebRootPath);
        // Console.WriteLine(_webHostEnvironment.ContentRootPath);
        // Console.WriteLine(_webHostEnvironment.EnvironmentName);
        // Console.WriteLine(_webHostEnvironment.IsDevelopment());
        // Console.WriteLine(_webHostEnvironment.IsProduction());
        // Console.WriteLine(_webHostEnvironment.WebRootFileProvider);
        // Console.WriteLine(_webHostEnvironment.ContentRootFileProvider);
        
    }



    [Authorize]
    public IActionResult UserPage()
    {
        // 取得該User帳號資訊
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out var guid))
        {
            return RedirectToAction("Main", "Index");
        }

        var user = _db.users.FirstOrDefault(x => x.UserId == guid);

        if (user == null)
        {
            return NotFound();
        }


        // 取得該User為作者的Markdown
        var posts = _db.posts
            .Where(x => x.AuthorId == guid)
            .ToList();

        var UserViewModel = new UserPageViewModel
        {
            User = user,
            Posts = posts
        };

        return View(UserViewModel);
    }

    [Authorize]
    [HttpGet]
    public IActionResult EditMarkdown(Guid id)
    {
        var post = _db.posts.FirstOrDefault(p => p.PostId == id);

        if (post == null)
            return NotFound();

        return View(post);
    }


    [Authorize]
    [HttpPost]
    public IActionResult DeletePost(Guid postid)
    {

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out var guid))
            return RedirectToAction("Main", "Index");

        var post = _db.posts.FirstOrDefault(x =>
            x.PostId == postid &&
            x.AuthorId == guid);

        if (post == null)
            return NotFound();

        _db.posts.Remove(post);
        _db.SaveChanges();


        // delete image folder
        var imageFolder = Path.Combine(
            _webHostEnvironment.WebRootPath,
            "uploads",
            "images",
            postid.ToString()
        );

        Console.WriteLine(imageFolder);

        if (Directory.Exists(imageFolder))
        {
            Directory.Delete(imageFolder, true);  // 參數 true 代表遞迴刪除整個資料夾及其所有內容。
        }

        return RedirectToAction("UserPage");
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddNewMarkdown(CreateMarkdownRequest form)
    {
        // var post_title = form.Title.Trim();
        // var post_preview = form.Preview.Trim();
        // Boolean is_public = form.PublicSetting;
        
        // // 測試接收表單內容
        // Console.WriteLine(post_title);
        // Console.WriteLine(post_preview);
        // Console.WriteLine(is_public);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var post_id =  Guid.NewGuid();

        var newPost = new Post
        {
            PostId = post_id,
            AuthorId = userId,
            PostTitle = form.Title.Trim(),
            PostPreview =  form.Preview.Trim(),
            PostContent = "# Hello MarkDown",
            IsPublic = form.PublicSetting,
            CreatedAt = DateTime.Now,
            UpdateAt = DateTime.Now
        };

        _db.posts.Add(newPost);
        var result = _db.SaveChanges();
        Console.WriteLine(result);


        var imageFolder = Path.Combine(
            _webHostEnvironment.WebRootPath,
            "uploads",
            "images",
            post_id.ToString()
        );

        Directory.CreateDirectory(imageFolder);

        return RedirectToAction("EditMarkdown", "UserPage", new { id = post_id });
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateMarkdown(UpdateMarkdownContentRequest req)
    {
        // var postID = req.post_id;
        // var updatecontent = req.UpdateContent;
        // var UpdateTime = DateTime.Now;
        // Console.WriteLine(postID);
        // Console.WriteLine(updatecontent);
        // Console.WriteLine(UpdateTime);

        var thisPost = _db.posts.FirstOrDefault(p=>p.PostId ==  req.post_id);
        if(thisPost != null)
        {
            thisPost.PostContent = req.UpdateContent;
            thisPost.UpdateAt = DateTime.Now;
            _db.SaveChanges();
            return Ok();
        }




        return Ok();
    }



    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt", new CookieOptions{
            Path = "/" ,
            Domain = Request.Host.Host
        });

        return RedirectToAction("Index", "Main");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UploadImage(UploadImageRequest form)
    {
        if (form.Image == null || form.Image.Length == 0) return BadRequest("沒有上傳圖片");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        var extension = Path.GetExtension(form.Image.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension)) return BadRequest("不支援的圖片格式");
        
        var folder = Path.Combine(
            _webHostEnvironment.WebRootPath,
            "uploads",
            "images",
            form.post_id.ToString()
        );

        // 先檢查folder中是否已經有form.Image.name 如果有 在此檔案名後方加上數字
        var originalName = Path.GetFileNameWithoutExtension(form.Image.FileName);
        var fileName = originalName + extension;
        var filePath = Path.Combine(folder, fileName);

        int count = 2;
        while (System.IO.File.Exists(filePath))
        {
            fileName = $"{originalName}({count}){extension}";
            filePath = Path.Combine(folder, fileName);
            count++;
        }

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await form.Image.CopyToAsync(stream);
        }


        return Ok(new{ success = true, url = $"/uploads/images/{form.post_id.ToString()}/{fileName}" }); 
    }






}



