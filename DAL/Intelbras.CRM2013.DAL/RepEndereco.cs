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

namespace Intelbras.CRM2013.DAL
{
    public class RepEndereco<T> : CrmServiceRepository<T>, IEndereco<T>
    {
        public T ObterPor(ClienteParticipanteEndereco clienteParticipanteEndereco)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.TopCount = 1;
            queryHelper.AddLink("new_cliente_participante_endereco", "customeraddressid", "new_enderecoid");
            queryHelper.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_cliente_participante_enderecoid", ConditionOperator.Equal, clienteParticipanteEndereco.Id));
            var colecao = this.RetrieveMultiple(queryHelper);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }
        public List<T> ListarPor(int stateCode)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, stateCode);

            #endregion

            #region Validações
            query.Criteria.Conditions.Add(cond1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(ClienteParticipanteDoContrato clienteParticipanteDoContrato)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.AddLink("new_cliente_participante_endereco", "customeraddressid", "new_enderecoid");
            queryHelper.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_cliente_participanteid", ConditionOperator.Equal, clienteParticipanteDoContrato.Id));
            return (List<T>)this.RetrieveMultiple(queryHelper).List;
        }

        public T ObterPor(string codigo, Domain.Model.Conta cliente = null)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.TopCount = 1;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("name", ConditionOperator.Equal, codigo));
            if (cliente != null && cliente.Id != Guid.Empty)
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("parentid", ConditionOperator.Equal, cliente.Id));
            var colecao = this.RetrieveMultiple(queryHelper);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<Endereco> ObterTodosOsEnderecosPor(Domain.Model.Conta cliente)
        {
            List<Endereco> listaEnderecos = new List<Endereco>();
            var queryHelper = new QueryExpression("customeraddress");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("parentid", ConditionOperator.Equal, cliente.Id));

            var bec = base.Provider.RetrieveMultiple(queryHelper);
            if (bec.Entities.Count > 0)
            {
                foreach (Entity item in bec.Entities)
                {
                    Endereco endereco = new Endereco(this.OrganizationName, this.IsOffline);
                    endereco.Id = item.Id;
                    if(item.Contains("line1"))
                        endereco.Logradouro = Convert.ToString(item["line1"]);

                    if (item.Contains("line2"))
                        endereco.Bairro = Convert.ToString(item["line2"]);

                    if (item.Contains("line3"))
                        endereco.Complemento = Convert.ToString(item["line3"]);

                    if (item.Contains("name"))
                        endereco.Nome = Convert.ToString(item["name"]);

                    if (item.Contains("city"))
                        endereco.Cidade = Convert.ToString(item["city"]);

                    if (item.Contains("postalcode"))
                        endereco.Cep = Convert.ToString(item["postalcode"]);

                    if (item.Contains("new_numero_endereco"))
                        endereco.Numero = Convert.ToString(item["new_numero_endereco"]);

                    if (item.Contains("country"))
                        endereco.Pais = Convert.ToString(item["country"]);

                    if (item.Contains("addressnumber"))
                        endereco.AddressNumber = Convert.ToInt16(item["addressnumber"]);

                    listaEnderecos.Add(endereco);
                }
            }
            return listaEnderecos;
        }

        public Endereco PesquisarEnderecoPor(string cep)
        {
            if (string.IsNullOrEmpty(cep)) return null;

            Endereco endereco = null;
            var row = new Buscar_DadosCep_ttRetornoCEPRow[1];

            Domain.Servicos.HelperWS.IntelbrasService.Buscar_DadosCep(cep.Replace("-", ""), out row);

            if (null != row && row.Length > 0)
                endereco = InstanciarEndereco(row[0], cep);

            return endereco;
        }

        internal Endereco InstanciarEndereco(Buscar_DadosCep_ttRetornoCEPRow row, string cep)
        {
            Endereco endereco = null;

            if (null != row)
            {
                if (!string.IsNullOrEmpty(row.Endereco) &&
                   !string.IsNullOrEmpty(row.Bairro) &&
                   !string.IsNullOrEmpty(row.Cidade) &&
                   !string.IsNullOrEmpty(row.UF) &&
                    row.ibge != null)
                {
                    endereco = new Endereco(this.OrganizationName, this.IsOffline);
                    endereco.Logradouro = row.Endereco.Trim();
                    endereco.Bairro = row.Bairro.Trim();
                    endereco.Cidade = row.Cidade.Trim();
                    endereco.Uf = row.UF.Trim();
                    endereco.Cep = cep.Trim();
                    endereco.CodigoIBGE = row.ibge.Value;

                    if (row.CidadeZF.HasValue)
                        endereco.ZonaFranca = row.CidadeZF.HasValue;
                }
            }

            return endereco;
        }

        public T ObterPor(Guid conta, string codigoEntrega)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("name", ConditionOperator.Equal, codigoEntrega);
            query.Criteria.AddCondition("parentid", ConditionOperator.Equal, conta);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPorEnderecoId(string enderecoid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("customeraddressid", ConditionOperator.Equal, enderecoid);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}
