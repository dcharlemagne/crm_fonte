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
    public class CondicaoPagamentoService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public CondicaoPagamentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public CondicaoPagamentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public CondicaoPagamento Persistir(Guid usuarioId, CondicaoPagamento condicaoPagamento)
        {

            CondicaoPagamento condicaoPagamentoAtual = new CondicaoPagamento(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);

            condicaoPagamentoAtual = RepositoryService.CondicaoPagamento.ObterPor((int)condicaoPagamento.Codigo);

            if (condicaoPagamentoAtual != null)
            {
                condicaoPagamento.ID = condicaoPagamentoAtual.ID;

                RepositoryService.CondicaoPagamento.Update(condicaoPagamento, usuarioId);

                if (condicaoPagamento.Status != null && !condicaoPagamentoAtual.Status.Equals(condicaoPagamento.Status))
                    MudarStatus(condicaoPagamento.ID.Value, condicaoPagamento.Status.Value, usuarioId);

                return condicaoPagamento;
            }
            else
            {
                condicaoPagamento.ID = RepositoryService.CondicaoPagamento.Create(condicaoPagamento, usuarioId);
                return condicaoPagamento;
            }
        }

        public CondicaoPagamento BuscaCondicaoPagamento(Guid itbc_condicao_pagamentoid)
        {
            CondicaoPagamento condicaoPagamento = RepositoryService.CondicaoPagamento.ObterPor(itbc_condicao_pagamentoid);
            if (condicaoPagamento != null)
                return condicaoPagamento;
            return null;
        }

        public CondicaoPagamento BuscaCondicaoPagamentoPorCodigo(int itbc_condicao_pagamento)
        {
            CondicaoPagamento condicaoPagamento = RepositoryService.CondicaoPagamento.ObterPor(itbc_condicao_pagamento);
            if (condicaoPagamento != null)
                return condicaoPagamento;
            return null;
        }

        public void MudarStatus(Guid condicaoPagamentoid, int state, Guid usuarioId)
        {
            int statuscode = 0;

            switch (state)
            {
                case (int)Enum.StateCode.Ativo:
                    statuscode = (int)Enum.CondicaoPagamento.Status.Ativo;
                    break;

                case (int)Enum.StateCode.Inativo:
                    statuscode = (int)Enum.CondicaoPagamento.Status.Inativo;
                    break;
            }

            RepositoryService.CondicaoPagamento.AlterarStatus(condicaoPagamentoid, state, statuscode, usuarioId);
        }

        #endregion
    }
}
