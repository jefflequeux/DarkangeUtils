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

namespace NeosSdiConfiguration.Controls
{
    /// <summary>
    /// Logique d'interaction pour ConfigurationTreeview.xaml
    /// </summary>
    public partial class ConfigurationTreeview : UserControl
    {
        /// <summary>
        /// Grid to update with treeview selected control
        /// </summary>
        public Grid ContentGrid { get; set; }

        public ConfigurationTreeview()
        {
            InitializeComponent();
            ItemUINode root = new ItemUINode("Root", null);
            root.Child.Add(new ItemUINode("General", new GeneralGeneral()));
            root.Child.Add(new ItemUINode("Editor", null));

            treeViewConfigurationGeneral.ItemsSource = root.Child;

            

            ItemUINode rootCI = new ItemUINode("Root", null);
            rootCI.Child.Add(new ItemUINode("Setting", new CodeInspectSetting()));
            rootCI.Child.Add(new ItemUINode("Inspection Severity", null));

            treeViewConfigurationCodeInspect.ItemsSource = rootCI.Child;
        }

        private void treeViewConfigurationGeneral_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectNode(sender, e);
        }

        private void treeViewConfigurationCodeInspect_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectNode(sender, e);
        }

        /// <summary>
        /// Display the control associated with the selected item in the treeview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectNode(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView treeView = (TreeView)sender;

            UserControl uc = (treeView.SelectedItem as ItemUINode).Tag as UserControl;
            ContentGrid.Children.Clear();
            if (uc != null)
            {
                ContentGrid.Children.Add(uc);
            }
        }



    }
}
