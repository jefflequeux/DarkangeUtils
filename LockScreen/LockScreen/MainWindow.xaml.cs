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

namespace LockScreen
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] listChar = new string[4];
        int position = 0;
        
        //readonly HookMachine hookMachine = new HookMachine();

        //public MainWindow()
        //{
        //    InitializeComponent();
        //    this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        //}

        //void MainWindow_Loaded(object sender, RoutedEventArgs e)
        //{
        //    hookMachine.HookKeyboard(this);
        //}
        KeyboardListener KListener = new KeyboardListener();
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Window_Loaded);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            KListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);
        }
        private void KListener_KeyDown(object sender, RawKeyEventArgs args)
        {
            //Console.WriteLine(args.Key.ToString());
            //Console.WriteLine(args.ToString()); // Prints the text of pressed button, takes in account big and small letters. E.g. "Shift+a" => "A"
            if (args.Key == Key.J || args.Key == Key.E || args.Key == Key.F)
            {
                switch (args.Key)
                {
                    case Key.J: listChar[position++] = "J"; break;
                    case Key.E: listChar[position++] = "E"; break;
                    case Key.F: listChar[position++] = "F"; break;
                }
                if (position == 4)
                    position = 0;
                if (listChar[0] == "J" && listChar[1] == "E" && listChar[2] == "F" && listChar[3] == "F")
                {
                    this.Close();
                }
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            KListener.Dispose();
        }
    }
}
