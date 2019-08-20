using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk;
using SDKore.Configuration;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class CanalServices
    {
        #region Atributos

        private static bool _isOffline = false;
        public static bool IsOffline
        {
            get { return _isOffline; }
            set { _isOffline = value; }
        }

        private static string _nomeDaOrganizacao = "";
        public static string NomeDaOrganizacao
        {
            get
            {
                if (String.IsNullOrEmpty(_nomeDaOrganizacao))
                    _nomeDaOrganizacao = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");

                return _nomeDaOrganizacao;
            }
            set { _nomeDaOrganizacao = value; }
        }

        public static object Provider { get; set; }

        #endregion

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public CanalServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public CanalServices(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public CanalServices(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }
        #endregion

        public String PersistirDistribuidor(Conta conta, IPluginExecutionContext context)
        {
            string cpfCnpj = string.Empty;
            string usuario = SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutCRMWSUser");
            string senha = SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutCRMWSPasswd");
            XDocument xmlroot;
            string xml;
            string resposta;
            Boolean resultado;

            if (!Domain.Servicos.Helper.somenteNumeros(conta.CpfCnpj))
                cpfCnpj = Intelbras.CRM2013.Domain.Servicos.Helper.FormatarCnpj(conta.CpfCnpj);

            xmlroot = new XDocument(
            new XDeclaration("1.0", "utf-8", "no"),
            new XElement("Distribuidor",
                new XElement("Iddistribuidorcrm", conta.ID),
                new XElement("Iddistribuidorerp", conta.CodigoMatriz),
                new XElement("Cnpj", cpfCnpj),
                new XElement("Statuscode", conta.RazaoStatus),
                new XElement("Statecode", conta.Status),
                new XElement("EnvioSelloutEstoque", conta.EnvioSelloutEstoque),
                new XElement("Classificacao", conta.Classificacao.Id.ToString()),
                new XElement("Subclassificacao", conta.Subclassificacao.Id.ToString()),
                new XElement("Categoria", conta.Categoria.Id.ToString())
                ));
            xml = xmlroot.Declaration.ToString() + Environment.NewLine + xmlroot.ToString(); ;

            resultado =  new Domain.Servicos.SellOutService(context.OrganizationName,
            context.IsExecutingOffline).PersistirDistribuidor(usuario, senha, xml, out resposta);

            if (resultado)
            {
                return String.Empty;
            }
            else
            {
                return resposta;
            }
        }

        public void AtribuiPedidoAEquipeDoCanal(Guid pedidoId, Guid usuarioId)
        {
            //Consulta o pedido para obter qual é o canal do Pedido.
            var salesorder = RepositoryService.Pedido.ObterPor(pedidoId);

            //Consulta a equipe que é proprietário do canal.
            if (salesorder.CanalVendaID == null || salesorder.CanalVendaID.Id == null)
                throw new ArgumentException("(CRM) O Canal de Venda deve ser informado.");

            if (salesorder.ClienteID == null)
                throw new ArgumentException("(CRM) O Cliente Provável deve ser informado.");

            //chama a DAL de Pedidos para atribuir o pedido a equipe proprietária do canal
            RepositoryService.Pedido.AtribuirEquipeParaPedido(usuarioId, salesorder.ID.Value);
        }

        public Conta ObterCanalPorId(Guid contaId)
        {
            return this.RepositoryService.Conta.Retrieve(contaId);
        }
    }
}