using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_produtodokit")]
    public class ProdutoKit : DomainBase
    {
        #region Construtores

        public ProdutoKit() { }

        public ProdutoKit(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
        }

        public ProdutoKit(string organization, bool isOffline)
            : base(organization, isOffline)
        {
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_produtodokitid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome {get; set;}

        [LogicalAttribute("itbc_produtopaiid")]
        public Lookup ProdutoPai { get; set; }

        [LogicalAttribute("itbc_ProdutoFilhoId")]
        public Lookup ProdutoFilho { get; set; }

        [LogicalAttribute("itbc_quantidade")]
        public int Quantidade { get; set; }

        [LogicalAttribute("statecode")]
        public int? Status { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }

        #endregion
    }
}