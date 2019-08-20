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
    public class ClassificacaoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ClassificacaoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public ClassificacaoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public Classificacao Persistir(Classificacao objClassificacao)
        {
            Classificacao tmpClassificacao = null;
            if (objClassificacao.ID.HasValue)
            {
                tmpClassificacao = RepositoryService.Classificacao.ObterPor(objClassificacao.ID.Value);

                if (tmpClassificacao != null)
                {
                    objClassificacao.ID = tmpClassificacao.ID;
                    RepositoryService.Classificacao.Update(objClassificacao);
                    //Altera Status - Se necessário
                    if (!tmpClassificacao.Status.Equals(objClassificacao.Status) && objClassificacao.Status != null)
                        this.MudarStatus(tmpClassificacao.ID.Value, objClassificacao.Status.Value);
                    return tmpClassificacao;
                }
                else
                    return null;
            }
            else
            {
                objClassificacao.ID = RepositoryService.Classificacao.Create(objClassificacao);
                return objClassificacao;
            }
        }

        public Classificacao BuscaClassificacao(Guid itbc_Classificacaoid)
        {
            Classificacao classificacao = RepositoryService.Classificacao.ObterPor(itbc_Classificacaoid);
            if (classificacao != null)
                return classificacao;
            return null;
        }

        
        public Classificacao ObterClassificacaoPorNome(String nome)
        {
            return RepositoryService.Classificacao.ObterPor(nome);    
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.Classificacao.AlterarStatus(id, status);
        }

        public string IntegracaoBarramento(Classificacao classificacao)
        {
            Domain.Integracao.MSG0014 msgClassificacao = new Domain.Integracao.MSG0014(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            
            return msgClassificacao.Enviar(classificacao);
        }

        #endregion
    }
}
