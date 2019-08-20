using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using Pollux = Intelbras.Message.Helper;


namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0192 : Base, IBase<Pollux.MSG0192, Domain.Model.Contato>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private Domain.Model.Usuario usuarioIntegracao;
        #endregion

        public MSG0192(string org, bool isOffline) : base(org, isOffline) { }
        
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0192>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0192R1>(numeroMensagem, retorno);
            }

            objeto = new ContatoService(this.Organizacao, this.IsOffline).Persistir(objeto, objeto.CodigoRepresentante);
            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Registro não encontrado!";
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            }

            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0192R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public Contato DefinirPropriedades(Pollux.MSG0192 xml)
        {
            var crm = new Contato(this.Organizacao, this.IsOffline);

            #region Propriedades Xml->Crm

            if (xml.EnderecoPrincipal == null)
            {
                resultadoPersistencia.Sucesso  = false;
                resultadoPersistencia.Mensagem = "Endereço Principal não enviado!";
                return crm;
            }

            crm.IntegrarNoPlugin = true;

            crm.CodigoRepresentante = xml.CodigoRepresentante.ToString();
            crm.NomeCompleto = xml.NomeRepresentante;
            if (xml.NomeRepresentante.Length >= 40)
            {
                crm.PrimeiroNome = xml.NomeRepresentante.Substring(0, 40);
            }
            else
            {
                crm.PrimeiroNome = xml.NomeRepresentante;
            }
            crm.SegundoNome = xml.NomeAbreviado;
            crm.Natureza = xml.Natureza;
            crm.Status = xml.Situacao;
            crm.TipoRelacao  = (int)Enum.Contato.TipoRelacao.KeyAccount;
            crm.Email2 = xml.Email;
            if (!String.IsNullOrEmpty(xml.Site))
                crm.WebSite = xml.Site;
            else
                crm.AddNullProperty("WebSite");
            
            crm.IntegradoEm = DateTime.Now;
            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            crm.UsuarioIntegracao = xml.LoginUsuario;

            #region Bloco Endereco
            ///Bloco Endereco
            // Cidade
            if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Cidade))
            {
                Municipio cidade = new Municipio(this.Organizacao, this.IsOffline);
                cidade = new EnderecoServices(this.Organizacao, this.IsOffline).BuscaMunicipio(xml.EnderecoPrincipal.Cidade);

                if (cidade != null && cidade.ID.HasValue)
                {
                    crm.Endereco1Municipioid = new Lookup(cidade.ID.Value, "");
                }

                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Cidade não encontrada!";
                    return crm;
                }
            }

            // Estado (UF)
            if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Estado))
            {
                Estado estado = new Estado(this.Organizacao, this.IsOffline);
                estado = new EnderecoServices(this.Organizacao, this.IsOffline).BuscaEstado(xml.EnderecoPrincipal.Estado);

                if (estado != null && estado.ID.HasValue)
                    crm.Endereco1Estadoid = new Lookup(estado.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Estado não encontrado!";
                    return crm;
                }
            }

            // País
            if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Pais))
            {
                Pais pais = new Pais(this.Organizacao, this.IsOffline);
                pais = new EnderecoServices(this.Organizacao, this.IsOffline).BuscaPais(xml.EnderecoPrincipal.Pais);

                if (pais != null && pais.ID.HasValue)
                    crm.Endereco1Pais = new Lookup(pais.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "País não encontrado!";
                    return crm;
                }
            }
            
            if (xml.EnderecoPrincipal.TipoEndereco.HasValue)
                crm.Endereco1TipoEndereco = xml.EnderecoPrincipal.TipoEndereco.Value;
            else
                crm.AddNullProperty("Endereco1TipoEndereco");

            if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.NomeEndereco))
                crm.Endereco1Nome = xml.EnderecoPrincipal.NomeEndereco;
            else
                crm.AddNullProperty("Endereco1Nome");

            if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Logradouro))
                crm.Endereco1Rua = xml.EnderecoPrincipal.Logradouro;
            if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Numero))
                crm.Endereco1Numero = xml.EnderecoPrincipal.Numero;
            if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Complemento))
                crm.Endereco1Complemento = xml.EnderecoPrincipal.Complemento;
            else
                crm.AddNullProperty("Endereco1Complemento");

            if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Bairro))
                crm.Endereco1Bairro = xml.EnderecoPrincipal.Bairro;
            if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.CEP))
                crm.Endereco1CEP = Helper.FormatarCep(xml.EnderecoPrincipal.CEP);
            if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Telefone))
            {
                crm.Endereco1Telefone = xml.EnderecoPrincipal.Telefone;
                // para garantir que só será atualizado caso o usuário não tenha preenchido pelo formulário do CRM
                if (crm.TelefoneComercial == null)
                {
                    crm.TelefoneComercial = xml.EnderecoPrincipal.Telefone;
                }
            } 
            else
            {
                crm.AddNullProperty("Endereco1Telefone");
            }
                
            #endregion

            #endregion

            return crm;
        }

        #endregion

        #region Métodos Auxiliares
        public string Enviar(Contato objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}