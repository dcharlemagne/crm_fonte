using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;
using Intelbras.CRM2013.Domain.Exceptions;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0279 : Base, IBase<Message.Helper.MSG0279, Domain.Model.Ocorrencia>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private string tipoProprietario;
        private Domain.Model.Usuario usuarioIntegracao;
        #endregion

        public MSG0279(string org, bool isOffline) : base(org, isOffline) { }
        
        #region trace
        private SDKore.Helper.Trace Trace { get; set; }
        public void DefinirObjetoTrace(SDKore.Helper.Trace trace)
        {
            this.Trace = trace;
        }
        #endregion

        #region Executar

        public string Executar(string mensagem, string numeroMensagem, Usuario usuario)
        {
            usuarioIntegracao = usuario;
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0279>(mensagem));


            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0279R1>(numeroMensagem, retorno);
            }

            try
            {
                objeto.Atualizar();
                if (objeto == null)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Não foi possível salvar a alteração. Integração não realizada.";
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                }
                
            }
            catch (ChaveIntegracaoContatoException ex)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = ex.Message;
            }

            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0279R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public Ocorrencia DefinirPropriedades(Intelbras.Message.Helper.MSG0279 xml)
        {
            var crm = new Model.Ocorrencia(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(xml.CodigoOcorrencia))
            {
                Ocorrencia ocorrencia = new Servicos.OcorrenciaService(this.Organizacao, this.IsOffline).BuscaOcorrencia(new Guid(xml.CodigoOcorrencia));
                if (ocorrencia != null)
                {
                    crm = ocorrencia;
                    if (!string.IsNullOrEmpty(xml.Foto))
                    {
                        Anotacao anotacao = new Anotacao();
                        anotacao.Assunto = "Anexo";
                        anotacao.EntidadeRelacionada = new Lookup(ocorrencia.Id, "incident");
                        anotacao.Body = xml.Foto;
                        anotacao.Tipo = xml.ExtensaoArquivo;
                        anotacao.NomeArquivos = xml.Nome + "." + xml.ExtensaoArquivo;
                        try
                        {
                            anotacao.Texto = "A operação foi concluida com sucesso.";
                            new RepositoryService().Anexo.Create(anotacao);
                        }
                        catch (Exception ex)
                        {
                            anotacao.Texto = ex.Message;
                            new RepositoryService().Anexo.Create(anotacao);
                        }
                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoOcorrencia informado não existe para ser atualizado.";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoOcorrencia é obrigatório.";
                return crm;
            }

            #endregion

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Ocorrencia objModel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}