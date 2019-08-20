using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_categoriasdocanal")]
    public class CategoriasCanal : DomainBase
    {
        #region Construtores

            private RepositoryService RepositoryService { get; set; }

        public CategoriasCanal() { }

        public CategoriasCanal(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }

            public CategoriasCanal(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }        

        #endregion


        #region Atributos
            [LogicalAttribute("itbc_categoriasdocanalid")]
            public Guid? ID { get; set; }     

            [LogicalAttribute("itbc_name")]
            public String Nome {get; set;}
  
            [LogicalAttribute("itbc_categoria")]
            public Lookup Categoria { get; set; }
            [LogicalAttribute("itbc_canalid")]
            public Lookup Canal { get; set; }
            private Conta _Conta = null;
            public Domain.Model.Conta ContaObj
            {
                get
                {
                    if (_Conta == null && Canal.Id != null && Canal.Id != Guid.Empty)
                        _Conta = RepositoryService.Conta.Retrieve(Canal.Id);
                    return _Conta;
                }
            }
            [LogicalAttribute("itbc_businessunit")]
            public Lookup UnidadeNegocios { get; set; }

            [LogicalAttribute("itbc_classificacaoid")]
            public Lookup Classificacao { get; set; }

            [LogicalAttribute("itbc_subclassificacaoid")]
            public Lookup SubClassificacao { get; set; }
        #endregion
    }
}
