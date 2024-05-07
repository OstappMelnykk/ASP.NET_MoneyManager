using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class ApplicationDbContext : IdentityDbContext<Person>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>()
                .HasMany(p => p.Accounts)
                .WithOne(a => a.Person)
                .HasForeignKey(a => a.PersonId);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Goals)
                .WithOne(g => g.Account)
                .HasForeignKey(g => g.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.TransactionsFrom)
                .WithOne(t => t.AccountFrom)
                .HasForeignKey(t => t.AccountFromId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Account>()
                .HasMany(a => a.TransactionsTo)
                .WithOne(t => t.AccountTo)
                .HasForeignKey(t => t.AccountToId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
