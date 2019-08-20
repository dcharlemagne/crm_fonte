using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0165 : Base, IBase<Message.Helper.MSG0165, Domain.Model.BeneficioDoCanal>
    {
        #region Construtor

        public MSG0165(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }

        #endregion

        #region Propriedades

        private Dictionary<string, object> retorno = new Dictionary<string, object>();
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

        public BeneficioDoCanal DefinirPropriedades(Intelbras.Message.Helper.MSG0165 xml)
        {
            throw new NotImplementedException();
        }

        public Pollux.MSG0165 DefinirPropriedadesPlugin(BeneficioDoCanal objModel)
        {
            Pollux.MSG0165 retMsgProp = new Pollux.MSG0165(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), objModel.ID.ToString());
            BeneficioService benefService = new BeneficioService(this.Organizacao, this.IsOffline);
            StatusBeneficiosService statusBenefService = new StatusBeneficiosService(this.Organizacao, this.IsOffline);

            retMsgProp.CodigoBeneficioCanal = objModel.ID.ToString();
            retMsgProp.NomeBeneficioCanal = objModel.Nome;
            retMsgProp.CodigoConta = objModel.Canal.Id.ToString();
            retMsgProp.CodigoBeneficio = objModel.Beneficio.Id.ToString();

            Beneficio benefObj = benefService.ObterPor(objModel.Beneficio.Id);

            retMsgProp.BeneficioCodigo = benefObj.Codigo;
            retMsgProp.NomeBeneficio = benefObj.Nome;
            retMsgProp.CodigoCategoria = objModel.Categoria.Id.ToString();
            retMsgProp.CategoriaCodigo = Int32.Parse(objModel.CategoriaObj.CodigoCategoria);
            retMsgProp.NomeCategoria = objModel.CategoriaObj.Nome;
            retMsgProp.CodigoUnidadeNegocio = objModel.UnidadeNegocioObj.ChaveIntegracao;
            retMsgProp.NomeUnidadeNegocio = objModel.UnidadeNegocioObj.Nome;

            if (objModel.StatusBeneficio != null)
            {
                retMsgProp.CodigoStatusBeneficio = objModel.StatusBeneficio.Id.ToString();
                StatusBeneficios statusBenefObj = statusBenefService.ObterPor(objModel.StatusBeneficio.Id);
                retMsgProp.NomeStatusBeneficio = statusBenefObj.Nome;
            }

            retMsgProp.CalcularVerba = objModel.CalculaVerba;
            retMsgProp.AcumularVerba = objModel.AcumulaVerba;
            retMsgProp.PassivelSolicitacao = objModel.BeneficioObj.PassivelDeSolicitacao;
            retMsgProp.PossuiControleContaCorrente = objModel.BeneficioObj.PossuiControleContaCorrente;
            retMsgProp.Situacao = objModel.Status;
            retMsgProp.Proprietario = "259A8E4F-15E9-E311-9420-00155D013D39";
            retMsgProp.TipoProprietario = "systemuser";

            return retMsgProp;
        }

        public string Enviar(BeneficioDoCanal objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0165 mensagem = DefinirPropriedadesPlugin(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);

            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0165R1 retorno = CarregarMensagem<Pollux.MSG0165R1>(resposta);
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
