using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Arya.Interface
{
    class LowLevelKeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104; // Occurs while ALT key was held down.
        private const int WM_SYSKEYUP = 0x0105;
        private LowLevelKeyboardProc _HookCallback;
        private IntPtr _Hook = IntPtr.Zero;

        public LowLevelKeyboardHook()
        {
            _HookCallback = new LowLevelKeyboardProc(HookCallback);
            Hook();
        }
        ~LowLevelKeyboardHook()
        {
            Dispose();
        }

        public void Dispose()
        {
            Unhook();
        }

        public void Hook()
        {
            if (_Hook != IntPtr.Zero)
                Unhook();
            _Hook = SetHook(_HookCallback);
        }
        public void Unhook()
        {
            if(_Hook == IntPtr.Zero)
                return;
            UnhookWindowsHookEx(_Hook);
            _Hook = IntPtr.Zero;
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process cProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule cModule = cProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(cModule.ModuleName), 0);
                }
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (nCode >= 0)
            {
                if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    Keys key = (Keys)vkCode;
                    if (key == Keys.LControlKey ||
                        key == Keys.RControlKey)
                    {
                        key = key | Keys.Control;
                    }

                    if (key == Keys.LShiftKey ||
                        key == Keys.RShiftKey)
                    {
                        key = key | Keys.Shift;
                    }

                    if (key == Keys.LMenu ||
                        key == Keys.RMenu)
                    {
                        key = key | Keys.Alt;
                    }

                    if (OnKeyDown != null)
                        OnKeyDown(key, wParam == (IntPtr)WM_SYSKEYDOWN);
                }
                else if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    Keys key = (Keys)vkCode;
                    if (key == Keys.LControlKey ||
                        key == Keys.RControlKey)
                    {
                        key = key | Keys.Control;
                    }

                    if (key == Keys.LShiftKey ||
                        key == Keys.RShiftKey)
                    {
                        key = key | Keys.Shift;
                    }

                    if (key == Keys.LMenu ||
                        key == Keys.RMenu)
                    {
                        key = key | Keys.Alt;
                    }

                    if (OnKeyUp != null)
                        OnKeyUp(key, wParam == (IntPtr)WM_SYSKEYUP);
                }
            }
            return CallNextHookEx(_Hook, nCode, wParam, lParam);
            //return (IntPtr)1;     // Call this to not forward key to other applications.
        }

        public delegate void KeyPressEvent(Keys key, bool syskey);
        public event KeyPressEvent OnKeyDown;
        public event KeyPressEvent OnKeyUp;

        private delegate IntPtr LowLevelKeyboardProc(
        int nCode, IntPtr wParam, IntPtr lParam);

        struct KBDLLHOOKSTRUCT
        {
            int vkCode;
            int scanCode;
            int flags;
            int time;
            IntPtr dwExtraInfo;
        }

        #region DllImport

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }
}
