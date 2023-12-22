using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Teuria.Generator;

public readonly struct TypeContext 
{
    public readonly SyntaxKind Kind;
    public readonly string Name;
    public readonly List<string> Namespaces;
    public readonly List<(string Name, SyntaxKind Kind)> ParentTypeInfos;
    public readonly TypeParameterListSyntax? TypeParemeters;

    public TypeContext(TypeDeclarationSyntax syn) 
    {
        Kind = syn.Kind();
        Name = syn.Identifier.ValueText;
        var namespaceNames = new List<string>();
        var parentTypeInfos = new List<(string Name, SyntaxKind Kind)>();

        for (var parent = syn.Parent; parent != null; parent = parent.Parent) 
        {
            switch (parent) 
            {
            case FileScopedNamespaceDeclarationSyntax ns:
                namespaceNames.Add(ns.Name.ToString());
                break;
            case NamespaceDeclarationSyntax ns:
                namespaceNames.Add(ns.Name.ToString());
                break;
            case TypeDeclarationSyntax t:
                parentTypeInfos.Add((t.Identifier.ValueText, t.Kind()));
                break;
            }
        }

        Namespaces = namespaceNames;
        ParentTypeInfos = parentTypeInfos;
        TypeParemeters = syn.TypeParameterList;
    }

    public MemberDeclarationSyntax IncludeTypeHierarchy(MemberDeclarationSyntax newType)
    {
        for (int i = ParentTypeInfos.Count - 1; i >= 0; i--)
        {
            var (name, kind) = ParentTypeInfos[i];
            newType = TypeDeclaration(kind, Identifier(name))
                .WithModifiers(TokenList(Token(SyntaxKind.PartialKeyword)))
                .WithMembers(List(new[] { newType }));
        }
        for (int i = Namespaces.Count - 1; i >= 0; i--)
        {
            newType = NamespaceDeclaration(
                IdentifierName(Namespaces[i]),
                externs: default,
                usings: default,
                members: List(new[] { newType }));
        }

        return newType;
    }
}

public sealed class ExecutionContext 
{
    public Compilation Compilation;
    private SortedSet<FileContent> sources = new();


    public ExecutionContext(GeneratorAttributeSyntaxContext ctx) 
    {
        Compilation = ctx.SemanticModel.Compilation;
    }

    public GeneratedOutput Finish() 
    {
        return new GeneratedOutput(sources);
    }

    public void AddSources(string fileName, string content) 
    {
        sources.Add(new FileContent(fileName, content));
    }
}

public readonly struct GeneratedOutput
{
    public readonly ImmutableSortedSet<FileContent> Sources;

    public GeneratedOutput(IEnumerable<FileContent> sources) 
    {
        var outputSet = ImmutableSortedSet.Create<FileContent>();
        foreach (var source in sources) 
        {
            outputSet.Add(source);
        }
        Sources = outputSet;
    }
}

public struct FileContent 
{
    public string Filename;
    public string Content;

    public FileContent(string fileName, string content) 
    {
        Filename = fileName;
        Content = content;
    }
}