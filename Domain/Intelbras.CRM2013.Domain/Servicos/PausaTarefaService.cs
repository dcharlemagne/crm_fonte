using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class PausaTarefaService
    {
        #region Objetos
        private string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        private Boolean isOffline = false;

        string usuarioSharePoint = SDKore.Helper.Cryptography.Decrypt(SDKore.Configuration.ConfigurationManager.GetSettingValue("UsuarioSharePoint"));
        string senhaSharePoint = SDKore.Helper.Cryptography.Decrypt(SDKore.Configuration.ConfigurationManager.GetSettingValue("SenhaSharePoint"));

        #endregion

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PausaTarefaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public PausaTarefaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public PausaTarefaService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Métodos

        public Tarefa BuscaTarefa(Guid TarefaID)
        {
            List<Tarefa> lstTarefa = RepositoryService.Tarefa.ListarPor(TarefaID);
            if (lstTarefa.Count > 0)
                return lstTarefa.First();
            return null;
        }

        public PausaTarefa BuscaPausaTarefa(Guid PausaID)
        {
            List<PausaTarefa> lstPausaTarefa = RepositoryService.PausaTarefa.ListarPor(PausaID);
            if (lstPausaTarefa.Count > 0)
                return lstPausaTarefa.First();
            return null;
        }

        public bool PersistirMotivoPausaNaTarefa(PausaTarefa pausa)
        {
            bool persistiu = false;
            var _mTarefa = BuscaTarefa(pausa.Tarefa.Id);

            if (_mTarefa != null)
            {
                //atribui para a Tarefa o mesmo motivo
                _mTarefa.MotivoPausa = pausa.Motivo.Name;
                string retorno;

                TarefaService tarefaService = new TarefaService(OrganizationName, isOffline);
                persistiu = tarefaService.Persistir(_mTarefa, out retorno) != null;
            }

            return persistiu;
        }

        public bool Persistir(PausaTarefa pausa)
        {
            try
            {
                PausaTarefaService pausaTarefaService = new PausaTarefaService(OrganizationName, isOffline);
                RepositoryService.PausaTarefa.Update(pausa);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<PausaTarefa> ListarPausaTarefa(Guid TarefaID)
        {
            return RepositoryService.PausaTarefa.ListarTarefaPor(TarefaID);
        }

        #endregion
    }
}