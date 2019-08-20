using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0122 : Base, IBase<Pollux.MSG0122, Model.CategoriasCanal>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado();

        #endregion


        #region Construtor
        public MSG0122(string org, bool isOffline)
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
            usuarioIntegracao = usuario;
            //Trace.Add("Mensagem " + numeroMensagem + " XML: {0}", mensagem);

            resultadoPersistencia.Sucesso = false;
            resultadoPersistencia.Mensagem = "Ação não permitida.";
            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0122R1>(numeroMensagem, retorno);
        }
        #endregion


        #region Definir Propriedades
       
        public CategoriasCanal DefinirPropriedades(Intelbras.Message.Helper.MSG0122 xml)
        {
            var crm = new CategoriasCanal(this.Organizacao, this.IsOffline);

            return crm;
        }
        private Intelbras.Message.Helper.MSG0122 DefinirPropriedades(CategoriasCanal crm)
        {
            Intelbras.Message.Helper.MSG0122 xml = new Pollux.MSG0122(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.Nome, 40));
            

            xml.CodigoCategoriaCanal = crm.ID.ToString();
            xml.Nome = crm.Nome;

            UnidadeNegocio unidade = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocio(crm.UnidadeNegocios.Id);

            if (unidade != null)
            {
                xml.UnidadeNegocio = unidade.ChaveIntegracao;
            }
            xml.Conta = crm.Canal.Id.ToString();
            //Categoria categ = new Servicos.CategoriaService(this.Organizacao, this.IsOffline).IntegracaoBarramento

            xml.Categoria = crm.Categoria.Id.ToString();
            xml.Classificacao = crm.Classificacao.Id.ToString();
            xml.SubClassificacao = crm.SubClassificacao.Id.ToString();
            xml.Situacao = (crm.Status.HasValue ? crm.Status.Value : 0);
            xml.Proprietario = "259A8E4F-15E9-E311-9420-00155D013D39";
            xml.TipoProprietario = "systemuser";

            return xml;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(CategoriasCanal objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0122 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0122R1 retorno = CarregarMensagem<Pollux.MSG0122R1>(resposta);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new Exception(retorno.Resultado.Mensagem);
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(resposta);
                throw new Exception(erro001.GenerateMessage(false));
            }
            return resposta;
        }
        

        #endregion
    }
}
