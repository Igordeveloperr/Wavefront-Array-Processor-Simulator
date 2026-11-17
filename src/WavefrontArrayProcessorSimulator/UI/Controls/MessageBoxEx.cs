using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaveVS.VSGraphics
{
    public static class MessageBoxEx
    {
        private static Form _owner;
        private static readonly HookProc _hookProc = new HookProc(MessageBoxHookProc);
        private static IntPtr _hHook = IntPtr.Zero;

        public static DialogResult Show(Form owner, string text)
        {
            Initialize(owner);
            return MessageBox.Show(owner, text);
        }

        public static DialogResult Show(Form owner, string text, string caption)
        {
            Initialize(owner);
            return MessageBox.Show(owner, text, caption);
        }

        public static DialogResult Show(Form owner, string text, string caption, MessageBoxButtons buttons)
        {
            Initialize(owner);
            return MessageBox.Show(owner, text, caption, buttons);
        }

        public static DialogResult Show(Form owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            Initialize(owner);
            return MessageBox.Show(owner, text, caption, buttons, icon);
        }

        public static DialogResult Show(Form owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton)
        {
            Initialize(owner);
            return MessageBox.Show(owner, text, caption, buttons, icon, defButton);
        }

        public static DialogResult Show(Form owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton, MessageBoxOptions options)
        {
            Initialize(owner);
            return MessageBox.Show(owner, text, caption, buttons, icon, defButton, options);
        }

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_CALLWNDPROCRET = 12;

        private enum CbtHookAction : int
        {
            HCBT_MOVESIZE = 0,
            HCBT_MINMAX = 1,
            HCBT_QS = 2,
            HCBT_CREATEWND = 3,
            HCBT_DESTROYWND = 4,
            HCBT_ACTIVATE = 5,
            HCBT_CLICKSKIPPED = 6,
            HCBT_KEYSKIPPED = 7,
            HCBT_SYSCOMMAND = 8,
            HCBT_SETFOCUS = 9
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle lpRect);

        [DllImport("user32.dll")]
        private static extern int MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr idHook);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        [StructLayout(LayoutKind.Sequential)]
        private struct CWPRETSTRUCT
        {
            public IntPtr lResult;
            public IntPtr lParam;
            public IntPtr wParam;
            public CbtHookAction message;
            public IntPtr hwnd;
        };

        private static void Initialize(Form owner)
        {
            _owner = owner;
            if (owner is Form form && form.WindowState == FormWindowState.Normal)
            {
                if (_hHook != IntPtr.Zero)
                {
                    throw new NotSupportedException("Multiple calls are not supported");
                }

                _hHook = SetWindowsHookEx(WH_CALLWNDPROCRET, _hookProc, IntPtr.Zero, GetCurrentThreadId());
            }
        }

        private static IntPtr MessageBoxHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return CallNextHookEx(_hHook, nCode, wParam, lParam);
            }

            CWPRETSTRUCT msg = Marshal.PtrToStructure<CWPRETSTRUCT>(lParam);
            IntPtr hook = _hHook;

            if (msg.message == CbtHookAction.HCBT_ACTIVATE)
            {
                try
                {
                    CenterWindow(msg.hwnd);
                }
                finally
                {
                    UnhookWindowsHookEx(_hHook);
                    _hHook = IntPtr.Zero;
                }
            }

            return CallNextHookEx(hook, nCode, wParam, lParam);
        }

        private static void CenterWindow(IntPtr hChildWnd)
        {
            if (_owner is Form form)
            {
                Rectangle recChild = new Rectangle(0, 0, 0, 0);
                _ = GetWindowRect(hChildWnd, ref recChild);

                int width = recChild.Width - recChild.X;
                int height = recChild.Height - recChild.Y;

                int x = form.Left + ((form.Width - width) / 2);
                int y = form.Top + ((form.Height - height) / 2);

                _ = MoveWindow(hChildWnd, x, y, width, height, false);
            }
        }
    }
}
