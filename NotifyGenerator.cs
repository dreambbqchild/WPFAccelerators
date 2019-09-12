using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFAccelerators
{
    using static SyntaxFactory;

    public class NotifyGenerator : IGenerator
    {
        private static SyntaxTrivia space = Whitespace(" ");

        private static string FieldName(SyntaxToken token)
        {
            return string.Concat(char.ToLowerInvariant(token.Text[0]), token.Text.Substring(1));
        }

        private static SyntaxToken FieldNameToken(SyntaxToken token)
        {
            return Identifier(FieldName(token));
        }

        private static IEnumerable<FieldDeclarationSyntax> CreateField(VariableDeclarationSyntax variable)
        {
            foreach (var declerator in variable.Variables)
            {
                yield return FieldDeclaration(VariableDeclaration(variable.Type.WithoutTrivia().WithTrailingTrivia(space))
                        .WithVariables(SingletonSeparatedList(VariableDeclarator(FieldNameToken(declerator.Identifier)))))
                    .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword).WithTrailingTrivia(space)));
            }
        }

        private static IEnumerable<PropertyDeclarationSyntax> CreateProperty(VariableDeclarationSyntax variable)
        {
            foreach (var declerator in variable.Variables)
            {
                yield return PropertyDeclaration(variable.Type.WithoutTrivia().WithTrailingTrivia(space), declerator.Identifier)
                    .WithTrailingTrivia(CarriageReturnLineFeed)
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword).WithTrailingTrivia(space)))
                    .AddAccessorListAccessors(
                        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithLeadingTrivia(CarriageReturnLineFeed, Tab)
                            .WithBody(Block(ReturnStatement(IdentifierName(FieldNameToken(declerator.Identifier)).WithLeadingTrivia(space)).WithLeadingTrivia(space).WithTrailingTrivia(space))),
                        AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithLeadingTrivia(CarriageReturnLineFeed, Tab)
                            .WithBody(Block(ExpressionStatement(InvocationExpression(IdentifierName("SetValue"), ArgumentList(SeparatedList(new ArgumentSyntax[] { Argument(IdentifierName(FieldName(declerator.Identifier))).WithRefKindKeyword(Token(SyntaxKind.RefKeyword).WithTrailingTrivia(space)), Argument(IdentifierName("value")).WithLeadingTrivia(space) }))))).WithLeadingTrivia(space).WithTrailingTrivia(space))
                        .WithTrailingTrivia(CarriageReturnLineFeed));
            }
        }

        public string Source { get; set; }

        public string LastResult { get; private set; } = string.Empty;

        private string ProcessClass(ClassDeclarationSyntax @class)
        {
            var builder = new StringBuilder();
            var variables = @class.DescendantNodes().OfType<VariableDeclarationSyntax>().ToArray();
            foreach (var variable in variables)
                builder.AppendLine(string.Join(Environment.NewLine, CreateField(variable)));


            builder.AppendLine();

            foreach (var variable in variables)
                builder.AppendLine(string.Join(Environment.NewLine, CreateProperty(variable)));

            return builder.ToString();
        }

        public void Transform()
        {
            if (string.IsNullOrWhiteSpace(Source))
                LastResult = string.Empty;

            var tree = CSharpSyntaxTree.ParseText(Source);
            var root = tree.GetRoot();
            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

            LastResult = string.Join(Environment.NewLine, classes.Select(@class => ProcessClass(@class)));
        }
    }
}
