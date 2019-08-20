using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;
using SDKore.Configuration;
using Microsoft.SharePoint.ApplicationPages;
using Microsoft.SharePoint.Client;
using System.Net;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0269 : Base, IBase<Message.Helper.MSG0269, Domain.Model.ClientePotencial>
    {
        #region Construtor

        public MSG0269(string org, bool isOffline) : base(org, isOffline)
        {
            Organizacao = org;
            IsOffline = isOffline;
        }

        #endregion

        #region Propriedades

        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        string usuarioSharePoint = SDKore.Helper.Cryptography.Decrypt(ConfigurationManager.GetSettingValue("UsuarioSharePoint"));
        string senhaSharePoint = SDKore.Helper.Cryptography.Decrypt(ConfigurationManager.GetSettingValue("SenhaSharePoint"));
        string domain = ConfigurationManager.GetDomain(ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));

        #endregion

        #region trace

        private SDKore.Helper.Trace Trace { get; set; }
        public void DefinirObjetoTrace(SDKore.Helper.Trace trace)
        {
            this.Trace = trace;
        }

        #endregion

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            try
            {
                usuarioIntegracao = usuario;
                ClientePotencial clientePotencialConsulta = null;
                Oportunidade oportunidadeConsulta = null;

                var xml = this.CarregarMensagem<Pollux.MSG0269>(mensagem);
                Pollux.Entities.ProjetoItem projetoItem = new Pollux.Entities.ProjetoItem();

                if (!string.IsNullOrEmpty(xml.NumeroProjeto)){
                    clientePotencialConsulta = new Servicos.LeadService(this.Organizacao, this.IsOffline).ObterPorNumeroProjeto(xml.NumeroProjeto);
                    if (clientePotencialConsulta != null)
                    {
                        oportunidadeConsulta = new Servicos.RepositoryService().Oportunidade.BuscarPor(clientePotencialConsulta);
                    }
                    
                    if (clientePotencialConsulta == null)
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Valor do parâmetro " + xml.NumeroProjeto + " não encontrado.";
                        retorno.Add("Resultado", resultadoPersistencia);
                        return CriarMensagemRetorno<Pollux.MSG0269R1>(numeroMensagem, retorno);
                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Parâmetro obrigatório para a consulta não enviado.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0269R1>(numeroMensagem, retorno);
                }

                if (oportunidadeConsulta != null)
                {
                    projetoItem.NumeroProjeto = clientePotencialConsulta.NumeroProjeto;
                    projetoItem.ClassificacaoProjeto = 993520001;
                    projetoItem.CodigoClientePotencial = oportunidadeConsulta.ClientePotencialOriginador.Id.ToString();
                    projetoItem.CodigoOportunidade = oportunidadeConsulta.Id.ToString();
                    projetoItem.SituacaoProjeto = ObterRazaoStatusOportunidade(oportunidadeConsulta);
                    if (oportunidadeConsulta.RevendaIntegrador != null)
                    {
                        projetoItem.CodigoRevenda = oportunidadeConsulta.RevendaIntegrador.Id.ToString();
                        projetoItem.NomeRevenda = oportunidadeConsulta.RevendaIntegrador.Name;
                    }
                    
                    if (oportunidadeConsulta.Distribuidor != null)
                    {
                        projetoItem.CodigoDistribuidor = oportunidadeConsulta.Distribuidor.Id.ToString();
                        projetoItem.NomeDistribuidor = oportunidadeConsulta.Distribuidor.Name;
                    }
                    if (oportunidadeConsulta.Executivo != null)
                    {
                        projetoItem.CodigoExecutivo = oportunidadeConsulta.Executivo.Id.ToString();
                        projetoItem.NomeExecutivo = oportunidadeConsulta.Executivo.Name;
                    }
                    projetoItem.CNPJCliente = clientePotencialConsulta.Cnpj.Replace("-", "").Replace(".", "").Replace("/", "").Trim();
                    projetoItem.RazaoSocial = clientePotencialConsulta.NomeDaEmpresa;
                    projetoItem.NomeContatoCliente = clientePotencialConsulta.NomeCompletoDoContato;
                    if (!string.IsNullOrEmpty(clientePotencialConsulta.TelefoneComercial))
                    {
                        projetoItem.TelefoneContatoCliente = clientePotencialConsulta.TelefoneComercial;
                    }
                    projetoItem.EmailContatoCliente = clientePotencialConsulta.Email;
                    projetoItem.TipoProjeto = clientePotencialConsulta.TipoProjeto;
                    if (clientePotencialConsulta.EnvolverEngenharia)
                    {
                        projetoItem.EnvolverEngenharia = 993520000;
                    }
                    else
                    {
                        projetoItem.EnvolverEngenharia = 993520001;
                    }
                    projetoItem.ReceitaEstimada = clientePotencialConsulta.ValorEstimado;
                    projetoItem.DataPrevisaoFechamento = clientePotencialConsulta.DataEstimada;
                    UnidadeNegocio unidadeNegocio = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocio(clientePotencialConsulta.UnidadeNegocio.Id);
                    if (unidadeNegocio != null)
                    {
                        projetoItem.CodigoUnidadeNegocio = unidadeNegocio.ChaveIntegracao;
                        projetoItem.NomeUnidadeNegocio = unidadeNegocio.Nome;
                    }
                    projetoItem.DataCadastro = clientePotencialConsulta.DataCriacao;
                    projetoItem.DescricaoProjeto = clientePotencialConsulta.Descricao;
                    if (clientePotencialConsulta.CotacaoEnviada.HasValue)
                    {
                        projetoItem.CotacaoEnviada = clientePotencialConsulta.CotacaoEnviada;
                    }
                    if (oportunidadeConsulta.DataEnvioCotacao.HasValue) {
                        projetoItem.DataEnvioCotacao = oportunidadeConsulta.DataEnvioCotacao;
                    }
                    if (oportunidadeConsulta.TeveReuniao.HasValue)
                    {
                        projetoItem.TeveReuniao = oportunidadeConsulta.TeveReuniao;
                    }
                    if (oportunidadeConsulta.DataReuniao.HasValue)
                    {
                        projetoItem.DataReuniao = oportunidadeConsulta.DataReuniao;
                    }
                    if (oportunidadeConsulta.DataEstimativaAprovacao.HasValue)
                    {
                        projetoItem.DataEstimativaAprovacao = oportunidadeConsulta.DataEstimativaAprovacao;
                    }
                    if (oportunidadeConsulta.PropostaAprovada.HasValue)
                    {
                        projetoItem.PropostaAprovada = oportunidadeConsulta.PropostaAprovada;
                    }
                    if (oportunidadeConsulta.DataAprovacao.HasValue)
                    {
                        projetoItem.DataAprovacao = oportunidadeConsulta.DataAprovacao;
                    }
                    if (oportunidadeConsulta.DataEnvioPedidos.HasValue)
                    {
                        projetoItem.DataEnvioPedidos = oportunidadeConsulta.DataEnvioPedidos;
                    }
                    if (oportunidadeConsulta.PedidosFaturados.HasValue)
                    {
                        projetoItem.PedidosFaturados = oportunidadeConsulta.PedidosFaturados;
                    }
                    Usuario proprietarioOportunidade = new Servicos.UsuarioService(this.Organizacao, this.IsOffline).BuscarProprietario("opportunity", "opportunityid", oportunidadeConsulta.Id);
                    if (proprietarioOportunidade != null)
                    {
                        projetoItem.CodigoExecutivoIntelbras = proprietarioOportunidade.Id.ToString();
                        projetoItem.NomeExecutivoIntelbras = proprietarioOportunidade.NomeCompleto;
                    }
                }
                else
                {
                    projetoItem.NumeroProjeto = clientePotencialConsulta.NumeroProjeto;
                    projetoItem.ClassificacaoProjeto = 993520000;
                    projetoItem.CodigoClientePotencial = clientePotencialConsulta.Id.ToString();
                    projetoItem.SituacaoProjeto = ObterRazaoStatusClientePotencial(clientePotencialConsulta);
                    if (clientePotencialConsulta.RevendaIntegrador != null)
                    {
                        projetoItem.CodigoRevenda = clientePotencialConsulta.RevendaIntegrador.Id.ToString();
                        projetoItem.NomeRevenda = clientePotencialConsulta.RevendaIntegrador.Name;
                    }
                    if (clientePotencialConsulta.Distribuidor != null)
                    {
                        projetoItem.CodigoDistribuidor = clientePotencialConsulta.Distribuidor.Id.ToString();
                        projetoItem.NomeDistribuidor = clientePotencialConsulta.Distribuidor.Name;
                    }
                    if (clientePotencialConsulta.Executivo != null)
                    {
                        projetoItem.CodigoExecutivo = clientePotencialConsulta.Executivo.Id.ToString();
                        projetoItem.NomeExecutivo = clientePotencialConsulta.Executivo.Name;
                    }
                    projetoItem.CNPJCliente = clientePotencialConsulta.Cnpj.Replace("-", "").Replace(".", "").Replace("/", "").Trim();
                    projetoItem.RazaoSocial = clientePotencialConsulta.NomeDaEmpresa;
                    projetoItem.NomeContatoCliente = clientePotencialConsulta.NomeCompletoDoContato;
                    if (!string.IsNullOrEmpty(clientePotencialConsulta.TelefoneComercial))
                    {
                        projetoItem.TelefoneContatoCliente = clientePotencialConsulta.TelefoneComercial;
                    }
                    projetoItem.EmailContatoCliente = clientePotencialConsulta.Email;
                    projetoItem.TipoProjeto = clientePotencialConsulta.TipoProjeto;
                    if (clientePotencialConsulta.EnvolverEngenharia)
                    {
                        projetoItem.EnvolverEngenharia = 993520000;
                    }
                    else
                    {
                        projetoItem.EnvolverEngenharia = 993520001;
                    }
                    projetoItem.ReceitaEstimada = clientePotencialConsulta.ValorEstimado;
                    projetoItem.DataPrevisaoFechamento = clientePotencialConsulta.DataEstimada;
                    UnidadeNegocio unidadeNegocio = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocio(clientePotencialConsulta.UnidadeNegocio.Id);
                    if (unidadeNegocio != null)
                    {
                        projetoItem.CodigoUnidadeNegocio = unidadeNegocio.ChaveIntegracao;
                        projetoItem.NomeUnidadeNegocio = unidadeNegocio.Nome;
                    }
                    projetoItem.DataCadastro = clientePotencialConsulta.DataCriacao;
                    projetoItem.DescricaoProjeto = clientePotencialConsulta.Descricao;
                    if (clientePotencialConsulta.CotacaoEnviada.HasValue)
                    {
                        projetoItem.CotacaoEnviada = clientePotencialConsulta.CotacaoEnviada;
                    }

                    Usuario proprietario = new Servicos.UsuarioService(this.Organizacao, this.IsOffline).BuscarProprietario("lead", "leadid", clientePotencialConsulta.Id);
                    if (proprietario != null)
                    {
                        projetoItem.CodigoExecutivoIntelbras = proprietario.Id.ToString();
                        projetoItem.NomeExecutivoIntelbras = proprietario.NomeCompleto;
                    }

                    if (clientePotencialConsulta.TipoSolucao.HasValue)
                    {
                        projetoItem.TipoSolucao = clientePotencialConsulta.TipoSolucao;
                    }
                }
                #region Endereço
                projetoItem.EnderecoClienteFinal = new Pollux.Entities.Endereco();
                if (!string.IsNullOrEmpty(clientePotencialConsulta.Endereco1CEP))
                    projetoItem.EnderecoClienteFinal.CEP = clientePotencialConsulta.Endereco1CEP.Replace("-", "").PadLeft(8, '0'); ;
                projetoItem.EnderecoClienteFinal.Logradouro = clientePotencialConsulta.Endereco1Rua;
                projetoItem.EnderecoClienteFinal.Numero = clientePotencialConsulta.Endereco1Numero;
                if (!string.IsNullOrEmpty(clientePotencialConsulta.Endereco1Complemento))
                {
                    projetoItem.EnderecoClienteFinal.Complemento = clientePotencialConsulta.Endereco1Complemento;
                }
                projetoItem.EnderecoClienteFinal.Bairro = clientePotencialConsulta.Endereco1Bairro;

                if (clientePotencialConsulta.Endereco1Municipioid != null)
                {
                    Municipio municipio = new Servicos.MunicipioServices(this.Organizacao, this.IsOffline).ObterPor(clientePotencialConsulta.Endereco1Municipioid.Id);
                    projetoItem.EnderecoClienteFinal.NomeCidade = municipio.Nome;
                    projetoItem.EnderecoClienteFinal.Cidade = municipio.ChaveIntegracao;
                }
                if (clientePotencialConsulta.Endereco1Estadoid != null)
                {
                    Estado estado = new Servicos.EstadoServices(this.Organizacao, this.IsOffline).BuscaEstadoPorId(clientePotencialConsulta.Endereco1Estadoid.Id);
                    if (estado != null)
                    {
                        projetoItem.EnderecoClienteFinal.UF = estado.SiglaUF;
                        projetoItem.EnderecoClienteFinal.Estado = estado.ChaveIntegracao;
                    }
                }
                if (clientePotencialConsulta.Endereco1Pais != null)
                {
                    Pais pais = new Servicos.PaisServices(this.Organizacao, this.IsOffline).BuscaPais(clientePotencialConsulta.Endereco1Pais.Id);
                    if (pais != null)
                    {
                        projetoItem.EnderecoClienteFinal.NomePais = pais.Nome;
                        projetoItem.EnderecoClienteFinal.Pais = pais.Nome;
                    }
                }

                #endregion

                #region Atividades Projeto
                projetoItem.ListaAtividadesProjeto = new List<Pollux.Entities.AtividadeProjeto>();

                List<Postagem> lstAtividadesClientePotencial = new Servicos.PostagemService(this.Organizacao, this.IsOffline).ListarPorReferenteA(clientePotencialConsulta.Id);

                if (oportunidadeConsulta != null)
                {
                    List<Postagem> lstAtividadesOportunidade = new Servicos.PostagemService(this.Organizacao, this.IsOffline).ListarPorReferenteA(oportunidadeConsulta.Id);

                    if (lstAtividadesOportunidade.Count > 0)
                    {
                        foreach (Postagem postagem in lstAtividadesOportunidade)
                        {
                            Pollux.Entities.AtividadeProjeto atividadeProjeto = new Pollux.Entities.AtividadeProjeto();

                            atividadeProjeto.DataAtividade = postagem.CriadoEm;
                            atividadeProjeto.CodigoContato = postagem.UsuarioAtividade.Id.ToString();
                            atividadeProjeto.UsuarioAtividade = postagem.UsuarioAtividade.Name;
                            atividadeProjeto.DescricaoAtividade = postagem.Texto;

                            projetoItem.ListaAtividadesProjeto.Add(atividadeProjeto);
                        }
                    }
                }
                if (lstAtividadesClientePotencial.Count > 0)
                { 
                    foreach (Postagem postagem in lstAtividadesClientePotencial)
                    {
                        Pollux.Entities.AtividadeProjeto atividadeProjeto = new Pollux.Entities.AtividadeProjeto();

                        atividadeProjeto.DataAtividade = postagem.CriadoEm;
                        atividadeProjeto.CodigoContato = postagem.UsuarioAtividade.Id.ToString();
                        atividadeProjeto.UsuarioAtividade = postagem.UsuarioAtividade.Name;
                        atividadeProjeto.DescricaoAtividade = postagem.Texto;

                        projetoItem.ListaAtividadesProjeto.Add(atividadeProjeto);
                    }
                }
                #endregion

                #region Anexos Projeto
                projetoItem.ListaAnexosProjeto = new List<Pollux.Entities.AnexoProjeto>();
                if (oportunidadeConsulta != null)
                {
                    List<DocumentoSharePoint> lstAnexosOportunidade = new Servicos.SharePointSiteService(this.Organizacao, this.IsOffline).ListarPorIdRegistro(oportunidadeConsulta.ID.Value);

                    foreach (DocumentoSharePoint anexo in lstAnexosOportunidade)
                    {
                        try
                        {
                            string urlSite = ConfigurationManager.GetSettingValue("UrlSiteSharePoint");
                            string urlFolderDetail = "";
                            if (!string.IsNullOrEmpty(anexo.UrlAbsoluta))
                            {
                                urlFolderDetail = anexo.UrlAbsoluta;
                            }
                            else if (!string.IsNullOrEmpty(anexo.UrlRelativa))
                            {
                                urlFolderDetail = urlSite + "/opportunity/" + anexo.UrlRelativa;
                            }
                            using (ClientContext spClientContext = new ClientContext(urlSite))
                            {
                                spClientContext.Credentials = new NetworkCredential(usuarioSharePoint, senhaSharePoint, domain);
                                var rootWeb = spClientContext.Web;

                                Folder pastaPrincipal = rootWeb.GetFolderByServerRelativeUrl(urlFolderDetail);

                                if (pastaPrincipal.Files.AreItemsAvailable)
                                {
                                    spClientContext.Load(pastaPrincipal, fs => fs.Files, p => p.Folders);
                                    spClientContext.ExecuteQuery();
                                    FolderCollection folderCollection = pastaPrincipal.Folders;
                                    FileCollection fileCollection = pastaPrincipal.Files;


                                    foreach (var arquivo in fileCollection)
                                    {
                                        Pollux.Entities.AnexoProjeto anexoProjeto = new Pollux.Entities.AnexoProjeto();

                                        anexoProjeto.NomeArquivo = arquivo.Name;
                                        anexoProjeto.DataArquivo = arquivo.TimeCreated;
                                        anexoProjeto.UsuarioArquivo = anexo.ModificadoPor.Name;
                                        anexoProjeto.URL = ObterUrlArquivo(urlSite, arquivo.ServerRelativeUrl);

                                        projetoItem.ListaAnexosProjeto.Add(anexoProjeto);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            continue;
                        }

                    }
                }
                List<DocumentoSharePoint> lstAnexosClientePotencial = new Servicos.SharePointSiteService(this.Organizacao, this.IsOffline).ListarPorIdRegistro(clientePotencialConsulta.ID.Value);

                foreach (DocumentoSharePoint anexo in lstAnexosClientePotencial)
                {
                    try
                    {
                        string urlSite = ConfigurationManager.GetSettingValue("UrlSiteSharePoint");
                        string urlFolderDetail = "";
                        if (!string.IsNullOrEmpty(anexo.UrlAbsoluta))
                        {
                            urlFolderDetail = anexo.UrlAbsoluta;
                        }
                        else if (!string.IsNullOrEmpty(anexo.UrlRelativa))
                        {
                            urlFolderDetail = urlSite + "/lead/" + anexo.UrlRelativa;
                        }
                        using (ClientContext spClientContext = new ClientContext(urlSite))
                        {
                            spClientContext.Credentials = new NetworkCredential(usuarioSharePoint, senhaSharePoint, domain);
                            var rootWeb = spClientContext.Web;

                            Folder pastaPrincipal = rootWeb.GetFolderByServerRelativeUrl(urlFolderDetail);

                            spClientContext.Load(pastaPrincipal, fs => fs.Files, p => p.Folders);
                            spClientContext.ExecuteQuery();
                            FolderCollection folderCollection = pastaPrincipal.Folders;
                            FileCollection fileCollection = pastaPrincipal.Files;

                            foreach (var arquivo in fileCollection)
                            {
                                Pollux.Entities.AnexoProjeto anexoProjeto = new Pollux.Entities.AnexoProjeto();

                                anexoProjeto.NomeArquivo = arquivo.Name;
                                anexoProjeto.DataArquivo = arquivo.TimeLastModified;
                                anexoProjeto.UsuarioArquivo = anexo.ModificadoPor.Name;
                                anexoProjeto.URL = ObterUrlArquivo(urlSite, arquivo.ServerRelativeUrl);

                                projetoItem.ListaAnexosProjeto.Add(anexoProjeto);
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                #endregion

                #region Produtos Projeto

                projetoItem.ListaProdutosProjeto = new List<Pollux.Entities.ProdutoProjeto>();
                List<ProdutoProjeto> lstProdutosClientePotencial = new Servicos.ProdutoProjetoService(this.Organizacao, this.IsOffline).ListarPorClientePotencial(clientePotencialConsulta.Id);

                if (lstProdutosClientePotencial.Count > 0)
                {
                    foreach (ProdutoProjeto produtoCliente in lstProdutosClientePotencial)
                    {
                        Pollux.Entities.ProdutoProjeto produtoProjeto = new Pollux.Entities.ProdutoProjeto();
                        var produto = new Servicos.ProdutoService(this.Organizacao, this.IsOffline).ObterPor(produtoCliente.Produto.Id);

                        produtoProjeto.CodigoProdutoClientePotencial = produtoCliente.Id.ToString();
                        produtoProjeto.CodigoProduto = produto.Codigo;
                        produtoProjeto.DescricaoProduto = produto.Nome;
                        produtoProjeto.Quantidade = produtoCliente.Quantidade;
                        produtoProjeto.PrecoUnitario = produtoCliente.ValorUnitario;
                        produtoProjeto.ValorTotal = produtoCliente.ValorTotal;

                        projetoItem.ListaProdutosProjeto.Add(produtoProjeto);
                    }
                }

                #endregion

                if (!resultadoPersistencia.Sucesso)
                {
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0269R1>(numeroMensagem, retorno);
                }

                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";
                retorno.Add("ProjetoItem", projetoItem);
                retorno.Add("Resultado", resultadoPersistencia);

                return CriarMensagemRetorno<Pollux.MSG0269R1>(numeroMensagem, retorno);
            }
            catch (Exception e)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(e);
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0269R1>(numeroMensagem, retorno);
            }
        }

        #region Definir Propriedades
        public ClientePotencial DefinirPropriedades(Intelbras.Message.Helper.MSG0269 xml)
        {
            ClientePotencial retorno = new ClientePotencial(this.Organizacao, this.IsOffline);
            return retorno;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(ClientePotencial objModel)
        {
            string resposta = string.Empty;

            return resposta;
        }

        public int? ObterRazaoStatusClientePotencial(ClientePotencial clientePotencial)
        {
            int? razaostatus = 993520003;
            if (clientePotencial.RazaoStatus == 1)
            {
                razaostatus = 993520003;
            }
            else if (clientePotencial.RazaoStatus == 2)
            {
                if ((clientePotencial.StageId.Value.ToString().ToUpper() == "00D0B952-45A4-3C15-E2A3-04E57485816E") || (clientePotencial.StageId.Value.ToString().ToUpper() == "B25D7A8D-C61D-1103-492C-AE77C795C9EE"))
                {
                    razaostatus = 993520005;
                }
                if ((clientePotencial.StageId.Value.ToString().ToUpper() == "8AA82057-E761-70CE-4A4C-0184CA48D938") || (clientePotencial.StageId.Value.ToString().ToUpper() == "5106C715-92B3-0C12-3503-D3810765A259"))
                {
                    razaostatus = 993520006;
                }
            }
            else if (clientePotencial.RazaoStatus == 3)
            {
                razaostatus = 993520007;
            }
            else if ((clientePotencial.RazaoStatus == 4) || (clientePotencial.RazaoStatus == 5))
            {
                razaostatus = 993520000;
            }
            else if ((clientePotencial.RazaoStatus == 6) || (clientePotencial.RazaoStatus == 7))
            {
                razaostatus = 993520002;
            }
            else
            {
                razaostatus = clientePotencial.RazaoStatus;
            }
            return razaostatus;
        }

        public int? ObterRazaoStatusOportunidade(Oportunidade oportunidade)
        {
            int? razaostatus = 993520003;
            if ((oportunidade.RazaoStatus == 1) || (oportunidade.RazaoStatus == 200011))
            {
                if (oportunidade.StageId != null)
                {
                    if (oportunidade.StageId.Value.ToString().ToUpper() == "A5C2D354-2233-D85E-CE43-366685757134")
                    {
                        razaostatus = 993520001;
                    }
                    if ((oportunidade.StageId.Value.ToString().ToUpper() == "1DB22B78-19E7-5CA1-CCB1-BD84AE230087") || (oportunidade.StageId.Value.ToString().ToUpper() == "E2B91BCD-488A-5410-9049-1E7C8ADBE72C"))
                    {
                        razaostatus = 993520007;
                    }
                    if (oportunidade.StageId.Value.ToString().ToUpper() == "CDA64E48-BABC-4EBB-CE63-8D7DE2C9729D")
                    {
                        razaostatus = 993520008;
                    }
                    if (oportunidade.StageId.Value.ToString().ToUpper() == "08B6E87E-5D19-3952-64BC-D6C0EDE6DA37")
                    {
                        razaostatus = 993520009;
                    }
                    if (oportunidade.StageId.Value.ToString().ToUpper() == "6E7E126A-A9A0-6CDE-74AC-1509ABA87087")
                    {
                        razaostatus = 993520010;
                    }
                }
                else { razaostatus = 993520007; }
            }
            else if (oportunidade.RazaoStatus == 3)
            {
                razaostatus = 993520001;
            }
            else if (oportunidade.RazaoStatus == 4)
            {
                razaostatus = 993520002;
            }
            else if (oportunidade.RazaoStatus == 5)
            {
                razaostatus = 993520000;
            }
            else
            {
                razaostatus = oportunidade.RazaoStatus;
            }
            return razaostatus;
        }

        private string ObterUrlArquivo(string urlSite, string urlServerRelative)
        {
            var siteSplit = urlSite.Split('/');
            urlSite = "";

            foreach (var itemSplited in siteSplit)
            {
                if (!string.IsNullOrEmpty(itemSplited))
                {
                    if (!urlServerRelative.Contains(itemSplited))
                    {
                        if (siteSplit.First() == itemSplited)
                            urlSite += itemSplited + "/";
                        else
                            urlSite += "/" + itemSplited;
                    }
                }
            }

            return urlSite + urlServerRelative;
        }
        #endregion

    }
}