using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0147 : Base, IBase<Message.Helper.MSG0147, Domain.Model.SolicitacaoBeneficio>
    {
        public MSG0147(string org, bool isOffline) : base(org, isOffline)
        {
        }

        #region Propriedades

        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Executar

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            try
            {
                usuarioIntegracao = usuario;
                BeneficioDoCanal beneficioCanalConsulta = null;
                List<Pollux.Entities.SolicitacaoBeneficio> lstRetorno = new List<Pollux.Entities.SolicitacaoBeneficio>();

                var xml = this.CarregarMensagem<Pollux.MSG0147>(mensagem);
                //BeneficioCanal
                if (!String.IsNullOrEmpty(xml.CodigoBeneficioCanal) && xml.CodigoBeneficioCanal.Length == 36)
                {
                    beneficioCanalConsulta = new Servicos.BeneficioDoCanalService(this.Organizacao, this.IsOffline).ObterPor(new Guid(xml.CodigoBeneficioCanal));
                    if (beneficioCanalConsulta == null)
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Valor do parâmetro " + xml.CodigoBeneficioCanal + " não existe.";
                        retorno.Add("Resultado", resultadoPersistencia);
                        return CriarMensagemRetorno<Pollux.MSG0147R1>(numeroMensagem, retorno);
                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Parâmetro obrigatório para a consulta não enviado.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0147R1>(numeroMensagem, retorno);
                }
                //Solicitacao Beneficios

                List<SolicitacaoBeneficio> lstSolicitacaoBeneficioCrm = new List<SolicitacaoBeneficio>();
                if (xml.SolicitacaoAjuste.HasValue)
                    lstSolicitacaoBeneficioCrm = new Servicos.SolicitacaoBeneficioService(this.Organizacao, this.IsOffline).ListarPorBeneficioCanalEAjusteSaldo(beneficioCanalConsulta.ID.Value, xml.SolicitacaoAjuste.Value);
                else
                    lstSolicitacaoBeneficioCrm = new Servicos.SolicitacaoBeneficioService(this.Organizacao, this.IsOffline).ListarPorBeneficioCanal(beneficioCanalConsulta.ID.Value);

                if (lstSolicitacaoBeneficioCrm != null && lstSolicitacaoBeneficioCrm.Count > 0)
                {
                    var listaOrdenada = lstSolicitacaoBeneficioCrm.OrderByDescending(t => t.DataCriacao).ToList();
                    lstRetorno = this.DefinirRetorno(listaOrdenada);
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0147R1>(numeroMensagem, retorno);
                }
                if (lstRetorno != null && lstRetorno.Count > 0)
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";
                    retorno.Add("SolicitacaoBeneficioItens", lstRetorno);
                    retorno.Add("Resultado", resultadoPersistencia);
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                    retorno.Add("Resultado", resultadoPersistencia);
                }
                return CriarMensagemRetorno<Pollux.MSG0147R1>(numeroMensagem, retorno);
            }
            catch (Exception e)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(e);
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0147R1>(numeroMensagem, retorno);
            }
        }
        #endregion

        #region Definir Propriedades
        public SolicitacaoBeneficio DefinirPropriedades(Intelbras.Message.Helper.MSG0147 xml)
        {
            SolicitacaoBeneficio retorno = new SolicitacaoBeneficio(Organizacao, IsOffline);
            return retorno;
        }
        public List<Pollux.Entities.SolicitacaoBeneficio> DefinirRetorno(List<Model.SolicitacaoBeneficio> lstSolBeneficioCrm)
        {
            List<Pollux.Entities.SolicitacaoBeneficio> lstRetorno = new List<Pollux.Entities.SolicitacaoBeneficio>();
            #region Propriedades Crm->Xml
            foreach (var itemCrm in lstSolBeneficioCrm)
            {
                var solBeneficioPollux = new Pollux.Entities.SolicitacaoBeneficio();

                if (itemCrm.DataCriacao.HasValue)
                    solBeneficioPollux.DataCriacaoSolicitacao = itemCrm.DataCriacao.Value;
                else
                    solBeneficioPollux.DataCriacaoSolicitacao = DateTime.MinValue;

                solBeneficioPollux.CodigoSolicitacaoBeneficio = itemCrm.ID.Value.ToString();

                if (!String.IsNullOrEmpty(itemCrm.Nome))
                    solBeneficioPollux.NomeSolicitacaoBeneficio = itemCrm.Nome;
                else
                    solBeneficioPollux.NomeSolicitacaoBeneficio = "N/A";

                if (itemCrm.UnidadedeNegocio != null)
                {
                    UnidadeNegocio unidadeNeg = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocio(itemCrm.UnidadedeNegocio.Id);
                    if (unidadeNeg != null)
                    {
                        if (!String.IsNullOrEmpty(unidadeNeg.ChaveIntegracao))
                            solBeneficioPollux.CodigoUnidadeNegocio = unidadeNeg.ChaveIntegracao;
                        else solBeneficioPollux.CodigoUnidadeNegocio = "N/A";
                        if (!String.IsNullOrEmpty(unidadeNeg.Nome))
                            solBeneficioPollux.NomeUnidadeNegocio = unidadeNeg.Nome;
                        else solBeneficioPollux.NomeUnidadeNegocio = "N/A";
                    }
                    else
                    {
                        solBeneficioPollux.CodigoUnidadeNegocio = "N/A";
                        solBeneficioPollux.NomeUnidadeNegocio = "N/A";
                    }
                }
                else
                {
                    solBeneficioPollux.CodigoUnidadeNegocio = "N/A";
                    solBeneficioPollux.NomeUnidadeNegocio = "N/A";
                }
                if (itemCrm.ValorAcao.HasValue)
                    solBeneficioPollux.ValorAcao = itemCrm.ValorAcao.Value;
                else
                    solBeneficioPollux.ValorAcao = 0;
                if (!String.IsNullOrEmpty(itemCrm.SituacaoIrregular))
                    solBeneficioPollux.DescricaoSituacaoIrregular = itemCrm.SituacaoIrregular;
                if (itemCrm.AcaoSubsidiadaVmc != null)
                {
                    solBeneficioPollux.CodigoAcaoSubsidiadaVMC = itemCrm.AcaoSubsidiadaVmc.Id.ToString();
                    solBeneficioPollux.NomeAcaoSubsidiadaVMC = itemCrm.AcaoSubsidiadaVmc.Name;
                }

                if (itemCrm.ValorSolicitado.HasValue)
                    solBeneficioPollux.ValorSolicitado = itemCrm.ValorSolicitado.Value;
                else
                    solBeneficioPollux.ValorSolicitado = 0;

                if (itemCrm.Status.HasValue)
                    solBeneficioPollux.SituacaoSolicitacaoBeneficio = itemCrm.StatusSolicitacao.Value;

                if (itemCrm.FormaPagamento != null)
                {
                    solBeneficioPollux.CodigoFormaPagamento = itemCrm.FormaPagamento.Id.ToString();
                    solBeneficioPollux.NomeFormaPagamento = itemCrm.FormaPagamento.Name;
                }
                else
                {
                    solBeneficioPollux.CodigoFormaPagamento = Guid.Empty.ToString();
                    solBeneficioPollux.NomeFormaPagamento = "N/A";
                }
                if (itemCrm.ValorAprovado.HasValue)
                    solBeneficioPollux.ValorAprovado = itemCrm.ValorAprovado.Value;

                if (itemCrm.TipoPriceProtection.HasValue)
                {
                    solBeneficioPollux.TipoPriceProtection = itemCrm.TipoPriceProtection.Value;
                    if (itemCrm.StatusCalculoPriceProtection.HasValue)
                    {
                        solBeneficioPollux.StatusCalculoPriceProtection = itemCrm.StatusCalculoPriceProtection.Value;
                    }
                }

                if (itemCrm.ResultadoPrevisto.HasValue)
                    solBeneficioPollux.ResultadoPrevisto = itemCrm.ResultadoPrevisto.Value;

                if (itemCrm.ResultadoAlcancado.HasValue)
                    solBeneficioPollux.ResultadoAlcancado = itemCrm.ResultadoAlcancado.Value;

                if (!string.IsNullOrEmpty(itemCrm.TrimestreCompetencia))
                    solBeneficioPollux.TrimestreCompetencia = itemCrm.TrimestreCompetencia;

                if (!string.IsNullOrEmpty(itemCrm.Descricao))
                    solBeneficioPollux.DescricaoSolicitacao = itemCrm.Descricao;

                if (itemCrm.DataIniAcao.HasValue)
                    solBeneficioPollux.DataInicioAcao = itemCrm.DataIniAcao.Value; 

                if (itemCrm.DataFimAcao.HasValue)
                    solBeneficioPollux.DataPrevistaRetornoAcao = itemCrm.DataFimAcao.Value;

                if (itemCrm.ValorPago.HasValue)
                    solBeneficioPollux.ValorPago = itemCrm.ValorPago.Value;
                if (itemCrm.BeneficioPrograma != null)
                {
                    solBeneficioPollux.CodigoBeneficio = itemCrm.BeneficioPrograma.Id.ToString();
                    solBeneficioPollux.NomeBeneficio = itemCrm.BeneficioPrograma.Name;

                    Beneficio benefProg = new Intelbras.CRM2013.Domain.Servicos.BeneficioService(this.Organizacao, this.IsOffline).ObterPor(itemCrm.BeneficioPrograma.Id);
                    if (benefProg != null && benefProg.Codigo.HasValue)
                        solBeneficioPollux.BeneficioCodigo = benefProg.Codigo.Value;
                }
                else
                {
                    solBeneficioPollux.CodigoBeneficio = Guid.Empty.ToString();
                    solBeneficioPollux.NomeBeneficio = "N/A";
                    solBeneficioPollux.BeneficioCodigo = (int)this.PreencherAtributoVazio("int");
                }
                if (itemCrm.AlteradaParaStockRotation.HasValue)
                    solBeneficioPollux.AlteradaStockRotation = itemCrm.AlteradaParaStockRotation.Value;
                else
                    solBeneficioPollux.AlteradaStockRotation = false;
                if (itemCrm.SituacaoIrregularidades.HasValue)
                    solBeneficioPollux.SolicitacaoIrregular = itemCrm.SituacaoIrregularidades.Value;
                else
                    solBeneficioPollux.SolicitacaoIrregular = false;

                if (itemCrm.AjusteSaldo.HasValue)
                    solBeneficioPollux.SolicitacaoAjuste = itemCrm.AjusteSaldo.Value;

                if (itemCrm.ValorCancelado.HasValue)
                    solBeneficioPollux.ValorCancelado = itemCrm.ValorCancelado.Value;

                //Busca as unidades de negócio relacionadas ao Benefício do canal
                List<SolicitacaoXUnidades> lstUnidadesBenef = new Servicos.SolicitacaoXUnidadesService(this.Organizacao, this.IsOffline).ListarPor(itemCrm.ID.Value);
                solBeneficioPollux.UnidadesRelacionadas = this.ConverteLista(lstUnidadesBenef);
                
                lstRetorno.Add(solBeneficioPollux);
            }
            #endregion
            return lstRetorno;
        }

        private List<Pollux.Entities.UnidadeNegocio> ConverteLista(List<SolicitacaoXUnidades> lstUnidadesBenef)
        {
            List<Pollux.Entities.UnidadeNegocio> lstPollux = new List<Pollux.Entities.UnidadeNegocio>();

            foreach (SolicitacaoXUnidades unidades in lstUnidadesBenef)
            {
                Pollux.Entities.UnidadeNegocio Objeto = new Pollux.Entities.UnidadeNegocio();

                Objeto.CodigoUnidadeNegocio = unidades.UnidadeNegocio.ChaveIntegracao;
                Objeto.NomeUnidadeNegocio = unidades.UnidadeNegocio.Nome;

                lstPollux.Add(Objeto);
            }
            return lstPollux;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(SolicitacaoBeneficio objModel)
        {
            string resposta = string.Empty;

            return resposta;
        }

        #endregion

    }
}
