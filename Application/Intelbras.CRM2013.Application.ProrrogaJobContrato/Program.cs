using System;
using System.Data.SqlClient;

namespace Intelbras.CRM2013.Application.ProrrogaJobContrato
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = SDKore.Configuration.ConfigurationManager.GetDataBase("crm2015db");
            con.Open();
            var querydel = "UPDATE AsyncOperationBase SET PostponeUntil = DATEADD(yy,10,GETDATE()) WHERE AsyncOperationId = '5EB14ECB-60ED-4F59-800D-A6C09455C9D8'";
            SqlCommand commanddel = new SqlCommand(querydel, con);
            commanddel.ExecuteNonQuery();
            con.Close();
        }
    }
}
