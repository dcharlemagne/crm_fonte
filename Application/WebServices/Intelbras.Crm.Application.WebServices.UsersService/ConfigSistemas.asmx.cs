using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Web.Services.Protocols;
using Intelbras.CRM2013.Domain;

namespace Intelbras.CRM2013.Application.WebServices.UsersService
{
    /// <summary>
    /// Summary description for ConfigSistemas
    /// </summary>
    [WebService(Namespace = "http://schemas.microsoft.com/crm/2009/WebServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class ConfigSistemas : WebServiceBase
    {
        [WebMethod]
        public XmlDocument ObterConfiguracoesPorChaveXML(string chave)
        {
            try
            {
                var configuracao = SDKore.Configuration.ConfigurationManager.GetSettingValue(chave);

                base.Mensageiro.AdicionarTopico("Atributo", chave);
                base.Mensageiro.AdicionarTopico("Valor", configuracao);
                base.Mensageiro.AdicionarTopico("Grupo", "");
                base.Mensageiro.AdicionarTopico("Sucesso", true);
            }
            catch (Exception ex) { base.TratarErro("Operação não executada. Por favor, feche o formulário e tente novamente. Se o problema persistir, contate o administrador do Sistema.", ex, "ObterConfiguracoesPorChaveXML"); }

            return base.Mensageiro.Mensagem;
        }
    }
}
