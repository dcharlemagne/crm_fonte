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
    public class PortadorService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PortadorService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public PortadorService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        //persistir utilizado para nao permitir mudar o proprietario
        public Portador Persistir(Model.Portador ObjPortador)
        {
            Portador TmpPortador = null;
            if (ObjPortador.CodigoPortador.HasValue)
            {
                TmpPortador = RepositoryService.Portador.ObterPor(ObjPortador.CodigoPortador.Value);

                if (TmpPortador != null)
                {
                    ObjPortador.ID = TmpPortador.ID;

                    RepositoryService.Portador.Update(ObjPortador);

                    if (!TmpPortador.Status.Equals(ObjPortador.Status) && ObjPortador.Status != null)
                        this.MudarStatus(TmpPortador.ID.Value, ObjPortador.Status.Value);

                    return TmpPortador;
                }
                else
                {
                    ObjPortador.ID = RepositoryService.Portador.Create(ObjPortador);
                    return ObjPortador;
                }
            }
            else
            {
                return null;
            }

        }

        public Model.Portador BuscaPorCodigo(int Codigo)
        {
            return RepositoryService.Portador.ObterPor(Codigo);
        }

        public Model.Portador BuscaPorCodigo(Guid Codigo)
        {
            return RepositoryService.Portador.ObterPor(Codigo);
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

            return RepositoryService.Portador.AlterarStatus(id, stateCode, statusCode);
        }
    }
}
