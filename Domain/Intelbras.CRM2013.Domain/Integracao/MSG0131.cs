using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0131 : Base, IBase<Message.Helper.MSG0131, Domain.Model.Conexao>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0131(string org, bool isOffline)
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

            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0131>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0131R1>(numeroMensagem, retorno);
            }

            objeto = new Intelbras.CRM2013.Domain.Servicos.ConexaoService(this.Organizacao, this.IsOffline).Persistir((Guid)usuario.ID, objeto);

            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Registro não encontrado!";

            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                retorno.Add("CodigoConexao", objeto.ID.Value.ToString());
            }

            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0131R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public Conexao DefinirPropriedades(Intelbras.Message.Helper.MSG0131 xml)
        {
            var crm = new Conexao(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            if (!string.IsNullOrEmpty(xml.Descricao))
            crm.Descricao = xml.Descricao;
            else
                crm.AddNullProperty("Descricao");

            if (xml.DataInicio.HasValue)
            crm.Inicio = xml.DataInicio;
            else
                crm.AddNullProperty("Inicio");

            if (xml.DataTermino.HasValue)
                crm.Termino = xml.DataTermino;
            else
                crm.AddNullProperty("Termino");
            
            crm.Status = xml.Situacao;

            if (!String.IsNullOrEmpty(xml.FuncaoPartePrincipal))
            {
                List<FuncaoConexao> lstFuncaoConexao = new Intelbras.CRM2013.Domain.Servicos.ConexaoService(this.Organizacao, this.IsOffline).ListarFuncaoConexaoPorNome(xml.FuncaoPartePrincipal);

                if (lstFuncaoConexao.Any<FuncaoConexao>())
                {
                    crm.Funcao_De = new SDKore.DomainModel.Lookup(lstFuncaoConexao.First<FuncaoConexao>().ID.Value, "");
                }
                else
                {      
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Funcão Parte Principal não encontrado.";
                    return crm;
                }
            }
            if (!String.IsNullOrEmpty(xml.FuncaoParteSecundaria))
            {
                List<FuncaoConexao> lstFuncaoConexaoSecundaria = new Intelbras.CRM2013.Domain.Servicos.ConexaoService(this.Organizacao, this.IsOffline).ListarFuncaoConexaoPorNome(xml.FuncaoParteSecundaria);

                if (lstFuncaoConexaoSecundaria.Any<FuncaoConexao>())
                {
                    crm.Funcao_Ate = new SDKore.DomainModel.Lookup(lstFuncaoConexaoSecundaria.First<FuncaoConexao>().ID.Value, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Funcão Parte Secundária não encontrada.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("Funcao_Ate");
            }

            crm.Conectado_a = new SDKore.DomainModel.Lookup(new Guid(xml.CodigoContato), "contact");
            crm.Conectado_de = new SDKore.DomainModel.Lookup(new Guid(xml.CodigoConta), "account");

            Guid conexaoId = Guid.Empty;
            if (!String.IsNullOrEmpty(xml.CodigoConexao) && Guid.TryParse(xml.CodigoConexao, out conexaoId))
                crm.ID = conexaoId;

            #endregion

            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(Conexao objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
