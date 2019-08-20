using System;
using Intelbras.CRM2013.Domain.Model;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class TipoPagamentoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public TipoPagamentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public TipoPagamentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public string IntegracaoBarramento(TipoPagamento tipoPagamento)
        {
            Domain.Integracao.MSG0282 msgTipoPagamento = new Domain.Integracao.MSG0282(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            return msgTipoPagamento.Enviar(tipoPagamento);
        }

        public TipoPagamento ObterPor(Guid tipoPagamentoId)
        {
            return RepositoryService.TipoPagamento.ObterPor(tipoPagamentoId);
        }
        
        #endregion
    }
}