using RuleEvalApi.Models;
namespace RuleEvalApi.Endpoints;

public static class AccountsController
{
    public static void MapAccountEndpoints(this WebApplication app)
    {
        app.MapPost("/accounts", async (Account account, AppDbContext db) =>
        {
           account.Id = Guid.NewGuid();
           account.CreatedAt = DateTime.UtcNow;
           db.Accounts.Add(account);
           await db.SaveChangesAsync();
           return Results.Created($"/accounts/{account.Id}", account); 
        });

        app.MapGet("/accounts/{id}", async (Guid id, AppDbContext db) =>
        {
            var account = await db.Accounts.FindAsync(id);
            return account != null ? Results.Ok (account) : Results.NotFound();
        });

         app.MapPut("/accounts/{id}", async (Guid id, Account updateAccount, AppDbContext db) =>
        {
            var account = await db.Accounts.FindAsync(id);
            if (account == null) return Results.NotFound();

            account.Username = updateAccount.Username;
            account.Email = updateAccount.Email;
            account.PasswordHash = updateAccount.PasswordHash;
            account.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok();
        });

        app.MapDelete("/accounts/{id}", async (Guid id, AppDbContext db) =>
        {
            var account = await db.Accounts.FindAsync(id);
            if (account == null) return Results.NotFound();
             db.Accounts.Remove(account);
            await db.SaveChangesAsync();
            return Results.Ok();
        });
    }
}