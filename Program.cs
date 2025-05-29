using Mono.Cecil;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CuttingMachineReport
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            bool createdNew;
            using (Mutex mutex = new Mutex(true, "Global\\CuttingMachineReport", out createdNew))
            {
                if (createdNew)
                {
                    try
                    {
                        using (var mgr = await UpdateManager.GitHubUpdateManager("https://github.com/tao-a-man/CuttingMachineReport"))
                        {
                            await mgr.UpdateApp();
                            UpdateManager.RestartApp();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logg("Update failed", ex);
                    }

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    Application.ThreadException += Application_ThreadException;
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                    Application.Run(new Form1());
                }
                else
                {
                    Logg("Duplicate", new Exception("Exits APP"));
                }
            }
        }
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Logg("UI", e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            Logg("Non-UI", ex);
        }

        public static void Logg(string type, Exception ex)
        {
            string logPath = Path.Combine($@"C:\Users\{Environment.UserName}\", "CDTApp_error_log.txt");
            string errorMessage = $"[{DateTime.Now}] {type} Error: {ex.Message}\n{ex.ToString()}\n{ex.StackTrace}\n";
            try
            {
                File.AppendAllText(logPath, errorMessage);
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"Failed to write log: {logEx.Message}");
            }
        }
        
    }
}
