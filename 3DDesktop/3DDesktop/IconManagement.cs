using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using Microsoft.Win32;
using TAFactory.IconPack;
using TraceOne.Framework.TFS.IconHelper;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;
using Point = System.Drawing.Point;

namespace _3DDesktop
{
    public class IconManagement
    {
        public static int iconSize = 32;
        public static int iconSize3D = 48;
        public static int defaultIconSize = 32;
        
        public const uint LVM_FIRST = 0x1000;
        public const uint LVM_GETITEMCOUNT = LVM_FIRST + 4;
        public const uint LVM_GETITEMW = LVM_FIRST + 75;
        public const uint LVM_GETITEMPOSITION = LVM_FIRST + 16;
        public const uint PROCESS_VM_OPERATION = 0x0008;
        public const uint PROCESS_VM_READ = 0x0010;
        public const uint PROCESS_VM_WRITE = 0x0020;
        public const uint MEM_COMMIT = 0x1000;
        public const uint MEM_RELEASE = 0x8000;
        public const uint MEM_RESERVE = 0x2000;
        public const uint PAGE_READWRITE = 4;
        public const int LVIF_TEXT = 0x0001;

        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
            uint dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll")]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
           uint dwSize, uint dwFreeType);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);
        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
           IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
           IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess,
            bool bInheritHandle, uint dwProcessId);
        [DllImport("user32.DLL")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.DLL")]
        public static extern IntPtr FindWindow(string lpszClass, string lpszWindow);
        [DllImport("user32.DLL")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent,
            IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd,
            out uint dwProcessId);

        public struct LVITEM
        {
            public int mask;
            public int iItem;
            public int iSubItem;
            public int state;
            public int stateMask;
            public IntPtr pszText; // string
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
            public int iGroupId;
            public int cColumns;
            public IntPtr puColumns;
        }

        /// <summary>
        /// FxCop requires all Marshalled functions to be in a class called NativeMethods.
        /// </summary>
        internal static class NativeMethods
        {
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DeleteObject(IntPtr hObject);
        }

        [DllImport("shell32.dll")]
        static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        // path.ToString() should now hold the path

        Dictionary<FileInfo, System.Drawing.Point> dicoOfIcons = new Dictionary<FileInfo, System.Drawing.Point>();
        Dictionary<string, System.Drawing.Point> dicoOfSpecialIcons = new Dictionary<string, System.Drawing.Point>();
        Dictionary<FileInfo, Icon> dicoOfFileIcons = new Dictionary<FileInfo, Icon>();
        Dictionary<FileInfo, int> dicoOfFile = new Dictionary<FileInfo, int>();

        Dictionary<string, int> listOfSpecialNameIconIndex = new Dictionary<string, int>();

        public List<DesktopIcon> ListOfDesktopIcon
        {
            get { return listOfDesktopIcon; }
            set { listOfDesktopIcon = value; }
        }

        List<DesktopIcon> listOfDesktopIcon = new List<DesktopIcon>();


        public void DrawIconOnBackground(Canvas backgroundGrid)
        {
            listOfDesktopIcon = new List<DesktopIcon>();
            GetInformations();
            CGFolder f = new CGFolder(Environment.SpecialFolder.MyComputer);
            listOfSpecialNameIconIndex.Add(f.Pidl.DisplayName.ToLowerInvariant(), 15);
            listOfSpecialNameIconIndex.Add("corbeille", 32);
            listOfSpecialNameIconIndex.Add("recycle bin", 32);
            f = new CGFolder(Environment.SpecialFolder.MyDocuments);
            if (f != null && f.Pidl != null)
                listOfSpecialNameIconIndex.Add(f.Pidl.DisplayName.ToLowerInvariant(), 126);
            try
            {
                f = new CGFolder(Environment.SpecialFolder.MyPictures);
                if (f != null && f.Pidl != null)
                    listOfSpecialNameIconIndex.Add(f.Pidl.DisplayName.ToLowerInvariant(), 127);
            }
            catch (Exception ex) { }
            try
            {
                f = new CGFolder(Environment.SpecialFolder.MyMusic);
                if (f != null && f.Pidl != null)
                    listOfSpecialNameIconIndex.Add(f.Pidl.DisplayName.ToLowerInvariant(), 128);
            }
            catch (Exception ex) { }

            Dictionary<string, System.Drawing.Point> dicoOfSpecialIconsAllreadeyAdded = new Dictionary<string, System.Drawing.Point>();

            foreach (KeyValuePair<FileInfo, Icon> icon in dicoOfFileIcons)
            {
                DesktopIcon desktopIcon = new DesktopIcon();

                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                System.Windows.Controls.Label label = new Label();

                desktopIcon.Image = icon.Value.ToBitmap();
                img.Source = Imaging.ToBitmapSource(icon.Value.ToBitmap());
                img.HorizontalAlignment = HorizontalAlignment.Left;
                img.VerticalAlignment = VerticalAlignment.Top;
                img.Stretch = Stretch.Fill;

                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignment = VerticalAlignment.Top;

                bool positionFound = false;
                foreach (KeyValuePair<FileInfo, System.Drawing.Point> pair in dicoOfIcons)
                {
                    if (pair.Key.FullName == icon.Key.FullName)
                    {
                        desktopIcon.FileName = icon.Key;

                        img.Margin = new Thickness(pair.Value.X - (iconSize / 2), pair.Value.Y - (iconSize / 2), 0, 0);
                        img.Width = icon.Value.Width;
                        img.Height = icon.Value.Height;

                        label.Margin = new Thickness(pair.Value.X - (iconSize / 2) - 25, pair.Value.Y - (iconSize / 2) + icon.Value.Height, 0, 0);
                        label.Width = icon.Value.Width + 50;
                        label.Height = icon.Value.Height;
                        label.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                        label.Content = pair.Key.Name;
                        positionFound = true;
                    }
                }
                if (!positionFound)
                {
                    foreach (KeyValuePair<string, System.Drawing.Point> pair in dicoOfSpecialIcons)
                    {
                        if (pair.Key == icon.Key.Name)
                        {
                            desktopIcon.FileName = icon.Key;

                            desktopIcon.Image = GetFileIcon(icon.Key.FullName, IconReader.IconSize.Large, false).ToBitmap();
                            img.Source =
                                _3DDesktop.Imaging.ToBitmapSource(desktopIcon.Image);
                            img.Margin = new Thickness(pair.Value.X - (iconSize / 2), pair.Value.Y - (iconSize / 2), 0, 0);
                            img.Width = icon.Value.Width;
                            img.Height = icon.Value.Height;

                            label.Margin = new Thickness(pair.Value.X - (iconSize / 2) - 20, pair.Value.Y - (iconSize / 2) + icon.Value.Height, 0, 0);
                            label.Width = icon.Value.Width + 40;
                            label.Height = icon.Value.Height;
                            label.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            label.Content = pair.Key;
                            positionFound = true;
                            dicoOfSpecialIconsAllreadeyAdded.Add(pair.Key, pair.Value);
                        }
                    }
                }
                if (positionFound)
                {
                    desktopIcon.ImageIcon = img;
                    desktopIcon.Label = label;
                    desktopIcon.Name = label.Content.ToString();
                    desktopIcon.Location = new System.Windows.Point((int)img.Margin.Left, (int)img.Margin.Top);
                    listOfDesktopIcon.Add(desktopIcon);
                    //backgroundGrid.Children.Add(img);
                    //backgroundGrid.Children.Add(label);
                }
            }
            foreach (KeyValuePair<string, System.Drawing.Point> pair in dicoOfSpecialIcons)
            {
                if (!dicoOfSpecialIconsAllreadeyAdded.ContainsKey(pair.Key))
                {
                    DesktopIcon desktopIcon = new DesktopIcon();
                    System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                    System.Windows.Controls.Label label = new Label();

                    foreach (KeyValuePair<string, int> pairName in listOfSpecialNameIconIndex)
                    {
                        if (pair.Key.ToLowerInvariant() == pairName.Key.ToLowerInvariant())
                        {
                            Icon openFolderIcon = IconHelper.ExtractIcon(@"%SystemRoot%\system32\shell32.dll", pairName.Value);
                            desktopIcon.Image = openFolderIcon.ToBitmap();
                            img.Source = Imaging.ToBitmapSource(openFolderIcon.ToBitmap());

                        }
                    }
                    img.HorizontalAlignment = HorizontalAlignment.Left;
                    img.VerticalAlignment = VerticalAlignment.Top;
                    img.Stretch = Stretch.Fill;

                    label.HorizontalAlignment = HorizontalAlignment.Left;
                    label.VerticalAlignment = VerticalAlignment.Top;

                    //img.Source =
                    //    ToBitmapSource(GetFileIcon(icon.Key.FullName, IconReader.IconSize.Large, false).ToBitmap());
                    img.Margin = new Thickness(pair.Value.X - (iconSize / 2), pair.Value.Y - (iconSize / 2), 0, 0);
                    img.Width = iconSize;
                    img.Height = iconSize;

                    label.Margin = new Thickness(pair.Value.X - (iconSize / 2) - 20, pair.Value.Y - (iconSize / 2) + iconSize, 0, 0);
                    label.Width = iconSize + 40;
                    label.Height = iconSize;
                    label.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                    label.Content = pair.Key;
                    dicoOfSpecialIconsAllreadeyAdded.Add(pair.Key, pair.Value);

                    desktopIcon.Label = label;
                    desktopIcon.ImageIcon = img;
                    desktopIcon.Name = label.Content.ToString();
                    desktopIcon.Location = new System.Windows.Point((int)img.Margin.Left, (int)img.Margin.Top);
                    listOfDesktopIcon.Add(desktopIcon);
                    //backgroundGrid.Children.Add(img);
                    //backgroundGrid.Children.Add(label);
                }
            }
        }

        private void GetInformations()
        {
            dicoOfFile = new Dictionary<FileInfo, int>();
            dicoOfFileIcons = new Dictionary<FileInfo, Icon>();

            // For User Desktop
            GetFilesAndFolder(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            // For All User Desktop

            const int CSIDL_PROGRAMS = 0x0019;  // \Windows\Start Menu\Programs
            StringBuilder pathAllUserDesktop = new StringBuilder(260);
            SHGetSpecialFolderPath(IntPtr.Zero, pathAllUserDesktop, CSIDL_PROGRAMS, false);
            GetFilesAndFolder(pathAllUserDesktop.ToString());




            dicoOfIcons = new Dictionary<FileInfo, System.Drawing.Point>();
            dicoOfSpecialIcons = new Dictionary<string, System.Drawing.Point>();
            // get the handle of the desktop listview
            IntPtr vHandle = FindWindow("Progman", "Program Manager");
            vHandle = FindWindowEx(vHandle, IntPtr.Zero, "SHELLDLL_DefView", null);
            vHandle = FindWindowEx(vHandle, IntPtr.Zero, "SysListView32", "FolderView");

            //Get total count of the icons on the desktop
            int vItemCount = SendMessage(vHandle, LVM_GETITEMCOUNT, 0, 0);

            uint vProcessId;
            GetWindowThreadProcessId(vHandle, out vProcessId);

            IntPtr vProcess = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ |
                PROCESS_VM_WRITE, false, vProcessId);
            IntPtr vPointer = VirtualAllocEx(vProcess, IntPtr.Zero, 4096,
                MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
            try
            {
                for (int j = 0; j < vItemCount; j++)
                {
                    byte[] vBuffer = new byte[256];
                    LVITEM[] vItem = new LVITEM[1];
                    vItem[0].mask = LVIF_TEXT;
                    vItem[0].iItem = j;
                    vItem[0].iSubItem = 0;
                    vItem[0].cchTextMax = vBuffer.Length;
                    vItem[0].pszText = (IntPtr)((int)vPointer + Marshal.SizeOf(typeof(LVITEM)));
                    uint vNumberOfBytesRead = 0;

                    WriteProcessMemory(vProcess, vPointer,
                        Marshal.UnsafeAddrOfPinnedArrayElement(vItem, 0),
                        Marshal.SizeOf(typeof(LVITEM)), ref vNumberOfBytesRead);
                    SendMessage(vHandle, LVM_GETITEMW, j, vPointer.ToInt32());
                    ReadProcessMemory(vProcess,
                        (IntPtr)((int)vPointer + Marshal.SizeOf(typeof(LVITEM))),
                        Marshal.UnsafeAddrOfPinnedArrayElement(vBuffer, 0),
                        vBuffer.Length, ref vNumberOfBytesRead);
                    string vText = Encoding.Unicode.GetString(vBuffer, 0,
                        (int)vNumberOfBytesRead);
                    string IconName = vText;
                    //Get icon location
                    SendMessage(vHandle, LVM_GETITEMPOSITION, j, vPointer.ToInt32());
                    System.Drawing.Point[] vPoint = new System.Drawing.Point[1];
                    ReadProcessMemory(vProcess, vPointer,
                        Marshal.UnsafeAddrOfPinnedArrayElement(vPoint, 0),
                        Marshal.SizeOf(typeof(System.Drawing.Point)), ref vNumberOfBytesRead);
                    vPoint[0].X += (defaultIconSize / 2);
                    vPoint[0].Y += (defaultIconSize / 2);
                    string IconLocation = vPoint[0].ToString();

                    int pos = IconName.IndexOf('\0');
                    IconName = IconName.Substring(0, pos);
                    //Insert an item into the ListView
                    FileInfo fileFound = null;
                    foreach (KeyValuePair<FileInfo, int> pair in dicoOfFile)
                    {
                        try
                        {
                            if (!Directory.Exists(pair.Key.FullName) && pair.Key.Extension != string.Empty)
                            {
                                //if ((pair.Key.Name.Replace(pair.Key.Extension, "") == IconName &&
                                //    pair.Value == 0) || (pair.Key.Name ==  IconName))
                                if (pair.Key.Name == IconName && pair.Value == 0)
                                {
                                    fileFound = pair.Key;
                                    dicoOfIcons.Add(fileFound, vPoint[0]);
                                    break;
                                }
                            }
                            else
                            {
                                if (pair.Key.Name == IconName && pair.Value == 0)
                                {
                                    fileFound = pair.Key;
                                    dicoOfIcons.Add(fileFound, vPoint[0]);
                                    break;
                                }

                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    foreach (KeyValuePair<FileInfo, int> pair in dicoOfFile)
                    {
                        try
                        {
                            if (!Directory.Exists(pair.Key.FullName) && pair.Key.Extension != string.Empty)
                            {
                                if (pair.Key.Name.Replace(pair.Key.Extension, "") == IconName && pair.Value == 0)
                                {
                                    fileFound = pair.Key;
                                    dicoOfIcons.Add(fileFound, vPoint[0]);
                                    break;
                                }
                            }
                            else
                            {
                                if (pair.Key.Name == IconName && pair.Value == 0)
                                {
                                    fileFound = pair.Key;
                                    dicoOfIcons.Add(fileFound, vPoint[0]);
                                    break;
                                }

                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    if (fileFound != null)
                        dicoOfFile[fileFound] = 1;
                    else
                    {
                        dicoOfSpecialIcons.Add(IconName, vPoint[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                VirtualFreeEx(vProcess, vPointer, 0, MEM_RELEASE);
                CloseHandle(vProcess);
            }
        }

        private void GetFilesAndFolder(string path)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string fileName in files)
            {
                dicoOfFile.Add(new FileInfo(fileName), 0);
                System.Drawing.Icon fileIcon = System.Drawing.Icon.ExtractAssociatedIcon(fileName);
                dicoOfFileIcons.Add(new FileInfo(fileName), fileIcon);
            }

            files = Directory.GetDirectories(path);
            foreach (string fileName in files)
            {
                dicoOfFile.Add(new FileInfo(fileName), 0);
                Console.WriteLine(fileName);
                System.Drawing.Icon fileIcon = GetDirectoryIcon(fileName, IconReader.IconSize.Large, false);
                dicoOfFileIcons.Add(new FileInfo(fileName), fileIcon);
            }
        }



        public static System.Drawing.Icon GetDirectoryIcon(string name, IconReader.IconSize size,
                                              bool linkOverlay)
        {
            Shell32.SHFILEINFO shfi = new Shell32.SHFILEINFO();
            uint flags = Shell32.SHGFI_ICON | Shell32.SHGFI_USEFILEATTRIBUTES;

            if (true == linkOverlay) flags += Shell32.SHGFI_LINKOVERLAY;


            /* Check the size specified for return. */
            if (IconReader.IconSize.Small == size)
            {
                flags += Shell32.SHGFI_SMALLICON; // include the small icon flag

            }
            else
            {
                flags += Shell32.SHGFI_LARGEICON;  // include the large icon flag

            }

            Shell32.SHGetFileInfo(name,
                Shell32.FILE_ATTRIBUTE_DIRECTORY,
                ref shfi,
                (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                flags);


            // Copy (clone) the returned icon to a new object, thus allowing us 

            // to call DestroyIcon immediately

            System.Drawing.Icon icon = (System.Drawing.Icon)
                                 System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();
            User32.DestroyIcon(shfi.hIcon); // Cleanup

            return icon;
        }

        public static System.Drawing.Icon GetFileIcon(string name, IconReader.IconSize size,
                                              bool linkOverlay)
        {
            Shell32.SHFILEINFO shfi = new Shell32.SHFILEINFO();
            uint flags = Shell32.SHGFI_ICON | Shell32.SHGFI_USEFILEATTRIBUTES;

            if (true == linkOverlay) flags += Shell32.SHGFI_LINKOVERLAY;


            /* Check the size specified for return. */
            if (IconReader.IconSize.Small == size)
            {
                flags += Shell32.SHGFI_SMALLICON; // include the small icon flag

            }
            else
            {
                flags += Shell32.SHGFI_LARGEICON;  // include the large icon flag

            }

            Shell32.SHGetFileInfo(name,
                Shell32.FILE_ATTRIBUTE_NORMAL,
                ref shfi,
                (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                flags);


            // Copy (clone) the returned icon to a new object, thus allowing us 

            // to call DestroyIcon immediately

            System.Drawing.Icon icon = (System.Drawing.Icon)
                                 System.Drawing.Icon.FromHandle(shfi.hIcon).Clone();
            User32.DestroyIcon(shfi.hIcon); // Cleanup

            return icon;
        }
    }
}
