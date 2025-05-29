using System.Data.Entity;
using System.Reflection;
using System.Data.SqlClient;
using System.Configuration;

public class MyDbContext : DbContext
{
    public MyDbContext(string connectionString) : base(connectionString)
    {
    }
}