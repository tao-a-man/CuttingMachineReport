using Squirrel;
using System;
using System.Collections.Generic;
using System.Linq;
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
        static void Main()
        {

            // Chạy update trước khi mở form
            Task.Run(async () => await CheckForUpdates()).Wait();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static async Task CheckForUpdates()
        {
            try
            {
                using (var mgr = new UpdateManager("https://github.com/tao-a-man/CuttingMachineReport/tree/master/Releases"))
                {
                    await mgr.UpdateApp();
                }
            }
            catch (Exception ex)
            {
                // Log hoặc bỏ qua nếu không thể update
                Console.WriteLine("Update failed: " + ex.Message);
            }
        }
    }
}
