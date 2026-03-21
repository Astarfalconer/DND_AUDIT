using Microsoft.EntityFrameworkCore;
using RuleEvalApi.Models;

namespace RuleEvalApi;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<CharacterSheet> CharacterSheets { get; set; }
}