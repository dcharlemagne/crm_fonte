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
    public class ListaPSDService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ListaPSDService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public ListaPSDService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        public ListaPrecoPSDPPPSCF ObterPor(Guid PsdId)
        {
            return RepositoryService.ListaPrecoPSD.ObterPor(PsdId);
        }

        /// <summary>
        /// Retorna true se o registro é duplicado
        /// </summary>
        /// <param name="listaPSD"></param>
        /// <returns></returns>
        public bool ValidarExistencia(Model.ListaPrecoPSDPPPSCF listaPSD,List<Guid> lstEstados)
        {
            Guid? listaGuid = Guid.Empty;

            #region Validação de valores
            if (listaPSD.UnidadeNegocio == null)
                throw new ArgumentException("Unidade de Negócio não informada.");
            //throw new ArgumentNullException("Unidade de Negócio não informada.");
            if (!listaPSD.DataInicio.HasValue)
                throw new ArgumentException("Data início não informada.");
            //throw new ArgumentNullException("Data início não informada.");
            if (!listaPSD.DataFim.HasValue)
                throw new ArgumentException("Data fim não informada.");
            //throw new ArgumentNullException("Data fim não informada.");

            //Verificamos se ele enviou o proprio guid pois a funcao eh usada no create/update
            if (listaPSD.ID.HasValue)
            {
                //se enviou o guid é porque nao é create, ai pega o guid do registro e pega os estados relacionados
                listaGuid = listaPSD.ID.Value;
                if (lstEstados.Count <= 0)
                {
                    List<ListaPrecoPSDEstado> lstPrecoPsdEstado = RepositoryService.ListaPrecoPSDEstado.ListarPor(null, listaGuid);
                    foreach (var _registro in lstPrecoPsdEstado)
                    {
                        lstEstados.Add(_registro.Estado.Value);
                    }
                }
            }
            else
                listaGuid = null;

            #endregion

            var lista = RepositoryService.ListaPrecoPSD.ListarPor(listaPSD.UnidadeNegocio.Id, lstEstados,listaGuid, listaPSD.DataInicio.Value, listaPSD.DataFim.Value);
            if (lista.Count == 0)
                return false;

            return true;
            //var lista = RepositoryService.ListaPrecoPSD.ListarPor(listaPSD.UnidadeNegocio.Id, listaPSD.ID);
            //if (lista.Count == 0)
            //    return false;

            //var inicio = lista.FindAll(obj => obj.DataInicio.Value <= listaPSD.DataInicio.Value && obj.DataFim.Value >= listaPSD.DataInicio.Value);
            //var fim = lista.FindAll(obj => obj.DataInicio.Value <= listaPSD.DataFim.Value && obj.DataFim.Value >= listaPSD.DataFim.Value);
            //if (inicio.Count >= 1 || fim.Count >= 1)
            //    return true;

            //return false;
        }

        public string IntegracaoBarramento(ListaPrecoPSDPPPSCF objListaPreco)
        {
            Domain.Integracao.MSG0087 msgProdEstab = new Domain.Integracao.MSG0087(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            return msgProdEstab.Enviar(objListaPreco);
        }
    }
}
