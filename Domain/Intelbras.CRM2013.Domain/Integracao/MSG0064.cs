using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0064 : Base,IBase<Message.Helper.MSG0064,Domain.Model.SeguroConta>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0064(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0064>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0064R1>(numeroMensagem, retorno);
            }
            
            objeto = new Intelbras.CRM2013.Domain.Servicos.SeguroContaService(this.Organizacao, this.IsOffline).Persistir(objeto);
            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Registro não encontrado!";
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                retorno.Add("CodigoSeguro", objeto.ID.Value.ToString());
            }
            retorno.Add("Resultado", resultadoPersistencia);
            
            return CriarMensagemRetorno<Pollux.MSG0064R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public SeguroConta DefinirPropriedades(Intelbras.Message.Helper.MSG0064 xml)
        {
            var crm = new SeguroConta(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(xml.CodigoSeguro))
                crm.ID = new Guid(xml.CodigoSeguro);

            if (!String.IsNullOrEmpty(xml.Conta))
            {
                crm.Conta = new Lookup(new Guid(xml.Conta), "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Canal não enviado.";
                return crm;
            }

            if(!String.IsNullOrEmpty(xml.Nome))
                crm.Nome = xml.Nome;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Nome não enviado.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.Modalidade))
                crm.Modalidade = xml.Modalidade;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Modalidade não enviado.";
                return crm;
            }

            if(xml.Valor.HasValue)
                crm.ValorSegurado = xml.Valor;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Valor Segurado não enviado.";
                return crm;
            }


            if(xml.DataVencimento.HasValue)
                crm.Vencimento = xml.DataVencimento;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Data Vencimento não enviado.";
                return crm;
            }

            crm.State = xml.Situacao;
            crm.IntegradoEm = DateTime.Now;
            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            crm.UsuarioIntegracao = xml.LoginUsuario;


            #endregion

            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(SeguroConta objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
