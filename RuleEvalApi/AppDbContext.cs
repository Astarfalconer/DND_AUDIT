using Microsoft.EntityFrameworkCore;
using RuleEvalApi.Models;

namespace RuleEvalApi;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<CharacterSheet> CharacterSheets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CharacterSheet>()
            .Property(c => c.SheetData)
            .HasColumnType("jsonb");

        modelBuilder.Entity<Account>()
            .Property(a => a.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<CharacterSheet>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();
    }
}