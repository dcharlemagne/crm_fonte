//using Intelbras.CRM2013.Domain.Model;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Pollux = Intelbras.Message.Helper;
//using SDKore.DomainModel;

//namespace Intelbras.CRM2013.Domain.Integracao
//{
//    public class MSG0133 : Base, IBase<Message.Helper.MSG0133, Domain.Model.NivelPosVendaClassificacao>
//    {

//        #region Propriedades
//        //Dictionary que sera enviado como resposta do request
//        private Dictionary<string, object> retorno = new Dictionary<string, object>();

//        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

//        #endregion

//        #region Construtor
//        public MSG0133(string org, bool isOffline)
//            : base(org, isOffline)
//        {
//            this.Organizacao = org;
//            this.IsOffline = isOffline;
//            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
//        }
//        #endregion

//        #region trace
//        private SDKore.Helper.Trace Trace { get; set; }
//        public void DefinirObjetoTrace(SDKore.Helper.Trace trace)
//        {
//            this.Trace = trace;
//        }
//        #endregion

//        #region Executar
//        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
//        {
//            //Trace.Add("Mensagem " + numeroMensagem + " XML: {0}", mensagem);
//            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0133>(mensagem));

//            if (!resultadoPersistencia.Sucesso)
//            {
//                retorno.Add("Resultado", resultadoPersistencia);
//                return CriarMensagemRetorno<Pollux.MSG0133R1>(numeroMensagem, retorno);
//            }

//            //objeto.ID = new Intelbras.CRM2013.Domain.Servicos.RegiaoService(this.Organizacao, this.IsOffline).Persistir(objeto, objeto.ID);
//            if (!objeto.ID.HasValue)
//                throw new Exception("Erro de persistência");

//            //new Intelbras.CRM2013.Domain.Servicos.RegiaoService(this.Organizacao, this.IsOffline).MudarStatus((Guid)objeto.ID, (int)objeto.Status);

//            resultadoPersistencia.Sucesso = true;
//            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
//            retorno.Add("Resultado", resultadoPersistencia);


//            return CriarMensagemRetorno<Pollux.MSG0133R1>(numeroMensagem, retorno);
//        }
//        #endregion

//        #region Definir Propriedades
//        public NivelPosVendaClassificacao DefinirPropriedades(Intelbras.Message.Helper.MSG0133 xml)
//        {
//            var crm = new NivelPosVendaClassificacao(this.Organizacao, this.IsOffline);

//            #region Propriedades Crm->Xml

//            crm.ID = xml.codigo;

//            crm.Nome = xml.Nome;

//            crm.Classificação = xml.classificacao;

//            crm.Proprietario = xml.proprietario;

//            crm.Status = xml.Situacao;


//            #endregion

//            return crm;
//        }
//        #endregion

//        #region Métodos Auxiliares

//        public string Enviar(Regiao objModel)
//        {
//            throw new NotImplementedException();
//        }
//        #endregion
//    }
//}
