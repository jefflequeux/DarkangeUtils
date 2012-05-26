using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace LockScreen
{
    public class NativeWindow : INotifyPropertyChanged
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        private const int SW_HIDE = 0;
        private const int SW_SHOWMINIMIZED = 2;

        public IntPtr Handle { get; set; }

        public string Name { get; set; }

        public ImageSource Icon { get; set; }

        bool _protected;
        public bool Protected
        {
            get { return _protected; }
            set
            {
                if (value == _protected)
                    return;
                _protected = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Protected"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void ProtectMyJob()
        {
            ShowWindowAsync(Handle, SW_SHOWMINIMIZED);
        }
    }
}
