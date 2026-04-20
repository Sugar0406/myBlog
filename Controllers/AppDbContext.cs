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