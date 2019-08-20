using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using SDKore.Crm;
using SDKore.Crm.Util;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using SDKore.DomainModel;
using System.Web.UI.WebControls;

namespace Intelbras.CRM2013.DAL
{
    class RepUtil<T> : CrmServiceRepository<T>, IUtil<T>
    {
        /// <summary>
        /// Mudar Proprietário do registro
        /// </summary>
        /// <param name="ObjInstanciado">Um objeto instanciado da classe</param>
        /// <param name="proprietario">Novo Proprietário</param>
        /// <param name="TipoProprietario">Novo Tipo de Proprietário</param>
        /// <returns></returns>
        public bool MudarProprietarioRegistro(T objProprietario, Guid idProprietarioNovo, T entidadeDestino, Guid idEntidadeDestino)
        {
            Microsoft.Crm.Sdk.Messages.AssignRequest assignRequest = new Microsoft.Crm.Sdk.Messages.AssignRequest()
            {
                Assignee = new Microsoft.Xrm.Sdk.EntityReference
                {
                    LogicalName = this.ObterNomeEntidade(objProprietario),
                    Id = idProprietarioNovo
                },

                Target = new Microsoft.Xrm.Sdk.EntityReference(this.ObterNomeEntidade(entidadeDestino), idEntidadeDestino)
            };

            if (this.Execute(assignRequest).Results.Any())
                return false;
            else
                return true;
        }

        public bool MudarProprietarioRegistro(string tipoProprietario, Guid idProprietarioNovo, string entidadeDestino, Guid idEntidadeDestino)
        {
            Microsoft.Crm.Sdk.Messages.AssignRequest assignRequest = new Microsoft.Crm.Sdk.Messages.AssignRequest()
            {
                Assignee = new Microsoft.Xrm.Sdk.EntityReference
                {
                    LogicalName = tipoProprietario, //team ou systemuser
                    Id = idProprietarioNovo
                },

                Target = new Microsoft.Xrm.Sdk.EntityReference(entidadeDestino, idEntidadeDestino)
            };

            if (this.Execute(assignRequest).Results.Any())
                return false;
            else
                return true;
        }

        /// <summary>
        /// Obtem o nome da entidade da classe passada
        /// </summary>
        /// <param name="classe">Objeto instanciado da classe</param>
        /// <returns></returns>
        public string ObterNomeEntidade(T classe)
        {
            LogicalEntity atributo = (LogicalEntity)classe.GetType().GetCustomAttributes(typeof(LogicalEntity), false)[0];

            if (!String.IsNullOrEmpty(((LogicalEntity)atributo).Name))
                return ((LogicalEntity)atributo).Name;
            else
                return null;
        }

        /// <summary>
        /// Criar lookup de proprietário
        /// </summary>
        /// <param name="Proprietario">Guid do proprietário</param>
        /// <param name="TipoProprietario">String do tipo de proprietário</param>
        /// <returns>Lookup criado do proprietário</returns>
        public SDKore.DomainModel.Lookup CriarLookupProprietario(Guid Proprietario, string TipoProprietario)
        {
            string tipoObjetoProprietario;

            if (TipoProprietario == "team" || TipoProprietario == "systemuser")
                tipoObjetoProprietario = TipoProprietario;
            else
                tipoObjetoProprietario = "systemuser";

            return new SDKore.DomainModel.Lookup(Proprietario, tipoObjetoProprietario);
        }
    }
}