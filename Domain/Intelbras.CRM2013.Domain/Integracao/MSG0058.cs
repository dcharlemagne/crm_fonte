using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using Pollux = Intelbras.Message.Helper;
using Intelbras.CRM2013.Domain.Exceptions;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0058 : Base, IBase<Intelbras.Message.Helper.MSG0058, Domain.Model.Contato>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private string tipoProprietario;
        private Domain.Model.Usuario usuarioIntegracao;
        #endregion

        public MSG0058(string org, bool isOffline) : base(org, isOffline) { }

        #region trace
        private SDKore.Helper.Trace Trace { get; set; }
        public void DefinirObjetoTrace(SDKore.Helper.Trace trace)
        {
            this.Trace = trace;
        }
        #endregion

        #region Executar

        public string Executar(string mensagem, string numeroMensagem, Usuario usuario)
        {
            var objPollux = CarregarMensagem<Pollux.MSG0058>(mensagem);

            if (objPollux.IdentidadeEmissor == Enum.Sistemas.RetornaSistema(Enum.Sistemas.Sistema.Senior))
            {
                this.AtualizaUsuario(objPollux);
            }
            else
            {
                usuarioIntegracao = usuario;
                var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0058>(mensagem));

                if (!resultadoPersistencia.Sucesso)
                {
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0058R1>(numeroMensagem, retorno);
                }

                try
                {
                    if (objPollux.IdentidadeEmissor == Enum.Sistemas.RetornaSistema(Enum.Sistemas.Sistema.Extranet))
                        objeto.IntegrarNoPlugin = true;

                    ContatoService contatoService = new ContatoService(this.Organizacao, this.IsOffline);
                    objeto = contatoService.Persistir(objeto);
                    if (objeto == null)
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Chave não enviada e existem vários registros com o mesmo documento de identificação Integração não realizada.";
                    }
                    else
                    {

                        RelacionaContatoMarca(this.CarregarMensagem<Pollux.MSG0058>(mensagem), objeto, contatoService);
                        RelacionaContatoAreaAtuacao(this.CarregarMensagem<Pollux.MSG0058>(mensagem), objeto, contatoService);

                        resultadoPersistencia.Sucesso = true;
                        resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                        retorno.Add("CodigoContato", objeto.ID.Value.ToString());
                        if (usuarioIntegracao != null)
                            retorno.Add("Proprietario", usuarioIntegracao.ID.Value.ToString());
                        retorno.Add("TipoProprietario", "systemuser");
                    }

                }
                catch (ChaveIntegracaoContatoException ex)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = ex.Message;
                }
            }
            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0058R1>(numeroMensagem, retorno);

        }

        #endregion

        #region Definir Propriedades

        public Contato DefinirPropriedades(Intelbras.Message.Helper.MSG0058 xml)
        {
            var crm = new Contato(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml
            crm.IntegrarNoPlugin = true;
            if (!String.IsNullOrEmpty(xml.Email))
                crm.Email1 = xml.Email;
            else
                crm.AddNullProperty("Email1");

            if (!String.IsNullOrEmpty(xml.EmailAlternativo))
                crm.Email2 = xml.EmailAlternativo;
            else
                crm.AddNullProperty("Email2");

            if (!String.IsNullOrEmpty(xml.Naturalidade))
                crm.Naturalidade = xml.Naturalidade;

            else
                crm.AddNullProperty("Naturalidade");

            if (!String.IsNullOrEmpty(xml.Nacionalidade))
                crm.Nacionalidade = xml.Nacionalidade;
            else
                crm.AddNullProperty("Nacionalidade");

            if (xml.SuspensaoCredito.HasValue)
                crm.SuspensaoCredito = xml.SuspensaoCredito.Value;
            else
                crm.AddNullProperty("SuspensaoCredito");

            if (xml.Sexo.HasValue)
                crm.Sexo = xml.Sexo.Value;
            else
                crm.AddNullProperty("Sexo");

            if (xml.ContatoNFE.HasValue)
                crm.ContatoNFE = xml.ContatoNFE.Value;
            else
                crm.AddNullProperty("ContatoNFE");

            if (!String.IsNullOrEmpty(xml.RamalFax))
                crm.RamalFax = xml.RamalFax;
            else
                crm.AddNullProperty("RamalFax");

            if (!String.IsNullOrEmpty(xml.Celular))
                crm.TelefoneCelular = xml.Celular;
            else
                crm.AddNullProperty("TelefoneCelular");

            if (!String.IsNullOrEmpty(xml.TelefoneAssistente))
                crm.TelefoneAssistente = xml.TelefoneAssistente;
            else
                crm.AddNullProperty("TelefoneAssistente");

            if (!String.IsNullOrEmpty(xml.CNPJ))
            {
                if(xml.CNPJ.Length == 11)
                {
                    crm.CpfCnpj = Intelbras.CRM2013.Domain.Servicos.Helper.FormatarCpf(xml.CNPJ);
                }
                else
                {
                    crm.CpfCnpj = xml.CNPJ;
                }
            }
            else if (!String.IsNullOrEmpty(xml.CPF))
            {
                if(xml.CPF.Length == 11)
                {
                    crm.CpfCnpj = Intelbras.CRM2013.Domain.Servicos.Helper.FormatarCpf(xml.CPF);
                }
                else
                {
                    crm.CpfCnpj = xml.CPF;
                }
            }
            else
            {
                crm.AddNullProperty("CpfCnpj");
            }

            if (!String.IsNullOrEmpty(xml.TipoObjetoCanal) && !String.IsNullOrEmpty(xml.Canal))
            {
                string tipoObjetoCanal;
                if (xml.TipoObjetoCanal == "account" || xml.TipoObjetoCanal == "contact")
                {
                    tipoObjetoCanal = xml.TipoObjetoCanal;
                    crm.AssociadoA = new Lookup(new Guid(xml.Canal), xml.TipoObjetoCanal);
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "TipoObjetoCanal ou Canal fora do padrão.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("AssociadoA");
            }
            //Pode ser um Representante, que não tenha ligação com nenhum canal
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "TipoObjetoCanal ou Canal não enviado.";
            //    return crm;
            //}

            if (xml.Cargo.HasValue)
                crm.Cargo = xml.Cargo.Value;
            else
                crm.AddNullProperty("Cargo");

            if (xml.LimiteCredito.HasValue)
                crm.LimiteCredito = xml.LimiteCredito.Value;
            else
                crm.AddNullProperty("LimiteCredito");

            if (xml.DataNascimento.HasValue)
                crm.Aniversario = xml.DataNascimento.Value;
            else
                crm.AddNullProperty("Aniversario");

            if (!String.IsNullOrEmpty(xml.TelefoneGerente))
                crm.TelefoneGerente = xml.TelefoneGerente;
            else
                crm.AddNullProperty("TelefoneGerente");

            if (!String.IsNullOrEmpty(xml.RamalTelefoneAlternativo))
                crm.Ramal2 = xml.RamalTelefoneAlternativo;
            else
                crm.AddNullProperty("Ramal2");

            if (!String.IsNullOrEmpty(xml.RG))
                crm.DocIdentidade = xml.RG;
            else
                crm.AddNullProperty("DocIdentidade");

            if (xml.Escolaridade.HasValue && xml.Escolaridade.Value != -1)
                crm.Escolaridade = xml.Escolaridade.Value;
            else
                crm.AddNullProperty("Escolaridade");

            if (!String.IsNullOrEmpty(xml.TelefoneAlternativo))
                crm.TelefoneComercial2 = xml.TelefoneAlternativo;
            else
                crm.AddNullProperty("TelefoneComercial2");

            if (xml.TipoContato.HasValue)
                crm.TipoRelacao = xml.TipoContato.Value;
            else
                crm.AddNullProperty("TipoRelacao");

            if (!String.IsNullOrEmpty(xml.Telefone))
                crm.TelefoneComercial = xml.Telefone;
            else
                crm.AddNullProperty("TelefoneComercial");

            if (!String.IsNullOrEmpty(xml.NomeContato))
                crm.PrimeiroNome = xml.NomeContato;
            if (!String.IsNullOrEmpty(xml.DescricaoContato))
                crm.Descricao = xml.DescricaoContato;
            else
                crm.AddNullProperty("Descricao");

            if (!String.IsNullOrEmpty(xml.NumeroContato))
                crm.NumeroContato = xml.NumeroContato;
            else
                crm.AddNullProperty("NumeroContato");

            if (!String.IsNullOrEmpty(xml.SegundoNome))
                crm.SegundoNome = xml.SegundoNome;
            else
                crm.AddNullProperty("SegundoNome");

            if (!String.IsNullOrEmpty(xml.Ramal))
                crm.Ramal1 = xml.Ramal;
            else
                crm.AddNullProperty("Ramal1");

            if (xml.Funcao.HasValue)
                crm.Funcao = xml.Funcao.Value;
            else
                crm.AddNullProperty("Funcao");

            if (!String.IsNullOrEmpty(xml.OrgaoExpeditor))
                crm.EmissorDocIdentidade = xml.OrgaoExpeditor;
            else
                crm.AddNullProperty("EmissorDocIdentidade");

            if (xml.Cargo.HasValue)
                crm.Cargo = xml.Cargo.Value;
            else
                crm.AddNullProperty("Cargo");

            if (!String.IsNullOrEmpty(xml.NomeGerente))
                crm.Gerente = xml.NomeGerente;
            else
                crm.AddNullProperty("Gerente");

            if (!String.IsNullOrEmpty(xml.NomeFilhos))
                crm.NomeFilhos = xml.NomeFilhos;
            else
                crm.AddNullProperty("NomeFilhos");

            if (!String.IsNullOrEmpty(xml.Fax))
                crm.Fax = xml.Fax;
            else
                crm.AddNullProperty("Fax");

            if (!String.IsNullOrEmpty(xml.NomeAssistente))
                crm.Assistente = xml.NomeAssistente;
            else
                crm.AddNullProperty("Assistente");

            if (!String.IsNullOrEmpty(xml.Saudacao))
                crm.Saudacao = xml.Saudacao;
            else
                crm.AddNullProperty("Saudacao");

            if (xml.CodigoRepresentante.HasValue)
                crm.CodigoRepresentante = xml.CodigoRepresentante.ToString();
            else
                crm.AddNullProperty("CodigoRepresentante");

            if (xml.Area.HasValue)
                crm.Area = xml.Area.Value;
            else
                crm.AddNullProperty("Area");

            if (xml.DataEspecial.HasValue)
                crm.DatasEspeciais = xml.DataEspecial.Value;
            else
                crm.AddNullProperty("DatasEspeciais");

            if (xml.TemFilhos.HasValue)
                crm.TemFilhos = xml.TemFilhos.Value == 2 ? 0 : 1;
            else
                crm.AddNullProperty("TemFilhos");

            if (xml.EstadoCivil.HasValue)
                crm.EstadoCivil = xml.EstadoCivil.Value;
            else
                crm.AddNullProperty("EstadoCivil");

            if (!String.IsNullOrEmpty(xml.Departamento))
                crm.Departamento = xml.Departamento;
            else
                crm.AddNullProperty("Departamento");

            if (xml.PapelCanal.HasValue)
                crm.PapelCanal = xml.PapelCanal.Value;
            else
                crm.AddNullProperty("PapelCanal");

            if (!String.IsNullOrEmpty(xml.DescricaoPapelCanal))
                crm.DescricaoPapelCanal = xml.DescricaoPapelCanal;
            else
                crm.AddNullProperty("DescricaoPapelCanal");

            if (xml.ListaAreaAtuacao != null && xml.ListaAreaAtuacao.Count > 0)
            {
                crm.AreaAtuacaoTexto = "";
                foreach (var areaAtuacao in xml.ListaAreaAtuacao)
                {
                    if (!String.IsNullOrEmpty(areaAtuacao.ToString()))
                    {
                        crm.AreaAtuacaoTexto += areaAtuacao.ToString() + ",";
                    }
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Área de Atuação " + areaAtuacao.ToString() + " não contém um código válido";
                        return null;
                    }
                }
            }

            if (!String.IsNullOrEmpty(xml.DescricaoAreaAtuacao))
                crm.DescricaoAreaAtuacao = xml.DescricaoAreaAtuacao;
            else
                crm.AddNullProperty("DescricaoAreaAtuacao");

            if (!String.IsNullOrEmpty(xml.Sobrenome))
                crm.Sobrenome = xml.Sobrenome;
            else
                crm.AddNullProperty("Sobrenome");

            if (xml.NumeroFilhos.HasValue)
                crm.NumeroFilhos = xml.NumeroFilhos.Value;
            else
                crm.AddNullProperty("NumeroFilhos");

            if (!String.IsNullOrEmpty(xml.CodigoContato))
                crm.ID = new Guid(xml.CodigoContato);

            if (!String.IsNullOrEmpty(xml.Regiao))
                crm.Endereco1Regiaoid = new Lookup(new Guid(xml.Regiao), "");
            else
                crm.AddNullProperty("Endereco1Regiaoid");

            if (!String.IsNullOrEmpty(xml.NomeConjuge))
                crm.NomeConjuge = xml.NomeConjuge;
            else
                crm.AddNullProperty("NomeConjuge");

            if (xml.IntegraIntelbrasPontua != null)
                crm.IntegraIntelbrasPontua = xml.IntegraIntelbrasPontua;
            else
                crm.IntegraIntelbrasPontua = false;

            crm.IntegradoEm = DateTime.Now;
            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            crm.UsuarioIntegracao = xml.LoginUsuario;
            crm.IntegrarNoBarramento = true;

            if (!String.IsNullOrEmpty(xml.IMEI))
                crm.IMEI = xml.IMEI;
            else
                crm.AddNullProperty("IMEI");

            if (xml.AcessoPortal.HasValue)
            {
                crm.AcessoPortal = xml.AcessoPortal;
            }

            if (xml.AcessoAPP.HasValue)
            {
                crm.AcessoAPPISOL = xml.AcessoAPP;
            }

            if (!string.IsNullOrEmpty(xml.Treinamentos))
            {
                crm.Treinamento = xml.Treinamentos;
            }

            if (xml.AcessoSolar.HasValue)
            {
                crm.AcessoSolar = xml.AcessoSolar;
            }

            #region Services

            if (!String.IsNullOrEmpty(xml.Loja))
            {
                crm.Loja = new Lookup(new Guid(xml.Loja), "");
            }
            else
            {
                crm.AddNullProperty("Loja");
            }

            if (xml.CondicaoFrete.HasValue)
                crm.Endereco1CondicoesFrete = xml.CondicaoFrete;
            else
                crm.AddNullProperty("Endereco1CondicoesFrete");

            crm.Status = xml.Situacao;

            if (xml.CodigoCliente.HasValue)
                crm.CodigoRemetente = xml.CodigoCliente.ToString();

            // Moeda
            if (!String.IsNullOrEmpty(xml.Moeda))
            {
                Model.Moeda moeda = new Model.Moeda(this.Organizacao, this.IsOffline);
                moeda = new Intelbras.CRM2013.Domain.Servicos.MoedaService(this.Organizacao, this.IsOffline).BuscaMoedaPorNome(xml.Moeda);

                if (moeda != null && moeda.ID.HasValue)
                    crm.Moeda = new Lookup(moeda.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Moeda não encontrado!";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("Moeda");
            }

            // ListaPreco
            if (!String.IsNullOrEmpty(xml.ListaPreco))
            {
                Model.ListaPreco listaPreco = new Model.ListaPreco(this.Organizacao, this.IsOffline);
                listaPreco = new Intelbras.CRM2013.Domain.Servicos.ListaPrecoService(this.Organizacao, this.IsOffline).BuscaListaPreco(xml.ListaPreco);

                if (listaPreco != null && listaPreco.ID.HasValue)
                    crm.ListaPrecos = new Lookup(listaPreco.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "ListaPreco não encontrado!";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("ListaPrecos");
            }

            if (xml.EnderecoPrincipal != null)
            {
                // Cidade
                if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Cidade))
                {
                    Model.Municipio cidade = new Model.Municipio(this.Organizacao, this.IsOffline);
                    cidade = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaMunicipio(xml.EnderecoPrincipal.Cidade);

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
                    Model.Estado estado = new Model.Estado(this.Organizacao, this.IsOffline);
                    estado = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaEstado(xml.EnderecoPrincipal.Estado);

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
                    Model.Pais pais = new Model.Pais(this.Organizacao, this.IsOffline);
                    pais = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaPais(xml.EnderecoPrincipal.Pais);

                    if (pais != null && pais.ID.HasValue)
                        crm.Endereco1Pais = new Lookup(pais.ID.Value, "");
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "País não encontrado!";
                        return crm;
                    }
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Endereço Principal não enviado!";
                return crm;
            }

            ///TODO: ClientePotencialOriginador - Aguardar Rollout - Modelagem não prevista para esta etapa.

            if (!String.IsNullOrEmpty(xml.ClientePotencialOriginador))
                crm.ClientePotencialOriginador = new Lookup(new Guid(xml.ClientePotencialOriginador), "");
            else
                crm.AddNullProperty("ClientePotencialOriginador");

            #endregion

            #region Bloco Endereco
            ///Bloco Endereco

            if (xml.EnderecoPrincipal != null)
            {
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
                    crm.Endereco1CEP = Intelbras.CRM2013.Domain.Servicos.Helper.FormatarCep(xml.EnderecoPrincipal.CEP);
                if (!String.IsNullOrEmpty(xml.EnderecoPrincipal.Telefone))
                    crm.Endereco1Telefone = xml.EnderecoPrincipal.Telefone;
                else
                    crm.AddNullProperty("Endereco1Telefone");

                if (xml.MetodoEntrega.HasValue)
                    crm.Endereco1MetodoEntrega = xml.MetodoEntrega.Value;
                else
                    crm.AddNullProperty("Endereco1MetodoEntrega");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Endereço Principal não enviado!";
                return crm;
            }

            #endregion

            #endregion

            return crm;
        }

        public Pollux.MSG0058 DefinirPropriedades(Contato objModel)
        {
            #region Endereco

            Pollux.Entities.Endereco enderecoContato = new Pollux.Entities.Endereco();
            enderecoContato.Bairro = objModel.Endereco1Bairro;
            enderecoContato.Numero = objModel.Endereco1Numero;
            if (!String.IsNullOrEmpty(objModel.Endereco1CEP))
                enderecoContato.CEP = objModel.Endereco1CEP.Replace("-", "").PadLeft(8, '0'); ;
            if (objModel.Endereco1Municipioid != null)
            {
                Municipio municipio = new Servicos.MunicipioServices(this.Organizacao, this.IsOffline).ObterPor(objModel.Endereco1Municipioid.Id);
                enderecoContato.Cidade = municipio.ChaveIntegracao;
                enderecoContato.NomeCidade = municipio.Nome;
            }
            enderecoContato.Complemento = objModel.Endereco1Complemento;
            if (objModel.Endereco1Estadoid != null)
            {
                Estado estado = new Servicos.EstadoServices(this.Organizacao, this.IsOffline).BuscaEstadoPorId(objModel.Endereco1Estadoid.Id);
                if (estado != null)
                {
                    enderecoContato.Estado = estado.ChaveIntegracao;
                    enderecoContato.UF = estado.SiglaUF;
                }
                if (objModel.Endereco1Pais == null)
                {
                    objModel.Endereco1Pais = estado.Pais;
                }
            }
            enderecoContato.Logradouro = objModel.Endereco1Rua;
            enderecoContato.NomeEndereco = objModel.Endereco1Nome;
            if (objModel.Endereco1Pais != null)
            {
                Pais pais = new Servicos.PaisServices(this.Organizacao, this.IsOffline).BuscaPais(objModel.Endereco1Pais.Id);
                enderecoContato.NomePais = pais.Nome;
                enderecoContato.Pais = pais.ChaveIntegracao; ;
            }

            enderecoContato.TipoEndereco = objModel.Endereco1TipoEndereco;
            if (!String.IsNullOrEmpty(objModel.Endereco1CaixaPostal))
                enderecoContato.CaixaPostal = objModel.Endereco1CaixaPostal;
            //msg0072.EnderecoPrincipal = endPrincipal;

            #endregion

            #region Msg
            //Monta o identificador de operação do contato
            var idOperacao = objModel.PrimeiroNome.Trim();
            if (objModel.Sobrenome != null)
                idOperacao = idOperacao + " " + objModel.Sobrenome.Trim();
            idOperacao = idOperacao + "-" + objModel.CpfCnpj.Replace("-", "").Replace(".", "").Replace("/", "").Trim();

            Pollux.MSG0058 msg0058 = new Pollux.MSG0058(itb.RetornaSistema(itb.Sistema.CRM), Helper.Truncate(idOperacao, 40));

            if (objModel.Area.HasValue)
                msg0058.Area = objModel.Area;

            if (objModel.AssociadoA != null)
            {
                msg0058.Canal = objModel.AssociadoA.Id.ToString();
                msg0058.TipoObjetoCanal = objModel.AssociadoA.Type;
            }

            if (objModel.Cargo.HasValue)
                msg0058.Cargo = objModel.Cargo;

            msg0058.Celular = objModel.TelefoneCelular;

            if (objModel.ClientePotencialOriginador != null)
                msg0058.ClientePotencialOriginador = objModel.ClientePotencialOriginador.Id.ToString();

            if (!string.IsNullOrEmpty(objModel.CpfCnpj))
            {
                string cnpjCpfObj = objModel.CpfCnpj.Replace("-", "").Replace(".", "").Replace("/", "").Trim();

                if (!String.IsNullOrEmpty(cnpjCpfObj))
                {
                    if (cnpjCpfObj.Length <= 11)
                    {
                        msg0058.CPF = cnpjCpfObj;
                    }
                    else
                    {
                        msg0058.CNPJ = cnpjCpfObj;
                    }
                }
            }

            msg0058.CodigoCliente = objModel.CodigoRemetente != String.Empty ? (int?)Convert.ToInt32(objModel.CodigoRemetente) : null;

            if (objModel.ID != null)
                msg0058.CodigoContato = objModel.ID.Value.ToString();

            msg0058.ContatoNFE = objModel.ContatoNFE;
            if (objModel.DatasEspeciais.HasValue)
                msg0058.DataEspecial = objModel.DatasEspeciais.Value.ToLocalTime();
            else
                msg0058.DataEspecial = null;
            if (objModel.Aniversario.HasValue)
                msg0058.DataNascimento = objModel.Aniversario.Value.ToLocalTime();
            else
                msg0058.DataNascimento = null;
            msg0058.Departamento = objModel.Departamento;
            if (objModel.Cargo.HasValue)
                msg0058.DescricaoCargo = objModel.Cargo.Value.ToString();
            msg0058.DescricaoContato = objModel.Descricao;
            msg0058.Email = objModel.Email1;
            msg0058.EmailAlternativo = objModel.Email2;
            if (enderecoContato != null)
                msg0058.EnderecoPrincipal = enderecoContato;
            if (objModel.Escolaridade.HasValue)
                msg0058.Escolaridade = objModel.Escolaridade.Value;
            msg0058.EstadoCivil = objModel.EstadoCivil;
            msg0058.Fax = objModel.Fax;
            msg0058.Funcao = objModel.Funcao;
            msg0058.LimiteCredito = objModel.LimiteCredito;
            if (objModel.ListaPrecos != null)
                msg0058.ListaPreco = objModel.ListaPrecos.Name;
            if (objModel.Loja != null)
                msg0058.Loja = objModel.Loja.Id.ToString();
            msg0058.MetodoEntrega = objModel.Endereco1MetodoEntrega;
            if (objModel.Moeda != null)
                msg0058.Moeda = objModel.Moeda.Name;
            msg0058.Nacionalidade = objModel.Nacionalidade;
            msg0058.Naturalidade = objModel.Naturalidade;
            msg0058.NomeAssistente = objModel.Assistente;
            msg0058.NomeContato = objModel.PrimeiroNome;
            msg0058.NomeFilhos = objModel.NomeFilhos;
            msg0058.NomeConjuge = objModel.NomeConjuge;
            msg0058.NomeGerente = objModel.Gerente;
            msg0058.NumeroContato = objModel.NumeroContato;
            msg0058.NumeroFilhos = objModel.NumeroFilhos;
            msg0058.OrgaoExpeditor = objModel.EmissorDocIdentidade;
            msg0058.PapelCanal = objModel.PapelCanal;
            msg0058.DescricaoPapelCanal = objModel.DescricaoPapelCanal;

            List<AreaAtuacao> lstAreasAtuacao = new Servicos.AreaAtuacaoService(this.Organizacao, this.IsOffline).ListarPorContato(objModel.Id);
            foreach (var itemAreaAtuacao in lstAreasAtuacao)
            {
                if (itemAreaAtuacao.Codigo.HasValue)
                {
                    msg0058.ListaAreaAtuacao.Add(Convert.ToInt32(itemAreaAtuacao.Codigo));
                }
            }

            msg0058.DescricaoAreaAtuacao = objModel.DescricaoAreaAtuacao;
            msg0058.IMEI = objModel.IMEI;
            if (objModel.AcessoPortal != null)
            {
                msg0058.AcessoPortal = objModel.AcessoPortal;
            }
            if (objModel.AcessoAPPISOL != null)
            {
                msg0058.AcessoAPP = objModel.AcessoAPPISOL;
            }

            msg0058.Treinamentos = objModel.Treinamento;

            List<Marca> lstMarcas = new Servicos.MarcaService(this.Organizacao, this.IsOffline).ListarPorContato(objModel.Id);
            foreach (var itemMarca in lstMarcas)
            {
                msg0058.ListaMarcas.Add(itemMarca.Id.ToString());
            }

            msg0058.Ramal = objModel.Ramal1;
            msg0058.RamalFax = objModel.RamalFax;
            msg0058.RamalTelefoneAlternativo = objModel.Ramal2;
            msg0058.RG = objModel.DocIdentidade;
            msg0058.Saudacao = objModel.Saudacao;
            msg0058.SegundoNome = objModel.SegundoNome;
            msg0058.Sexo = objModel.Sexo;
            if (objModel.Status.HasValue)
                msg0058.Situacao = objModel.Status.Value;
            msg0058.Sobrenome = objModel.Sobrenome;
            msg0058.SuspensaoCredito = objModel.SuspensaoCredito;
            msg0058.Telefone = objModel.TelefoneComercial;
            msg0058.TelefoneAlternativo = objModel.TelefoneComercial2;
            msg0058.TelefoneAssistente = objModel.TelefoneAssistente;
            msg0058.TelefoneGerente = objModel.TelefoneGerente;
            if (objModel.IntegraIntelbrasPontua != null)
                msg0058.IntegraIntelbrasPontua = objModel.IntegraIntelbrasPontua;
            //Aguardando ajuste Pollux
            //TemFilhos = true;
            msg0058.TipoContato = objModel.TipoRelacao;
            msg0058.CondicaoFrete = objModel.Endereco1CondicoesFrete;

            if (objModel.AcessoSolar != null)
                msg0058.AcessoSolar = objModel.AcessoSolar;

            if (objModel.ExibirSite.HasValue)
            {
                msg0058.ExibirSite = objModel.ExibirSite;
            }
            msg0058.CanalVendaAtendimento = objModel.CanaisDeVenda;

            #region Validar Codigo Representante

            if (!String.IsNullOrEmpty(objModel.CodigoRepresentante))
            {
                if (this.ValidarCodigoRepresentante(objModel.CodigoRepresentante, "systemuser"))
                {
                    msg0058.CodigoRepresentante = (int?)Convert.ToInt32(objModel.CodigoRepresentante);
                }
                else
                {
                    throw new ArgumentException("(CRM) Representante com o Código  : " + objModel.CodigoRepresentante + " - não cadastrado no ERP Totvs.");
                }
            }

            #endregion

            #endregion

            return msg0058;
        }

        #endregion

        #region Métodos Auxiliares


        public string Enviar(Contato objModel)
        {

            //Antes de executar o envio, verifica se o contato não é do tipoRelação "KeyAccount"
            //if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Contato.TipoRelacao), objModel.TipoRelacao))
            //{
            string resposta;
            Intelbras.Message.Helper.MSG0058 mensagem = this.DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0058R1 retorno = CarregarMensagem<Pollux.MSG0058R1>(resposta);
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

        private bool ValidarCodigoRepresentante(String codigoRepresentante, String tipoProprietario)
        {
            string resposta = string.Empty;
            int codigo = 0;

            if (Int32.TryParse(codigoRepresentante, out codigo))
            {
                MSG0161 msg161 = new MSG0161(this.Organizacao, this.IsOffline);
                return msg161.ValidarRepresentante(codigo, tipoProprietario, ref resposta);
            }
            else
            {
                throw new ArgumentException("(CRM) Codigo de Representante inválido.");
            }
        }

        private void AtualizaUsuario(Pollux.MSG0058 objPollux)
        {
            var desativado = false;
            try
            {
                if (objPollux.DadosAlatur.Acao == "E")
                {
                    desativado = new UsuarioService(this.Organizacao, this.IsOffline).DesabilitaUsuarioDesligado("intelbras\\" + objPollux.DadosAlatur.LoginAlatur);
                }
                else if (objPollux.DadosAlatur.Acao == "A")
                {
                    desativado = new UsuarioService(this.Organizacao, this.IsOffline).DesabilitaUsuarioTrocaDepartamento("intelbras\\" + objPollux.DadosAlatur.LoginAlatur, objPollux.Departamento);
                }

                if (!desativado)
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Não foi possível desativar o usuário.";
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Usuário desabilitado com sucesso";
                }

            }
            catch (ChaveIntegracaoContatoException ex)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = ex.Message;
            }
        }

        //persiste o relacionamento com Marcas
        private bool RelacionaContatoMarca(Intelbras.Message.Helper.MSG0058 xml, Contato contato, ContatoService contatoService)
        {
            if (xml.IdentidadeEmissor == Enum.Sistemas.RetornaSistema(Enum.Sistemas.Sistema.API))
            {
                if (xml.ListaMarcas != null && xml.ListaMarcas.Count > 0)
                {
                    List<Marca> listaMarcas = new List<Marca>();
                    foreach (var marca in xml.ListaMarcas)
                    {
                        Marca novaMarca = new Servicos.MarcaService(this.Organizacao, this.IsOffline).ObterPor(new Guid(marca));
                        if (novaMarca != null)
                        {
                            listaMarcas.Add(novaMarca);
                        }
                        else
                        {
                            resultadoPersistencia.Sucesso = false;
                            throw new ArgumentException("Fabricante não cadastrado no Crm.");
                        }
                    }
                    try
                    {
                        contatoService.PersistirMarcas(contato, listaMarcas);
                    }
                    catch
                    {
                        throw new ArgumentException("Erro ao salvar relacionamento.");
                    }
                }
            }
            return true;
        }

        //persiste o relacionamento com Area de Atuação
        private bool RelacionaContatoAreaAtuacao(Intelbras.Message.Helper.MSG0058 xml, Contato contato, ContatoService contatoService)
        {
            if (xml.IdentidadeEmissor == Enum.Sistemas.RetornaSistema(Enum.Sistemas.Sistema.API))
            {
                if (xml.ListaAreaAtuacao != null && xml.ListaAreaAtuacao.Count > 0)
                {
                    List<AreaAtuacao> listaAreaAtuacao = new List<AreaAtuacao>();
                    foreach (var area in xml.ListaAreaAtuacao)
                    {
                        AreaAtuacao novaArea = new Servicos.AreaAtuacaoService(this.Organizacao, this.IsOffline).ObterPorCodigo(area);
                        if (novaArea != null)
                        {
                            listaAreaAtuacao.Add(novaArea);

                        }
                        else
                        {
                            resultadoPersistencia.Sucesso = false;
                            throw new ArgumentException("Área de Atuação não cadastrado no Crm.");
                        }
                    }
                    try
                    {
                        contatoService.PersistirAreasAtuacao(contato, listaAreaAtuacao);
                    }
                    catch
                    {
                        throw new ArgumentException("Erro ao salvar relacionamento.");
                    }
                }
            }
            return true;
        }
        #endregion


    }
}