using System.Runtime.InteropServices;

namespace ORBIT9000.Engine.GUI.Console
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.Win32.SafeHandles;

    public static class ConsoleHelper
    {
        private const int STD_OUTPUT_HANDLE = -11;

        public static void StartConsoleMode()
        {
            bool consoleAllocated = AllocConsole();

            if (!consoleAllocated)
            {
                // AllocConsole failed. Maybe a console already exists.
                Console.WriteLine("Console allocation failed or already exists.");
                return;
            }

            var stdOutHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (stdOutHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Couldn't get standard output handle.");
            }

            var safeFileHandle = new SafeFileHandle(stdOutHandle, true);
            using var fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            var encoding = Encoding.ASCII;
            using var standardOutput = new StreamWriter(fileStream, encoding)
            {
                AutoFlush = true
            };

            Console.SetOut(standardOutput);
            Console.WriteLine("Console started from DLL or Windows App!");
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
    }

}
