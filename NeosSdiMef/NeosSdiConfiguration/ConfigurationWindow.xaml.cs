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

namespace NeosSdiConfiguration
{
    /// <summary>
    /// Logique d'interaction pour ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public ConfigurationWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            configurationTreeview.ContentGrid = contentGrid;
        }

        public void Display()
        {
            this.ShowDialog();
        }
    }
}
