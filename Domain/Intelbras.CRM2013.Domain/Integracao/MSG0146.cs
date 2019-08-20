using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0146 : Base, IBase<Message.Helper.MSG0146, Domain.Model.BeneficioDoCanal>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0146(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0146>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0146R1>(numeroMensagem, retorno);
            }

            
            //SolicitacoesItens
            BeneficioDoCanal beneficioDoCanal = new Servicos.BeneficioDoCanalService(this.Organizacao, this.IsOffline).ObterPor(objeto.ID.Value);


            if (beneficioDoCanal == null)
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0146R1>(numeroMensagem, retorno);
            }
            else 
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
                retorno.Add("BeneficioCanal", this.DefinirRetorno(beneficioDoCanal));
            }
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0146R1>(numeroMensagem, retorno);
        }
            catch (Exception e)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(e);
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0146R1>(numeroMensagem, retorno);
            }
        }
        #endregion

        #region Definir Propriedades

        public BeneficioDoCanal DefinirPropriedades(Intelbras.Message.Helper.MSG0146 xml)
        {
            var crm = new Model.BeneficioDoCanal(this.Organizacao, this.IsOffline);


            if (!String.IsNullOrEmpty(xml.CodigoBeneficioCanal) && xml.CodigoBeneficioCanal.Length == 36)
                crm.ID = new Guid(xml.CodigoBeneficioCanal);
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Valor do parâmetro - " + xml.CodigoBeneficioCanal + "- não existe.";
                return crm;
            }

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public Pollux.Entities.ObterBeneficio DefinirRetorno(Model.BeneficioDoCanal crmBeneficio)
        {
            Pollux.Entities.ObterBeneficio beneficioPollux = new Pollux.Entities.ObterBeneficio();


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
            if (crmBeneficio.CategoriaObj != null)
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

            if (crmBeneficio.BeneficioObj != null)
            {
                if (crmBeneficio.BeneficioObj.Codigo.HasValue)
                    beneficioPollux.BeneficioCodigo = crmBeneficio.BeneficioObj.Codigo.Value;
                else
                    beneficioPollux.BeneficioCodigo = 0;

                beneficioPollux.CodigoBeneficio = crmBeneficio.BeneficioObj.ID.Value.ToString();
                beneficioPollux.NomeBeneficio = crmBeneficio.BeneficioObj.Nome;
            }
            else
            {
                beneficioPollux.CategoriaCodigo = 0;
                beneficioPollux.CodigoBeneficio = Guid.Empty.ToString();
                beneficioPollux.NomeBeneficio = "N/A";
            }

            if (crmBeneficio.VerbaReembolsada.HasValue)
                beneficioPollux.VerbaReembolsada = crmBeneficio.VerbaReembolsada.Value;
            else
                beneficioPollux.VerbaReembolsada = 0;
            
            if (crmBeneficio.UnidadeNegocioObj != null)
            {
                beneficioPollux.CodigoUnidadeNegocio = crmBeneficio.UnidadeNegocioObj.ChaveIntegracao;
                beneficioPollux.NomeUnidadeNegocio = crmBeneficio.UnidadeNegocioObj.Nome;
            }
            else
            {
                beneficioPollux.CodigoUnidadeNegocio = Guid.Empty.ToString();
                beneficioPollux.NomeUnidadeNegocio = "N/A";
            }
            if (crmBeneficio.TotalSolicitacoesAprovadasNaoPagas.HasValue)
                beneficioPollux.VerbaEmpenhada = crmBeneficio.TotalSolicitacoesAprovadasNaoPagas.Value;
            else
                beneficioPollux.VerbaEmpenhada = 0;
            if (crmBeneficio.Canal != null)
            {
                beneficioPollux.CodigoConta = crmBeneficio.Canal.Id.ToString();
                beneficioPollux.NomeConta = crmBeneficio.Canal.Name;
            }

            if (crmBeneficio.VerbaPeriodosAnteriores.HasValue)
            {
                beneficioPollux.VerbaPeriodoAnterior = crmBeneficio.VerbaPeriodosAnteriores.Value;
            }
            else
            {
                beneficioPollux.VerbaPeriodoAnterior = 0;
            }

            if(crmBeneficio.VerbaBrutaPeriodoAtual.HasValue)
            {
                beneficioPollux.VerbaTotal = crmBeneficio.VerbaBrutaPeriodoAtual;
            }
            else
            {
                beneficioPollux.VerbaTotal = 0;
            }

            if(crmBeneficio.TotalSolicitacoesAprovadasNaoPagas.HasValue)
            {
                beneficioPollux.VerbaEmpenhada = crmBeneficio.TotalSolicitacoesAprovadasNaoPagas.Value;
            }
            else
            {
                beneficioPollux.VerbaEmpenhada = 0;
            }

            if (crmBeneficio.VerbaReembolsada.HasValue)
            {
                beneficioPollux.VerbaReembolsada = crmBeneficio.VerbaReembolsada.Value;
            }
            else
            {
                beneficioPollux.VerbaReembolsada = 0;
            }

            if (crmBeneficio.VerbaCancelada.HasValue)
            {
                beneficioPollux.VerbaCancelada = crmBeneficio.VerbaCancelada.Value;
            }
            else
            {
                beneficioPollux.VerbaCancelada = 0;
            }

            if (crmBeneficio.VerbaAjustada.HasValue)
            {
                beneficioPollux.VerbaAjustada = crmBeneficio.VerbaAjustada.Value;
            }
            else
            {
                beneficioPollux.VerbaAjustada = 0;
            }

            if (crmBeneficio.VerbaDisponivel.HasValue)
            {
                beneficioPollux.VerbaDisponivel = crmBeneficio.VerbaDisponivel.Value;
            }
            else
            {
                beneficioPollux.VerbaDisponivel = 0;
            }

            if (crmBeneficio.CalculaVerba.HasValue)
            {
                beneficioPollux.CalcularVerba = crmBeneficio.CalculaVerba.Value;
            }
            else
            {
                beneficioPollux.CalcularVerba = false;
            }

            if (crmBeneficio.AcumulaVerba.HasValue)
            {
                beneficioPollux.AcumularVerba = crmBeneficio.AcumulaVerba.Value;
            }
            else
            {
                beneficioPollux.AcumularVerba = false;
            }

            if (crmBeneficio.BeneficioObj.PassivelDeSolicitacao.HasValue)
            {
                beneficioPollux.PassivelSolicitacao = crmBeneficio.BeneficioObj.PassivelDeSolicitacao.Value;
            }
            else
            {
                beneficioPollux.PassivelSolicitacao = false;
            }

            if (crmBeneficio.BeneficioObj.PossuiControleContaCorrente.HasValue)
            {
                beneficioPollux.PossuiControleContaCorrente = crmBeneficio.BeneficioObj.PossuiControleContaCorrente.Value;
            }
            else
            {
                beneficioPollux.PossuiControleContaCorrente = 0;
            }

            return beneficioPollux;
        }

        public string Enviar(BeneficioDoCanal objModel)
        {
            return String.Empty;
        }
        #endregion
    }
}
