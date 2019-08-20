using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Xml.Linq;
using Intelbras.Message.Helper;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using System.ServiceModel;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Sellout;
//using SDKore.Configuration;
//using Intelbras.CRM2013.Application.Barramento;



namespace Intelbras.CRM2013.Domain.Servicos
{
    public class SellOutService
    {
        public string Organizacao { get; set; }
        public bool IsOffline { get; set; }
        private BasicHttpBinding myBinding { get; set; }
        private EndpointAddress endPointAddress { get; set; }
        private Sellout.SellOutCRMWS ConexaoWSSellOut { get; set; }


        public SellOutService(string org, bool isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));

            this.ConexaoWSSellOut = new Sellout.SellOutCRMWS();
            this.ConexaoWSSellOut.Url = SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutURL");

            //this.ConexaoWSSellOut.Url = System.Configuration.ConfigurationManager.AppSettings["Sellout.CaminhoWS"];

            //throw new Exception(this.ConexaoWSSellOut.Url + "-" + SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutURL"));

            //this.myBinding = new BasicHttpBinding();
            //this.myBinding.Name = "SelloutWSSoap";
            
            
            //////Dev
            //this.endPointAddress = new EndpointAddress("http://sjo-crm-dev-01:5554/ISV/sellout/SelloutWS.asmx");
            //this.myBinding.Security.Mode = BasicHttpSecurityMode.None;
            //this.myBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            //this.myBinding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;

            ////Homolog
            //this.endPointAddress = new EndpointAddress("https://crm2013h.intelbras.com.br:5554/ISV/wssellout/selloutws.asmx");
            ////this.myBinding.Security.Mode = SecurityMode.Transport;
            //this.myBinding.Security.Mode = BasicHttpSecurityMode.Transport;
            //this.myBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;

            ////localhost.ServiceContractClient a = new ServiceContractClient

            //this.myBinding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Windows;
            
            ////Prod
            //this.endPointAddress = new EndpointAddress("https://crm2013.intelbras.com.br/ISV/wssellout/selloutws.asmx");
            //this.myBinding.Security.Mode = BasicHttpSecurityMode.Transport;
            //this.myBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            

        }
        public bool PersistirProdutoSellOut(string usuario, string senha, string xml, out string resposta)
        {
            bool response = this.ConexaoWSSellOut.PersistirProduto(usuario, senha, xml, out resposta);

            return response;
        }
        public bool PersistirRevenda(string usuario, string senha, string xml, out string resposta)
        {
            bool response = this.ConexaoWSSellOut.PersistirRevenda(usuario, senha, xml, out resposta);

            return response;
        }
        public bool PersistirDistribuidor(string usuario, string senha, string xml, out string resposta)
        {
            bool response = this.ConexaoWSSellOut.PersistirDistribuidor(usuario, senha, xml, out resposta);

            return response;
        }
        public bool MudarStatusDistribuidor(string usuario, string senha, string xml, out string resposta)
        {
            bool response = this.ConexaoWSSellOut.MudarStatusDistribuidor(usuario, senha, xml, out resposta);

            return response;
        }
        public bool MudarStatusRevenda(string usuario, string senha, string xml, out string resposta)
        {
            bool response = this.ConexaoWSSellOut.MudarStatusRevenda(usuario, senha, xml, out resposta);

            return response;
        }
        public bool MudarStatusProduto(string usuario, string senha, string xml, out string resposta)
        {
            bool response = this.ConexaoWSSellOut.MudarStatusProduto(usuario, senha, xml, out resposta);

            return response;
        }
        public List<QtdProdutoSellout> listarContagemVenda(DateTime dataIni, DateTime dataFim, Guid distGuid, string listaProdIds)
        {
            QtdProdutoSellout[] response = this.ConexaoWSSellOut.listarContagemVenda(dataIni, dataFim, distGuid, listaProdIds);

            return response.OfType<QtdProdutoSellout>().ToList();
        }
        public int CargaSellOutContas()
        {
            int pagina = 1;
            int contagem = 10;
            bool moreRecords = true;
            int registrosAtualizados = 0;
            string usuario = SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutCRMWSUser");
            string senha = SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutCRMWSPasswd");

            while (moreRecords)
            {
                List<Model.Conta> lstConta = new Intelbras.CRM2013.Domain.Servicos.ContaService(this.Organizacao, this.IsOffline).ListarContas(ref pagina, contagem, out moreRecords);

                foreach (var item in lstConta)
                {
                    string xml;
                    XDocument xmlroot;
                    bool resultado;
                    string resposta;
                    if (!string.IsNullOrEmpty(item.CodigoMatriz) && item.Status.HasValue && item.Status.HasValue && item.ID.HasValue && !String.IsNullOrEmpty(item.CpfCnpj))
                    {
                        string cpfCnpj = item.CpfCnpj;

                        switch (item.Classificacao.Name)
                        {
                            case Domain.Enum.Conta.Classificacao.Dist_BoxMover:
                            case Domain.Enum.Conta.Classificacao.Dist_VAD:
                                if (!Intelbras.CRM2013.Domain.Servicos.Helper.somenteNumeros(item.CpfCnpj))
                                    cpfCnpj = Intelbras.CRM2013.Domain.Servicos.Helper.FormatarCnpj(item.CpfCnpj);

                                xmlroot = new XDocument(
                                new XDeclaration("1.0", "utf-8", "no"),
                                new XElement("Distribuidor",
                                    new XElement("Iddistribuidorcrm", item.ID),
                                    new XElement("Iddistribuidorerp", item.CodigoMatriz),
                                    new XElement("Cnpj", cpfCnpj),
                                    new XElement("Statuscode", item.RazaoStatus),
                                    new XElement("Statecode", item.Status)
                                    ));
                                xml = xmlroot.Declaration.ToString() + Environment.NewLine + xmlroot.ToString(); ;

                                resultado = new Domain.Servicos.SellOutService(this.Organizacao,
                                this.IsOffline).PersistirDistribuidor(usuario, senha, xml, out resposta);
                                if (resultado == false)
                                {
                                    throw new ArgumentException(resposta);
                                }
                                registrosAtualizados++;
                                break;
                            case Domain.Enum.Conta.Classificacao.Rev_Rel:
                            case Domain.Enum.Conta.Classificacao.Rev_Trans:
                                if (!Intelbras.CRM2013.Domain.Servicos.Helper.somenteNumeros(item.CpfCnpj))
                                    cpfCnpj = Intelbras.CRM2013.Domain.Servicos.Helper.FormatarCnpj(item.CpfCnpj);

                                xmlroot = new XDocument(
                                new XDeclaration("1.0", "utf-8", "no"),
                                new XElement("Revenda",
                                    new XElement("Idrevendacrm", item.ID),
                                    new XElement("Idrevendaerp", item.CodigoMatriz),
                                    new XElement("Cpfcnpj", cpfCnpj),
                                    new XElement("Statuscode", item.RazaoStatus),
                                    new XElement("Statecode", item.Status)
                                    ));

                                xml = xmlroot.Declaration.ToString() + Environment.NewLine + xmlroot.ToString(); ;

                                resultado = new Domain.Servicos.SellOutService(this.Organizacao,
                                this.IsOffline).PersistirRevenda(usuario, senha, xml, out resposta);

                                if (resultado == false)
                                {
                                    throw new ArgumentException(resposta);
                                }
                                registrosAtualizados++;
                                break;
                        }
                    }
                }
            }
            return registrosAtualizados;
        }

        public int CargaSellOutProdutos()
        {
            int pagina = 1;
            int contagem = 500;
            bool moreRecords = true;
            int registrosAtualizados = 0;
            string usuario = SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutCRMWSUser");
            string senha = SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutCRMWSPasswd");

            while (moreRecords)
            {
                try
                {
                    List<Model.Product> lstProduto = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(this.Organizacao, this.IsOffline).ListarTodosProdutos(ref pagina, contagem, out moreRecords);

                    foreach (var item in lstProduto)
                    {
                        string xml;
                        bool resultado;
                        string resposta;

                        XDocument xmlroot = new XDocument(
                        new XDeclaration("1.0", "utf-8", "no"),
                        new XElement("Produto",
                            new XElement("Idprodutocrm", item.ID),
                            new XElement("Idprodutoerp", item.Codigo),
                            new XElement("Statuscode", item.RazaoStatus),
                            new XElement("Statecode", item.Status)
                            ));

                        xml = xmlroot.Declaration.ToString() + Environment.NewLine + xmlroot.ToString(); ;

                        resultado = new Domain.Servicos.SellOutService(this.Organizacao,
                        this.IsOffline).PersistirProdutoSellOut(usuario, senha, xml, out resposta);

                        if (resultado == false)
                        {
                            throw new ArgumentException(resposta);
                        }
                        registrosAtualizados++;
                    }
                }
                catch (TimeoutException e)
                {
                    continue;
                }
            }
            return registrosAtualizados;
        }

        

    }
}
