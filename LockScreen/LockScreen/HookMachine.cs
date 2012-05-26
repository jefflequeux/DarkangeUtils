using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows;

namespace LockScreen
{
    public class HookMachine
    {
        MainWindow mainWindow;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(HookType idHook, KeyboardProc lpfn, IntPtr hMod, int dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        public delegate IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        IntPtr hookId;
        KeyboardProc registeredKeyboardProc;

        enum HookType
        {
            WH_KEYBOARD_LL = 13,
        }

        public enum KeyEvent
        {
            WM_KEYDOWN = 256,
        }

        public void HookKeyboard(MainWindow _mainWindow)
        {
            mainWindow = _mainWindow;
            registeredKeyboardProc = MyKeyboardProc;
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    hookId = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, registeredKeyboardProc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        IntPtr MyKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int w = wParam.ToInt32();
                int vkCode = Marshal.ReadInt32(lParam);
                if (w == (int)KeyEvent.WM_KEYDOWN)
                {
                    Key key = KeyInterop.KeyFromVirtualKey(vkCode);

                    if (key == Key.F11)
                    {
                        mainWindow.Close();

                    }
                    if (key == Key.A)
                    {
                        MessageBox.Show("ok");
                    }
                }
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        public void UnhookKeyboard()
        {
            UnhookWindowsHookEx(hookId);
        }
    }
}
