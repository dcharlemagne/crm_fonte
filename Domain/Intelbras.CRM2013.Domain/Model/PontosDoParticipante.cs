using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    //[LogicalEntity("")] não tem entidade no CRM
    public class PontosDoParticipante: DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PontosDoParticipante(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public PontosDoParticipante(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        private Guid distribuidor;
        private Guid contatoDistribuidor;
        private Guid participante;
        private Guid produto;
        private string keyCode;
        private string numeroSerie;
        private int pontos;
        private DateTime dataExpiracao;


        public Guid Distribuidor
        {
            get { return distribuidor; }
            set { distribuidor = value; }
        }

        public Guid ContatoDistribuidor
        {
            get { return contatoDistribuidor; }
            set { contatoDistribuidor = value; }
        }

        public Guid Participante
        {
            get { return participante; }
            set { participante = value; }
        }

        public Guid Produto
        {
            get { return produto; }
            set { produto = value; }
        }

        public string KeyCode
        {
            get { return keyCode; }
            set { keyCode = value; }
        }

        public string NumeroSerie
        {
            get { return numeroSerie; }
            set { numeroSerie = value; }
        }

        public int Pontos
        {
            get { return pontos; }
            set { pontos = value; }
        }

        public DateTime DataExpiracao
        {
            get { return dataExpiracao; }
            set { dataExpiracao = value; }
        }

        #endregion
    }
}
