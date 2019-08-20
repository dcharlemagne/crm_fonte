//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Intelbras.CRM2013.Domain.Model;
//using System.Text;
//using Pollux = Intelbras.Message.Helper;
//using SDKore.DomainModel;
//using Intelbras.CRM2013.Domain.Servicos;

//namespace Intelbras.CRM2013.Domain.Integracao
//{
//    public class MSG0168 : Base, IBase<Message.Helper.MSG0168, Domain.Model.ItemFila>
//    {
//        #region Propriedades
//        //Dictionary que sera enviado como resposta do request
//        private Dictionary<string, object> retorno = new Dictionary<string, object>();
//        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
//        #endregion

//        #region Construtor
//        public MSG0168(string org, bool isOffline)
//            : base(org, isOffline)
//        {
//            this.Organizacao = org;
//            this.IsOffline = isOffline;
//        }
//        #endregion

//        #region Trace
//        private TrideaFramework.Helper.Trace Trace { get; set; }
//        public void DefinirObjetoTrace(TrideaFramework.Helper.Trace trace)
//        {
//            this.Trace = trace;
//        }
//        #endregion

//        #region Executar

//        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario = null )
//        {
//            //TODO: Falta implementar a Model
//            //var entidade = new PesquisaSatisfacao(Organizacao, IsOffline);
//            //Guid id = base.Repositorios.RepositorioPesquisaSatisfacao.Create(entidade);
//            Guid id = new Guid();

//            if (id != Guid.Empty)
//            {
//                resultadoPersistencia.Sucesso = false;
//                resultadoPersistencia.Mensagem = "Registro não encontrado!";
//            }
//            else
//            {
//                resultadoPersistencia.Sucesso = true;
//                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
//            }

//            retorno.Add("Resultado", resultadoPersistencia);

//            return CriarMensagemRetorno<Intelbras.Message.Helper.MSG0168R1>(numeroMensagem, retorno);
//        }

//        #endregion

//        #region Métodos Auxiliares

//        public string Enviar(ItemFila objModel)
//        {
//            return String.Empty;
//        }

//        public PrioridadeLigacaoCallCenter DefinirPropriedades(Intelbras.Message.Helper.MSG0170 xml)
//        {
//            throw new NotImplementedException();
//        }
//        #endregion
//    }
//}
