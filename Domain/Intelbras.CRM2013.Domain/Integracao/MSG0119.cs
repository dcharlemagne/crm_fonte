using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using SDKore.DomainModel;
using SDKore.Helper;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0119 : Base, IBase<Intelbras.Message.Helper.MSG0119, Domain.Model.AcessoExtranetContato>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao = null;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor

        public MSG0119(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }
        #endregion

        #region Executar
        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            usuarioIntegracao = usuario;
            var Trace = new SDKore.Helper.Trace("MSG0119");

            try
            {
                Trace.Add("Mensagem " + numeroMensagem + " XML: {0}", mensagem);

                var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0119>(mensagem));

                Trace.Add("DefinirPropriedades");
                
                if (!resultadoPersistencia.Sucesso)
                {
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0119R1>(numeroMensagem, retorno);
                }

                objeto = new Intelbras.CRM2013.Domain.Servicos.AcessoExtranetContatoService(this.Organizacao, this.IsOffline).Persistir(objeto);
                if (objeto == null)
                {
                    Trace.Add("Registro não encontrado");

                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Registro não encontrado!";
                    return CriarMensagemRetorno<Pollux.MSG0119R1>(numeroMensagem, retorno);
                }
                else
                {
                    Trace.Add("Integração com sucesso");

                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                }

                Trace.Add("CodigoAcesso: {0}", objeto.ID.Value.ToString());

                retorno.Add("CodigoAcesso", objeto.ID.Value.ToString());
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0119R1>(numeroMensagem, retorno);
            }
            finally
            {
                Trace.SaveClear();
            }
        }
        #endregion

        #region Definir Propriedades
        public AcessoExtranetContato DefinirPropriedades(Intelbras.Message.Helper.MSG0119 xml)
        {
            if (!xml.AceitouTermoDeUso.HasValue)
            {
                throw new ArgumentException("(CRM) Identificador do AceitouTermoDeUso não encontrado!");
            }

            var crm = new AcessoExtranetContato(this.Organizacao, this.IsOffline)
            {
                IntegrarNoPlugin = true,
                Status = xml.Situacao,
                Nome = xml.Nome,
                UsuarioAceitouTermoUso = xml.AceitouTermoDeUso,
                DataAceiteTermo = xml.DataAceiteTermoDeUso
            };

            #region Propriedades Crm->Xml

            if (!string.IsNullOrEmpty(xml.CodigoAcesso)) crm.ID = new Guid(xml.CodigoAcesso);

            if (!string.IsNullOrEmpty(xml.TipoAcesso))
            {
                crm.TipoAcesso = new Lookup(new Guid(xml.TipoAcesso), "");
            }
            else
            {
                throw new ArgumentException("(CRM) Identificador do Tipo de Acesso não encontrado!");
            }

            if (!string.IsNullOrEmpty(xml.Contato))
            {
                Contato contato = new ContatoService(this.Organizacao, this.IsOffline).BuscaContato(new Guid(xml.Contato));
                if (contato != null)
                {
                    crm.Contato = new Lookup(contato.ID.Value, "");
                }
                else
                {
                    throw new ArgumentException("(CRM) Contato não cadastrado no Crm!");
                }
            }
            else
            {
                throw new ArgumentException("(CRM) Identificador do Contato não encontrado!");
            }

            if (!string.IsNullOrEmpty(xml.PerfilAcesso))
            {
                crm.AcessoExtranetid = new Lookup(new Guid(xml.PerfilAcesso), "");
            }
            else
            {
                throw new ArgumentException("(CRM) Identificador Perfil Acesso Extranet não encontrado!");
            }

            if (!string.IsNullOrEmpty(xml.Conta))
            {
                Conta contaObj = new Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(new Guid(xml.Conta));
                if (contaObj != null)
                {
                    crm.Canal = new Lookup(contaObj.ID.Value, "");
                }
                else
                {
                    throw new ArgumentException("(CRM) Conta não cadastrada no Crm!");
                }
            }



            if (xml.DataValidade.HasValue)
                crm.Validade = xml.DataValidade;
            else
            {
                throw new ArgumentException("(CRM) Identificador da Data Validade não encontrado!");
            }

            #endregion

            return crm;
        }

        private Intelbras.Message.Helper.MSG0119 DefinirPropriedades(AcessoExtranetContato crm)
        {
            var xml = new Pollux.MSG0119(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), crm.Nome.Truncate(40));

            xml.Nome = crm.Nome;
            if (crm.Validade.HasValue)
                xml.DataValidade = crm.Validade.Value.ToLocalTime();
            else
                xml.DataValidade = null;
            if (crm.DataAceiteTermo.HasValue)
                xml.DataAceiteTermoDeUso = crm.DataAceiteTermo.Value.ToLocalTime();
            else
                xml.DataAceiteTermoDeUso = null;
            xml.AceitouTermoDeUso = crm.UsuarioAceitouTermoUso;

            if (crm.ID.HasValue) xml.CodigoAcesso = crm.ID.Value.ToString();
            if (crm.TipoAcesso != null) xml.TipoAcesso = crm.TipoAcesso.Id.ToString();
            if (crm.Contato != null) xml.Contato = crm.Contato.Id.ToString();
            if (crm.AcessoExtranetid != null) xml.PerfilAcesso = crm.AcessoExtranetid.Id.ToString();
            if (crm.Canal != null) xml.Conta = crm.Canal.Id.ToString();
            xml.Situacao = (crm.Status.HasValue ? crm.Status.Value : 0);

            if (usuarioIntegracao != null)
            {
                xml.Proprietario = usuarioIntegracao.ID.Value.ToString();
                xml.TipoProprietario = "systemuser";
            }else
            {
                xml.Proprietario = "259A8E4F-15E9-E311-9420-00155D013D39";
                xml.TipoProprietario = "systemuser";
            }

            return xml;
        }
        #endregion

        public string Enviar(AcessoExtranetContato objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0119 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0119R1 retorno = CarregarMensagem<Pollux.MSG0119R1>(resposta);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new ArgumentException("(CRM) " + retorno.Resultado.Mensagem);
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(resposta);
                throw new ArgumentException("(CRM) " + erro001.GenerateMessage(false));
            }
            return resposta;
        }
    }
}
