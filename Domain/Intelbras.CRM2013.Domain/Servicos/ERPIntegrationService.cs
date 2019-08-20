using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Diagnostics;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ERPIntegrationService
    {
        private static string connStr
        {
            get { return SDKore.Configuration.ConfigurationManager.GetSettingValue("connector"); }
        }

        public static object InserirNoIntegrationLog(string organizationName, string origin, string tableName, string action, DateTime? createdOn, string sXML, int status)
        {
            var sqlConn = new SqlConnection(connStr);

            var sp0 = new SqlParameter("@OrganizationName", SqlDbType.NVarChar);
            var sp1 = new SqlParameter("@Origin", SqlDbType.NVarChar);
            var sp2 = new SqlParameter("@Entity", SqlDbType.NVarChar);
            var sp3 = new SqlParameter("@Action", SqlDbType.NVarChar);
            var sp4 = new SqlParameter("@CreatedOn", SqlDbType.DateTime);
            var sp5 = new SqlParameter("@Message", SqlDbType.NText);
            var sp6 = new SqlParameter("@status", SqlDbType.Int);

            sp0.Value = RemoveWhiteSpaces(organizationName);
            sp1.Value = origin;
            sp2.Value = tableName;
            sp3.Value = action;
            sp4.Value = createdOn;
            sp5.Value = sXML;
            sp6.Value = status;

            var sqlComm = new SqlCommand();
            sqlComm.CommandText = "Insert Into " +
                                    "IntegrationLog (OrganizationName, Origin, entity, action, message , messageDate, status) " +
                                        "Values (@OrganizationName, @Origin, @Entity, @Action, @Message, @CreatedOn, @status) SELECT @@IDENTITY";

            sqlComm.Connection = sqlConn;

            sqlComm.Parameters.Add(sp0);
            sqlComm.Parameters.Add(sp1);
            sqlComm.Parameters.Add(sp2);
            sqlComm.Parameters.Add(sp3);
            sqlComm.Parameters.Add(sp4);
            sqlComm.Parameters.Add(sp5);
            sqlComm.Parameters.Add(sp6);

            object result = null;
            try
            {
                sqlConn.Open();
                result = sqlComm.ExecuteScalar();
            }
            catch (Exception ex)
            {
                //LogService.GravaLog(ex, TipoDeLog.WSIntegradorERP, "InserirNoIntegrationLog(string organizationName, string origin, string tableName, string action, DateTime? createdOn, string sXML, int status)");
            }
            finally
            {
                if (sqlConn != null)
                {
                    if (sqlConn.State == ConnectionState.Open)
                        sqlConn.Close();
                }
            }

            return result;
        }

        private static string RemoveWhiteSpaces(string str)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, " ", String.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
    }
}
