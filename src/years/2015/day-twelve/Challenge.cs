using System.Text.Json;

namespace DayTwelve2015
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 12), "JSAbacusFramework.io");

        public override void PartOne(IInput input, IOutput output)
        {
            var document = input.Parse();

            var numbers = 0;

            Walk(document.RootElement, node =>
            {
                numbers += node.GetInt32();
            });

            output.WriteProperty("Sum", numbers);

            static void Walk(JsonElement node, Action<JsonElement> action)
            {
                if (node.ValueKind is JsonValueKind.Array)
                {
                    foreach (var child in node.EnumerateArray())
                    {
                        Walk(child, action);
                    }
                }
                else if (node.ValueKind == JsonValueKind.Object)
                {
                    foreach (var child in node.EnumerateObject())
                    {
                        Walk(child.Value, action);
                    }
                }
                else if (node.ValueKind == JsonValueKind.Number)
                {
                    action(node);
                }
            }
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var document = input.Parse();

            var numbers = 0;

            Walk(document.RootElement, node =>
            {
                numbers += node.GetInt32();
            });

            output.WriteProperty("Sum", numbers);

            static void Walk(JsonElement node, Action<JsonElement> action)
            {
                if (node.ValueKind is JsonValueKind.Array)
                {
                    foreach (var child in node.EnumerateArray())
                    {
                        Walk(child, action);
                    }
                }
                else if (node.ValueKind == JsonValueKind.Object)
                {
                    foreach (var child in node.EnumerateObject())
                    {
                        if (child.Value.ValueKind == JsonValueKind.String &&
                            child.Value.GetString() == "red")
                        {
                            return;
                        }
                    }

                    foreach (var child in node.EnumerateObject())
                    {
                        Walk(child.Value, action);
                    }
                }
                else if (node.ValueKind == JsonValueKind.Number)
                {
                    action(node);
                }
            }
        }
    }

    internal static class ParseExtensions
    {
        public static JsonDocument Parse(this IInput input)
            => JsonDocument.Parse(input.Content.AsMemory());
    }
}
