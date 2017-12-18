using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<NewsItem> NewsItems { get; set; }
        public DbSet<Download> Downloads { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Newsletter> NewsLetters { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
