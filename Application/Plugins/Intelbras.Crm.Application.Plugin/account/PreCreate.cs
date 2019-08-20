using System;
using Microsoft.Crm.Sdk;
using Intelbras.Helper.Log;
using Intelbras.Crm.Domain.Model;

namespace Intelbras.Crm.Application.Plugin.account
{
    public class PreCreate : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                new FacedeCliente(context).Atender();
            }
            catch (Exception ex)
            {
                var mensagemErro = LogHelper.Process(ex, ClassificacaoLog.PluginContact);
                throw new InvalidPluginExecutionException(mensagemErro, ex);
            }
        }
    }
}
