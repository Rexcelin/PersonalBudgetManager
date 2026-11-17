
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalBudgetManager.Models;

namespace PersonalBudgetManager.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<Budget> Budgets { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .Property(user => user.FullName)
            .IsRequired()
            .HasMaxLength(100);

        // Transaction relationship
        builder.Entity<Transaction>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(transaction => transaction.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Budget relationship
        builder.Entity<Budget>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(budget => budget.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // One budget per user for each month and year
        builder.Entity<Budget>()
            .HasIndex(budget => new
            {
                budget.UserId,
                budget.Month,
                budget.Year
            })
            .IsUnique();
    }
}
