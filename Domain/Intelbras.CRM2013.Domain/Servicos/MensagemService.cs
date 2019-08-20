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
    public class MensagemService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public MensagemService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public MensagemService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public Mensagem Persistir(Mensagem objMensagem)
        {
            Mensagem tmpMensagem = null;
            if (objMensagem.Codigo.HasValue)
            {
                tmpMensagem = RepositoryService.Mensagem.ObterPor(objMensagem.Codigo.Value);

                if (tmpMensagem != null)
                {
                    objMensagem.ID = tmpMensagem.ID;

                    RepositoryService.Mensagem.Update(objMensagem);

                    //Altera Status - Se necessário
                    if (!tmpMensagem.State.Equals(objMensagem.State) && objMensagem.State != null)
                        this.MudarStatus(tmpMensagem.ID.Value, objMensagem.State.Value);

                    return tmpMensagem;
                }
                else
                {
                    objMensagem.ID = RepositoryService.Mensagem.Create(objMensagem);
                    return objMensagem;
                }
            }
            else
            {
                objMensagem.ID = RepositoryService.Mensagem.Create(objMensagem);
                return objMensagem;
            }
        }

        public Mensagem BuscaMensagem(Guid itbc_mensagemid)
        {
            Mensagem mensagem = RepositoryService.Mensagem.ObterPor(itbc_mensagemid);
            if (mensagem != null)
                return mensagem;
            return null;
        }

        public Mensagem BuscaMensagemPorCodigo(int itbc_codigo_mensagem)
        {
            Mensagem mensagem = RepositoryService.Mensagem.ObterPor(itbc_codigo_mensagem);
            if (mensagem != null)
                return mensagem;
            return null;
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.Mensagem.AlterarStatus(id, status);
        }

        #endregion
    }
}
