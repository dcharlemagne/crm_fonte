using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_acessoaokonviva")]
    public class AcessoKonviva : DomainBase, ICloneable
    {

        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public AcessoKonviva() { }

        public AcessoKonviva(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public AcessoKonviva(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_acessoaokonvivaid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_contatoid")]
        public Lookup Contato { get; set; }

        [LogicalAttribute("itbc_contaid")]
        public Lookup Conta { get; set; }

        [LogicalAttribute("itbc_unidadekonvivaid")]
        public Lookup UnidadeKonviva { get; set; }

        [LogicalAttribute("itbc_deparadeunidadedokonvivaid")]
        public Lookup DeParaUnidadeKonviva { get; set; }

        [LogicalAttribute("itbc_perfil_aluno")]
        public Boolean? PerfilAluno { get; set; }

        [LogicalAttribute("itbc_perfil_gestor")]
        public Boolean? PerfilGestor { get; set; }

        [LogicalAttribute("itbc_perfil_autor")]
        public Boolean? PerfilAutor { get; set; }

        [LogicalAttribute("itbc_perfil_administrador")]
        public Boolean? PerfilAdministrador { get; set; }
        
        [LogicalAttribute("itbc_perfil_monitor")]
        public Boolean? PerfilMonitor { get; set; }

        [LogicalAttribute("itbc_perfil_moderador")]
        public Boolean? PerfilModerador { get; set; }

        [LogicalAttribute("itbc_perfil_instrutor")]
        public Boolean? PerfilInstrutor { get; set; }

        [LogicalAttribute("itbc_perfil_analista")]
        public Boolean? PerfilAnalista { get; set; }

        [LogicalAttribute("itbc_perfil_tutor")]
        public Boolean? PerfilTutor { get; set; }

        //Para envio de mensgamne retorno apenas
        public Int32? IdUnidadeAcessoKonviva { get; set; }

        [LogicalAttribute("itbc_acaocrm")]
        public Boolean IntegrarNoPlugin { get; set; }

        #endregion
    }
}
