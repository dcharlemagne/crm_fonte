using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_produto_assistecia_tecica")]
    public class ProdutoAssisteciaTecnica : DomainBase
    {
        public ProdutoAssisteciaTecnica()
        {

        }

        public ProdutoAssisteciaTecnica(string organization, bool isOffline)
            : base(organization, isOffline)
        {

        }

        [LogicalAttribute("new_produto_assistecia_tecicaid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("new_assistencia_tecnicaid")]
        public Lookup AssistenciaTecnica { get; set; }

        [LogicalAttribute("new_produtoid")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("statecode")]
        public int? StateCode { get; set; }

        //public void AssistenciaTec()
        //{
        //    #region Update

        //    ////Atualização efetuada com o usuário do configurado no arquivo de configuração.
        //    //var atualizacao = new Exemplo(this.OrganizationName, this.IsOffline)
        //    //{
        //    //    ID = exemploId,
        //    //    Nome = "Atualização"
        //    //};
        //    //this.ProdutoAssisteciaTecica.Update(atualizacao);

        //    ////Sobrecarga (Overload)
        //    ////Atualização efetuada com o usuário informado.
        //    //this.ProdutoAssisteciaTecica.Update(atualizacao, usuarioId);
        //}
    }
}
