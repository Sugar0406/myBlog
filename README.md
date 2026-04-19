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
dotnet add package MySql.EntityFrameworkCore<br>

2. 設定連線環境變數<br>
setx ConnectionStrings__DefaultConnection "server=localhost;database=資料庫名稱;user=root;password=密碼;"<br>


3. 建立 AppDbContext class <br>
```cs
using Microsoft.EntityFrameworkCore;

// 繼承 EF Core 的 DbContext class
public class AppDbContext : DbContext
{
    // EF Core 的 DbContext 建構子
    // 用來帶入 Program.cs 註冊的設定 (連線字串, 資料庫 provider ...)
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // 宣告users這張資料表的查詢+操作管道
    // User 這個Model要和 users 這個table 欄位一一對應 -> 見 /Model/User.cs
    public DbSet<User> users { get; set; }
}
```


4. 在Program.cs中進行註冊設定<br>
```cs
//連線字串
var conn = builder.Configuration.GetConnectionString("DefaultConnection");  

//註冊AppDbContext（EF Core 的資料庫上下文）， 之後的controller需要 可以隨時注入連線
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(conn)  // Database provider mysql
);  
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