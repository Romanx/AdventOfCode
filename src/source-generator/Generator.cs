using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace SourceGenerator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        private static readonly SymbolDisplayFormat displayFormat = new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
        private const string ClassTemplate = @"
using System;
using System.Collections.Immutable;

namespace Runner
{
    public static class Challenges
    {
        public static ImmutableList<Shared.Challenge> BuildChallenges()
        {
            var builder = ImmutableList.CreateBuilder<Shared.Challenge>();
            {{~ for challenge in challenges ~}}
            builder.Add(new {{challenge}}());
            {{~ end ~}}

            builder.Sort((x, y) => x.Info.Date.CompareTo(y.Info.Date));

            return builder.ToImmutable();
        }
    }
}";

        public void Execute(GeneratorExecutionContext context)
        {
            var sourceBuilder = Generate(context.Compilation);
            context.AddSource("GeneratedChallenges.cs", SourceText.From(sourceBuilder, Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public static string Generate(Compilation compilation)
        {
            var typesToCreate = GetAllChallenges(compilation);
            var template = Template.Parse(ClassTemplate.Trim());

            return template.Render(new
            {
                Challenges = typesToCreate.Select(static challenge => challenge.ToDisplayString(displayFormat)).ToArray(),
            });
        }

        public static IEnumerable<INamedTypeSymbol> GetAllChallenges(Compilation compilation)
        {
            var challengeSync = compilation.GetTypeByMetadataName("Shared.ChallengeSync")!;
            var challengeAsync = compilation.GetTypeByMetadataName("Shared.ChallengeAsync")!;

            return GetAllTypes(compilation)
                .OfType<INamedTypeSymbol>()
                .Where(a => SymbolEqualityComparer.Default.Equals(a.BaseType, challengeSync) || SymbolEqualityComparer.Default.Equals(a.BaseType, challengeAsync))
                .Where(a => a.DeclaredAccessibility == Accessibility.Public)
                .ToArray();
        }

        private static IEnumerable<INamedTypeSymbol> GetAllTypes(Compilation compilation) => GetAllTypes(compilation.GlobalNamespace);

        private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol @namespace)
        {
            foreach (var type in @namespace.GetTypeMembers())
                foreach (var nestedType in GetNestedTypes(type))
                    yield return nestedType;

            foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
                foreach (var type in GetAllTypes(nestedNamespace))
                    yield return type;
        }

        private static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol type)
        {
            yield return type;
            foreach (var nestedType in type.GetTypeMembers()
                .SelectMany(nestedType => GetNestedTypes(nestedType)))
                yield return nestedType;
        }
    }
}
