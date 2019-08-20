using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using Pollux = Intelbras.Message.Helper;


namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0278 : Base, IBase<Message.Helper.MSG0278, Domain.Model.Ocorrencia>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0278(string org, bool isOffline) : base(org, isOffline)
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
            var xml = this.CarregarMensagem<Pollux.MSG0278>(mensagem);

            List<Intelbras.Message.Helper.Entities.OcorrenciaItem> lstOcorrenciaItem = new List<Pollux.Entities.OcorrenciaItem>();

            if (!xml.DataInicial.HasValue || !xml.DataFinal.HasValue)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "É necessário informar os 2 critérios de busca para a consulta.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0278R1>(numeroMensagem, retorno);
            }

            List<Ocorrencia> lstOcorrencias = new Servicos.OcorrenciaService(this.Organizacao, this.IsOffline).ListarOcorrenciasPorDataCriacao(xml.DataInicial, xml.DataFinal);

            #region Lista

            if (lstOcorrencias != null && lstOcorrencias.Count > 0)
            {
                foreach (Ocorrencia crmItem in lstOcorrencias)
                {
                    Pollux.Entities.OcorrenciaItem objPollux = new Pollux.Entities.OcorrenciaItem();

                    //Atualizar o campo que atualiza o portal operações e suporte quando a flag estiver setado como true
                    if (crmItem.AtualizarOperacoesSuporte == true)
                    {
                        crmItem.AtualizarOperacoesSuporte = false;
                        crmItem.Atualizar();
                    }

                    objPollux.CodigoOcorrencia = crmItem.ID.Value.ToString();
                    if (!string.IsNullOrEmpty(crmItem.Numero))
                    {
                        objPollux.NumeroOcorrencia = crmItem.Numero;
                    }
                    if (crmItem.RazaoStatus.HasValue)
                    {
                        objPollux.StatusOcorrencia = crmItem.RazaoStatus;
                    }
                    if (crmItem.PrioridadeValue.HasValue)
                    {
                        objPollux.PrioridadeOcorrencia = crmItem.PrioridadeValue;
                    }
                    if (crmItem.TipoDeOcorrencia.HasValue)
                    {
                        objPollux.TipoOcorrencia = crmItem.TipoDeOcorrencia;
                    }
                    if (!string.IsNullOrEmpty(crmItem.DefeitoAlegado))
                    {
                        objPollux.DefeitoAlegado = crmItem.DefeitoAlegado;
                    }
                    if (!string.IsNullOrEmpty(crmItem.AtividadeExecutada))
                    {
                        objPollux.AtividadeExecutada = crmItem.AtividadeExecutada;
                    }
                    if (!string.IsNullOrEmpty(crmItem.Anexo))
                    {
                        objPollux.Observacao = crmItem.Anexo;
                    }
                    if (crmItem.DataPrevistaParaVisita.HasValue)
                    {
                        objPollux.DataHoraPrevistaVisita = crmItem.DataPrevistaParaVisita.Value.ToLocalTime();
                    }
                    if (crmItem.DataInicioTecnico.HasValue)
                    {
                        objPollux.DataHoraChegadaTecnico = crmItem.DataInicioTecnico.Value.ToLocalTime(); 
                    }
                    if (crmItem.DataSaidaTecnico.HasValue)
                    {
                        objPollux.DataHoraSaidaTecnico = crmItem.DataSaidaTecnico.Value.ToLocalTime();
                    }
                    if (crmItem.DataDeConclusao.HasValue)
                    {
                        objPollux.DataHoraConclusao = crmItem.DataDeConclusao.Value.ToLocalTime();
                    }
                    if (crmItem.DataEscalacao.HasValue)
                    {
                        objPollux.DataHoraEscalacao = crmItem.DataEscalacao.Value.ToLocalTime();
                    }
                    if (crmItem.DataOrigem.HasValue)
                    {
                        objPollux.DataHoraAbertura = crmItem.DataOrigem.Value.ToLocalTime();
                    }
                    if (crmItem.ModificadoEm.HasValue)
                    {
                        objPollux.DataHoraModificacao = crmItem.ModificadoEm.Value.ToLocalTime();
                    }
                    if (crmItem.DataSLA.HasValue)
                    {
                        objPollux.TempoSLA = crmItem.DataSLA.Value.ToLocalTime();
                    }
                    if (!string.IsNullOrEmpty(crmItem.OsCliente))
                    {
                        objPollux.OrdemServicoCliente = crmItem.OsCliente;
                    }
                    if (crmItem.SolicitanteId != null)
                    {
                        objPollux.CodigoContato = crmItem.SolicitanteId.Id.ToString();
                    }

                    Usuario proprietario = new Domain.Servicos.UsuarioService(this.Organizacao, this.IsOffline).BuscarProprietario("incident", "incidentid", crmItem.ID.Value);

                    if (proprietario != null)
                    {
                        objPollux.NomeProprietario = proprietario.NomeCompleto;
                    }
                    if (crmItem.ClienteId != null)
                    {
                        objPollux.CodigoConta = crmItem.ClienteId.Id.ToString();
                        Conta conta = new Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(crmItem.ClienteId.Id);
                        if (conta != null)
                        {
                            objPollux.RazaoSocialConta = conta.RazaoSocial;

                            if(conta.CpfCnpj != null)
                            objPollux.CNPJConta = conta.CpfCnpj.Replace("-", "").Replace(".", "").Replace("/", "").Trim();
                        }
                    }
                    if (crmItem.EmpresaExecutanteId != null)
                    {
                        objPollux.CodigoEmpresaExecutante = crmItem.EmpresaExecutanteId.Id.ToString();
                        Conta empresaexecutante = new Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(crmItem.EmpresaExecutanteId.Id);
                        if (empresaexecutante != null)
                        {
                            objPollux.RazaoSocialEmpresaExecutante = crmItem.EmpresaExecutante.RazaoSocial;
                            if (objPollux.CNPJEmpresaExecutante != null)
                                objPollux.CNPJEmpresaExecutante = crmItem.EmpresaExecutante.CpfCnpj.Replace("-", "").Replace(".", "").Replace("/", "").Trim();
                        }
                    }
                    if (crmItem.TecnicoDaVisitaId != null)
                    {
                        objPollux.TecnicoResponsavel = crmItem.TecnicoDaVisitaId.Id.ToString();
                        Contato tecnico = new Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(crmItem.TecnicoDaVisitaId.Id);
                        if (tecnico != null)
                        {
                            objPollux.NomeContato = tecnico.NomeCompleto;
                            if (!string.IsNullOrEmpty(crmItem.TecnicoDaVisita.CpfCnpj))
                            {
                                objPollux.CPF = crmItem.TecnicoDaVisita.CpfCnpj.Replace("-", "").Replace(".", "").Replace("/", "").Trim();
                            }
                            if (!string.IsNullOrEmpty(crmItem.TecnicoDaVisita.IMEI))
                            {
                                objPollux.IMEI = crmItem.TecnicoDaVisita.IMEI;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(crmItem.ContatoVisita))
                    {
                        objPollux.ContatoVisita = crmItem.ContatoVisita;
                    }
                    if (crmItem.MarcaId != null)
                    {
                        objPollux.CodigoMarca = crmItem.MarcaId.Id.ToString();
                        objPollux.NomeMarca = crmItem.MarcaId.Name;
                    }
                    if (crmItem.ModeloId != null)
                    {
                        objPollux.CodigoModelo = crmItem.ModeloId.Id.ToString();
                        objPollux.NomeModelo = crmItem.ModeloId.Name;
                    }

                    if (!string.IsNullOrEmpty(crmItem.Rua))
                    {
                        objPollux.Logradouro = crmItem.Rua;
                    }

                    if (!string.IsNullOrEmpty(crmItem.Bairro))
                    {
                        objPollux.Bairro = crmItem.Bairro;
                    }

                    if (!string.IsNullOrEmpty(crmItem.Cidade))
                    {
                        objPollux.NomeCidade = crmItem.Cidade;
                    }

                    if (!string.IsNullOrEmpty(crmItem.ProdutosDoCliente))
                    {
                        objPollux.NumeroSerieProduto = crmItem.ProdutosDoCliente.Trim();
                    }

                    lstOcorrenciaItem.Add(objPollux);
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0278R1>(numeroMensagem, retorno);
            }

            #endregion
            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";
            retorno.Add("OcorrenciaItens", lstOcorrenciaItem);
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0278R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public Ocorrencia DefinirPropriedades(Intelbras.Message.Helper.MSG0278 xml)
        {
            var crm = new Model.Ocorrencia(this.Organizacao, this.IsOffline);
            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Ocorrencia objModel)
        {
            return String.Empty;
        }

        #endregion
    }
}



