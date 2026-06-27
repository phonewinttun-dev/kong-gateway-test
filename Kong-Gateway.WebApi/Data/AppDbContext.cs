using Kong_Gateway.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace Kong_Gateway.ConsoleApp.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
    }
}
