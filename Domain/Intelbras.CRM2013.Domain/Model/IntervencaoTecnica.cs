using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("new_intervencao_tecnica")]
    public class IntervencaoTecnica : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public IntervencaoTecnica() { }

        public IntervencaoTecnica(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public IntervencaoTecnica(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }


        #endregion

        #region Atributos
        [LogicalAttribute("new_name")]
        public String Nome { get; set; }

        [LogicalAttribute("new_justificativa_posto")]
        public string ObservacaoPostoDeServico { get; set; }

        private string observacaoIntelbras = ""; //new_observacao_intervencao_tecnica
        public string ObservacaoIntelbras
        {
            get { return observacaoIntelbras; }
            set { observacaoIntelbras = value; }
        }

        [LogicalAttribute("new_data_liberacao")]
        public DateTime? DataLiberacao { get; set; }

        [LogicalAttribute("new_data_aprovacao_automatica")]
        public DateTime? DataAprovacaoAutomatica { get; set; }

        private bool emIntervencao = false; //new_intervencao_tecnica
        public bool EmIntervencao
        {
            get { return emIntervencao; }
            set { emIntervencao = value; }
        }

        [LogicalAttribute("new_ocorrenciaid")]
        public Lookup OcorrenciaId { get; set; }


        #endregion

        #region Métodos

        public void IncluirIntervencao(Ocorrencia ocorrencia, List<IntervencaoTecnica> colecaoIntervencoes, Diagnostico servico, string nome)
        {
            var inclui = true;
            foreach (var inter in colecaoIntervencoes)
                if (inter.Nome.Contains(servico.Produto.Codigo + " Peça solicitada em intervenção técnica"))
                {
                    inclui = false;
                    break;
                }

            if (inclui && (ocorrencia.StatusDaOcorrencia == StatusDaOcorrencia.Aguardando_Analise || ocorrencia.StatusDaOcorrencia == StatusDaOcorrencia.Aguardando_Peça))
            {
                ocorrencia.EmIntervencaoTecnica = true;

                IntervencaoTecnica intervencao = new IntervencaoTecnica(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                intervencao.OcorrenciaId = new Lookup(this.Id, "incident");
                intervencao.Nome = servico.Produto.Codigo + " Peça solicitada em intervenção técnica";
                intervencao.RazaoStatus = 1;

                RepositoryService.Intervencao.Create(intervencao);
            }
        }

        #endregion
    }
}
