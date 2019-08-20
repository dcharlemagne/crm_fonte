using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;
using CodeK.Connector.PluginIntegration.CRM_ERP;
using Microsoft.Crm.Sdk;
using Tridea.Framework.DomainModel;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Model.Enum;
using Intelbras.Crm.Domain.Services;
using Intelbras.Crm.Domain.Repository;

namespace Intelbras.Crm.Application.Plugins.IntegradorEms
{
    public class Integrador : IDataIntegration
    {
        Organizacao organizacao = null;

        public void Send(string organizationName, string type, string xmlIntegration, DynamicEntity entity, IPluginExecutionContext context)
        {
            try
            {
                if (string.IsNullOrEmpty(xmlIntegration)) return;

                organizacao = new Organizacao(organizationName);

                switch (entity.Name)
                {
                    case "account":
                        SalvarClienteNoEMS(type, xmlIntegration, entity);
                        break;

                    case "contact":
                        SalvarContatoNoEMS(type, xmlIntegration, context);
                        break;

                    case "customeraddress":
                        SalvarEnderecoNoEMS(type, xmlIntegration, entity);
                        break;

                    case "new_relacionamento":
                        SalvarRelacionamentoNoEMS(type, xmlIntegration, entity);
                        break;
                }
            }
            catch (Exception ex) { LogService.GravaLog(ex, TipoDeLog.PluginIntegradorERP); }
        }


        #region Métodos Privados

        private void SalvarRelacionamentoNoEMS(string action, string xmlIntegrationLog, DynamicEntity entity)
        {
            Guid relacionamentoId = Guid.Empty;

            if (entity.Properties.Contains("new_relacionamentoid"))
                relacionamentoId = ((Key)entity.Properties["new_relacionamentoid"]).Value;

            IntegrarService integrarRelacionamento = new IntegrarService();
            MensagemDeRetornoEMS mensagemRetorno = new MensagemDeRetornoEMS();

            mensagemRetorno = integrarRelacionamento.Integrar("new_relacionamento", action, xmlIntegrationLog, relacionamentoId);

            Relacionamento relacionamento = new Relacionamento();

            relacionamento.Id = relacionamentoId;
            relacionamento.DescricaoDaMensagemDeIntegracao = mensagemRetorno.Descricao;
            relacionamento.CodigoEms = mensagemRetorno.CodigoEMS;
            relacionamento.StatusDaIntegracao = mensagemRetorno.StatusIntegracao;
            relacionamento.ExportaERP = "";

            DomainService.RepositoryCliente.AtualizarRelacionamento(relacionamento);
        }

        private void SalvarEnderecoNoEMS(string action, string xmlIntegrationLog, DynamicEntity entity)
        {
            Guid enderecoId = Guid.Empty;
            bool eOEnderecoDeUmaConta = false;

            if (entity.Properties.Contains("customeraddressid"))
                enderecoId = ((Key)entity.Properties["customeraddressid"]).Value;

            if (entity.Properties.Contains("objecttypecode"))
                eOEnderecoDeUmaConta = ((EntityNameReference)entity.Properties["objecttypecode"]).Value == "account";


            if (eOEnderecoDeUmaConta)
            {
                IntegrarService integrarEndereco = new IntegrarService();
                MensagemDeRetornoEMS mensagemRetorno = new MensagemDeRetornoEMS();

                mensagemRetorno = integrarEndereco.Integrar(entity.Name, action, xmlIntegrationLog, enderecoId);


                if (action.Equals("D"))
                    return;

                Endereco endereco = new Endereco(this.organizacao);
                endereco.Id = enderecoId;
                endereco.DescricaoDaMensagemDeIntegracao = mensagemRetorno.Descricao;
                endereco.CodigoEms = mensagemRetorno.CodigoEMS;
                endereco.StatusDaIntegracao = mensagemRetorno.StatusIntegracao;
                endereco.ExportaERP = "";
                endereco.CodigoIncrementalEMS = mensagemRetorno.CodigoEndereco;

                DomainService.RepositoryEndereco.Update(endereco);
            }
        }

        private void SalvarContatoNoEMS(string action, string xmlIntegrationLog, IPluginExecutionContext context)
        {
            if (context.SharedVariables.Contains("action"))
                action = context.SharedVariables["action"].ToString();

            if (action.Equals(" "))
                return;

            Contato contato = new Contato() { Id = this.GetId(context, "contact") };
            //if (!this.ContatoPodeIntegrar(context, ref action, out contato)) return;

            IntegrarService integrarContato = new IntegrarService();
            MensagemDeRetornoEMS mensagemRetorno = integrarContato.Integrar("contact", action, xmlIntegrationLog, contato.Id);

            if (context.MessageName == MessageName.Delete) return;
            if (context.MessageName == MessageName.Update && action.Equals("D")) mensagemRetorno.Descricao = string.Empty;

            contato.CodigoEms = mensagemRetorno.CodigoEMS;
            contato.DescricaoDaMensagemDeIntegracao = mensagemRetorno.Descricao;
            contato.StatusDaIntegracao = mensagemRetorno.StatusIntegracao;
            contato.ExportaERP = string.Empty;

            DomainService.RepositoryContato.AtualizarStatusIntegracaoERP(contato);
        }

        private void SalvarClienteNoEMS(string action, string xmlIntegrationLog, DynamicEntity entity)
        {
            Cliente cliente = new Cliente(organizacao);

            if (entity.Properties.Contains("accountid"))
                cliente.Id = ((Key)entity.Properties["accountid"]).Value;

            if (entity.Properties.Contains("new_status_cadastro"))
                cliente.StatusDaIntegracao = (StatusDaIntegracao)((Picklist)entity.Properties["new_status_cadastro"]).Value;

            IntegrarService integrarCliente = new IntegrarService();
            MensagemDeRetornoEMS mensagemRetorno = new MensagemDeRetornoEMS();

            mensagemRetorno = integrarCliente.Integrar("account", action, xmlIntegrationLog, cliente.Id);

            cliente.CodigoEms = mensagemRetorno.CodigoEMS;
            cliente.StatusDaIntegracao = mensagemRetorno.StatusIntegracao;
            cliente.DescricaoDaMensagemDeIntegracao = mensagemRetorno.Descricao;
            cliente.ExportaERP = "";

            DomainService.RepositoryCliente.Update(cliente);

            if (mensagemRetorno.StatusIntegracao == StatusDaIntegracao.CRMERP)
            {
                EnderecoPadraoDoCliente(entity, cliente);
            }
        }

        #region ENDEREÇOS

        private void CriarEndereco(DynamicEntity entity, Cliente cliente)
        {
            Endereco enderecoPrincipal = new Endereco(organizacao)
            {
                CodigoEms = cliente.CodigoEms + ",Padrão",
                Nome = "Padrão",
                ParentId = new Domain.ValueObjects.LookupVO(cliente.Id, "", entity.Name)
            };

            if (entity.Properties.Contains("address1_line1")) enderecoPrincipal.Logradouro = entity.Properties["address1_line1"].ToString();
            if (entity.Properties.Contains("address1_postalcode")) enderecoPrincipal.Cep = entity.Properties["address1_postalcode"].ToString();
            if (entity.Properties.Contains("address1_city")) enderecoPrincipal.Cidade = entity.Properties["address1_city"].ToString();
            if (entity.Properties.Contains("address1_line3")) enderecoPrincipal.Bairro = entity.Properties["address1_line3"].ToString();
            if (entity.Properties.Contains("address1_stateorprovince")) enderecoPrincipal.Uf = entity.Properties["address1_stateorprovince"].ToString();
            if (entity.Properties.Contains("address1_country")) enderecoPrincipal.Pais = entity.Properties["address1_country"].ToString();
            if (entity.Properties.Contains("address1_line2")) enderecoPrincipal.Complemento = entity.Properties["address1_line2"].ToString();
            if (entity.Properties.Contains("new_numero_endereco_principal")) enderecoPrincipal.Numero = entity.Properties["new_numero_endereco_principal"].ToString();

            DomainService.RepositoryEndereco.Create(enderecoPrincipal);
        }

        private void AtualizarEndereco(DynamicEntity entity, Endereco enderecoPrincipal)
        {
            if (entity.Properties.Contains("address1_line1")) enderecoPrincipal.Logradouro = entity.Properties["address1_line1"].ToString();
            if (entity.Properties.Contains("address1_postalcode")) enderecoPrincipal.Cep = entity.Properties["address1_postalcode"].ToString();
            if (entity.Properties.Contains("address1_city")) enderecoPrincipal.Cidade = entity.Properties["address1_city"].ToString();
            if (entity.Properties.Contains("address1_line3")) enderecoPrincipal.Bairro = entity.Properties["address1_line3"].ToString();
            if (entity.Properties.Contains("address1_stateorprovince")) enderecoPrincipal.Uf = entity.Properties["address1_stateorprovince"].ToString();
            if (entity.Properties.Contains("address1_country")) enderecoPrincipal.Pais = entity.Properties["address1_country"].ToString();
            if (entity.Properties.Contains("address1_line2")) enderecoPrincipal.Complemento = entity.Properties["address1_line2"].ToString();
            if (entity.Properties.Contains("new_numero_endereco_principal")) enderecoPrincipal.Numero = entity.Properties["new_numero_endereco_principal"].ToString();

            enderecoPrincipal.Nome = "Padrão";

            DomainService.RepositoryEndereco.UpdateAsync(enderecoPrincipal);
        }

        private void EnderecoPadraoDoCliente(DynamicEntity entity, Cliente cliente)
        {
            if (!entity.Properties.Contains("new_altera_endereco_padrao"))
            {
                return;
            }

            switch (entity.Properties["new_altera_endereco_padrao"].ToString())
            {
                case "u":
                    Endereco enderecoPrincipal = DomainService.RepositoryEndereco.ObterPor("Padrão", cliente, "customeraddressid");

                    if (enderecoPrincipal != null)
                    {
                        this.AtualizarEndereco(entity, enderecoPrincipal);
                    }
                    else
                    {
                        this.CriarEndereco(entity, cliente);
                    }
                    break;

                case "c":
                    this.CriarEndereco(entity, cliente);
                    break;

                case "n":
                    Endereco enderecoPrincipal1 = DomainService.RepositoryEndereco.ObterPor("Padrão", cliente, "customeraddressid", "new_chaveintegracao");

                    if (enderecoPrincipal1 != null)
                    {
                        string[] verificaDorDoCodigoEMS = enderecoPrincipal1.CodigoEms.Split(',');
                        if (String.IsNullOrEmpty(verificaDorDoCodigoEMS[0]))
                        {
                            enderecoPrincipal1.CodigoEms = cliente.CodigoEms + "," + verificaDorDoCodigoEMS[1];
                            ClienteService.AtualizaEnderecoPadrao(enderecoPrincipal1);
                        }
                    }
                    else
                        this.CriarEndereco(entity, cliente);
                    break;
            }
        }

        #endregion

        #endregion

        private Guid GetId(IPluginExecutionContext context, string entityName)
        {
            switch (context.MessageName)
            {
                case MessageName.Create:
                    if (context.OutputParameters.Contains(ParameterName.Id))
                    {
                        return (Guid)context.OutputParameters[ParameterName.Id];
                    }
                    break;

                case MessageName.Update:
                    if (context.InputParameters[ParameterName.Target] is DynamicEntity)
                    {
                        Key key = (Key)((DynamicEntity)context.InputParameters[ParameterName.Target]).Properties[entityName + "id"];
                        return key.Value;
                    }
                    break;

                case MessageName.Delete:
                    if (context.InputParameters[ParameterName.Target] is Moniker)
                    {
                        Moniker monikerId = (Moniker)context.InputParameters[ParameterName.Target];
                        return monikerId.Id;
                    }
                    break;
            }

            return Guid.Empty;
        }
    }
}