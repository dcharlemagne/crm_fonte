using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class TabelaFinanciamentoService
    {
        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public TabelaFinanciamentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }

        public TabelaFinanciamentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion
        //persistir utilizado para nao permitir mudar o proprietario
        public TabelaFinanciamento Persistir(TabelaFinanciamento ObjTblFinanciamento)
        {

            TabelaFinanciamento TmpTabelaFinanciamento = null;

            if (!String.IsNullOrEmpty(ObjTblFinanciamento.Nome))
            {
                TmpTabelaFinanciamento = RepositoryService.TabelaFinanciamento.ObterPor(ObjTblFinanciamento.Nome);

                if (TmpTabelaFinanciamento != null)
                {
                    ObjTblFinanciamento.ID = TmpTabelaFinanciamento.ID;

                    RepositoryService.TabelaFinanciamento.Update(ObjTblFinanciamento);

                    if (!TmpTabelaFinanciamento.State.Equals(ObjTblFinanciamento.State) && ObjTblFinanciamento.State != null)
                        this.MudarStatus(TmpTabelaFinanciamento.ID.Value, ObjTblFinanciamento.State.Value);

                    return TmpTabelaFinanciamento;
                }
                else
                {
                    ObjTblFinanciamento.ID = RepositoryService.TabelaFinanciamento.Create(ObjTblFinanciamento);
                    return ObjTblFinanciamento;
                }
            }
            else
            {
                return null;
            }
        }

        public TabelaFinanciamento ObterTabelaFinanciamento(string NumeroTabela)
        {
            return RepositoryService.TabelaFinanciamento.ObterPor(NumeroTabela);
        }
        public bool MudarStatus(Guid id, int stateCode)
        {
            int statusCode;
            if (stateCode == 0)
            {
                //Ativar
                statusCode = 1;
            }
            else
            {
                //Inativar
                statusCode = 2;
            }

            return RepositoryService.TabelaFinanciamento.AlterarStatus(id, stateCode, statusCode);
        }

    }
}
