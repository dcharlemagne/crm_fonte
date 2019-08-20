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
    public class EstabelecimentoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public EstabelecimentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public EstabelecimentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion


        #region Métodos

        public Estabelecimento Persistir(Estabelecimento objEstabelecimento)
        {
            Estabelecimento tmpEstabelecimento = RepositoryService.Estabelecimento.ObterPor(objEstabelecimento.Codigo.Value);

            if (tmpEstabelecimento != null)
            {

                objEstabelecimento.ID = tmpEstabelecimento.ID;
                RepositoryService.Estabelecimento.Update(objEstabelecimento);
                //Altera Status - Se necessário
                if (!tmpEstabelecimento.State.Equals(objEstabelecimento.State) && objEstabelecimento.State != null)
                    this.MudarStatus(tmpEstabelecimento.ID.Value, objEstabelecimento.State.Value);
                return tmpEstabelecimento;

            }
            else
            {
                objEstabelecimento.ID = RepositoryService.Estabelecimento.Create(objEstabelecimento);
                return objEstabelecimento;
            }
        }

        public Estabelecimento BuscaEstabelecimento(Guid itbc_estabelecimentoid)
        {
            List<Estabelecimento> lstEstabelecimento = RepositoryService.Estabelecimento.ListarPor(itbc_estabelecimentoid);
            if (lstEstabelecimento.Count > 0)
                return lstEstabelecimento.First<Estabelecimento>();
            return null;
        }

        public List<Estabelecimento> BuscaTodosEstabelecimentos(params string[] columns)
        {
            return RepositoryService.Estabelecimento.ListarTodos(columns);
        }

        public Estabelecimento BuscaEstabelecimentoPorCodigo(int? itbc_codigo_estabelecimento)
        {
            Estabelecimento estabelecimento = RepositoryService.Estabelecimento.ObterPor(itbc_codigo_estabelecimento.Value);
            if (estabelecimento != null)
                return estabelecimento;
            return null;
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.Estabelecimento.AlterarStatus(id, status);
        }

        #endregion

    }
}
