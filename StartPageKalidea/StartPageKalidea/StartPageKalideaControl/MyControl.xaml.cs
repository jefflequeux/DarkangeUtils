// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;

namespace StartPageKalideaControl
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MyControl : UserControl
    {
        public MyControl()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Load control user settings from previous session.
            // Example: string value = StartPageSettings.RetrieveString("MySetting");
        }

        private StartPageSettings _settings;
        /// <summary>
        /// Return a StartPageSettings object.
        ///
        /// Use: StartPageSettings.StoreString("MySettingName", "MySettingValue");
        /// Use: string value = StartPageSettings.RetrieveString("MySettingName");
        ///
        /// Note: As this property is using the Start Page tool window DataContext to retrieve the Visual Studio DTE, 
        /// this property can only be used after the UserControl is loaded and the inherited DataContext is set.
        /// </summary>
        private StartPageSettings StartPageSettings
        {
            get
            {
                if (_settings == null)
                {
                    DTE2 dte = Utilities.GetDTE(DataContext);
                    ServiceProvider serviceProvider = Utilities.GetServiceProvider(dte);
                    _settings = new StartPageSettings(serviceProvider);
                }
                return _settings;
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("IExplore.exe", "http://salarievmc.idf.canalce.com/");
        }

        private void Wiki_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("IExplore.exe", "http://sharecorp009/canalce/intranet/wiki/default.aspx");
        }
        

        private void PlSQL_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\PLSQL Developer\plsqldev.exe");
        }
        

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock source = sender as TextBlock;
            source.TextDecorations.Add(TextDecorations.Underline);
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock source = sender as TextBlock;
            source.TextDecorations.Clear();
        }

        
    }
}
