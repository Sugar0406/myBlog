using Microsoft.EntityFrameworkCore;

// 繼承 DbContext
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // 宣告users這張資料表的查詢+操作管道， 這個DbSet對應了User這個Model
    public DbSet<User> users { get; set; }
}