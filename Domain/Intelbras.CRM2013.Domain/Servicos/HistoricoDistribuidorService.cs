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
    public class HistoricoDistribuidorService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public HistoricoDistribuidorService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public HistoricoDistribuidorService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public Guid Persistir(HistoricoDistribuidor item)
        {
            HistoricoDistribuidor historicoDistribuidor = new HistoricoDistribuidor(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
            
            if (item.ID.HasValue)
            {
                RepositoryService.HistoricoDistribuidor.Update(item);

                historicoDistribuidor = RepositoryService.HistoricoDistribuidor.Retrieve(item.ID.Value);

                if (item.Status != null && !item.Status.Equals(historicoDistribuidor.Status) && item.ID != null)
                    MudarStatus(item.ID.Value, item.Status.Value);
            }
            else
            {
                item.ID = RepositoryService.HistoricoDistribuidor.Create(item);
            }

            return item.ID.Value;
        }

        public HistoricoDistribuidor ObterPor(Guid id)
        {
            return RepositoryService.HistoricoDistribuidor.Retrieve(id);
        }

        public bool ExisteDuplicidade(HistoricoDistribuidor historicoDistribuidor, Boolean create, ParametroGlobal qtdDistribuidorPreferencial)
        {
            if (historicoDistribuidor.Revenda != null)
            {
                if (create || (historicoDistribuidor.Status.HasValue && historicoDistribuidor.Status.Value == (int)Enum.HistoricoDistribuidor.Statecode.Ativo))
                {
                    var lista = new List<HistoricoDistribuidor>();

                    if (historicoDistribuidor.DataFim == null)
                    {
                        lista = RepositoryService.HistoricoDistribuidor.ListarPorRevendaSemDataFim(historicoDistribuidor.Revenda.Id, historicoDistribuidor.DataInicio);
                    }
                    else
                    {
                        lista = RepositoryService.HistoricoDistribuidor.ListarPorRevendaComDataFim(historicoDistribuidor.Revenda.Id, historicoDistribuidor.DataInicio, historicoDistribuidor.DataFim);
                    }
                    if (historicoDistribuidor.ID.HasValue)
                    {
                        lista.RemoveAll(x => x.ID.Value == historicoDistribuidor.ID.Value);
                    }
                    if (lista.Count >= Convert.ToInt32(qtdDistribuidorPreferencial.Valor)) //valida quantidade cadastrada no parâmetro global
                        return lista.Count > 0;
                }
            }

            return false;
        }

        public void ValidaDuplicidade(HistoricoDistribuidor historicoDistribuidor, Boolean create)
        {
            var quantidadeDistribuidorPreferencial = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.TipoParametroGlobal.QuantidadeDistribuidorPref);

            if (ValidaDataRetroativa(historicoDistribuidor))
            {
                if (ExisteDuplicidade(historicoDistribuidor, create, quantidadeDistribuidorPreferencial))
                {
                    throw new ArgumentException("(CRM) Já existem outros " + quantidadeDistribuidorPreferencial.Valor + " registros ativos na mesma data de vigência.");
                }
            }
        }

        public bool ValidaDataRetroativa(HistoricoDistribuidor historicoDistribuidor)
        {
            if (historicoDistribuidor.DataFim.HasValue && historicoDistribuidor.DataFim < historicoDistribuidor.DataInicio)
            {
                throw new ArgumentException("(CRM) O campo 'Data Fim' não pode ser menor que a 'Data Início'!");
            }
            return true;
        }

        public void ValidaCamposObrigatorios(HistoricoDistribuidor historicoDistribuidor)
        {
            if (historicoDistribuidor.Revenda == null)
            {
                throw new ArgumentException("(CRM) O campo 'Revenda' é obrigatório!");
            }

            if (historicoDistribuidor.Distribuidor == null)
            {
                throw new ArgumentException("(CRM) O campo 'Distribuidor' é obrigatório!");
            }

            if (!historicoDistribuidor.DataInicio.HasValue)
            {
                throw new ArgumentException("(CRM) O campo 'Data Início' é obrigatório!");
            }
        }

        public void Integrar(HistoricoDistribuidor historicoDistribuidor)
        {
            var mensagem = new Intelbras.CRM2013.Domain.Integracao.MSG0179(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            mensagem.Enviar(historicoDistribuidor);
        }

        public void Atualizar(HistoricoDistribuidor historicoDistribuidor)
        {
            RepositoryService.HistoricoDistribuidor.Update(historicoDistribuidor);
        }

        public List<HistoricoDistribuidor> ListarPorPeriodo(DateTime data)
        {
            return RepositoryService.HistoricoDistribuidor.ListarPorPeriodo(data);
        }

        public Boolean AlterarStatus(Guid historicoid, int status)
        {
            return RepositoryService.HistoricoDistribuidor.AlterarStatus(historicoid, status);
        }

        public void MudarStatus(Guid condicaoPagamentoid, int state)
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

            RepositoryService.HistoricoDistribuidor.AlterarStatus(condicaoPagamentoid, state);
        }

        #endregion
    }
}
