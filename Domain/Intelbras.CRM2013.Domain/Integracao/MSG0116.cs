using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0116 : Base, IBase<Pollux.MSG0116, Model.Tarefa>
    {

        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        #endregion

        #region Construtor

        public MSG0116(string org, bool isOffline)
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
            var xml = this.CarregarMensagem<Pollux.MSG0116>(mensagem);
            retorno.Add("Resultado", resultadoPersistencia);

            var objeto = this.DefinirPropriedades(xml);

            if (!resultadoPersistencia.Sucesso)
            {
                return CriarMensagemRetorno<Pollux.MSG0116R1>(numeroMensagem, retorno);
            }

            List<Tarefa> lstTarefa = new Intelbras.CRM2013.Domain.Servicos.TarefaService(this.Organizacao, this.IsOffline).ListarTarefas(objeto, xml.DataInicial, xml.DataFinal);

            if (lstTarefa == null || lstTarefa.Count == 0)
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Tarefa(s) não encontradas.";
                return CriarMensagemRetorno<Pollux.MSG0116R1>(numeroMensagem, retorno);
            }

            List<Pollux.Entities.Tarefa> lstTarefaPollux = this.GerarRetornoTarefas(lstTarefa);


            if (lstTarefaPollux == null || lstTarefaPollux.Count == 0)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro ao gerar retorno da lista de tarefas.";
                return CriarMensagemRetorno<Pollux.MSG0116R1>(numeroMensagem, retorno);
            }

            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
            retorno.Add("TarefasItens", lstTarefaPollux);
            return CriarMensagemRetorno<Pollux.MSG0116R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public Tarefa DefinirPropriedades(Intelbras.Message.Helper.MSG0116 xml)
        {
            var crm = new Tarefa(this.Organizacao, this.IsOffline);


            string[] tiposObj = new string[] { "account",
                                                "campaign",
                                                "campaignactivity",
                                                "contact",
                                                "contract",
                                                "incident",
                                                "invoice",
                                                "itbc_acessoextranet",
                                                "itbc_acessosextranetcontatos",
                                                "itbc_compdocanal",
                                                "itbc_docsrequeridoscanal",
                                                "itbc_metadetalhadadocanal",
                                                "itbc_metadocliente",
                                                "itbc_metas",
                                                "itbc_politicacomercial",
                                                "itbc_solicitacaodebeneficio",
                                                "itbc_solicitacaodecadastro",
                                                "lead",
                                                "msdyn_postalbum",
                                                "opportunity",
                                                "quote",
                                                "salesorder"};


            if (!String.IsNullOrEmpty(xml.TipoObjeto)
                && !String.IsNullOrEmpty(xml.CodigoObjeto)
                && tiposObj.Contains(xml.TipoObjeto.ToLower().Trim()))
            {
                crm.ReferenteA = new Lookup(new Guid(xml.CodigoObjeto), xml.TipoObjeto);
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "TipoObjeto/CodigoObjeto não enviado.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.TipoAtividade))
            {
                crm.TipoDeAtividade = new Lookup(new Guid(xml.TipoAtividade), "");
            }

            crm.State = xml.Situacao;

            return crm;
        }

        public List<Pollux.Entities.Tarefa> GerarRetornoTarefas(List<Tarefa> lstTarefa)
        {
            List<Pollux.Entities.Tarefa> lstPollux = new List<Pollux.Entities.Tarefa>();
            foreach (Tarefa item in lstTarefa)
            {
                Pollux.Entities.Tarefa tarefa = new Pollux.Entities.Tarefa();

                if (item.Prioridade.HasValue)
                    tarefa.Prioridade = item.Prioridade.Value;

                tarefa.CodigoTarefa = item.ID.ToString();

                if (item.Subcategoria != null)
                    tarefa.SubCategoria = item.Subcategoria;
                else
                    tarefa.SubCategoria = "N/A";

                if (item.ReferenteA != null)
                {
                    tarefa.TipoObjeto = item.ReferenteA.Type;
                    tarefa.CodigoObjeto = item.ReferenteA.Id.ToString();
                    tarefa.NomeObjeto = item.ReferenteA.Name;
                }
                else
                {
                    tarefa.TipoObjeto = "N/A";
                    tarefa.CodigoObjeto = Guid.Empty.ToString();
                    tarefa.NomeObjeto = "N/A";
                }
                if (item.State.HasValue)
                    tarefa.Situacao = item.State.Value;
                else
                    tarefa.Situacao = 0;

                if (item.Assunto != null)
                    tarefa.Assunto = item.Assunto;
                else
                    tarefa.Assunto = "N/A";

                if (!String.IsNullOrEmpty(item.PareceresAnteriores))
                    tarefa.ParecerAnterior = item.PareceresAnteriores;
                else
                    tarefa.ParecerAnterior = "N/A";

                if (item.Resultado.HasValue)
                    tarefa.Resultado = item.Resultado.Value;
                else
                    tarefa.Resultado = 0;
                if (!String.IsNullOrEmpty(tarefa.Categoria))
                    tarefa.Categoria = item.Categoria;
                else
                    tarefa.Categoria = "N/A";

                if (item.DescricaoSolicitacao != null)
                    tarefa.DescricaoSolicitacao = item.DescricaoSolicitacao;
                else
                    tarefa.DescricaoSolicitacao = "N/A";


                if (item.Descricao != null)
                    tarefa.Descricao = item.Descricao;
                else
                    tarefa.Descricao = "N/A";

                if (item.TipoDeAtividade != null)
                {
                    tarefa.CodigoTipoAtividade = item.TipoDeAtividade.Id.ToString();
                    tarefa.NomeTipoAtividade = item.TipoDeAtividade.Name;
                }
                else
                {
                    tarefa.CodigoTipoAtividade = Guid.Empty.ToString();
                    tarefa.NomeTipoAtividade = "N/A";

                }

                if (item.TerminoReal != null)
                    tarefa.DataHoraTerminoReal = item.TerminoReal.Value;

                if (item.Conclusao != null)
                    tarefa.DataHoraTerminoEsperada = item.Conclusao.Value;

                if (item.Duracao.HasValue)
                    tarefa.Duracao = item.Duracao.Value;

                if (item.CriadoEm != null)
                    tarefa.DataHoraCriacao = item.CriadoEm.Value;
                else
                    tarefa.DataHoraCriacao = DateTime.MinValue;

                if (item.ModificadoEm != null)
                    tarefa.DataHoraModificacao = item.ModificadoEm.Value;
                else
                    tarefa.DataHoraModificacao = DateTime.MinValue;

                lstPollux.Add(tarefa);
            }
            return lstPollux;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Tarefa objModel)
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
