using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_benefdocanal")]
    public class BeneficioDoCanal : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public BeneficioDoCanal() { }

        public BeneficioDoCanal(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public BeneficioDoCanal(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_benefdocanalid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

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

        [LogicalAttribute("itbc_businessunitid")]
        
        public Lookup UnidadeDeNegocio { get; set; }

        private UnidadeNegocio _UnidadeNegocio = null;
        public UnidadeNegocio UnidadeNegocioObj
        {
            get
            {
                if (_UnidadeNegocio == null && UnidadeDeNegocio.Id != null && UnidadeDeNegocio.Id != Guid.Empty)
                    _UnidadeNegocio = RepositoryService.UnidadeNegocio.Retrieve(UnidadeDeNegocio.Id);
                return _UnidadeNegocio;
            }
        }


        [LogicalAttribute("itbc_categoriaid")]
        public Lookup Categoria { get; set; }

        private Categoria _Categoria = null;
        public Categoria CategoriaObj
        {
            get
            {
                if (_Categoria == null && Categoria.Id != null && Categoria.Id != Guid.Empty)
                    _Categoria = RepositoryService.Categoria.Retrieve(Categoria.Id);
                return _Categoria;
            }
        }

        [LogicalAttribute("itbc_beneficioid")]
        public Lookup Beneficio { get; set; }

        private Domain.Model.Beneficio _Beneficio = null;
        public Domain.Model.Beneficio BeneficioObj
        {
            get
            {
                if (_Beneficio == null && Beneficio.Id != null && Beneficio.Id != Guid.Empty)
                    _Beneficio = RepositoryService.Beneficio.Retrieve(Beneficio.Id);
                return _Beneficio;
            }
        }

        [LogicalAttribute("itbc_statusbeneficiosid")]
        public Lookup StatusBeneficio { get; set; }

        [LogicalAttribute("itbc_verbadisponivel")]
        public Decimal? VerbaDisponivel { get; set; }

        [LogicalAttribute("itbc_verbabrutaperiodoatual")]
        public Decimal? VerbaBrutaPeriodoAtual { get; set; }

        [LogicalAttribute("itbc_verbaperiodoatual")]
        public Decimal? VerbaPeriodoAtual { get; set; }

        [LogicalAttribute("itbc_verbareembolsada")]
        public Decimal? VerbaReembolsada { get; set; }

        [LogicalAttribute("itbc_totalsolicitacoesaprovadas")]
        public Decimal? TotalSolicitacoesAprovadasNaoPagas { get; set; }

        [LogicalAttribute("itbc_calculaverba")]
        public Boolean? CalculaVerba { get; set; }

        [LogicalAttribute("itbc_acumularverba")]
        public Boolean? AcumulaVerba { get; set; }

        [LogicalAttribute("itbc_verbaperiodosanteriores")]
        public decimal? VerbaPeriodosAnteriores { get; set; }

        [LogicalAttribute("itbc_verbacancelada")]
        public decimal? VerbaCancelada { get; set; }

        [LogicalAttribute("itbc_verbaajustada")]
        public decimal? VerbaAjustada { get; set; }

        #endregion
    }
}
