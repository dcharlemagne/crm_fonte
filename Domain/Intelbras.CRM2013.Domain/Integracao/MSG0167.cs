using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0167 : Base, IBase<Message.Helper.MSG0167, Domain.Model.ParametroGlobal>
    {
        #region Construtor

        public MSG0167(string org, bool isOffline)
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

        public ParametroGlobal DefinirPropriedades(Intelbras.Message.Helper.MSG0167 xml)
        {
            throw new NotImplementedException();
        }

        public Pollux.MSG0167 DefinirPropriedadesPlugin(ParametroGlobal objModel)
        {
            Pollux.MSG0167 retMsgProp = new Pollux.MSG0167(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), objModel.ID.ToString());
            TipodeParametroGlobalService tipoParamService = new TipodeParametroGlobalService(this.Organizacao, this.IsOffline);
            CategoriaService catService = new CategoriaService(this.Organizacao, this.IsOffline);
            BeneficioService benefService = new BeneficioService(this.Organizacao, this.IsOffline);
            UnidadeNegocioService unidNegService = new UnidadeNegocioService(this.Organizacao, this.IsOffline);

            retMsgProp.CodigoParametroGlobal = objModel.ID.Value.ToString();
            retMsgProp.NomeParametroGlobal = objModel.Nome;

            var tipoParamObj = tipoParamService.ObterPor(objModel.TipoParametro.Id);
            retMsgProp.TipoParametroGlobal = tipoParamObj.Codigo;

            if (objModel.Classificacao != null)
            {
                retMsgProp.CodigoClassificacao = objModel.Classificacao.Id.ToString();
            }

            if (objModel.Compromisso != null)
            {
                retMsgProp.CodigoCompromisso = objModel.Compromisso.Id.ToString();
            }

            if (objModel.Categoria != null)
            {
                retMsgProp.CodigoCategoria = objModel.Categoria.Id.ToString();
                var catObj = catService.ObterPor(objModel.Categoria.Id);
                retMsgProp.CategoriaCodigo = Int32.Parse(catObj.CodigoCategoria);
            }

            if (objModel.Beneficio != null)
            {
                retMsgProp.CodigoBeneficio = objModel.Beneficio.Id.ToString();
                var benefObj = benefService.ObterPor(objModel.Beneficio.Id);
                retMsgProp.BeneficioCodigo = benefObj.Codigo;
            }

            if (objModel.NivelPosVenda != null)
            {
                retMsgProp.CodigoNivelPosVenda = objModel.NivelPosVenda.Id.ToString();
            }

            if (objModel.UnidadeNegocio != null)
            {
                var unidNegObj = unidNegService.BuscaUnidadeNegocio(objModel.UnidadeNegocio.Id);
                retMsgProp.CodigoUnidadeNegocio = unidNegObj.ChaveIntegracao;
            }

            retMsgProp.TipoDado = objModel.TipoDado;
            retMsgProp.ValorParametroGlobal = objModel.Valor;
            retMsgProp.Situacao = objModel.State;

            return retMsgProp;
        }

        public string Enviar(ParametroGlobal objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0167 mensagem = DefinirPropriedadesPlugin(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);

            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0167R1 retorno = CarregarMensagem<Pollux.MSG0167R1>(resposta);
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
