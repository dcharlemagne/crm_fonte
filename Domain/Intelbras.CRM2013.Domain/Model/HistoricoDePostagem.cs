using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    /// <summary>
    /// Autor: Marcelo Ferreira de Láias (MSBS Tridea)
    /// Data: 02/09/2011
    /// Descrição: Classe para controlar o histórico de postagem dos Correios.
    /// </summary>
    [LogicalEntity("new_historico_postagem")]
    public class HistoricoDePostagem : DomainBase
    {
        #region Atributos
        
        public DateTime DataDeCriacao { get; set; }
        public string TipoDeSituacaoDaPostagem { get; set; }
        public string CodigoSituacaoDaPostagem { get; set; }
        public string DescricaoSituacaoDaPostagem { get; set; }
        public DateTime DataHoraDaSituacaoDaPostagem { get; set; }
        public string LocalDoEvento { get; set; }
        public string CodigoDePostagem { get; set; }
        public string NumeroDeObjeto { get; set; }
        public string TipoDeETiket { get; set; }
        [LogicalAttribute("new_name")]
        public String Nome { get; set; }
        Ocorrencia ocorrencia = null;
        public Ocorrencia Ocorrencia
        {
            get
            {
                if (ocorrencia == null && this.Id != Guid.Empty)
                    ocorrencia = RepositoryService.Ocorrencia.ObterPor(this);
                return ocorrencia;
            }
            set { ocorrencia = value; }
        }

        #endregion

        #region Contrutores
        private RepositoryService RepositoryService { get; set; }

        public HistoricoDePostagem() { }

        public HistoricoDePostagem(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public HistoricoDePostagem(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Metodos

        public Guid Salvar()
        {
            return RepositoryService.HistoricoDePostagem.Create(this);
        }

        public Guid Salvar(HistoricoDePostagem historicoDePostagem)
        {
            return RepositoryService.HistoricoDePostagem.Create(historicoDePostagem);
        }

        public void Atualizar()
        {
            RepositoryService.HistoricoDePostagem.Update(this);
        }

        public void Atualizar(HistoricoDePostagem historicoDePostagem)
        {
            RepositoryService.HistoricoDePostagem.Update(historicoDePostagem);
        }

        public List<HistoricoDePostagem> PesquisarHistoricoDePostagemPor(string eTiket)
        {
            return RepositoryService.HistoricoDePostagem.PesquisarHistoricoDePostagemPor(eTiket);
        }

        public List<HistoricoDePostagem> PesquisarHistoricoDePostagemPor(Ocorrencia ocorrencia)
        {
            return RepositoryService.HistoricoDePostagem.PesquisarHistoricoDePostagemPor(ocorrencia);
        }


        public List<HistoricoDePostagem> PesquisarHistoricoDePostagemPor(Model.Conta cliente)
        {
            return RepositoryService.HistoricoDePostagem.PesquisarHistoricoDePostagemPor(cliente);
        }

        public void CriarHistoricoDePostagem(HistoricoDePostagem historicoDePostagem)
        {
            RepositoryService.HistoricoDePostagem.Create(historicoDePostagem);
        }

        public bool ExcluirHistoricoDePostagem(Guid historicoDePostagemId)
        {
            RepositoryService.HistoricoDePostagem.Delete(historicoDePostagemId);
            return true;
        }

        #endregion

    }
}
