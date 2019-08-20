using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace  Intelbras.CRM2013.Application.Plugin
{

    public static class Helper
    {
        #region Method

        //public static void TornarTextoEmCaixaAlta(string nomeEntidade, ref Microsoft.Xrm.Sdk.Entity entidadeContexto)
        //{
        //    string atributos = string.Empty;
        //    try
        //    {
        //        atributos = SDKore.Configuration.ConfigurationManager.GetSettingValue(string.Format("CaixaAlta{0}", nomeEntidade.ToUpper()));
        //    }
        //    catch { }

        //    foreach (var item in atributos.Split(','))
        //    {
        //        if (entidadeContexto.Contains(item))
        //            entidadeContexto[item] = entidadeContexto[item].ToString().ToUpper();
        //    }
        //}

         /// <summary>
        /// Retorna XML de um objeto
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string CriarXML(Object obj)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());

            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, obj);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
                return xmlDoc.InnerXml;
            }
        }

        #endregion

        #region Enum

        public static T EnumConverter<T>(string valor)
        {
            return (T)System.Enum.Parse(typeof(T), valor, true);
        }

        public enum MessageName
        {
            Create,
            Update,
            Delete,
            Send,
            SetState,
            SetStateDynamicEntity,
            Associate,
            Disassociate,
            Win
        }

        public enum Stage
        {
            PreValidation = 10,
            PreOperation = 20,
            PostOperation = 40
        }

        public enum Mode
        {
            Asynchonous = 1,
            Synchonous = 0
        }

        #endregion
    }
}
