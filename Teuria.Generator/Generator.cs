using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Teuria.Generator;

[Generator]
public class TeuriaSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var entryPointSerializer = context.SyntaxProvider.ForAttributeWithMetadataName(
            "Teuria.TeuriaGameAttribute",
            static (_, _) => true,
            static (ctx, token) => {
                var syn = ctx.TargetNode;

                var context = new ExecutionContext(ctx);
                if (syn is TypeDeclarationSyntax s) 
                {
                    var symbol = ctx.SemanticModel.GetDeclaredSymbol(s);
                    if (symbol is not null and INamedTypeSymbol named)
                        return context.Finish();
                    var typeContext = new TypeContext(s);
                }
                return context.Finish();
            }
        );

        var compiled = entryPointSerializer.Collect().Select(static (values, _) => {
            return new GeneratedOutput(
                values.SelectMany(static o => o.Sources)
            );
        });

        context.RegisterSourceOutput(compiled, RegisterCompiler);
    }

    private static void RegisterCompiler(SourceProductionContext ctx, GeneratedOutput output) 
    {
        foreach (var fileContent in output.Sources) 
        {
            ctx.AddSource(fileContent.Filename, fileContent.Content);
        }
    }
}
