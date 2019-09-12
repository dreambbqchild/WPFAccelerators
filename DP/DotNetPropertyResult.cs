﻿using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WPFAccelerators.DP
{
    using static SyntaxFactory;

    public class DotNetPropertyResult
    {
        private readonly TypeSyntax type;
        private readonly IdentifierNameSyntax dpName;
        private readonly IdentifierNameSyntax dpKey;
        private readonly VariableDeclaratorSyntax variable;

        public DotNetPropertyResult(TypeSyntax type, IdentifierNameSyntax dpName, VariableDeclaratorSyntax variable)
        {
            this.type = type;
            this.dpName = dpName;
            this.variable = variable;
        }

        public DotNetPropertyResult(TypeSyntax type, IdentifierNameSyntax dpName, VariableDeclaratorSyntax variable, IdentifierNameSyntax dpKey)
            : this(type, dpName, variable)
        {
            this.dpKey = dpKey;
        }

        private PropertyDeclarationSyntax WrapProperty()
        {
            return PropertyDeclaration(type, variable.Identifier)
                .WithTrailingTrivia(CarriageReturnLineFeed)
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword).WithTrailingTrivia(Whitespace(" "))))
                .AddAccessorListAccessors(
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithLeadingTrivia(CarriageReturnLineFeed, Tab)
                        .WithBody(Block(ReturnStatement(CastExpression(type.WithoutTrailingTrivia(), InvocationExpression(Identifiers.GetValue, ArgumentList(SingletonSeparatedList(Argument(dpName)))))
                                                                                                    .WithLeadingTrivia(Whitespace(" ")))
                                                                    .WithLeadingTrivia(Whitespace(" "))
                                                                    .WithTrailingTrivia(Whitespace(" ")))),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithModifiers(dpKey != null ? TokenList(Token(SyntaxKind.PrivateKeyword).WithTrailingTrivia(Whitespace(" "))) : TokenList())
                        .WithLeadingTrivia(CarriageReturnLineFeed, Tab)
                        .WithBody(Block(ExpressionStatement(InvocationExpression(Identifiers.SetValue, ArgumentList(SeparatedList(new ArgumentSyntax[] { Argument(dpKey ?? dpName), Argument(IdentifierName("value")).WithLeadingTrivia(Whitespace(" ")) }))))
                                                                    .WithLeadingTrivia(Whitespace(" "))
                                                                    .WithTrailingTrivia(Whitespace(" "))))
                        .WithTrailingTrivia(CarriageReturnLineFeed));
        }

        public override string ToString()
        {
            return WrapProperty().ToString();
        }
    }
}
