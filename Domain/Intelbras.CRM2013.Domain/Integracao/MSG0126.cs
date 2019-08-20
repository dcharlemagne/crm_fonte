using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0126 : Base, IBase<Message.Helper.MSG0126, Domain.Model.AcessoExtranet>
    {
        #region Propriedades
            //Dictionary que sera enviado como resposta do request
            private Dictionary<string, object> retorno = new Dictionary<string, object>();
            private Domain.Model.Usuario usuarioIntegracao;
            Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
            public MSG0126(string org, bool isOffline)
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
                var xml = this.CarregarMensagem<Pollux.MSG0126>(mensagem);

                if (!String.IsNullOrEmpty(xml.TipoAcesso))
                {
                    List<AcessoExtranet> lstAcessoExtranet = new List<AcessoExtranet>();
                    List<Pollux.Entities.Perfil> lstPerfil = new List<Pollux.Entities.Perfil>();
                    lstAcessoExtranet = new Servicos.AcessoExtranetService(this.Organizacao, this.IsOffline).ListarAcessoExtranet(new Guid(xml.TipoAcesso));
                    if (lstAcessoExtranet != null && lstAcessoExtranet.Count > 0)
                    {
                        foreach (AcessoExtranet item in lstAcessoExtranet)
                        {
                            Pollux.Entities.Perfil perfil = new Pollux.Entities.Perfil();
                            perfil.CodigoPerfilAcesso = item.ID.Value.ToString();
                            perfil.Nome = item.Nome;
                            perfil.Situacao = item.Status.Value;
                            lstPerfil.Add(perfil);
                        }

                        resultadoPersistencia.Sucesso = true;
                        resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
                        retorno.Add("PerfisAcessoItens", lstPerfil);
                    }
                    else
                    {
                        resultadoPersistencia.Sucesso = true;
                        resultadoPersistencia.Mensagem = "Perfil acesso não encontrado no CRM.";
                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Tipo de Acesso não enviado.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0126R1>(numeroMensagem, retorno);
                }
                
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0126R1>(numeroMensagem, retorno);
            }
        #endregion

        #region Definir Propriedades

            public AcessoExtranet DefinirPropriedades(Intelbras.Message.Helper.MSG0126 xml)
            {
                Model.AcessoExtranet crm = new AcessoExtranet(this.Organizacao, this.IsOffline);

                return crm;
            }

            
        #endregion

        #region Métodos Auxiliares

            public string Enviar(AcessoExtranet objModel)
            {
                string resposta = String.Empty;
                return resposta;

            }
        #endregion        

    }
}
