using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Intelbras.CRM2013.Aplication.Orcamentos
{
    public class Program
    {
        #region Objetos
        OrcamentodaUnidadeService _ServiceOrcamentodaUnidade = null;
        private OrcamentodaUnidadeService ServiceOrcamentodaUnidade
        {
            get
            {
                if (_ServiceOrcamentodaUnidade == null)
                    _ServiceOrcamentodaUnidade = new OrcamentodaUnidadeService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceOrcamentodaUnidade;
            }
        }
        #endregion

        static int Main(string[] args)
        {
            try
            {
                EventLog.WriteEntry("Task Orcamento", "Iniciando");
                new Program().Principal();

                //Mesmo quando roda normal, após ele entra neste método, caso a task trabalhe thread achar outro tipo
                //AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

                return 0;
            }
            catch (Exception erro)
            {
                EventLog.WriteEntry("Task Orcamento Erro", erro.Message);
                return erro.GetHashCode();
            }
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            new Program().EndTask();
            EventLog.WriteEntry("EndTask Orcamento", "Cancelando Orçamento que estavam sendo processadas.");
        }

        public void EndTask()
        {
            try
            {
                ServiceOrcamentodaUnidade.EndTaskOfWindows();
            }
            catch (Exception erro)
            {
                EventLog.WriteEntry("EndTask Orcamento", erro.Message);
            }
        }

        public void Principal()
        {
            try
            {
                #region Orçamento
                Console.WriteLine("Gerando Modelo de Orçamento: " + DateTime.Now.ToString());
                ServiceOrcamentodaUnidade.GerarPlanilhaOrcamento();
                Console.WriteLine("Término Geração de Modelo Orçamento: " + DateTime.Now.ToString());
                Console.WriteLine();

                Console.WriteLine("Iniciando Leitura de Planilha Orçamento: " + DateTime.Now.ToString());
                ServiceOrcamentodaUnidade.ProcessarPlanilhaOrcamento();
                Console.WriteLine("Término Leitura de Planilha Orçamento: " + DateTime.Now.ToString());
                Console.WriteLine();
                #endregion

                #region Orçamento Manual
                Console.WriteLine("Gerando Modelo de Orçamento Manual: " + DateTime.Now.ToString());
                ServiceOrcamentodaUnidade.GerarPlanilhaOrcamentoManual();
                Console.WriteLine("Término Geração de Modelo Orçamento Manual: " + DateTime.Now.ToString());
                Console.WriteLine();

                Console.WriteLine("Iniciando Leitura de Planilha Orçamento Manual: " + DateTime.Now.ToString());
                ServiceOrcamentodaUnidade.ProcessarPlanilhaOrcamentoManual();
                Console.WriteLine("Término Leitura de Planilha Orçamento Manual: " + DateTime.Now.ToString());
                Console.WriteLine();
                #endregion

                Console.Clear();
            }
            catch (Exception erro)
            {
                Console.WriteLine(erro.Message);
            }
        }


    }
}
