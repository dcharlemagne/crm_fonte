//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.ServiceModel;
//using System.ServiceModel.Web;
//using System.Text;
//using Intelbras.Message.Helper;
//using System.Xml.Linq;

//namespace Intelbras.CRM2013.Application.WCF
//{
//    public class ServicoBarramento : IServicoBarramento
//    {
//        public bool Postar(string mensagem, out string resposta)
//        {
//            bool retorno = true;
//            try
//            {
//                new Domain.Servicos.Integracao("Adicionar a Organização (Com base no arquivo de configuração)", false).Postar(mensagem, out resposta);
//            }
//            catch (Exception ex)
//            {
//                //TODO: Adicionar o código identificador do Emissor.
//                Intelbras.Message.Helper.ERR0001 erro = new Message.Helper.ERR0001("Identificador do Emissor", "1");
//                erro.DescricaoErro = Util.Utilitario.ObterMensagemErro(ex);
//                resposta = erro.GenerateMessage(true);
//                retorno = false;
//            }
//            return retorno;
//        }

//        public void Pollux()
//        {
//            var teste = new Pollux.MessageReceiverClient();

//            try
//            {
//                MSG0004 msg0004 = new MSG0004("DBFC273E-4811-40C4-8A4E-1629731ADD9A", "04511060");


//                string Message = msg0004.GenerateMessage();
//                string Response = string.Empty;
                
//                if (teste.PostMessage(out Response, Message, "1", "1"))
//                {
//                    MSG0001R1 MsgR1 = MessageBase.LoadMessage<MSG0001R1>(XDocument.Parse(Response), true);
//                }

//            }
//            catch (Exception)
//            {
//                //ksdjf sjkka joa 
//            }

//        }

//        public bool Ping()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
