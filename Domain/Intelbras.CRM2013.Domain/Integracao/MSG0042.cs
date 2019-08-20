using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0042 : Base, IBase<Message.Helper.MSG0042, Domain.Model.Estabelecimento>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor

        public MSG0042(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0042>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0042R1>(numeroMensagem, retorno);
            }
            objeto = new Intelbras.CRM2013.Domain.Servicos.EstabelecimentoService(this.Organizacao, this.IsOffline).Persistir(objeto);
            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro de persistência!";

            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            }

            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0042R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public Estabelecimento DefinirPropriedades(Intelbras.Message.Helper.MSG0042 xml)
        {
            var crm = new Estabelecimento(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml
            if (!String.IsNullOrEmpty(xml.Nome))
            {
                crm.Nome = xml.Nome;
            }
            else 
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Nome não enviado.";
                return crm;
            }
            crm.Codigo = xml.CodigoEstabelecimento;
            if (!String.IsNullOrEmpty(xml.RazaoSocial))
            {
                crm.RazaoSocial = xml.RazaoSocial;
            }
            else
            {
                crm.AddNullProperty("RazaoSocial");
            }

            if (!String.IsNullOrEmpty(xml.Endereco))
            {
                crm.Endereco = xml.Endereco;
            }
            else
            {
                crm.AddNullProperty("Endereco");
            }

            if (!String.IsNullOrEmpty(xml.Cidade))
            {
                crm.Cidade = xml.Cidade;
            }
            else
            {
                crm.AddNullProperty("Cidade");
            }

            if (!String.IsNullOrEmpty(xml.CEP))
            {
                crm.CEP = xml.CEP;
            }
            else
            {
                crm.AddNullProperty("CEP");
            }

            if (!String.IsNullOrEmpty(xml.UF))
            {
                crm.UF = xml.UF;
            }
            else
            {
                crm.AddNullProperty("UF");
            }

            if (!String.IsNullOrEmpty(xml.CNPJ))
            {
                crm.CNPJ = xml.CNPJ;
            }
            else
            {
                crm.AddNullProperty("CNPJ");
            }

            if (!String.IsNullOrEmpty(xml.InscricaoEstadual))
            {
                crm.InscricaoEstadual = xml.InscricaoEstadual;
            }
            else
            {
                crm.AddNullProperty("InscricaoEstadual");
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

        public string Enviar(Estabelecimento objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
