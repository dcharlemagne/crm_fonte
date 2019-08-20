using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0149 : Base, IBase<Message.Helper.MSG0149, Domain.Model.Parecer>
    {

        #region Construtor

        public MSG0149(string org, bool isOffline)
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


            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0149>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0149R1>(numeroMensagem, retorno);
            }

            objeto = new Servicos.ParecerService(this.Organizacao, this.IsOffline).Persistir(objeto);

            if (objeto != null)
            {
                retorno.Add("CodigoParecer", objeto.ID.Value.ToString());
                if (usuario != null)
                {
                    retorno.Add("Proprietario", usuario.ID.Value.ToString());
                    retorno.Add("TipoProprietario", "systemuser");
                }
                retorno.Add("Resultado", resultadoPersistencia);
            }
            else 
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro de Persistência!";
                retorno.Add("Resultado", resultadoPersistencia);
            }
            return CriarMensagemRetorno<Pollux.MSG0149R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public Parecer DefinirPropriedades(Intelbras.Message.Helper.MSG0149 xml)
        {
            var crm = new Parecer(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            if (!string.IsNullOrEmpty(xml.CodigoParecer))
                crm.ID = new Guid(xml.CodigoParecer);

            if (xml.PossuiSuporteTecnico.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Parecer.DispoeSuporteTecnico), xml.PossuiSuporteTecnico))
                {
                    crm.DispoedeSuporteTecnico = xml.PossuiSuporteTecnico;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Indentificador do Possui Suporte Técnico não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("DispoedeSuporteTecnico");
            }

            if (!string.IsNullOrEmpty(xml.Distribuidores))
                crm.Distribuidores = xml.Distribuidores;
            else
                crm.AddNullProperty("Distribuidores");

            Contato objContato = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContatoPorCodigoRepresentante(xml.CodigoRepresentante.ToString());
            if (objContato != null)
            {
                crm.KeyAccountouRepresentante = new Lookup(objContato.ID.Value, "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Indentificador do Key Account Representante não encontrado.";
                return crm;
            }

            if (!string.IsNullOrEmpty(xml.NomeParecer))
                crm.Nome = xml.NomeParecer;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Campo Nome obrigatório.";
                return crm;
            }

            if (xml.AprovadoRedir.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Parecer.AprovadoRedir), xml.AprovadoRedir))
                {
                    crm.AprovadopeloREDIR = xml.AprovadoRedir;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Indentificador do Aprovado Redir não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("AprovadopeloREDIR");
            }

            if (xml.VendeClienteFinal.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Parecer.AtuaVendasClientesFinais), xml.VendeClienteFinal))
                {
                    crm.AtuacomVendaparaClientesFinais = xml.VendeClienteFinal;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Indentificador do 'Atua com Venda Para Clientes Finais' não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("AtuacomVendaparaClientesFinais");
            }

            if (xml.PossuiCompetenciaTecnica.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Parecer.CompetenciaTecnica), xml.PossuiCompetenciaTecnica))
                {
                    crm.DispoeCompetenciaTecnicaeComercial = xml.PossuiCompetenciaTecnica;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Indentificador do 'Possui Competência Técnica' não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("DispoeCompetenciaTecnicaeComercial");
            }

            if (xml.AceitouEnviarSellout.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Parecer.EnviaSellOut), xml.AceitouEnviarSellout))
                {
                    crm.AceitouEnviarInformacoesdeSellouteEstoque = xml.AceitouEnviarSellout;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Indentificador do 'Aceitou Enviar Sell Out' não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("AceitouEnviarInformacoesdeSellouteEstoque");
            }

            if (xml.ValorCapitalProprio.HasValue)
                crm.CapitalProprio = xml.ValorCapitalProprio;
            else
                crm.AddNullProperty("CapitalProprio");

            if (xml.PorcentagemCapitalProprio.HasValue)
                crm.CapitalProprioPct = xml.PorcentagemCapitalProprio.Value;
            else
                crm.AddNullProperty("CapitalProprioPct");

            if (xml.VolumeAnual.HasValue)
                crm.VolumeTotalAnual = xml.VolumeAnual;
            else
                crm.AddNullProperty("VolumeTotalAnual");

            crm.Status = xml.Situacao;

            if (xml.ValorCapitalTerceiro.HasValue)
                crm.CapitaldeTerceiros = xml.ValorCapitalTerceiro.Value;
            else
                crm.AddNullProperty("CapitaldeTerceiros");

            if (!String.IsNullOrEmpty(xml.DescricaoImpactoAbertura))
                crm.QuaisosImpactosdeAberturadoDistribuidor = xml.DescricaoImpactoAbertura;
            else
                crm.AddNullProperty("QuaisosImpactosdeAberturadoDistribuidor");

            if (xml.AtendeCondicaoMinima.HasValue)
                crm.Condicoesatendempadraominimo = xml.AtendeCondicaoMinima;
            else
                crm.AddNullProperty("Condicoesatendempadraominimo");

            if (!string.IsNullOrEmpty(xml.CodigoConta) && xml.CodigoConta.Length == 36)
            {
                crm.Canal = new Lookup(new Guid(xml.CodigoConta), "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Indentificador do 'Canal' não encontrado.";
                return crm;
            }

            if (!string.IsNullOrEmpty(xml.DescricaoDemandaProduto))
                crm.HademandaporLinhasdeProdutosnosMunicipios = xml.DescricaoDemandaProduto;
            else
                crm.AddNullProperty("HademandaporLinhasdeProdutosnosMunicipios");

            if (xml.PossuiSistemaGestao.HasValue)
                crm.DispoedeSistemadeGestaoAdmeFinanceira = xml.PossuiSistemaGestao;
            else
                crm.AddNullProperty("DispoedeSistemadeGestaoAdmeFinanceira");

            if (!string.IsNullOrEmpty(xml.ObservacaoGerente))
                crm.ObservacoesdoGerenteNacional = xml.ObservacaoGerente;
            else
                crm.AddNullProperty("ObservacoesdoGerenteNacional");

            if (xml.EnvioDocumentacao.HasValue)
                crm.Envioudocumentacaocompleta = xml.EnvioDocumentacao;
            else
                crm.AddNullProperty("Envioudocumentacaocompleta");

            if (!string.IsNullOrEmpty(xml.DefesaAbertura))
                crm.PorqueDefendeAberturadesseDistribuidor = xml.DefesaAbertura;
            else
                crm.AddNullProperty("PorqueDefendeAberturadesseDistribuidor");

            if (!String.IsNullOrEmpty(xml.CodigoTarefa) && xml.CodigoTarefa.Length == 36)
            {
                crm.Tarefa = new Lookup(new Guid(xml.CodigoTarefa), "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Campo 'Código Tarefa' obrigatório.";
                return crm;
            }

            if (xml.PorcentagemCapitalTerceiro.HasValue)
                crm.CapitaldeTerceirosPct = xml.PorcentagemCapitalTerceiro.Value;
            else
                crm.AddNullProperty("CapitaldeTerceirosPct");

            if (!String.IsNullOrEmpty(xml.FundamentacaoAdequacao))
                crm.FundamentacaoDistribuidorAprovado = xml.FundamentacaoAdequacao;
            else
                crm.AddNullProperty("FundamentacaoDistribuidorAprovado");

            if (!String.IsNullOrEmpty(xml.FundamentacaoAprovacao))
                crm.Fundamentacao = xml.FundamentacaoAprovacao;
            else
                crm.AddNullProperty("Fundamentacao");

            if (xml.ValorPotencialRegiao.HasValue)
                crm.PotencialdaRegiao = xml.ValorPotencialRegiao;
            else
                crm.AddNullProperty("PotencialdaRegiao");

            if (xml.ExclusivoIntelbras.HasValue)
                crm.IraatuarsomentecomprodutosIntelbras = xml.ExclusivoIntelbras;
            else
                crm.AddNullProperty("IraatuarsomentecomprodutosIntelbras");

            if (xml.PossuiParecerRepresentante.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Parecer.ParecerKARepresentante), xml.PossuiParecerRepresentante))
                    crm.ParecerKeyAccountRepresentante = xml.PossuiParecerRepresentante;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Indentificador do 'Parecer Key Account Representante' não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("ParecerKeyAccountRepresentante");
            }

            if (xml.AprovadoPeloComite.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Parecer.AprovadoComite), xml.AprovadoPeloComite))
                    crm.AprovadoPeloComite = xml.AprovadoPeloComite;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Indentificador do 'Aprovado pelo Comitê' não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("AprovadoPeloComite");
            }

            if (xml.ValorFaturamentoRegiao.HasValue)
                crm.FaturamentoDiretoparaaRegiao = xml.ValorFaturamentoRegiao;
            else
                crm.AddNullProperty("FaturamentoDiretoparaaRegiao");

            if (xml.EnviouFichaCadastral.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Parecer.FichaAvalicaoDistribuidor), xml.EnviouFichaCadastral))
                    crm.FichadeAvaliacaodoDistribuidor = xml.EnviouFichaCadastral;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Indentificador do 'Ficha Avaliação Distribuidor' não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("FichadeAvaliacaodoDistribuidor");
            }

            if (!String.IsNullOrEmpty(xml.MotivoAberturaDistribuidor))
                crm.MotivosprincipaisdeaberturadoDistribuidor = xml.MotivoAberturaDistribuidor;
            else
                crm.AddNullProperty("MotivosprincipaisdeaberturadoDistribuidor");

            if (xml.AceitaNovasPraticas.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Parecer.DispostoAtuarNovasPraticas), xml.AceitaNovasPraticas))
                    crm.DispostoaAtuarDentrodasNovasPraticas = xml.AceitaNovasPraticas;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Indentificador do 'Disposto Atuar Novas Práticas' não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("DispostoaAtuarDentrodasNovasPraticas");
            }

            if (!String.IsNullOrEmpty(xml.PrevisaoLinhaCorte))
                crm.PrevisaoLinhadeCorteMinima = xml.PrevisaoLinhaCorte;
            else
                crm.AddNullProperty("PrevisaoLinhadeCorteMinima");

            if (xml.PossuiParecerSetorFinanceiro.HasValue)
            { 
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Parecer.ParecerSetorFinanc), xml.PossuiParecerSetorFinanceiro))
                    crm.ParecerSetorFinanceiro = xml.PossuiParecerSetorFinanceiro;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Indentificador do 'Parecer Setor Financeiro' não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("ParecerSetorFinanceiro");
            }

            if (!String.IsNullOrEmpty(xml.ConflitoDistribuidores))
                crm.TeremosConflitoscomosDistribuidores = xml.ConflitoDistribuidores;
            else
                crm.AddNullProperty("TeremosConflitoscomosDistribuidores");

            if (!String.IsNullOrEmpty(xml.ObservacaoRepresentante))
                crm.ObservacoesKeyAccountRepres = xml.ObservacaoRepresentante;
            else
                crm.AddNullProperty("ObservacoesKeyAccountRepres");

            if (xml.LimiteCreditoLiberado.HasValue)
                crm.QualLimitedeCreditoLiberado = xml.LimiteCreditoLiberado;
            else
                crm.AddNullProperty("QualLimitedeCreditoLiberado");

            if (xml.PossuiParecerGerente.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Parecer.ParecerGerenteNacVendas), xml.PossuiParecerGerente))
                    crm.ParecerdoGerenteNacionaldeVendas = xml.PossuiParecerGerente;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Indentificador do 'Parecer Gerente Nacional de Vendas' não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("ParecerdoGerenteNacionaldeVendas");
            }

            if (xml.PorcentagemRegiao.HasValue)
                crm.FaturamentoPorcentagemRegiao = xml.PorcentagemRegiao.Value;
            else
                crm.AddNullProperty("FaturamentoPorcentagemRegiao");

            if (xml.DistribuidorAprovado.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Parecer.DistribuidorAprovado), xml.DistribuidorAprovado))
                    crm.DistribuidorAprovado = xml.DistribuidorAprovado;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Indentificador do 'Distribuidor Aprovado' não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("DistribuidorAprovado");
            }

            if (xml.AtuaVendaSemNota.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Parecer.VendaSemNotaFiscal), xml.AtuaVendaSemNota))
                    crm.AtuacomVendasemNotaFiscal = xml.AtuaVendaSemNota;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Indentificador do 'Atua Sem Nota Fiscal' não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("AtuacomVendasemNotaFiscal");
            }

            if (!String.IsNullOrEmpty(xml.ObservacaoSetorFinaceiro))
                crm.ObservacoesSetorFinanceiro = xml.ObservacaoSetorFinaceiro;
            else
                crm.AddNullProperty("ObservacoesSetorFinanceiro");

            if (xml.DistribuidorAdequado.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Parecer.DistribuidorAdequado), xml.DistribuidorAdequado))
                    crm.DistribuidorAdequado = xml.DistribuidorAdequado;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Indentificador do 'Distribuidor Adequado' não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("DistribuidorAdequado");
            }

            if (xml.PorcentagemPotencialRegiao.HasValue)
                crm.PotencialPorcentagem = xml.PorcentagemPotencialRegiao.Value;
            else
                crm.AddNullProperty("PotencialPorcentagem");

            if (xml.PossuiExperiencia.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Parecer.Experiencia5AnosDistri), xml.PossuiExperiencia))
                    crm.PossuiExperienciade5anosemDistribuicao = xml.PossuiExperiencia;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Indentificador do 'Possui Experiência de 5 anos em distribuição' não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("PossuiExperienciade5anosemDistribuicao");
            }
            //
            if (xml.PorcentagemFaturamento.HasValue)
                crm.PorcentagemFaturamento = xml.PorcentagemFaturamento.Value;
            else
                crm.AddNullProperty("PorcentagemFaturamento");

            if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Parecer.TipoParecer), xml.TipoParecer))
                crm.TipodoParecer = xml.TipoParecer;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Indentificador do 'Tipo do Parecer' não encontrado.";
                    return crm;
                }

            string tipoProprietario;

            if (xml.TipoProprietario == "team" || xml.TipoProprietario == "systemuser")
                tipoProprietario = xml.TipoProprietario;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Tipo Proprietário não Enviado.";
                return crm;
            }

            #endregion

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Parecer objModel)
        {
            string resposta = string.Empty;

            return resposta;
        }

        #endregion

    }
}
