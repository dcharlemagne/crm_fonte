using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;

namespace teste
{
    class Program
    {
        static void Main(string[] args)
        {
            string organizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");  

            List<Ocorrencia> lstOcorrencias = new Intelbras.CRM2013.Domain.Servicos.OcorrenciaService(organizationName, false).ListarOcorrenciasPorDataModificacao();
        }
    }
}
