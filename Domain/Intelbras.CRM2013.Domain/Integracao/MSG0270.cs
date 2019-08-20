using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0270 : Base, IBase<Message.Helper.MSG0270, Domain.Model.Postagem>
    {
        #region Construtor

        public MSG0270(string org, bool isOffline)
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

            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0270>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0270R1>(numeroMensagem, retorno);
            }

            objeto = new Domain.Servicos.PostagemService(this.Organizacao, this.IsOffline).Persistir(objeto);
            if (objeto.Id != null)
            {
                retorno.Add("Resultado", resultadoPersistencia);
            }

            return CriarMensagemRetorno<Pollux.MSG0270R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public Postagem DefinirPropriedades(Intelbras.Message.Helper.MSG0270 xml)
        {
            var crm = new Model.Postagem(this.Organizacao, this.IsOffline);

            if (!String.IsNullOrEmpty(xml.NumeroProjeto))
            {
                ClientePotencial cliente = new Servicos.LeadService(this.Organizacao, this.IsOffline).ObterPorNumeroProjeto(xml.NumeroProjeto);
                if (cliente != null)
                {
                    Oportunidade oportunidade = new Servicos.RepositoryService().Oportunidade.BuscarPor(cliente);

                    if (xml.ClassificacaoProjeto.HasValue)
                    {
                        if (xml.ClassificacaoProjeto == 993520000) // Cliente Potencial
                        {
                            crm.CriadoEm = xml.DataAtividade;
                            crm.UsuarioAtividade = new Lookup(usuarioIntegracao.Id, "");

                            Contato contato = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(new Guid(xml.CodigoContato));
                            if (contato != null)
                            {
                                crm.Texto = "Mensagem postada por " + contato.Nome + ": " + xml.DescricaoAtividade;
                            }
                            else
                            {
                                resultadoPersistencia.Sucesso = false;
                                resultadoPersistencia.Mensagem = "CodigoExecutivo não encontrado no Crm.";
                                return crm;
                            }
                            crm.ReferenteA = new SDKore.DomainModel.Lookup(cliente.ID.Value, "lead");
                            crm.Source = 2;
                        }
                        if (xml.ClassificacaoProjeto == 993520001 ) // Oportunidade
                        {
                            if (oportunidade != null)
                            {
                                crm.CriadoEm = xml.DataAtividade;
                                crm.UsuarioAtividade = new Lookup(usuarioIntegracao.Id, "");
                                Contato contato = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(new Guid(xml.CodigoContato));
                                if (contato != null)
                                {
                                    crm.Texto = "Mensagem postada por " + contato.Nome + ": " + xml.DescricaoAtividade;
                                }
                                else
                                {
                                    resultadoPersistencia.Sucesso = false;
                                    resultadoPersistencia.Mensagem = "CodigoExecutivo não encontrado no Crm.";
                                    return crm;
                                }
                                crm.ReferenteA = new SDKore.DomainModel.Lookup(oportunidade.ID.Value, "opportunity");
                                crm.Source = 2;
                            }
                        }
                    } else {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "ClassificacaoProjeto não informado, campo obrigatório.";
                        return crm;
                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "NumeroProjeto informado não existe para ser atualizado.";
                    return crm;
                }
            } else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "NumeroProjeto não informado, campo obrigatório.";
                return crm;
            }

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Postagem objModel)
        {
            return String.Empty;
        }

        #endregion
    }
}
