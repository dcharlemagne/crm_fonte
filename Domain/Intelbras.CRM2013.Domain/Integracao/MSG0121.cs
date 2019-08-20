using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0121 : Base, IBase<Pollux.MSG0121, Model.Tarefa>
    {

        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        #endregion

        #region Construtor

        public MSG0121(string org, bool isOffline)
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
            var xml = this.CarregarMensagem<Pollux.MSG0121>(mensagem);

            var objeto = this.DefinirPropriedades(xml);

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0121R1>(numeroMensagem, retorno);
            }

            objeto = new Intelbras.CRM2013.Domain.Servicos.TarefaService(this.Organizacao, this.IsOffline).BuscaTarefa(objeto.ID.Value);

            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Tarefa não encontradas.";
                return CriarMensagemRetorno<Pollux.MSG0121R1>(numeroMensagem, retorno);
            }

            Pollux.Entities.ObterTarefa tarefaPollux = this.GerarRetornoTarefas(objeto);


            if (tarefaPollux == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro de persistência..";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0121R1>(numeroMensagem, retorno);
            }

            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
            retorno.Add("Resultado", resultadoPersistencia);
            retorno.Add("Tarefa", tarefaPollux);
            return CriarMensagemRetorno<Pollux.MSG0121R1>(numeroMensagem, retorno);

        }
        #endregion

        #region Definir Propriedades

        public Tarefa DefinirPropriedades(Intelbras.Message.Helper.MSG0121 xml)
        {
            var crm = new Tarefa(this.Organizacao, this.IsOffline);
            if (!String.IsNullOrEmpty(xml.CodigoTarefa) && xml.CodigoTarefa.Length == 36)
                crm.ID = new Guid(xml.CodigoTarefa);
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Codigo Tarefa não enviado, ou fora do padrão.";
                return crm;

            }

            return crm;
        }

        public Pollux.Entities.ObterTarefa GerarRetornoTarefas(Tarefa tarefaCrm)
        {

            Pollux.Entities.ObterTarefa tarefa = new Pollux.Entities.ObterTarefa();

            if (tarefaCrm.Prioridade.HasValue)
                tarefa.Prioridade = tarefaCrm.Prioridade.Value;
            else
                tarefa.Prioridade = 0;

            if (tarefaCrm.Subcategoria != null)
                tarefa.SubCategoria = tarefaCrm.Subcategoria;
            else
                tarefa.SubCategoria = "N/A";
            if (tarefaCrm.ReferenteA != null)
            {
                tarefa.TipoObjeto = tarefaCrm.ReferenteA.Type;
                tarefa.CodigoObjeto = tarefaCrm.ReferenteA.Id.ToString();
                tarefa.NomeObjeto = tarefaCrm.ReferenteA.Name;
            }
            else
            {
                tarefa.TipoObjeto = "N/A";
                tarefa.CodigoObjeto = Guid.Empty.ToString();
                tarefa.NomeObjeto = "N/A";

            }
            if (tarefaCrm.State.HasValue)
                tarefa.Situacao = tarefaCrm.State.Value;
            else
                tarefa.Situacao = 0;
            if (tarefaCrm.Assunto != null)
                tarefa.Assunto = tarefaCrm.Assunto;
            else
                tarefa.Assunto = "N/A";

            if (!String.IsNullOrEmpty(tarefaCrm.PareceresAnteriores))
                tarefa.ParecerAnterior = tarefaCrm.PareceresAnteriores;
            else
                tarefa.ParecerAnterior = "N/A";

            if (tarefaCrm.Resultado.HasValue)
                tarefa.Resultado = tarefaCrm.Resultado.Value;
            else
                tarefa.Resultado = 0;

            if (!String.IsNullOrEmpty(tarefaCrm.Categoria))
                tarefa.Categoria = tarefaCrm.Categoria;
            else
                tarefa.Categoria = "N/A";

            if (tarefaCrm.DescricaoSolicitacao != null)
                tarefa.DescricaoSolicitacao = tarefaCrm.DescricaoSolicitacao;
            else
                tarefa.DescricaoSolicitacao = "N/A";
            if (tarefaCrm.Descricao != null)
                tarefa.Descricao = tarefaCrm.Descricao;
            else
                tarefa.Descricao = "N/A";
            if (tarefaCrm.TipoDeAtividade != null)
            {
                tarefa.CodigoTipoAtividade = tarefaCrm.TipoDeAtividade.Id.ToString();
                tarefa.NomeTipoAtividade = tarefaCrm.TipoDeAtividade.Name;
            }
            else 
            {
                tarefa.CodigoTipoAtividade = Guid.Empty.ToString();
                tarefa.NomeTipoAtividade = "N/A";
            }
            if (tarefaCrm.TerminoReal.HasValue)
                tarefa.DataHoraTerminoReal = tarefaCrm.TerminoReal.Value;

            if (tarefaCrm.Conclusao.HasValue)
                tarefa.DataHoraTerminoEsperada = tarefaCrm.Conclusao.Value;
            if (tarefaCrm.Duracao.HasValue)
                tarefa.Duracao = tarefaCrm.Duracao.Value;
            if (tarefaCrm.CriadoEm.HasValue)
                tarefa.DataHoraCriacao = tarefaCrm.CriadoEm.Value;
            if (tarefaCrm.ModificadoEm.HasValue)
                tarefa.DataHoraModificacao = tarefaCrm.ModificadoEm.Value;


            return tarefa;
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
