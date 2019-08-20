using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.ViewModels;
using Intelbras.CRM2013.Util.CustomException;
using Intelbras.CRM2013.Domain.Enum;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;
using System.Linq;
using System.Text;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0315 : Base, IBase<Message.Helper.MSG0315, Domain.Model.Ocorrencia>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private string tipoProprietario;
        private Domain.Model.Usuario usuarioIntegracao;
        #endregion

        #region Construtor

        public MSG0315(string org, bool isOffline)
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
            usuarioIntegracao = usuario;
            var xml = this.CarregarMensagem<Pollux.MSG0315>(mensagem);

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0315R1>(numeroMensagem, retorno);
            }

            List<Ocorrencia> lstOcorrencia = new Domain.Servicos.OcorrenciaService(this.Organizacao, this.IsOffline).ListarOcorrenciasPorNumeroSerie(xml.NumeroSerieProduto);

            if (lstOcorrencia == null || lstOcorrencia.Count == 0)
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Ocorrência(s) não encontrada(s).";
                return CriarMensagemRetorno<Pollux.MSG0315R1>(numeroMensagem, retorno);
            }

            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração Ocorrida com sucesso.";
            retorno.Add("Ocorrencia", this.GerarListaOcorrencias(lstOcorrencia));
            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0315R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        // public List<Ocorrencia> DefinirPropriedadesLista(Intelbras.Message.Helper.MSG0315 xml)
        // {
        //     List<Ocorrencia> lstOcorrencia = new List<Ocorrencia>();
        //     return lstOcorrencia;
        // }

         public Pollux.MSG0315 DefinirPropriedades(string numeroSerie)
         {
            Pollux.MSG0315 msg0315 = new Pollux.MSG0315(itb.RetornaSistema(itb.Sistema.CRM), Helper.Truncate(numeroSerie, 40));
            msg0315.NumeroSerieProduto = numeroSerie;
            msg0315.NumeroOperacao = numeroSerie;

            return msg0315;
        }
        
        #endregion

        #region Métodos Auxiliares

        public string Enviar(Ocorrencia objModel)
        {
            string retMsg = String.Empty;
            string message = string.Empty;

            if(objModel.ProdutosDoCliente == null)
            {
                throw new ArgumentException("(CRM) Número de série não informado.");
            }

            string numeroSerie = objModel.ProdutosDoCliente;
            Intelbras.Message.Helper.MSG0315 mensagem = this.DefinirPropriedades(numeroSerie);
            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);

            try
            {
                message = mensagem.GenerateMessage(true);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("(CRM) (XSD) " + ex.Message, ex);
            }

            if (integracao.EnviarMensagemBarramento(message, "1", "1", out retMsg))
            {
                Intelbras.Message.Helper.MSG0315R1 retorno = CarregarMensagem<Pollux.MSG0315R1>(retMsg);
                if (retorno.Resultado.Sucesso)
                {
                    return retMsg;
                }
                else
                {
                    throw new ArgumentException("(CRM) " + string.Concat(retorno.Resultado.Mensagem));
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(retMsg);
                throw new ArgumentException("(CRM) " + string.Concat(erro001.GenerateMessage(false)));
            }
            return retMsg;
        }
        
        private string getTipodeOcorrencia(int enumTipoDeOcorrencia)
        {
            int[] enumValues = new int[] { 300000, 300002, 200094, 300012, 300005, 300021, 300020, 200005, 300016, 200001, 300014, 300001, 200090, 200008, 200000, 200004, 200095, 200091, 300009, 1, 2, 3, 200007, 300013, 300008, 300010, 300006, 300003, 300015, 300007, 200092, 200002, 200096, 200093, 300017, 300004, 300011, 300018, 300019, 200006, 200003};
            string[] enumNames = new string[] { "Duvida", "Dúvida*", "Ampliação (SLA)", "Análise de Defeito", "Atendimento Facilitado", "Análise de Sugestão", "Atendimento Remanufatura", "Consultoria", "Descarte de Produto", "Desinstalação", "DOA", "Elogios", "Improcedente (SLA)", "Infraestrutura", "Instalação", "Inventário", "Locação (SLA)", "Manutenção (SLA)" , "Ordem de Serviço", "Padrão 1", "Padrão 2", "Padrão 3", "Portabilidade", "Reclamação Improcedente", "Reclamação/ Atraso no Conserto", "Reclamação/ Falha no Processo", "Reclamação/ Mau Atendimento", "Reclamação/ Mau Funcionamento", "Reclamação/ Mau Funcionamento c/ Solução", "Reclamação/ Procon", "Reconfiguração (SLA)", "Reinstalação", "Remanejamento (SLA)", "Sinistro (SLA)", "Solução para o Cliente", "Sugestão", "Tratativa de Pesquisa", "Tratativa Logística Reversa", "Troca Subsidiada", "Visita Técnica", "Vistoria"};
            var indice = 0;
            var nomeTipo = "";

            foreach (var value in enumValues)
            {
                if(value == enumTipoDeOcorrencia)
                {
                    nomeTipo = enumNames[indice];
                    break;
                }
                indice++;
            }

            return (nomeTipo == "" ? getTipodeOcorrenciaDefault(enumTipoDeOcorrencia) : nomeTipo) ;
        }

        private string getStatusOcorrencia(int statusCode)
        {
            Dictionary<int, string> tiposStatus =  new Dictionary<int, string>();

            tiposStatus.Add(200000, "Aberta");
            tiposStatus.Add(200001, "Andamento");
            tiposStatus.Add(200002, "Pendente");
            tiposStatus.Add(993520009, "Aguardando análise de Orçamento");
            tiposStatus.Add(993520006, "Pendente Operadora");
            tiposStatus.Add(993520007, "Redirecionado ao grupo Telefonia IP");
            tiposStatus.Add(993520008, "Redirecionado ao grupo Voz e Video");
            tiposStatus.Add(200003, "Cancelada");
            tiposStatus.Add(200004, "Fechada");
            tiposStatus.Add(200005, "Aguardando Aprovação");
            tiposStatus.Add(200006, "Aguardando Fechamento");
            tiposStatus.Add(993520001, "Aguardando Visita Técnica");
            tiposStatus.Add(993520002, "Aguardando Confirmação");
            tiposStatus.Add(993520003, "Atendimento Confirmado");
            tiposStatus.Add(993520004, "Atendimento Rejeitado");
            tiposStatus.Add(993520005, "Visita Concluída");
            tiposStatus.Add(993520010, "Fechamento Cobrado");

            try
            {
                return tiposStatus[statusCode];
            }
            catch (System.Exception)
            {
                return "";
            }
        }

        private string getTipodeOcorrenciaDefault(int enumTipoDeOcorrencia)
        {
            var enumNames = Enum.TipoDeOcorrencia.GetNames(typeof(TipoDeOcorrencia)).ToArray();
            var enumValues = Enum.TipoDeOcorrencia.GetValues(typeof(TipoDeOcorrencia)).Cast<TipoDeOcorrencia>().ToList().Select(e => (int)e);
            var indice = 0;
            var nomeTipo = "";

            foreach (var value in enumValues)
            {
                if(value == enumTipoDeOcorrencia)
                {
                    nomeTipo = enumNames[indice];
                    break;
                }
                indice++;
            }

            return nomeTipo;
        }

        private List<Pollux.Entities.Ocorrencia> GerarListaOcorrencias(List<Ocorrencia> lstOcorrencia)
        {
            List<Pollux.Entities.Ocorrencia> lstOcorrenciaItem = new List<Pollux.Entities.Ocorrencia>();

            foreach (var item in lstOcorrencia)
            {
                Pollux.Entities.Ocorrencia ocorrencia = new Pollux.Entities.Ocorrencia();

                ocorrencia.TituloOcorrencia = item.Nome;
                ocorrencia.NumeroOcorrencia = item.Numero;
                ocorrencia.DescricaoStatusOcorrencia =  this.getStatusOcorrencia((int)item.StatusCode);//item.StatusDaOcorrencia.ToString();
                ocorrencia.DescricaoTipoOcorrencia = this.getTipodeOcorrencia((int)item.TipoDeOcorrencia);
                ocorrencia.DataHoraAbertura = item.DataOrigem;
                ocorrencia.DataHoraConclusao = item.DataDeConclusao;
                ocorrencia.ResumoOcorrencia = item.ResumoDaOcorrencia;
                ocorrencia.NomeProprietario = "";
                ocorrencia.NomeCliente = "";

                if (item.ClienteId != null)
                {
                    Model.Conta conta = new Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(item.ClienteId.Id);
                    if (conta != null)
                    {
                        ocorrencia.NomeCliente = conta.RazaoSocial;
                    }
                }

                Domain.Model.Usuario proprietario = new Domain.Servicos.UsuarioService(this.Organizacao, this.IsOffline).BuscarProprietario("incident", "incidentid", item.ID.Value);
                if(proprietario != null)
                {
                    ocorrencia.NomeProprietario = proprietario.NomeCompleto;
                }

                lstOcorrenciaItem.Add(ocorrencia);
            }

            return lstOcorrenciaItem;
        }

        public Ocorrencia DefinirPropriedades(Pollux.MSG0315 legado)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}