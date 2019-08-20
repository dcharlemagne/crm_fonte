using System;
using System.Data.SqlClient;
using System.Data;
using System.Collections;


namespace Intelbras.CRM2013.DAL
{
    public class DataBaseSqlServer
    {
        public DataBaseSqlServer()
        {

        }

        public DataBaseSqlServer(string banco)
        {
            _stringconection = banco;
        }

        private string _stringconection = null;
        private string StringConection
        {
            get
            {
                if (_stringconection == null)
                    _stringconection = SDKore.Configuration.ConfigurationManager.GetSettingValue("dwdatabase");

                return _stringconection;
            }
        }

        private SqlConnection _conexao = null;
        private SqlConnection Conexao
        {
            get
            {
                if (_conexao == null)
                    _conexao = new SqlConnection(StringConection);

                return _conexao;
            }
        }

        private void abrirConexao()
        {
            try
            {
                if (this.Conexao.State != ConnectionState.Open)
                    this.Conexao.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void fecharConexao()
        {
            try
            {
                this.Conexao.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable executeQuery(string sql)
        {
            try
            {
                this.abrirConexao();
                SqlCommand sqlComm = new SqlCommand(sql);
                sqlComm.Connection = this.Conexao;
                sqlComm.CommandTimeout = 0;
                sqlComm.ExecuteNonQuery();

                SqlDataAdapter da = new SqlDataAdapter(sqlComm);

                DataTable dt = new DataTable();
                da.Fill(dt);

                this.fecharConexao();

                return dt;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string executeQueryComStringRetorno(string sql)
        {
            try
            {
                this.abrirConexao();
                string dado;

                SqlCommand sqlComm = new SqlCommand(sql);
                sqlComm.Connection = this.Conexao;

                sqlComm.ExecuteNonQuery();

                SqlDataAdapter da = new SqlDataAdapter(sqlComm);

                DataTable dt = new DataTable();
                da.Fill(dt);

                this.fecharConexao();

                dado = dt.Rows[0][0].ToString();

                return dado;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable executeProcedure(string procedure, ArrayList SqlsParameters)
        {
            this.abrirConexao();

            SqlCommand cmd = new SqlCommand(procedure);
            cmd.Connection = this.Conexao;
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (SqlParameter param in SqlsParameters)
                cmd.Parameters.Add(param);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            this.fecharConexao();

            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }

        public string executeProcedureP_TelefoneAssistencias(string procedure, ArrayList SqlsParameters, string pSaida)
        {
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.CRM2013.Database");
                con.Open();

                SqlCommand cmd = new SqlCommand(procedure);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter param in SqlsParameters)
                    cmd.Parameters.Add(param);

                if (!string.IsNullOrEmpty(pSaida))
                {
                    cmd.Parameters.Add(pSaida, SqlDbType.VarChar, 50);
                    cmd.Parameters[pSaida].Direction = ParameterDirection.Output;
                }

                cmd.ExecuteNonQuery();

                return cmd.Parameters["@P_RESULTADO"].Value.ToString();
            }
            catch
            {
                return "";
            }
        }

    }
}
