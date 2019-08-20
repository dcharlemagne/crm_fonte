using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0169 : Base, IBase<Message.Helper.MSG0169, Domain.Model.ItemFila>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0169(string org, bool isOffline)
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
            var xml = this.CarregarMensagem<Pollux.MSG0169>(mensagem);

            try
            {
                Validacao(xml);

                Guid itemFilaGuid;
                if (!String.IsNullOrEmpty(xml.CodigoItemFila) && !Guid.TryParse(xml.CodigoItemFila, out itemFilaGuid))
                {
                    throw new ArgumentException("(CRM) Código item fila em formato inválido.");
                }

                Guid contatoGuid;
                if (!Guid.TryParse(xml.CodigoContato, out contatoGuid))
                {
                    throw new ArgumentException("(CRM) Guid do código do contato em formato inválido.");
                }

                Contato contato = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(contatoGuid);

                if (contato == null)
                {
                    throw new ArgumentException("(CRM) Contato não encontrado.");
                }

                Fila fila = new Intelbras.CRM2013.Domain.Servicos.FilaServices(this.Organizacao, this.IsOffline).BuscaFilaPorNome(xml.NomeFilaAtendimento);

                if (fila == null)
                {
                    throw new ArgumentException("(CRM) Fila de atendimento não encontrada.");
                }

                Guid referenteA;

                if (xml.Referente == SDKore.Crm.Util.Utility.GetEntityName<Pedido>())
                {
                    Pedido pedido = new Intelbras.CRM2013.Domain.Servicos.PedidoService(this.Organizacao, this.IsOffline).BuscaPedidoEMS(xml.NumeroPedido);

                    if (pedido == null)
                    {
                        throw new ArgumentException("(CRM) Pedido não encontrado.");
                    }

                    referenteA = pedido.ID.Value;
                }
                else
                {
                    if (!Guid.TryParse(xml.CodigoObjeto, out referenteA))
                    {
                        throw new ArgumentException("(CRM) Não foi possível conversa o campo 'CodigoObjeto'.");
                    }
                }


                Email email = new Email(this.Organizacao, this.IsOffline);

                email.De = new Lookup[1];
                email.De[0] = new Lookup(contato.ID.Value, "contact");
                email.Para = new Lookup[1];
                email.Para[0] = new Lookup(fila.ID.Value, "queue");
                email.Assunto = xml.Assunto;
                email.Mensagem = xml.DescricaoEmail;
                email.ReferenteA = new Lookup(referenteA, xml.Referente);
                email.Direcao = xml.Direcao.Value == 1;

                email.ID = new RepositoryService().Email.Create(email);

                new RepositoryService().Email.AlterarStatus(email.ID.Value, xml.StatusEmail.Value);

                ItemFila itemFila = new ItemFila(this.Organizacao, this.IsOffline);
                itemFila.Fila = new Lookup(fila.ID.Value, "queue");
                itemFila.Objeto = new Lookup(email.ID.Value, "email");
                itemFila.RazaoStatus = 1;


                itemFila.ID = new RepositoryService().ItemFila.Create(itemFila);

                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração realizada com sucesso!";
                retorno.Add("CodigoItemFila", itemFila.ID.Value.ToString());
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0169R1>(numeroMensagem, retorno);
            }
            catch (Exception e)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(e);
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0169R1>(numeroMensagem, retorno);
            }
        }
        #endregion

        public ItemFila DefinirPropriedades(Intelbras.Message.Helper.MSG0169 xml)
        {
            var crm = new Model.ItemFila(this.Organizacao, this.IsOffline);
            return crm;
        }

        private void Validacao(Pollux.MSG0169 xml)
        {
            if (String.IsNullOrEmpty(xml.CodigoContato))
            {
                throw new ArgumentException("(CRM) Código do contato não foi enviado.");
            }

            if (String.IsNullOrEmpty(xml.NomeFilaAtendimento))
            {
                throw new ArgumentException("(CRM) Nome da fila de atendimento não foi enviado.");
            }

            if (String.IsNullOrEmpty(xml.Assunto))
            {
                throw new ArgumentException("(CRM) Assunto não foi enviado.");
            }

            if (String.IsNullOrEmpty(xml.DescricaoEmail))
            {
                throw new ArgumentException("(CRM) Descrição do e-mail não foi enviada.");
            }

            if (xml.Direcao != 0 && xml.Direcao != 1)
            {
                throw new ArgumentException("(CRM) Direção inválida.");
            }

            if (String.IsNullOrEmpty(xml.Referente))
            {
                throw new ArgumentException("(CRM) Referente não foi enviado.");
            }

            if (xml.Referente == SDKore.Crm.Util.Utility.GetEntityName<Pedido>())
            {
                if (String.IsNullOrEmpty(xml.NumeroPedido))
                {
                    throw new ArgumentException("(CRM) Número do pedido não foi enviado.");
                }
            }
            else
            {
                if (String.IsNullOrEmpty(xml.CodigoObjeto))
                {
                    throw new ArgumentException("(CRM) Código do Objeto não foi enviado.");
                }
            }

            if (!System.Enum.IsDefined(typeof(Enum.Email.StatusEmail), xml.StatusEmail))
            {
                throw new ArgumentException("(CRM) Status de e-mail inválido.");
            }
        }

        #region Métodos Auxiliares

        public string Enviar(ItemFila objModel)
        {
            return String.Empty;
        }
        #endregion

    }
}
