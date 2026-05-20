using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


public class UserPageController : Controller
{

    private readonly AppDbContext _db;
    public UserPageController(AppDbContext db)
    {
        _db = db;
    }



    [Authorize]
    public IActionResult UserPage()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out var guid))
        {
            return RedirectToAction("Main", "Index");
        }

        var user = _db.users.FirstOrDefault(x => x.UserId == guid);
        return View(user);
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

}