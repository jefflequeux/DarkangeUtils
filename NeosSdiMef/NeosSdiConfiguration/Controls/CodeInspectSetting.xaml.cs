using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NeosSdiConfiguration.Controls.Helpers;
using System.Collections.ObjectModel;

namespace NeosSdiConfiguration.Controls
{
    /// <summary>
    /// Logique d'interaction pour CodeInspectSetting.xaml
    /// </summary>
    public partial class CodeInspectSetting : UserControl
    {
        private ConfigurationSettings configurationSettings;

        public ObservableCollection<CodingRule> Rules
        {
            get { return (ObservableCollection<CodingRule>)GetValue(RulesProperty); }
            set { SetValue(RulesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WorkItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RulesProperty =
            DependencyProperty.Register("Rules", typeof(ObservableCollection<CodingRule>), typeof(CodeInspectSetting));


        public CodeInspectSetting()
        {
            InitializeComponent();
            configurationSettings = new ConfigurationSettings();
            configurationSettings = configurationSettings.Load();
            if (configurationSettings == null || (configurationSettings != null && configurationSettings.CodingRules.Count == 0))
            {
                Rules = new ObservableCollection<CodingRule>();
                Rules.Add(new CodingRule() { RuleName = "Class Property Name", Type=CodingRulesTypeEnum.ClassPropertyName, RuleCase = CodingRuleCaseEnum.Normal, RuleFirstLetter = "" });
                Rules.Add(new CodingRule() { RuleName = "Class Variable Name", Type = CodingRulesTypeEnum.ClassVariableName, RuleCase = CodingRuleCaseEnum.UpperCamelCase, RuleFirstLetter = "_" });
            }
            else
            {
                Rules = configurationSettings.CodingRules;

            }
            //listOfRules.ItemsSource = rules;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            configurationSettings.CodingRules = Rules;
            configurationSettings.Save();
        }


        
    }
}
