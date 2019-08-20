using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Intelbras.CRM2013.Domain;
using System.Web.UI.HtmlControls;
using SDKore.Configuration;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Intelbras.CRM2013.UI.Web
{
    public partial class BuscaProdutosPortfolio : System.Web.UI.Page
    {
        public Guid estadoID;
        public Guid? politicaComercialComboBox = null;
        public Domain.Model.Conta canal;
        private string organizationName = string.Empty;
        System.Diagnostics.Stopwatch w = System.Diagnostics.Stopwatch.StartNew();

        public string OrganizationName
        {
            get
            {
                if (string.IsNullOrEmpty(organizationName))
                    organizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
                return organizationName;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
            NomeOrganizacao.Value = OrganizationName;
            Guid canalId = new Guid();

            //variavel usada para testes
            //canalId = new Guid("f7e8e1af-d500-e411-9420-00155d013d39");

            if (Request["id"] != string.Empty)
                canalId = new Guid(Request["id"].ToString());

            if (canalId != null)
            {
                canal = new Domain.Servicos.ContaService(this.OrganizationName, false).BuscaConta((canalId));

                if (canal == null)
                {
                    mensagemRetorno.Text = "Canal não encontrado";
                }
                else if (canal.ParticipantePrograma.Value.CompareTo(((int)Domain.Enum.Conta.ParticipaDoPrograma.Sim)) != 0)
                {
                    mensagemRetorno.Text = "Canal / Conta não participante do Programa de Canais portanto sem configuração de portfólio";
                    nomeCliente.Text = canal.RazaoSocial;
                    CodigoCliente.Text = canal.CodigoMatriz;
                }
                else
                {//canal participante do programa
                    GuidCliente.Value = canal.ID.ToString();
                    nomeCliente.Text = canal.RazaoSocial;
                    CodigoCliente.Text = canal.CodigoMatriz;
                }
            }
        }

        [WebMethod]
        public static string GerarNovaTabela(string organizationName, string guidCliente, bool gerarPSD, bool gerarPP, bool gerarPSCF)
        {
            Guid cliente;
            if (!Guid.TryParse(guidCliente, out cliente))
                throw new ArgumentException("Guid cliente em formato incorreto");

            Domain.TabelaPrecoExtranet.Resposta resposta = new Intelbras.CRM2013.Domain.Servicos.TabelaPrecoExtranetService(organizationName, false).IncluirPlanilhaTabelaPreco(cliente, gerarPSD, gerarPP, gerarPSCF);
            
            var jsSerializer = new JavaScriptSerializer();
            jsSerializer.MaxJsonLength = Int32.MaxValue;

            var json = jsSerializer.Serialize(resposta);

            return json.ToString();
        }

        [WebMethod]
        public static string ListarTabelas(string organizationName, string guidCliente)
        {
            try
            {
                Guid cliente;
                if (!Guid.TryParse(guidCliente, out cliente))
                    throw new ArgumentException("ERRO | Guid cliente em formato incorreto");

                Domain.TabelaPrecoExtranet.TabelaPrecoPlanilhaResposta resposta = new Intelbras.CRM2013.Domain.Servicos.TabelaPrecoExtranetService(organizationName, false).ListarPlanilhasTabelaPreco(cliente);
                if (!resposta.resultado.HasValue || resposta.resultado.Value == false)
                    throw new ArgumentException("ERRO | Nenhum resultado encontrado");

                StringBuilder sbResposta = new StringBuilder();

                sbResposta.Append(@"{""resposta"":[");

                foreach (var item in resposta.planilhas.Any)
                {
                    Regex regex = new Regex("(?s)<id[^>]*>((?:(?!</id>).)*)</id>");
                    var r = regex.Match(item.InnerXml);
                    string id = r.Groups[1].ToString();

                    regex = new Regex("(?s)<dataCriacao[^>]*>((?:(?!</dataCriacao>).)*)</dataCriacao>");
                    r = regex.Match(item.InnerXml);
                    string data = r.Groups[1].ToString();

                    regex = new Regex("(?s)<url[^>]*>((?:(?!</url>).)*)</url>");
                    r = regex.Match(item.InnerXml);
                    string url = r.Groups[1].ToString();

                    regex = new Regex("(?s)<status[^>]*>((?:(?!</status>).)*)</status>");
                    r = regex.Match(item.InnerXml);
                    string status = r.Groups[1].ToString();

                    regex = new Regex("(?s)<descricaoErro[^>]*>((?:(?!</descricaoErro>).)*)</descricaoErro>");
                    r = regex.Match(item.InnerXml);
                    string descricaoErro = r.Groups[1].ToString();

                    string linha = @"{""status"":""" + status + @""",""data"":"""+data+@""",""mensagem"":"""+ descricaoErro + @""",""url"":"""+url+@""",""id"":"+id+"},";
                    sbResposta.Append(linha);
                }
                //removemos o ultimo caractere pois é a virgula
                sbResposta.Length--;

                sbResposta.Append("]}");
                
                return sbResposta.ToString();
            }
            catch (Exception ex)
            {
                return SDKore.Helper.Error.Handler(ex);
            }
        }

    }
}
