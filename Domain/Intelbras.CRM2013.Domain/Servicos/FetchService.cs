using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class FetchService
    {
        private RepositoryService RepositoryService { get; set; }

        public FetchService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public FetchService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        private XmlDocument fetch;

        private string entityName;
        public string EntityName
        {
            get { return entityName; }
        }
        public FetchService(string fetch)
        {
            this.fetch = new XmlDocument();
            this.fetch.LoadXml(fetch);
            this.entityName = this.fetch.SelectSingleNode("//fetch/entity").Attributes["name"].InnerText;
        }

        public FetchService(XmlDocument fetch)
        {
            this.fetch = fetch;
            this.entityName = this.fetch.SelectSingleNode("//fetch/entity").Attributes["name"].InnerText;
        }

        public int QuantidadeDeRegistrosNaConsulta()
        {
            XmlDocument indoc = this.fetch;

            indoc.DocumentElement.Attributes.Remove(indoc.DocumentElement.Attributes["count"]);
            indoc.DocumentElement.Attributes.Remove(indoc.DocumentElement.Attributes["page"]);

            #region remove attribute e order

            foreach (XmlNode node in indoc.SelectNodes("//fetch/entity/attribute"))
                indoc.SelectSingleNode("//fetch/entity").RemoveChild(node);

            foreach (XmlNode node in indoc.SelectNodes("//fetch/entity/order"))
                indoc.SelectSingleNode("//fetch/entity").RemoveChild(node);

            foreach (XmlNode node in indoc.SelectNodes("//fetch/entity/link-entity"))
                foreach (XmlNode subnode in node.SelectNodes("./attribute"))
                    node.RemoveChild(subnode);

            #endregion

            #region  adiciona atrribute aggregate

            XmlAttribute aggrAttr = indoc.CreateAttribute("aggregate");
            aggrAttr.Value = "true";
            indoc.DocumentElement.Attributes.Append(aggrAttr);

            XmlNode field = indoc.CreateNode(XmlNodeType.Element, "attribute", null);

            XmlAttribute nameAttr = indoc.CreateAttribute("name");
            nameAttr.Value = string.Format("{0}id", (entityName == "activitypointer" ? "activity" : entityName));
            field.Attributes.Append(nameAttr);
                        

            XmlAttribute aggregateAttr = indoc.CreateAttribute("aggregate");
            aggregateAttr.Value = "count";
            field.Attributes.Append(aggregateAttr);

            XmlAttribute aliasAttr = indoc.CreateAttribute("alias");
            aliasAttr.Value = "C";
            field.Attributes.Append(aliasAttr);

            indoc.SelectSingleNode("//fetch/entity").AppendChild(field);

            #endregion

            var fullResultDocument = ExecutaFetch(indoc);
            int totalRecordCount = int.Parse(fullResultDocument.SelectSingleNode("//resultset/result/C").InnerText);

            
            return totalRecordCount;
        }

        public XmlDocument ExecutaFetch(XmlDocument xml)
        {
            //string fullResult = base.Provider.Fetch(xml.InnerXml);
            XmlDocument fullResultDocument = new XmlDocument();
            //fullResultDocument.LoadXml(fullResult);
            return fullResultDocument;
        }

    }
}
