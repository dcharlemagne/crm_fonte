using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using System.Reflection;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0151 : Base, IBase<Message.Helper.MSG0151, Domain.Model.Parecer>
    {

        #region Construtor

        public MSG0151(string org, bool isOffline)
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
            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";

            Pollux.Entities.ObterParecer Objeto = this.DefinirRetorno(this.CarregarMensagem<Pollux.MSG0151>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0151R1>(numeroMensagem, retorno);
            }

            //Pollux.MSG0151R1 resposta = new Pollux.MSG0151R1(mensagem, numeroMensagem);
            //resposta.Parecer = Objeto;
            //resposta.Resultado.CodigoErro = resultadoPersistencia.CodigoErro;
            //resposta.Resultado.Mensagem = resultadoPersistencia.
            if (Objeto != null)
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
                retorno.Add("Parecer", Objeto);
                retorno.Add("Resultado", resultadoPersistencia);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "ObterParecer não encontrado no Crm.";
                retorno.Add("Resultado", resultadoPersistencia);

            }
            return CriarMensagemRetorno<Pollux.MSG0151R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public Parecer DefinirPropriedades(Intelbras.Message.Helper.MSG0151 xml)
        {
            Parecer retorno = new Parecer(this.Organizacao, this.IsOffline);
            return retorno;
        }
        public Pollux.Entities.ObterParecer DefinirRetorno(Intelbras.Message.Helper.MSG0151 xml)
        {
            #region Propriedades Crm->Xml
            Pollux.Entities.ObterParecer retornoParecer = new Pollux.Entities.ObterParecer();

            if (string.IsNullOrEmpty(xml.CodigoParecer))
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Valor do parâmetro 'Código Parecer' é obrigatório";
                return retornoParecer;
            }

            Parecer objeto = new Intelbras.CRM2013.Domain.Servicos.ParecerService(this.Organizacao, this.IsOffline).ObterPor(new Guid(xml.CodigoParecer));
            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Valor do parâmetro 'Código Parecer' não existe";
                return retornoParecer;
            }

            if (objeto.DispoedeSuporteTecnico.HasValue)
            {
                retornoParecer.PossuiSuporteTecnico = objeto.DispoedeSuporteTecnico.Value;
                retornoParecer.NomePossuiSuporteTecnico = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.DispoedeSuporteTecnico.Value);
            }

            if (!string.IsNullOrEmpty(objeto.Distribuidores))
                retornoParecer.Distribuidores = objeto.Distribuidores;

            if (objeto.KeyAccountouRepresentante != null)
            {
                Contato contatoObjeto = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(objeto.KeyAccountouRepresentante.Id);
                if (contatoObjeto != null)
                {
                    retornoParecer.CodigoRepresentante = Convert.ToInt32(contatoObjeto.CodigoRepresentante);
                    if (contatoObjeto.PrimeiroNome == null)
                        retornoParecer.NomeRepresentante = (string)this.PreencherAtributoVazio("string");
                    else
                        retornoParecer.NomeRepresentante = contatoObjeto.PrimeiroNome;
                }
                else
                {
                    retornoParecer.NomeRepresentante = (string)this.PreencherAtributoVazio("string");
                    retornoParecer.CodigoRepresentante = (int)this.PreencherAtributoVazio("int");
                }
            }
            else
            {
                retornoParecer.NomeRepresentante = (string)this.PreencherAtributoVazio("string");
                retornoParecer.CodigoRepresentante = (int)this.PreencherAtributoVazio("int");
            }

            if (!string.IsNullOrEmpty(objeto.Nome))
            {
                retornoParecer.NomeParecer = objeto.Nome;
            }
            else
                retornoParecer.NomeParecer = (string)this.PreencherAtributoVazio("string");

            if (objeto.AprovadopeloREDIR.HasValue)
            {
                retornoParecer.AprovadoRedir = objeto.AprovadopeloREDIR.Value;
                retornoParecer.NomeAprovadoRedir = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.AprovadopeloREDIR.Value);
            }

            if (objeto.AtuacomVendaparaClientesFinais.HasValue)
            {
                retornoParecer.VendeClienteFinal = objeto.AtuacomVendaparaClientesFinais.Value;
                retornoParecer.NomeVendeClienteFinal = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.AtuacomVendaparaClientesFinais.Value);
            }

            if (objeto.DispoeCompetenciaTecnicaeComercial.HasValue)
            {
                retornoParecer.PossuiCompetenciaTecnica = objeto.DispoeCompetenciaTecnicaeComercial.Value;
                retornoParecer.NomePossuiCompetenciaTecnica = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.DispoeCompetenciaTecnicaeComercial.Value);
            }

            if (objeto.AceitouEnviarInformacoesdeSellouteEstoque.HasValue)
            {
                retornoParecer.AceitouEnviarSellout = objeto.AceitouEnviarInformacoesdeSellouteEstoque.Value;
                retornoParecer.NomeAceitouEnviarSellout = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.AceitouEnviarInformacoesdeSellouteEstoque.Value);
            }

            if (objeto.CapitalProprio.HasValue)
                retornoParecer.ValorCapitalProprio = objeto.CapitalProprio.Value;

            if (!String.IsNullOrEmpty(objeto.FundamentacaoDistribuidorAprovado))
                retornoParecer.FundamentacaoAprovacao = objeto.FundamentacaoDistribuidorAprovado;

            retornoParecer.PorcentagemCapitalProprio = objeto.CapitalProprioPct;

            if (objeto.VolumeTotalAnual.HasValue)
                retornoParecer.VolumeAnual = objeto.VolumeTotalAnual;


            if (objeto.Status == null)
            {
                retornoParecer.Situacao = (int)this.PreencherAtributoVazio("int");
                retornoParecer.NomeSituacao = (string)this.PreencherAtributoVazio("string");
            }
            else
            {
                retornoParecer.Situacao = objeto.Status.Value;
                if (objeto.Status.Value == 0)
                    retornoParecer.NomeSituacao = "Ativo";
                else
                    retornoParecer.NomeSituacao = "Inativo";

            }

            if (objeto.CapitaldeTerceiros.HasValue)
                retornoParecer.ValorCapitalTerceiro = objeto.CapitaldeTerceiros;

            if (!String.IsNullOrEmpty(objeto.QuaisosImpactosdeAberturadoDistribuidor))
                retornoParecer.DescricaoImpactoAbertura = objeto.QuaisosImpactosdeAberturadoDistribuidor;

            if (objeto.Condicoesatendempadraominimo.HasValue)
            {
                retornoParecer.AtendeCondicaoMinima = objeto.Condicoesatendempadraominimo;
                retornoParecer.NomeAtendeCondicaoMinima = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.Condicoesatendempadraominimo.Value);
            }

            if (objeto.Canal != null)
            {
                retornoParecer.CodigoConta = objeto.Canal.Id.ToString();
                retornoParecer.NomeConta = objeto.Canal.Name;
            }
            else
            {
                retornoParecer.CodigoConta = this.PreencherAtributoVazio("Guid").ToString();
                retornoParecer.NomeConta = (string)this.PreencherAtributoVazio("string");
            }

            if (!String.IsNullOrEmpty(objeto.HademandaporLinhasdeProdutosnosMunicipios))
                retornoParecer.DescricaoDemandaProduto = objeto.HademandaporLinhasdeProdutosnosMunicipios;

            if (objeto.DispoedeSistemadeGestaoAdmeFinanceira.HasValue)
            {
                retornoParecer.PossuiSistemaGestao = objeto.DispoedeSistemadeGestaoAdmeFinanceira;
                retornoParecer.NomePossuiSistemaGestao = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.DispoedeSistemadeGestaoAdmeFinanceira.Value);
            }

            if (!String.IsNullOrEmpty(objeto.ObservacoesdoGerenteNacional))
            {
                retornoParecer.ObservacaoGerente = objeto.ObservacoesdoGerenteNacional;
            }

            if (objeto.Envioudocumentacaocompleta.HasValue)
            {
                retornoParecer.EnvioDocumentacao = objeto.Envioudocumentacaocompleta;
                retornoParecer.NomeEnvioDocumentacao = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.Envioudocumentacaocompleta.Value);
            }

            if (!String.IsNullOrEmpty(objeto.PorqueDefendeAberturadesseDistribuidor))
                retornoParecer.DefesaAbertura = objeto.PorqueDefendeAberturadesseDistribuidor;


            if (objeto.Tarefa != null)
            {
                retornoParecer.CodigoTarefa = objeto.Tarefa.Id.ToString();
                retornoParecer.NomeTarefa = objeto.Tarefa.Name;
            }
            else
            {
                retornoParecer.CodigoTarefa = this.PreencherAtributoVazio("Guid").ToString();
                retornoParecer.NomeTarefa = (string)this.PreencherAtributoVazio("string");
            }

            retornoParecer.PorcentagemCapitalTerceiro = objeto.CapitaldeTerceirosPct;

            if (!String.IsNullOrEmpty(objeto.Fundamentacao))
                retornoParecer.FundamentacaoAdequacao = objeto.Fundamentacao;

            if (objeto.PotencialdaRegiao.HasValue)
                retornoParecer.ValorPotencialRegiao = objeto.PotencialdaRegiao.Value;

            if (usuarioIntegracao != null)
            {
                retornoParecer.Proprietario = usuarioIntegracao.ID.Value.ToString();
                retornoParecer.NomeProprietario = usuarioIntegracao.Nome;
                retornoParecer.TipoProprietario = "systemuser";
            }
            else
            {
                retornoParecer.Proprietario = (string)this.PreencherAtributoVazio("guid").ToString();
                retornoParecer.NomeProprietario = (string)this.PreencherAtributoVazio("string");
                retornoParecer.TipoProprietario = (string)this.PreencherAtributoVazio("string");

            }

            if (objeto.IraatuarsomentecomprodutosIntelbras.HasValue)
            {
                retornoParecer.ExclusivoIntelbras = objeto.IraatuarsomentecomprodutosIntelbras;
                retornoParecer.NomeExclusivoIntelbras = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.IraatuarsomentecomprodutosIntelbras.Value);
            }

            if (objeto.ParecerKeyAccountRepresentante.HasValue)
            {
                retornoParecer.PossuiParecerRepresentante = objeto.ParecerKeyAccountRepresentante;
                retornoParecer.NomePossuiParecerRepresentante = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.ParecerKeyAccountRepresentante.Value);

            }

            if (objeto.AprovadoPeloComite.HasValue)
            {
                retornoParecer.AprovadoPeloComite = objeto.AprovadoPeloComite;
                retornoParecer.NomeAprovadoPeloComite = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.AprovadoPeloComite.Value);
            }

            if (objeto.FaturamentoDiretoparaaRegiao.HasValue)
                retornoParecer.ValorFaturamentoRegiao = objeto.FaturamentoDiretoparaaRegiao;

            if (objeto.FichadeAvaliacaodoDistribuidor.HasValue)
            {
                retornoParecer.EnviouFichaCadastral = objeto.FichadeAvaliacaodoDistribuidor;
                retornoParecer.NomeEnviouFichaCadastral = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.FichadeAvaliacaodoDistribuidor.Value);
            }

            if (!String.IsNullOrEmpty(objeto.MotivosprincipaisdeaberturadoDistribuidor))
                retornoParecer.MotivoAberturaDistribuidor = objeto.MotivosprincipaisdeaberturadoDistribuidor;

            if (objeto.DispostoaAtuarDentrodasNovasPraticas.HasValue)
            {
                retornoParecer.AceitaNovasPraticas = objeto.DispostoaAtuarDentrodasNovasPraticas;
                retornoParecer.NomeAceitaNovasPraticas = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.DispostoaAtuarDentrodasNovasPraticas.Value);
            }

            if (!String.IsNullOrEmpty(objeto.PrevisaoLinhadeCorteMinima))
                retornoParecer.PrevisaoLinhaCorte = objeto.PrevisaoLinhadeCorteMinima;

            if (objeto.ParecerSetorFinanceiro.HasValue)
            {
                retornoParecer.PossuiParecerSetorFinanceiro = objeto.ParecerSetorFinanceiro;
                retornoParecer.NomePossuiParecerSetorFinanceiro = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.ParecerSetorFinanceiro.Value);
            }

            if (!String.IsNullOrEmpty(objeto.TeremosConflitoscomosDistribuidores))
                retornoParecer.ConflitoDistribuidores = objeto.TeremosConflitoscomosDistribuidores;

            if (!String.IsNullOrEmpty(objeto.ObservacoesKeyAccountRepres))
                retornoParecer.ObservacaoRepresentante = objeto.ObservacoesKeyAccountRepres;

            if (objeto.QualLimitedeCreditoLiberado.HasValue)
                retornoParecer.LimiteCreditoLiberado = objeto.QualLimitedeCreditoLiberado;

            if (objeto.ParecerdoGerenteNacionaldeVendas.HasValue)
            {
                retornoParecer.PossuiParecerGerente = objeto.ParecerdoGerenteNacionaldeVendas;
                retornoParecer.NomePossuiParecerGerente = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.ParecerdoGerenteNacionaldeVendas.Value);
            }
            // no crm nao estava campo obrigatorio
            retornoParecer.PorcentagemRegiao = objeto.FaturamentoPorcentagemRegiao;

            if (objeto.DistribuidorAprovado.HasValue)
            {
                retornoParecer.DistribuidorAprovado = objeto.DistribuidorAprovado;
                retornoParecer.NomeDistribuidorAprovado = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.DistribuidorAprovado.Value);
            }

            if (objeto.AtuacomVendasemNotaFiscal.HasValue)
            {
                retornoParecer.AtuaVendaSemNota = objeto.AtuacomVendasemNotaFiscal;
                retornoParecer.NomeAtuaVendaSemNota = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.AtuacomVendasemNotaFiscal.Value);
            }

            if (!String.IsNullOrEmpty(objeto.ObservacoesSetorFinanceiro))
                retornoParecer.ObservacaoSetorFinaceiro = objeto.ObservacoesSetorFinanceiro;

            if (objeto.DistribuidorAdequado.HasValue)
            {
                retornoParecer.DistribuidorAdequado = objeto.DistribuidorAdequado;
                retornoParecer.NomeDistribuidorAdequado = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.DistribuidorAdequado.Value);
            }

            retornoParecer.PorcentagemPotencialRegiao = objeto.PotencialPorcentagem;

            if (objeto.PossuiExperienciade5anosemDistribuicao.HasValue)
            {
                retornoParecer.PossuiExperiencia = objeto.PossuiExperienciade5anosemDistribuicao;
                retornoParecer.NomePossuiExperiencia = new Intelbras.CRM2013.Domain.Enum.Parecer().pegarNomeEnum(objeto.PossuiExperienciade5anosemDistribuicao.Value);
            }

            retornoParecer.PorcentagemFaturamento = objeto.PorcentagemFaturamento;

            if (objeto.TipodoParecer.HasValue)
            {
                retornoParecer.TipoParecer = objeto.TipodoParecer.Value;
                retornoParecer.NomeTipoParecer = new Intelbras.CRM2013.Domain.Enum.Parecer().TipoParecerNome(objeto.TipodoParecer.Value);
            }
            else
            {
                retornoParecer.TipoParecer = (int)this.PreencherAtributoVazio("int");
                retornoParecer.NomeTipoParecer = this.PreencherAtributoVazio("string").ToString();
            }

            return retornoParecer;





            #endregion
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Parecer objModel)
        {
            string resposta = string.Empty;

            return resposta;
        }

        #endregion

        Parecer IBase<Pollux.MSG0151, Parecer>.DefinirPropriedades(Pollux.MSG0151 legado)
        {
            throw new NotImplementedException();
        }
    }
}
