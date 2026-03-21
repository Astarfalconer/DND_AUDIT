using System.Text.Json;

namespace RuleEvalApi.Services;

public record AuditResult(string Severity, string Message);

public class Evaluator
{
    public List<AuditResult> Results { get; private set; } = new();

    private List<JsonElement> LoadRules(string path)
    {
        var jsonText = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<JsonElement>>(jsonText) ?? new List<JsonElement>();
    }

    private JsonElement Resolve(JsonElement root, string path)
    {
        var segments = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
        JsonElement current = root;
        foreach (var segment in segments)
        {
            if (current.ValueKind == JsonValueKind.Object)
            {
                current = current.GetProperty(segment);
            }
        }
        return current;
    }

    private object ConvertedValue(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                return element.GetString() ?? "";
            case JsonValueKind.Number:
                return element.GetDouble();
            case JsonValueKind.True:
            case JsonValueKind.False:
                return element.GetBoolean();
            default:
                throw new Exception($"Unsupported JsonElement type: {element.ValueKind}");
        }
    }

    private bool Compare(JsonElement element, string op, JsonElement ruleRoot, string path)
    {
        var condition = ruleRoot.GetProperty("Condition");
        var target = condition.GetProperty("Target");
        var targetValue = ConvertedValue(target);

        JsonElement valueToCompare = Resolve(element, path);
        var valueToCompareConverted = ConvertedValue(valueToCompare);

        switch (op)
        {
            case "==":
                return valueToCompareConverted == targetValue;
            case "!=":
                return valueToCompareConverted != targetValue;
            case ">":
                return (double)valueToCompareConverted > (double)targetValue;
            case "<":
                return (double)valueToCompareConverted < (double)targetValue;
            case ">=":
                return (double)valueToCompareConverted >= (double)targetValue;
            case "<=":
                return (double)valueToCompareConverted <= (double)targetValue;
            default:
                throw new Exception($"Unsupported operator: {op}");
        }
    }

    public void Evaluate()
    {
        Results.Clear();
        var loadedRules = LoadRules("Schema/Rule_Block.json");
        JsonElement sampleData = JsonSerializer.Deserialize<JsonElement>(File.ReadAllText("Schema/character_sheet.json"))!;

        foreach (var rule in loadedRules)
        {
            var className = sampleData.GetProperty("Class").GetString();
            var appliesTo = rule.GetProperty("AppliesTo");
            var severity = rule.GetProperty("Severity").GetString()!;
            var condition = rule.GetProperty("Condition");
            var op = condition.GetProperty("Operator").GetString()!;
            var result = rule.GetProperty("Result");
            var passMessage = result.GetProperty("Pass").GetProperty("Message").GetString()!;
            var failMessage = result.GetProperty("Fail").GetProperty("Message").GetString()!;

            if (!appliesTo.EnumerateArray().Any(e => e.GetString() == "all" || e.GetString() == className))
            {
                continue;
            }

            var message = Compare(sampleData, op, rule, condition.GetProperty("Path").GetString()!)
                ? passMessage
                : failMessage;

            Results.Add(new AuditResult(severity, message));
        }
    }
}
