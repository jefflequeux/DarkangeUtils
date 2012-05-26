using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeosSdiConfiguration.Controls.Helpers
{
    public enum CodingRuleCaseEnum
    {
        Normal = 1,
        UpperCamelCase = 2,
        LowerCamelCase = 3
    }

    public enum CodingRulesTypeEnum
    {
        ClassPropertyName = 1,
        ClassVariableName = 2
    }

    [Serializable]
    public class CodingRule
    {

        public string RuleName { get; set; }
        public CodingRulesTypeEnum Type { get; set; }
        public CodingRuleCaseEnum RuleCase { get; set; }
        public string RuleFirstLetter { get; set; }

        public CodingRule()
        {
            RuleCase = CodingRuleCaseEnum.Normal;
        }
    }
}
