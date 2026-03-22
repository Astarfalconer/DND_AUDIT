
using Microsoft.EntityFrameworkCore;
using RuleEvalApi.Models;
namespace RuleEvalApi.Endpoints;

public static class CharacterSheetController
{
    public static void MapSheetEndpoints(this WebApplication app)
    {
       app.MapPost("accounts/{accountId}/sheets", async (Guid accountid, CharacterSheet sheet, AppDbContext db) =>
       {
         sheet.Id = Guid.NewGuid();
         sheet.CreatedAt = DateTime.UtcNow;
         sheet.AccountId = accountid;
         db.CharacterSheets.Add(sheet);
         await db.SaveChangesAsync();
         return Results.Created($"/sheets/{sheet.Id}", sheet); 
       }
      
       );
        app.MapGet("accounts/{accountId}/sheets", async (Guid accountid, AppDbContext db) =>
        {
           var sheets = await db.CharacterSheets
           .Where( s => s.AccountId == accountid)
           .ToListAsync();
           return Results.Ok(sheets);
        }
        );
        app.MapGet("sheets/{id}", async (Guid id, AppDbContext db) =>
        {
            var sheet = await db.CharacterSheets.FindAsync(id);
            return sheet != null? Results.Ok (sheet) : Results.NotFound();
        }
        );

        app.MapDelete("sheet/{id}", async (Guid id, AppDbContext db) =>
        {
           var sheet = await db.CharacterSheets.FindAsync(id);
           if (sheet == null) return Results.NotFound();  
              db.CharacterSheets.Remove(sheet);
           await db.SaveChangesAsync();
           return Results.Ok();
        });

        app.MapPut("sheet/{id}", async (Guid id, CharacterSheet updatedSheet, AppDbContext db) =>
        {
           var sheet = await db.CharacterSheets.FindAsync(id);
           if (sheet == null) return Results.NotFound();
           sheet.SheetData = updatedSheet.SheetData;
             await db.SaveChangesAsync();
            return Results.Ok();
        }
        );
    }
}