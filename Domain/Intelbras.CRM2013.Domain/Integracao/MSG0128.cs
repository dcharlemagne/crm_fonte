using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0128 : Base, IBase<Intelbras.Message.Helper.MSG0128, Domain.Model.FuncaoConexao>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0128(string org, bool isOffline)
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
            //Trace.Add("Mensagem " + numeroMensagem + " XML: {0}", mensagem);
            usuarioIntegracao = usuario;
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0128>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0128R1>(numeroMensagem, retorno);
            }

            List<FuncaoConexao> lstFuncoes = new Intelbras.CRM2013.Domain.Servicos.ConexaoService(this.Organizacao, this.IsOffline).ListarFuncaoConexao(objeto.Categoria);

            List<Pollux.Entities.FuncaoConexao> lstRetorno = new List<Pollux.Entities.FuncaoConexao>();

            if (lstFuncoes.Count > 0)
            {
                lstRetorno = ConverteLista(lstFuncoes);
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                retorno.Add("FuncaoConexaoItens", lstRetorno);
            }
            else
            {
                resultadoPersistencia.Mensagem = "FuncaoConexao não encontrada no Crm.";
            }

            resultadoPersistencia.Sucesso = true;
            retorno.Add("Resultado", resultadoPersistencia);
            
            return CriarMensagemRetorno<Pollux.MSG0128R1>(numeroMensagem, retorno);
        }

        private List<Pollux.Entities.FuncaoConexao> ConverteLista(List<FuncaoConexao> lstFuncoes)
        {
            List<Pollux.Entities.FuncaoConexao> lstPollux = new List<Pollux.Entities.FuncaoConexao>();

            foreach (FuncaoConexao fnCon in lstFuncoes)
            {
                Pollux.Entities.FuncaoConexao polluxObj = new Pollux.Entities.FuncaoConexao();
                polluxObj.CategoriaFuncaoConexao = fnCon.Categoria;
                polluxObj.CodigoFuncaoConexao = fnCon.ID.ToString();
                polluxObj.DescricaoFuncaoConexao = fnCon.Descricao;
                polluxObj.NomeFuncaoConexao = fnCon.Nome;

                lstPollux.Add(polluxObj);
            }

            return lstPollux;
        }
        #endregion

        #region Definir Propriedades

        public FuncaoConexao DefinirPropriedades(Intelbras.Message.Helper.MSG0128 xml)
        {
            var crm = new FuncaoConexao(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml
            // Não orbigatorio
            //if (xml.CategoriaFuncaoConexao == null)
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Categoria inválida.";
            //    return crm;
            //}

            crm.Categoria = xml.CategoriaFuncaoConexao;

            #endregion

            return crm;
        }

        #endregion

        #region Métodos Auxiliares
     
        public string Enviar(FuncaoConexao objModel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
