using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WPFAccelerators.DP
{
    using static SyntaxFactory;

    public static class Identifiers
    {
        public static readonly IdentifierNameSyntax GetValue = IdentifierName("GetValue");
        public static readonly IdentifierNameSyntax SetValue = IdentifierName("SetValue");

        public static string GetCallbackMethodName(VariableDeclaratorSyntax variable)
        {
            return string.Concat(variable.Identifier.Text, "ChangedCallback");
        }
    }
}
