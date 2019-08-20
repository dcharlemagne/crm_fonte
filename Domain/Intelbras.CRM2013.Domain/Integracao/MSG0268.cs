using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0268 : Base, IBase<Message.Helper.MSG0268, Domain.Model.ClientePotencial>
    {
        #region Construtor

        public MSG0268(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }

        #endregion

        #region Propriedades
        //Dictionary que sera enviado como resposta do request

        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private bool originadoExtranet = false;


        #endregion

        #region trace

        private SDKore.Helper.Trace Trace { get; set; }
        public void DefinirObjetoTrace(SDKore.Helper.Trace trace)
        {
            this.Trace = trace;
        }

        #endregion

        #region Executar

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            usuarioIntegracao = usuario;

            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0268>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0268R1>(numeroMensagem, retorno);
            }

            objeto = new Domain.Servicos.LeadService(this.Organizacao, this.IsOffline).Persistir(objeto);

            var objeto2 = this.DefinirPropriedades2(this.CarregarMensagem<Pollux.MSG0268>(mensagem), new Servicos.RepositoryService().ClientePotencial.Retrieve(objeto.ID.Value));
            if (objeto2 != null)
            {
                if (!resultadoPersistencia.Sucesso)
                {
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0268R1>(numeroMensagem, retorno);
                }
                new Servicos.RepositoryService().Oportunidade.Update(objeto2);
            }

            #region Lista de Produtos
            if (objeto.StatusValidacao != (int)Enum.StatusDaValidacao.Aceito)
            {
                List<ProdutoProjeto> lstProdutosProjeto = this.DefinirPropriedadesProdutos(this.CarregarMensagem<Pollux.MSG0268>(mensagem), new Servicos.RepositoryService().ClientePotencial.Retrieve(objeto.ID.Value));
                var produtosTemp = new Domain.Servicos.ProdutoProjetoService(this.Organizacao, this.IsOffline).Persistir(lstProdutosProjeto);
                new Domain.Servicos.ProdutoProjetoService(this.Organizacao, this.IsOffline).AtualizarListagemProdutos(objeto.ID.Value, produtosTemp);
            }
            #endregion

            if (objeto != null)
            {
                if (originadoExtranet)
                {
                    //seta um usuário do comercial como proprietário de cliente potencial originado na extranet                
                    new Servicos.UtilService(this.Organizacao, this.IsOffline).MudarProprietarioRegistro("systemuser", new Guid(SDKore.Configuration.ConfigurationManager.GetSettingValue("ProprietarioComercial")), "lead", objeto.ID.Value);
                }
                while (string.IsNullOrEmpty(objeto.NumeroProjeto))
                {
                    objeto = new Servicos.RepositoryService().ClientePotencial.Retrieve(objeto.ID.Value);
                }
                retorno.Add("NumeroProjeto", objeto.NumeroProjeto);
                retorno.Add("CodigoClientePotencial", objeto.ID.Value.ToString());
                if (objeto2 != null)
                {
                  retorno.Add("CodigoOportunidade", objeto2.ID.Value.ToString());
                }
                retorno.Add("Resultado", resultadoPersistencia);
            }

            return CriarMensagemRetorno<Pollux.MSG0268R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        
        public ClientePotencial DefinirPropriedades(Intelbras.Message.Helper.MSG0268 xml)
        {
            var crm = new Model.ClientePotencial(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            crm.IntegrarNoPlugin = true;

            if (!String.IsNullOrEmpty(xml.NumeroProjeto))
            {
                ClientePotencial cliente = new Servicos.LeadService(this.Organizacao, this.IsOffline).ObterPorNumeroProjeto(xml.NumeroProjeto);
                if (cliente != null)
                {
                    crm.NumeroProjeto = xml.NumeroProjeto;
                    crm.StatusValidacao = cliente.StatusValidacao;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "NumeroProjeto informado não existe para ser atualizado.";
                    return crm;
                }
            }

            if (!String.IsNullOrEmpty(xml.CodigoClientePotencial))
            {
                crm.ID = new Guid(xml.CodigoClientePotencial);
            }else
            {
                crm.RazaoStatus = 993520003;

                originadoExtranet = true;
            }

            if (!String.IsNullOrEmpty(xml.CodigoRevenda) && xml.CodigoRevenda.Length == 36)
            {
                crm.RevendaIntegrador = new Lookup(new Guid(xml.CodigoRevenda), "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoRevenda não enviado ou fora do padrão (Guid).";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.CodigoDistribuidor) && xml.CodigoDistribuidor.Length == 36)
            {
                crm.Distribuidor = new Lookup(new Guid(xml.CodigoDistribuidor), "");
            }

            Contato executivo = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(new Guid(xml.CodigoExecutivo));
            if (executivo != null)
            {
                crm.Executivo = new Lookup(executivo.ID.Value, "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoExecutivo não encontrado no Crm.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.CNPJCliente))
            {
                crm.Cnpj = Intelbras.CRM2013.Domain.Servicos.Helper.FormatarCnpj(xml.CNPJCliente);
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CNPJCliente não enviado.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.RazaoSocial))
            {
                crm.NomeDaEmpresa = xml.RazaoSocial;
                crm.Topico = xml.RazaoSocial;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "RazaoSocial não enviado.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.NomeContatoCliente))
            {
                crm.NomeCompletoDoContato = xml.NomeContatoCliente;

                if (xml.NomeContatoCliente.Contains(" "))
                {
                    crm.PrimeiroNomeDoContato = xml.NomeContatoCliente.Substring(0, xml.NomeContatoCliente.IndexOf(' '));
                    crm.SobreNomeDoContato = xml.NomeContatoCliente.Substring(xml.NomeContatoCliente.IndexOf(' ') + 1, (xml.NomeContatoCliente.Length - xml.NomeContatoCliente.IndexOf(' ') - 1));
                }else
                {
                    crm.PrimeiroNomeDoContato = xml.NomeContatoCliente;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "NomeContatoCliente não enviado.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.TelefoneContatoCliente))
            {
                crm.TelefoneComercial = xml.TelefoneContatoCliente;
                crm.TelefoneCelular = xml.TelefoneContatoCliente;
            }

            if (!String.IsNullOrEmpty(xml.EmailContatoCliente))
            {
                crm.Email = xml.EmailContatoCliente;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "EmailContatoCliente não enviado.";
                return crm;
            }

            if (xml.TipoProjeto.HasValue)
            {
                crm.TipoProjeto = xml.TipoProjeto;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "TipoProjeto não enviado.";
                return crm;
            }

            if (xml.EnvolverEngenharia.HasValue)
            {
                if (xml.EnvolverEngenharia.Value == 993520000)
                {
                    crm.EnvolverEngenharia = true;
                }
                else
                {
                    crm.EnvolverEngenharia = false;

                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "EnvolverEngenharia não enviado.";
                return crm;
            }

            if (xml.ReceitaEstimada.HasValue)
            {
                crm.ValorEstimado = (decimal)xml.ReceitaEstimada;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "ReceitaEstimada não enviado.";
                return crm;
            }

            if (xml.DataPrevisaoFechamento.HasValue)
            {
                crm.DataEstimada = (DateTime)xml.DataPrevisaoFechamento;

                crm.DataProximaInteracao = DateTime.Now.AddDays(30);
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "DataPrevisaoFechamento não enviado.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.CodigoUnidadeNegocio))
            {
                UnidadeNegocio unidadeNegocio = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocioPorChaveIntegracao(xml.CodigoUnidadeNegocio);
                if (unidadeNegocio != null)
                {
                    crm.UnidadeNegocio = new Lookup(unidadeNegocio.Id, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoUnidadeNegocio não enencontrado no CRM.";
                    return crm;
                }
            }


            if (!String.IsNullOrEmpty(xml.DescricaoProjeto))
            {
                crm.Descricao = xml.DescricaoProjeto;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "DescricaoProjeto não enviado ou fora do padrão (Guid).";
                return crm;
            }

            if (xml.CotacaoEnviada.HasValue)
            {
                crm.CotacaoEnviada = xml.CotacaoEnviada;
            }

            if (xml.TipoSolucao.HasValue)
            {
                crm.TipoSolucao = xml.TipoSolucao;
            }
            else
            {
                crm.AddNullProperty("TipoSolucao");
            }

            if(xml.PossivelDuplicidade.HasValue)
            {
                crm.PossivelDuplicidade = (bool)xml.PossivelDuplicidade;
            }

            #endregion

            #region Endereco
            crm.Endereco1CEP = xml.EnderecoClienteFinal.CEP;
            crm.Endereco1Rua = xml.EnderecoClienteFinal.Logradouro;
            crm.Endereco1Numero = xml.EnderecoClienteFinal.Numero;
            if (!String.IsNullOrEmpty(xml.EnderecoClienteFinal.Complemento))
            {
                crm.Endereco1Complemento = xml.EnderecoClienteFinal.Complemento;
            }
            crm.Endereco1Bairro = xml.EnderecoClienteFinal.Bairro;
            //Cidade
            if (!String.IsNullOrEmpty(xml.EnderecoClienteFinal.Cidade))
            {
                Municipio cidade = new Intelbras.CRM2013.Domain.Servicos.MunicipioServices(this.Organizacao, this.IsOffline).BuscaCidade(xml.EnderecoClienteFinal.Cidade);

                if (cidade != null && cidade.ID.HasValue)
                {
                    crm.Endereco1Municipioid = new Lookup(cidade.ID.Value, "");
                    crm.Endereco1Municipio = cidade.Nome;
                }
            }

            //Estado
            if (!String.IsNullOrEmpty(xml.EnderecoClienteFinal.Estado))
            {
                Estado estado = new Intelbras.CRM2013.Domain.Servicos.EstadoServices(this.Organizacao, this.IsOffline).BuscaEstado(xml.EnderecoClienteFinal.Estado);

                if (estado != null && estado.ID.HasValue)
                {
                    crm.Endereco1Estadoid = new Lookup(estado.ID.Value, "");
                }
            }

            //Pais
            if (!String.IsNullOrEmpty(xml.EnderecoClienteFinal.Pais))
            {
                Pais Pais = new Intelbras.CRM2013.Domain.Servicos.PaisServices(this.Organizacao, this.IsOffline).BuscaPais(xml.EnderecoClienteFinal.Pais);

                if (Pais != null && Pais.ID.HasValue)
                {
                    crm.Endereco1Pais = new Lookup(Pais.ID.Value, "");
                }
            }
            #endregion

            return crm;
        }

        public Oportunidade DefinirPropriedades2(Intelbras.Message.Helper.MSG0268 xml, ClientePotencial ClientePotencial)
        {
            #region Propriedades Crm->Xml

            var crm2 = new Servicos.RepositoryService().Oportunidade.BuscarPor(ClientePotencial);

            if (crm2 != null)
            {
                crm2.IntegrarNoPlugin = true;

                crm2.NumeroProjeto = ClientePotencial.NumeroProjeto;

                if (!String.IsNullOrEmpty(xml.CodigoRevenda) && xml.CodigoRevenda.Length == 36)
                {
                    if ((crm2.RevendaIntegrador == null) || (crm2.RevendaIntegrador.Id.ToString() != xml.CodigoRevenda))
                    {
                        crm2.RevendaIntegrador = new Lookup(new Guid(xml.CodigoRevenda), "");
                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoRevenda não enviado ou fora do padrão (Guid).";
                    return crm2;
                }

                Contato executivo = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(new Guid(xml.CodigoExecutivo));
                if (executivo != null) 
                {
                    if ((crm2.Executivo == null) || (crm2.Executivo.Id != executivo.ID.Value))
                    {
                        crm2.Executivo = new Lookup(executivo.ID.Value, "");
                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoExecutivo não encontrado no Crm.";
                    return crm2;
                }

                if (xml.DataEnvioCotacao.HasValue)
                {
                    crm2.DataEnvioCotacao = xml.DataEnvioCotacao;
                }
                else
                {
                    crm2.DataEnvioCotacao = null;
                }

                if (xml.TeveReuniao.HasValue)
                {
                    crm2.TeveReuniao = xml.TeveReuniao;
                }
                else
                {
                    crm2.TeveReuniao = (Int32)this.PreencherAtributoVazio("int");
                }

                if (xml.DataReuniao.HasValue)
                {
                    crm2.DataReuniao = xml.DataReuniao;
                }
                else
                {
                    crm2.DataReuniao = null;
                }

                if (xml.DataEstimativaAprovacao.HasValue)
                {
                    crm2.DataEstimativaAprovacao = xml.DataEstimativaAprovacao;
                }
                else
                {
                    crm2.DataEstimativaAprovacao = null;
                }

                if (xml.PropostaAprovada.HasValue)
                {
                    crm2.PropostaAprovada = xml.PropostaAprovada;
                }
                else
                {
                    crm2.PropostaAprovada = (Int32)PreencherAtributoVazio("int");
                }

                if (xml.DataAprovacao.HasValue)
                {
                    crm2.DataAprovacao = xml.DataAprovacao;
                }
                else
                {
                    crm2.DataAprovacao = null;
                }

                if (xml.DataEnvioPedidos.HasValue)
                {
                    crm2.DataEnvioPedidos = xml.DataEnvioPedidos;
                }
                else
                {
                    crm2.DataEnvioPedidos = null;
                }

                if (xml.PedidosFaturados.HasValue)
                {
                    crm2.PedidosFaturados = xml.PedidosFaturados;
                }
                else
                {
                    crm2.PedidosFaturados = 993520001;
                }

                #endregion

                return crm2;
            }
            return null;
        }
   
        public List<ProdutoProjeto> DefinirPropriedadesProdutos(Intelbras.Message.Helper.MSG0268 xml, ClientePotencial ClientePotencial)
        {
            List<ProdutoProjeto> lstRetorno = new List<ProdutoProjeto>();

            #region Lista ProdutoProjeto
            foreach (var item in xml.ListaProdutosProjeto)
            {
                //verificar se o vinculo existe criar ou editar

                ProdutoProjeto produtoProjeto = new ProdutoProjeto(this.Organizacao, this.IsOffline);
                if (!String.IsNullOrEmpty(item.CodigoProdutoClientePotencial))
                {
                    ProdutoProjeto produtoProjetoTemp = new Servicos.RepositoryService().ProdutoProjeto.Retrieve(new Guid(item.CodigoProdutoClientePotencial)); //new Servicos.ProdutoService(this.Organizacao, this.IsOffline).BuscaPorCodigo(item.CodigoProdutoClientePotencial);
                    if (produtoProjeto == null)
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Produto não cadastrado no Crm!";
                        return null;
                    }

                    produtoProjeto = produtoProjetoTemp;
                }

                if (!String.IsNullOrEmpty(item.CodigoProduto))
                {
                    Product produto = new Servicos.ProdutoService(this.Organizacao, this.IsOffline).BuscaPorCodigo(item.CodigoProduto);
                    if (produto != null)
                    {
                        produtoProjeto.Produto = new Lookup(produto.ID.Value, "");
                    }
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Produto não cadastrado no Crm!";
                        return null;
                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Codigo Produto não enviado.";
                    return null;
                }

                produtoProjeto.ClientePotencial =  new Lookup(ClientePotencial.ID.Value, "");
                produtoProjeto.ValorUnitario = (item.PrecoUnitario == null) ? 0 : item.PrecoUnitario;
                produtoProjeto.Quantidade = item.Quantidade;
                produtoProjeto.ValorTotal = (item.ValorTotal == null) ? 0 : item.ValorTotal;

                lstRetorno.Add(produtoProjeto);
            }
            #endregion

            return lstRetorno;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(ClientePotencial objModel)
        {
            return String.Empty;
        }

        #endregion
    }
}
