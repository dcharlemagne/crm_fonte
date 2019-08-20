using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;
using Intelbras.CRM2013.Domain.Exceptions;
using SDKore.DomainModel;
using System.IO;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0284 : Base, IBase<Message.Helper.MSG0284, Domain.Model.Anotacao>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private string tipoProprietario;
        private Domain.Model.Usuario usuarioIntegracao;
        #endregion

        public MSG0284(string org, bool isOffline) : base(org, isOffline) { }
        
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
            var xml = this.CarregarMensagem<Pollux.MSG0284>(mensagem);

            List<Intelbras.Message.Helper.Entities.Anexo> lstAnexosOcorrencia = new List<Pollux.Entities.Anexo>();

            if (string.IsNullOrEmpty(xml.CodigoOcorrencia))
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoOcorrencia é obrigatório.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0284R1>(numeroMensagem, retorno);
            }

            string extensaoArquivo = "";
            if (!string.IsNullOrEmpty(xml.ExtensaoArquivo))
            {
               extensaoArquivo = xml.ExtensaoArquivo; 
            }

            List<Anotacao> lstAnexos = new Servicos.NotasService(this.Organizacao, this.IsOffline).ListarAnotacoesPorTipoArquivo(xml.CodigoOcorrencia, extensaoArquivo);


            #region Lista

            if (lstAnexos != null && lstAnexos.Count > 0)
            {
                foreach (Anotacao crmItem in lstAnexos)
                {
                    try
                    {
                        Pollux.Entities.Anexo objPollux = new Pollux.Entities.Anexo();

                        objPollux.CodigoOcorrencia = crmItem.EntidadeRelacionada.Id.ToString();
                        objPollux.Nome = crmItem.NomeArquivos;
                        objPollux.ExtensaoArquivo = ObterExtensao(crmItem);
                        objPollux.ConteudoArquivo = crmItem.Body;

                        if (!string.IsNullOrEmpty(objPollux.CodigoOcorrencia) && !string.IsNullOrEmpty(objPollux.Nome) && !string.IsNullOrEmpty(objPollux.ExtensaoArquivo)
                            && !string.IsNullOrEmpty(objPollux.ConteudoArquivo))
                        {
                            lstAnexosOcorrencia.Add(objPollux);
                        }
                        
                    }catch (Exception e)
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Ocorreu erro ao montar a mensagem: "+ e.Message;
                        retorno.Add("Resultado", resultadoPersistencia);
                        return CriarMensagemRetorno<Pollux.MSG0284R1>(numeroMensagem, retorno);
                    }
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0284R1>(numeroMensagem, retorno);
            }

            #endregion
            resultadoPersistencia.Sucesso = true;
            retorno.Add("Anexos", lstAnexosOcorrencia);
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0284R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public Anotacao DefinirPropriedades(Intelbras.Message.Helper.MSG0284 xml)
        {
            var crm = new Model.Anotacao(this.Organizacao, this.IsOffline);
            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Anotacao objModel)
        {
            return String.Empty;
        }

        private string ObterExtensao(Anotacao objAnexo)
        {
            string strExtensao = "";

            strExtensao = Path.GetExtension(objAnexo.NomeArquivos).Replace(".","");

            return strExtensao;
        }

        #endregion
    }
}
