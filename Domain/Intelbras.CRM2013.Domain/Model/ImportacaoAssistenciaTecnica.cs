using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_importacao_assistencia_tecnica")]
    public class ImportacaoAssistenciaTecnica : DomainBase
    {
        public ImportacaoAssistenciaTecnica()
        {

        }

        public ImportacaoAssistenciaTecnica(string organization, bool isOffline)
            : base(organization, isOffline)
        {

        }

        [LogicalAttribute("new_importacao_assistencia_tecnicaid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("new_produtosid")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("new_familia_comercialid")]
        public Lookup FamiliaComercial { get; set; }

        [LogicalAttribute("createdby")]
        public Lookup Usuario { get; set; }

        [LogicalAttribute("new_pesquisa")]
        public string PesquisaFetch { get; set; }

        [LogicalAttribute("new_acao")]
        public int? Acao { get; set; }

        [LogicalAttribute("statuscode")]
        public int? Status { get; set; }
    }
}
