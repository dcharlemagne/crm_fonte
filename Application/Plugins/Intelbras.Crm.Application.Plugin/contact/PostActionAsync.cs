using System;
using System.Text;
using System.Web.Services.Protocols;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Services;
using Intelbras.Crm.Domain.ValueObjects;
using Intelbras.Helper.Log;

namespace Intelbras.Crm.Application.Plugin.contact
{
    public class PostActionAsync : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                FacadeContato facede = new FacadeContato(context);
                facede.PostAction();
            }
            catch (Exception ex)
            {
                string mensagem = LogHelper.Process(ex, ClassificacaoLog.PluginContact);
                throw new InvalidPluginExecutionException(mensagem, ex);
            }
        }
    }
}
