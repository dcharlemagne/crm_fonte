using System;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model;

namespace Intelbras.Crm.Application.Plugin.account
{
    public class FacedeCliente
    {
        private readonly DynamicEntity Entidade;
        private readonly string OrganizacaoName;
        private readonly string MessageName;
        private readonly Guid Id;

        public FacedeCliente(IPluginExecutionContext contexto)
        {
            this.Entidade = PluginHelper.GetDynamicEntity(contexto);
            this.OrganizacaoName = contexto.OrganizationName;
            this.Id = PluginHelper.GetEntityId(contexto);
            this.MessageName = contexto.MessageName;
        }

        public void Atender()
        {
            switch (this.MessageName)
            {
                case Microsoft.Crm.Sdk.MessageName.Create:
                    Create();
                    break;

                case Microsoft.Crm.Sdk.MessageName.Update:
                    
                    break;
            }
        }

        private void Create()
        {
            Cliente cliente = new FactoryCliente(this.OrganizacaoName, this.Id).Instanciar(this.Entidade);

            if (cliente.ExisteDuplicidade())
            {
                throw new ArgumentException("O cliente não pode ser criado, existe duplicidade no CPF/CNPJ ou Código do Cliente");
            }

            PluginHelper.AdicionarPropriedadeEmEntidadeDinamica(this.Entidade, "new_sem_masc_cnpj_cpf", cliente.ObterCpfCnpjSemMascara());
        }

    }
}
