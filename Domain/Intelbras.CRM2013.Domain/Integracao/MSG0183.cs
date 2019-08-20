using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;
using SDKore.Helper;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0183 : Base, IBase<Pollux.MSG0183, Conta>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        #endregion

        #region Construtor
        public MSG0183(string org, bool isOffline)
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
            retorno.Add("Resultado", resultadoPersistencia);

            try
            {
                var xml = this.CarregarMensagem<Pollux.MSG0183>(mensagem);

                var conta = DefinirPropriedades(xml);

                if (conta == null)
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "(CRM) Conta não encontrada.";

                    return CriarMensagemRetorno<Pollux.MSG0183R1>(numeroMensagem, retorno);
                }

                var obj = DefinirPropriedades(conta);

                return obj.GenerateMessage();
            }
            catch (Exception ex)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(ex);
                return CriarMensagemRetorno<Pollux.MSG0183R1>(numeroMensagem, retorno);
            }
        }
        #endregion

        #region Métodos Auxiliares
        public Pollux.MSG0183R1 DefinirPropriedades(Conta objModel)
        {
            #region Propriedades Crm->Xml
            Pollux.MSG0183R1 msg0183R1;
            if (objModel.RazaoSocial != null && objModel.RazaoSocial.Length > 0)
            {
                msg0183R1 = new Pollux.MSG0183R1(itb.RetornaSistema(itb.Sistema.CRM), Helper.Truncate(objModel.RazaoSocial, 40));
            }
            else
            {
                msg0183R1 = new Pollux.MSG0183R1(itb.RetornaSistema(itb.Sistema.CRM), Helper.Truncate(objModel.ID.Value.ToString(), 40));
            }

            msg0183R1.CodigoConta = objModel.ID.Value.ToString();

            if (!String.IsNullOrEmpty(objModel.CodigoMatriz))
                msg0183R1.CodigoCliente = Convert.ToInt32(objModel.CodigoMatriz);
            msg0183R1.NomeRazaoSocial = objModel.RazaoSocial;
            msg0183R1.NomeFantasia = objModel.NomeFantasia;
            msg0183R1.NomeAbreviado = objModel.NomeAbreviado;
            msg0183R1.DescricaoConta = objModel.Descricao;
            if (objModel.TipoRelacao.HasValue)
                msg0183R1.TipoRelacao = objModel.TipoRelacao.Value;
            msg0183R1.NumeroBanco = objModel.Banco;

            if (objModel.ContaPrimaria != null)
                msg0183R1.ContaPrimaria = objModel.ContaPrimaria.Id.ToString();
            if (objModel.ContatoPrimario != null)
                msg0183R1.ContatoPrincipal = objModel.ContatoPrimario.Id.ToString();
            if (objModel.Regiao != null)
                msg0183R1.Regiao = objModel.Regiao.Name;

            msg0183R1.NumeroAgencia = objModel.Agencia;
            msg0183R1.NumeroContaCorrente = objModel.ContaCorrente;
            msg0183R1.EmiteBloqueto = objModel.EmiteBloqueto;
            msg0183R1.GeraAvisoCredito = objModel.GeraAvisoCredito;
            msg0183R1.CalculaMulta = objModel.CalculaMulta;
            msg0183R1.RecebeInformacaoSCI = objModel.RecebeInformacaoSCI;
            msg0183R1.Telefone = objModel.Telefone;
            msg0183R1.Ramal = objModel.RamalTelefonePrincipal;
            if (objModel.TelefoneAlternativo != null && !String.IsNullOrEmpty(objModel.TelefoneAlternativo))
                msg0183R1.TelefoneAlternativo = objModel.TelefoneAlternativo;
            if (objModel.RamalOutroTelefone != null && !String.IsNullOrEmpty(objModel.RamalOutroTelefone))
                msg0183R1.RamalTelefoneAlternativo = objModel.RamalOutroTelefone;
            msg0183R1.Fax = objModel.Fax;
            msg0183R1.RamalFax = objModel.RamalFax;
            msg0183R1.Email = objModel.Email;
            msg0183R1.Site = objModel.Site;
            if (objModel.Natureza.HasValue)
                msg0183R1.Natureza = objModel.Natureza.Value;

            string cnpjCpfObj = objModel.CpfCnpj.Replace("-", "").Replace(".", "").Replace("/", "").Trim();

            if (objModel.TipoConstituicao.HasValue)
            {
                switch (objModel.TipoConstituicao)
                {
                    case (int)Domain.Enum.Conta.TipoConstituicao.Cnpj:
                        msg0183R1.CNPJ = cnpjCpfObj;
                        break;

                    case (int)Domain.Enum.Conta.TipoConstituicao.Cpf:
                        msg0183R1.CPF = cnpjCpfObj;
                        break;

                    case (int)Domain.Enum.Conta.TipoConstituicao.Estrangeiro:
                        msg0183R1.CodigoEstrangeiro = objModel.CpfCnpj;
                        break;
                }
            }

            if (objModel.InscricaoEstadual != null)
            {
                if (objModel.InscricaoEstadual.Length < 2)
                {
                    msg0183R1.InscricaoEstadual = null;
                }
                else
                {
                    msg0183R1.InscricaoEstadual = objModel.InscricaoEstadual.Replace(".", "").Trim();
                }
            }
            else
            {
                msg0183R1.InscricaoEstadual = null;
            }

            msg0183R1.InscricaoMunicipal = objModel.InscricaoMunicipal;
            msg0183R1.SuspensaoCredito = objModel.SuspensaoCredito;
            msg0183R1.LimiteCredito = objModel.LimiteCredito;
            msg0183R1.DataLimiteCredito = objModel.DataLimiteCredito;
            msg0183R1.SaldoCredito = objModel.SaldoCredito;
            msg0183R1.ModalidadeCobranca = objModel.Modalidade;
            msg0183R1.ContribuinteICMS = objModel.ContribuinteICMS;
            msg0183R1.CodigoSUFRAMA = objModel.CodigoSuframa;
            msg0183R1.InscricaoSubstituicaoTributaria = objModel.SubstituicaoTributaria;
            msg0183R1.OptanteSuspensaoIPI = objModel.OptanteSuspensaoIPI;
            msg0183R1.AgenteRetencao = objModel.AgenteRetencao;
            msg0183R1.PisCofinsUnidade = objModel.PISCOFINSPorUnidade;
            msg0183R1.RecebeNotaFiscalEletronica = objModel.RecebeNFE;
            msg0183R1.FormaTributacao = objModel.FormaTributacao;
            msg0183R1.ObservacaoPedido = objModel.ObservacoesPedido;
            msg0183R1.TipoEmbalagem = objModel.TipoEmbalagem;
            msg0183R1.CodigoIncoterm = objModel.Incoterm;
            msg0183R1.LocalEmbarque = objModel.LocalEmbarque;
            msg0183R1.ViaEmbarque = objModel.EmbarqueVia;
            msg0183R1.DataImplantacao = objModel.DataImplantacao;
            if (!String.IsNullOrEmpty(objModel.DocIdentidade))
                msg0183R1.RG = objModel.DocIdentidade;
            if (!String.IsNullOrEmpty(objModel.EmissorIdentidade))
                msg0183R1.OrgaoExpeditor = objModel.EmissorIdentidade;
            msg0183R1.DataVencimentoConcessao = objModel.DataVenctoConcessao;
            msg0183R1.DescontoAssistenciaTecnica = objModel.DescontoCAT;
            msg0183R1.CoberturaGeografica = objModel.CoberturaGeografica;
            msg0183R1.DataConstituicao = objModel.DataConstituicao;
            msg0183R1.DistribuicaoUnicaFonteReceita = objModel.DistribuicaoFonteReceita;
            msg0183R1.QualificadoTreinamento = objModel.EspacoFisicoQualificado;
            if (objModel.Exclusividade.HasValue)
                msg0183R1.Exclusividade = objModel.Exclusividade.Value;
            msg0183R1.Historico = objModel.Historico;
            msg0183R1.IntencaoApoio = objModel.IntencaoApoio;
            msg0183R1.MetodoComercializacao = objModel.MetodoComercializacaoProduto;
            msg0183R1.ModeloOperacao = objModel.ModeloOperacaoFiliais;
            msg0183R1.NumeroFuncionarios = objModel.NumeroFuncionarios;
            msg0183R1.NumeroColaboradoresAreaTecnica = objModel.NumeroColaboradores;
            msg0183R1.NumeroRevendasAtivas = objModel.NumeroRevendasAtivas;
            msg0183R1.NumeroRevendasInativas = objModel.NumeroRevendasInativas;
            msg0183R1.NumeroTecnicosSuporte = objModel.NumeroTecnicosSuporte;
            msg0183R1.NumeroVendedores = objModel.NumeroVendedores;
            msg0183R1.OutraFonteReceita = objModel.OutraFonteReceita;
            if (objModel.ParticipantePrograma.HasValue)
                msg0183R1.ParticipaProgramaCanais = objModel.ParticipantePrograma.Value;

            #region Asistencia Tecnica
            if (objModel.AssistenciaTecnica.HasValue)
                msg0183R1.AssistenciaTecnica = objModel.AssistenciaTecnica.Value;

            if (objModel.PossuiEstruturaCompleta.HasValue)
                msg0183R1.PossuiEstruturaCompleta = objModel.PossuiEstruturaCompleta;

            if (objModel.PerfilAssistenciaTecnica.HasValue)
                msg0183R1.PerfilAssistenciaTecnica = objModel.PerfilAssistenciaTecnica;

            if (objModel.TabelaPrecoAstec.HasValue)
                msg0183R1.TabelaPrecoAssistenciaTecnica = objModel.TabelaPrecoAstec;

            if (objModel.NomeAbreviadoMatrizEconomica != null)
                msg0183R1.NomeAbreviadoMatrizEconomica = objModel.NomeAbreviadoMatrizEconomica;
            #endregion

            msg0183R1.PerfilRevendasDistribuidor = objModel.PerfilRevendaDistribuidor;
            msg0183R1.PossuiEstruturaCompleta = objModel.PossuiEstruturaCompleta;
            msg0183R1.PossuiFiliais = objModel.PossuiFiliais;
            msg0183R1.QuantidadeFiliais = objModel.QuantasFiliais;
            if (objModel.PrazoMedioCompras.HasValue)
                msg0183R1.PrazoMedioCompra = (decimal)objModel.PrazoMedioCompras.Value;
            if (objModel.PrazoMedioVendas.HasValue)
                msg0183R1.PrazoMedioVenda = (decimal)objModel.PrazoMedioVendas.Value;
            if (objModel.Setor.HasValue)
                msg0183R1.Setor = objModel.Setor;
            msg0183R1.ValorMedioCompra = objModel.ValorMedioComprasMensais;
            msg0183R1.ValorMedioVenda = objModel.ValorMedioVendasMensais;
            msg0183R1.VendeAtacadista = objModel.VendasParaAtacadistaVarejista;
            msg0183R1.ObservacaoNotaFiscal = objModel.ObservacoesNF;
            msg0183R1.EstruturaPropriedade = objModel.Propriedade;
            msg0183R1.ReceitaAnual = objModel.ReceitaAnual;

            if (objModel.Status.HasValue)
            {
                msg0183R1.Situacao = objModel.Status.Value;
            }

            if (objModel.Classificacao != null)
                msg0183R1.Classificacao = objModel.Classificacao.Id.ToString();

            if (objModel.Subclassificacao == null)
            {
                throw new ArgumentException("campo Subclassificação obrigatório");
            }

            msg0183R1.SubClassificacao = objModel.Subclassificacao.Id.ToString();

            msg0183R1.CodigoCRM4 = objModel.GUIDCRM40;

            if (objModel.Portador != null)
            {
                Domain.Model.Portador portador = new Intelbras.CRM2013.Domain.Servicos.PortadorService(this.Organizacao, this.IsOffline)
                    .BuscaPorCodigo(objModel.Portador.Id);

                if (portador == null || !portador.CodigoPortador.HasValue)
                {
                    throw new ArgumentException("(CRM) Portador não localizado. (ID: " + objModel.Portador.Id + ")");
                }

                msg0183R1.Portador = portador.CodigoPortador;
            }

            if (objModel.NivelPosVendas == null)
            {
                throw new ArgumentException("Campo nivel pós venda obrigatório");
            }

            msg0183R1.NivelPosVenda = objModel.NivelPosVendas.Id.ToString();

            if (objModel.ClientePotencialOriginador != null)
                msg0183R1.ClientePotencialOriginador = objModel.ClientePotencialOriginador.Id.ToString();
            if (objModel.CondicaoPagamento != null)
            {
                CondicaoPagamento codPagto = new Servicos.CondicaoPagamentoService(this.Organizacao, this.IsOffline).BuscaCondicaoPagamento(objModel.CondicaoPagamento.Id);
                if (codPagto != null && codPagto.Codigo.HasValue)
                {
                    msg0183R1.CondicaoPagamento = codPagto.Codigo;
                }
            }

            Domain.Model.Usuario proprietario = new Domain.Servicos.UsuarioService(this.Organizacao, this.IsOffline).BuscarProprietario("account", "accountid", objModel.ID.Value);
            if (proprietario != null)
            {
                msg0183R1.Proprietario = proprietario.Id.ToString();
                msg0183R1.TipoProprietario = "systemuser";
            }

            if (objModel.Transportadora != null)
            {
                Transportadora transp = new Servicos.TransportadoraService(this.Organizacao, this.IsOffline).ObterPor(objModel.Transportadora.Id);
                if (transp != null && transp.Codigo.HasValue)
                {
                    msg0183R1.Transportadora = transp.Codigo.Value;
                }
            }
            if (objModel.TransportadoraRedespacho != null)
            {
                Transportadora transpRed = new Servicos.TransportadoraService(this.Organizacao, this.IsOffline).ObterPor(objModel.TransportadoraRedespacho.Id);
                if (transpRed != null && transpRed.Codigo.HasValue)
                {
                    msg0183R1.TransportadoraRedespacho = transpRed.Codigo.Value;
                }
            }

            if (objModel.TipoConta.HasValue)
                msg0183R1.TipoConta = objModel.TipoConta.Value;
            msg0183R1.ApuracaoBeneficio = objModel.ApuracaoBeneficiosCompromissos;

            if (objModel.DataAdesao.HasValue)
                msg0183R1.DataAdesao = objModel.DataAdesao.Value.ToLocalTime();


            if (objModel.DiasAtraso.HasValue)
                msg0183R1.NumeroDiasAtraso = (int)objModel.DiasAtraso;

            if (objModel.ReceitaPadrao != null)
            {
                ReceitaPadrao receitaPadrao = new Servicos.ReceitaPadraoService(this.Organizacao, this.IsOffline).BuscaPorCodigo(objModel.ReceitaPadrao.Id);
                if (receitaPadrao != null)
                    msg0183R1.ReceitaPadrao = receitaPadrao.CodReceitaPadrao;
            }
            if (objModel.Portador != null)
            {
                Portador mPortador = new Servicos.PortadorService(this.Organizacao, this.IsOffline).BuscaPorCodigo(objModel.Portador.Id);
                if (mPortador != null)
                    msg0183R1.Portador = mPortador.CodigoPortador;
            }

            msg0183R1.SistemaGestao = objModel.SoftwareNegocios;
            msg0183R1.DataImplantacao = objModel.DataImplantacao;
            msg0183R1.DataUltimoSellOut = objModel.DataUltimoSelloutRevenda;
            msg0183R1.EscolheuDistrForaSellOut = objModel.EscolheuDistribuidorSemSellout;
            msg0183R1.AdesaoPciRealizadaPor = objModel.AdesaoAoPCIRealizadaPor;
            msg0183R1.ParticipaProgramaCanaisMotivo = objModel.MotivoParticipantePrograma;
            msg0183R1.FiguraNoSite = objModel.FiguraNoSite;
            msg0183R1.CNAE = objModel.CNAE;
            msg0183R1.RamoAtividadeEconomica = objModel.RamoAtividadeEconomica;

            if (objModel.CnaeId != null) msg0183R1.CodigoRamoAtividadeEconomica = objModel.CnaeId.Id.ToString();
            if (objModel.ListaPreco != null) msg0183R1.ListaPreco = objModel.ListaPreco.Name;
            if (objModel.TipoConstituicao.HasValue) msg0183R1.TipoConstituicao = objModel.TipoConstituicao.Value;

            //Atualizacao Konviva
            if (objModel.TipoConstituicao.HasValue && objModel.TipoConstituicao.Value == (int)Domain.Enum.Conta.TipoConstituicao.Estrangeiro)
                msg0183R1.CodigoEstrangeiro = objModel.CpfCnpj;

            if (objModel.OrigemConta.HasValue)
                msg0183R1.OrigemConta = objModel.OrigemConta;

            if (!String.IsNullOrEmpty(objModel.NumeroPassaporte))
                msg0183R1.NumeroPassaporte = objModel.NumeroPassaporte;

            if (objModel.StatusIntegracaoSefaz.HasValue)
                msg0183R1.StatusIntegracaoSefaz = objModel.StatusIntegracaoSefaz;

            if (objModel.DataHoraIntegracaoSefaz.HasValue)
                msg0183R1.DataHoraIntegracaoSefaz = objModel.DataHoraIntegracaoSefaz;

            if (!String.IsNullOrEmpty(objModel.RegimeApuracao))
                msg0183R1.RegimeApuracao = objModel.RegimeApuracao;

            if (objModel.DataBaixaContribuinte.HasValue)
                msg0183R1.DataBaixaContribuinte = objModel.DataBaixaContribuinte;

            #region Segmentos do Canal
            msg0183R1.SegmentosCanal = new List<Message.Helper.Entities.Segmento>();

            List<Segmento> lstSegmentos = new RepositoryService(this.Organizacao, this.IsOffline).Segmento.ListarPorContaSegmento((Guid)objModel.ID.Value);

            if (lstSegmentos.Count > 0)
                foreach (Segmento segmentoItem in lstSegmentos)
                {
                    Message.Helper.Entities.Segmento segmento = new Message.Helper.Entities.Segmento();

                    segmento.CodigoSegmento = segmentoItem.CodigoSegmento;
                    segmento.NomeSegmento = segmentoItem.Nome;

                    UnidadeNegocio unidadeNegocio = new UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocio(segmentoItem.UnidadeNegocios.Id);
                    if (unidadeNegocio != null)
                        segmento.CodigoUnidadeNegocio = unidadeNegocio.ChaveIntegracao;

                    msg0183R1.SegmentosCanal.Add(segmento);
                }
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
            msg0183R1.EnderecoPrincipal = endPrincipal;

            msg0183R1.Resultado.Sucesso = true;
            msg0183R1.Resultado.Mensagem = "Integração ocorrida com sucesso.";

            return msg0183R1;


            #endregion

            #endregion
        }

        public Conta DefinirPropriedades(Pollux.MSG0183 xml)
        {
            if (string.IsNullOrEmpty(xml.CpfCnpjCodEstrangeiro))
            {
                throw new ArgumentException("(CRM) O campo 'CpfCnpjCodEstrangeiro' é obrigatório!");
            }

            Conta conta = new Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaContaPorCpfCnpj(xml.CpfCnpjCodEstrangeiro);

            return conta;
        }

        public string Enviar(Conta objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}