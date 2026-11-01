using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ExtensionMembers.Tests;

/// <summary>
/// The article makes two specific compiler-diagnostic claims. Rather than assert them
/// in prose, this class compiles the exact snippets in-memory with Roslyn and checks the
/// emitted diagnostic IDs — so "CS9296" and "CS9339" are verified by the test suite,
/// not just quoted from documentation.
/// </summary>
public class CompilerDiagnosticsTests
{
    private static readonly IEnumerable<MetadataReference> References =
        ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path));

    private static string[] Compile(string source) =>
        CSharpCompilation.Create(
                assemblyName: "Snippet",
                syntaxTrees: [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp14))],
                references: References,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Select(d => d.Id)
            .ToArray();

    [Fact]
    public void A_clean_extension_block_compiles_with_no_errors()
    {
        const string source = """
            public record Order(decimal Total, string Region);

            public static class OrderExtensions
            {
                extension(Order order)
                {
                    public bool IsEligibleForFreeShipping => order.Total >= 50m && order.Region == "EU";
                }
            }
            """;

        var errors = Compile(source);

        Assert.Empty(errors);
    }

    [Fact]
    public void Extension_property_inside_an_expression_tree_fails_with_CS9296()
    {
        const string source = """
            using System;
            using System.Linq.Expressions;

            public record Order(decimal Total, string Region);

            public static class OrderExtensions
            {
                extension(Order order)
                {
                    public bool IsEligibleForFreeShipping => order.Total >= 50m && order.Region == "EU";
                }
            }

            public static class Usage
            {
                // This is what an EF Core IQueryable<Order>.Where(...) predicate compiles to.
                public static Expression<Func<Order, bool>> Predicate =
                    order => order.IsEligibleForFreeShipping;
            }
            """;

        var errors = Compile(source);

        Assert.Contains("CS9296", errors);
    }

    [Fact]
    public void Two_extension_blocks_declaring_the_same_member_name_fail_with_CS9339()
    {
        const string source = """
            public record Order(decimal Total, string Region);

            public static class OrderExtensionsA
            {
                extension(Order order)
                {
                    public bool IsEligibleForFreeShipping => order.Total >= 50m && order.Region == "EU";
                }
            }

            public static class OrderExtensionsB
            {
                extension(Order order)
                {
                    public bool IsEligibleForFreeShipping => order.Total >= 50m;
                }
            }

            public static class Usage
            {
                public static bool Check(Order order) => order.IsEligibleForFreeShipping;
            }
            """;

        var errors = Compile(source);

        Assert.Contains("CS9339", errors);
    }
}
