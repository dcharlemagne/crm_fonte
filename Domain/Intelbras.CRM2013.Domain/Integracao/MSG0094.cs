// MENSAGEM RELACIONADA AO PROCESSO PEDIDO
// IMPLEMENTADA EM 21/03/14 POR FCJ - PARCIALMENTE

using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0094 : Base, IBase<Intelbras.Message.Helper.MSG0094, Domain.Model.Fatura>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        List<ProdutoFatura> lstProdutoFaturaUpdate = new List<ProdutoFatura>();
        List<ProdutoFatura> lstProdutoFaturaDelete = new List<ProdutoFatura>();


        #endregion

        #region Construtor
        public MSG0094(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }
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
            Pollux.MSG0094 polluxMsg0094 = this.CarregarMensagem<Pollux.MSG0094>(mensagem);

            if (!string.IsNullOrWhiteSpace(polluxMsg0094.ChaveIntegracao))
            {
                var statusFatura = new Intelbras.CRM2013.Domain.Servicos.RepositoryService(this.Organizacao, this.IsOffline)
                    .Fatura.ObterPorChaveIntegracao(polluxMsg0094.ChaveIntegracao, "statecode");

                if (statusFatura != null)
                {
                    if (statusFatura.Status.Value == (int)Enum.Fatura.Status.Cancelada)
                    {
                        resultadoPersistencia.Sucesso = true;
                        resultadoPersistencia.Mensagem = "Fatura foi ignorado para atualização, o status no CRM é Cancelada e não é feito mais modificações!";
                        retorno.Add("Resultado", resultadoPersistencia);
                        return CriarMensagemRetorno<Pollux.MSG0094R1>(numeroMensagem, retorno);
                    }
                }
            }

            Fatura ObjFatura = this.DefinirPropriedades(polluxMsg0094);
            this.DefinirPropriedadesProdutoFatura(polluxMsg0094, Guid.Empty);

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0094R1>(numeroMensagem, retorno);
            }

            //Para poder atualizar state posteriormente
            int? stateUpdate = ObjFatura.Status;
            int? razaoStatusUpdate = ObjFatura.RazaoStatus;
            bool alteraStatus = false;

            int countErrosIntegracaoItem = lstProdutoFaturaUpdate.Where(x => x.IntegradoRepresentanteComErro.Value).Count();

            if (countErrosIntegracaoItem > 0)
            {
                ObjFatura.IntegradoComErros = true;
            }

            //Persistir Fatura para obter o id que sera utilizado no ProdutoFatura
            ObjFatura = new Intelbras.CRM2013.Domain.Servicos.FaturaService(this.Organizacao, this.IsOffline).Persistir(ObjFatura, ref alteraStatus);
            if (ObjFatura == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro ao persistir Fatura!";

                retorno.Add("Resultado", resultadoPersistencia);

                return CriarMensagemRetorno<Pollux.MSG0094R1>(numeroMensagem, retorno);
            }
            else

                if (lstProdutoFaturaUpdate == null && lstProdutoFaturaDelete == null)
            {
                this.DefinirPropriedadesProdutoFatura(polluxMsg0094, ObjFatura.ID.Value);
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0094R1>(numeroMensagem, retorno);
            }

            if (!resultadoPersistencia.Sucesso)
            //if (lstProdutoFaturaUpdate.Any())
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0094R1>(numeroMensagem, retorno);
            }

            // foreach (ProdutoFatura item in lstProdutoFaturaUpdate)
            //  {
            //bool mudarprop = false;
            //item.Fatura = new Lookup(ObjFatura.ID.Value, "");

            //if (lstProdutoFaturaUpdate == null && lstProdutoFaturaDelete == null)
            //    ProdutoFatura ObjProdutoFatura = new Intelbras.CRM2013.Domain.Servicos.ProdutoFaturaService(this.Organizacao, this.IsOffline).Atualizar(item, ref mudarprop);

            //if (ObjProdutoFatura == null)
            //{
            //retorno.Add("Resultado", resultadoPersistencia);
            //return CriarMensagemRetorno<Pollux.MSG0094R1>(numeroMensagem, retorno);
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Erro de persistência no item da fatura.";
            //    retorno.Add("Resultado", resultadoPersistencia);
            //    return CriarMensagemRetorno<Pollux.MSG0094R1>(numeroMensagem, retorno);
            //}

            if (lstProdutoFaturaUpdate.Any())
            {
                foreach (ProdutoFatura item in lstProdutoFaturaUpdate)
                {
                    bool mudarprop = false;
                    item.Fatura = new Lookup(ObjFatura.ID.Value, "");

                    ProdutoFatura ObjProdutoFatura = new Intelbras.CRM2013.Domain.Servicos.ProdutoFaturaService(this.Organizacao, this.IsOffline).Atualizar(item, ref mudarprop);

                    if (ObjProdutoFatura == null)
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Erro de persistência no item da fatura.";
                        retorno.Add("Resultado", resultadoPersistencia);
                        return CriarMensagemRetorno<Pollux.MSG0094R1>(numeroMensagem, retorno);
                    }
                }
            }
            if (lstProdutoFaturaDelete.Any())
            {
                foreach (ProdutoFatura item in lstProdutoFaturaUpdate)
                {
                    new Intelbras.CRM2013.Domain.Servicos.ProdutoFaturaService(this.Organizacao, this.IsOffline).Deletar(item);
                }
            }
            //}
            //Alterar Status da Fatura após todo processo
            if (alteraStatus)
            {
                if (ObjFatura.ID.HasValue && stateUpdate.HasValue && razaoStatusUpdate.HasValue)
                    new Intelbras.CRM2013.Domain.Servicos.FaturaService(this.Organizacao, this.IsOffline).MudarStatus(ObjFatura.ID.Value, stateUpdate.Value, razaoStatusUpdate.Value);
            }

            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0094R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        
        public Fatura DefinirPropriedades(Intelbras.Message.Helper.MSG0094 xml)
        {
            Fatura crm = new Fatura(this.Organizacao, this.IsOffline);
            
            if (!String.IsNullOrEmpty(xml.NumeroNotaFiscal))
            {
                crm.NumeroNF = xml.NumeroNotaFiscal;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "NumeroNotaFiscal não enviado.";
                return crm;
            }

            crm.Serie = xml.NumeroSerie;
            crm.ClienteId = new Lookup(new Guid(xml.CodigoClienteCRM), xml.TipoObjetoCliente);


            crm.ChaveIntegracao = xml.ChaveIntegracao; //NumeroPedido.ToString();

            if (xml.NumeroPedido != null)
            {
                Pedido pedido = new Servicos.PedidoService(this.Organizacao, this.IsOffline).BuscaPedidoEMS(xml.NumeroPedido.ToString());
                if (pedido != null)
                {
                    crm.PedidoCRM = new Lookup(pedido.ID.Value, "");

                    Fatura fatura = new Servicos.FaturaService(this.Organizacao, this.IsOffline).ObterFaturaPorPedidoEMS(xml.NumeroPedido.ToString());
                    if (fatura != null)
                    {
                        crm.ID = fatura.ID.Value;
                    }
                }
            }

            if (!String.IsNullOrEmpty(xml.NumeroPedidoCliente))
                crm.PedidoCliente = xml.NumeroPedidoCliente;
            else
                crm.AddNullProperty("PedidoCliente");

            if (!String.IsNullOrEmpty(xml.Descricao))
                crm.Descricao = xml.Descricao;
            else
                crm.AddNullProperty("Descricao");

            //Service Estabelecimento
            Estabelecimento ObjEstabelecimento = new Intelbras.CRM2013.Domain.Servicos.EstabelecimentoService(this.Organizacao, this.IsOffline).BuscaEstabelecimentoPorCodigo(xml.Estabelecimento.Value);

            if (ObjEstabelecimento != null)
            {
                crm.Estabelecimento = new Lookup((Guid)ObjEstabelecimento.ID, "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador do Estabelecimento não encontrado.";
                return crm;
            }

            //Service Condição Pagamento
            CondicaoPagamento ObjCondicaoPagamento = null;
            if (xml.CondicaoPagamento.HasValue)
            {
                ObjCondicaoPagamento = new Intelbras.CRM2013.Domain.Servicos.CondicaoPagamentoService(this.Organizacao, this.IsOffline).BuscaCondicaoPagamentoPorCodigo(xml.CondicaoPagamento.Value);

                if (ObjCondicaoPagamento != null)
                {
                    crm.CondicaoPagamento = new Lookup((Guid)ObjCondicaoPagamento.ID, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Identificador da Condição de Pagamento não encontrado.";
                    return crm;
                }
            }
            else
                crm.AddNullProperty("CondicaoPagamento");

            crm.NomeAbreviado = xml.NomeAbreviadoCliente;

            //Service Natureza Operação
            NaturezaOperacao ObjNaturezaOperacao = new Intelbras.CRM2013.Domain.Servicos.NaturezaOperacaoService(this.Organizacao, this.IsOffline).BuscaNaturezaOperacaoPorCodigo(xml.NaturezaOperacao);

            if (ObjNaturezaOperacao != null)
            {
                crm.NaturezaOperacao = new Lookup((Guid)ObjNaturezaOperacao.ID, "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador da Natureza de Operação não encontrado.";
                return crm;
            }

            //Service Moeda
            Moeda ObjMoeda = new Intelbras.CRM2013.Domain.Servicos.MoedaService(this.Organizacao, this.IsOffline).BuscaMoedaPorCodigo(xml.Moeda);

            if (ObjMoeda != null)
            {
                crm.Moeda = new Lookup((Guid)ObjMoeda.ID, "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador da Moeda não encontrado.";
                return crm;
            }

            //Service para resolver o status
            if (System.Enum.IsDefined(typeof(Enum.Fatura.Status), xml.SituacaoNota))
                crm.Status = xml.SituacaoNota;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador da Situação da nota não encontrado.";
                return crm;
            }

            if (System.Enum.IsDefined(typeof(Enum.Fatura.RazaoStatus), xml.SituacaoEntrega))
                crm.RazaoStatus = xml.SituacaoEntrega;

            crm.DataEmissao = xml.DataEmissao;

            if (xml.DataSaida.HasValue)
                crm.DataSaida = xml.DataSaida;
            else
                crm.AddNullProperty("DataSaida");

            if (xml.DataEntrega.HasValue)
                crm.DataEntrega = xml.DataEntrega;
            else
                crm.AddNullProperty("DataEntrega");

            if (xml.DataConfirmacao != null)
            {
                crm.DataConfirmacao = xml.DataConfirmacao;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "DataConfirmacao não enviada.";
                return crm;
            }

            if (xml.DataCancelamento.HasValue)
                crm.DataCancelamento = xml.DataCancelamento;
            else
                crm.AddNullProperty("DataCancelamento");

            if (xml.DataConclusao.HasValue)
                crm.DataConclusao = xml.DataConclusao;
            else
                crm.AddNullProperty("DataConclusao");

            crm.ValorFrete = xml.ValorFrete;

            crm.PesoLiquido = xml.PesoLiquido;

            crm.PesoBruto = xml.PesoBruto;

            if (!String.IsNullOrEmpty(xml.Observacao))
                crm.Observacao = xml.Observacao;
            else
                crm.AddNullProperty("Observacao");

            crm.Volume = xml.Volume.ToString();

            crm.BaseICMS = xml.ValorBaseICMS;
            crm.ValorICMS = xml.ValorICMS;
            crm.ValorIPI = xml.ValorIPI;
            crm.BaseSubstTributaria = xml.ValorBaseSubstituicaoTributaria;
            crm.ValorSubstituicao = xml.ValorSubstituicaoTributaria;
            crm.ClienteRetira = xml.RetiraNoLocal;

            if (xml.MetodoEntrega.HasValue)
                crm.MetodoEntrega = xml.MetodoEntrega;
            else
                crm.AddNullProperty("MetodoEntrega");

            //Service Transportadora
            Transportadora ObjTransportadora = new Intelbras.CRM2013.Domain.Servicos.TransportadoraService(this.Organizacao, this.IsOffline).ObterPorCodigoTransportadora(xml.Transportadora.Value);

            if (ObjTransportadora != null)
            {
                crm.Transportadora = new Lookup((Guid)ObjTransportadora.ID, "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador da Transportadora não encontrado.";
                return crm;
            }

            crm.Frete = xml.Frete;

            //crm.CondicoesFrete = xml.tipo;

            if (!String.IsNullOrEmpty(xml.TelefoneCobranca))
                crm.TelefoneCobranca = xml.TelefoneCobranca;
            else
                crm.AddNullProperty("TelefoneCobranca");

            if (!String.IsNullOrEmpty(xml.FaxCobranca))
                crm.FaxCobranca = xml.FaxCobranca;
            else
                crm.AddNullProperty("FaxCobranca");

            //Removido essa validação para que as NF de clientes estrangeiros sejam integrados no CRM
            //if (!String.IsNullOrEmpty(xml.CNPJ))
            //    crm.CpfCnpj = Intelbras.CRM2013.Domain.Servicos.Helper.FormatarCnpj(xml.CNPJ);
            //else if (!String.IsNullOrEmpty(xml.CPF))
            //    crm.CpfCnpj = Intelbras.CRM2013.Domain.Servicos.Helper.FormatarCpf(xml.CPF);
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "CNPJ/CPF não enviado.";
            //    return crm;
            //}

            if (!String.IsNullOrEmpty(xml.InscricaoEstadual))
                crm.InscricaoEstadual = xml.InscricaoEstadual;
            else
                crm.AddNullProperty("InscricaoEstadual");

            if (!string.IsNullOrEmpty(xml.Oportunidade))
                crm.Oportunidade = new Lookup(new Guid(xml.Oportunidade), "");

            xml.PrecoBloqueado = xml.PrecoBloqueado;

            if (xml.ValorDesconto.HasValue)
                crm.ValorDescontoFatura = xml.ValorDesconto;
            else
                crm.AddNullProperty("ValorDescontoFatura");

            if (xml.PercentualDesconto.HasValue)
                crm.DescontoGlobalTotal = xml.PercentualDesconto;
            else
                crm.AddNullProperty("DescontoGlobalTotal");

            //Service Lista Preço
            ListaPreco ObjListaPreco = new Intelbras.CRM2013.Domain.Servicos.ListaPrecoService(this.Organizacao, this.IsOffline).BuscaListaPreco(xml.ListaPreco);

            if (ObjListaPreco != null)
            {
                crm.ListaPrecos = new Lookup((Guid)ObjListaPreco.ID, "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador da Lista de Preço não encontrado.";
                return crm;
            }

            crm.Prioridade = xml.Prioridade;

            if (xml.ValorTotal.HasValue)
                crm.ValorTotal = xml.ValorTotal;
            else
                crm.AddNullProperty("ValorTotal");

            if (xml.ValorTotalImpostos.HasValue)
                crm.TotalImpostos = xml.ValorTotalImpostos;
            else
                crm.AddNullProperty("TotalImpostos");

            //if (xml.ValorTotalProdutosSemImposto.HasValue)
            //    crm.ValorTotalProdutosSemIPIST = xml.ValorTotalProdutosSemImposto;

            if (xml.ValorTotalSemFrete.HasValue)
                crm.ValorTotalSemFrete = xml.ValorTotalSemFrete;
            else
                crm.AddNullProperty("ValorTotalSemFrete");

            if (xml.ValorTotalDesconto.HasValue)
                crm.DescontoTotal = xml.ValorTotalDesconto;
            else
                crm.AddNullProperty("DescontoTotal");

            if (xml.ValorTotalProdutos.HasValue)
                crm.TotalProdutosComIPIST = xml.ValorTotalProdutos;
            else
                crm.AddNullProperty("TotalProdutosComIPIST");

            if (xml.ValorTotalProdutosSemImposto.HasValue)
                crm.ValorTotalProdutosSemIPIST = xml.ValorTotalProdutosSemImposto;
            else
                crm.AddNullProperty("ValorTotalProdutosSemIPIST");

            //Service Endereco Entrega

            if (!string.IsNullOrEmpty(xml.EnderecoEntrega.CaixaPostal))
                crm.EnderecoEntregaCaixaPostal = xml.EnderecoEntrega.CaixaPostal;
            else
                crm.AddNullProperty("EnderecoEntregaCaixaPostal");

            if (!string.IsNullOrEmpty(xml.EnderecoEntrega.NomeEndereco))
                crm.NomeEntrega = xml.EnderecoEntrega.NomeEndereco;
            else
                crm.AddNullProperty("NomeEntrega");

            crm.CEPEntrega = xml.EnderecoEntrega.CEP;
            crm.EnderecoEntregaRua = xml.EnderecoEntrega.Logradouro;
            crm.EnderecoEntregaNumero = xml.EnderecoEntrega.Numero;
            crm.BairroEntrega = xml.EnderecoEntrega.Bairro;
            crm.ComplementoEntrega = xml.EnderecoEntrega.Complemento;
            crm.TipoNotaFiscal = xml.TipoNotaFiscal;
            crm.NotaDevolucao = xml.NotaDevolucao;
            crm.IdentificadorUnicoNfe = xml.IdentificadorUnicoNFE;

            if (!string.IsNullOrEmpty(xml.EnderecoEntrega.Complemento))
                crm.ComplementoEntrega = xml.EnderecoEntrega.Complemento;
            else
                crm.AddNullProperty("ComplementoEntrega");


            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.Cidade))
            {
                Model.Municipio cidade = new Model.Municipio(this.Organizacao, this.IsOffline);
                cidade = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaMunicipio(xml.EnderecoEntrega.Cidade);

                if (cidade != null && cidade.ID.HasValue)
                    crm.EnderecoEntregaCidade = new Lookup(cidade.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Identificador da Cidade não encontrado!";
                    return crm;
                }
            }

            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.Estado))
            {
                Model.Estado estado = new Model.Estado(this.Organizacao, this.IsOffline);
                estado = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaEstado(xml.EnderecoEntrega.Estado);

                if (estado != null && estado.ID.HasValue)
                    crm.EnderecoEntregaEstado = new Lookup(estado.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Identificador do Estado não encontrado!";
                    return crm;
                }
            }

            ///Service Pais
            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.Pais))
            {
                Model.Pais pais = new Model.Pais(this.Organizacao, this.IsOffline);
                pais = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaPais(xml.EnderecoEntrega.Pais);

                if (pais != null && pais.ID.HasValue)
                    crm.EnderecoEntregaPais = new Lookup(pais.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Identificador do País não encontrado.";
                    return crm;
                }
            }

            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.NomeContato))
                crm.NomeEntrega = xml.EnderecoEntrega.NomeContato;
            else
                crm.AddNullProperty("NomeEntrega");


            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.Telefone))
                crm.TelefoneEntrega = xml.EnderecoEntrega.Telefone;
            else
                crm.AddNullProperty("TelefoneEntrega");


            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.Fax))
                crm.FaxEntrega = xml.EnderecoEntrega.Fax;
            else
                crm.AddNullProperty("FaxEntrega");


            #region Representante

            crm.IntegradoRepresentanteComErro = false;
            crm.IntegradoComErros = false;
            crm.RepresentanteOriginal = xml.Representante.ToString();

            Contato ObjRepresentante = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline)
                .BuscaContatoPorCodigoRepresentante(xml.Representante.ToString());

            if (ObjRepresentante != null)
            {
                crm.KARepresentante = new Lookup(ObjRepresentante.ID.Value, "");
            }
            else
            {
                crm.IntegradoRepresentanteComErro = true;
                crm.IntegradoComErros = true;
                crm.KARepresentante = new Lookup(ObterRepresentatePadrao(), "");
            }

            #endregion

            return crm;
        }

        private void DefinirPropriedadesProdutoFatura(Intelbras.Message.Helper.MSG0094 xml, Guid FaturaID)
        {
            ProdutoFatura crmProdFatura = null;

            foreach (Pollux.Entities.NotaFiscalItem item in xml.NotaFiscalItens)
            {
                crmProdFatura = new ProdutoFatura(this.Organizacao, this.IsOffline);

                crmProdFatura.ChaveIntegracao = item.ChaveIntegracao;
                crmProdFatura.Fatura = new Lookup(FaturaID, "");

                if (!String.IsNullOrEmpty(item.CodigoProduto))
                {
                    Model.Product ObjProduto = new Product(this.Organizacao, this.IsOffline);
                    ObjProduto = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(this.Organizacao, this.IsOffline).BuscaPorCodigo(item.CodigoProduto);

                    if (ObjProduto != null && ObjProduto.ID.HasValue)
                        crmProdFatura.ProdutoId = new Lookup(ObjProduto.ID.Value, "");
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Identificador do Produto não encontrado.";
                        return;
                    }
                }

                if (!String.IsNullOrEmpty(item.Descricao))
                    crmProdFatura.Descricao = item.Descricao;
                else
                    crmProdFatura.AddNullProperty("Descricao");

                crmProdFatura.PrecoOriginal = item.PrecoOriginal;
                crmProdFatura.ValorLiquido = item.PrecoUnitario;
                crmProdFatura.PrecoLiquido = item.PrecoLiquido;
                crmProdFatura.ValorMercadoriaTabela = item.ValorMercadoriaTabela;
                crmProdFatura.ValorMercadoriaOriginal = item.ValorMercadoriaOriginal;
                crmProdFatura.ValorMercadoriaLiquida = item.ValorMercadoriaLiquido;
                crmProdFatura.Precificacao = item.PermiteSubstituirPreco;

                //Service Natureza Operação
                NaturezaOperacao ObjNaturezaOperacao = new Intelbras.CRM2013.Domain.Servicos.NaturezaOperacaoService(this.Organizacao, this.IsOffline).BuscaNaturezaOperacaoPorCodigo(item.CodigoNaturezaOperacao);

                if (ObjNaturezaOperacao != null)
                {
                    crmProdFatura.NaturezaOperacao = new Lookup((Guid)ObjNaturezaOperacao.ID, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Identificador da Natureza de Operação não encontrado.";
                    return;
                }

                crmProdFatura.SelecionarProduto = item.ProdutoForaCatalogo;

                if (!String.IsNullOrEmpty(item.DescricaoProdutoForaCatalogo))
                    crmProdFatura.DescricaoProdutoSemCatalogo = item.DescricaoProdutoForaCatalogo;
                else
                    crmProdFatura.AddNullProperty("DescricaoProdutoSemCatalogo");

                crmProdFatura.Precificacao = item.PermiteSubstituirPreco;

                //Service Unidade Medida
                Unidade ObjUnidadeMedida = new Intelbras.CRM2013.Domain.Servicos.UnidadeService(this.Organizacao, this.IsOffline).BuscaUnidadePorNome(item.UnidadeMedida);

                if (ObjUnidadeMedida != null)
                {
                    crmProdFatura.Unidade = new Lookup((Guid)ObjUnidadeMedida.ID, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Identificador da Unidade de Medida não encontrado.";
                    return;
                }

                if (item.ValorBaseICMS.HasValue)
                    crmProdFatura.ValorBaseICMS = item.ValorBaseICMS;
                else
                    crmProdFatura.AddNullProperty("ValorBaseICMS");

                if (item.ValorBaseICMSSubstituicao.HasValue)
                    crmProdFatura.ValorSubstTributaria = item.ValorBaseICMSSubstituicao;
                else
                    crmProdFatura.AddNullProperty("ValorSubstTributaria");

                if (item.ValorICMS.HasValue)
                    crmProdFatura.ValorICMSItem = item.ValorICMS;
                else
                    crmProdFatura.AddNullProperty("ValorICMSItem");

                if (item.ValorICMSSubstituicao.HasValue)
                    crmProdFatura.ValorICMSSubstTributaria = item.ValorICMSSubstituicao;
                else
                    crmProdFatura.AddNullProperty("ValorICMSSubstTributaria");

                if (item.ValorICMSNaoTributado.HasValue)
                    crmProdFatura.ValorICMSNaoTributado = item.ValorICMSNaoTributado;
                else
                    crmProdFatura.AddNullProperty("ValorICMSNaoTributado");

                if (item.ValorICMSOutras.HasValue)
                    crmProdFatura.ValorICMSOutras = item.ValorICMSOutras;
                else
                    crmProdFatura.AddNullProperty("ValorICMSOutras");

                crmProdFatura.CodigoTributarioICMS = item.CodigoTributarioICMS;

                if (!String.IsNullOrEmpty(item.CodigoTributarioISS))
                    crmProdFatura.CodigoTributarioISS = item.CodigoTributarioISS;
                else
                    crmProdFatura.AddNullProperty("CodigoTributarioISS");

                crmProdFatura.CodigoTributarioIPI = item.CodigoTributarioIPI;

                if (item.ValorBaseIPI.HasValue)
                    crmProdFatura.ValorBaseIPI = item.ValorBaseIPI;
                else
                    crmProdFatura.AddNullProperty("ValorBaseIPI");

                if (item.AliquotaISS.HasValue)
                    crmProdFatura.AliquotaISS = (double)item.AliquotaISS;
                else
                    crmProdFatura.AddNullProperty("AliquotaISS");

                crmProdFatura.CodigoTributarioIPI = item.CodigoTributarioIPI;
                crmProdFatura.ValorBaseISS = item.ValorBaseISS;
                crmProdFatura.AliquotaIPI = (double)item.AliquotaIPI;
                crmProdFatura.AliquotaICMS = (double)item.AliquotaICMS;
                crmProdFatura.ValorISSItem = item.ValorISS;
                crmProdFatura.ValorISSNaoTributado = item.ValorISSNaoTributado;
                crmProdFatura.ValorISSOutras = item.ValorISSOutras;

                crmProdFatura.CalcularRebate = item.CalcularRebate;

                if (item.ValorIPI.HasValue)
                    crmProdFatura.ValorIPIItem = item.ValorIPI;
                else
                    crmProdFatura.AddNullProperty("ValorIPIItem");

                if (item.ValorIPINaoTributado.HasValue)
                    crmProdFatura.ValorIPINaoTributado = item.ValorIPINaoTributado;
                else
                    crmProdFatura.AddNullProperty("ValorIPINaoTributado");

                if (item.ValorIPIOutras.HasValue)
                    crmProdFatura.ValorIPIOutras = item.ValorIPIOutras;
                else
                    crmProdFatura.AddNullProperty("ValorIPIOutras");

                if (item.PrecoConsumidor.HasValue)
                    crmProdFatura.PrecoConsumidor = item.PrecoConsumidor;
                else
                    crmProdFatura.AddNullProperty("PrecoConsumidor");

                if (item.QuantidadeCancelada.HasValue)
                    crmProdFatura.QtdeCancelada = item.QuantidadeCancelada;
                else
                    crmProdFatura.AddNullProperty("QtdeCancelada");

                if (item.QuantidadePendente.HasValue)
                    crmProdFatura.QtdePedidoPendente = item.QuantidadePendente;
                else
                    crmProdFatura.AddNullProperty("QtdePedidoPendente");

                if (item.DataEntrega.HasValue)
                    crmProdFatura.Entregueem = item.DataEntrega;
                else
                    crmProdFatura.AddNullProperty("Entregueem");

                if (item.CondicaoFrete.HasValue)
                    crmProdFatura.CondicoesFrete = item.CondicaoFrete;
                else
                    crmProdFatura.AddNullProperty("CondicoesFrete");

                if (item.CalcularRebate.HasValue)
                {
                    //TODO: Registrar valor de CalcularRebate no Item da nota
                }

                if (item.ValorOriginal.HasValue)
                    crmProdFatura.ValorOriginal = item.ValorOriginal;
                else
                    crmProdFatura.AddNullProperty("ValorOriginal");

                crmProdFatura.TotalImpostos = item.ValorTotalImposto;
                crmProdFatura.DescontoManual = item.ValorDescontoManual;
                crmProdFatura.Quantidade = item.Quantidade;
                crmProdFatura.Remessa = item.RetiraNoLocal;

                if (item.QuantidadeEntregue.HasValue)
                    crmProdFatura.QtdeEntregue = item.QuantidadeEntregue;
                else
                    crmProdFatura.AddNullProperty("QtdeEntregue");

                if (item.NumeroSequencia.HasValue)
                    crmProdFatura.NumeroSequencia = item.NumeroSequencia;
                else
                    crmProdFatura.AddNullProperty("NumeroSequencia");

                //Service Unidade Negócio
                UnidadeNegocio ObjUnidadeNegocio = new Intelbras.CRM2013.Domain.Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocioPorChaveIntegracao(item.CodigoUnidadeNegocio);

                if (ObjUnidadeNegocio != null)
                {
                    crmProdFatura.UnidadeNegocio = new Lookup((Guid)ObjUnidadeNegocio.ID, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Identificador da Unidade de Negócio não encontrado.";
                    return;
                }

                #region Representante

                crmProdFatura.IntegradoRepresentanteComErro = false;
                crmProdFatura.RepresentanteOriginal = item.CodigoRepresentante.ToString();

                Contato ObjRepresentante = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline)
                    .BuscaContatoPorCodigoRepresentante(item.CodigoRepresentante.ToString());

                if (ObjRepresentante != null)
                {
                    crmProdFatura.Representante = new Lookup(ObjRepresentante.ID.Value, "");
                }
                else
                {
                    crmProdFatura.IntegradoRepresentanteComErro = true;
                    crmProdFatura.Representante = new Lookup(ObterRepresentatePadrao(), string.Empty);
                }

                #endregion

                //Service Moeda
                Moeda ObjMoeda = new Intelbras.CRM2013.Domain.Servicos.MoedaService(this.Organizacao, this.IsOffline).BuscaMoedaPorCodigo(xml.Moeda);

                if (ObjMoeda != null)
                {
                    crmProdFatura.Moeda = new Lookup((Guid)ObjMoeda.ID, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Identificador da Moeda não encontrado.";
                    return;
                }

                crmProdFatura.RazaoStatus = 1;

                string acaoItemFatura = item.Acao;

                if (!string.IsNullOrEmpty(item.EnderecoEntrega.NomeEndereco))
                    crmProdFatura.NomeEntrega = item.EnderecoEntrega.NomeEndereco;
                else
                    crmProdFatura.AddNullProperty("NomeEntrega");

                crmProdFatura.CEPEntrega = item.EnderecoEntrega.CEP;
                crmProdFatura.Rua1Entrega = item.EnderecoEntrega.Logradouro;

                if (!string.IsNullOrEmpty(item.EnderecoEntrega.Complemento))
                    crmProdFatura.Rua2Entrega = item.EnderecoEntrega.Complemento;
                else
                    crmProdFatura.AddNullProperty("Rua2Entrega");

                crmProdFatura.Rua3Entrega = item.EnderecoEntrega.Bairro;
                crmProdFatura.CidadeEntrega = item.EnderecoEntrega.NomeCidade;

                if (!String.IsNullOrEmpty(item.EnderecoEntrega.Estado))
                {

                    crmProdFatura.EstadoEntrega = item.EnderecoEntrega.Estado;
                }

                if (!String.IsNullOrEmpty(item.EnderecoEntrega.Pais))
                {
                    crmProdFatura.PaisEntrega = item.EnderecoEntrega.Pais;
                }

                if (!String.IsNullOrEmpty(item.EnderecoEntrega.Telefone))
                    crmProdFatura.TelefoneEntrega = item.EnderecoEntrega.Telefone;
                else
                    crmProdFatura.AddNullProperty("TelefoneEntrega");

                if (!String.IsNullOrEmpty(item.EnderecoEntrega.Fax))
                    crmProdFatura.FAXEntrega = item.EnderecoEntrega.Fax;
                else
                    crmProdFatura.AddNullProperty("FAXEntrega");

                if (acaoItemFatura == "A" || acaoItemFatura == "I")
                    lstProdutoFaturaUpdate.Add(crmProdFatura);
                if (acaoItemFatura == "E")
                    lstProdutoFaturaDelete.Add(crmProdFatura);
            }

        }

        private Guid ObterRepresentatePadrao()
        {
            string representanteIdConfig = SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.Pedido.RepresentantePadrao", true);
            return new Guid(representanteIdConfig);
        }

        #endregion

        public string Enviar(Fatura objModel)
        {
            throw new NotImplementedException();
        }
    }
}