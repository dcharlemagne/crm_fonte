using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using Intelbras.CRM2013.Domain;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.WebServices.Intelbras
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class Intelbras : WebServiceBase
    {
        [WebMethod]
        public Endereco PesquisarCep(string cep)
        {
            var cepService = new CepService(cep);
            return cepService.Pesquisar();
        }

        [WebMethod]
        public XmlDocument QuantidadeDeRegistros(string fetch)
        {
            try
            {
                FetchService f = new FetchService(fetch);
                int count = f.QuantidadeDeRegistrosNaConsulta();

                base.Mensageiro.AdicionarTopico("count", count);

                return base.Mensageiro.Mensagem;
            }
            catch (Exception ex)
            {
                //LogService.GravaLog(ex, TipoDeLog.WSIntelbras);
                throw ex;
            }
        }

        //[WebMethod]
        //public bool GerarFaleConosco(string nome, string email, string cidade, string uf, string telefone, string fila, string mensagem)
        //{
        //    try
        //    {
        //        Cadastro cadastro = new Cadastro(Organizacao);
        //        Referencia referencia = cadastro.ObterReferenciaPorEmail(email);

        //        if (referencia == null) referencia = new Contato(Organizacao).Converter(nome, email, cidade, uf, telefone);
        //        new Email(Organizacao).EnviarPor(referencia, "Fale Conosco", mensagem, fila);

        //        return true;
        //    }
        //    catch (SoapException ex)
        //    {
        //        LogService.GravaLog(ex, TipoDeLog.WSIntelbras);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.GravaLog(ex, TipoDeLog.WSIntelbras);
        //    }

        //    return false;
        //}

        //[WebMethod]
        //public bool GerarFaleConosco(XmlDocument xml, string cpf, string fila, string mensagem)
        //{
        //    if (String.IsNullOrEmpty(cpf)) return false;

        //    try
        //    {
        //        Cadastro cadastro = new Cadastro(Organizacao);
        //        Referencia referencia = null;
        //        Contato contato = null;

        //        contato = RepositoryService.Contato.ObterPor(cpf);
        //        if (contato != null) referencia = new Referencia() { Id = contato.Id, Nome = contato.Nome, Tipo = "contact" };

        //        if (referencia == null)
        //        {
        //            contato = RepositoryService.Contato.Create(xml);
        //            referencia = new Referencia() { Id = contato.Id, Nome = contato.Nome, Tipo = "contact" };
        //        }

        //        new Email(Organizacao).EnviarPor(referencia, "Fale Conosco", mensagem, fila);

        //        return true;
        //    }
        //    catch (SoapException ex)
        //    {
        //        LogService.GravaLog(ex, TipoDeLog.WSIntelbras);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.GravaLog(ex, TipoDeLog.WSIntelbras);
        //    }

        //    return false;
        //}

        //[WebMethod]
        //public bool QueroParticipar(XmlDocument xml)
        //{
        //    try
        //    {
        //        var repositoryContato = RepositoryFactory.GetRepository<IContatoRepository>();
        //        repositoryContato.Organizacao = Organizacao;

        //        Contato contato = repositoryContato.ObtemPor(xml.GetElementsByTagName("CPF") != null ? xml.GetElementsByTagName("CPF")[0].InnerText : string.Empty, xml.GetElementsByTagName("Email") != null ? xml.GetElementsByTagName("Email")[0].InnerText : String.Empty);
        //        if (contato == null) contato = repositoryContato.Create(xml);

        //        contato.QueroParticiparFidelidade();

        //        return true;
        //    }
        //    catch (SoapException ex)
        //    {
        //        LogService.GravaLog(ex, TipoDeLog.WSIntelbras);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.GravaLog(ex, TipoDeLog.WSIntelbras);
        //    }

        //    return false;
        //}

        [WebMethod]
        public void RetornoDoFaturamento(string xmlRetorno)
        {
            PedidoService pedidoService = new PedidoService(nomeOrganizacao, false);
            pedidoService.RetornoDoFaturamento(xmlRetorno);
        }
    }
}