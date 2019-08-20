using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelbras.CRM2013.Aplication.CargaCompromissosCanalTipoTarefa
{
    class Program
    {
        private string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        private Boolean isOffline = false;

        static void Main(string[] args)
        {
            Program prog = new Program();
            prog.Processo();
        }
        protected void Processo()
        {
            new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(this.OrganizationName, this.isOffline).CargaCompromissosCanalPorTarefa();
        }
    }
}
