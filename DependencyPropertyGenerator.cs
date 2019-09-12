using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WPFAccelerators.DP;

namespace WPFAccelerators
{
    using static SyntaxFactory;

    public class DependencyPropertyGenerator : IGenerator
    {
        public string Source { get; set; }

        public string LastResult { get; private set; } = string.Empty;

        private static IEnumerable<PropertySupport> ProcessVariable(ClassDeclarationSyntax @class, VariableDeclarationSyntax variableDeclaration)
        {
            foreach (var variable in variableDeclaration.Variables)
            {
                var dpName = IdentifierName(string.Concat(variable.Identifier.Text, "Property"));
                var dpKey = IdentifierName(string.Concat(variable.Identifier.Text, "Key"));
                yield return new PropertySupport(dpName, dpKey, @class, variableDeclaration.Type, variable);
            }
        }

        private static string ProcessClass(ClassDeclarationSyntax @class)
        {
            var builder = new StringBuilder();
            var results = Enumerable.Empty<PropertySupport>();
            foreach (var variableDeclaration in @class.DescendantNodes().OfType<VariableDeclarationSyntax>())
                results = results.Concat(ProcessVariable(@class, variableDeclaration));

            var finalResults = results.ToArray();

            foreach (var result in finalResults)
            {
                builder.AppendLine(result.DotNetProperty.ToString());
                builder.AppendLine();
            }

            foreach (var result in finalResults)
            {
                builder.AppendLine(result.ReadOnlyDotNetProperty.ToString());
                builder.AppendLine();
            }

            foreach (var result in finalResults)
            {
                builder.AppendLine(result.AttachedPropertyAccessors.ToString());
                builder.AppendLine();
            }

            foreach (var result in finalResults)
            {
                builder.AppendLine(result.ChangedCallback.ToString());
                builder.AppendLine();
            }

            builder.AppendLine();

            foreach (var result in finalResults)
                builder.AppendLine(result.DependencyProperty.ToString(DpOptions.RenderFrameworkCallback));

            builder.AppendLine();

            foreach (var result in finalResults)
                builder.AppendLine(result.DependencyProperty.ToString(DpOptions.None));

            builder.AppendLine();

            foreach (var result in finalResults)
                builder.AppendLine(result.DependencyProperty.ToString(DpOptions.RenderAttachedProperty));

            builder.AppendLine();

            foreach (var result in finalResults)
                builder.AppendLine(result.DependencyProperty.ToString(DpOptions.RenderReadOnlyProperty));

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
