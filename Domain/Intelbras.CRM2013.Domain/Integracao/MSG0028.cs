using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0028 : Base, IBase<Intelbras.Message.Helper.MSG0028, Domain.Model.FamiliaProduto>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0028(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0028>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0028R1>(numeroMensagem, retorno);
            }

            objeto = new Intelbras.CRM2013.Domain.Servicos.FamiliaProdutoService(this.Organizacao, this.IsOffline).Persistir(objeto);

            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(Erro ao persistir Família Produto!";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0028R1>(numeroMensagem, retorno);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "(Integração ocorrida com sucesso";
            }

            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0028R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public FamiliaProduto DefinirPropriedades(Intelbras.Message.Helper.MSG0028 xml)
        {
            var crm = new FamiliaProduto(this.Organizacao, this.IsOffline);

            FamiliaProduto familiaProduto = new Intelbras.CRM2013.Domain.Servicos.RepositoryService(this.Organizacao, this.IsOffline).FamiliaProduto.ObterPor(xml.CodigoFamilia);

            if (familiaProduto != null)
            {
                crm.DescontoVerdeHabilitado = familiaProduto.DescontoVerdeHabilitado;
            }

            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(xml.Nome))
                crm.Nome = xml.Nome;
            else 
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(Nome não enviado.";
                return crm;
            }
            
            if (!String.IsNullOrEmpty(xml.CodigoFamilia))
                crm.Codigo = xml.CodigoFamilia;
            else 
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(Código Família não enviado.";
                return crm;
            }

            crm.Status = xml.Situacao;
            //Segmento
            if (!String.IsNullOrEmpty(xml.Segmento))
            {
                Model.Segmento segmento = new Model.Segmento(this.Organizacao, this.IsOffline);
                segmento = new Intelbras.CRM2013.Domain.Servicos.SegmentoService(this.Organizacao, this.IsOffline).BuscaSegmento(xml.Segmento);

                if (segmento != null && segmento.ID.HasValue)
                    crm.Segmento = new Lookup(segmento.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Segmento não encontrado!";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Segmento não enviado.";
                return crm;
            }

            crm.IntegradoEm = DateTime.Now;
            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            crm.UsuarioIntegracao = xml.LoginUsuario;

            #endregion

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(FamiliaProduto objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
