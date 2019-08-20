using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_treinamcertif")]
    public class TreinamentoCertificacao : DomainBase
    {

        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public TreinamentoCertificacao(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public TreinamentoCertificacao(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_treinamcertifid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_categoria_curso")]
        public String CategoriaCurso { get; set; }

        [LogicalAttribute("itbc_codigodotreinamento")]
        public String CodigoTreinamento { get; set; }

        [LogicalAttribute("itbc_idinterno")]
        public Int32? IdCurso { get; set; }

        [LogicalAttribute("itbc_modalidade_curso")]
        public String ModalidadeCurso { get; set; }

        [LogicalAttribute("statuscode")]
        public int? Status { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }

        [LogicalAttribute("itbc_horas_treinamento")]
        public Int32? HorasTreinamento { get; set; }
        #endregion
    }
}
