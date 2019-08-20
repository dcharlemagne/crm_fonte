using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Intelbras.CRM2013.Application.AtualizarTimesDosCanais
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static int Main()
        {
            try
            {
                string organizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
                var relacionamentoCanalService = new Intelbras.CRM2013.Domain.Servicos.RelacionamentoCanalService(organizationName, false);
                var CanaisExistentes = relacionamentoCanalService.ListarAtivos();
                foreach (Domain.Model.RelacionamentoCanal relacionamentoCanal in CanaisExistentes)
                {
                    //Verifica se o relacionamento do canal expirou (data atual é maior que a final
                    if (relacionamentoCanal.DataFinal.Value < DateTime.Now.Date)
                    {
                        relacionamentoCanalService.MudarStatus(relacionamentoCanal.ID.Value, (int)Intelbras.CRM2013.Domain.Enum.StateCode.Inativo);
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                string mensagem = SDKore.Helper.Error.Handler(ex);
                Console.WriteLine(mensagem);
                return ex.GetHashCode();
            }
        }
    }
}
