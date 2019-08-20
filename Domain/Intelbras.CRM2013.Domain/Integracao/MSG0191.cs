using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using Intelbras.CRM2013.Domain.Servicos;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0191 : Base, IBase<Pollux.MSG0191, Endereco>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        #endregion


        #region Construtor
        public MSG0191(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0191>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0191R1>(numeroMensagem, retorno);
            }

            var service = new EnderecoService(this.Organizacao, this.IsOffline);
            if(objeto.StatusAtivo == true)
            {
                objeto = service.Persistir(objeto);
            }
            else
            {
                objeto = service.Remover(objeto);
            }
            
            if (objeto == null)
            {
                resultadoPersistencia.Sucesso  = false;
                resultadoPersistencia.Mensagem = "Registro não encontrado!";
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            }

            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0191R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public Endereco DefinirPropriedades(Intelbras.Message.Helper.MSG0191 xml)
        {
            var crm = new Endereco(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            crm.IntegrarNoPlugin = false;
            crm.CodigoEndereco = xml.CodigoEntrega;
            
            Conta conta = new ContaService(this.Organizacao, this.IsOffline).BuscarPorCodigoEmitente(xml.CodigoCliente.ToString());
            if (conta == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(CRM) O Código do Cliente não foi encontrado.";
                return crm;
            }

            crm.Conta = new Lookup(new Guid(conta.ID.ToString()), "account");
            crm.StatusAtivo = (xml.Situacao == (int)Enum.StateCode.Ativo) ? true : false;

            if (xml.CodigoTaxa.HasValue)
                crm.CodigoTaxa = xml.CodigoTaxa.Value;
            else
                crm.AddNullProperty("CodigoTaxa");

            if (xml.CodigoTipoEntrega.HasValue)
                crm.CodigoTipoEntrega = xml.CodigoTipoEntrega.Value;
            else
                crm.AddNullProperty("CodigoTipoEntrega");

            if (!String.IsNullOrEmpty(xml.CpfCnpjCodEstrangeiro))
                crm.Identificacao = xml.CpfCnpjCodEstrangeiro;
            else
                crm.AddNullProperty("Identificacao");

            if (!String.IsNullOrEmpty(xml.InscricaoEstadual))
                crm.InscricaoEstadual = xml.InscricaoEstadual;
            else
                crm.AddNullProperty("InscricaoEstadual");

            if (!String.IsNullOrEmpty(xml.Email))
                crm.Email = xml.Email;
            else
                crm.AddNullProperty("Email");

            if (xml.TipoEndereco.HasValue)
                crm.TipoEndereco = xml.TipoEndereco.Value;
            else
                crm.AddNullProperty("TipoEndereco");
            
            crm.Numero = xml.NomeEndereco;

            if (!String.IsNullOrEmpty(xml.CaixaPostal))
                crm.CaixaPostal = xml.CaixaPostal;
            else
                crm.AddNullProperty("CaixaPostal");

            crm.Cep = xml.CEP;
            crm.Numero = xml.Numero;

            if (!String.IsNullOrEmpty(xml.NomeEndereco))
                crm.EnderecoNumero = xml.NomeEndereco;
            else
                crm.AddNullProperty("EnderecoNumero");

            if (!String.IsNullOrEmpty(xml.Complemento))
                crm.Complemento = xml.Complemento;
            else
                crm.AddNullProperty("Complemento");

            crm.Bairro = xml.Bairro;
            crm.NomeCidade = xml.NomeCidade;
            crm.SiglaEstado = xml.UF;
            crm.NomePais = xml.NomePais;

            if (!String.IsNullOrEmpty(xml.Observacao))
                crm.Observacao = xml.Observacao;
            else
                crm.AddNullProperty("Observacao");

            #endregion

            return crm;
        }
        #endregion

        public Pollux.MSG0191 DefinirPropriedades(Endereco crm)
        {
            #region Msg
            Conta conta = new ContaService(this.Organizacao, this.IsOffline).BuscaConta(crm.Conta.Id);

            Pollux.MSG0191 xml = new Pollux.MSG0191(
                itb.RetornaSistema(itb.Sistema.CRM), 
                Helper.Truncate(String.Format("{0} / {1}", crm.CodigoEndereco, conta.NomeAbreviado), 40)
            );            

            #region campos-obrigatorios
            xml.CodigoCliente = (int)Convert.ToInt32(conta.CodigoMatriz);
            xml.CodigoEntrega = crm.CodigoEndereco;
            xml.NomeAbreviado = conta.NomeAbreviado;
            xml.Situacao = crm.StatusAtivo ? (int)Enum.StateCode.Ativo : (int)Enum.StateCode.Inativo;
            xml.NomeEndereco = crm.EnderecoNumero;
            xml.CEP = crm.Cep.Replace("-","").Replace(".","");
            xml.Numero = crm.Numero;
            xml.Bairro = crm.Bairro;
            xml.NomeCidade = crm.NomeCidade;
            xml.Cidade = String.Format("{0},{1},{2}", crm.NomeCidade, crm.SiglaEstado, crm.NomePais);
            xml.UF = crm.SiglaEstado;
            xml.Estado = String.Format("{0},{1}", crm.NomePais, crm.SiglaEstado);
            xml.NomePais = crm.NomePais;
            xml.Pais = crm.NomePais;
            #endregion

            #region campos-nao-obrigatorios
            if (crm.CodigoTaxa.HasValue)
                xml.CodigoTaxa = crm.CodigoTaxa;

            if (crm.CodigoTipoEntrega.HasValue)
                xml.CodigoTipoEntrega = crm.CodigoTipoEntrega;

            if (!String.IsNullOrEmpty(crm.Identificacao))
                xml.CpfCnpjCodEstrangeiro = crm.Identificacao;

            if (!String.IsNullOrEmpty(crm.InscricaoEstadual))
                xml.InscricaoEstadual = crm.InscricaoEstadual;

            if (!String.IsNullOrEmpty(crm.Email))
                xml.Email = crm.Email;

            if (crm.TipoEndereco.HasValue)
                xml.TipoEndereco = crm.TipoEndereco;

            if (!String.IsNullOrEmpty(crm.CaixaPostal))
                xml.CaixaPostal = crm.CaixaPostal;
            
            if (!String.IsNullOrEmpty(crm.Complemento))
                xml.Complemento = crm.Complemento;

            if (!String.IsNullOrEmpty(crm.Observacao))
                xml.Observacao = crm.Observacao;
            #endregion

            #endregion
            return xml;
        }

        #region Métodos Auxiliares

        public string Enviar(Endereco objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0191 mensagem = this.DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0191R1 retorno = CarregarMensagem<Pollux.MSG0191R1>(resposta);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new ArgumentException("(CRM) " + string.Concat(retorno.Resultado.Mensagem));
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(resposta);
                throw new ArgumentException("(CRM) " + string.Concat(erro001.GenerateMessage(false)));
            }
            return resposta;
        }
        #endregion
    }
}

