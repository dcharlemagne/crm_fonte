using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_deparadeunidadedokonviva")]
    public class DeParaDeUnidadeDoKonviva : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }


        public DeParaDeUnidadeDoKonviva() { }

        public DeParaDeUnidadeDoKonviva(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public DeParaDeUnidadeDoKonviva(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_deparadeunidadedokonvivaid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_categoriaid")]
        public Lookup Categoria { get; set; }

        [LogicalAttribute("itbc_classificacaoid")]
        public Lookup Classificacao { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_papelnocanalintelbras")]
        public int? PapelNoCanalIntelbras { get; set; }

        [LogicalAttribute("itbc_subclassificacaoid")]
        public Lookup SubClassificacao { get; set; }

        [LogicalAttribute("itbc_tipodedepara")]
        public int? TipoDeDePara { get; set; }

        [LogicalAttribute("itbc_tipoderelacao")]
        public int? TipoDeRelacao { get; set; }

        [LogicalAttribute("itbc_unidadedokonvivaid")]
        public Lookup UnidadeDoKonviva { get; set; }

        #endregion
    }
}
