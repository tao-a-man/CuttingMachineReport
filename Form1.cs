using CuttingMachineReport.Data;
using CuttingMachineReport.Models;
using CuttingMachineReport.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace CuttingMachineReport
{
    public partial class Form1 : Form
    {
        private Thread workerThreadLoadData;
        private NotifyIcon trayIcon;
        private ApiService apiService;

        // time run and time get data
        private int delayMinutes = 1;
        private int timeGetData = 1;

        //Pc info
        private string machineName = Environment.MachineName;
        private string version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        public Form1()
        {
            InitializeComponent();
            trayIcon = new NotifyIcon()
            {
                Icon = new Icon("../../assets/logo.ico"),
                ContextMenuStrip = new ContextMenuStrip(),
                Text = "Background Application",
                Visible = true
            };

            trayIcon.ContextMenuStrip.Items.Add("Exit", null, OnExit);

            string logPath = Path.Combine($@"C:\Users\{Environment.UserName}\", "CDTApp_error_log.txt");

            trayIcon.DoubleClick += (s, e) =>
            {
                if (File.Exists(logPath))
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = logPath,
                        UseShellExecute = true
                    });
                }
            };

            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();

            apiService = new ApiService();
            StartworkerThreadLoadData();
        }
        private void OnExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        private void StartworkerThreadLoadData()
        {
            workerThreadLoadData = new Thread(() =>
            {
                while (true)
                {
                    DoWork();

                    int delayMs;
                    lock (this)
                    {
                        Console.WriteLine("Log");
                        delayMs = delayMinutes * 60 * 1000;
                    }

                    Thread.Sleep(delayMs);
                }
            });

            workerThreadLoadData.IsBackground = true;
            workerThreadLoadData.Start();
        }

        private void DoWork()
        {
            if (this.InvokeRequired)
            {
                if (this.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)(async () =>
                    {
                        var obj = new { MachineName = machineName, Version = version };
                        string response = await apiService.PostPcCuttingInfo(JsonConvert.SerializeObject(obj));

                        Root root = JsonConvert.DeserializeObject<Root>(response);

                        OutputData outputData = JsonConvert.DeserializeObject<OutputData>(root.output);

                        int timeAsync = outputData.time_async;
                        int timeGetDataResponse = outputData.time_get_data;

                        UpdateDelay(timeAsync, timeGetDataResponse);
                        LoadData(timeGetData);
                    }));
                }
                else
                {
                    LoadData(timeGetData);
                }
            }
            else
            {
                LoadData(timeGetData);
            }
        }
        public void UpdateDelay(int newDelay, int timeGetDataIn)
        {
            lock (this)
            {
                delayMinutes = newDelay;
                timeGetData = timeGetDataIn;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (workerThreadLoadData != null && workerThreadLoadData.IsAlive)
            {
                workerThreadLoadData.Abort();
            }
            e.Cancel = true;
        }
        private async void buttonLogin_Click(object sender, EventArgs e)
        {
           
        }
        private async void LoadData(Int32 timeGetDataIn)
        {
            var factory = new MyDbContextFactory(apiService);
            using (var context = await factory.CreateAsync())
            {
                var delayMinuteParam = new SqlParameter("@timeGetData", -timeGetDataIn);
                string sql = @"
                    SELECT top 11 HOST_NAME() AS MachineName, --varchar(20)
                           cci.CutfileReportId, --int
                           cn.CutterName, --nvarchar(1024)
                           cn.SystemType, --nvarchar(1024)
                           c.CutfileName, --nvarchar(1024)
                           cci.StartTimeStamp, --datetime
                           cci.EndTimeStamp, --datetime
                           cci.Status, --int
                           cci.FeedRateTotal, --int
                           cci.FeedRateCount, --int
                           cci.CutTime, --float
                           cci.DryHaulTime, --float
                           cci.SharpenTime, --float
                           cci.DryRunTime, --float
                           cci.BiteTime, --float
                           cci.InterruptTime, --float
                           cci.PrepTime, --float
                           cci.CutDistance, --float
                           cci.DryHaulDistance, --float
                           cci.DryRunDistance, --float
                           cci.BiteDistance, --float
                           cci.TotalPiecesCut, --int
                           cci.NumBitesCut, --int
                           cci.Length, --float
                           cci.Width, --float
                           cci.PlyCount --int
                    FROM dbo.CUTFILE_CUT_INFO AS cci
                        INNER JOIN dbo.CUTFILES AS c
                            ON cci.CutfileId = c.CutfileId
                        INNER JOIN dbo.CUTTER_NAME AS cn
                            ON cci.CutterId = cn.CutterId
                    WHERE cci.EndTimeStamp BETWEEN DATEADD(MINUTE,@timeGetData, GETDATE()) AND GETDATE()
                    ";


                List<CUTFILES_DTO> result = context.Database.SqlQuery<CUTFILES_DTO>(sql, delayMinuteParam).ToList();

                if (result != null)
                {
                    string response = await apiService.PostPcCuttingReportAsync(JsonConvert.SerializeObject(result));
                }
                else
                {
                    Console.WriteLine("data null");
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
