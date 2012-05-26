using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using NeosSdiMef.Extension;
using Microsoft.VisualStudio.Text;
using NeosSdiConfiguration;
using NeosSdiConfiguration.Controls.Helpers;

namespace NeosSdiMef.CodingRules
{
    public static class ClassVariableFormat
    {
        /// <summary>
        /// Check for class variable matching rules
        /// </summary>
        /// <param name="neosSdiMef"></param>
        public static void ParseClassVariable(this NeosSdiMef neosSdiMef)
        {
            ConfigurationSettings configurationSettings = new ConfigurationSettings();
            configurationSettings = configurationSettings.Load();

            CodingRule rule = configurationSettings.CodingRules.Where(p => p.Type == NeosSdiConfiguration.Controls.Helpers.CodingRulesTypeEnum.ClassVariableName).SingleOrDefault();

            var root = (CompilationUnitSyntax)neosSdiMef.tree.Root;
            var _properties = root.DescendentNodes()
                        .OfType<Roslyn.Compilers.CSharp.VariableDeclarationSyntax>()
                        .ToList();

            //foreach (var _property in _properties)
            //{
            //    string text = _property.Identifier.ValueText;
            //    string textFull = _property.ToString().Replace(System.Environment.NewLine, "");
            //    CheckFormat.CheckForFormat(neosSdiMef, rule, _property, text, textFull, _property.Identifier.Span.Start, _property.Identifier.Span.End);
            //}
        }


    }
}
