using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using SDKore.Helper;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class IntegracaoSellout : Base
    {
        #region Construtor

        public IntegracaoSellout(string org, bool isOffline) : base(org, isOffline) { }

        #endregion

        public Revenda CriarRevendaSellout(string xml)
        {
            Revenda revenda = DeserializeRevenda(xml);
            Conta conta = this.SelloutParseConta(revenda);

            var msg0164 = new MSG0164(this.Organizacao, this.IsOffline);
            conta = new Servicos.ContaService(this.Organizacao, this.IsOffline).CriarRevendaSellout(conta, msg0164);

            return SelloutParseRevenda(conta);
        }

        private Revenda DeserializeRevenda(string XMLString)
        {
            XmlSerializer oXmlSerializer = new XmlSerializer(typeof(Revenda));
            return (Revenda)oXmlSerializer.Deserialize(new StringReader(XMLString));
        }

        private string SerializerRevenda(Revenda revenda)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(revenda.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, revenda);
                return textWriter.ToString();
            }
        }

        private Conta SelloutParseConta(Revenda revenda)
        {
            Conta conta = new Conta(this.Organizacao, this.IsOffline);
            conta.RazaoSocial = revenda.RazaoSocial;
            conta.NomeFantasia = revenda.RazaoSocial.Truncate(60);
            conta.CpfCnpj = revenda.CpfCnpj;
            conta.Telefone = revenda.TelefoneCliente;
            conta.IntegrarNoPlugin = true;
            conta.QuantasFiliais = 0;
            conta.PossuiFiliais = (int)Enum.Conta.PossuiFiliais.Nao;
            conta.RecebeNFE = false;
            conta.StatusIntegracaoSefaz = (int)Enum.Conta.StatusIntegracaoSefaz.NaoValidado;
            conta.ParticipantePrograma = (int)Enum.Conta.ParticipaDoPrograma.Nao;
            conta.AssistenciaTecnica = false;
            conta.Status = (int)Enum.Conta.StateCode.Ativo;
            conta.RazaoStatus = (int)Enum.Conta.StatusCode.Ativo;
            conta.TipoConta = (int)Enum.Conta.MatrizOuFilial.Matriz;
            conta.TipoRelacao = (int)Enum.Conta.TipoRelacao.Canal;
            conta.OrigemConta = (int)Enum.Conta.OrigemConta.SellOut;

            if (revenda.TipoPessoaCliente == "FISICA")
            {
                conta.Natureza = 993520003;
                conta.TipoConstituicao = (int)Enum.Conta.TipoConstituicao.Cpf;
                conta.InscricaoEstadual = "ISENTO";
                conta.ContribuinteICMS = false;
            }
            else
            {
                conta.Natureza = 993520000;
                conta.TipoConstituicao = (int)Enum.Conta.TipoConstituicao.Cnpj;
                conta.InscricaoEstadual = revenda.InscricaoEstadual;
            }

            //Controle Interno
            conta.Endereco1Rua1 = revenda.EnderecoCliente;
            conta.Endereco2Rua2 = revenda.EnderecoCliente;

            //Endereço Principal
            conta.TipoEndereco = (int)Enum.Conta.Tipoendereco.Primario;
            conta.Endereco1Rua = revenda.EnderecoCliente;
            conta.Endereco1Numero = revenda.NumeroEnderecoCliente;
            conta.Endereco1CEP = revenda.CepCliente;
            conta.Endereco1Bairro = revenda.BairroCliente;

            //Endereço de Cobrança
            conta.Endereco2TipoEndereco = (int)Enum.Conta.Tipoendereco2.Cobranca;
            conta.Endereco2Rua = revenda.EnderecoCliente;
            conta.Endereco2Numero = revenda.NumeroEnderecoCliente;
            conta.Endereco2Bairro = revenda.BairroCliente;
            conta.Endereco2CEP = revenda.CepCliente;

            //Estado
            if (!string.IsNullOrEmpty(revenda.EstadoCliente))
            {
                Estado estado = new Servicos.EstadoServices(this.Organizacao, this.IsOffline).BuscaEstadoPorSigla(revenda.EstadoCliente);

                if (estado != null)
                {
                    conta.Endereco1Estadoid = new Lookup(estado.ID.Value, "");
                    conta.Endereco2Estadoid = new Lookup(estado.ID.Value, "");

                    if (estado.Pais != null)
                    {
                        conta.Endereco2Pais = new Lookup(estado.Pais.Id, "");
                        conta.Endereco1Pais = new Lookup(estado.Pais.Id, "");
                        conta.Endereco1Pais1 = estado.Pais.Name;
                        conta.Endereco2Pais2 = estado.Pais.Name;
                    }

                    //Controle interno
                    conta.Endereco2Estado = estado.Nome;
                    conta.Endereco1Estado = estado.Nome;

                    //Cidade
                    Municipio municipio = new Servicos.MunicipioServices(this.Organizacao, this.IsOffline).ObterCidadePorEstadoIdNomeCidade(conta.Endereco1Estadoid.Id, revenda.CidadeCliente);
                    if (municipio != null)
                    {
                        //Principal
                        conta.Endereco1Municipioid = new Lookup(municipio.ID.Value, "");
                        //Cobrança
                        conta.Endereco2Municipioid = new Lookup(municipio.ID.Value, "");

                        //Controle Interno
                        //conta.Endereco2Cidade = municipio.Nome;
                        conta.Endereco1Cidade = municipio.Nome;
                    }
                }
            }


            Guid classificacaoId = new Guid(SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.Sellout.Classificacao"));
            conta.Classificacao = new Lookup(classificacaoId, SDKore.Crm.Util.Utility.GetEntityName<Classificacao>());

            Guid subClassificacaoId = new Guid(SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.Sellout.SubClassificacao"));
            conta.Subclassificacao = new Lookup(subClassificacaoId, SDKore.Crm.Util.Utility.GetEntityName<Subclassificacoes>());

            Guid nivelPosVendaId = SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Sellout.NivelPosVenda");
            conta.NivelPosVendas = new Lookup(nivelPosVendaId, SDKore.Crm.Util.Utility.GetEntityName<NivelPosVenda>());
            

            return conta;
        }

        private Revenda SelloutParseRevenda(Conta conta)
        {
            if (conta == null)
                return null;

            Revenda revenda = new Revenda();

            if (conta.ID.HasValue)
            {
                revenda.Idrevendacrm = conta.ID.Value;
            }

            if (conta.Natureza.HasValue)
            {
                revenda.TipoPessoaCliente = conta.Natureza.Value == (int)Domain.Enum.Conta.TipoConstituicao.Cpf ? "FISÍCA" : "JURÍDICA";
            }

            if (conta.Endereco1Estadoid != null)
            {
                revenda.EstadoCliente = conta.Endereco1Estadoid.Name;
            }

            if (conta.Endereco1Municipioid != null)
            {
                revenda.CidadeCliente = conta.Endereco1Municipioid.Name;
            }

            revenda.RazaoSocial = conta.RazaoSocial;
            revenda.InscricaoEstadual = conta.InscricaoEstadual;
            revenda.CpfCnpj = conta.CpfCnpj;
            revenda.TelefoneCliente = conta.Telefone;
            revenda.EnderecoCliente = conta.Endereco1Rua;
            revenda.NumeroEnderecoCliente = conta.Endereco1Numero;
            revenda.CepCliente = conta.Endereco1CEP;
            revenda.BairroCliente = conta.Endereco1Bairro;
            revenda.Statecode = conta.Status.Value;
            revenda.Statuscode = conta.RazaoStatus.Value;

            return revenda;
        }
    }
}