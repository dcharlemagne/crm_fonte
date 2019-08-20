using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.Crm.Domain.Model;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model.Enum;
using Intelbras.Crm.Domain.Services;

namespace Intelbras.Crm.Application.Plugin.incident
{
    public class FacadeOcorrencia
    {
        Ocorrencia ocorrencia = null;

        public Ocorrencia Ocorrencia
        {
            get { return service.Ocorrencia; }
        }

        bool alteracaoDoStatus = false;
        Guid Id = Guid.Empty;
        string Organizacao = null;
        string MessageName;
        OcorrenciaService service; 

        public FacadeOcorrencia(IPluginExecutionContext contexto)
        {
            var entidade = PluginHelper.GetDynamicEntity(contexto);
            
            if (entidade.Properties.Contains("statuscode"))
                alteracaoDoStatus = true;

            this.Organizacao = contexto.OrganizationName;
            this.Id = PluginHelper.GetEntityId(contexto);
            this.MessageName = contexto.MessageName;

            var factory = new FactoryOcorrencia(this.Organizacao, this.Id);

            if (contexto.PreEntityImages.Contains("preImage") && contexto.PreEntityImages["preImage"] is DynamicEntity)
                ocorrencia = factory.CriarOcorrencia(entidade, contexto.PreEntityImages["preImage"] as DynamicEntity);
            else if (contexto.PostEntityImages.Contains("postImage") && contexto.PostEntityImages["postImage"] is DynamicEntity)
                ocorrencia = factory.CriarOcorrencia(entidade, contexto.PostEntityImages["postImage"] as DynamicEntity);
            else
                ocorrencia = factory.CriarOcorrencia(entidade);


            service = new OcorrenciaService(ocorrencia);
        }

        public void Atender()
        {
            switch (this.MessageName)
            {
                case Microsoft.Crm.Sdk.MessageName.Create:
                    this.service.Criar();
                    break;

                case Microsoft.Crm.Sdk.MessageName.Update:
                    this.service.Atualizar();

                    if (alteracaoDoStatus)
                    {
                        this.service.AtualizarValorDoServicoASTEC();
                    }
                    break;
            }
        }
        
         public void PosAlteracao() {

            this.service.EncerraOcorrenciaSeNecessario();
        }
    
    }
}
