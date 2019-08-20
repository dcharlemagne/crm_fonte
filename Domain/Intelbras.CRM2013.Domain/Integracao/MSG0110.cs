using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.Message.Helper;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0110 : Base, IBase<Message.Helper.MSG0110, Domain.Model.TipoDeDenuncia>
    {

        #region Construtor

        public MSG0110(string org, bool isOffline)
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
            var xml = this.CarregarMensagem<Pollux.MSG0110>(mensagem);

            List<TipoDeDenuncia> lstTipoDenuncia = new Servicos.DenunciaService(this.Organizacao, this.IsOffline).ListarTipoDenuncia();


            if (lstTipoDenuncia != null && lstTipoDenuncia.Count > 0)
            {
                var objeto = this.DefinirRetorno(lstTipoDenuncia);
                if (objeto != null)
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";
                    retorno.Add("TiposDenunciaItens", objeto);
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0110R1>(numeroMensagem, retorno);
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Erro de consulta no Crm.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0110R1>(numeroMensagem, retorno);
                }

            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0110R1>(numeroMensagem, retorno);

            }
        }
        #endregion

        #region Definir Propriedades

        public TipoDeDenuncia DefinirPropriedades(Intelbras.Message.Helper.MSG0110 xml)
        {
            TipoDeDenuncia retorno = new TipoDeDenuncia(this.Organizacao, this.IsOffline);
            return retorno;
        }
        public List<Pollux.Entities.TipoDenuncia> DefinirRetorno(List<Model.TipoDeDenuncia> lstTipoDenuncia)
        {
            List<Pollux.Entities.TipoDenuncia> lstRetornoPollux = new List<Pollux.Entities.TipoDenuncia>();

            #region Propriedades Crm->Xml

            foreach (TipoDeDenuncia itemCrm in lstTipoDenuncia)
            {
                Pollux.Entities.TipoDenuncia tipoDenuncia = new Pollux.Entities.TipoDenuncia();
                if (itemCrm.ID.HasValue)
                    tipoDenuncia.CodigoTipoDenuncia = itemCrm.ID.Value.ToString();
                else
                    tipoDenuncia.CodigoTipoDenuncia = Guid.Empty.ToString();
                if (!String.IsNullOrEmpty(itemCrm.Nome))
                    tipoDenuncia.NomeTipoDenuncia = itemCrm.Nome;
                else
                    tipoDenuncia.NomeTipoDenuncia = "N/A";
                lstRetornoPollux.Add(tipoDenuncia);
            }

            #endregion

            return lstRetornoPollux;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(TipoDeDenuncia objModel)
        {
            string resposta = string.Empty;

            return resposta;
        }

        #endregion

    }
}
