using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intelbras.CRM2013.Domain;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Aplication.CargaSellOut
{
    class Program
    {
        #region Objetos
        private string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        private Boolean isOffline = false;

        SellOutService _SellOutService = null;
        private SellOutService SellOut
        {
            get
            {
                if (_SellOutService == null)
                    _SellOutService = new SellOutService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), isOffline);

                return _SellOutService;
            }
        }
        #endregion

        static int Main(string[] args)
        {
            try
            {
                Program prog = new Program();
                prog.Processo();
                return 0;
            }
            catch (Exception ex)
            {
                string mensagem = SDKore.Helper.Error.Handler(ex);
                Console.WriteLine(mensagem);
                return ex.GetHashCode();
            }
        }
        public void Processo()
        {
            int atualizacoes;
            Console.WriteLine("Começando Processamento de contas");
            Console.WriteLine(DateTime.Now.ToString());
            atualizacoes = SellOut.CargaSellOutContas();
            Console.WriteLine(atualizacoes + " registros modificados");
            Console.WriteLine("Finalizado Processamento de contas");
            Console.WriteLine(DateTime.Now.ToString());
            Console.WriteLine("Começando Processamento de Produtos");
            atualizacoes = SellOut.CargaSellOutProdutos();
            Console.WriteLine(atualizacoes + " registros modificados");
            Console.WriteLine("Finalizado Processamento de Produtos");
            Console.WriteLine(DateTime.Now.ToString());
            Console.ReadLine();
        }
    }
}
