using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Intelbras.CRM2013.Domain.SharepointWebService;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.IRepository;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.DAL
{
    public class RepClienteParticipante<T> : CrmServiceRepository<T>, IClienteParticipante<T>    
    {
        public ClienteParticipante InstanciarClienteParticipante(ClienteParticipanteDoContrato cli)
        {
            ClienteParticipante clienteParticipante = null;
            if (cli != null)
            {
                clienteParticipante = new ClienteParticipante(this.OrganizationName, this.IsOffline);

                clienteParticipante.Id = cli.Id;
                clienteParticipante.ContratoId = cli.Contrato.Id;
                clienteParticipante.ClienteId = cli.Cliente.Id;
                clienteParticipante.InicioVigencia = cli.InicioVigencia;
                clienteParticipante.FimVigencia = cli.FimVigencia;
                clienteParticipante.Descricao = cli.Descricao;
                clienteParticipante.Nome = cli.Nome;
                clienteParticipante.CodigoCliente = cli.CodigoCliente;
            }
            return clienteParticipante;
        }

        public ClienteParticipanteDoContrato InstanciarClienteParticipanteDoContrato(ClienteParticipante cli)
        {
            ClienteParticipanteDoContrato clienteParticipante = null;
            if (cli != null)
            {
                clienteParticipante = new ClienteParticipanteDoContrato(this.OrganizationName, this.IsOffline);

                clienteParticipante.Id = cli.Id;
                clienteParticipante.Contrato = new Lookup(cli.ContratoId.Value,"contract");
                clienteParticipante.Cliente = new Lookup(cli.ClienteId.Value,"account");
                clienteParticipante.InicioVigencia = cli.InicioVigencia;
                clienteParticipante.FimVigencia = cli.FimVigencia;
                clienteParticipante.Descricao = cli.Descricao;
                clienteParticipante.Nome = cli.Nome;
                clienteParticipante.CodigoCliente = cli.CodigoCliente;
            }
            return clienteParticipante;
        }

        public ClienteParticipante InstanciarClienteParticipante(ClienteParticipanteEndereco cli)
        {
            ClienteParticipante clienteParticipante = null;
            if (cli != null)
            {
                clienteParticipante = new ClienteParticipante(this.OrganizationName, this.IsOffline);

                clienteParticipante.Id = cli.Id;
                clienteParticipante.ContratoId = cli.ContratoId.Id;
                clienteParticipante.ClienteId = cli.ClienteId.Id;
                clienteParticipante.InicioVigencia = cli.DataInicialVigencia;
                clienteParticipante.FimVigencia = cli.DataFinalVigencia;
                clienteParticipante.Nome = cli.CodigoEndereco;
                clienteParticipante.ClienteParticipanteId = cli.ClienteParticipanteId.Id;
                clienteParticipante.EnderecoId = new Guid(cli.Endereco);
                clienteParticipante.CodigoDaLocalidade = cli.LocalidadeId.Id;
                clienteParticipante.Localidade = cli.LocalidadeId.Name;
                clienteParticipante.Cep = cli.Cep;
                clienteParticipante.Cidade = cli.Cidade;
                clienteParticipante.Logradouro = cli.Rua;
                clienteParticipante.Uf = cli.Uf;
                clienteParticipante.Bairro = cli.Bairro;
            }
            return clienteParticipante;
        }

        public ClienteParticipanteEndereco InstanciarClienteParticipanteEndereco(ClienteParticipante cli)
        {
            ClienteParticipanteEndereco clienteParticipante = null;
            if (cli != null)
            {
                clienteParticipante = new ClienteParticipanteEndereco(this.OrganizationName, this.IsOffline);

                clienteParticipante.Id = cli.Id;
                clienteParticipante.ContratoId = new Lookup(cli.ContratoId.Value, "contract");
                clienteParticipante.ClienteId = new Lookup(cli.ClienteId.Value, "account");
                clienteParticipante.DataInicialVigencia = cli.InicioVigencia.Value;
                clienteParticipante.DataFinalVigencia = cli.FimVigencia.Value;
                clienteParticipante.CodigoEndereco = cli.Nome;
                clienteParticipante.ClienteParticipanteId = new Lookup(cli.ClienteParticipanteId.Value, "new_cliente_participante_contrato");
                clienteParticipante.Endereco = cli.EnderecoId.Value.ToString();
                clienteParticipante.Localidade.Id = cli.CodigoDaLocalidade.Value;
                clienteParticipante.Localidade.Nome = cli.Localidade;
                clienteParticipante.Cep = cli.Cep;
                clienteParticipante.Cidade = cli.Cidade;
                clienteParticipante.Rua = cli.Logradouro;
                clienteParticipante.Uf = cli.Uf;
                clienteParticipante.Bairro = cli.Bairro;
            }
            return clienteParticipante;
        }

        public ClienteParticipante InstanciarClienteParticipante(Entity cli)
        {
            ClienteParticipante clienteParticipante = null;
            if (cli != null)
            {
                clienteParticipante = new ClienteParticipante(this.OrganizationName, this.IsOffline);

                clienteParticipante.Id = cli.Id;

                if (cli.Attributes.Contains("new_data_inicial"))
                    clienteParticipante.InicioVigencia = Convert.ToDateTime(cli["new_data_inicial"]);

                if (cli.Attributes.Contains("new_data_final"))
                    clienteParticipante.FimVigencia = Convert.ToDateTime(cli["new_data_final"]);

                if (cli.Attributes.Contains("new_name"))
                    clienteParticipante.Nome = Convert.ToString(cli["new_name"]);

                //Contrato
                if (cli.Attributes.Contains("new_contratoid"))
                    clienteParticipante.ContratoId = ((EntityReference)cli["new_contratoid"]).Id;

                if (cli.Attributes.Contains("new_clienteid"))
                    clienteParticipante.ClienteId = ((EntityReference)cli["new_clienteid"]).Id;

                if (cli.Attributes.Contains("new_descricao"))
                    clienteParticipante.Descricao = Convert.ToString(cli["new_descricao"]);

                if (cli.Attributes.Contains("new_codigo_cliente"))
                    clienteParticipante.CodigoCliente = Convert.ToString(cli["new_codigo_cliente"]);

                //Endereço
                if (cli.Attributes.Contains("new_contratoid"))
                    clienteParticipante.ContratoId = ((EntityReference)cli["new_contratoid"]).Id;

                if (cli.Attributes.Contains("new_codigoendereco"))
                    clienteParticipante.Nome = Convert.ToString(cli["new_codigoendereco"]);

                if (cli.Attributes.Contains("new_cliente_participanteid"))
                    clienteParticipante.ClienteParticipanteId = ((EntityReference)cli["new_cliente_participanteid"]).Id;


                if (cli.Attributes.Contains("new_enderecoid"))
                    clienteParticipante.EnderecoId = ((EntityReference)cli["new_enderecoid"]).Id;

                if (cli.Attributes.Contains("new_localidadeid"))
                {
                    clienteParticipante.CodigoDaLocalidade = ((EntityReference)cli["new_localidadeid"]).Id;
                    clienteParticipante.Localidade = ((EntityReference)cli["new_localidadeid"]).Name;
                }

                if (cli.Attributes.Contains("new_cep"))
                    clienteParticipante.Cep = Convert.ToString(cli["new_cep"]);

                if (cli.Attributes.Contains("new_cidad"))
                    clienteParticipante.Cidade = Convert.ToString(cli["new_cidade"]);

                if (cli.Attributes.Contains("new_rua"))
                    clienteParticipante.Logradouro = Convert.ToString(cli["new_rua"]);

                if (cli.Attributes.Contains("new_uf"))
                    clienteParticipante.Uf = Convert.ToString(cli["new_uf"]);

                if (cli.Attributes.Contains("new_bairro"))
                    clienteParticipante.Bairro = Convert.ToString(cli["new_bairro"]);


            }
            return clienteParticipante;
        }

        public ClienteParticipante ObterPor(Guid clienteParticipanteContratoId)
        {
            ClienteParticipanteDoContrato cli = (new Domain.Servicos.RepositoryService()).ClienteParticipanteDoContrato.Retrieve(clienteParticipanteContratoId);
            ClienteParticipante clienteParticipante = InstanciarClienteParticipante(cli);
            return clienteParticipante;
        }

        public ClienteParticipante ObterPorEnderecoId(Guid clienteParticipanteEnderecoId)
        {
            ClienteParticipanteEndereco cli = (new Domain.Servicos.RepositoryService()).ClienteParticipanteDoEndereco.Retrieve(clienteParticipanteEnderecoId);
            ClienteParticipante clienteParticipante = InstanciarClienteParticipante(cli);
            return clienteParticipante;
        }
       
        public List<ClienteParticipante> ListarPor(Guid clienteid, Guid enderecoid)
        {
            var lstClientesParticipantes = new List<ClienteParticipante>();

            var queryHelper = new QueryExpression("new_cliente_participante_endereco");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_clienteid", ConditionOperator.Equal, clienteid));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_enderecoid", ConditionOperator.Equal, enderecoid));

            var bec = base.Provider.RetrieveMultiple(queryHelper);

            if (bec.Entities.Count > 0)
                foreach (var item in bec.Entities)
                    lstClientesParticipantes.Add(InstanciarClienteParticipante(item));


            return lstClientesParticipantes;
        }

        public List<ClienteParticipante> ListarPor(Guid clienteid)
        {
            List<ClienteParticipante> lstClientesParticipantes = new List<ClienteParticipante>();

            var queryHelper = new QueryExpression("new_cliente_participante_endereco");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_clienteid", ConditionOperator.Equal, clienteid));

            var bec = base.Provider.RetrieveMultiple(queryHelper);

            if (bec.Entities.Count > 0)
                foreach (var item in bec.Entities)
                    lstClientesParticipantes.Add(InstanciarClienteParticipante(item));


            return lstClientesParticipantes;
        }

        public List<ClienteParticipante> ListarPor(Domain.Model.Conta cliente)
        {
            return this.ListarPor(cliente.Id);
        }

        public ClienteParticipante ObterPor(Guid clienteId, Guid contratoId)
        {
            ClienteParticipante ClienteParticipante = null;

            var queryHelper = new QueryExpression("new_cliente_participante_contrato");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_clienteid", ConditionOperator.Equal, clienteId));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_contratoid", ConditionOperator.Equal, contratoId));

            var bec = base.Provider.RetrieveMultiple(queryHelper);

            if (bec.Entities.Count > 0)
                ClienteParticipante = InstanciarClienteParticipante(bec.Entities[0]);


            return ClienteParticipante;
        }

        public List<ClienteParticipante> ListarPor(Contrato contrato)
        {
            List<ClienteParticipante> lstClientesParticipantes = new List<ClienteParticipante>();

            var queryHelper = new QueryExpression("new_cliente_participante_contrato");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));

            var bec = base.Provider.RetrieveMultiple(queryHelper);

            if (bec.Entities.Count > 0)
                foreach (var item in bec.Entities)
                    lstClientesParticipantes.Add(InstanciarClienteParticipante(item));


            return lstClientesParticipantes;
        }

    }
}
