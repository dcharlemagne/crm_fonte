using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0277 : Base, IBase<Message.Helper.MSG0277, Domain.Model.Ocorrencia>
    {
        #region Construtor

        public MSG0277(string org, bool isOffline)
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

            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0277>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0277R1>(numeroMensagem, retorno);
            }

            objeto.Atualizar();

            if (objeto != null)
            {
                resultadoPersistencia.Sucesso = true;
                retorno.Add("Resultado", resultadoPersistencia);
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Ocorreu problema na atualização da ocorrencia.";
            }

            return CriarMensagemRetorno<Pollux.MSG0277R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public Ocorrencia DefinirPropriedades(Intelbras.Message.Helper.MSG0277 xml)
        {
            var crm = new Model.Ocorrencia(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(xml.CodigoOcorrencia))
            {
                Ocorrencia ocorrencia = new Servicos.OcorrenciaService(this.Organizacao, this.IsOffline).BuscaOcorrencia(new Guid(xml.CodigoOcorrencia));
                if (ocorrencia != null)
                {
                    crm.Id = new Guid(xml.CodigoOcorrencia);
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoOcorrencia informado não existe para ser atualizado.";
                    return crm;
                }
                if (ocorrencia.RazaoStatus == (int)Domain.Enum.StatusDaOcorrencia.Cancelada)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Ocorrência " + ocorrencia.Numero + " está cancelada.";
                    return crm;
                }

            }

            if (xml.StatusOcorrencia.HasValue)
            {
                crm.RazaoStatus = xml.StatusOcorrencia;
            }

            if (xml.PrioridadeOcorrencia.HasValue)
            {
                crm.PrioridadeValue = xml.PrioridadeOcorrencia;
            }

            if (xml.TipoOcorrencia.HasValue)
            {
                crm.TipoDeOcorrencia = xml.TipoOcorrencia;
            }

            if (!string.IsNullOrEmpty(xml.DefeitoAlegado))
            {
                crm.DefeitoAlegado = xml.DefeitoAlegado;
            }

            if (!string.IsNullOrEmpty(xml.AtividadeExecutada))
            {
                crm.AtividadeExecutada = xml.AtividadeExecutada;
            }

            if (!string.IsNullOrEmpty(xml.Observacao))
            {
                crm.Anexo = xml.Observacao;
            }

            if (xml.DataHoraPrevistaVisita.HasValue)
            {
                crm.DataPrevistaParaVisita = xml.DataHoraPrevistaVisita;
            }

            if (xml.DataHoraChegadaTecnico.HasValue)
            {
                crm.DataInicioTecnico = xml.DataHoraChegadaTecnico;
            }

            if (xml.DataHoraSaidaTecnico.HasValue)
            {
                crm.DataSaidaTecnico = xml.DataHoraSaidaTecnico;
            }

            if (xml.DataHoraConclusao.HasValue)
            {
                crm.DataDeConclusao = xml.DataHoraConclusao;
            }

            if (xml.DataHoraEscalacao.HasValue)
            {
                crm.DataEscalacao = xml.DataHoraEscalacao;
            }

            if (xml.TempoSLA.HasValue)
            {
                crm.DataSLA = xml.TempoSLA;
            }

            if (!string.IsNullOrEmpty(xml.OrdemServicoCliente))
            {
                crm.OsCliente = xml.OrdemServicoCliente;
            }

            if (!string.IsNullOrEmpty(xml.CodigoContato))
            {
                Contato contato = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(new Guid(xml.CodigoContato));
                if (contato != null)
                {
                    crm.SolicitanteId = new Lookup(contato.ID.Value, "contact");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoContato não encontrado no Crm.";
                    return crm;
                }
            }

            if (!string.IsNullOrEmpty(xml.CodigoConta))
            {
                Conta cliente = new Intelbras.CRM2013.Domain.Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(new Guid(xml.CodigoConta));
                if (cliente != null)
                {
                    crm.ClienteId = new Lookup(cliente.ID.Value, "account");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoConta não encontrado no Crm.";
                    return crm;
                }
            }

            if (!string.IsNullOrEmpty(xml.CodigoEmpresaExecutante))
            {
                Conta empresaexecutante = new Intelbras.CRM2013.Domain.Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(new Guid(xml.CodigoEmpresaExecutante));
                if (empresaexecutante != null)
                {
                    crm.EmpresaExecutanteId = new Lookup(empresaexecutante.ID.Value, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoEmpresaExecutante não encontrado no Crm.";
                    return crm;
                }
            }
            
            if (!string.IsNullOrEmpty(xml.TecnicoResponsavel))
            {
                Contato tecnicoresponsavel = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(new Guid(xml.TecnicoResponsavel));
                if (tecnicoresponsavel != null)
                {
                    //crm.TecnicoResponsavelId = new Lookup(tecnicoresponsavel.ID.Value, ""); Trocado para Técnico da Visita, por solicitação da Silvana - Chamado 133127
                    crm.TecnicoDaVisitaId = new Lookup(tecnicoresponsavel.ID.Value, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "TecnicoResponsavel não encontrado no Crm.";
                    return crm;
                }
            }

            if (!string.IsNullOrEmpty(xml.Logradouro))
            {
                crm.Rua = xml.Logradouro;
            }

            if (!string.IsNullOrEmpty(xml.Bairro))
            {
                crm.Bairro = xml.Bairro;
            }

            if (!string.IsNullOrEmpty(xml.NomeCidade))
            {
                crm.Cidade = xml.NomeCidade;
            }

            if (!string.IsNullOrEmpty(xml.UF))
            {
                crm.Estado = xml.UF;
            }

            if (!string.IsNullOrEmpty(xml.DescricaoFCA))
            {
                crm.DescricaoDaMensagemDeIntegracao = xml.DescricaoFCA;
            }

            if (!string.IsNullOrEmpty(xml.ResumoOcorrencia))
            {
                crm.ResumoDaOcorrencia = xml.ResumoOcorrencia;
            }

            if (!string.IsNullOrEmpty(xml.CausaFinal))
            {
                Causa causa = new Intelbras.CRM2013.Domain.Servicos.CausaService(this.Organizacao, this.IsOffline).ObterPor(new Guid(xml.CausaFinal));
                if (causa != null)
                {
                    crm.CausaFinal = new Lookup(causa.Id, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CausaFinal não encontrada no Crm.";
                    return crm;
                }
            }

            if (!string.IsNullOrEmpty(xml.AcaoFinal))
            {
                Acao acao = new Intelbras.CRM2013.Domain.Servicos.AcaoService(this.Organizacao, this.IsOffline).ObterPor(new Guid(xml.AcaoFinal));
                if (acao != null)
                {
                    crm.AcaoFinal = new Lookup(acao.Id, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "AcaoFinal não encontrada no Crm.";
                    return crm;
                }
            }

            if (xml.StatusOcorrencia == 993520004) // Atendimento Rejeitado
            {
                Conta conta = new Intelbras.CRM2013.Domain.Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(new Guid(xml.CodigoEmpresaExecutante));
                Ocorrencia ocorrencia = new Intelbras.CRM2013.Domain.Servicos.OcorrenciaService(this.Organizacao, this.IsOffline).BuscaOcorrencia(new Guid(xml.CodigoOcorrencia));
                string justificativa = "";
                if (ocorrencia.EmpresaAtendimentoRejeitado == null)
                {
                    if(!string.IsNullOrEmpty(xml.Justificativa))    
                    {
                        justificativa= xml.Justificativa;
                    }   
                    crm.EmpresaAtendimentoRejeitado = conta.RazaoSocial + " - " + justificativa + " - " + DateTime.Now;  
                }
                else
                {
                    if(!string.IsNullOrEmpty(xml.Justificativa))
                    {
                        justificativa = xml.Justificativa;
                    }                    
                    crm.EmpresaAtendimentoRejeitado += "\r\n" + (conta.RazaoSocial + " - " + justificativa + " - " + DateTime.Now); 
                }                
                crm.AddNullProperty("EmpresaExecutanteId");                
            }

            #endregion

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
