using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0077 : Base, IBase<Pollux.MSG0077, Model.DocumentoCanal>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Pollux.Entities.Resultado resultadoConsulta = new Pollux.Entities.Resultado() { Sucesso = true };
        private Pollux.Entities.Documento listDocumentoCanal = new Pollux.Entities.Documento { };
        private List<Pollux.Entities.Documento> responseDocumento = new List<Pollux.Entities.Documento>();
        #endregion

        #region Construtor

        public MSG0077(string org, bool isOffline)
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
            var xml = this.CarregarMensagem<Pollux.MSG0077>(mensagem);


            if (!String.IsNullOrEmpty(xml.CodigoConta))
            {
                Conta objetoConta = new Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(new Guid(xml.CodigoConta));
                List<Pollux.Entities.Documento> lstDocumento = new List<Pollux.Entities.Documento>();

                if (objetoConta.ID.HasValue)
                {
                    List<DocumentoCanal> lstDocCanal = new Servicos.DocumentoCanalService(this.Organizacao, this.IsOffline).ListarDocumentoCanal(objetoConta.ID.Value);

                    if (lstDocCanal == null || lstDocCanal.Count == 0)
                    {
                        resultadoConsulta.Sucesso = true;
                        resultadoConsulta.Mensagem = "Não há documentos para este canal.";
                        retorno.Add("Resultado", resultadoConsulta);
                        return CriarMensagemRetorno<Pollux.MSG0077R1>(numeroMensagem, retorno);
                    }

                    foreach (DocumentoCanal item in lstDocCanal)
                    {
                        Pollux.Entities.Documento documento = new Pollux.Entities.Documento();
                        documento.NomeDocumento = item.Nome;
                        documento.CodigoDocumento = item.ID.Value.ToString();
                        //documento.DescricaoSituacao = item.Status.ToString();
                        if (item.TipoDocumento != null)
                        {
                            documento.CodigoTipoDocumento = item.TipoDocumento.Id.ToString();
                            documento.NomeTipoDocumento = item.TipoDocumento.Name;
                        }
                        if (item.Validade != null)
                            documento.DataValidade = (DateTime)item.Validade;
                        if (item.Compromisso != null)
                        {
                            documento.CodigoCompromisso = item.Compromisso.Id.ToString();
                            documento.NomeCompromisso = item.Compromisso.Name;
                        }
                        if (item.Status.HasValue)
                            documento.Situacao = (int)item.Status;
                        lstDocumento.Add(documento);
                    }
                    resultadoConsulta.Sucesso = true;
                    retorno.Add("DocumentosItens", lstDocumento);
                    retorno.Add("Resultado", resultadoConsulta);
                    return CriarMensagemRetorno<Pollux.MSG0077R1>(numeroMensagem, retorno);
                }
                else
                {
                    resultadoConsulta.Sucesso = true;
                    resultadoConsulta.Mensagem = "Conta/Canal não encontrado.";
                    retorno.Add("Resultado", resultadoConsulta);
                    return CriarMensagemRetorno<Pollux.MSG0077R1>(numeroMensagem, retorno);
                }
            }
            else
            {
                resultadoConsulta.Sucesso = false;
                resultadoConsulta.Mensagem = "Identificador do Canal não enviado.";
                retorno.Add("Resultado", resultadoConsulta);
                return CriarMensagemRetorno<Pollux.MSG0077R1>(numeroMensagem, retorno);
            }
        }

        #endregion

        #region Definir Propriedades

        public DocumentoCanal DefinirPropriedades(Intelbras.Message.Helper.MSG0077 xml)
        {
            return new DocumentoCanal(this.Organizacao, this.IsOffline);
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(DocumentoCanal objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
