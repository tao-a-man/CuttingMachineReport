using CuttingMachineReport.Models;
using CuttingMachineReport.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CuttingMachineReport.Data
{
    public class MyDbContextFactory
    {
        private readonly ApiService apiService;

        public MyDbContextFactory(ApiService apiService)
        {
            this.apiService = apiService;
        }

        public async Task<MyDbContext> CreateAsync()
        {
            var configConnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var builder = new SqlConnectionStringBuilder(configConnection);

            var machineName = Environment.MachineName;
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

            var obj = new { MachineName = machineName, Version = version };
            string response = await apiService.GetPcCuttingConnectString(JsonConvert.SerializeObject(obj));

            Root root = JsonConvert.DeserializeObject<Root>(response);
            PcConnectString outputData = JsonConvert.DeserializeObject<PcConnectString>(root.output);

            if (outputData.user_id.Equals("win"))
            {
                builder.DataSource = outputData.server;
                builder.InitialCatalog = outputData.database_name;
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.DataSource = outputData.server + "," + outputData.port;
                builder.InitialCatalog = outputData.database_name;
                builder.UserID = outputData.user_id;
                builder.Password = outputData.password;
            }

            return new MyDbContext(builder.ConnectionString);
        }
    }
}
