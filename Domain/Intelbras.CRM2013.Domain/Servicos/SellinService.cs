using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos.Docs;
using Intelbras.CRM2013.Domain.ViewModels;
using Microsoft.Xrm.Sdk;
using SDKore.Helper;
using SDKore.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using System.IO;
using System.Data.Common;
using System.Net;
using System.Text;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class SellinService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SellinService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public SellinService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public SellinService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Métodos
        public void EnviaRegistroSellinFieloAstec()
        {
            #region recupera parametro global de data de envio

            var parametroGlobal = new ParametroGlobalService(RepositoryService).ObterPor((int)TipoParametroGlobal.DataEnvioRegistroSellinFielo);
            if (parametroGlobal == null)
            {
                throw new ArgumentException("(CRM) Parâmetro Global(" + (int)TipoParametroGlobal.DataEnvioRegistroSellinFielo + ") não encontrado!");
            }
            var dataConsulta = Convert.ToDateTime(parametroGlobal.Valor);
            #endregion

            try
            {
                if (dataConsulta.Date == DateTime.Now.Date)
                {
                    #region Recuperar revendas a enviar.
                    DateTime database = dataConsulta.AddMonths(-1);

                    String dt_inicio = new DateTime(database.Year, database.Month, 1).ToString("yyyy-MM-dd HH:mm:ss");
                    String dt_fim = new DateTime(database.Year, database.Month, DateTime.DaysInMonth(database.Year, database.Month)).ToString("yyyy-MM-dd HH:mm:ss");

                    DataTable dtSellin = RepositoryService.Conta.ObterSellinAstec(dt_inicio, dt_fim);
                    #endregion

                    #region Montar CSV
                    string data = DateTime.Now.ToString();
                    data = data.Replace("/", "-").Replace(":", "-");
                    string nomeArquivo = "SELLIN_" + data + ".csv";

                    if (dtSellin != null)
                    {
                        StreamWriter csvArquivo = new StreamWriter(@"c:\\temp\\" + nomeArquivo, false, Encoding.UTF8);

                        //Preenche primeira linha da planilha com o cabeçalho
                        const string aspas = "\"";

                        csvArquivo.WriteLine("SellinId,IdRevendaCRM,CodigoEmitente,Emitente,CompAno,CompMes,TipoOperacao,Item,Valor");

                        int linha = 2;
                        foreach (DataRow item in dtSellin.Rows)
                        {
                            //Adiciona linhas de Sellin da Revenda na planilha do Excell
                            string line = aspas;
                            line += item.Field<string>("SellinId");
                            line += aspas + "," + aspas;
                            if (item.Field<string>("IdRevendaCRM") != "")
                            {
                                line += item.Field<string>("IdRevendaCRM");
                            }
                            line += aspas + "," + aspas;
                            line += item.Field<int>("CodigoEmitente");
                            line += aspas + "," + aspas;
                            if (item.Field<string>("Emitente") != "")
                            {
                                line += item.Field<string>("Emitente");
                            }
                            line += aspas + "," + aspas;
                            line += item.Field<Int16>("CompAno");
                            line += aspas + "," + aspas;
                            line += item.Field<byte>("CompMes");
                            line += aspas + "," + aspas;
                            if (item.Field<string>("TipoOperacao") != "")
                            {
                                line += item.Field<string>("TipoOperacao");
                            }
                            line += aspas + "," + aspas;
                            if (item.Field<string>("Item") != "")
                            {
                                line += item.Field<string>("Item");
                            }
                            line += aspas + "," + aspas;
                            line += item.Field<decimal>("Valor");
                            line += aspas;

                            csvArquivo.WriteLine(line);
                            linha++;
                        }
                        csvArquivo.Close();
                    }
                    #endregion

                    #region Enviar para Fielo via FTP
                    this.enviaArquivoFieloFTP(nomeArquivo);
                    #endregion

                    #region Atualiza data do próximo envio Sellin

                    if (DateTime.Now.Month == 12)
                    {
                        parametroGlobal.Valor = DateTime.Now.Day + "/" + 01 + "/" + (DateTime.Now.Year + 1);
                    }
                    else
                    {
                        parametroGlobal.Valor = DateTime.Now.Day + "/" + (DateTime.Now.Month + 1) + "/" + DateTime.Now.Year;
                    }

                    RepositoryService.ParametroGlobal.Update(parametroGlobal);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("(CRM) ERRO Geração Sellin : " + ex.Message);
            }
        }

        private void enviaArquivoFieloFTP(string nomeArquivo)
        {
            try
            {
                string EnderecoFTP = SDKore.Configuration.ConfigurationManager.GetSettingValue("EnderecoFTP");
                string UsuarioFTP = SDKore.Configuration.ConfigurationManager.GetSettingValue("UsuarioFTP");
                string SenhaFTP = SDKore.Configuration.ConfigurationManager.GetSettingValue("SenhaFTP");

                FtpWebRequest ftpRequest;
                FtpWebResponse ftpResponse;

                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(EnderecoFTP + nomeArquivo));
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.Proxy = null;
                ftpRequest.UseBinary = true;
                ftpRequest.Credentials = new NetworkCredential(UsuarioFTP, SenhaFTP);

                //Seleção do arquivo a ser enviado
                FileInfo arquivo = new FileInfo("c:\\temp\\" + nomeArquivo);
                byte[] fileContents = new byte[arquivo.Length];

                using (FileStream fr = arquivo.OpenRead())
                {
                    fr.Read(fileContents, 0, Convert.ToInt32(arquivo.Length));
                }

                using (Stream writer = ftpRequest.GetRequestStream())
                {
                    writer.Write(fileContents, 0, fileContents.Length);
                }

                //obtem o FtpWebResponse da operação de upload
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            }
            catch (WebException webex)
            {
                throw new ArgumentException(webex.Message);
            }
        }
        
        public void EnviaRegistroSellinFieloProvedoresSolucoes()
        {
            #region recupera parametro global de data de envio
            var parametroGlobal = new ParametroGlobalService(RepositoryService).ObterPor((int)TipoParametroGlobal.DataEnvioRegistroSellinProvedoresSolucoesFielo);
            if (parametroGlobal == null)
            {
                throw new ArgumentException("(CRM) Parâmetro Global(" + (int)TipoParametroGlobal.DataEnvioRegistroSellinProvedoresSolucoesFielo + ") não encontrado!");
            }
            var dataConsulta = Convert.ToDateTime(parametroGlobal.Valor);
            #endregion

            try
            {
                if (dataConsulta.Date == DateTime.Now.Date)
                {
                    #region Recuperar revendas a enviar.
                    DateTime database = dataConsulta.AddMonths(-1);
                    String dt_inicio = new DateTime(database.Year, database.Month, 1).ToString("yyyy-MM-dd HH:mm:ss");
                    String dt_fim = new DateTime(database.Year, database.Month, DateTime.DaysInMonth(database.Year, database.Month)).ToString("yyyy-MM-dd HH:mm:ss");
                    DataTable dtSellin = RepositoryService.Conta.ObterSellinProvedoresSolucoes(dt_inicio, dt_fim);
                    #endregion

                    #region Montar CSV
                    string data = DateTime.Now.ToString();
                    data = data.Replace("/", "-").Replace(":", "-");
                    string nomeBaseArquivo = "SELLIN_PROVEDORES_SOLUCOES" + data + ".csv";
                    string nomeArquivo = "c:\\temp\\"+nomeBaseArquivo;

                    if (dtSellin != null)
                    {
                        StringBuilder sb = new StringBuilder(); 
                        string[] columnNames = dtSellin.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
                        sb.AppendLine(string.Join(",", columnNames));

                        foreach (DataRow row in dtSellin.Rows)
                        {
                            string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                            sb.AppendLine(string.Join(",", fields));
                        }

                        File.WriteAllText(nomeArquivo, sb.ToString(), Encoding.UTF8);

                        #region Enviar para Fielo via FTP
                        //this.enviaArquivoFieloFTP(nomeBaseArquivo);
                        // TODO: copia arquivo para uma pasta para conferencia, apos fase de teste, usar a funcao comentada a cima
                        System.IO.File.Copy(nomeArquivo, "c:\\ArquivoSellout-PCI\\"+nomeBaseArquivo, true);
                        #endregion
                    }
                    #endregion

                    #region Atualiza data do próximo envio Sellin
                    dataConsulta = dataConsulta.AddMonths(1);
                    parametroGlobal.Valor = dataConsulta.GetDateTimeFormats('d')[0];
                    RepositoryService.ParametroGlobal.Update(parametroGlobal);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("(CRM) ERRO Geração Sellin : " + ex.Message);
            }
        }

        #endregion

    }
}
