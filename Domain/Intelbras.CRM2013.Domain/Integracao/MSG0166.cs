using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0166 : Base, IBase<Message.Helper.MSG0166, Domain.Model.ParametroBeneficio>
    {
        #region Construtor

        public MSG0166(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }

        #endregion

        #region Propriedades

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
            throw new NotImplementedException();
        }

        #endregion

        public ParametroBeneficio DefinirPropriedades(Intelbras.Message.Helper.MSG0166 xml)
        {
            throw new NotImplementedException();
        }

        public Pollux.MSG0166 DefinirPropriedadesPlugin(ParametroBeneficio objModel)
        {
            Pollux.MSG0166 retMsgProp = new Pollux.MSG0166(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), objModel.ID.ToString());
            BeneficioService benefService = new BeneficioService(this.Organizacao, this.IsOffline);
            UnidadeNegocioService unidNegService = new UnidadeNegocioService(this.Organizacao, this.IsOffline);
            EstabelecimentoService estabService = new EstabelecimentoService(this.Organizacao, this.IsOffline);

            retMsgProp.CodigoBeneficio = objModel.Beneficio.Id.ToString();
            retMsgProp.CodigoParametroBeneficio = objModel.ID.ToString();

            var benefObj = benefService.ObterPor(objModel.Beneficio.Id);
            retMsgProp.BeneficioCodigo = benefObj.Codigo;

            var unidNegObj = unidNegService.BuscaUnidadeNegocio(objModel.UnidadeNegocio.Id);
            retMsgProp.CodigoUnidadeNegocio = unidNegObj.ChaveIntegracao;

            var estabObj = estabService.BuscaEstabelecimento(objModel.Estabelecimento.Id);
            retMsgProp.CodigoEstabelecimento = estabObj.Codigo;

            retMsgProp.TipoFluxoFinanceiro = objModel.TipoFluxoFinanceiro;
            retMsgProp.EspecieDocumento = objModel.EspecieDocumento;
            retMsgProp.ContaContabil = objModel.ContaContabil;
            retMsgProp.CentroCusto = objModel.CentroCusto;
            retMsgProp.PercentualAtingimentoMeta = objModel.AtingimentoMetaPrevisto;
            retMsgProp.PercentualCusto = objModel.Custo;
            retMsgProp.Situacao = objModel.Status;
            retMsgProp.Proprietario = "259A8E4F-15E9-E311-9420-00155D013D39";
            retMsgProp.TipoProprietario = "systemuser"; 

            return retMsgProp;
        }

        public string Enviar(ParametroBeneficio objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0166 mensagem = DefinirPropriedadesPlugin(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);

            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0166R1 retorno = CarregarMensagem<Pollux.MSG0166R1>(resposta);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new ArgumentException("(CRM) " + retorno.Resultado.Mensagem);
                }

            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(resposta);
                throw new ArgumentException("(CRM) " + erro001.GenerateMessage(false));
            }
            return resposta;
        }

    }
}
