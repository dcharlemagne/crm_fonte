using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.Crm.Util;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0030 : Base,IBase<Message.Helper.MSG0030,Domain.Model.SubfamiliaProduto>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0030(string org, bool isOffline)
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

            SubfamiliaProduto objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0030>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0030R1>(numeroMensagem, retorno);
            }

            //Checa dentro da service se ele tentou mudar o proprietario,se positivo recusa e retorna erro
            bool mudancaProprietario = false;

            objeto = new Intelbras.CRM2013.Domain.Servicos.SubFamiliaProdutoService(this.Organizacao, this.IsOffline).Persistir(objeto, ref mudancaProprietario);

            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(Registro não encontrado!";
            }
            else
            {
                if (mudancaProprietario == true)
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "(Integração ocorrida com sucesso, não houve alteração do proprietário.";
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                }

            }
            retorno.Add("Resultado", resultadoPersistencia);
            

            return CriarMensagemRetorno<Pollux.MSG0030R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public SubfamiliaProduto DefinirPropriedades(Intelbras.Message.Helper.MSG0030 xml)
        {
            var crm = new SubfamiliaProduto(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

                crm.Codigo = xml.CodigoSubFamilia;

                crm.Nome = xml.Nome;

                FamiliaProduto Familia = new Intelbras.CRM2013.Domain.Servicos.FamiliaProdutoService(this.Organizacao, this.IsOffline)
                                            .BuscaFamiliaProduto(xml.Familia);
                if (Familia != null)
                {
                    crm.Familia = new  Lookup((Guid)Familia.ID, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "(Identificador Família não encontrado";
                    return crm;
                }

                crm.Status = xml.Situacao;

                crm.IntegradoEm = DateTime.Now;

                crm.IntegradoPor = usuarioIntegracao.NomeCompleto;

                crm.UsuarioIntegracao = xml.LoginUsuario;

            #endregion

            return crm;
        }
        #endregion

        #region Métodos Auxiliares
    
        public string Enviar(SubfamiliaProduto objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
