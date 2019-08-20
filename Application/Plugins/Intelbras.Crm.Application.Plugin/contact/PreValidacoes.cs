using System;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Services;
using Microsoft.Crm.Sdk;

/* *
  * Valida se está tantando salvar um contato com Login(new_login) ou CPF(new_cpf) já existente!
 * Filtering Attributes: birthdate
 *                       emailaddress1
 *                       firstname
 *                       new_cpf     
 *                       new_login                   
 *                       parentcustomerid
 *                       
 * */


namespace Intelbras.Crm.Application.Plugin.contact
{
    public class PreValidacoes : IPlugin
    {
        DynamicEntity entity = null;

        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                var isDynamicEntity = (context.InputParameters.Properties.Contains("Target") && context.InputParameters.Properties["Target"] is DynamicEntity);
                if (!isDynamicEntity) return;

                Organizacao organizacao = new Organizacao(context.OrganizationName);
                entity = (DynamicEntity)context.InputParameters.Properties["Target"];

                Contato contato = new Contato(organizacao);

                if (context.MessageName == MessageName.Update) contato.Id = PluginHelper.GetEntityId(context);
                if (entity.Properties.Contains("new_login")) contato.Login = entity.Properties["new_login"].ToString();
                if (entity.Properties.Contains("new_cpf")) contato.Cpf = entity.Properties["new_cpf"].ToString();

                String mensagem;
                if (contato.ExisteDuplicidade(out mensagem))
                    throw new InvalidPluginExecutionException(mensagem);

                this.PopulaCamposDataAniversario();

                FacadeContato facade = new FacadeContato(context);
                    facade.PodeIntegrar(ref entity);
            }
            catch (Exception ex) { PluginHelper.TratarExcecao(ex, TipoDeLog.PluginContact); }
        }

        public void PopulaCamposDataAniversario()
        {
            if (!entity.Properties.Contains("birthdate"))
                return;

            CrmDateTime dataAniversarioCrm = (CrmDateTime)entity.Properties["birthdate"];

            if (dataAniversarioCrm.IsNull)
                return;

            DateTime dataAniversario = Convert.ToDateTime(dataAniversarioCrm.Value);

            #region Dia
            PicklistProperty DiaCrm = new PicklistProperty()
            {
                Value = new Picklist(dataAniversario.Day),
                Name = "new_aniversario_dia"
            };
            if (entity.Properties.Contains("new_aniversario_dia")) entity.Properties.Remove("new_aniversario_dia");
            entity.Properties.Add(DiaCrm);
            #endregion

            #region Mes
            PicklistProperty MesCrm = new PicklistProperty()
            {
                Value = new Picklist(dataAniversario.Month),
                Name = "new_aniversario_mes"
            };
            if (entity.Properties.Contains("new_aniversario_mes")) entity.Properties.Remove("new_aniversario_mes");
            entity.Properties.Add(MesCrm);
            #endregion
        }

    }
}
