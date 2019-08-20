using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class FormaPagamentoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public FormaPagamentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public FormaPagamentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos Forma Pagamento

        /// <summary>
        /// Verifica se o status e state code existem
        /// </summary>
        /// <param name="status">Status</param>
        /// <param name="stateCode">Razão Status</param>
        /// <returns>Bool</returns>
        public FormaPagamento ObterPorNome(string nomeFormaPagamento)
        {
            return RepositoryService.FormaPagamento.ObterPor(nomeFormaPagamento);
        }

        public FormaPagamento ObterPorGuid(Guid formaPagamentoId)
        {
            return RepositoryService.FormaPagamento.Retrieve(formaPagamentoId);
        }

        public List<FormaPagamento> Listar()
        {
            return RepositoryService.FormaPagamento.Listar();
        }
        #endregion

    }
}
