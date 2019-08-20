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
    public class RepClientePotencial<T> : CrmServiceRepository<T>, IClientePotencial<T>
    {
        public T ObterPorEmail(string email)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("emailaddress1", ConditionOperator.Equal, email));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPorNumeroProjeto(string numeroprojeto)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_numeroprojeto", ConditionOperator.Equal, numeroprojeto));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }
        public List<T> ListarPor(Revenda revenda)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_revendaid", ConditionOperator.Equal, revenda.Revendaid));
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, 0));
            query.Orders.Add(new OrderExpression("subject", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarProjetosPor(String CodigoRevenda, String CodigoDistribuidor, String CodigoExecutivo, String CNPJCliente, int? SituacaoProjeto, string CodigoSegmento, string CodigoUnidadeNegocio)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("itbc_businessunit", ConditionOperator.NotNull);
            query.Criteria.AddCondition("itbc_numeroprojeto", ConditionOperator.NotNull);
            query.Criteria.AddCondition("itbc_cpfoucnpj", ConditionOperator.NotNull);

            if (!string.IsNullOrEmpty(CodigoRevenda) && !string.IsNullOrWhiteSpace(CodigoRevenda))
            {
                query.Criteria.AddCondition("itbc_revendaintegrid", ConditionOperator.Equal, new Guid(CodigoRevenda));
            }

            if (!string.IsNullOrEmpty(CodigoDistribuidor) && !string.IsNullOrWhiteSpace(CodigoDistribuidor))
            {
                query.Criteria.AddCondition("itbc_distribuidorid", ConditionOperator.Equal, CodigoDistribuidor);
            }

            if (!string.IsNullOrEmpty(CodigoExecutivo) && !string.IsNullOrWhiteSpace(CodigoExecutivo))
            {
                query.Criteria.AddCondition("itbc_keyaccountreprdistrid", ConditionOperator.Equal, CodigoExecutivo);
            }

            if (!string.IsNullOrEmpty(CNPJCliente) && !string.IsNullOrWhiteSpace(CNPJCliente))
            {
                CNPJCliente = CNPJCliente.Substring(0, 10);
                query.Criteria.AddCondition("itbc_cpfoucnpj", ConditionOperator.BeginsWith, CNPJCliente);
            }

            if(!string.IsNullOrEmpty(CodigoSegmento) && !string.IsNullOrWhiteSpace(CodigoSegmento))
            {
                query.AddLink("itbc_produto_do_cliente_potencial", "leadid", "itbc_cliente_potencial")
                    .AddLink("product", "itbc_produto", "productid")
                    .AddLink("itbc_segmento", "itbc_segmentoid", "itbc_segmentoid")
                    .LinkCriteria.AddCondition("itbc_codigo_segmento", ConditionOperator.Equal, CodigoSegmento);
            }

            if(!string.IsNullOrEmpty(CodigoUnidadeNegocio) && !string.IsNullOrWhiteSpace(CodigoUnidadeNegocio))
            {
                query.AddLink("itbc_produto_do_cliente_potencial", "leadid", "itbc_cliente_potencial")
                    .AddLink("product", "itbc_produto", "productid")
                    .AddLink("itbc_businessunitid", "itbc_businessunitid",  "businessunitid")
                    .LinkCriteria.AddCondition("itbc_chave_integracao", ConditionOperator.Equal, CodigoUnidadeNegocio);
            }

            if (SituacaoProjeto.HasValue)
            {
                if (SituacaoProjeto == 993520010)
                {
                    LinkEntity link = query.AddLink("opportunity", "leadid", "originatingleadid", JoinOperator.Inner);
                    link.EntityAlias = "o";
                    query.Criteria.AddCondition("o", "statuscode", ConditionOperator.In, new String[] { SituacaoProjeto.ToString(), "1", "200011"});
                    query.Criteria.AddCondition("o", "stageid", ConditionOperator.Equal, "6E7E126A-A9A0-6CDE-74AC-1509ABA87087");
                }
                else
                {
                    var f = new FilterExpression();
                    f.FilterOperator = LogicalOperator.Or;
                    f.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, SituacaoProjeto));
                    if (SituacaoProjeto == 993520000)
                    {
                        f.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 4));
                        f.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 5));
                    }
                    else if (SituacaoProjeto == 993520002)
                    {
                        f.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 6));
                        f.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 7));
                    }
                    else if (SituacaoProjeto == 993520003)
                    {
                        f.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 1));
                    }
                    else if ((SituacaoProjeto == 993520005) || (SituacaoProjeto == 993520006))
                    {
                        f.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 2));
                    }
                    else if (SituacaoProjeto == 993520007)
                    {
                        f.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 3));
                    }
                    query.Criteria.Filters.Add(f);
                }
            }

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarProjetosDuplicidade(String CNPJCliente, String UnidadeNegocio)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("itbc_businessunit", ConditionOperator.NotNull);
            query.Criteria.AddCondition("itbc_numeroprojeto", ConditionOperator.NotNull);
            query.Criteria.AddCondition("itbc_cpfoucnpj", ConditionOperator.Equal, CNPJCliente);
            query.Criteria.AddCondition("itbc_businessunit", ConditionOperator.Equal, UnidadeNegocio);
                
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(String CNPJ)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, CNPJ));
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, 0));
            query.Orders.Add(new OrderExpression("subject", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public int ObterUltimoNumeroProjeto(string Ano)
        {
            string numero = string.Empty;
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.ColumnSet.AddColumn("itbc_numeroprojeto");
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_numeroprojeto", ConditionOperator.BeginsWith, DateTime.Now.Year.ToString()));
            query.Orders.Add(new OrderExpression("itbc_numeroprojeto", OrderType.Descending));

            var colecao = base.Provider.RetrieveMultiple(query);

            if (colecao.Entities.Count > 0 && colecao.Entities[0].Contains("itbc_numeroprojeto"))
                numero = Convert.ToString(colecao.Entities[0]["itbc_numeroprojeto"]).Substring(5,7);

            if (!string.IsNullOrEmpty(numero))
            {
                return Convert.ToInt32(numero);
            }else
            {
                return 0;
            }           
        }

        public void EnviaEmailRegistroProjeto(ClientePotencial clientepotencial, bool duplicado)
        {
            //Envia e-mail e salva como tarefa no CRM 
            try
            {
                Email email = new Email(OrganizationName, IsOffline);
                email.ReferenteAId = clientepotencial.Id;
                email.ReferenteAType = "lead";
                email.ReferenteAName = clientepotencial.NomeDaEmpresa;
                email.ReferenteA = new Lookup(clientepotencial.Id, clientepotencial.NomeDaEmpresa, "lead");
                email.Assunto = "Projeto "+ clientepotencial.NumeroProjeto +" enviado";
                if (duplicado)
                {
                    string corpoEmail = @"<style type=""text/css""> pre.mscrmpretag {  font-family: Tahoma, Verdana, Arial; style=""word-wrap: break-word;"" }</style>
                    <FONT size=2 face=Calibri>Prezado(a), <br /><br />
                    No sistema já há um registro para esse cliente final, sua solicitação será avaliada e em breve terá um retorno.
                    <br /><br />Pedimos que aguarde 48hs para receber o parecer se o projeto foi aceito ou não.
                    <br /><br />Registro de Projetos<br /><br />Soluções e Projetos<br /><br />intelbras.com.br</FONT>";
                    email.Mensagem = corpoEmail;
                }else
                {
                    string corpoEmail = @"<style type=""text/css""> pre.mscrmpretag {  font-family: Tahoma, Verdana, Arial; style=""word-wrap: break-word;"" }</style>
                    <FONT size=2 face=Calibri>Prezado(a), <br /><br />
                    Sua solicitação foi enviada com sucesso para área de Registro de Projetos.
                    <br /><br />Pedimos que aguarde 48hs para receber o parecer se o projeto foi aceito ou não.
                    <br /><br />Registro de Projetos<br /><br />Soluções e Projetos<br /><br />intelbras.com.br</FONT>";
                    email.Mensagem = corpoEmail;
                }
                email.De = new Lookup[] { new Lookup(new Guid(SDKore.Configuration.ConfigurationManager.GetSettingValue("ID_EMAIL_CORPORATIVO")), "systemuser") };

                if (duplicado)
                {
                    email.Para = new Lookup[4];
                    email.Para[0] = new Lookup { Id = clientepotencial.RevendaIntegrador.Id, Type = "account" };
                    email.Para[1] = new Lookup { Id = clientepotencial.Distribuidor.Id, Type = "account" };
                    email.Para[2] = new Lookup { Id = clientepotencial.Executivo.Id, Type = "contact" };
                    Usuario proprietario = new CRM2013.Domain.Servicos.RepositoryService().Usuario.BuscarProprietario("lead", "leadid", clientepotencial.Id);
                    if (proprietario != null)
                    {
                        email.Para[3] = new Lookup { Id = proprietario.Id, Type = "systemuser" };
                    }
                }
                else
                {
                    email.Para = new Lookup[3];
                    email.Para[0] = new Lookup { Id = clientepotencial.RevendaIntegrador.Id, Type = "account" };
                    email.Para[1] = new Lookup { Id = clientepotencial.Distribuidor.Id, Type = "account" };
                    email.Para[2] = new Lookup { Id = clientepotencial.Executivo.Id, Type = "contact" };
                }


                email.Direcao = false;
                email.ID = (new CRM2013.Domain.Servicos.RepositoryService()).Email.Create(email);

                (new CRM2013.Domain.Servicos.RepositoryService()).Email.EnviarEmail(email.ID.Value);

            }
            catch (Exception ex)
            {
                SDKore.Helper.Log.Logar("emailprojetoduplicado.txt", ex.Message + ex.StackTrace);
            }
        }
    }
}