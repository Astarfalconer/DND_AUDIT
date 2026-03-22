using RuleEvalApi.Services;

namespace RuleEvalApi.Endpoints;

public static class EvaluatorController
{
    public static void MapEvaluatorEndpoints(this WebApplication app)
    {
        app.MapGet("/evaluate/{id}", async (Guid id, Evaluator evaluator, AppDbContext db) =>
        {
            var sheet = await db.CharacterSheets.FindAsync(id);
            if (sheet == null) return Results.NotFound();

            evaluator.Evaluate(sheet.SheetData);
            return Results.Ok(evaluator.Results);
        });
    }
}
