using System;
using Intelbras.CRM2013.Domain.Model;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class PagamentoServicoService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PagamentoServicoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public PagamentoServicoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public PagamentoServico Persistir(PagamentoServico pagamentoServico)
        {
            PagamentoServico tmpPagamentoServico = null;

            if (pagamentoServico.ID.HasValue)
            {
                tmpPagamentoServico = RepositoryService.PagamentoServico.Retrieve(pagamentoServico.ID.Value);
                if (tmpPagamentoServico == null)
                {
                    return tmpPagamentoServico; // retorna NULL
                }

                // Atualiza o pagamento serviço pelo GUID
                pagamentoServico.ID = tmpPagamentoServico.ID;
                RepositoryService.PagamentoServico.Update(pagamentoServico);

                return pagamentoServico;
            }

            //cria pagamento servico
            pagamentoServico.ID = RepositoryService.PagamentoServico.Create(pagamentoServico);
            return pagamentoServico;
        }

        public PagamentoServico BuscaPagamentoServico(Guid pagamentoServico)
        {
            return RepositoryService.PagamentoServico.Retrieve(pagamentoServico);
        }

        public string IntegracaoBarramento(PagamentoServico pagamentoServico)
        {
            Domain.Integracao.MSG0283 msgModelo = new Domain.Integracao.MSG0283(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            return msgModelo.Enviar(pagamentoServico);
        }
        #endregion
    }
}
