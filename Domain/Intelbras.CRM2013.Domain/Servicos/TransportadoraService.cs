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
    public class TransportadoraService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public TransportadoraService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public TransportadoraService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        public Transportadora Persistir(Transportadora ObjTransportadora, ref bool MudancaProprietario)
        {
            Transportadora TmpTransportadora = null;

            if (ObjTransportadora.Codigo.HasValue)
            {
                TmpTransportadora = RepositoryService.Transportadora.ObterPor(ObjTransportadora.Codigo.Value.ToString());
                if (TmpTransportadora != null)
                {
                    ObjTransportadora.ID = TmpTransportadora.ID;

                    RepositoryService.Transportadora.Update(ObjTransportadora);

                     if (!TmpTransportadora.Status.Equals(ObjTransportadora.Status) && ObjTransportadora.Status != null)
                        this.MudarStatus(TmpTransportadora.ID.Value, ObjTransportadora.Status.Value);

                    return TmpTransportadora;
                }
                else
                {
                    ObjTransportadora.ID = RepositoryService.Transportadora.Create(ObjTransportadora);
                    return ObjTransportadora;
                }
            }
            else
            {
                return null;
            }
            
        }

        public  bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.Transportadora.AlterarStatus(id, status);
        }

        public Transportadora ObterPorCodigoTransportadora(int? codigoTransportadora)
        {
            Transportadora transportadora = RepositoryService.Transportadora.ObterPor(codigoTransportadora.ToString());
            if (transportadora != null)
                return transportadora;
            return null;
        
        }

        public Transportadora ObterPor(Guid transpId)
        {
            return RepositoryService.Transportadora.Retrieve(transpId);
        }

    }
}
