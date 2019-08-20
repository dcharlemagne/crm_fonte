using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_compdocanal")]
    public class CompromissosDoCanal : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public CompromissosDoCanal() { }

        public CompromissosDoCanal(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public CompromissosDoCanal(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

            [LogicalAttribute("itbc_compdocanalid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_name")]
            public String Nome { get; set; }

            [LogicalAttribute("itbc_canalid")]
            public Lookup Canal { get; set; }

            [LogicalAttribute("itbc_compdoprogid")]
            public Lookup Compromisso { get; set; }

            [LogicalAttribute("itbc_statuscompromissosid")]
            public Lookup StatusCompromisso { get; set; }

            [LogicalAttribute("itbc_businessunitid")]
            public Lookup UnidadeDeNegocio { get; set; }

            [LogicalAttribute("itbc_cumprir_compromisso_em")]
            public DateTime? CumprirCompromissoEm { get; set; }

            [LogicalAttribute("itbc_validade")]
            public DateTime? Validade { get; set; }
    
            private CompromissosDoPrograma _CompromissosDoPrograma = null;
            public CompromissosDoPrograma CompromissosDoPrograma
            {
                get
                {
                    if (_CompromissosDoPrograma == null && ID.Value != Guid.Empty)
                        _CompromissosDoPrograma = RepositoryService.CompromissosPrograma.ObterPor(this.Compromisso.Id);

                    return _CompromissosDoPrograma;
                }
            }


        #endregion
    }
}
