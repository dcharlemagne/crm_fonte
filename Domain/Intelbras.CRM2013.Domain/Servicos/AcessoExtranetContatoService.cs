using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class AcessoExtranetContatoService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public AcessoExtranetContatoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public AcessoExtranetContatoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public AcessoExtranetContatoService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Property/Objetos

        #endregion

        #region Métodos
        public void PreCreate(Model.AcessoExtranetContato mAcessoExtranetContato)
        {
            List<AcessoExtranetContato> lstAcessExtranetContato = RepositoryService.AcessoExtranetContato.ListarPor((mAcessoExtranetContato.Canal == null ? Guid.Empty : mAcessoExtranetContato.Canal.Id), mAcessoExtranetContato.Contato.Id);
            if (lstAcessExtranetContato.Count > 0)
                throw new ArgumentException("(CRM) Já existe registro para este contato.");
        }

        public AcessoExtranetContato Persistir(AcessoExtranetContato ObjExtranet)
        {
            AcessoExtranetContato TmpAcessoExtranet = null;

            if (ObjExtranet.ID.HasValue)
            {
                TmpAcessoExtranet = RepositoryService.AcessoExtranetContato.ObterPor(ObjExtranet.ID.Value);

                if (TmpAcessoExtranet != null)
                {
                    ObjExtranet.ID = TmpAcessoExtranet.ID;

                    RepositoryService.AcessoExtranetContato.Update(ObjExtranet);

                    if (!TmpAcessoExtranet.Status.Equals(ObjExtranet.Status) && ObjExtranet.Status != null)
                        this.MudarStatus(TmpAcessoExtranet.ID.Value, ObjExtranet.Status.Value);

                    return TmpAcessoExtranet;
                }
                else
                    return null;
            }
            else
            {
                ObjExtranet.ID = RepositoryService.AcessoExtranetContato.Create(ObjExtranet);
                return ObjExtranet;
            }
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.AcessoExtranetContato.AlterarStatus(id, status);
        }

        public TipoDeAcessoExtranet buscaTipoAcesso(Guid ObjTipoAcesso)
        {
            TipoDeAcessoExtranet TmpTipoAcesso = null;

            TmpTipoAcesso = RepositoryService.TipoAcessoExtranet.ObterPor(ObjTipoAcesso);

            if (TmpTipoAcesso != null)
            {
                return TmpTipoAcesso;
            }

            return null;
        }

        public List<AcessoExtranetContato> ListarTodos()
        {
            return RepositoryService.AcessoExtranetContato.ListarTodos();
        }

        /// <summary>
        /// Duplicidade acesso extranet contato
        /// Retorna false se registro nao existe e true se existe
        /// </summary>
        /// <param name="contatoId"></param>
        /// <returns>Retorna false se registro nao existe e true se existe</returns>
        public Boolean ValidarExistenciaAcessoExtranet(Guid contatoId)
        {
            if (RepositoryService.AcessoExtranetContato.ListarPorContato(contatoId).Count > 0)
                return true;

            return false;
        }
        public void InativarAcessoExtranetContato(Conta canal)
        {
            var lstAcessosExtranetContatosDoCanal = this.RepositoryService.AcessoExtranetContato.ListarPorCanal(canal.ID.Value);
            foreach (var acessoExtranetContatos in lstAcessosExtranetContatosDoCanal)
            {
                if (acessoExtranetContatos.Status == (int)Enum.AcessoExtranetContatos.StateCode.Ativo)
                {
                    this.RepositoryService.AcessoExtranetContato.AlterarStatus(acessoExtranetContatos.ID.Value, (int)Enum.AcessoExtranetContatos.StateCode.Inativo);
                }       
            }
        }

        public void CriarAcessoExtranetContato(Contato contato)
        {
            var acessoExtranetContato = new AcessoExtranetContato(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            if (contato.TipoRelacao.HasValue)
            {
                if (contato.TipoRelacao.Value == (int)Enum.Contato.TipoRelacao.KeyAccount)
                {
                    acessoExtranetContato.TipoAcesso = new Lookup();
                    acessoExtranetContato.AcessoExtranetid = new Lookup();
                }
                else if (contato.TipoRelacao.Value == (int)Enum.Contato.TipoRelacao.ColaboradorIntelbras)
                {
                    acessoExtranetContato.TipoAcesso = new Lookup();
                    acessoExtranetContato.AcessoExtranetid = new Lookup();
                }
                else //"Colaborador do Canal", "Cliente Final" ou "Outros"
                {
                    acessoExtranetContato.TipoAcesso = new Lookup();
                    acessoExtranetContato.AcessoExtranetid = new Lookup();
                }

                acessoExtranetContato.Contato = new Lookup(contato.ID.Value, "");
                acessoExtranetContato.Validade = new DateTime(2059, 12, 31);
                RepositoryService.AcessoExtranetContato.Create(acessoExtranetContato);
            }
            
        }

        /// <summary>
        /// Altera o canal associado ao AcessoExtranetContato
        /// </summary>
        /// <param name="contatoId"></param>
        /// <param name="novoCanalId"></param>
        public void MudarCanal(Guid contatoId, Guid? novoCanalId, bool integracao)
        {
            foreach (var acesso in RepositoryService.AcessoExtranetContato.ListarPorContato(contatoId))
            {
                if (novoCanalId.HasValue)
                {
                    if (acesso.Canal == null || acesso.Canal.Id != novoCanalId.Value)
                    {
                        acesso.Canal = new Lookup(novoCanalId.Value, "account");
                    }
                }
                else
                {
                    acesso.AddNullProperty("Canal");
                }

                acesso.IntegrarNoPlugin = integracao;
                this.Persistir(acesso);
            }
        }
       
        #endregion

        #region Integracao

        public string IntegracaoBarramento(AcessoExtranetContato objAcessoExtranet)
        {
            Domain.Integracao.MSG0119 msgAcessoExtranet = new Domain.Integracao.MSG0119(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            return msgAcessoExtranet.Enviar(objAcessoExtranet);
        }
        #endregion

    }
}


