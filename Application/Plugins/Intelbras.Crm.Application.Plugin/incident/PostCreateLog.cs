using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using System.Diagnostics;
using Intelbras.Crm.Domain.Model;
using Tridea.Framework.DomainModel;
using Intelbras.Crm.Domain.Model.Enum;
using Intelbras.Crm.Domain.Repository;

namespace Intelbras.Crm.Application.Plugin.incident
{
    public class PostCreateLog: IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                DynamicEntity entity = null;
                var isDynamicEntity = (context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity);

                if (isDynamicEntity)
                {
                    entity = context.InputParameters.Properties["Target"] as DynamicEntity;
                    var incidentid = PluginHelper.GetEntityId(context);

                    this.CriarLog(context.UserId, entity, incidentid, context.OrganizationName);
                }

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", String.Format("Houve um problema ao executar o plugin 'incident.PostCreateLog': Mensagem: {0} -- StackTrace: {1} \n--{2}", ex.Message, ex.StackTrace, ex.InnerException));
            }
        }

        #region PRIVATE

        private void CriarLog(Guid usuarioId, DynamicEntity entity, Guid incidentId, string organizacao)
        {
            var _organizacao = new Organizacao(organizacao);

            Ocorrencia incident = new Ocorrencia(_organizacao);

            Usuario usuario = DomainService.RepositoryUsuario.Retrieve(usuarioId);

            incident = DomainService.RepositoryOcorrencia.Retrieve(incidentId);

            var LogOcorrencia = new LogOcorrencia(_organizacao);

            LogOcorrencia.UsuarioId = usuario.Id;
            LogOcorrencia.Usuario = string.Format("{0} {1}", usuario.Nome, usuario.Sobrenome);
            LogOcorrencia.Categoria = CategoriaLogOcorrencia.Diversos;
            LogOcorrencia.Alteracoes = string.Empty;
            LogOcorrencia.GrupoProprietario = usuario.GrupoCallCenter;
            LogOcorrencia.Nome = "Ocorrência criada.";
            LogOcorrencia.OcorrenciaId = incidentId;
            LogOcorrencia.Status = string.Empty;
            LogOcorrencia.DataAlteracao = DateTime.Now;
            LogOcorrencia.Salvar();
        }

        #endregion
    }
}
