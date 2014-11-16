using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VarAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class VarAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "VarAnalyzer";
        internal const string Title = "Type name contains lowercase letters";
        internal const string MessageFormat = "Type name contains lowercase letters";
        internal const string Category = "Naming";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, 
            Title, 
            MessageFormat, 
            Category, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.VariableDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            VariableDeclarationSyntax variableDecleration = (VariableDeclarationSyntax)context.Node;
            var typeSyntax = (TypeSyntax)variableDecleration.Type;
            if (typeSyntax.IsVar)
            {
                foreach (VariableDeclaratorSyntax variable in variableDecleration.Variables)
                {
                    EqualsValueClauseSyntax initializer = variable.Initializer;
                    if (initializer != null)
                    {
                        TypeSyntax variableTypeName = variableDecleration.Type;
                        ITypeSymbol variableType = context.SemanticModel.GetTypeInfo(variableTypeName).ConvertedType;
                        if(variableType.IsAnonymousType == false)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Rule, typeSyntax.GetLocation()));
                        }
                    }
                }
            }
        }
    }
}
