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
    public class EmpresasColigadasService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public EmpresasColigadasService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public EmpresasColigadasService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion
        //persistir utilizado para nao permitir mudar o proprietario
        public EmpresasColigadas Persistir(EmpresasColigadas objEmpresa,ref bool MudancaProprietario)
        {
            EmpresasColigadas TmpEmpresasColigadas = null;
            if (objEmpresa.ID.HasValue)
            {
                TmpEmpresasColigadas = RepositoryService.EmpresasColigadas.ObterPor((Guid)objEmpresa.ID);

                if (TmpEmpresasColigadas != null)
                {
                    objEmpresa.ID = TmpEmpresasColigadas.ID;

 
                    RepositoryService.EmpresasColigadas.Update(objEmpresa);

                        //this.MudarProprietario(objEmpresa.Proprietario.Id, objEmpresa.Proprietario.Type,(Guid)objEmpresa.ID);

                    if (!TmpEmpresasColigadas.Status.Equals(objEmpresa.Status) && objEmpresa.Status != null)
                        this.MudarStatus(TmpEmpresasColigadas.ID.Value, objEmpresa.Status.Value);

                    return TmpEmpresasColigadas;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                objEmpresa.ID = RepositoryService.EmpresasColigadas.Create(objEmpresa);
                return objEmpresa;
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

            return RepositoryService.EmpresasColigadas.AlterarStatus(id, stateCode, statusCode);
        }

        public bool MudarProprietario(Guid proprietario, string TipoProprietario,Guid EmpresaColigada)
        {
            return RepositoryService.EmpresasColigadas.AlterarProprietario(proprietario, TipoProprietario, EmpresaColigada);
        }
    }
}
