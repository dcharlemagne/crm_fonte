using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using SDKore.Configuration;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    class TestesCrmWebServices : Base
    {
        int counter = 0;
        [Test]
        public void TesteProdutoPortfolioPlanilha()
        {
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
            string nomeEmpresa = @"nome empresa $%¨@:\/";

            var invalids = System.IO.Path.GetInvalidFileNameChars();
            nomeEmpresa = String.Join("_", nomeEmpresa.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');

            string data = "data1,data2,data3,data4" + Environment.NewLine + "data5,data6";

            if (string.IsNullOrEmpty(nomeEmpresa))
                nomeEmpresa = "";

            string direcotoryPath = @"C:\TrideaByAlfa\planilhas/";
            string filePath = direcotoryPath + nomeEmpresa + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss") + ".csv";


            if (!System.IO.Directory.Exists(direcotoryPath))
                System.IO.Directory.CreateDirectory(direcotoryPath);

            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.Create(filePath).Close();
            }
            System.IO.File.AppendAllText(filePath, data);

            
        }
        [Test]
        public void TestesListarArquivos()
        {
            string resposta = string.Empty;
            List<Domain.Model.ArquivoDeSellOut> lstArquivoSellout = new Intelbras.CRM2013.Domain.Servicos.ArquivoDeSellOutServices(ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false).ListarPor(null, (int)Domain.Enum.ArquivoSellOut.RazaoStatus.NaoProcessado, null, null);

            if (lstArquivoSellout.Count <= 0)
                throw new ArgumentException("Não existem registros na fila para serem processados");

            var xmlroot = new XDocument(
                        new XDeclaration("1.0", "utf-8", "no"),
                        new XElement("ListaArquivosSellout",
                        from arquivo in lstArquivoSellout
                        select new XElement("ArquivoSellout",
                            new XElement("Id", arquivo.ID.Value.ToString(),
                            new XElement("Conta", arquivo.Conta.Id.ToString())
                            ))));
        }
        [Test]
        public void TestesProdutosPortfolio()
        {
            JavaScriptSerializer jsonConverter = new JavaScriptSerializer();
            Dictionary<string, object> dictResposta = new Dictionary<string, object>();
            Dictionary<string, object> listResposta = new Dictionary<string, object>();
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Domain.Servicos.ProdutoService prodServ = new Domain.Servicos.ProdutoService(organizationName, false);

            string canalGuid = "9484CD60-D400-E411-9420-00155D013D39";
            try
            {
                Guid canal = Guid.Parse(canalGuid);
                Domain.Model.Conta contaCliente = new Domain.Servicos.ContaService(organizationName, false).BuscaConta(canal);
                if (contaCliente == null)
                    throw new ArgumentException("Canal não encontrado");

                List<Domain.Model.ProdutoPortfolio> prodPortfolio = prodServ.ProdutosPortfolio(contaCliente, null, null);

                if (prodPortfolio.Count <= 0)
                    throw new ArgumentException("Canal não possui produtos cadastrados no portfólio");

                foreach (var item in prodPortfolio)
                {
                    counter++;
                    if (item.Product != null && item.Product.Showroom != null && item.Product.Showroom == true)
                    {
                        if (!string.IsNullOrEmpty(item.Product.Nome) && item.Product.ID != null)
                            if(!listResposta.ContainsKey(item.Product.ID.ToString()))
                                listResposta.Add(item.Product.ID.ToString(), item.Product.Nome.ToString());
                    }
                }
                dictResposta.Add("Produtos", listResposta);
                dictResposta.Add("Sucesso", true);
            }
            catch (FormatException)
            {
                dictResposta.Add("Sucesso", false);
                dictResposta.Add("Mensagem", "Guid em formato incorreto!Esperado : (xxxxxxxx-xxxx-xxxxx-xxxx-xxxxxxxxxxxx)");
            }
            catch (Exception e)
            {
                dictResposta.Add("Sucesso", false);
                dictResposta.Add("Mensagem", e.Message);
            }
            string retorno = jsonConverter.Serialize(dictResposta);
        }
    }
}
