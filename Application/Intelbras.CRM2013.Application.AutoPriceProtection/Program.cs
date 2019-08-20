using Intelbras.CRM2013.Domain.Servicos;
using System;

namespace Intelbras.CRM2013.Application.AutoPriceProtection
{
    public class Program
    {
        #region Objetos
        static SolicitacaoBeneficioService _ServiceSolicBenef = null;
        private static SolicitacaoBeneficioService ServiceSolicBenef
        {
            get
            {
                if (_ServiceSolicBenef == null)
                    _ServiceSolicBenef = new SolicitacaoBeneficioService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceSolicBenef;
            }
        }
        #endregion

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("{0} - Iniciando!", DateTime.Now);

                Principal();

                Console.WriteLine("{0} - Fim!", DateTime.Now);
            }
            catch (Exception ex)
            {
                string mensagem = SDKore.Helper.Error.Handler(ex);
                Console.WriteLine(mensagem);
            }
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            try
            {
                SDKore.Helper.Error.Create("AutoPriceProtection \nCancelando calculos de Price Protection que estavam sendo processados.", System.Diagnostics.EventLogEntryType.Warning);
                ServiceSolicBenef.EndTaskOfWindows();
            }
            catch (Exception ex)
            {
                SDKore.Helper.Error.Handler(ex);
            }
        }

        public static void Principal()
        {
            #region Metas

            //Console.WriteLine("{0} - Iniciando Leitura de Planilha de Metas da Unidade", DateTime.Now);
            //ServiceMetadaUnidade.MetaUnidadeImportar();
            //Console.WriteLine("{0} - Término Leitura de Planilha de Metas da Unidade", DateTime.Now);

            //#endregion

            //#region Metas Manual

            //Console.WriteLine("{0} - Iniciando Modelo de Metas Canal", DateTime.Now);
            //ServiceMetadaUnidade.MetaCanalGerar();
            ////ServiceMetadaUnidade.GerarPlanilhaMetaManual();
            //Console.WriteLine("{0} - Término de Modelo de Metas Canal", DateTime.Now);

            //Console.WriteLine("{0} - Iniciando Leitura de Planilha de Metas Canal", DateTime.Now);
            //ServiceMetadaUnidade.MetaCanalImportar();
            ////ServiceMetadaUnidade.ProcessarPlanilhaMetaManual();
            //Console.WriteLine("{0} - Término Leitura de Planilha de Metas Canal", DateTime.Now);

            //#endregion

            //#region Metas KA/Representante

            //Console.WriteLine("{0} - Iniciando Modelo de Metas KA/Representante", DateTime.Now);
            //ServiceMetadaUnidade.PotencialKAGerar();
            //Console.WriteLine("{0} - Término Modelo de Metas KA/Representante", DateTime.Now);

            //Console.WriteLine("{0} - Iniciando Leitura de Planilha de Metas KA/Representante", DateTime.Now);
            //ServiceMetadaUnidade.PotencialKAImportar();
            //Console.WriteLine("{0} - Término Leitura de Planilha de Metas KA/Representante", DateTime.Now);

            //#endregion

            //#region Metas Supervisor

            //Console.WriteLine("{0} - Iniciando Leitura de Planilha de Metas do Supervisor", DateTime.Now);
            //ServiceMetadaUnidade.PotencialSupervisorImportar();
            //Console.WriteLine("{0} - Término Leitura de Planilha de Metas do Supervisor", DateTime.Now);

            #endregion
        }
    }
}