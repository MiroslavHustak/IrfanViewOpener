using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExcelKiller
{
    public static partial class ExcelKiller
    {
        private const string processName = "Excel";
        private const int numberOfAttempts = 42;

        private static readonly string[] buttonNames = new string[]
            {
                "OK", "Yes", "&Yes", "Ano", "&Ano", "&Uložit", "Uložit", "Save", "&Save"
            };

        //private const string desktopWindowClassName = "#32769";
        //private const string dialogBoxClassName = "#32770";
        private static readonly string[] classNames = new string[]
            {
                "#32769", "#32770", "Message"
            };

        //"Microsoft Office Excel" pro Office 2007 //"Microsoft Excel" pro Office 2010
        private static readonly string[] windowNames = new string[]
            {
                "Microsoft Office Excel", "Microsoft Excel"
            };

        //********Windows API**********
        private const int HWND_TOPMOST = -1;
        //private const int HWND_NOTOPMOST = -2;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;

        private const int WM_CLOSE = 16;
        private const int WM_DESTROY = 0x0002;
        private const uint WM_LBUTTONDOWN = 0x0201;
        private const uint WM_LBUTTONUP = 0x0202;
        private const uint WM_KEYDOWN = 0x13;
        private const uint WM_KEYUP = 0x13;

        const int INPUT_KEYBOARD = 1;
        const uint KEYEVENTF_KEYUP = 0x0002;

        const ushort VK_SHIFT = 0x10;
        const ushort VK_CONTROL = 0x11;
        const ushort VK_MENU = 0x12;

        //The name user32.dll is misleading. It's the 64 bit version of user32.dll you're calling. 
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos
            (
            IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags
            );

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(ushort vKey);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        public static void SaveAndCloseExcel()
        {
            System.Diagnostics.Process[] Process = System.Diagnostics.Process.GetProcessesByName(processName);

            Process.Where(item => !string.IsNullOrEmpty(item.ProcessName)).ToList().ForEach(item => GetHandle(item));

            void GetHandle(System.Diagnostics.Process p)
            {
                //if (p.MainWindowTitle.Contains(excelFileName))//TODO
                if (true)//V Krnove je p.MainWindowTitle = null, divne. Asi tim, ze maji W7... 
                {
                    //vsimni si zpusobu ziskani handle !!!  IntPtr hwnd = p.MainWindowHandle

                    SetWindowPos(p.MainWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);

                    SaveAndCloseExcelApp(p.MainWindowHandle, p.MainWindowTitle);
                }
            }
        }

        private static void SaveAndCloseExcelApp(IntPtr hwnd, string mwt)
        {
            //You can use IntPtr.Size to find out whether you're running in a 32-bit or 64-bit process, as it will be 4 or 8 bytes respectively.
            //IntPtr hwndChild = IntPtr.Zero; 
            int counter = 0;

            while (System.Diagnostics.Process.GetProcessesByName(processName).Length > 0)
            {
                ++counter;

                //Toto jako prvni, pro pripad, ze Excel by mel otevrene nejake okenko 
                classNames.ToList().ForEach(item => windowNames.ToList().ForEach(item1 => GetHandleAndClickOnMessageBox(item, item1)));

                //Tohle az po zavreni pripadnych okenek
                SaveExcelFile();

                MakeAClick(hwnd);//Klikneme pro jistotu do okna Excelu                 

                SendMessage(hwnd, WM_CLOSE, 0, IntPtr.Zero);//Klikneme na krizek 

                classNames.ToList().ForEach(item => windowNames.ToList().ForEach(item1 => GetHandleAndClickOnMessageBox(item, item1)));

                if (mwt.Contains("csv"))
                {
                    SendMessage(hwnd, WM_CLOSE, 0, IntPtr.Zero);//asi to nepomoze, bo csv v Excelu ma udajne bug
                    SendMessage(hwnd, WM_DESTROY, 0, IntPtr.Zero);
                    break;
                }

                if (counter > numberOfAttempts)//Po nejakem mnozstvi pokusu to vzdam...
                {
                    SendMessage(hwnd, WM_DESTROY, 0, IntPtr.Zero);
                    break;
                }
            }

            //To same, co WM_DESTROY, ale pro jistotu, kdyby neco...
            if (System.Diagnostics.Process.GetProcessesByName(processName).Length > 0)
            {
                KillSingleProcess.Process.KillSingleProcess(processName, String.Empty, false); //Dll F#
            } 

            //**********lokalni metody***************
            void SaveExcelFile()
            {
                SetForegroundWindow(hwnd);//okno Excelu do popredi
                MakeAClick(hwnd);//Klikneme pro jistotu do okna Excelu   
                SimulateKeyStroke('s', ctrl: true);//CTRL+S 
            }

            void ClickOnIt(IntPtr hwndX)
            {
                //...klikneme na vynorivsi se nadavajici okno
                MakeAClick(hwndX);

                //a klikneme na tlacitka, ktera by se mohla objevit (obecne zjistit pomoci Spy++)                         
                buttonNames.ToList().ForEach(item => ClickOnButton(hwndX, "Button", item));
            }

            void GetHandleAndClickOnMessageBox(string lpClassName, string lpWindowName)
            {
                int counter1 = 0; IntPtr hwndX = IntPtr.Zero;

                //Pozor na zpozdeni, program nesmi zaviset na momentalnim proces == 0 bez moznosti opakovani cyklu
                if (System.Diagnostics.Process.GetProcessesByName(processName).Length > 0)
                {
                    hwndX = FindWindow(lpClassName, lpWindowName);

                    ClickOnIt(hwndX);

                    //a znovu
                    hwndX = FindWindow(lpClassName, lpWindowName);

                    while (hwndX != (IntPtr)0x0000000000000000)
                    {
                        ++counter1;

                        ClickOnIt(hwndX);

                        if (counter1 > numberOfAttempts)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private static void ClickOnButton(IntPtr hwndX, string className, string windowTitle)
        {
            IntPtr hwndChild = IntPtr.Zero;

            hwndChild = FindWindowEx((IntPtr)hwndX, IntPtr.Zero, className, windowTitle);//viz Spy++

            MakeAClick(hwndChild);

            SendMessage(hwndX, WM_CLOSE, 0, IntPtr.Zero);
            MakeAClick(hwndChild);
        }

        private static void MakeAClick(IntPtr hwnd)
        {
            SendMessage(hwnd, WM_LBUTTONDOWN, 0, IntPtr.Zero);
            SendMessage(hwnd, WM_LBUTTONUP, 0, IntPtr.Zero);
            SendMessage(hwnd, WM_KEYDOWN, 0, IntPtr.Zero);
            SendMessage(hwnd, WM_KEYUP, 0, IntPtr.Zero);
        }
    }
}


