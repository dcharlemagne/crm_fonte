using Intelbras.CRM2013.Domain.Servicos;
using System;

namespace Intelbras.CRM2013.Aplication.Metas
{
    public class Program
    {
        #region Objetos
        static MetadaUnidadeService _ServiceMetadaUnidade = null;
        private static MetadaUnidadeService ServiceMetadaUnidade
        {
            get
            {
                if (_ServiceMetadaUnidade == null)
                    _ServiceMetadaUnidade = new MetadaUnidadeService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceMetadaUnidade;
            }
        }
        #endregion

        static int Main(string[] args)
        {
            try
            {
                Console.WriteLine("{0} - Iniciando!", DateTime.Now);

                Principal();

                Console.WriteLine("{0} - Fim!", DateTime.Now);
                return 0;
            }
            catch (Exception ex)
            {
                string mensagem = SDKore.Helper.Error.Handler(ex);
                Console.WriteLine(mensagem);
                return ex.GetHashCode();
            }
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            try
            {
                SDKore.Helper.Error.Create("METAS \nCancelando metas que estavam sendo processadas.", System.Diagnostics.EventLogEntryType.Warning);
                ServiceMetadaUnidade.EndTaskOfWindows();
            }
            catch (Exception ex)
            {
                SDKore.Helper.Error.Handler(ex);
            }
        }

        public static void Principal()
        {
            #region Metas Manual

            Console.WriteLine("{0} - Iniciando Modelo de Metas Canal", DateTime.Now);
            ServiceMetadaUnidade.MetaCanalGerar();
            Console.WriteLine("{0} - Término de Modelo de Metas Canal", DateTime.Now);

            Console.WriteLine("{0} - Iniciando Leitura de Planilha de Metas Canal", DateTime.Now);
            ServiceMetadaUnidade.MetaCanalImportar();
            Console.WriteLine("{0} - Término Leitura de Planilha de Metas Canal", DateTime.Now);

            #endregion

            #region Metas KA/Representante

            Console.WriteLine("{0} - Iniciando Modelo de Metas KA/Representante", DateTime.Now);
            ServiceMetadaUnidade.PotencialKAGerar();
            Console.WriteLine("{0} - Término Modelo de Metas KA/Representante", DateTime.Now);

            Console.WriteLine("{0} - Iniciando Leitura de Planilha de Metas KA/Representante", DateTime.Now);
            ServiceMetadaUnidade.PotencialKAImportar();
            Console.WriteLine("{0} - Término Leitura de Planilha de Metas KA/Representante", DateTime.Now);

            #endregion

            #region Metas Supervisor

            Console.WriteLine("{0} - Iniciando Leitura de Planilha de Metas do Supervisor", DateTime.Now);
            ServiceMetadaUnidade.PotencialSupervisorImportar();
            Console.WriteLine("{0} - Término Leitura de Planilha de Metas do Supervisor", DateTime.Now);

            #endregion

            #region Metas Unidade

            Console.WriteLine("{0} - Iniciando Leitura de Planilha de Metas da Unidade", DateTime.Now);
            ServiceMetadaUnidade.MetaUnidadeImportar();
            Console.WriteLine("{0} - Término Leitura de Planilha de Metas da Unidade", DateTime.Now);

            #endregion
        }
    }
}