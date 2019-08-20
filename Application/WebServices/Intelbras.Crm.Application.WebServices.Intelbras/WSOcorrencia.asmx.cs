using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using Intelbras.CRM2013.Domain;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Application.WebServices.Intelbras
{

    [WebService(Namespace = "http://schemas.microsoft.com/crm/2009/WebServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WSOcorrencia : WebServiceBase
    {

        [WebMethod]
        public XmlDocument RecalcularValorExtratoDePagamentoOcorrencia(string new_extrato_pagamento_ocorrenciaid)
        {
            try
            {
                Extrato extrato = new Extrato(nomeOrganizacao, false) { Id = new Guid(new_extrato_pagamento_ocorrenciaid) };
                extrato.AtualizarValor();
                (new Domain.Servicos.RepositoryService()).Extrato.Update(extrato);

                base.Mensageiro.AdicionarTopico("Sucesso", true);
                return base.Mensageiro.Mensagem;
            }
            catch (Exception ex)
            {
                //LogService.GravaLog(ex, TipoDeLog.WSIntelbras_WSOcorrencia, string.Format("Metodo: RecalcularValorExtratoDePagamentoOcorrencia(string new_extrato_pagamento_ocorrenciaid) \n Parametro: {0} - {1}", new_extrato_pagamento_ocorrenciaid, organizacaoNome));
                return base.TratarErro("Não foi possível concluir a operação!", ex, "RecalcularValorExtratoDePagamentoOcorrencia(string new_extrato_pagamento_ocorrenciaid)");
            }
        }
    }
}
