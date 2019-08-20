using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_denuncia")]
    public class Denuncia:DomainBase
    {
        #region Construtores

            private RepositoryService RepositoryService { get; set; }

        public Denuncia() { }

        public Denuncia(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }


            public Denuncia(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_denunciaid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_acaotomada")]
            public int? AcaoTomada { get; set; }

            [LogicalAttribute("itbc_account")]
            public Lookup Denunciado { get; set; }

            [LogicalAttribute("itbc_compromissos")]
            public Lookup Compromisso { get; set; }

            [LogicalAttribute("itbc_datacumprimento")]
            public DateTime? DataCumprimento { get; set; }

            [LogicalAttribute("itbc_denunciante")]
            public String Denunciante { get; set; }

            [LogicalAttribute("itbc_descricao")]
            public String Descricao { get; set; }

            [LogicalAttribute("itbc_justificativa")]
            public String Justificativa { get; set; }

            [LogicalAttribute("itbc_name")]
            public String Nome { get; set; }

            [LogicalAttribute("itbc_tipododenunciante")]
            public int? TipoDenunciante { get; set; }

            [LogicalAttribute("itbc_keyaccountourepresentanteid")]
            public Lookup Representante { get; set; }

            [LogicalAttribute("itbc_colaboradordocanalid")]
            public Lookup ColaboradorCanal { get; set; }

            [LogicalAttribute("itbc_colaboradorintelbrasid")]
            public Lookup ColaboradorIntebras { get; set; }
            
            [LogicalAttribute("itbc_canaldenuncianteid")]
            public Lookup CanalDenunciante { get; set; }

            [LogicalAttribute("itbc_tipodedenunciaid")]
            public Lookup TipoDenuncia { get; set; }

            [LogicalAttribute("createdon")]
            public DateTime? DataCriacao { get; set; }

        #endregion
    }
}
