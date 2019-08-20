using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Services;

namespace Intelbras.Crm.Application.Plugin.incident
{
    public class PostUpdateLogOwner : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                DynamicEntity preEntity = context.PreEntityImages["PreIncidentImage"] as DynamicEntity;
                DynamicEntity postEntity = context.PostEntityImages["PostIncidentImage"] as DynamicEntity;

                if (!preEntity.Properties.Contains("ownerid") && !postEntity.Properties.Contains("ownerid")) return;
                if (((Owner)preEntity.Properties["ownerid"]).Value == ((Owner)postEntity.Properties["ownerid"]).Value) return;


                DomainService.Organizacao = new Organizacao(context.OrganizationName);

                DomainService.RepositoryUsuario.Colunas = new string[] { "fullname", "new_grupo_callcenter" };
                Usuario proprietarioAntigo = DomainService.RepositoryUsuario.Retrieve(((Owner)preEntity.Properties["ownerid"]).Value);
                Usuario proprietarioNovo = DomainService.RepositoryUsuario.Retrieve(((Owner)postEntity.Properties["ownerid"]).Value);


                var log = new LogOcorrencia();
                log.Nome = "Atribuir Proprietário";
                log.OcorrenciaId = PluginHelper.GetEntityId(context);
                log.Categoria = Domain.Model.Enum.CategoriaLogOcorrencia.PorProprietario;
                log.DataAlteracao = DateTime.Now;

                log.Alteracoes = string.Format("Proprietário Anterior: {0} ({1}) \n Proprietário Novo: {2} (3)",
                                                proprietarioAntigo.NomeCompleto,
                                                proprietarioAntigo.Id,
                                                proprietarioNovo.NomeCompleto,
                                                proprietarioNovo.Id);


                log.Usuario = proprietarioAntigo.NomeCompleto;
                log.UsuarioId = proprietarioAntigo.Id;
                log.GrupoProprietario = proprietarioAntigo.GrupoCallCenter;
                log.GrupoDestino = proprietarioNovo.GrupoCallCenter;

                log.Salvar();
            }
            catch (Exception ex) { LogService.GravaLog(ex, TipoDeLog.PluginIncident, "incident.PostUpdateLogOwner"); }

        }
    }
}
