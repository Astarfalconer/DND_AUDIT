using RuleEvalApi.Services;

namespace RuleEvalApi.Endpoints;

public static class EvaluatorController
{
    public static void MapEvaluatorEndpoints(this WebApplication app)
    {
        app.MapGet("/evaluate", (Evaluator evaluator) =>
        {
            evaluator.Evaluate();
            return Results.Ok(evaluator.Results);
        });
    }
}
