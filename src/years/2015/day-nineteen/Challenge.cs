using System.Text.RegularExpressions;

namespace DayNineteen2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 19), "Medicine for Rudolph");

        public void PartOne(IInput input, IOutput output)
        {
            var (replacements, originalMolecule) = input.Parse();

            var constructor = new MoleculeConstructor(replacements);
            var generatedMolecules = constructor.BuildMolecules(originalMolecule);

            output.WriteProperty("Number of distinct molescules", generatedMolecules.Count());
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var (replacements, target) = input.Parse();

            var search = target;
            var count = 0;
            while (search != "e")
            {
                foreach (var (key, replacement) in replacements)
                {
                    if (search.Contains(replacement))
                    {
                        var regex = new Regex(replacement);
                        search = regex.Replace(search, key, 1);
                        count++;
                    }
                }
            }

            output.WriteProperty("Number of steps to 'e'", count);
        }
    }

    public class MoleculeConstructor
    {
        private readonly ImmutableArray<(string Target, string Replacement)> _replacements;

        public MoleculeConstructor(ImmutableArray<(string Target, string Replacement)> replacements)
        {
            _replacements = replacements;
        }

        public IEnumerable<string> BuildMolecules(string originalMolecule)
        {
            return _replacements
                .SelectMany(transform => ApplyReplacement(
                    transform.Target,
                    transform.Replacement,
                    originalMolecule))
                .Distinct();
        }

        private static IEnumerable<string> ApplyReplacement(string target, string replacement, string molecule)
        {
            var indexes = new List<int>();
            var span = molecule.AsSpan();

            while (span.IsEmpty is false)
            {
                var index = span.LastIndexOf(target);
                if (index == -1)
                {
                    break;
                }
                indexes.Add(index);
                span = span[..index];
            }

            var length = molecule.Length - target.Length + replacement.Length;
            foreach (var index in indexes)
            {
                var context = (
                    TargetIndex: index,
                    TargetLength: target.Length,
                    Replacement: replacement,
                    Molecule: molecule
                );

                yield return string.Create(length, context, static (span, state) =>
                {
                    var original = state.Molecule.AsSpan();
                    var result = span;

                    var start = original[0..state.TargetIndex];
                    start.CopyTo(result);
                    result = result[start.Length..];

                    var replacement = state.Replacement.AsSpan();
                    replacement.CopyTo(result);
                    result = result[replacement.Length..];

                    var end = original[(state.TargetIndex + state.TargetLength)..];
                    end.CopyTo(result);
                    result = result[end.Length..];
                });
            }
        }
    }
}
