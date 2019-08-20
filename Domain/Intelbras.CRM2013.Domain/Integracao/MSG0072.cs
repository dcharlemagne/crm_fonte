using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0072 : Base, IBase<Intelbras.Message.Helper.MSG0072, Domain.Model.Conta>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private string tipoProprietario;
        private Domain.Model.Usuario usuarioIntegracao;
        #endregion

        #region Construtor
        public MSG0072(string org, bool isOffline)
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
            //Trace.Add("Mensagem " + numeroMensagem + " XML: {0}", mensagem);
            usuarioIntegracao = usuario;
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0072>(mensagem));

            //Teste do post plugin, feito pelo messi, foi parado para priorizar outras atividades e voltarei aqui mais tarde
            //string xmlResposta = new Domain.Servicos.ContaService(this.Organizacao, this.IsOffline).IntegracaoBarramento(objeto);

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", this.resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0072R1>(numeroMensagem, retorno);
            }

            objeto = new Intelbras.CRM2013.Domain.Servicos.ContaService(this.Organizacao, this.IsOffline).Persistir(objeto);

            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro ao persisrtir conta.";
                return CriarMensagemRetorno<Pollux.MSG0072R1>(numeroMensagem, retorno);
            }

            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";

            if (objeto.ID.HasValue)
            {
                retorno.Add("CodigoConta", objeto.ID.Value.ToString());

                Domain.Model.Usuario proprietario = new Domain.Servicos.UsuarioService(this.Organizacao, this.IsOffline).BuscarProprietario("account", "accountid", objeto.ID.Value);
                if (proprietario != null)
                    retorno.Add("Proprietario", proprietario.Id.ToString());
            }

            retorno.Add("TipoProprietario", "systemuser");

            retorno.Add("Resultado", resultadoPersistencia);


            return CriarMensagemRetorno<Pollux.MSG0072R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public Conta DefinirPropriedades(Intelbras.Message.Helper.MSG0072 xml)
        {
            var crm = new Conta(this.Organizacao, this.IsOffline);


            #region Propriedades Xml->Crm

            //Controle Looping Msg
            crm.IntegrarNoPlugin = true;

            if (!String.IsNullOrEmpty(xml.CodigoConta))
                crm.ID = new Guid(xml.CodigoConta);

            if (xml.CodigoCliente.HasValue)
                crm.CodigoMatriz = xml.CodigoCliente.Value.ToString();

            crm.RazaoSocial = xml.NomeRazaoSocial;

            if (!String.IsNullOrEmpty(xml.Regiao))
            {
                Regiao regiao = new Servicos.RegiaoService(this.Organizacao, this.IsOffline).ObterPorNome(xml.Regiao);
                if (regiao != null)
                {
                    crm.Regiao = new Lookup(regiao.ID.Value, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Região :" + xml.Regiao + " - não cadastrada no Crm.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("Regiao");
            }

            if (xml.NomeFantasia != "")
                crm.NomeFantasia = xml.NomeFantasia;

            if (!string.IsNullOrEmpty(xml.NomeAbreviado))
                crm.NomeAbreviado = xml.NomeAbreviado;
            else
                crm.AddNullProperty("NomeAbreviado");

            crm.TipoConta = xml.TipoConta;

            if (xml.ApuracaoBeneficio.HasValue)
                crm.ApuracaoBeneficiosCompromissos = xml.ApuracaoBeneficio;

            if (!String.IsNullOrEmpty(xml.DescricaoConta))
                crm.Descricao = xml.DescricaoConta;
            else
                crm.AddNullProperty("Descricao");

            crm.TipoRelacao = xml.TipoRelacao;

            if (!String.IsNullOrEmpty(xml.NumeroBanco))
                crm.Banco = xml.NumeroBanco;
            else
                crm.AddNullProperty("Banco");

            if (!String.IsNullOrEmpty(xml.NumeroAgencia))
                crm.Agencia = xml.NumeroAgencia;
            else
                crm.AddNullProperty("Agencia");

            if (!String.IsNullOrEmpty(xml.NumeroContaCorrente))
                crm.ContaCorrente = xml.NumeroContaCorrente;
            else
                crm.AddNullProperty("ContaCorrente");

            if (xml.EmiteBloqueto.HasValue)
                crm.EmiteBloqueto = xml.EmiteBloqueto;
            else
                crm.AddNullProperty("EmiteBloqueto");

            if (xml.GeraAvisoCredito.HasValue)
                crm.GeraAvisoCredito = xml.GeraAvisoCredito;
            else
                crm.AddNullProperty("GeraAvisoCredito");

            if (xml.CalculaMulta.HasValue)
                crm.CalculaMulta = xml.CalculaMulta;
            else
                crm.AddNullProperty("CalculaMulta");

            if (xml.RecebeInformacaoSCI.HasValue)
                crm.RecebeInformacaoSCI = xml.RecebeInformacaoSCI;
            else
                crm.AddNullProperty("RecebeInformacaoSCI");

            if (!string.IsNullOrEmpty(xml.Telefone))
                crm.Telefone = xml.Telefone;
            else
            {
                crm.AddNullProperty("Telefone");
            }

            if (xml.ParticipaProgramaCanais != null)
                crm.ParticipantePrograma = xml.ParticipaProgramaCanais;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Campo Participa Programa Canais não informado, por favor verifique o conteúdo";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.Ramal))
                crm.RamalTelefonePrincipal = xml.Ramal;
            else
                crm.AddNullProperty("RamalTelefonePrincipal");

            if (!String.IsNullOrEmpty(xml.TelefoneAlternativo))
                crm.TelefoneAlternativo = xml.TelefoneAlternativo;
            else
                crm.AddNullProperty("TelefoneAlternativo");

            if (!String.IsNullOrEmpty(xml.RamalTelefoneAlternativo))
                crm.RamalOutroTelefone = xml.RamalTelefoneAlternativo;
            else
                crm.AddNullProperty("RamalOutroTelefone");

            if (!String.IsNullOrEmpty(xml.Fax))
                crm.Fax = xml.Fax;
            else
                crm.AddNullProperty("Fax");

            if (!String.IsNullOrEmpty(xml.RamalFax))
                crm.RamalFax = xml.RamalFax;
            else
                crm.AddNullProperty("RamalFax");

            if (!string.IsNullOrEmpty(xml.Email))
                crm.Email = xml.Email;
            else
            {
                crm.AddNullProperty("Email");
            }

            if (!String.IsNullOrEmpty(xml.Site))
                crm.Site = xml.Site;
            else
                crm.AddNullProperty("Site");

            crm.Natureza = xml.Natureza;

            crm.TipoConstituicao = xml.TipoConstituicao;

            switch (crm.TipoConstituicao)
            {
                case (int)Domain.Enum.Conta.TipoConstituicao.Cnpj:
                    if (!String.IsNullOrEmpty(xml.CNPJ))
                    {
                        crm.CpfCnpj = Intelbras.CRM2013.Domain.Servicos.Helper.FormatarCnpj(xml.CNPJ);
                    }
                    else
                        crm.AddNullProperty("CpfCnpj");
                    break;

                case (int)Domain.Enum.Conta.TipoConstituicao.Cpf:
                    if (!String.IsNullOrEmpty(xml.CPF))
                    {
                        crm.CpfCnpj = Intelbras.CRM2013.Domain.Servicos.Helper.FormatarCpf(xml.CPF);
                    }
                    else
                        crm.AddNullProperty("CpfCnpj");
                    break;

                case (int)Domain.Enum.Conta.TipoConstituicao.Estrangeiro:
                    if (!String.IsNullOrEmpty(xml.CodigoEstrangeiro))
                    {
                        crm.CpfCnpj = xml.CodigoEstrangeiro;
                    }
                    else
                        crm.AddNullProperty("CpfCnpj");
                    break;
            }

            if (!String.IsNullOrEmpty(xml.InscricaoEstadual))
                crm.InscricaoEstadual = xml.InscricaoEstadual;
            else
                crm.AddNullProperty("InscricaoEstadual");

            if (!String.IsNullOrEmpty(xml.InscricaoMunicipal))
                crm.InscricaoMunicipal = xml.InscricaoMunicipal;
            else
                crm.AddNullProperty("InscricaoMunicipal");

            if (xml.SuspensaoCredito.HasValue)
                crm.SuspensaoCredito = xml.SuspensaoCredito;
            else
                crm.AddNullProperty("SuspensaoCredito");

            if (xml.LimiteCredito.HasValue)
                crm.LimiteCredito = xml.LimiteCredito;
            else
                crm.AddNullProperty("LimiteCredito");

            if (xml.DataLimiteCredito.HasValue)
                crm.DataLimiteCredito = xml.DataLimiteCredito;
            else
                crm.AddNullProperty("DataLimiteCredito");

            if (xml.DataAdesao.HasValue)
                crm.DataAdesao = xml.DataAdesao;

            if (xml.SaldoCredito.HasValue)
                crm.SaldoCredito = xml.SaldoCredito;
            else
                crm.AddNullProperty("SaldoCredito");

            if (xml.ModalidadeCobranca.HasValue)
                crm.Modalidade = xml.ModalidadeCobranca;
            else
                crm.AddNullProperty("Modalidade");

            if (xml.ContribuinteICMS.HasValue)
                crm.ContribuinteICMS = xml.ContribuinteICMS;
            else
                crm.AddNullProperty("ContribuinteICMS");

            if (!String.IsNullOrEmpty(xml.CodigoSUFRAMA))
                crm.CodigoSuframa = xml.CodigoSUFRAMA;
            else
                crm.AddNullProperty("CodigoSuframa");

            if (!String.IsNullOrEmpty(xml.InscricaoSubstituicaoTributaria))
                crm.SubstituicaoTributaria = xml.InscricaoSubstituicaoTributaria;
            else
                crm.AddNullProperty("SubstituicaoTributaria");

            if (xml.OptanteSuspensaoIPI.HasValue)
                crm.OptanteSuspensaoIPI = xml.OptanteSuspensaoIPI;
            else
                crm.AddNullProperty("OptanteSuspensaoIPI");

            if (xml.AgenteRetencao.HasValue)
                crm.AgenteRetencao = xml.AgenteRetencao;
            else
                crm.AddNullProperty("AgenteRetencao");

            if (xml.PisCofinsUnidade.HasValue)
                crm.PISCOFINSPorUnidade = xml.PisCofinsUnidade;
            else
                crm.AddNullProperty("PISCOFINSPorUnidade");

            if (xml.RecebeNotaFiscalEletronica.HasValue)
                crm.RecebeNFE = xml.RecebeNotaFiscalEletronica;
            else
                crm.AddNullProperty("RecebeNFE");

            if (xml.FormaTributacao.HasValue)
                crm.FormaTributacao = xml.FormaTributacao;
            else
                crm.AddNullProperty("FormaTributacao");

            if (!String.IsNullOrEmpty(xml.ObservacaoPedido))
                crm.ObservacoesPedido = xml.ObservacaoPedido;
            else
                crm.AddNullProperty("ObservacoesPedido");

            if (!String.IsNullOrEmpty(xml.TipoEmbalagem))
                crm.TipoEmbalagem = xml.TipoEmbalagem;
            else
                crm.AddNullProperty("TipoEmbalagem");

            if (!String.IsNullOrEmpty(xml.CodigoIncoterm))
                crm.Incoterm = xml.CodigoIncoterm;
            else
                crm.AddNullProperty("Incoterm");

            if (!String.IsNullOrEmpty(xml.LocalEmbarque))
                crm.LocalEmbarque = xml.LocalEmbarque;
            else
                crm.AddNullProperty("LocalEmbarque");

            if (!String.IsNullOrEmpty(xml.ViaEmbarque))
                crm.EmbarqueVia = xml.ViaEmbarque;
            else
                crm.AddNullProperty("EmbarqueVia");

            if (xml.DataImplantacao.HasValue)
                crm.DataImplantacao = xml.DataImplantacao;
            else
                crm.AddNullProperty("DataImplantacao");

            if (!String.IsNullOrEmpty(xml.RG))
                crm.DocIdentidade = xml.RG;
            else
                crm.AddNullProperty("DocIdentidade");

            if (!String.IsNullOrEmpty(xml.OrgaoExpeditor))
                crm.EmissorIdentidade = xml.OrgaoExpeditor;
            else
                crm.AddNullProperty("EmissorIdentidade");

            if (xml.DataVencimentoConcessao.HasValue)
                crm.DataVenctoConcessao = xml.DataVencimentoConcessao;
            else
                crm.AddNullProperty("DataVenctoConcessao");

            if (xml.DescontoAssistenciaTecnica.HasValue)
                crm.DescontoCAT = xml.DescontoAssistenciaTecnica;
            else
                crm.AddNullProperty("DescontoCAT");

            if (!String.IsNullOrEmpty(xml.CoberturaGeografica))
                crm.CoberturaGeografica = xml.CoberturaGeografica;
            else
                crm.AddNullProperty("CoberturaGeografica");

            if (xml.DataConstituicao.HasValue)
                crm.DataConstituicao = xml.DataConstituicao;
            else
                crm.AddNullProperty("DataConstituicao");

            if (xml.DistribuicaoUnicaFonteReceita.HasValue)
                crm.DistribuicaoFonteReceita = xml.DistribuicaoUnicaFonteReceita;
            else
                crm.AddNullProperty("DistribuicaoFonteReceita");

            if (!String.IsNullOrEmpty(xml.QualificadoTreinamento))
                crm.EspacoFisicoQualificado = xml.QualificadoTreinamento;
            else
                crm.AddNullProperty("EspacoFisicoQualificado");

            if (xml.CondicaoPagamento.HasValue)
            {
                CondicaoPagamento condicaoPagto = new Servicos.CondicaoPagamentoService(this.Organizacao, this.IsOffline).BuscaCondicaoPagamentoPorCodigo(xml.CondicaoPagamento.Value);
                if (condicaoPagto != null)
                {
                    crm.CondicaoPagamento = new Lookup(condicaoPagto.ID.Value, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Condição Pagamento codigo : " + xml.CondicaoPagamento.Value.ToString() + " não cadastrado no Crm.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("CondicaoPagamento");
            }

            if (!String.IsNullOrEmpty(xml.Historico))
                crm.Historico = xml.Historico;
            else
                crm.AddNullProperty("Historico");

            if (!String.IsNullOrEmpty(xml.IntencaoApoio))
                crm.IntencaoApoio = xml.IntencaoApoio;
            else
                crm.AddNullProperty("IntencaoApoio");

            if (xml.FiguraNoSite.HasValue)
                crm.FiguraNoSite = xml.FiguraNoSite;
            else
                crm.AddNullProperty("FiguraNoSite");

            if (xml.ParticipaProgramaCanaisMotivo.HasValue)
                crm.MotivoParticipantePrograma = xml.ParticipaProgramaCanaisMotivo;
            else
                crm.AddNullProperty("MotivoParticipantePrograma");

            if (!string.IsNullOrEmpty(xml.AdesaoPciRealizadaPor))
                crm.AdesaoAoPCIRealizadaPor = xml.AdesaoPciRealizadaPor;
            else
                crm.AddNullProperty("AdesaoAoPCIRealizadaPor");

            if (xml.EscolheuDistrForaSellOut.HasValue)
                crm.EscolheuDistribuidorSemSellout = xml.EscolheuDistrForaSellOut;
            else
                crm.AddNullProperty("EscolheuDistribuidorSemSellout");

            if (xml.DataUltimoSellOut.HasValue)
                crm.DataUltimoSelloutRevenda = xml.DataUltimoSellOut;
            else
                crm.AddNullProperty("DataUltimoSelloutRevenda");

            if (!String.IsNullOrEmpty(xml.MetodoComercializacao))
                crm.MetodoComercializacaoProduto = xml.MetodoComercializacao;
            else
                crm.AddNullProperty("MetodoComercializacaoProduto");

            if (!String.IsNullOrEmpty(xml.ModeloOperacao))
                crm.ModeloOperacaoFiliais = xml.ModeloOperacao;
            else
                crm.AddNullProperty("ModeloOperacaoFiliais");

            if (xml.NumeroFuncionarios.HasValue)
                crm.NumeroFuncionarios = xml.NumeroFuncionarios;
            else
                crm.AddNullProperty("NumeroFuncionarios");

            if (xml.NumeroColaboradoresAreaTecnica.HasValue)
                crm.NumeroColaboradores = xml.NumeroColaboradoresAreaTecnica;
            else
                crm.AddNullProperty("NumeroColaboradores");

            if (xml.NumeroRevendasAtivas.HasValue)
                crm.NumeroRevendasAtivas = xml.NumeroRevendasAtivas;
            else
                crm.AddNullProperty("NumeroRevendasAtivas");

            if (!String.IsNullOrEmpty(xml.NumeroRevendasInativas))
                crm.NumeroRevendasInativas = xml.NumeroRevendasInativas;
            else
                crm.AddNullProperty("NumeroRevendasInativas");

            if (xml.NumeroTecnicosSuporte.HasValue)
                crm.NumeroTecnicosSuporte = xml.NumeroTecnicosSuporte;
            else
                crm.AddNullProperty("NumeroTecnicosSuporte");

            if (xml.NumeroVendedores.HasValue)
                crm.NumeroVendedores = xml.NumeroVendedores;
            else
                crm.AddNullProperty("NumeroVendedores");

            if (!String.IsNullOrEmpty(xml.OutraFonteReceita))
                crm.OutraFonteReceita = xml.OutraFonteReceita;
            else
                crm.AddNullProperty("OutraFonteReceita");

            if (!String.IsNullOrEmpty(xml.NomeAbreviadoMatrizEconomica))
                crm.NomeAbreviadoMatrizEconomica = xml.NomeAbreviadoMatrizEconomica;
            else
                crm.AddNullProperty("NomeAbreviadoMatrizEconomica");

            if (!String.IsNullOrEmpty(xml.PerfilRevendasDistribuidor))
                crm.PerfilRevendaDistribuidor = xml.PerfilRevendasDistribuidor;
            else
                crm.AddNullProperty("PerfilRevendaDistribuidor");

            if (xml.PossuiEstruturaCompleta.HasValue)
                crm.PossuiEstruturaCompleta = xml.PossuiEstruturaCompleta;
            else
                crm.AddNullProperty("PossuiEstruturaCompleta");

            if (xml.PossuiFiliais.HasValue)
                crm.PossuiFiliais = xml.PossuiFiliais;
            else
                crm.AddNullProperty("PossuiFiliais");

            if (xml.QuantidadeFiliais.HasValue)
                crm.QuantasFiliais = xml.QuantidadeFiliais;
            else
                crm.AddNullProperty("QuantasFiliais");

            if (xml.PrazoMedioCompra.HasValue)
                crm.PrazoMedioCompras = (double)xml.PrazoMedioCompra;
            else
                crm.AddNullProperty("PrazoMedioCompras");

            if (xml.PrazoMedioVenda.HasValue)
                crm.PrazoMedioVendas = (double)xml.PrazoMedioVenda;
            else
                crm.AddNullProperty("PrazoMedioVendas");

            if (xml.Setor.HasValue)
                crm.Setor = xml.Setor;
            else
                crm.AddNullProperty("Setor");

            if (!string.IsNullOrEmpty(xml.RamoAtividadeEconomica))
                crm.RamoAtividadeEconomica = xml.RamoAtividadeEconomica;
            else
                crm.AddNullProperty("RamoAtividadeEconomica");

            if (!string.IsNullOrEmpty(xml.CNAE))
                crm.CNAE = xml.CNAE;
            else
                crm.AddNullProperty("CNAE");

            if (!string.IsNullOrEmpty(xml.CodigoRamoAtividadeEconomica))
            {
                crm.CnaeId = new Lookup(new Guid(xml.CodigoRamoAtividadeEconomica), SDKore.Crm.Util.Utility.GetEntityName<CNAE>());
            }
            else
            {
                if (!string.IsNullOrEmpty(xml.CNAE))
                {
                    var cnae = new CnaeService(this.Organizacao, this.IsOffline).ObterPor(xml.CNAE);

                    if (cnae == null)
                    {
                        new ArgumentException("(CRM) Não encontrado o CNAE, é necessário preencher o campo 'CNAE' com o valor correto ou preencher o campo 'CodigoRamoAtividadeEconomica'");
                    }
                    else
                    {
                        crm.CnaeId = new Lookup(cnae.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(cnae));
                    }
                }
                else
                {
                    crm.AddNullProperty("CnaeId");
                }
            }

            if (xml.ValorMedioCompra.HasValue)
                crm.ValorMedioComprasMensais = xml.ValorMedioCompra;
            else
                crm.AddNullProperty("ValorMedioComprasMensais");

            if (xml.ValorMedioVenda.HasValue)
                crm.ValorMedioVendasMensais = xml.ValorMedioVenda;
            else
                crm.AddNullProperty("ValorMedioVendasMensais");

            if (xml.VendeAtacadista.HasValue)
                crm.VendasParaAtacadistaVarejista = xml.VendeAtacadista;
            else
                crm.AddNullProperty("VendasParaAtacadistaVarejista");

            if (!String.IsNullOrEmpty(xml.ObservacaoNotaFiscal))
                crm.ObservacoesNF = xml.ObservacaoNotaFiscal;
            else
                crm.AddNullProperty("ObservacoesNF");

            if (xml.EstruturaPropriedade.HasValue)
                crm.Propriedade = xml.EstruturaPropriedade;
            else
                crm.AddNullProperty("Propriedade");

            if (xml.ReceitaAnual.HasValue)
                crm.ReceitaAnual = xml.ReceitaAnual;
            else
                crm.AddNullProperty("ReceitaAnual");

            crm.Status = xml.Situacao;

            if (!String.IsNullOrEmpty(xml.Classificacao))
                crm.Classificacao = new Lookup(new Guid(xml.Classificacao), "");

            if (!String.IsNullOrEmpty(xml.SubClassificacao))
                crm.Subclassificacao = new Lookup(new Guid(xml.SubClassificacao), "");

            if (xml.PercentualComissaoSolar.HasValue)
                crm.PercentualComissaoProjeto = xml.PercentualComissaoSolar;

            if (xml.Portador.HasValue)
                crm.Portador = new Lookup(new Intelbras.CRM2013.Domain.Servicos.PortadorService(this.Organizacao, this.IsOffline).BuscaPorCodigo(xml.Portador.Value).ID.Value, "");
            else
                crm.AddNullProperty("Portador");

            if (!String.IsNullOrEmpty(xml.NivelPosVenda) && xml.NivelPosVenda.Length == 36)
                crm.NivelPosVendas = new Lookup(new Guid(xml.NivelPosVenda), "");
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "NivelPosVenda não enviado ou fora do padrão (Guid).";
                return crm;
            }
            if (xml.ReceitaPadrao.HasValue)
                crm.ReceitaPadrao = new Lookup(new Intelbras.CRM2013.Domain.Servicos.ReceitaPadraoService(this.Organizacao, this.IsOffline).BuscaPorCodigo(xml.ReceitaPadrao.Value).ID.Value, "");
            else
                crm.AddNullProperty("ReceitaPadrao");

            if (xml.TransportadoraRedespacho.HasValue)
                crm.TransportadoraRedespacho = new Lookup(new Intelbras.CRM2013.Domain.Servicos.TransportadoraService(this.Organizacao, this.IsOffline).ObterPorCodigoTransportadora(xml.TransportadoraRedespacho.Value).ID.Value, "");
            else
                crm.AddNullProperty("TransportadoraRedespacho");

            if (xml.Transportadora.HasValue)
                crm.Transportadora = new Lookup(new Intelbras.CRM2013.Domain.Servicos.TransportadoraService(this.Organizacao, this.IsOffline).ObterPorCodigoTransportadora(xml.Transportadora.Value).ID.Value, "");
            else
                crm.AddNullProperty("Transportadora");


            if (!String.IsNullOrEmpty(xml.ContatoPrincipal))
                crm.ContatoPrimario = new Lookup(new Guid(xml.ContatoPrincipal), "contact");
            else
                crm.AddNullProperty("ContatoPrimario");

            if (!String.IsNullOrEmpty(xml.ContaPrimaria))
                crm.ContaPrimaria = new Lookup(new Guid(xml.ContaPrimaria), "account");

            if (!String.IsNullOrEmpty(xml.SistemaGestao))
                crm.SoftwareNegocios = xml.SistemaGestao;
            else
                crm.AddNullProperty("SoftwareNegocios");

            crm.GUIDCRM40 = xml.CodigoCRM4;

            //Bloco atualização Konviva
            if (!string.IsNullOrEmpty(xml.CodigoEstrangeiro))
            {
                crm.CpfCnpj = xml.CodigoEstrangeiro;
                crm.TipoConstituicao = (int)Domain.Enum.Conta.TipoConstituicao.Estrangeiro;
            }
            if (xml.OrigemConta.HasValue)
                crm.OrigemConta = xml.OrigemConta;

            if (!String.IsNullOrEmpty(xml.NumeroPassaporte))
                crm.NumeroPassaporte = xml.NumeroPassaporte;
            else
                crm.AddNullProperty("NumeroPassaporte");

            if (xml.StatusIntegracaoSefaz.HasValue)
                crm.StatusIntegracaoSefaz = xml.StatusIntegracaoSefaz;

            if (xml.DataHoraIntegracaoSefaz.HasValue)
                crm.DataHoraIntegracaoSefaz = xml.DataHoraIntegracaoSefaz;
            else
                crm.AddNullProperty("DataHoraIntegracaoSefaz");

            if (!String.IsNullOrEmpty(xml.RegimeApuracao))
                crm.RegimeApuracao = xml.RegimeApuracao;
            else
                crm.AddNullProperty("RegimeApuracao");

            if (xml.DataBaixaContribuinte.HasValue)
                crm.DataBaixaContribuinte = xml.DataBaixaContribuinte;
            else
                crm.AddNullProperty("DataBaixaContribuinte");

            if (!string.IsNullOrEmpty(xml.Categoria))
                crm.Categoria = new Lookup(new Guid(xml.Categoria), "");
            else
                crm.AddNullProperty("Categoria");

            if (xml.CodigoCanalVenda != null)
                crm.CanaldeVenda = xml.CodigoCanalVenda;

            if (xml.IdentificacaoConta != null)
                crm.IdentificacaoConta = xml.IdentificacaoConta;

            if (xml.CodigoRepresentante != null && xml.CodigoRepresentante.HasValue)
            {
                var representante = new Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContatoPorCodigoRepresentante(xml.CodigoRepresentante.Value.ToString());

                if (representante != null)
                {
                    crm.Representante = new Lookup(representante.ID.Value, "");
                }
            }
            else
            {
                crm.AddNullProperty("Representante");
            }

            if (xml.IntegraIntelbrasPontua != null)
                crm.IntegraIntelbrasPontua = xml.IntegraIntelbrasPontua;


            if (xml.PodePontuarSellin != null)
            {
                crm.PodePontuarSellin = xml.PodePontuarSellin;
            }

            if (xml.PossuiAcessoSolar != null)
                crm.PossuiAcessoSolar = xml.PossuiAcessoSolar;

            #region Perfil da Revenda
            if (xml.DataAtualizacaoPerfil.HasValue)
                crm.DataAtualizacaoPerfil = xml.DataAtualizacaoPerfil;

            if (!String.IsNullOrEmpty(xml.UsuarioAtualizacaoPerfil))
                crm.UsuarioAtualizacaoPerfil = xml.UsuarioAtualizacaoPerfil;

            if (xml.AceitaIndicacaoOndeEncontrar.HasValue)
                crm.AceitaSerIndicadoOndeEncontrar = xml.AceitaIndicacaoOndeEncontrar;

            if (xml.VersaoTermoOndeEncontrar.HasValue)
                crm.VersaoTermoOndeEncontrar = xml.VersaoTermoOndeEncontrar;

            if (!String.IsNullOrEmpty(xml.CNPJFiliais))
                crm.CNPJFiliais = xml.CNPJFiliais;
            else
                crm.AddNullProperty("CNPJFiliais"); ;

            if (!String.IsNullOrEmpty(xml.MarcasEnviadas))
                crm.MarcasEnviadas = xml.MarcasEnviadas;
            #endregion

            #region Bloco Endereco Principal

            ///Bloco Endereco
            ///
            if (xml.EnderecoPrincipal != null)
            {
                if (xml.EnderecoPrincipal.TipoEndereco.HasValue && System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Conta.Tipoendereco), xml.EnderecoPrincipal.TipoEndereco))
                    crm.TipoEndereco = xml.EnderecoPrincipal.TipoEndereco.Value;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Tipo Endereco não enviado.";
                    return crm;
                }
                if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.NomeEndereco))
                {
                    crm.Endereco1Nome = xml.EnderecoPrincipal.NomeEndereco;
                }
                else
                    crm.AddNullProperty("Endereco1Nome");

                if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Logradouro))
                {
                    crm.Endereco1Rua = xml.EnderecoPrincipal.Logradouro;
                    crm.Endereco1Rua1 = xml.EnderecoPrincipal.Logradouro;
                }
                if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Numero))
                    crm.Endereco1Numero = xml.EnderecoPrincipal.Numero;

                if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Complemento))
                    crm.Endereco1Complemento = xml.EnderecoPrincipal.Complemento;
                else
                    crm.AddNullProperty("Endereco1Complemento");

                if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Bairro))
                {
                    crm.Endereco1Bairro = xml.EnderecoPrincipal.Bairro;
                    crm.Endereco1Bairro1 = xml.EnderecoPrincipal.Bairro;
                }

                if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.CEP))
                    crm.Endereco1CEP = Intelbras.CRM2013.Domain.Servicos.Helper.FormatarCep(xml.EnderecoPrincipal.CEP);

                if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Telefone))
                    crm.TelefoneEndereco = xml.EnderecoPrincipal.Telefone;
                else
                    crm.AddNullProperty("TelefoneEndereco");

                if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.CaixaPostal))
                    crm.Endereco1CaixaPostal = xml.EnderecoPrincipal.CaixaPostal;
                else
                    crm.AddNullProperty("Endereco1CaixaPostal");

                //xml

                //Cidade
                if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Cidade))
                {
                    Municipio cidade = new Intelbras.CRM2013.Domain.Servicos.MunicipioServices(this.Organizacao, this.IsOffline).BuscaCidade(xml.EnderecoPrincipal.Cidade);

                    if (cidade != null && cidade.ID.HasValue)
                    {
                        crm.Endereco1Municipioid = new Lookup(cidade.ID.Value, "");
                        crm.Endereco1Cidade = cidade.Nome;
                    }
                }

                //Estado
                if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Estado))
                {
                    Estado estado = new Intelbras.CRM2013.Domain.Servicos.EstadoServices(this.Organizacao, this.IsOffline).BuscaEstado(xml.EnderecoPrincipal.Estado);

                    if (estado != null && estado.ID.HasValue)
                    {
                        crm.Endereco1Estadoid = new Lookup(estado.ID.Value, "");
                        crm.Endereco1Estado = estado.Nome;
                    }
                }

                //Pais
                if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Pais))
                {
                    Pais Pais = new Intelbras.CRM2013.Domain.Servicos.PaisServices(this.Organizacao, this.IsOffline).BuscaPais(xml.EnderecoPrincipal.Pais);

                    if (Pais != null && Pais.ID.HasValue)
                    {
                        crm.Endereco1Pais = new Lookup(Pais.ID.Value, "");
                        crm.Endereco1Pais1 = Pais.Nome;
                    }
                }

                crm.IntegradoEm = DateTime.Now;
                crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
                crm.UsuarioIntegracao = xml.LoginUsuario;
            }


            #endregion

            #region    Endereco Cobranca

            if (xml.EnderecoCobranca != null)
            {
                //chamado 98554 - sempre utilizar o tipo cobrança para o endereço de cobrança
                crm.Endereco2TipoEndereco = (int)Enum.Conta.Tipoendereco2.Cobranca;

                if (!String.IsNullOrEmpty(xml.EnderecoCobranca.NomeEndereco))
                    crm.Endereco2Nome = xml.EnderecoCobranca.NomeEndereco;
                else
                    crm.AddNullProperty("Endereco2Nome");

                if (!String.IsNullOrEmpty(xml.EnderecoCobranca.Logradouro))
                {
                    crm.Endereco2Rua = xml.EnderecoCobranca.Logradouro;
                    crm.Endereco2Rua2 = xml.EnderecoCobranca.Logradouro;
                }

                if (!String.IsNullOrEmpty(xml.EnderecoCobranca.Numero))
                    crm.Endereco2Numero = xml.EnderecoCobranca.Numero;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Campo Número do Endereço de Cobrança não enviado, por favor verifique o conteúdo.";
                    return crm;
                }

                if (!String.IsNullOrEmpty(xml.EnderecoCobranca.Complemento))
                    crm.Endereco2Complemento = xml.EnderecoCobranca.Complemento;
                else
                    crm.AddNullProperty("Endereco2Complemento");

                if (!String.IsNullOrEmpty(xml.EnderecoCobranca.Bairro))
                {
                    crm.Endereco2Bairro = xml.EnderecoCobranca.Bairro;
                    crm.Endereco2Bairro2 = xml.EnderecoCobranca.Bairro;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Campo Bairro do Endereço de Cobrança não enviado, por favor verifique o conteúdo.";
                    return crm;
                }

                if (!String.IsNullOrEmpty(xml.EnderecoCobranca.CEP))
                    crm.Endereco2CEP = Intelbras.CRM2013.Domain.Servicos.Helper.FormatarCep(xml.EnderecoCobranca.CEP);
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Campo CEP do Endereço de Cobrança não enviado, por favor verifique o conteúdo.";
                    return crm;
                }

                if (!String.IsNullOrEmpty(xml.EnderecoCobranca.Telefone))
                    crm.TelefoneEnderecoCobranca = xml.EnderecoCobranca.Telefone;
                if (!String.IsNullOrEmpty(xml.EnderecoCobranca.CaixaPostal))
                    crm.Endereco2CaixaPostal = xml.EnderecoCobranca.CaixaPostal;
                else
                    crm.AddNullProperty("Endereco2CaixaPostal");

                //xml

                //Cidade
                if (!String.IsNullOrEmpty(xml.EnderecoCobranca.Cidade))
                {
                    Municipio cidade = new Intelbras.CRM2013.Domain.Servicos.MunicipioServices(this.Organizacao, this.IsOffline).BuscaCidade(xml.EnderecoCobranca.Cidade);

                    if (cidade != null && cidade.ID.HasValue)
                    {
                        crm.Endereco2Municipioid = new Lookup(cidade.ID.Value, "");
                        crm.Endereco2Cidade = cidade.Nome;
                    }
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Município com o código : " + xml.EnderecoCobranca.Cidade + " não cadastrado no Crm.";
                        return crm;
                    }

                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Campo Cidade do Endereço de Cobrança não enviado, por favor verifique o conteúdo.";
                    return crm;
                }

                //Estado
                if (!String.IsNullOrEmpty(xml.EnderecoCobranca.Estado))
                {
                    Estado estado = new Intelbras.CRM2013.Domain.Servicos.EstadoServices(this.Organizacao, this.IsOffline).BuscaEstado(xml.EnderecoCobranca.Estado);

                    if (estado != null && estado.ID.HasValue)
                    {
                        crm.Endereco2Estadoid = new Lookup(estado.ID.Value, "");
                        crm.Endereco2Estado = estado.Nome;
                    }
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Estado com o código : " + xml.EnderecoCobranca.Estado + " não cadastrado no Crm.";
                        return crm;
                    }

                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Campo Estado do Endereço de Cobrança não enviado, por favor verifique o conteúdo.";
                    return crm;
                }

                //Pais
                if (!String.IsNullOrEmpty(xml.EnderecoCobranca.Pais))
                {
                    Pais Pais = new Intelbras.CRM2013.Domain.Servicos.PaisServices(this.Organizacao, this.IsOffline).BuscaPais(xml.EnderecoCobranca.Pais);

                    if (Pais != null && Pais.ID.HasValue)
                    {
                        crm.Endereco2Pais = new Lookup(Pais.ID.Value, "");
                        crm.Endereco2Pais2 = Pais.Nome;
                    }
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "País com o código : " + xml.EnderecoCobranca.Pais + " não cadastrado no Crm.";
                        return crm;
                    }

                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Campo País do Endereço de Cobrança não enviado, por favor verifique o conteúdo.";
                    return crm;
                }
            }

            #endregion


            #endregion

            return crm;
        }

        public Pollux.MSG0072 DefinirPropriedades(Conta objModel)
        {
            #region Propriedades Crm->Xml

            Pollux.MSG0072 msg0072 = new Pollux.MSG0072(itb.RetornaSistema(itb.Sistema.CRM), Helper.Truncate(objModel.RazaoSocial, 40));
            msg0072.CodigoConta = objModel.ID.Value.ToString();
            if (!String.IsNullOrEmpty(objModel.CodigoMatriz))
                msg0072.CodigoCliente = Convert.ToInt32(objModel.CodigoMatriz);
            msg0072.NomeRazaoSocial = objModel.RazaoSocial;
            msg0072.NomeFantasia = objModel.NomeFantasia;
            msg0072.NomeAbreviado = objModel.NomeAbreviado;
            msg0072.DescricaoConta = objModel.Descricao;
            if (objModel.TipoRelacao.HasValue)
                msg0072.TipoRelacao = objModel.TipoRelacao.Value;
            msg0072.NumeroBanco = objModel.Banco;

            if (objModel.ContaPrimaria != null)
                msg0072.ContaPrimaria = objModel.ContaPrimaria.Id.ToString();
            if (objModel.ContatoPrimario != null)
                msg0072.ContatoPrincipal = objModel.ContatoPrimario.Id.ToString();
            if (objModel.Regiao != null)
                msg0072.Regiao = objModel.Regiao.Name;

            msg0072.NumeroAgencia = objModel.Agencia;
            msg0072.NumeroContaCorrente = objModel.ContaCorrente;
            msg0072.EmiteBloqueto = objModel.EmiteBloqueto;
            msg0072.GeraAvisoCredito = objModel.GeraAvisoCredito;
            msg0072.CalculaMulta = objModel.CalculaMulta;
            msg0072.RecebeInformacaoSCI = objModel.RecebeInformacaoSCI;
            msg0072.Telefone = objModel.Telefone;
            msg0072.Ramal = objModel.RamalTelefonePrincipal;
            if (objModel.TelefoneAlternativo != null && !String.IsNullOrEmpty(objModel.TelefoneAlternativo))
                msg0072.TelefoneAlternativo = objModel.TelefoneAlternativo;
            if (objModel.RamalOutroTelefone != null && !String.IsNullOrEmpty(objModel.RamalOutroTelefone))
                msg0072.RamalTelefoneAlternativo = objModel.RamalOutroTelefone;
            msg0072.Fax = objModel.Fax;
            msg0072.RamalFax = objModel.RamalFax;
            msg0072.Email = objModel.Email;
            msg0072.Site = objModel.Site;
            if (objModel.Natureza.HasValue)
                msg0072.Natureza = objModel.Natureza.Value;

            string cnpjCpfObj = objModel.CpfCnpj.Replace("-", "").Replace(".", "").Replace("/", "").Trim();

            if (objModel.TipoConstituicao.HasValue)
            {
                switch (objModel.TipoConstituicao)
                {
                    case (int)Domain.Enum.Conta.TipoConstituicao.Cnpj:
                        msg0072.CNPJ = cnpjCpfObj;
                        break;

                    case (int)Domain.Enum.Conta.TipoConstituicao.Cpf:
                        msg0072.CPF = cnpjCpfObj;
                        break;

                    case (int)Domain.Enum.Conta.TipoConstituicao.Estrangeiro:
                        msg0072.CodigoEstrangeiro = objModel.CpfCnpj;
                        break;
                }
            }

            msg0072.InscricaoEstadual = objModel.InscricaoEstadual;
            msg0072.InscricaoMunicipal = objModel.InscricaoMunicipal;
            msg0072.SuspensaoCredito = objModel.SuspensaoCredito;
            msg0072.LimiteCredito = objModel.LimiteCredito;
            msg0072.DataLimiteCredito = objModel.DataLimiteCredito;
            msg0072.SaldoCredito = objModel.SaldoCredito;
            msg0072.ModalidadeCobranca = objModel.Modalidade;
            msg0072.ContribuinteICMS = objModel.ContribuinteICMS;
            msg0072.CodigoSUFRAMA = objModel.CodigoSuframa;
            msg0072.InscricaoSubstituicaoTributaria = objModel.SubstituicaoTributaria;
            msg0072.OptanteSuspensaoIPI = objModel.OptanteSuspensaoIPI;
            msg0072.AgenteRetencao = objModel.AgenteRetencao;
            msg0072.PisCofinsUnidade = objModel.PISCOFINSPorUnidade;
            msg0072.RecebeNotaFiscalEletronica = objModel.RecebeNFE;
            msg0072.FormaTributacao = objModel.FormaTributacao;
            msg0072.ObservacaoPedido = objModel.ObservacoesPedido;
            msg0072.TipoEmbalagem = objModel.TipoEmbalagem;
            msg0072.CodigoIncoterm = objModel.Incoterm;
            msg0072.LocalEmbarque = objModel.LocalEmbarque;
            msg0072.ViaEmbarque = objModel.EmbarqueVia;
            msg0072.DataImplantacao = objModel.DataImplantacao;
            if (!String.IsNullOrEmpty(objModel.DocIdentidade))
                msg0072.RG = objModel.DocIdentidade;
            if (!String.IsNullOrEmpty(objModel.EmissorIdentidade))
                msg0072.OrgaoExpeditor = objModel.EmissorIdentidade;
            msg0072.DataVencimentoConcessao = objModel.DataVenctoConcessao;
            msg0072.DescontoAssistenciaTecnica = objModel.DescontoCAT;
            msg0072.CoberturaGeografica = objModel.CoberturaGeografica;
            msg0072.DataConstituicao = objModel.DataConstituicao;
            msg0072.DistribuicaoUnicaFonteReceita = objModel.DistribuicaoFonteReceita;
            msg0072.QualificadoTreinamento = objModel.EspacoFisicoQualificado;
            if (objModel.Exclusividade.HasValue)
                msg0072.Exclusividade = objModel.Exclusividade.Value;
            msg0072.Historico = objModel.Historico;
            msg0072.IntencaoApoio = objModel.IntencaoApoio;
            msg0072.MetodoComercializacao = objModel.MetodoComercializacaoProduto;
            msg0072.ModeloOperacao = objModel.ModeloOperacaoFiliais;
            msg0072.NumeroFuncionarios = objModel.NumeroFuncionarios;
            msg0072.NumeroColaboradoresAreaTecnica = objModel.NumeroColaboradores;
            msg0072.NumeroRevendasAtivas = objModel.NumeroRevendasAtivas;
            msg0072.NumeroRevendasInativas = objModel.NumeroRevendasInativas;
            msg0072.NumeroTecnicosSuporte = objModel.NumeroTecnicosSuporte;
            msg0072.NumeroVendedores = objModel.NumeroVendedores;
            msg0072.OutraFonteReceita = objModel.OutraFonteReceita;
            if (objModel.ParticipantePrograma.HasValue)
                msg0072.ParticipaProgramaCanais = objModel.ParticipantePrograma.Value;

            #region Asistencia Tecnica
            if (objModel.AssistenciaTecnica.HasValue)
            {
                msg0072.AssistenciaTecnica = objModel.AssistenciaTecnica.Value;
            }
            else
            {
                msg0072.AssistenciaTecnica = false;
            }

            if (objModel.PossuiEstruturaCompleta.HasValue)
                msg0072.PossuiEstruturaCompleta = objModel.PossuiEstruturaCompleta;

            if (objModel.PerfilAssistenciaTecnica.HasValue)
                msg0072.PerfilAssistenciaTecnica = objModel.PerfilAssistenciaTecnica;

            if (objModel.TabelaPrecoAstec.HasValue)
                msg0072.TabelaPrecoAssistenciaTecnica = objModel.TabelaPrecoAstec;

            if (objModel.NomeAbreviadoMatrizEconomica != null)
                msg0072.NomeAbreviadoMatrizEconomica = objModel.NomeAbreviadoMatrizEconomica;
            #endregion

            msg0072.PerfilRevendasDistribuidor = objModel.PerfilRevendaDistribuidor;
            msg0072.PossuiEstruturaCompleta = objModel.PossuiEstruturaCompleta;
            msg0072.PossuiFiliais = objModel.PossuiFiliais;
            msg0072.QuantidadeFiliais = objModel.QuantasFiliais;
            if (objModel.PrazoMedioCompras.HasValue)
                msg0072.PrazoMedioCompra = (decimal)objModel.PrazoMedioCompras.Value;
            if (objModel.PrazoMedioVendas.HasValue)
                msg0072.PrazoMedioVenda = (decimal)objModel.PrazoMedioVendas.Value;
            if (objModel.Setor.HasValue)
                msg0072.Setor = objModel.Setor;
            msg0072.ValorMedioCompra = objModel.ValorMedioComprasMensais;
            msg0072.ValorMedioVenda = objModel.ValorMedioVendasMensais;
            msg0072.VendeAtacadista = objModel.VendasParaAtacadistaVarejista;
            msg0072.ObservacaoNotaFiscal = objModel.ObservacoesNF;
            msg0072.EstruturaPropriedade = objModel.Propriedade;
            msg0072.ReceitaAnual = objModel.ReceitaAnual;

            msg0072.Situacao = (objModel.Status.HasValue ? objModel.Status.Value : (int)Enum.StateCode.Ativo);

            if (objModel.Classificacao != null)
                msg0072.Classificacao = objModel.Classificacao.Id.ToString();

            if (objModel.Subclassificacao == null)
            {
                throw new ArgumentException("(CRM) O campo Subclassificação obrigatório");
            }

            msg0072.SubClassificacao = objModel.Subclassificacao.Id.ToString();

            if (objModel.PercentualComissaoProjeto != null)
                msg0072.PercentualComissaoSolar = objModel.PercentualComissaoProjeto;

            msg0072.CodigoCRM4 = objModel.GUIDCRM40;

            if (objModel.Portador != null)
            {
                Domain.Model.Portador portador = new Intelbras.CRM2013.Domain.Servicos.PortadorService(this.Organizacao, this.IsOffline)
                    .BuscaPorCodigo(objModel.Portador.Id);

                if (portador == null || !portador.CodigoPortador.HasValue)
                {
                    throw new ArgumentException("(CRM) Portador não localizado. (ID: " + objModel.Portador.Id + ")");
                }

                msg0072.Portador = portador.CodigoPortador;
            }

            if (objModel.NivelPosVendas == null)
            {
                throw new ArgumentException("(CRM) Campo nivel pós venda obrigatório");
            }

            msg0072.NivelPosVenda = objModel.NivelPosVendas.Id.ToString();

            if (objModel.ClientePotencialOriginador != null)
                msg0072.ClientePotencialOriginador = objModel.ClientePotencialOriginador.Id.ToString();
            if (objModel.CondicaoPagamento != null)
            {
                CondicaoPagamento codPagto = new Servicos.CondicaoPagamentoService(this.Organizacao, this.IsOffline).BuscaCondicaoPagamento(objModel.CondicaoPagamento.Id);
                if (codPagto != null && codPagto.Codigo.HasValue)
                {
                    msg0072.CondicaoPagamento = codPagto.Codigo;
                }
            }

            Domain.Model.Usuario proprietario = new Domain.Servicos.UsuarioService(this.Organizacao, this.IsOffline).BuscarProprietario("account", "accountid", objModel.ID.Value);
            if (proprietario != null)
            {
                msg0072.Proprietario = proprietario.Id.ToString();
                msg0072.TipoProprietario = "systemuser";
            }
            else
            {
                msg0072.Proprietario = "259A8E4F-15E9-E311-9420-00155D013D39";
                msg0072.TipoProprietario = "systemuser";
            }

            if (objModel.Transportadora != null)
            {
                Transportadora transp = new Servicos.TransportadoraService(this.Organizacao, this.IsOffline).ObterPor(objModel.Transportadora.Id);
                if (transp != null && transp.Codigo.HasValue)
                {
                    msg0072.Transportadora = transp.Codigo.Value;
                }
            }
            if (objModel.TransportadoraRedespacho != null)
            {
                Transportadora transpRed = new Servicos.TransportadoraService(this.Organizacao, this.IsOffline).ObterPor(objModel.TransportadoraRedespacho.Id);
                if (transpRed != null && transpRed.Codigo.HasValue)
                {
                    msg0072.TransportadoraRedespacho = transpRed.Codigo.Value;
                }
            }

            if (objModel.TipoConta.HasValue)
                msg0072.TipoConta = objModel.TipoConta.Value;
            msg0072.ApuracaoBeneficio = objModel.ApuracaoBeneficiosCompromissos;
            msg0072.DataAdesao = objModel.DataAdesao;
            if (objModel.DiasAtraso.HasValue)
                msg0072.NumeroDiasAtraso = (int)objModel.DiasAtraso;

            if (objModel.ReceitaPadrao != null)
            {
                ReceitaPadrao receitaPadrao = new Servicos.ReceitaPadraoService(this.Organizacao, this.IsOffline).BuscaPorCodigo(objModel.ReceitaPadrao.Id);
                if (receitaPadrao != null)
                    msg0072.ReceitaPadrao = receitaPadrao.CodReceitaPadrao;
            }
            if (objModel.Portador != null)
            {
                Portador mPortador = new Servicos.PortadorService(this.Organizacao, this.IsOffline).BuscaPorCodigo(objModel.Portador.Id);
                if (mPortador != null)
                    msg0072.Portador = mPortador.CodigoPortador;
            }

            msg0072.SistemaGestao = objModel.SoftwareNegocios;
            msg0072.DataImplantacao = objModel.DataImplantacao;
            msg0072.DataUltimoSellOut = objModel.DataUltimoSelloutRevenda;
            msg0072.EscolheuDistrForaSellOut = objModel.EscolheuDistribuidorSemSellout;
            msg0072.AdesaoPciRealizadaPor = objModel.AdesaoAoPCIRealizadaPor;
            msg0072.ParticipaProgramaCanaisMotivo = objModel.MotivoParticipantePrograma;
            msg0072.FiguraNoSite = objModel.FiguraNoSite;
            msg0072.CNAE = objModel.CNAE;
            msg0072.RamoAtividadeEconomica = objModel.RamoAtividadeEconomica;

            if (objModel.CnaeId != null) msg0072.CodigoRamoAtividadeEconomica = objModel.CnaeId.Id.ToString();
            if (objModel.ListaPreco != null) msg0072.ListaPreco = objModel.ListaPreco.Name;
            if (objModel.TipoConstituicao.HasValue) msg0072.TipoConstituicao = objModel.TipoConstituicao.Value;

            //Atualizacao Konviva
            if (objModel.TipoConstituicao.HasValue && objModel.TipoConstituicao.Value == (int)Domain.Enum.Conta.TipoConstituicao.Estrangeiro)
                msg0072.CodigoEstrangeiro = objModel.CpfCnpj;

            if (objModel.OrigemConta.HasValue)
                msg0072.OrigemConta = objModel.OrigemConta;

            if (!String.IsNullOrEmpty(objModel.NumeroPassaporte))
                msg0072.NumeroPassaporte = objModel.NumeroPassaporte;

            if (objModel.StatusIntegracaoSefaz.HasValue)
                msg0072.StatusIntegracaoSefaz = objModel.StatusIntegracaoSefaz;

            if (objModel.DataHoraIntegracaoSefaz.HasValue)
                msg0072.DataHoraIntegracaoSefaz = objModel.DataHoraIntegracaoSefaz;

            if (!String.IsNullOrEmpty(objModel.RegimeApuracao))
                msg0072.RegimeApuracao = objModel.RegimeApuracao;

            if (objModel.DataBaixaContribuinte.HasValue)
                msg0072.DataBaixaContribuinte = objModel.DataBaixaContribuinte;

            if (objModel.Categoria != null)
                msg0072.Categoria = objModel.Categoria.Id.ToString();

            if (objModel.CanaldeVenda != null)
                msg0072.CodigoCanalVenda = objModel.CanaldeVenda;

            if (objModel.IdentificacaoConta != null)
                msg0072.IdentificacaoConta = objModel.IdentificacaoConta;

            if (objModel.Representante != null)
            {
                var representante = new ContatoService(this.Organizacao, this.IsOffline).BuscaContato(objModel.Representante.Id);
                if (representante != null && representante.CodigoRepresentante != null)
                    msg0072.CodigoRepresentante = Int32.Parse(representante.CodigoRepresentante);
            }

            if (objModel.IntegraIntelbrasPontua != null)
                msg0072.IntegraIntelbrasPontua = objModel.IntegraIntelbrasPontua;

            if (objModel.PodePontuarSellin != null)
            {
                msg0072.PodePontuarSellin = objModel.PodePontuarSellin;
            }
            else
            {
                msg0072.PodePontuarSellin = false;
            }

            if (objModel.EnvioSelloutEstoque != null)
                msg0072.EnvioSelloutEstoque = objModel.EnvioSelloutEstoque;

            if (objModel.PrestacaoServicoIsol != null)
            {
                msg0072.PrestadorServico = objModel.PrestacaoServicoIsol;
            }
            else
            {
                msg0072.PrestadorServico = false;
            }

            msg0072.PossuiAcessoSolar = objModel.PossuiAcessoSolar != null ? objModel.PossuiAcessoSolar : false;

            #region Perfil da Revenda
            if (objModel.DataAtualizacaoPerfil.HasValue)
                msg0072.DataAtualizacaoPerfil = objModel.DataAtualizacaoPerfil;

            if (!String.IsNullOrEmpty(objModel.UsuarioAtualizacaoPerfil))
                msg0072.UsuarioAtualizacaoPerfil = objModel.UsuarioAtualizacaoPerfil;

            if (objModel.AceitaSerIndicadoOndeEncontrar.HasValue)
                msg0072.AceitaIndicacaoOndeEncontrar = objModel.AceitaSerIndicadoOndeEncontrar;

            if (objModel.VersaoTermoOndeEncontrar.HasValue)
                msg0072.VersaoTermoOndeEncontrar = objModel.VersaoTermoOndeEncontrar;

            if (!String.IsNullOrEmpty(objModel.CNPJFiliais))
                msg0072.CNPJFiliais = objModel.CNPJFiliais;

            if (!String.IsNullOrEmpty(objModel.MarcasEnviadas))
                msg0072.MarcasEnviadas = objModel.MarcasEnviadas;

            if (objModel.Instalador.HasValue)
                msg0072.Instalador = objModel.Instalador;
            #endregion

            #region Endereço
            //Bloco Endereço 

            //Principal
            Pollux.Entities.Endereco endPrincipal = new Pollux.Entities.Endereco();
            endPrincipal.Bairro = objModel.Endereco1Bairro;
            endPrincipal.Numero = objModel.Endereco1Numero;
            if (!String.IsNullOrEmpty(objModel.Endereco1CEP))
                endPrincipal.CEP = objModel.Endereco1CEP.Replace("-", "").PadLeft(8, '0'); ;
            if (objModel.Endereco1Municipioid != null)
            {
                Municipio municipio = new Servicos.MunicipioServices(this.Organizacao, this.IsOffline).ObterPor(objModel.Endereco1Municipioid.Id);
                endPrincipal.Cidade = municipio.ChaveIntegracao;
                endPrincipal.NomeCidade = municipio.Nome;
            }
            endPrincipal.Complemento = objModel.Endereco1Complemento;
            if (objModel.Endereco1Estadoid != null)
            {
                Estado estado = new Servicos.EstadoServices(this.Organizacao, this.IsOffline).BuscaEstadoPorId(objModel.Endereco1Estadoid.Id);
                if (estado != null)
                {
                    endPrincipal.Estado = estado.ChaveIntegracao;
                    endPrincipal.UF = estado.SiglaUF;
                }
            }
            endPrincipal.Logradouro = objModel.Endereco1Rua;
            endPrincipal.NomeEndereco = objModel.Endereco1Nome;
            if (objModel.Endereco1Pais != null)
            {
                Pais pais = new Servicos.PaisServices(this.Organizacao, this.IsOffline).BuscaPais(objModel.Endereco1Pais.Id);
                if (pais != null)
                {
                    endPrincipal.NomePais = pais.Nome;
                    endPrincipal.Pais = pais.Nome;
                }
            }
            endPrincipal.TipoEndereco = objModel.TipoEndereco;
            if (!String.IsNullOrEmpty(objModel.Endereco1CaixaPostal))
                endPrincipal.CaixaPostal = objModel.Endereco1CaixaPostal;
            msg0072.EnderecoPrincipal = endPrincipal;

            //Cobrança
            if (objModel.Endereco2Estadoid != null && objModel.Endereco2Municipioid != null && objModel.Endereco2Pais != null)
            {
                Pollux.Entities.Endereco endCobranca = new Pollux.Entities.Endereco();
                endCobranca.Bairro = objModel.Endereco2Bairro;
                endCobranca.Numero = objModel.Endereco2Numero;
                if (!String.IsNullOrEmpty(objModel.Endereco2CEP))
                    endCobranca.CEP = objModel.Endereco2CEP.Replace("-", "").PadLeft(8, '0'); ;
                if (objModel.Endereco2Municipioid != null)
                {
                    Municipio municipio2 = new Servicos.MunicipioServices(this.Organizacao, this.IsOffline).ObterPor(objModel.Endereco2Municipioid.Id);
                    endCobranca.Cidade = municipio2.ChaveIntegracao;
                    endCobranca.NomeCidade = municipio2.Nome;
                }
                endCobranca.Complemento = objModel.Endereco2Complemento;
                if (objModel.Endereco2Estadoid != null)
                {
                    Estado estado2 = new Servicos.EstadoServices(this.Organizacao, this.IsOffline).BuscaEstadoPorId(objModel.Endereco2Estadoid.Id);
                    if (estado2 != null)
                    {
                        endCobranca.Estado = estado2.ChaveIntegracao;
                        endCobranca.UF = estado2.SiglaUF;
                    }
                }
                endCobranca.Logradouro = objModel.Endereco2Rua;
                endCobranca.NomeEndereco = objModel.Endereco2Nome;

                if (objModel.Endereco2Pais != null)
                {
                    Pais pais2 = new Servicos.PaisServices(this.Organizacao, this.IsOffline).BuscaPais(objModel.Endereco2Pais.Id);
                    if (pais2 != null)
                    {
                        endCobranca.NomePais = pais2.Nome;
                        endCobranca.Pais = pais2.Nome;
                    }
                }

                endCobranca.TipoEndereco = (int)Enum.Conta.Tipoendereco2.Cobranca;
                if (!String.IsNullOrEmpty(objModel.Endereco2CaixaPostal))
                    endCobranca.CaixaPostal = objModel.Endereco2CaixaPostal;
                msg0072.EnderecoCobranca = endCobranca;
            }

            #endregion

            #endregion

            return msg0072;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Conta objModel, ref string nomeAbrevRet, ref string codigoClienteRet, ref string nomeAbrevMatriEconom)
        {
            string retMsg = String.Empty;
            string message = string.Empty;

            Intelbras.Message.Helper.MSG0072 mensagem = this.DefinirPropriedades(objModel);
            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);

            try
            {
                message = mensagem.GenerateMessage(true);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("(CRM) (XSD) " + ex.Message, ex);
            }

            if (integracao.EnviarMensagemBarramento(message, "1", "1", out retMsg))
            {
                Intelbras.Message.Helper.MSG0072R1 retorno = CarregarMensagem<Pollux.MSG0072R1>(retMsg);
                if (retorno.Resultado.Sucesso)
                {
                    if (!String.IsNullOrEmpty(retorno.NomeAbreviadoMatrizEconomica))
                        nomeAbrevMatriEconom = retorno.NomeAbreviadoMatrizEconomica;
                    if (!String.IsNullOrEmpty(retorno.NomeAbreviado))
                    {
                        nomeAbrevRet = retorno.NomeAbreviado;
                    }

                    if (retorno.CodigoCliente.HasValue)
                    {
                        codigoClienteRet = retorno.CodigoCliente.Value.ToString();
                    }
                }
                else
                {
                    throw new ArgumentException("(CRM) " + string.Concat(retorno.Resultado.Mensagem));
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(retMsg);
                throw new ArgumentException("(CRM) " + string.Concat(erro001.GenerateMessage(false)));
            }
            return retMsg;
        }

        public string Enviar(Conta contaObj)
        {
            return String.Empty;
        }

        #endregion
    }
}