using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;

            var sourceBuilder = Generate(context, compilation);
            context.AddSource("GeneratedChallenges.cs", SourceText.From(sourceBuilder, Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public static string Generate(GeneratorExecutionContext context, Compilation compilation)
        {
            return @$"
using System;
using System.Collections.Immutable;

namespace Runner
{{
    public static class Challenges
    {{
        public static ImmutableArray<Shared.Challenge> BuildChallenges()
        {{
            var builder = ImmutableArray.CreateBuilder<Shared.Challenge>();
{GenerateChallenges(compilation)}

            builder.Sort((x, y) => x.Info.Date.CompareTo(y.Info.Date));

            return builder.ToImmutable();
        }}
    }}
}}";

            static string GenerateChallenges(Compilation compilation)
            {
                var builder = new StringBuilder();
                var displayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
                var typesToCreate = GetAllChallenges(compilation);
                foreach (var type in typesToCreate)
                {
                    builder.Append(new string(' ', 12));
                    builder.AppendLine($"builder.Add(new {type.ToDisplayString(displayFormat)}());");
                }

                return builder.ToString();
            }
        }

        public static IEnumerable<INamedTypeSymbol> GetAllChallenges(Compilation compilation)
        {
            var challengeBase = compilation.GetTypeByMetadataName("Shared.Challenge")!;

            return GetAllTypes(compilation)
                .OfType<INamedTypeSymbol>()
                .Where(a => SymbolEqualityComparer.Default.Equals(a.BaseType, challengeBase))
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
