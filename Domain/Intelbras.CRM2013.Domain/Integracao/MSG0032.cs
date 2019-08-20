using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0032 : Base, IBase<Message.Helper.MSG0032, Domain.Model.Origem>
    {

        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0032(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0032>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0032R1>(numeroMensagem, retorno);
            }

            objeto.ID = new Intelbras.CRM2013.Domain.Servicos.OrigemService(this.Organizacao, this.IsOffline).Persistir(objeto);
            if (!objeto.ID.HasValue)
                throw new Exception("(CRM) Erro de persistência");

            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0032R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public Origem DefinirPropriedades(Intelbras.Message.Helper.MSG0032 xml)
        {
            var crm = new Origem(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(xml.Nome))
            {
                crm.Nome = xml.Nome;
            }
            if (!String.IsNullOrEmpty(xml.CodigoOrigem))
            {
                crm.Codigo = xml.CodigoOrigem;
            }
            
            if (!String.IsNullOrEmpty(xml.SubFamilia))
            {
                Model.SubfamiliaProduto subFamilia = new Model.SubfamiliaProduto(this.Organizacao, this.IsOffline);
                subFamilia = new Intelbras.CRM2013.Domain.Servicos.SubFamiliaProdutoService(this.Organizacao, this.IsOffline).BuscaSubfamiliaProduto(xml.SubFamilia);

                if (subFamilia != null && subFamilia.Id != Guid.Empty)
                    crm.Subfamilia = new Lookup(subFamilia.Id, "");
                else
                {
                    //Se não achou a subfamilia, pode ter sido apagada do CRM e inserida novamente
                    //resultadoPersistencia.Sucesso = false;
                    //resultadoPersistencia.Mensagem = "SubfamiliaProduto não encontrado!";
                    //return crm;
                }
            }

            crm.Status = xml.Situacao;

            crm.IntegradoEm = DateTime.Now;
            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            crm.UsuarioIntegracao = xml.LoginUsuario;
            
            #endregion

            return crm;
        }
              

        #region Métodos Auxiliares


        public string EnviarOrigem(Origem objModel)
        {
            #region Retorno
            Pollux.MSG0032 msg0032 = new Pollux.MSG0032(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0032")
            {
                Nome = objModel.Nome,
                CodigoOrigem = objModel.Codigo,
                SubFamilia = objModel.Subfamilia.Name
                
            };
            #endregion

            return msg0032.GenerateMessage(false);
        }
                
        private Intelbras.Message.Helper.MSG0032 DefinirPropriedades(Origem crm)
        {
            Intelbras.Message.Helper.MSG0032 xml = new Pollux.MSG0032(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.Nome, 40));

                xml.Nome = crm.Nome;
                xml.CodigoOrigem = crm.Codigo;
                xml.SubFamilia = crm.Subfamilia.Name;

            return xml;
        }


        #endregion

        public string Enviar(Origem objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0032 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0032R1 retorno = CarregarMensagem<Pollux.MSG0032R1>(resposta);
                return retorno.Resultado.Mensagem;
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 retorno = CarregarMensagem<Pollux.ERR0001>(resposta);
                return retorno.DescricaoErro;
            }

        }   

        #endregion
    }
}
        