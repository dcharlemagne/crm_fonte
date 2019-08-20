using System;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelbras.CRM2013.Aplication.Sellin
{
    class Program
    {
        private static string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        private static bool IsOffline = false;

        static void Main(string[] args)
        {
            SellinService SellinServices = new SellinService(OrganizationName, IsOffline);

            switch (args[0].ToUpper())
            {
                case "ENVIA_REGISTRO_SELLIN_ASTEC":
                    SellinServices.EnviaRegistroSellinFieloAstec();
                    break;
                case "ENVIA_REGISTRO_SELLIN_PROVEDORES_SOLUCOES":
                    SellinServices.EnviaRegistroSellinFieloProvedoresSolucoes();
                    break;
            }
        }
    }
}
