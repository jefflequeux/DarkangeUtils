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
    public class CheckFormat
    {
        public static void CheckForFormat(NeosSdiMef neosSdiMef, CodingRule rule, BasePropertyDeclarationSyntax _property, string text, string textFull, int spanStart, int spanEnd)
        {
            switch (rule.RuleCase)
            {
                case CodingRuleCaseEnum.UpperCamelCase:
                    string upperText = rule.RuleFirstLetter + text.UpperCamelCase();
                    if (text != upperText)
                    {
                        neosSdiMef.AddDecorationError(_property, textFull, "Replace " + text + " by " + upperText, () =>
                        {
                            var span = Span.FromBounds(spanStart, spanEnd);
                            neosSdiMef._textView.TextBuffer.Replace(span, upperText);
                        });
                    }
                    break;
                case CodingRuleCaseEnum.LowerCamelCase:
                    string lowerText = rule.RuleFirstLetter + text.LowerCamelCase();
                    if (text != lowerText)
                    {
                        neosSdiMef.AddDecorationError(_property, textFull, "Replace " + text + " by " + lowerText, () =>
                        {
                            var span = Span.FromBounds(spanStart, spanEnd);
                            neosSdiMef._textView.TextBuffer.Replace(span, lowerText);
                        });
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
