using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// 建立「網站應用程式建構器」，並將命令列參數傳遞給它。這個建構器用於設定和建立 ASP.NET Core 網站應用程式。
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 註冊 MVC 服務，啟用 MVC 功能（Controller + View），讓應用程式能夠處理 HTTP 請求並返回 HTML 頁面。
// 這裡使用 AddControllersWithViews 方法，表示同時啟用 Controller 和 View 的功能。
builder.Services.AddControllersWithViews();




// builder database setting 
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(conn))
{
    throw new Exception("Database connection string is missing!");
}
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn))
);



// JWT Setting
var key = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(key))
{
    throw new Exception("JWT Sinature Key is missing!");
}
builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };

    // 從 Cookie 讀 JWT (後續會將登入資訊存到Cookie)
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["jwt"];
            return Task.CompletedTask;
        }
    };
});



// 建立 app，讓設定變成可執行的 Web 應用程式
var app = builder.Build();



// Configure the HTTP request pipeline.
// 非開發環境（例如生產環境）下，使用 ExceptionHandler 中介軟體來處理未捕獲的例外，並將用戶導向 /Home/Error 頁面。
// 同時啟用 HSTS（HTTP Strict Transport Security），強制瀏覽器使用 HTTPS 連接。
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// 自動把 HTTP 轉 HTTPS，確保所有的請求都使用安全的連接。使用者打 http:// → 自動跳 https://
app.UseHttpsRedirection();
// 啟用 URL 路由解析
app.UseRouting();


// 順序不能錯
// 檢查使用者是否登入
app.UseAuthentication();
// 檢查使用者權限
app.UseAuthorization();



// 啟用靜態檔案服務，允許應用程式直接提供 wwwroot 資料夾中的靜態資源（如 CSS、JavaScript、圖片等），而不需要經過 MVC 的處理。
app.MapStaticAssets();


// 定義一個路由規則，當 URL 符合 {controller}/{action}/{id?} 的模式時，將請求導向對應的 Controller 和 Action 方法來處理。
// URL 第一段是Controller名稱， Home是預設值，如果只打 /，就會用 Home
// URL 第二段 = 方法名稱，Index是預設值
// 所以 /Home = /Home/Index
// app.MapControllerRoute(
//     name: "default",     // name: "default" 是這條「路由規則的名字」。不是功能本身，只是一個標籤（identifier）
//     pattern: "{controller=Home}/{action=Index}/{id?}")  //定義一個預設的路由規則，當 URL 沒有指定 controller 和 action 時，預設使用 HomeController 的 Index 方法來處理請求
//     .WithStaticAssets();


// ASP.NET Core 是「routing pipeline」，輸入的網址會依序匹配所有的MapControllerRoute，不是單一路由
// 這條路由規則可以匹配任意的 controller+Action
// 如果改成 pattern: "main/{action}/{id?}" 則只會匹配main下的所有controlller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=main}/{action=Index}/{id?}"
).WithStaticAssets();


app.Run();

//  概念圖
//    Request
//      ↓
//  Middleware (UseXXX)
//      ↓
//    Routing
//      ↓
//   Controller
//      ↓
//  Action Method
//      ↓
//     View
//      ↓
//    Response
