using System;
using System.Collections.Generic;
using System.Text;
using Intelbras.Crm.Domain.Model;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model.Enum;
using Intelbras.Crm.Domain.Services;
using Intelbras.Helper.Log;

namespace Intelbras.Crm.Application.Plugin.contact
{
    public class FacadeContato
    {
        Contato contato = null;

        public Contato Contato
        {
            get { return this.contato; }
        }

        Guid Id = Guid.Empty;
        string Organizacao = null;
        string MessageName;
        IPluginExecutionContext contexto;
        DynamicEntity entidade;

        private DynamicEntity PostImage
        {
            get
            {
                if (this.contexto.PostEntityImages.Contains("postImage") && this.contexto.PostEntityImages["postImage"] is DynamicEntity)
                {
                    return contexto.PostEntityImages["postImage"] as DynamicEntity;
                }

                return null;
            }
        }

        private DynamicEntity PreImage
        {
            get
            {
                if (this.contexto.PreEntityImages.Contains("preImage") && this.contexto.PreEntityImages["preImage"] is DynamicEntity)
                {
                    return contexto.PreEntityImages["preImage"] as DynamicEntity;
                }

                return null;
            }
        }

        public FacadeContato(IPluginExecutionContext contexto)
        {
            entidade = PluginHelper.GetDynamicEntity(contexto);
            this.contexto = contexto;

            this.Organizacao = contexto.OrganizationName;
            this.Id = PluginHelper.GetEntityId(contexto);
            this.MessageName = contexto.MessageName;

            var factory = new FactoryContato(this.Organizacao);

            if (this.PreImage != null)
            {
                this.contato = factory.CriarContato(this.PreImage, entidade);
            }
            else if (this.PostImage != null)
            {
                this.contato = factory.CriarContato(this.PostImage, entidade);
            }
            else
            {
                this.contato = factory.CriarContato(entidade);
                if (this.contato.Id == Guid.Empty)
                    this.contato.Id = PluginHelper.GetEntityId(contexto);
            }
        }

        public void PodeIntegrar(ref DynamicEntity entity)
        {
            if (!entity.Properties.Contains("parentcustomerid")
                && !entity.Properties.Contains("firstname")
                && !entity.Properties.Contains("emailaddress1"))
                return;

            entity = PluginHelper.AdicionarPropriedadeEmEntidadeDinamica(entity, "new_status_integracao", string.Empty);
            entity = PluginHelper.AdicionarPropriedadeEmEntidadeDinamica(entity, "new_mensagem", string.Empty);
            entity = PluginHelper.AdicionarPropriedadeEmEntidadeDinamica(entity, "new_chave_integracao", string.Empty);


           // var factory = new FactoryContato(this.Organizacao);
           // var contatoAtual = factory.CriarContato(PluginHelper.GetDynamicEntity(contexto));


            bool integra = this.contato.PodeIntegrarComERP();
            this.IntegraConnector(ref entity, integra);

            // Se o contato estiver integrado com ERP e após salvar não cair mais na regra, precisa excluir de lá.
            if (this.MessageName == Microsoft.Crm.Sdk.MessageName.Update && !integra)
            {
                if (this.PreImage == null)
                    throw new ArgumentNullException("PreImage", "Não foi encontrado a preImage");

                var factory = new FactoryContato(this.Organizacao);
                var contatoAntigo = factory.CriarContato(this.PreImage);

                if (contatoAntigo.PodeIntegrarComERP())
                    this.IntegraConnector(ref entity, true, 'D');

            }
        }

        public void PostAction()
        {
            switch (this.MessageName)
            {
                case Microsoft.Crm.Sdk.MessageName.Create:
                    this.PostCreate();
                    break;

                case Microsoft.Crm.Sdk.MessageName.Update:
                    this.PostUpdate();
                    break;
            }
        }

        private void PostUpdate()
        {
            //Atualiza o e-mail do contato no banco do FBA
            if (entidade.Properties.Contains("emailaddress1") && !string.IsNullOrEmpty(this.Contato.Login))
                try
                {
                    this.Contato.UpdateEmailFBA();
                }
                catch (Exception ex)
                {
                    LogHelper.Process(ex, ClassificacaoLog.PluginContact);
                }
            
            if (entidade.Properties.Contains("new_login"))
                this.Contato.EnviaEmailAcessoPoralCorporativo();
        }

        private void PostCreate()
        {
            //Envia e-mail para o contato com o aviso de cadastro de login e senha no portal corporativo
            if (entidade.Properties.Contains("emailaddress1") && entidade.Properties.Contains("new_login"))
                this.Contato.EnviaEmailAcessoPoralCorporativo();
        }

        private void IntegraConnector(ref DynamicEntity entity, bool integra, char action = ' ')
        {
            if (integra)
                entity = PluginHelper.AdicionarPropriedadeEmEntidadeDinamica(entity, "new_exporta_erp", "1");
            else
                entity.Properties.Remove("new_exporta_erp");


            if (action == ' ' && integra)
            {
                switch (this.MessageName)
                {
                    case Microsoft.Crm.Sdk.MessageName.Create:
                        action = 'C';
                        break;

                    case Microsoft.Crm.Sdk.MessageName.Update:
                        action = 'W';
                        break;

                    case Microsoft.Crm.Sdk.MessageName.Delete:
                        action = 'D';
                        break;
                }
            }

            this.contexto.SharedVariables.Properties.Add(new PropertyBagEntry("action", action));

        }

    }
}
