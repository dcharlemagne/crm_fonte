using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using System.Diagnostics;
using Intelbras.Crm.Domain.Model;

namespace Intelbras.Crm.Application.Plugin.new_classificacao_assunto
{
    public class PreCreate : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            DynamicEntity entity = null;

            try
            {
                var isDynamicEntity = (context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity);
                if (!isDynamicEntity) return;

                    Organizacao organizacao = new Organizacao(context.OrganizationName); 
                    entity = (DynamicEntity)context.InputParameters.Properties["Target"];

                   

                    if (!entity.Properties.Contains("new_tipo_assunto")) return;

                    string newTipoAssunto = entity.Properties["new_tipo_assunto"].ToString();

                    if (!entity.Properties.Contains("new_assuntoid")) return;

                    string newAssundoid = entity.Properties["new_assuntoid"].ToString();




            }
            catch (InvalidPluginExecutionException exception)
            {
                throw exception;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("Application", String.Format("Houve um problema ao executar o plugin 'new_classificacao_assunto.PreCreate': Mensagem: {0} -- StackTrace: {1} \n--{2}", ex.Message, ex.StackTrace, ex.InnerException));
            }

        }
    }
}
