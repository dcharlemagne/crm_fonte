// MENSAGEM RELACIONADA AO PROCESSO REGISTRA IMOVEIS
// IMPLEMENTADA EM 18/03/14 POR FCJ

using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0066 : Base, IBase<Intelbras.Message.Helper.MSG0066, Domain.Model.Bens>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0066(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0066>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0066R1>(numeroMensagem, retorno);
            }

            objeto = new Intelbras.CRM2013.Domain.Servicos.BensService(this.Organizacao, this.IsOffline).Persistir(objeto);
            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Registro não encontrado!";

            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                retorno.Add("CodigoImovel", objeto.ID.Value.ToString());
            }
            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0066R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public Bens DefinirPropriedades(Intelbras.Message.Helper.MSG0066 xml)
        {
            var crm = new Bens(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml
            //ID não obrigatório - Create
            if (!String.IsNullOrEmpty(xml.CodigoImovel))
                crm.ID = new Guid(xml.CodigoImovel);


            if(String.IsNullOrEmpty(xml.Contato) && String.IsNullOrEmpty(xml.Conta))
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Conta e Contato não foram enviados, deve ser enviado ao menos um destes objetos.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.Contato))
            {
                if (xml.Contato.Length == 36)
                {
                    crm.Contato = new Lookup(new Guid(xml.Contato), "");

                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Identificador do Contato enviado fora do padrão.";
                    return crm;
                }
            }
            
            if (!String.IsNullOrEmpty(xml.Conta))
            {
                if (xml.Conta.Length == 36)
                {
                    crm.Conta = new Lookup(new Guid(xml.Conta), "");

                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Identificador do Canal enviado fora do padrão.";
                    return crm;
                }
            }

            if (!String.IsNullOrEmpty(xml.EnderecoImovel.Numero))
                crm.Numero = xml.EnderecoImovel.Numero;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Numero do endereço do imóvel não enviado.";
                return crm;
            }
            if (!String.IsNullOrEmpty(xml.EnderecoImovel.Logradouro))
                crm.Endereco = xml.EnderecoImovel.Logradouro;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Logradouro do endereço do imóvel não enviado.";
                return crm;
            }
            if (!String.IsNullOrEmpty(xml.EnderecoImovel.Bairro))
                crm.Bairro = xml.EnderecoImovel.Bairro;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(Bairro do endereço do imóvel não enviado.";
                return crm;
            }
            if (!String.IsNullOrEmpty(xml.EnderecoImovel.CEP))
                crm.CEP = xml.EnderecoImovel.CEP;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CEP do endereço do imóvel não enviado.";
                return crm;
            }
            //Cidade - service
            if (!String.IsNullOrEmpty(xml.EnderecoImovel.Cidade))
            {
                Municipio cidade = new Intelbras.CRM2013.Domain.Servicos.MunicipioServices(this.Organizacao, this.IsOffline).BuscaCidade(xml.EnderecoImovel.Cidade);

                if (cidade.ID.HasValue)
                    crm.Cidade = new Lookup(cidade.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Cidade do endereço do imóvel não encontrada!";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Cidade do endereço do imóvel não enviado.";
                return crm;
            }
            if (!String.IsNullOrEmpty(xml.EnderecoImovel.Complemento))
                crm.Complemento = xml.EnderecoImovel.Complemento;
            else
                crm.AddNullProperty("Complemento");

            // Estado (UF) - service
            if (!String.IsNullOrEmpty(xml.EnderecoImovel.Estado))
            {
                Model.Estado estado = new Model.Estado(this.Organizacao, this.IsOffline);
                estado = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaEstado(xml.EnderecoImovel.Estado);

                if (estado != null && estado.ID.HasValue)
                    crm.Estado = new Lookup(estado.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Estado não encontrado!";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Estado do endereço do imóvel não enviado.";
                return crm;
            }
            if (!String.IsNullOrEmpty(xml.Matricula))
                crm.Matrícula = xml.Matricula;
            else
                crm.AddNullProperty("Matrícula");

            if (!String.IsNullOrEmpty(xml.Nome))
                crm.Nome = xml.Nome;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Nome do imóvel não enviado.";
                return crm;
            }

            crm.Onus = xml.ValorOnus;

            // País
            if (!String.IsNullOrEmpty(xml.EnderecoImovel.Pais))
            {
                Model.Pais pais = new Model.Pais(this.Organizacao, this.IsOffline);
                pais = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaPais(xml.EnderecoImovel.Pais);

                if (pais != null && pais.ID.HasValue)
                    crm.Pais = new Lookup(pais.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "País não encontrado!";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "País do endereço do imóvel não enviado.";
                return crm;
            }
            crm.TipoImovel = xml.TipoImovel;
            crm.Valor = xml.ValorImovel;
            crm.Status = xml.Situacao;

            // Moeda - service
            if (!String.IsNullOrEmpty(xml.Moeda))
            {
                Model.Moeda moeda = new Model.Moeda(this.Organizacao, this.IsOffline);
                moeda = new Intelbras.CRM2013.Domain.Servicos.MoedaService(this.Organizacao, this.IsOffline).BuscaMoedaPorNome(xml.Moeda);

                if (moeda != null && moeda.ID.HasValue)
                    crm.Moeda = new Lookup(moeda.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Moeda não encontrada!";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Moeda não enviada.";
                return crm;
            }

            crm.NaturezaProprietario = xml.NaturezaProprietarioImovel;

            #endregion

            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(Bens objModel)
        {
            throw new NotImplementedException();
        }          
        #endregion
    }
}
