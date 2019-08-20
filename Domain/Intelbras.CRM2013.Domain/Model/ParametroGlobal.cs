using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_parmetrosglobais")]
    public class ParametroGlobal : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ParametroGlobal() { }

        public ParametroGlobal(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ParametroGlobal(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_parmetrosglobaisid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("statuscode")]
        public new int? Status { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }

        [LogicalAttribute("itbc_tipodedado")]
        public int? TipoDado { get; set; }

        [LogicalAttribute("itbc_businessunit")]
        public Lookup UnidadeNegocio { get; set; }

        [LogicalAttribute("itbc_valor")]
        public String Valor { get; set; }

        [LogicalAttribute("itbc_nivelposvendaid")]
        public Lookup NivelPosVenda { get; set; }

        [LogicalAttribute("itbc_compromissoid")]
        public Lookup Compromisso { get; set; }

        [LogicalAttribute("itbc_categoriaid")]
        public Lookup Categoria { get; set; }

        [LogicalAttribute("itbc_beneficioid")]
        public Lookup Beneficio { get; set; }

        [LogicalAttribute("itbc_tipoparametroglobalid")]
        public Lookup TipoParametro { get; set; }

        [LogicalAttribute("itbc_classificacaoid")]
        public Lookup Classificacao { get; set; }

        [LogicalAttribute("itbc_parametrizar")]
        public int? Parametrizar { get; set; }
        #endregion

        public T GetValue<T>()
        {
            var foo = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
            return (T)(foo.ConvertFromInvariantString(Valor));
        }

    }
}
