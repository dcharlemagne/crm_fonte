using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class RelacionamentoCanalService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public RelacionamentoCanalService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public RelacionamentoCanalService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public RelacionamentoCanalService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        public RelacionamentoCanal Persistir(RelacionamentoCanal objRelCanal)
        {
            //if (relCanal.ID.HasValue)
            //{
            //    RepositoryService.RelacionamentoDoCanal.Update(relCanal);
            //    return relCanal;
            //}
            //else
            //{
            //    relCanal.ID = RepositoryService.RelacionamentoDoCanal.Create(relCanal);
            //    return relCanal;
            //}

            RelacionamentoCanal TmpRelCanal = null;
            if (objRelCanal.ID.HasValue)
                TmpRelCanal = RepositoryService.RelacionamentoDoCanal.ObterPor(objRelCanal.ID.Value);

            if (TmpRelCanal != null)
            {
                //Para poder atualizar state posteriormente
                int? stateUpdate = objRelCanal.Status;
                objRelCanal.Status = TmpRelCanal.Status;

                objRelCanal.ID = TmpRelCanal.ID;
                RepositoryService.RelacionamentoDoCanal.Update(objRelCanal);

                //Retorna o state e razao do update
                objRelCanal.Status = stateUpdate;

                //Se statusCode for diferente do atual altera
                if (!TmpRelCanal.Status.Equals(objRelCanal.Status))
                    this.MudarStatus(objRelCanal.ID.Value, objRelCanal.Status.Value);
                return TmpRelCanal;
            }
            else
                objRelCanal.ID = RepositoryService.RelacionamentoDoCanal.Create(objRelCanal);
            return objRelCanal;
        }

        public bool  MudarStatus(Guid id, int stateCode)
        {
            int status;

            if (stateCode == (int)Domain.Enum.StateCode.Ativo)
                status = (int)Domain.Enum.Status.Ativo;
            else
            {
                stateCode = (int)Domain.Enum.StateCode.Inativo;
                status = (int)Domain.Enum.Status.Inativo;
            }

            return RepositoryService.RelacionamentoDoCanal.AlterarStatus(id, stateCode,status);
        }

        public void VerificarRegistroDuplicado(RelacionamentoCanal objRelacionamentoCanal)
        {
            Guid canal, supervisor, keyAccount, assistente;
            DateTime dtInicial,dtFinal;

            if (objRelacionamentoCanal.Canal != null)
            {
                canal = objRelacionamentoCanal.Canal.Id;
            }
            else
                throw new ArgumentException("Campo Canal Obrigatório");

            if (objRelacionamentoCanal.Supervisor != null)
            {
                supervisor = objRelacionamentoCanal.Supervisor.Id;
            }
            else
                throw new ArgumentException("Campo Supervisor Obrigatório");

            if (objRelacionamentoCanal.KeyAccount != null)
            {
                keyAccount = objRelacionamentoCanal.KeyAccount.Id;
            }
            else
                throw new ArgumentException("Campo KeyAccount Obrigatório");

            if (objRelacionamentoCanal.Assistente != null)
            {
                assistente = objRelacionamentoCanal.Assistente.Id;
            }
            else
                throw new ArgumentException("Campo Assistente Obrigatório");

            if (objRelacionamentoCanal.DataInicial != null)
            {
                dtInicial = objRelacionamentoCanal.DataInicial.Value;
            }
            else
                throw new ArgumentException("Campo Data Inicial Obrigatório");

            if (objRelacionamentoCanal.DataFinal != null)
            {
                dtFinal = objRelacionamentoCanal.DataFinal.Value;
            }
            else
                throw new ArgumentException("Campo Data Final Obrigatório");


            var retornoRelCanal = RepositoryService.RelacionamentoDoCanal.ListarPor(canal, supervisor, keyAccount, assistente, dtInicial, dtFinal);

            if (retornoRelCanal != null && retornoRelCanal.Count > 1)
                throw new ArgumentException("Criação de registro de Relacionamento Do Canal duplicado,operação suspensa.");

        }

        public List<RelacionamentoCanal> ListarAtivosPorCanal(Guid accountId)
        {
            return RepositoryService.RelacionamentoDoCanal.ListarPor(accountId, Domain.Enum.Conta.StateCode.Ativo);
        }

        public List<RelacionamentoCanal> ListarAtivos()
        {
            return RepositoryService.RelacionamentoDoCanal.ListarPor(Guid.Empty, Domain.Enum.Conta.StateCode.Ativo);
        }

        public RelacionamentoCanal ListarPorAssistente(Guid accountId, Guid Assistente)
        {
            return RepositoryService.RelacionamentoDoCanal.ObterPorAssistente(accountId, Assistente);
        }

        public RelacionamentoCanal ListarPorSupervisor(Guid accountId, Guid Supervisor)
        {
            return RepositoryService.RelacionamentoDoCanal.ObterPorSupervisor(accountId, Supervisor);
        }

        public List<RelacionamentoCanal> ListarPorKeyAccount(Guid accountId, Guid KeyAccount)
        {
            return RepositoryService.RelacionamentoDoCanal.ListarPorKeyAccount(accountId, KeyAccount, DateTime.Now);
        }

        public string IntegracaoBarramento(RelacionamentoCanal objConta)
        {
            Domain.Integracao.MSG0137 msgRelacionamentoCanal = new Domain.Integracao.MSG0137(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            return msgRelacionamentoCanal.Enviar(objConta);
        }

    }
}
