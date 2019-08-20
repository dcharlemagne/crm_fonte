using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_familiadapoliticacomercial")]
    public class FamiliaPoliticaComercial : DomainBase
    {
        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public FamiliaPoliticaComercial() { }

        public FamiliaPoliticaComercial(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public FamiliaPoliticaComercial(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }


        #endregion

        #region Atributos
        [LogicalAttribute("itbc_familiadapoliticacomercialid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_politicacomercialid")]
        public Lookup PoliticaComercial { get; set; }

        [LogicalAttribute("itbc_familiainicialid")]
        public Lookup FamiliaInicialId { get; set; }


        [LogicalAttribute("itbc_familiafinalid")]
        public Lookup FamiliaFinalId { get; set; }


        private FamiliaComercial _FamiliaComercialInicial = null;
        public FamiliaComercial FamiliaComercialInicial
        {
            get
            {
                if (_FamiliaComercialInicial == null)
                    _FamiliaComercialInicial = RepositoryService.FamiliaComercial.Retrieve(FamiliaInicialId.Id);

                return _FamiliaComercialInicial;
            }
        }

        private FamiliaComercial _FamiliaComercialFinal = null;
        public FamiliaComercial FamiliaComercialFinal
        {
            get
            {
                if (_FamiliaComercialFinal == null)
                    _FamiliaComercialFinal = RepositoryService.FamiliaComercial.Retrieve(FamiliaFinalId.Id);

                return _FamiliaComercialFinal;
            }
        }




        [LogicalAttribute("itbc_datainicial")]
        public DateTime DataInicial { get; set; }

        [LogicalAttribute("itbc_datafinal")]
        public DateTime DataFinal { get; set; }

        [LogicalAttribute("itbc_qtdeinicial")]
        public int QtdInicial { get; set; }

        [LogicalAttribute("itbc_qtdefinal")]
        public int QtdFinal { get; set; }

        [LogicalAttribute("itbc_fator")]
        public Decimal Fator { get; set; }

        [LogicalAttribute("statuscode")]
        public int? Status { get; set; }

        #endregion
    }
}
