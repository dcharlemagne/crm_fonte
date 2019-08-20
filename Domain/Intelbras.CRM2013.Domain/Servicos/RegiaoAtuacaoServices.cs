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


    public class RegiaoAtuacaoServices
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public RegiaoAtuacaoServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }


        public RegiaoAtuacaoServices(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        #region Metodos
        public List<RegiaoAtuacao> ListarRegiaoAtuacao(Guid itbc_RegiaoAtuacaoid)
        {
            List<RegiaoAtuacao> lstRegiaoAtuacao = RepositoryService.RegiaoAtuacao.ListarPor(itbc_RegiaoAtuacaoid);
            if (lstRegiaoAtuacao.Count > 0)
                return lstRegiaoAtuacao;
            return null;
        }

        public bool Acao(RegiaoAtuacao objRegiaoAtuacao, string acao)
        {
            RegiaoAtuacao tmpRegiaoAtuacao = null;
            
            if (acao.ToUpper().Contains("D"))
            {

                 

                tmpRegiaoAtuacao = RepositoryService.RegiaoAtuacao.ObterPor(objRegiaoAtuacao.MunicipioId.Value, objRegiaoAtuacao.Canal.Value);

                if (tmpRegiaoAtuacao != null)
                {
                    //RepositoryService.RegiaoAtuacao.Delete(tmpRegiaoAtuacao.ID.Value);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (acao.ToUpper().Contains("I"))
            {
                RepositoryService.RegiaoAtuacao.Create(objRegiaoAtuacao);
                return true;
            }

            return false;
        }
        #endregion
    }
}
