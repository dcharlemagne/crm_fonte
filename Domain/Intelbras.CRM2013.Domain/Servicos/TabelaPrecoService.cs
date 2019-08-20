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
    public class TabelaPrecoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public TabelaPrecoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public TabelaPrecoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }
        
        #endregion

        #region Métodos

        public TabelaPreco Persistir(TabelaPreco objTabelaPreco)
        {
            TabelaPreco tmpTabelaPreco = null;
            if (objTabelaPreco.ID.HasValue)
            {
                tmpTabelaPreco = RepositoryService.TabelaPreco.ObterPor(objTabelaPreco.ID.Value);

                if (tmpTabelaPreco != null)
                {
                    objTabelaPreco.ID = tmpTabelaPreco.ID;
                    RepositoryService.TabelaPreco.Update(objTabelaPreco);
                    //Altera Status - Se necessário
                    if (!tmpTabelaPreco.Status.Equals(objTabelaPreco.Status) && objTabelaPreco.Status != null)
                        this.MudarStatus(tmpTabelaPreco.ID.Value, objTabelaPreco.Status.Value);
                    return tmpTabelaPreco;
                }
                else
                    return null;
            }
            else
            {
                objTabelaPreco.ID = RepositoryService.TabelaPreco.Create(objTabelaPreco);
                return objTabelaPreco;
            }
        }

        public TabelaPreco BuscaTabelaPreco(Guid itbc_tabeladeprecoid)
        {
            TabelaPreco tabelaPreco = RepositoryService.TabelaPreco.ObterPor(itbc_tabeladeprecoid);
            if (tabelaPreco != null)
                return tabelaPreco;
            return null;
        }

        public TabelaPreco BuscaTabelaPrecoPorCodigo(int itbc_codigo_tabela)
        {
            TabelaPreco tabelaPreco = RepositoryService.TabelaPreco.ObterPor(itbc_codigo_tabela);
            if (tabelaPreco != null)
                return tabelaPreco;
            return null;
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.TabelaPreco.AlterarStatus(id, status);
        }

        #endregion
    }
}
