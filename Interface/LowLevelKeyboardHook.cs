using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private System.Windows.Forms.Keys lastKey = System.Windows.Forms.Keys.None;
        private IntPtr HookCallback(int nCode, UIntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (wParam.ToUInt32() == WM_KEYDOWN)
                {
                    System.Windows.Forms.Keys k = (System.Windows.Forms.Keys)wParam.ToUInt32();
                    if (OnKeyPress != null)
                        OnKeyPress(k, lastKey, false);
                    lastKey = k;
                }
                else if (wParam.ToUInt32() == WM_SYSKEYDOWN)
                {
                    System.Windows.Forms.Keys k = (System.Windows.Forms.Keys)wParam.ToUInt32();
                    if (OnKeyPress != null)
                        OnKeyPress(k, lastKey, true);
                    lastKey = k;
                }
            }
            return CallNextHookEx(_Hook, nCode, wParam, lParam);
        }

        public delegate void KeyPressEvent(System.Windows.Forms.Keys key, System.Windows.Forms.Keys lastKey, bool syskey);
        public event KeyPressEvent OnKeyPress;

        private delegate IntPtr LowLevelKeyboardProc(
        int nCode, UIntPtr wParam, IntPtr lParam);

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
    }
}
