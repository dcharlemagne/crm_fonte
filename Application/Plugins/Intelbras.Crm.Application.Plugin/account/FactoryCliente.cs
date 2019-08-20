using System;
using Intelbras.Crm.Domain.Model;
using Microsoft.Crm.Sdk;

namespace Intelbras.Crm.Application.Plugin.account
{
    public class FactoryCliente
    {
        private Organizacao Organizacao = null;
        private Guid Id = Guid.Empty;

        public FactoryCliente(string organizacao, Guid id)
        {
            this.Organizacao = new Organizacao(organizacao);
            this.Id = id;
        }

        public Cliente Instanciar(DynamicEntity entidade)
        {
            Cliente cliente  = new Cliente(this.Organizacao);
            cliente.Id = this.Id;

            if (entidade.Properties.Contains("new_cnpj"))
            {
                cliente.Cnpj = entidade.Properties["new_cnpj"].ToString();
            }

            if (entidade.Properties.Contains("new_cpf"))
            {
                cliente.Cpf = entidade.Properties["new_cpf"].ToString();
            }

            if (entidade.Properties.Contains("new_sem_masc_cnpj_cpf"))
            {
                cliente.CpfCnpjSemMascara = entidade.Properties["new_sem_masc_cnpj_cpf"].ToString();
            }

            if (entidade.Properties.Contains("accountnumber"))
            {
                cliente.CodigoEms = entidade.Properties["accountnumber"].ToString();
            }

            return cliente;
        }
    }
}
