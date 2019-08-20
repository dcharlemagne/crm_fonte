using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Enum;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    /// <summary>
    /// Autor: Marcelo Ferreira de Láias (MSBS Tridea)
    /// Data: 06/09/2011
    /// Descrição: Classe para controlar o número de postagem dos Correios.
    /// </summary>
    [LogicalEntity("new_autorizcao_postagem_eletronica_correios")]
    public class AutorizacaoPostagemCorreios : DomainBase
    {
        
        #region Atributos

        public DateTime DataDeCriacao { get; set; }
        public string NumeroDoContrato { get; set; }
        public string NumeroDoCartao { get; set; }
        public SituacaoContratoAutorizacaoPostagemCorreios SituacaoContrato { get; set; }

        public string CepDaUnidadeDePostagem { get; set; }
        public string EmailAlertaPostagem { get; set; }
        public string PrefixoTipoDeServico { get; set; }
        public string TipoDeServico { get; set; }
        public string DescricaoDaUnidadePostal { get; set; }

        public int CodigoDaUnidadePostal { get; set; }
        public int CodigoAdministrativo { get; set; }
        public int SequenciaInicial { get; set; }
        public int SequenciaFinal { get; set; }
        public int ProximoNumeroSequencia { get; set; }
        public int NumeroSequenciaAlerta { get; set; }

        //Ocorrencias.Ocorrencia ocorrencia;
        //public Ocorrencias.Ocorrencia Ocorrencia
        //{
        //    get { return ocorrencia; }
        //    set { ocorrencia = value; }
        //}

        #endregion

        #region Contrutores
        private RepositoryService RepositoryService { get; set; }

        public AutorizacaoPostagemCorreios() { }

        public AutorizacaoPostagemCorreios(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public AutorizacaoPostagemCorreios(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Metodos

        public Guid Salvar()
        {
            return RepositoryService.AutorizacaoDePostagem.Create(this);
        }

        public Guid Salvar(AutorizacaoPostagemCorreios AutorizacaoPostagemCorreios)
        {
            return RepositoryService.AutorizacaoDePostagem.Create(AutorizacaoPostagemCorreios);
        }

        public void Atualizar()
        {
            RepositoryService.AutorizacaoDePostagem.Update(this);
        }

        public void Atualizar(AutorizacaoPostagemCorreios AutorizacaoPostagemCorreios)
        {
            RepositoryService.AutorizacaoDePostagem.Update(AutorizacaoPostagemCorreios);
        }

        public List<AutorizacaoPostagemCorreios> PesquisarAutorizacaoPostagemCorreiosPor(string eTiket)
        {
            return RepositoryService.AutorizacaoDePostagem.PesquisarAutorizacaoPostagemCorreiosPor(eTiket);
        }

        public void CriarAutorizacaoPostagemCorreios(AutorizacaoPostagemCorreios AutorizacaoPostagemCorreios)
        {
            //TodasAsAutorizacoesDePostagensCorreios.CriarAutorizacaoPostagemCorreios(AutorizacaoPostagemCorreios);
            throw new NotImplementedException();
        }

        public bool ExcluirAutorizacaoPostagemCorreios(Guid AutorizacaoPostagemCorreiosId)
        {
           RepositoryService.AutorizacaoDePostagem.Delete(AutorizacaoPostagemCorreiosId);
            return true;
        }

        public AutorizacaoPostagemCorreios ObterAutorizacaoPostagemCorreiosPor(Guid autorizacaoPostagemCorreiosId)
        {
            return ObterAutorizacaoPostagemCorreiosPor(autorizacaoPostagemCorreiosId);
        }

        public string ObterCodigoAutorizacaoDePostagem()
        {
            // Pegar sequencia atual
            int proximaSeq = this.ProximoNumeroSequencia;

            // Atualizar sequencia
            this.ProximoNumeroSequencia += 1;

            this.Atualizar();

            return proximaSeq.ToString("000000000");
            //return this.PrefixoTipoDeServico.ToUpper() + proximaSeq.ToString("000000000") + "BR";
        }

        #endregion
    }
}
