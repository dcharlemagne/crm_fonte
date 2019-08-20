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
    public class SolicitacaoCadastroService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SolicitacaoCadastroService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }


        public SolicitacaoCadastroService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        public List<SolicitacaoCadastro> ListarSolicitacaoCadastro(SolicitacaoCadastro solCadastro, DateTime? dtInicio, DateTime? dtFim)
        {
            return RepositoryService.SolicitacaoCadastro.ListarPorConta(solCadastro.Representante.Id, dtInicio, dtFim, solCadastro.Status);
        }

        public SolicitacaoCadastro Persistir(SolicitacaoCadastro SolCadastro)
        {
            if (SolCadastro.ID.HasValue)
            {
                SolicitacaoCadastro tmpSolCad = RepositoryService.SolicitacaoCadastro.ObterPor(SolCadastro.ID.Value);

                if (tmpSolCad != null)
                {
                    //Para não tentar alterar status junto com o update
                    int statusAnterior = SolCadastro.Status.Value;
                    SolCadastro.Status = tmpSolCad.Status;
                    RepositoryService.SolicitacaoCadastro.Update(SolCadastro);
                    SolCadastro.Status = statusAnterior;
                    if (SolCadastro.State.HasValue 
                        && SolCadastro.Status.HasValue
                        && !tmpSolCad.State.Equals(SolCadastro.State.Value)
                        && (SolCadastro.Status.Value == (int)Intelbras.CRM2013.Domain.Enum.SolicitacaoCadastro.StatusSolicitacao.Criada
                             || SolCadastro.Status.Value == (int)Intelbras.CRM2013.Domain.Enum.SolicitacaoCadastro.StatusSolicitacao.EmAnalise
                             || SolCadastro.Status.Value == (int)Intelbras.CRM2013.Domain.Enum.SolicitacaoCadastro.StatusSolicitacao.Inativo))
                        MudarStatus(SolCadastro.ID.Value, SolCadastro.State.Value);
                    return tmpSolCad;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                SolCadastro.ID = RepositoryService.SolicitacaoCadastro.Create(SolCadastro);
                return SolCadastro;
            }
        }


        public SolicitacaoCadastro ObterPor(Guid solicitacaoCadastroID)
        {
            return RepositoryService.SolicitacaoCadastro.ObterPor(solicitacaoCadastroID);
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.SolicitacaoCadastro.AlterarStatus(id, status);
        }
    }
}
