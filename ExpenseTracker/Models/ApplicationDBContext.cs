using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Models {
    public class ApplicationDBContext : DbContext {
        public ApplicationDBContext(DbContextOptions options):base(options) {

        }
        public DbSet<Transaction> transactions { get; set; }
        public DbSet<Category> categories { get; set; }
    }
}
