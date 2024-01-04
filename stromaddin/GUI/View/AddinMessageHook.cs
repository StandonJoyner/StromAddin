using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace stromaddin.GUI.View
{
    internal class AddinMessageHook
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMessageProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private delegate IntPtr LowLevelMessageProc(int nCode, IntPtr wParam, IntPtr lParam);

        const int HW_GETMESSAGE = 3;    // Hook Code
        const int WM_KEYFIRST = 0x100;  // Key Down Message ID
        const int WM_KEYLAST = 0x109;   // Key Up Message ID
        const int WM_KEYDOWN = 0x100;   // Key Down Message ID
        const int WM_CHAR = 0x102;      // Char Message ID
        const int PM_REMOVE = 0x0001;   // Remove Message Flag

        private LowLevelMessageProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        private IntPtr _handle = IntPtr.Zero;
        private Window _win = null;

        public AddinMessageHook()
        {
            _proc = HookCallback;
        }

        public void HookKeyboard(Window win)
        {
            _hookID = SetHook(_proc);
            WindowInteropHelper helper = new WindowInteropHelper(win);
            _handle = helper.Handle;
            _win = win;
        }

        public void UnHookKeyboard()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private IntPtr SetHook(LowLevelMessageProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                // hook current thread
                return SetWindowsHookEx(HW_GETMESSAGE, proc, IntPtr.Zero, GetCurrentThreadId());
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (int)wParam == PM_REMOVE)
            {
                bool handled = false;
                MSG msg = (MSG)Marshal.PtrToStructure(lParam, typeof(MSG));
                if (msg.hwnd == _handle
                    && msg.message >= WM_KEYFIRST
                    && msg.message <= WM_KEYLAST)
                {
                    _win.Dispatcher.Invoke(() =>
                    {
                        var hwndSource = PresentationSource.FromVisual(_win) as HwndSource;
                        if (hwndSource != null)
                        {
                            ComponentDispatcher.RaiseThreadMessage(ref msg);
                        }
                    });
                    if (msg.message == WM_CHAR)
                    {
                        handled = true;
                    }
                    else if (msg.message == WM_KEYDOWN)
                    {
                        var vk = (int)msg.wParam;
                        char c = (char)vk;
                        bool isCtrlDown = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
                        if (isCtrlDown || vk > 127)
                            handled = true;
                        else if (char.IsSymbol(c) || char.IsLetterOrDigit(c))
                            handled = false;
                        else // other control keys
                            handled = true;
                    }
                }
                if (handled)
                {
                    msg.message = 0;
                    msg.wParam = IntPtr.Zero;
                    msg.lParam = IntPtr.Zero;
                    Marshal.StructureToPtr(msg, lParam, false);
                }
            }
            return CallNextHookEx(_hookID, nCode, (int)wParam, lParam);
        }
    }
}
