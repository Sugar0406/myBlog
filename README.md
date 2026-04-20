# myBlog (ASP.net project)

markdown editor<br>
建立.NET專案: dotnet new mvc -n projectName<br>
啟動.NET專案: dotnet run<br>

專案目標: 
一個Blog online editor


# ASP.net
Program.cs -> Router Setting <br>

Controller folder
MVC Routing 匹配的是繼承controller類別的 classname -> HomeController 會去掉 Controller 後綴 ，所以 HomeController 的路徑是 /Home -> URL 比對不分大小寫， 所以 /Home = /home <br>
而類別中的public方法就是 Action，就是該controller所有的action。  <br>
controller + action 是動態匹配，{controller}/{action} 只要 Controller 裡有這個方法，就可以當 URL <br>

wwwrote router
所有放在 wwwroot 裡的檔案，都可以被瀏覽器直接讀取

# MVC 架構
參考資料 <br>
[後端工程師的第一堂課 (19) : 現代系統架構 — MVC](https://medium.com/johnliu-%E7%9A%84%E8%BB%9F%E9%AB%94%E5%B7%A5%E7%A8%8B%E6%80%9D%E7%B6%AD/%E5%BE%8C%E7%AB%AF%E5%B7%A5%E7%A8%8B%E5%B8%AB%E7%9A%84%E7%AC%AC%E4%B8%80%E5%A0%82%E8%AA%B2-19-%E7%8F%BE%E4%BB%A3%E7%B3%BB%E7%B5%B1%E6%9E%B6%E6%A7%8B-mvc-abbd0fcfe078)

MVC 可以被分成三塊: Model、View、Controller

View負責呈現介面 <br>

Controller負責處理使用者的請求 <br>

Model 是資料的容器，不是指資料庫。程式會從資料庫取出資料，並放在 Model 內，讓Controller可以使用，和資料庫的欄位不一定完全吻合。有時候，可能會針對欄位內的資料做預處理。(例如匯率換算)<br>
Model真正的任務: 定義資料的內容


# MYSQL in ASP.NET

1. .NET MYSQL package<br>
dotnet add package MySql.EntityFrameworkCore   (Oracle 官方provider)  <br>
dotnet add package Pomelo.EntityFrameworkCore.MySql  (該專案使用的mysql provider) <br>

2. 設定連線環境變數<br>
setx ConnectionStrings__DefaultConnection "server=localhost;database=資料庫名稱;user=root;password=密碼;"<br>


3. 建立 AppDbContext class <br>
```cs
using Microsoft.EntityFrameworkCore;

// 繼承 EFCore DbContext
public class AppDbContext : DbContext
{

    // 該物件的建構由 EFCore 的 DbContext類 進行依賴注入(Dependency Injection)
    // 呼叫 DbContext 的建構子，並把 options 傳進去。
    // options是 Program.cs 設定的： options.UseMySql(conn, ServerVersion.AutoDetect(conn));
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)   
    {
    }

    // 宣告users這張資料表的查詢+操作管道， 這個DbSet對應了User這個Model
    public DbSet<User> users { get; set; }
}
```


4. 在Program.cs中進行註冊設定<br>
```cs
//連線字串
var conn = builder.Configuration.GetConnectionString("DefaultConnection");  

//註冊AppDbContext（EF Core 的資料庫上下文）， 之後的controller需要 可以隨時注入連線
// 使用 pomelo provider
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn))
);

// 使用官方provider
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseMySQL(conn) 
// );  
```

5.實際連線設定<br>
以RegisterController為例子<br>
```cs

public class RegisterController : Controller
{
    // 宣告一個資料庫操作工具 _db，這個 Controller 專用的資料庫工具
    private readonly AppDbContext _db;

    // 在 Constructor 使用依賴注入 (DI)
    // 當 Controller 被建立時，ASP.NET Core 會從 DI 容器取得已註冊的 AppDbContext 實例並注入進來
    public RegisterController(AppDbContext db)
    {
        _db = db;
    }
}
```

6.CRUD 範例<br>

CREATE
```cs
var user = new User
{
    Username = "test",
    Password = "1234"
};
_db.Users.Add(user);
_db.SaveChanges();
```

READ<br>
Entity Framework Core 裡有兩種類型<br>
IQueryable -> SQL 查詢（還沒執行）<br>
List / IEnumerable	-> 已經查完的資料<br>
```cs
// 查全部
var users = _db.Users.ToList();    // 已經查 DB

// 單筆條件查詢
var user = _db.Users.FirstOrDefault(u => u.Id == 1);

// 多筆條件查詢
var users = _db.Users
    .Where(u => (u.Username == "Max" || u.Username == "John") && u.Age > 18)
    .ToList();

// 動態條件查詢
string? name = "Max";
int? age = null;

var query = _db.Users.AsQueryable();  // 把 _db.Users 變成可組合的查詢物件，此時還沒有真正查資料庫  (建立查詢)

if (!string.IsNullOrEmpty(name))
{
    query = query.Where(u => u.Username.Contains(name));  // 如果有 name → 加條件
}

if (age.HasValue)
{
    query = query.Where(u => u.Age >= age.Value);  // 如果有 age → 再加條件
}

var result = query.ToList();  // .ToList() 才真正執行 SQL
```

UPDATE
```cs
var user = _db.Users.Find(1);
if (user != null)
{
    user.Username = "newName";
    _db.SaveChanges();
}
```
DELETE
```cs
var user = _db.Users.Find(1);
if (user != null)
{
    _db.Users.Remove(user);
    _db.SaveChanges();
}
```

流程圖<br>

Program.cs<br>
   ↓<br>
建立 DbContextOptions（含連線資訊）<br>
   ↓<br>
DI 注入到 AppDbContext(options)<br>
   ↓<br>
: base(options)<br>
   ↓<br>
DbContext 初始化（連資料庫）<br>




專案資料表<br>
```SQL
CREATE TABLE users (
    user_id VARCHAR(50) PRIMARY KEY,    
    user_name VARCHAR(100) NOT NULL UNIQUE,
    email_account VARCHAR(100) NOT NULL UNIQUE,
    hash_password VARCHAR(255) NOT NULL,
    user_avatar_path VARCHAR(255) NOT NULL              
);
```




# ASP.NET JWT
1.JWT package<br>
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer<br>

2.設定密鑰環境變數<br>
setx Jwt__Key "輸入金鑰"<br>
setx Jwt__Issuer "myBlog"     (這個 token 是誰發行的)<br>
setx Jwt__Audience "myBlogUser"   (這個 token 是給誰用的)<br>
```bash
# 測試指令(要在CMD中使用)
echo %Jwt__Key%
```
3.在program.cs中設定JWT並啟用auth middleware
```cs
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


// 順序不能錯
// 檢查使用者是否登入
app.UseAuthentication();
// 檢查使用者權限
app.UseAuthorization();
```

4.建立JWT簽發工具
見JWT.cs
```cs
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;


public class JwtService
{
    public string GenerateToken(User user, IConfiguration config)
    {

        // 建立放入token的資料
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), 
            new Claim(ClaimTypes.Name, user.Username),        //在前端可以用 @User.Identity.Name 取得該值           
            new Claim(ClaimTypes.Email, user.Email),                      
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
```