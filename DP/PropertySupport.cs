using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WPFAccelerators.DP
{
    public class PropertySupport
    {
        public readonly DotNetPropertyResult DotNetProperty;
        public readonly DotNetPropertyResult ReadOnlyDotNetProperty;
        public readonly AttachedPropertyAccessorsResult AttachedPropertyAccessors;
        public readonly ChangedCallbackResult ChangedCallback;
        public readonly DependencyPropertyResult DependencyProperty;

        public PropertySupport(IdentifierNameSyntax dpName, IdentifierNameSyntax dpKey, ClassDeclarationSyntax @class, TypeSyntax type, VariableDeclaratorSyntax variable)
        {
            DotNetProperty = new DotNetPropertyResult(type, dpName, variable);
            ReadOnlyDotNetProperty = new DotNetPropertyResult(type, dpName, variable, dpKey);
            AttachedPropertyAccessors = new AttachedPropertyAccessorsResult(type, variable);
            ChangedCallback = new ChangedCallbackResult(@class, variable);
            DependencyProperty = new DependencyPropertyResult(type, dpName, dpKey, @class, variable);
        }
    }
}
