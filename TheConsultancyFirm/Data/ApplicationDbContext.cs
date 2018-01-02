using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CarouselBlock>();
            modelBuilder.Entity<QuoteBlock>();
            modelBuilder.Entity<SolutionAdvantagesBlock>();
            modelBuilder.Entity<TextBlock>();

            modelBuilder.Entity<Slide>();

            modelBuilder.Entity<CaseTag>()
                .HasKey(ct => new {ct.CaseId, ct.TagId});

            modelBuilder.Entity<DownloadTag>()
                .HasKey(dt => new {dt.DownloadId, dt.TagId});

            modelBuilder.Entity<NewsItemTag>()
                .HasKey(nt => new {nt.NewsItemId, nt.TagId});

            modelBuilder.Entity<SolutionTag>()
                .HasKey(st => new {st.SolutionId, st.TagId});

            modelBuilder.Entity<CustomerSolution>()
                .HasKey(cs => new {cs.SolutionId, cs.CustomerId});

            modelBuilder.Entity<Newsletter>()
                .HasIndex(n => n.Email)
                .IsUnique();
        }
    }
}
