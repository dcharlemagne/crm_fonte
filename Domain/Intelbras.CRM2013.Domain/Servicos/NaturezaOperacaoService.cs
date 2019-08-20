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
    public class NaturezaOperacaoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public NaturezaOperacaoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public NaturezaOperacaoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public NaturezaOperacao Persistir(NaturezaOperacao objNaturezaOperacao)
        {
            NaturezaOperacao tmpNaturezaOperacao = null;
            if (!String.IsNullOrEmpty(objNaturezaOperacao.Codigo))
            {
                tmpNaturezaOperacao = RepositoryService.NaturezaOperacao.ObterPor(objNaturezaOperacao.Codigo);

                if (tmpNaturezaOperacao != null)
                {
                    objNaturezaOperacao.ID = tmpNaturezaOperacao.ID;
                    
                    RepositoryService.NaturezaOperacao.Update(objNaturezaOperacao);

                    if (!tmpNaturezaOperacao.State.Equals(objNaturezaOperacao.State) && objNaturezaOperacao.State != null)
                        this.MudarStatus(tmpNaturezaOperacao.ID.Value, objNaturezaOperacao.State.Value);
                    
                    return tmpNaturezaOperacao;
                }
                else
                {
                    objNaturezaOperacao.ID = RepositoryService.NaturezaOperacao.Create(objNaturezaOperacao);
                    return objNaturezaOperacao;
                }
            }
            else
            {
                return null;
            }
        }

        public NaturezaOperacao BuscaNaturezaOperacao(Guid itbc_natureza_operacaoid)
        {
            NaturezaOperacao naturezaOperacao = RepositoryService.NaturezaOperacao.ObterPor(itbc_natureza_operacaoid);
            if (naturezaOperacao != null)
                return naturezaOperacao;
            return null;
        }

        public List<NaturezaOperacao> ListarTodos()
        {
            return RepositoryService.NaturezaOperacao.ListarTodos();
        }

        public NaturezaOperacao BuscaNaturezaOperacaoPorCodigo(String itbc_codigo_natureza_operacao)
        {
            NaturezaOperacao naturezaOperacao = RepositoryService.NaturezaOperacao.ObterPor(itbc_codigo_natureza_operacao);
            if (naturezaOperacao != null)
                return naturezaOperacao;
            return null;
        }

        public List<NaturezaOperacao> BuscaNaturezaOperacaoPorCodigo(String[] conjCodigosNatureza)
        {
            List<NaturezaOperacao> naturezaOperacao = RepositoryService.NaturezaOperacao.ObterPor(conjCodigosNatureza);
            if (naturezaOperacao.Count() > 0)
                return naturezaOperacao;
            return null;
        }

        public NaturezaOperacao ExtrairNaturezaOperacaoPorCodigo(String itbc_codigo_natureza_operacao, List<NaturezaOperacao> listaNaturezaOperacao)
        {
            var naturezasOperacaoExtraidas = from x in listaNaturezaOperacao
                        where x.Codigo.ToLower() == itbc_codigo_natureza_operacao.ToLower()
                        orderby x.Nome ascending
                        select x;

            if (naturezasOperacaoExtraidas.Count() > 0)
                return naturezasOperacaoExtraidas.First();
            return null;
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.NaturezaOperacao.AlterarStatus(id, status);
        }

        #endregion


    }
}
