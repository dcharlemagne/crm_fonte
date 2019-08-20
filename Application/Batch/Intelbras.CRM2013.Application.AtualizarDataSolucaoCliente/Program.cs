using System;
using System.Collections.Generic;
using System.Data;
using Intelbras.CRM2013.Domain.Servicos;
using System.IO;

namespace Intelbras.CRM2013.Aplication.AtualizarDataSolucaoCliente
{
    class Program
    {
        private static string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        private static bool IsOffline = false;
        static void Main(string[] args)
        {
            try
            {
                GravaLog("Inicio rotina : " + DateTime.Now.ToString());
                OcorrenciaService OcorrenciaServices = new OcorrenciaService(OrganizationName, IsOffline);

                OcorrenciaServices.AtualizaDataSolucaoCliente();
                GravaLog("Fim rotina : " + DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                GravaLog("Erro: " + ex.Message);
            }
        }

        protected static void GravaLog(string log)
        {
            using (StreamWriter w = File.AppendText(@"c:\temp\logAtualizaDataSolucao.txt"))
                w.WriteLine(log);
        }
    }
}
