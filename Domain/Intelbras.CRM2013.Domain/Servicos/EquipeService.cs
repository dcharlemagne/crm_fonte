using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class EquipeService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public EquipeService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public EquipeService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public List<TeamMembership> listarMembrosEquipe(Guid equipe)
        {
            List<TeamMembership> Team = RepositoryService.TeamMembership.ListarPor(equipe);
            return Team;
        }

        public List<Equipe> ListarEquipe(Guid equipeId)
        {
            return RepositoryService.Equipe.ListarPor(equipeId);
        }

        public List<Equipe> ListarEquipePorNome(string nomeEquipe)
        {
            return RepositoryService.Equipe.ListarPorNome(nomeEquipe);
        }

        public void RemoverPerfilAcesso(Guid equipeId,Guid perfilId)
        {
            RepositoryService.Equipe.RemoverPerfilAcesso(equipeId,perfilId);
        }

        public void AdicionarPerfilAcesso(Guid equipeId,Guid perfilId)
        {
            RepositoryService.Equipe.AdicionarPerfilAcesso(equipeId,perfilId);
        }

        #endregion



    }
}


