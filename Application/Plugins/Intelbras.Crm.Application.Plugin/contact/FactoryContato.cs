using System;
using System.Collections.Generic;
using System.Text;
using Intelbras.Crm.Domain.Model;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model.Enum;
using Intelbras.Crm.Domain.ValueObjects;

namespace Intelbras.Crm.Application.Plugin.contact
{
    public class FactoryContato
    {
        Organizacao Organizacao = null;

        public FactoryContato(string organizacao)
        {
            this.Organizacao = new Organizacao(organizacao);
        }

        public Contato CriarContato(params  DynamicEntity[] entidades)
        {
            Contato contato = new Contato(this.Organizacao);

            foreach (var entidade in entidades)
            {
                contato = this.CriarContato(contato, entidade);
            }

            return contato;
        }

        private Contato CriarContato(Contato contato, DynamicEntity entidade)
        {
            if (entidade.Properties.Contains("contactid"))
                contato.Id = ((Key)entidade.Properties["contactid"]).Value;


            if (entidade.Properties.Contains("firstname"))
                contato.Nome = entidade.Properties["firstname"].ToString();


            if (entidade.Properties.Contains("emailaddress1"))
                contato.Email = entidade.Properties["emailaddress1"].ToString();


            if (entidade.Properties.Contains("new_login"))
                contato.Login = entidade.Properties["new_login"].ToString();

            if (entidade.Properties.Contains("new_participafidelidade") && !((CrmBoolean)entidade.Properties["new_participafidelidade"]).IsNull)
                contato.AcessoPortalFidelidade = ((CrmBoolean)entidade.Properties["new_participafidelidade"]).Value;

            
            if (entidade.Properties.Contains("parentcustomerid"))
            {
                var customerid = (Customer)entidade.Properties["parentcustomerid"];

                if (customerid.IsNull || customerid.IsNullSpecified)
                    contato.ClienteVO = null;
                else
                    contato.ClienteVO = new LookupVO
                    {
                        Id = customerid.Value,
                        Nome = customerid.name,
                        Tipo = customerid.type
                    };
            }

            return contato;
        }
    }
}
