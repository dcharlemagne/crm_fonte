using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model;
using System.Diagnostics;

namespace Intelbras.Crm.Application.Plugin.new_calendario_treinamento
{
    public class PreDelete : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                Agenda agenda = new Agenda(new Organizacao(context.OrganizationName)) { Id = PluginHelper.GetEntityId(context) };

                if (!agenda.PodeExcluirAgenda())
                    throw new InvalidPluginExecutionException("A agenda já iniciou ou tem participante inscrito, não pode mais ser excluida! Você pode cancelar!");
                               
                agenda.ExcluirCompromissos();
            }
            catch (Exception ex)
            {
                string errormessage = String.Format("Houve um problema ao executar o plugin new_calendario_treinamento.PostDelete': Mensagem: {0} -- StackTrace: {1} \n--{2}", ex.Message, ex.StackTrace, ex.InnerException);
                EventLog.WriteEntry("Application CRM", errormessage);
                throw ex;
            }
        }
    }
}
