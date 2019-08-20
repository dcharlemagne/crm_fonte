using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0141 : Base, IBase<Message.Helper.MSG0141, Domain.Model.BeneficioDoCanal>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private int? controleConta = null;
        private bool? passivelSolicitacao = null;
        private Guid? unidadeNegocioId = null;
        #endregion

        #region Construtor
        public MSG0141(string org, bool isOffline)
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
            try
            {
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0141>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0141R1>(numeroMensagem, retorno);
            }

            List<Intelbras.Message.Helper.Entities.Beneficio> lstPolluxBeneficio = new List<Pollux.Entities.Beneficio>();
            //SolicitacoesItens
            List<BeneficioDoCanal> lstBeneficioDoCanal = new Servicos.BeneficioDoCanalService(this.Organizacao, this.IsOffline).ListarPorContaUnidadeNegocio(objeto.Canal.Id, unidadeNegocioId);

            if (lstBeneficioDoCanal != null && lstBeneficioDoCanal.Count > 0)
            {
                lstPolluxBeneficio = ConverterListaBeneficio(lstBeneficioDoCanal);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0141R1>(numeroMensagem, retorno);
            }
            
            retorno.Add("BeneficioItens", lstPolluxBeneficio);
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0141R1>(numeroMensagem, retorno);
        }
            catch(Exception e)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(e);
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0141R1>(numeroMensagem, retorno); 
            }
        }
        #endregion

        #region Definir Propriedades

        public BeneficioDoCanal DefinirPropriedades(Intelbras.Message.Helper.MSG0141 xml)
        {
            var crm = new Model.BeneficioDoCanal(this.Organizacao, this.IsOffline);

            if (!String.IsNullOrEmpty(xml.CodigoConta) && xml.CodigoConta.Length == 36)
            {
                Conta conta = new Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(new Guid(xml.CodigoConta));
                if (conta != null)
                {
                    crm.Canal = new Lookup(conta.ID.Value, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Canal - " + xml.CodigoConta + "- não cadastrado no Crm.";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Codigo Conta não enviado ou fora do padrão(Guid).";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.CodigoUnidadeNegocio))
            {
                UnidadeNegocio unidadeNegocio = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocioPorChaveIntegracao(xml.CodigoUnidadeNegocio);
                if (unidadeNegocio == null)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "UnidadeNegocio: " + xml.CodigoUnidadeNegocio + " não encontrado no Crm.";
                    return crm;
                }
                unidadeNegocioId = unidadeNegocio.ID.Value;
            }

            if (xml.PassivelSolicitacao.HasValue)
                passivelSolicitacao = xml.PassivelSolicitacao.Value;

            if (xml.PossuiControleContaCorrente.HasValue
                && System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.BeneficiodoPrograma.ControleContaCorrente), xml.PossuiControleContaCorrente))
                controleConta = xml.PossuiControleContaCorrente;


            
            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(BeneficioDoCanal objModel)
        {
            return String.Empty;
        }


        private List<Intelbras.Message.Helper.Entities.Beneficio> ConverterListaBeneficio(List<BeneficioDoCanal> lstBeneficioDoCanal)
        {
            List<Intelbras.Message.Helper.Entities.Beneficio> lstPolluxBeneficio = new List<Pollux.Entities.Beneficio>();

            foreach (BeneficioDoCanal crmBeneficio in lstBeneficioDoCanal)
            {
                var teste = crmBeneficio.BeneficioObj.Nome;
                //Retirar do retorno caso não bater valores
                if (controleConta.HasValue
                   && crmBeneficio.BeneficioObj.PossuiControleContaCorrente.HasValue
                    && crmBeneficio.BeneficioObj.PossuiControleContaCorrente.Value != controleConta.Value)
                    continue;

                if (passivelSolicitacao.HasValue
                   && crmBeneficio.BeneficioObj.PassivelDeSolicitacao.HasValue
                    && crmBeneficio.BeneficioObj.PassivelDeSolicitacao.Value != passivelSolicitacao.Value)
                    continue;

                Pollux.Entities.Beneficio beneficioPollux = new Pollux.Entities.Beneficio();
                beneficioPollux.CodigoBeneficioCanal = crmBeneficio.ID.Value.ToString();

                if (crmBeneficio.VerbaDisponivel.HasValue)
                    beneficioPollux.VerbaDisponivel = crmBeneficio.VerbaDisponivel.Value;
                else
                    beneficioPollux.VerbaDisponivel = 0;
                if (crmBeneficio.StatusBeneficio != null)
                {
                    beneficioPollux.CodigoStatusBeneficio = crmBeneficio.StatusBeneficio.Id.ToString();
                    beneficioPollux.NomeStatusBeneficio = crmBeneficio.StatusBeneficio.Name;
                }
                else
                {
                    beneficioPollux.CodigoStatusBeneficio = Guid.Empty.ToString();
                    beneficioPollux.NomeStatusBeneficio = "N/A";
                }
                if (!String.IsNullOrEmpty(crmBeneficio.Nome))
                    beneficioPollux.NomeBeneficioCanal = crmBeneficio.Nome;
                else
                    beneficioPollux.NomeBeneficioCanal = "N/A";
                if (crmBeneficio.VerbaBrutaPeriodoAtual.HasValue)
                    beneficioPollux.VerbaTotal = crmBeneficio.VerbaBrutaPeriodoAtual.Value;
                else
                    beneficioPollux.VerbaTotal = 0;
                if (crmBeneficio.VerbaPeriodoAtual.HasValue)
                    beneficioPollux.VerbaCalculada = crmBeneficio.VerbaPeriodoAtual.Value;
                else
                    beneficioPollux.VerbaCalculada = 0;
                if (crmBeneficio.Categoria != null)
                {
                    int codCateg = 0;
                    if (int.TryParse(crmBeneficio.CategoriaObj.CodigoCategoria, out codCateg))
                        beneficioPollux.CategoriaCodigo = codCateg;
                    else
                        beneficioPollux.CategoriaCodigo = 0;
                    beneficioPollux.NomeCategoria = crmBeneficio.CategoriaObj.Nome;
                    beneficioPollux.CodigoCategoria = crmBeneficio.CategoriaObj.ID.Value.ToString();
                }
                else
                {
                    beneficioPollux.NomeCategoria = "N/A";
                    beneficioPollux.CodigoCategoria = Guid.Empty.ToString();
                    beneficioPollux.CategoriaCodigo = 0;
                }

                if (crmBeneficio.Beneficio != null)
                {
                    if (crmBeneficio.BeneficioObj.Codigo.HasValue)
                        beneficioPollux.BeneficioCodigo = crmBeneficio.BeneficioObj.Codigo.Value;
                    else
                        beneficioPollux.BeneficioCodigo = 0;

                    beneficioPollux.CodigoBeneficio = crmBeneficio.BeneficioObj.ID.Value.ToString();
                    beneficioPollux.NomeBeneficio = crmBeneficio.BeneficioObj.Nome;
                    if (crmBeneficio.BeneficioObj.PossuiControleContaCorrente.HasValue)
                        beneficioPollux.PossuiControleContaCorrente = crmBeneficio.BeneficioObj.PossuiControleContaCorrente.Value;
                    else
                        beneficioPollux.PossuiControleContaCorrente = 0;
                    
                    if (crmBeneficio.BeneficioObj.PassivelDeSolicitacao.HasValue)
                        beneficioPollux.PassivelSolicitacao = crmBeneficio.BeneficioObj.PassivelDeSolicitacao.Value;
                    else
                        beneficioPollux.PassivelSolicitacao = false;
                }
                else
                {
                    beneficioPollux.CategoriaCodigo = 0;
                    beneficioPollux.CodigoBeneficio = Guid.Empty.ToString();
                    beneficioPollux.NomeBeneficio = "N/A";
                    beneficioPollux.PossuiControleContaCorrente = 0;
                    beneficioPollux.PassivelSolicitacao = false;
                }

                if (crmBeneficio.VerbaReembolsada.HasValue)
                    beneficioPollux.VerbaReembolsada = crmBeneficio.VerbaReembolsada.Value;
                else
                    beneficioPollux.VerbaReembolsada = 0;

                if (crmBeneficio.UnidadeDeNegocio != null)
                {
                    beneficioPollux.CodigoUnidadeNegocio = crmBeneficio.UnidadeNegocioObj.ChaveIntegracao;
                    beneficioPollux.NomeUnidadeNegocio = crmBeneficio.UnidadeNegocioObj.Nome;
                }
                else
                {
                    beneficioPollux.CodigoUnidadeNegocio = "N/A";
                    beneficioPollux.NomeUnidadeNegocio = "N/A";
                }
                if (crmBeneficio.TotalSolicitacoesAprovadasNaoPagas.HasValue)
                    beneficioPollux.VerbaEmpenhada = crmBeneficio.TotalSolicitacoesAprovadasNaoPagas.Value;
                else
                    beneficioPollux.VerbaEmpenhada = 0;
                if (crmBeneficio.CalculaVerba.HasValue)
                    beneficioPollux.CalcularVerba = crmBeneficio.CalculaVerba.Value;
                if (crmBeneficio.AcumulaVerba.HasValue)
                    beneficioPollux.AcumularVerba = crmBeneficio.AcumulaVerba.Value;
                else
                    beneficioPollux.AcumularVerba = true;

                beneficioPollux.VerbaPeriodoAnterior = crmBeneficio.VerbaPeriodosAnteriores.HasValue ? 
                    crmBeneficio.VerbaPeriodosAnteriores.Value : 0;
                
                beneficioPollux.VerbaReembolsada = crmBeneficio.VerbaReembolsada.HasValue ? 
                    crmBeneficio.VerbaReembolsada.Value : 0;

                beneficioPollux.VerbaCancelada = crmBeneficio.VerbaCancelada.HasValue ?
                    crmBeneficio.VerbaCancelada.Value : 0;

                beneficioPollux.VerbaAjustada = crmBeneficio.VerbaAjustada.HasValue ?
                    crmBeneficio.VerbaAjustada.Value : 0;

                beneficioPollux.VerbaDisponivel = crmBeneficio.VerbaDisponivel.HasValue ?
                    crmBeneficio.VerbaDisponivel.Value : 0;

                lstPolluxBeneficio.Add(beneficioPollux);
            }

            return lstPolluxBeneficio;

        }
        #endregion
    }
}
