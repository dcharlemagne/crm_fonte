using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.Message.Helper;
using System.Xml.Linq;
using System.Xml;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class DicionarioMensagens
    {
        // rotina de verificação de xml para recuperação de código de mensagem
        // criada em 14/03/2014 - por equipe Tridea

        public static String VerificarCodigoMensagem(String requisicao) 
        {
            String retorno = String.Empty;

            var doc = XDocument.Load(requisicao);

            try
            {
                var tag = (from t in doc.Descendants("CABECALHO")
                           select new
                           {
                               //Identidade = Convert.ToString(t.Element("IdentidadeEmissor").Value),
                               //NumeroOperacao = Convert.ToString(t.Element("NumeroOperacao").Value),
                               //CodigoMensagem = Convert.ToString(t.Element("CodigoMensagem").Value),
                               //LoginUsuario = Convert.ToString(t.Element("LoginUsuario").Value)
                               CodigoMensagem = Convert.ToString(t.Element("CodigoMensagem").Value)
                           }
                          );

                foreach (var linha in tag)
                {
                    retorno = linha.CodigoMensagem;
                }
            }
            catch (Exception)
            {
                retorno = "tag inexistente";
            }

            return retorno;          
        }
    }
}
