using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Intelbras.Message.Helper;
using System.Xml.Linq;
using System.Web;
using System.Web.Services;


namespace Intelbras.CRM2013.Application.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Servicos" in code, svc and config file together.
    public class Servicos : IServicos
    {
        //public string DoWork()
        //{
        //    return "QualquerTeste";
        //}
        [WebMethod]
        public bool Postar(string requisicao, out string resposta)
        {
            resposta = requisicao;
         
           return true;
        }

         public bool Ping()
         {
             return true;
         }


         public string Pollux()
         {

             var teste = new Pollux.MessageReceiverClient("ServiceReceiver_Web");
             string retorno = string.Empty;

             try
             {


                 MSG0001 msg0001 = new MSG0001("DBFC273E-4811-40C4-8A4E-1629731ADD9A", "04511060");
                 msg0001.CEP = "04511060";

                 string Message = msg0001.GenerateMessage();
                 string Response = String.Empty;


                 if (teste.PostMessage(out Response, Message, "1", "1"))
                 {
                     MSG0001R1 MsgR1 = MessageBase.LoadMessage<MSG0001R1>(XDocument.Parse(Response), true);
                     //retorno = "OK:" + MsgR1.Logradouro;
                     //msgR1.Endereco
                 }
                 else
                 {
                     ERR0001 MsgE1 = MessageBase.LoadMessage<ERR0001>(XDocument.Parse(Response), true);
                     retorno = "ERRO:" + MsgE1.DescricaoErro;
                 }




             }
             catch (Exception)
             {
                 //ksdjf sjkka joa 

             }

             return retorno;

         }
    }
}
