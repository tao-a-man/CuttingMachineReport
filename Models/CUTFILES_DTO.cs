using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuttingMachineReport.Models
{
    public class Root
    {
        public string status { get; set; }
        public string message { get; set; }
        public string output { get; set; }
        public string i18n { get; set; }
    }

    public class OutputData
    {
        public int time_async { get; set; }
        public int time_get_data { get; set; }
    }


    public class CUTFILES_DTO
    {
        public string MachineName { get; set; }
        public int CutfileReportId { get; set; }
        public string CutterName { get; set; }
        public string SystemType { get; set; }
        public string CutfileName { get; set; }
        public DateTime StartTimeStamp { get; set; }
        public DateTime EndTimeStamp { get; set; }
        public int Status { get; set; }
        public int FeedRateTotal { get; set; }
        public int FeedRateCount { get; set; }
        public double CutTime { get; set; }
        public double DryHaulTime { get; set; }
        public double SharpenTime { get; set; }
        public double DryRunTime { get; set; }
        public double BiteTime { get; set; }
        public double InterruptTime { get; set; }
        public double PrepTime { get; set; }
        public double CutDistance { get; set; }
        public double DryHaulDistance { get; set; }
        public double DryRunDistance { get; set; }
        public double BiteDistance { get; set; }
        public int TotalPiecesCut { get; set; }
        public int NumBitesCut { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public int PlyCount { get; set; }
    }
    public class PcConnectString
    {
        public string server { get; set; }
        public string port { get; set; }
        public string database_name { get; set; }
        public string user_id { get; set; }
        public string password { get; set; }
    }
}
