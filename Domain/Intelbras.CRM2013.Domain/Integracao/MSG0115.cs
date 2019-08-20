using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0115 : Base, IBase<Pollux.MSG0115, Model.Tarefa>
    {

        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        #endregion

        public MSG0115(string org, bool isOffline) : base(org, isOffline) { }

        #region trace
        private SDKore.Helper.Trace Trace { get; set; }
        public void DefinirObjetoTrace(SDKore.Helper.Trace trace)
        {
            this.Trace = trace;
        }
        #endregion

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            try
            {
                usuarioIntegracao = usuario;
                var xml = this.CarregarMensagem<Pollux.MSG0115>(mensagem);
                string msgRetorno = string.Empty;
                var objeto = this.DefinirPropriedades(xml);
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0115R1>(numeroMensagem, retorno);

                if (!resultadoPersistencia.Sucesso)
                {
                    throw new ArgumentException("(CRM) Identificador da Tarefa não enviado (Ações de Criação não permitidas)!");
                }

                objeto = new Intelbras.CRM2013.Domain.Servicos.TarefaService(base.Organizacao, base.IsOffline).Persistir(objeto, out msgRetorno);

                if (objeto == null && !String.IsNullOrEmpty(msgRetorno))
                {
                    resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = msgRetorno;
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
                }
            }
            catch (ArgumentException ex)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = ex.Message;
            }

            return CriarMensagemRetorno<Pollux.MSG0115R1>(numeroMensagem, retorno);
        }

        public Tarefa DefinirPropriedades(Intelbras.Message.Helper.MSG0115 xml)
        {
            if (string.IsNullOrEmpty(xml.CodigoTarefa))
            {
                throw new ArgumentException("(CRM) Identificador da Tarefa não enviado (Ações de Criação não permitidas)!");
            }

            var crm = new Tarefa(this.Organizacao, this.IsOffline);

            if (xml.Situacao.HasValue)
                crm.State = xml.Situacao.Value;
            else
                crm.AddNullProperty("State");


            if (xml.Resultado.HasValue)
                crm.Resultado = xml.Resultado.Value;
            else
                crm.AddNullProperty("Resultado");

            if (!String.IsNullOrEmpty(xml.Descricao))
            crm.Descricao = xml.Descricao;
            else
                crm.AddNullProperty("Descricao");

            if (!String.IsNullOrEmpty(xml.DescricaoParecer))
                crm.PareceresAnteriores = xml.DescricaoParecer;
            else
                crm.AddNullProperty("PareceresAnteriores");

            if (xml.DataHoraTerminoEsperada.HasValue)
                crm.Conclusao = xml.DataHoraTerminoEsperada.Value;
            else
                crm.AddNullProperty("Conclusao");

            if (xml.Duracao.HasValue)
                crm.Duracao = xml.Duracao.Value;
            else
                crm.AddNullProperty("Duracao");

            return crm;
        }
 
        public string Enviar(Tarefa objModel)
        {
            throw new NotImplementedException();
        }
    }
}