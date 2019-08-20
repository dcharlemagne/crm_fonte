using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0127 : Base, IBase<Message.Helper.MSG0127, Domain.Model.Usuario>
    {

        #region Propriedades

        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        //private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0127(string org, bool isOffline)
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

            var xml = this.CarregarMensagem<Pollux.MSG0127>(mensagem);

            List<Pollux.Entities.Usuario> lstUserPollux = new List<Pollux.Entities.Usuario>();
            if (!String.IsNullOrEmpty(xml.CodigoUsuario) && xml.CodigoUsuario.Length == 36)
            {
                if (!String.IsNullOrEmpty(xml.TipoObjetoUsuario) && (xml.TipoObjetoUsuario.ToLower().Equals("systemuser") || xml.TipoObjetoUsuario.ToLower().Equals("team")))
                {
                    #region User
                    if (xml.TipoObjetoUsuario.ToLower().Equals("systemuser"))
                    {
                        List<Usuario> lstUsuario = new Servicos.UsuarioService(this.Organizacao, this.IsOffline).ListarPor(new Guid(xml.CodigoUsuario));

                        if (lstUsuario != null && lstUsuario.Count > 0)
                        {
                            foreach (Usuario item in lstUsuario)
                            {
                                Pollux.Entities.Usuario user = new Pollux.Entities.Usuario();
                                user.CodigoUsuario = item.ID.Value.ToString();
                                if (!String.IsNullOrEmpty(item.NomeCompleto))
                                    user.Nome = item.NomeCompleto;
                                else
                                {
                                    resultadoPersistencia.Sucesso = false;
                                    resultadoPersistencia.Mensagem = "NomeUsuario não preenchido no CRM. User: " + item.ID.Value.ToString();
                                    retorno.Add("Resultado", resultadoPersistencia);
                                    return CriarMensagemRetorno<Pollux.MSG0127R1>(numeroMensagem, retorno);
                                }
                                if (!String.IsNullOrEmpty(item.EmailPrimario))
                                    user.Email = item.EmailPrimario;
                                user.TipoObjetoUsuario = "systemuser";
                                user.Situacao = item.IsDisabled ? 0 : 1;
                                lstUserPollux.Add(user);
                            }
                        }
                        else
                        {
                            resultadoPersistencia.Sucesso = true;
                            resultadoPersistencia.Mensagem = "LoginUsuario não encontrado no CRM.";
                            retorno.Add("Resultado", resultadoPersistencia);
                            return CriarMensagemRetorno<Pollux.MSG0127R1>(numeroMensagem, retorno);
                        }



                    }
                    #endregion

                    #region Equipe
                    else if (xml.TipoObjetoUsuario.ToLower().Equals("team"))
                    {
                        List<Equipe> lstEquipe = new Servicos.EquipeService(this.Organizacao, this.IsOffline).ListarEquipe(new Guid(xml.CodigoUsuario));

                        if (lstEquipe != null && lstEquipe.Count > 0)
                        {
                            foreach (var item in lstEquipe)
                            {
                                Pollux.Entities.Usuario user = new Pollux.Entities.Usuario();
                                user.CodigoUsuario = item.ID.Value.ToString();
                                if (!String.IsNullOrEmpty(item.Nome))
                                    user.Nome = item.Nome;
                                else
                                {
                                    resultadoPersistencia.Sucesso = false;
                                    resultadoPersistencia.Mensagem = "NomeEquipe não preenchido no CRM. User: " + item.ID.Value.ToString();
                                    retorno.Add("Resultado", resultadoPersistencia);
                                    return CriarMensagemRetorno<Pollux.MSG0127R1>(numeroMensagem, retorno);
                                }
                                user.TipoObjetoUsuario = "team";
                                user.Situacao = 1;
                                lstUserPollux.Add(user);

                            }
                        }
                        else
                        {
                            resultadoPersistencia.Sucesso = false;
                            resultadoPersistencia.Mensagem = "Equipe não encontrado no CRM.";
                            retorno.Add("Resultado", resultadoPersistencia);
                            return CriarMensagemRetorno<Pollux.MSG0127R1>(numeroMensagem, retorno);
                        }
                    }
                    #endregion
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoUsuario/TipoObjetoUsuario não enviado ou fora do padrão.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0127R1>(numeroMensagem, retorno);
            }
            retorno.Add("UsuariosItens", lstUserPollux);
            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0127R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public Usuario DefinirPropriedades(Intelbras.Message.Helper.MSG0127 xml)
        {
            var crm = new Model.Usuario(this.Organizacao, this.IsOffline);

            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(Usuario objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
