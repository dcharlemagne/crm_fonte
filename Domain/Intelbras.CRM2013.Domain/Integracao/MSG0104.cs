using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0104 : Base, IBase<Message.Helper.MSG0104, Domain.Model.TurmaCanal>
    {

        #region Construtor

        public MSG0104(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0104>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0104R1>(numeroMensagem, retorno);
            }

            objeto = new Servicos.TurmaCanalService(this.Organizacao, this.IsOffline).Persistir(objeto);

            if (objeto != null)
            {
                retorno.Add("GUIDTurma", objeto.ID.Value.ToString());
                retorno.Add("Proprietario", usuarioIntegracao.ID.Value.ToString());
                retorno.Add("TipoProprietario", "systemuser");
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";
                retorno.Add("Resultado", resultadoPersistencia);
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro de Persistência!";
                retorno.Add("Resultado", resultadoPersistencia);
            }
            return CriarMensagemRetorno<Pollux.MSG0104R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public TurmaCanal DefinirPropriedades(Intelbras.Message.Helper.MSG0104 xml)
        {
            var crm = new TurmaCanal(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml


            if (!String.IsNullOrEmpty(xml.NomeTurma))
            {
                crm.Nome = xml.NomeTurma;
            }
            else 
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "NomeTurma não enviado!";
                return crm;
            }
            
            if (xml.Situacao == 1 || xml.Situacao == 0)
                crm.State = xml.Situacao;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Situação não enviada!";
                return crm;
            }

            if (xml.IdentificadorTurma.HasValue)
                crm.IdTurma = xml.IdentificadorTurma.Value.ToString();
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "IdentificadorTurma não enviado!";
                return crm;
            }

            if (xml.DataInicio.HasValue)
                crm.DataInicio = xml.DataInicio;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "DataInicio não enviada!";
                return crm;
            }

            if (xml.DataTermino.HasValue)
                crm.DataTermino = xml.DataTermino;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "DataTermino não enviada!";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.CodigoConta)
                && xml.CodigoConta.Length == 36)
            {
                crm.Canal = new Lookup(new Guid(xml.CodigoConta), "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoConta não enviada ou fora do padrão(Guid)!";
                return crm;
            }
            #endregion

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(TurmaCanal objModel)
        {
            string resposta = string.Empty;

            return resposta;
        }

        #endregion

    }
}
