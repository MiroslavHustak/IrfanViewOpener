using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExcelKiller
{
    public static partial class ExcelKiller
    {
        //neautorsky kod
        public static void SimulateKeyStroke(char key, bool ctrl = false, bool alt = false, bool shift = false)
        {
            List<ushort> keys = new List<ushort>();

            if (ctrl)
                keys.Add(VK_CONTROL);

            if (alt)
                keys.Add(VK_MENU);

            if (shift)
                keys.Add(VK_SHIFT);

            keys.Add(char.ToUpper(key));

            INPUT input = new INPUT();
            input.type = INPUT_KEYBOARD;

            int inputSize = Marshal.SizeOf(input);

            for (int i = 0; i < keys.Count; ++i)
            {
                input.mkhi.ki.wVk = keys[i];

                bool isKeyDown = (GetAsyncKeyState(keys[i]) & 0x10000) != 0;

                if (!isKeyDown)
                    SendInput(1, ref input, inputSize);
            }

            input.mkhi.ki.dwFlags = KEYEVENTF_KEYUP;

            for (int i = keys.Count - 1; i >= 0; --i)
            {
                input.mkhi.ki.wVk = keys[i];
                SendInput(1, ref input, inputSize);
            }
        }

        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;

            [FieldOffset(0)]
            public KEYBDINPUT ki;

            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        private struct INPUT
        {
            public int type;
            public MOUSEKEYBDHARDWAREINPUT mkhi;
        }
    }
}
