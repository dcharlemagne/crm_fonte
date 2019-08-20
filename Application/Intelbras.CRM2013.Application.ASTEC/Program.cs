using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Intelbras.CRM2013.Application.ASTEC
{
    class Program
    {
        static int Main(string[] args)
        {
            var acao = "PedidoASTEC";
            if (args != null && args.Length > 0)
                acao = args[0];
            if (acao.ToLower() == ("PedidoASTEC").ToLower())
                new PedidoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false).ExportaPedidos();
            if (acao.ToLower() == ("ExtratoASTEC").ToLower())
            {
                string mensagem = new ExtratoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false).GeraExtratoASTEC(DateTime.Now);
                SDKore.Helper.Log.Logar("pedidoasteclog.txt",mensagem);
            }
            return 0;
        }
    }
}