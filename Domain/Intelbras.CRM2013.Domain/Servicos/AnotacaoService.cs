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
    public class AnnotationService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public AnnotationService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public AnnotationService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        /*
        public Guid Persistir(Anexo item)
        {
            if (item.ID.HasValue)
            {
                RepositoryService.HistoricoDistribuidor.Update(item);
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

        public bool ExisteDuplicidade(HistoricoDistribuidor historicoDistribuidor, Boolean create)
        {
            if (historicoDistribuidor.Revenda != null)
            {
                if (create || (historicoDistribuidor.Status.HasValue && historicoDistribuidor.Status.Value == (int)Enum.HistoricoDistribuidor.Statecode.Ativo))
                {
                    var lista = RepositoryService.HistoricoDistribuidor.ListarPorRevenda(historicoDistribuidor.Revenda.Id, historicoDistribuidor.DataInicio, historicoDistribuidor.DataFim);

                    if (historicoDistribuidor.ID.HasValue)
                    {
                        lista.RemoveAll(x => x.ID.Value == historicoDistribuidor.ID.Value);
                    }

                    return lista.Count > 0;
                }
            }

            return false;
        }

        public void ValidaDuplicidade(HistoricoDistribuidor historicoDistribuidor, Boolean create)
        {
            if (ExisteDuplicidade(historicoDistribuidor, create))
            {
                throw new ArgumentException("(CRM) Já existe outro registro ativo com o mesmo dados, verifique a data de vigência.");
            }
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

            if (!historicoDistribuidor.DataFim.HasValue)
            {
                throw new ArgumentException("(CRM) O campo 'Data Fim' é obrigatório!");
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

        #endregion

        */
    }

}
