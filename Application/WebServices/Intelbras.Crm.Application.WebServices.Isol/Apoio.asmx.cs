using System;
using System.Web.Services;
using System.Xml;
using System.Diagnostics;
using System.ServiceModel;
using Intelbras.CRM2013.Domain;
using Intelbras.CRM2013.Domain.Model;
using System.Globalization;

namespace Intelbras.CRM2013.Application.WebServices.Isol
{
    [WebService(Namespace = "http://schemas.microsoft.com/crm/2009/WebServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [System.ComponentModel.ToolboxItem(false)]
    public class Apoio : WebServiceBase
    {
        [WebMethod]
        public XmlDocument PesquisarAssuntoPor(string assuntoId, string organizacaoNome)
        {
            try
            {
                Assunto assunto = (new Domain.Servicos.RepositoryService()).Assunto.Retrieve(new Guid(assuntoId));

                base.Mensageiro.AdicionarTopico("Titulo", assunto.Nome);
                base.Mensageiro.AdicionarTopico("Descricao", assunto.Descricao);
                base.Mensageiro.AdicionarTopico("TemAssuntoFilho", assunto.TemFilho);
                base.Mensageiro.AdicionarTopico("EstruturaAssunto", assunto.EstruturaDoAssuntoNome());
            }
            catch (Exception ex)
            {
                //LogService.GravaLog(ex, TipoDeLog.WSVendasApoio, "PesquisarAssuntoPor(string assuntoId, string organizacaoNome);");
                return base.TratarErro("Não foi possível realizar a pesquisa", ex, "PesquisaAssuntoPor(string _assuntoId, string _organizacao)");
            }

            return base.Mensageiro.Mensagem;
        }

        [WebMethod]
        public XmlDocument PesquisarRegionalPor(string cidadeId, string organizacaoNome)
        {
            try
            {
                var cidade = new Municipio(nomeOrganizacao, false) { Id = new Guid(cidadeId) };

                if (cidade.Regional != null)
                {
                    base.Mensageiro.AdicionarTopico("RegionalId", cidade.Regional.Id);
                    base.Mensageiro.AdicionarTopico("RegionalNome", cidade.Regional.Nome);
                }
                else
                {
                    base.Mensageiro.AdicionarTopico("Sucesso", false);
                    base.Mensageiro.AdicionarTopico("MensagemDeErro", "Não foi possível encontrar a regional");
                }

            }
            catch (Exception erro)
            {
                EventLog.WriteEntry("CRM Application", "Erro ao tentar executar o WebService Isol/Apoio.asmx. ***Menssage: " + erro.Message + ". ***StrackTrace: " + erro.StackTrace.ToString());
                return base.TratarErro("Não foi possível realizar a pesquisa pelo seguinte motivo: " + erro.Message + " - " + erro.StackTrace.ToString(),
                                        erro, "PesquisarRegionalPor(string _cidadeId, string _organizacaoNome)");
            }

            return base.Mensageiro.Mensagem;
        }

        [WebMethod]
        public XmlDocument PesquisarContatoPor(string contatoId)
        {
            try
            {
                var contato = (new Domain.Servicos.RepositoryService()).Contato.Retrieve(new Guid(contatoId));
                

                if (!string.IsNullOrEmpty(contato.Nome)) base.Mensageiro.AdicionarTopico("Nome", contato.Nome);
                if (!string.IsNullOrEmpty(contato.CpfCnpj)) base.Mensageiro.AdicionarTopico("CPF", contato.CpfCnpj);
                if (!string.IsNullOrEmpty(contato.TelefoneComercial)) base.Mensageiro.AdicionarTopico("Telefone", contato.TelefoneComercial);
                if (!string.IsNullOrEmpty(contato.Email1)) base.Mensageiro.AdicionarTopico("Email", contato.Email1);
                base.Mensageiro.AdicionarTopico("TipoRelacaoNome", contato.TipoRelacao);

                if (contato.AssociadoA != null) base.Mensageiro.AdicionarTopico("ClienteId", contato.AssociadoA.Id);

                if (!string.IsNullOrEmpty(contato.Endereco1Rua)) base.Mensageiro.AdicionarTopico("Endereco", contato.Endereco1Rua);
                if (!string.IsNullOrEmpty(contato.Endereco1Numero)) base.Mensageiro.AdicionarTopico("NumeroEndereco", contato.Endereco1Numero);
                if (contato.Endereco1Municipio != null) base.Mensageiro.AdicionarTopico("CidadeId", contato.Endereco1Municipioid);
                if (contato.Endereco1Estado != null) base.Mensageiro.AdicionarTopico("UfId", contato.Endereco1Estadoid);
                if (contato.Endereco1Pais != null) base.Mensageiro.AdicionarTopico("PaisId", contato.Endereco1Pais);
            }
            catch (Exception ex)
            {
                //LogService.GravaLog(ex, TipoDeLog.WSVendasApoio, "PesquisarContatoPor(string _contatoId, string _organizacaoNome)");
                return base.TratarErro("Não foi possível realizar a pesquisa pelo seguinte motivo: " + ex.Message,
                                        ex,
                                        "PesquisarContatoPor(string _contatoId, string _organizacaoNome)");
            }

            return base.Mensageiro.Mensagem;
        }

        [WebMethod]
        public XmlDocument PesquisarClientePor(string clienteId)
        {
            try
            {
                var cliente = (new Domain.Servicos.RepositoryService()).Conta.Retrieve(new Guid(clienteId));

                if (!string.IsNullOrEmpty(cliente.CpfCnpj))
                    base.Mensageiro.AdicionarTopico("Cnpj", cliente.CpfCnpj);
            }
            catch (Exception erro)
            {
                //LogService.GravaLog(erro, TipoDeLog.WSVendasApoio);
            }

            return base.Mensageiro.Mensagem;
        }

        [WebMethod]
        public XmlDocument PesquisarLocalidadePor(string cep, string organizacaoNome)
        {
            try
            {
                if (String.IsNullOrEmpty(organizacaoNome)) return base.TratarErro("A organização não pode ser nula.", null, "PesquisarLocalidadePor(string cep, string organizacaoNome)");
                if (String.IsNullOrEmpty(cep)) return base.TratarErro("O cep não pode ser nulo", null, "PesquisarLocalidadePor(string cep, string organizacaoNome)");
                Localidade localidade = (new Domain.Servicos.RepositoryService()).Localidade.PesquisarEnderecoPor(cep);

                if (localidade != null)
                {
                    // Cidade 
                    if (localidade.Cidade == null)
                        return base.TratarErro("Não existe código ibge para a cidade encontrada. \nAtualize no cadastro de cidade!", new Exception("Cidade não foi encontrada"), "PesquisarLocalidadePor(string cep, string organizacaoNome)");

                    base.Mensageiro.AdicionarTopico("CidadeId", localidade.Cidade.Id);
                    base.Mensageiro.AdicionarTopico("CidadeNome", localidade.Cidade.Nome);

                    // Regional
                    if (localidade.Cidade.Regional != null)
                    {
                        base.Mensageiro.AdicionarTopico("RegionalId", localidade.Cidade.Regional.Id);
                        base.Mensageiro.AdicionarTopico("RegionalNome", localidade.Cidade.Regional.Nome);
                    }

                    // UF
                    if (localidade.Uf != null)
                    {
                        base.Mensageiro.AdicionarTopico("UfId", localidade.Uf.Id);
                        base.Mensageiro.AdicionarTopico("UfNome", localidade.Uf.Nome);
                    }

                    // Pais
                    if (localidade.Pais != null)
                    {
                        base.Mensageiro.AdicionarTopico("PaisId", localidade.Pais.Id);
                        base.Mensageiro.AdicionarTopico("PaisNome", localidade.Pais.Nome);
                    }

                    base.Mensageiro.AdicionarTopico("Logradouro", localidade.Logradouro);
                    base.Mensageiro.AdicionarTopico("Bairro", localidade.Bairro);

                    base.Mensageiro.AdicionarTopico("Sucesso", true);
                }
                else
                {
                    base.Mensageiro.AdicionarTopico("Sucesso", false);
                    return base.TratarErro("O Cep não foi encontrado", null, "PesquisarLocalidadePor(string cep, string organizacaoNome)");
                }
            }
            catch (Exception ex)
            {
                //LogService.GravaLog(ex, TipoDeLog.WSVendasApoio, "PesquisarLocalidadePor(string cep, string organizacaoNome)");
                return base.TratarErro("Não foi possível realizar a pesquisa", ex, "PesquisarLocalidadePor(string cep, string organizacaoNome)");
            }

            return base.Mensageiro.Mensagem;
        }

        //[WebMethod]
        //public XmlDocument GetValues(string entidade, string campoPesquisado, string campoRetornado, string valorCampoPesquisado, string organizacaoNome)
        //{
        //    try
        //    {
        //        if (String.IsNullOrEmpty(organizacaoNome)) return base.TratarErro("A organização não pode ser nula.", null, "GetValues(string entidade, string campoPesquisado, string campoRetornado, string valorCampoPesquisado, string organizacaoNome)");
        //        if (String.IsNullOrEmpty(entidade)) return base.TratarErro("A entidade não pode ser nula", null, "GetValues(string entidade, string campoPesquisado, string campoRetornado, string valorCampoPesquisado, string organizacaoNome)");
        //        if (String.IsNullOrEmpty(entidade)) return base.TratarErro("O campo pesquisado não pode ser nulo", null, "GetValues(string entidade, string campoPesquisado, string campoRetornado, string valorCampoPesquisado, string organizacaoNome)");
        //        if (String.IsNullOrEmpty(entidade)) return base.TratarErro("O campo retornado não pode ser nulo", null, "GetValues(string entidade, string campoPesquisado, string campoRetornado, string valorCampoPesquisado, string organizacaoNome)");
        //        if (String.IsNullOrEmpty(entidade)) return base.TratarErro("O valor do campo pesquisado não pode ser nulo", null, "GetValues(string entidade, string campoPesquisado, string campoRetornado, string valorCampoPesquisado, string organizacaoNome)");
               
        //        base.Mensageiro.AdicionarTopico("Resultado", entidadeRepository.GetValue(entidade, campoPesquisado, campoRetornado, valorCampoPesquisado));
        //        base.Mensageiro.AdicionarTopico("Sucesso", true);
        //    }
        //    catch (Exception ex)
        //    {
        //        //LogService.GravaLog(ex, TipoDeLog.WSVendasApoio, "GetValues(string entidade, string campoPesquisado, string campoRetornado, string valorCampoPesquisado, string organizacaoNome)");
        //        //return base.TratarErro("Não foi possível realizar a pesquisa", ex, "GetValues(string entidade, string campoPesquisado, string campoRetornado, string valorCampoPesquisado, string organizacaoNome)");
        //    }

        //    return base.Mensageiro.Mensagem;
        //}

        #region Ocorrencia
        // 13-11-2012
        //[WebMethod]
        //public void AtualizarClassificacaoAssunto(string ocorrenciaId)
        //{
        //    var todasAsOcorrencias = RepositoryFactory.GetRepository<IOcorrenciaRepository>();
        //    todasAsOcorrencias.Organizacao = organizacao;
        //    var ocorrencia = todasAsOcorrencias.Retrieve(new Guid(ocorrenciaId));
        //    ocorrencia.AtualizarClassificacaoAssunto(ocorrencia);
        //}

        //[WebMethod]
        //public void AtualizarClassificacaoAssuntoPorIntervalo(string pDataInicial, string pDataFinal, string pUnidadeDeNegocioId)
        //{
        //    Ocorrencia ocorrencia = new Ocorrencia(organizacao);
        //    UnidadeDeNegocio un = new UnidadeDeNegocio(organizacao) { Id = new Guid(pUnidadeDeNegocioId) };
        //    DateTime dataInicial = DateTime.Parse(pDataInicial);
        //    DateTime dataFinal = DateTime.Parse(pDataFinal);

        //    ocorrencia.AtualizarClassificacaoAssuntoPorIntervalo(dataInicial, dataFinal, un);
        //}
        [WebMethod]
        public XmlDocument PesquisarOrcamentoPor(string ocorrenciaId)
        {
            try
            {
                var ocorrencia = (new Domain.Servicos.RepositoryService()).Ocorrencia.Retrieve(new Guid(ocorrenciaId));

                if (ocorrencia.LimiteOrcamento.HasValue)
                    base.Mensageiro.AdicionarTopico("Limite", ocorrencia.LimiteOrcamento.Value.ToString("#,0.00", new CultureInfo("pt-BR")));

                if (!string.IsNullOrEmpty(ocorrencia.ObsevacaoOrcamento))
                    base.Mensageiro.AdicionarTopico("Observacao", ocorrencia.ObsevacaoOrcamento);

                if (!string.IsNullOrEmpty(ocorrencia.EmpresaExecutante.ObservacoesServico))
                    base.Mensageiro.AdicionarTopico("ObservacoesServicos", ocorrencia.EmpresaExecutante.ObservacoesServico);
            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível realizar a pesquisa", erro, "PesquisarOrcamentoPor(string ocorrenciaId)");
            }

            return base.Mensageiro.Mensagem;
        }
        #endregion

    }
}
