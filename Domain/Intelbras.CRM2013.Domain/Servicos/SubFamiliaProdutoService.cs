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
    public class SubFamiliaProdutoService
    {
         #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SubFamiliaProdutoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }

        public SubFamiliaProdutoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        public SubfamiliaProduto BuscaSubfamiliaProduto(String codigoSubfamiliaProduto)
        {
            SubfamiliaProduto subfamiliaProduto = RepositoryService.SubfamiliaProduto.ObterPor(codigoSubfamiliaProduto);
            if (subfamiliaProduto != null)
                return subfamiliaProduto;
            return null;
        }

        public SubfamiliaProduto Persistir(SubfamiliaProduto ObjSubFamiliaProduto, ref bool MudancaProprietario)
        {
            SubfamiliaProduto TmpSubFamiliaProduto = null;
            if (!String.IsNullOrEmpty(ObjSubFamiliaProduto.Codigo))
            {
                TmpSubFamiliaProduto = RepositoryService.SubfamiliaProduto.ObterPor(ObjSubFamiliaProduto.Codigo);

                if (TmpSubFamiliaProduto != null)
                {
                    ObjSubFamiliaProduto.ID = TmpSubFamiliaProduto.ID;


                    RepositoryService.SubfamiliaProduto.Update(ObjSubFamiliaProduto);

                    //TODO:Proprietario autentificacao
                    //if (!TmpSubFamiliaProduto.Proprietario.Id.Equals(ObjSubFamiliaProduto.Proprietario.Id) && ObjSubFamiliaProduto.Proprietario.Id != null)
                    //{
                    //    MudancaProprietario = true;
                    //    return TmpSubFamiliaProduto;
                    //}

                    if (!TmpSubFamiliaProduto.Status.Equals(ObjSubFamiliaProduto.Status) && ObjSubFamiliaProduto.Status != null)
                        this.MudarStatus(TmpSubFamiliaProduto.ID.Value, ObjSubFamiliaProduto.Status.Value);

                    return TmpSubFamiliaProduto;
                }
                else
                {
                    ObjSubFamiliaProduto.ID = RepositoryService.SubfamiliaProduto.Create(ObjSubFamiliaProduto);
                    return ObjSubFamiliaProduto;
                }
            }
            else
            {
                return null;
            }
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

            return RepositoryService.SubfamiliaProduto.AlterarStatus(id, stateCode, statusCode);
        }

        public SubfamiliaProduto ObterPor(Guid subFamiliaProdutoId)
        {
            return RepositoryService.SubfamiliaProduto.ObterPor(subFamiliaProdutoId);
        }
    }
}
