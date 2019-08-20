using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Enum;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("subject")]
    public class Assunto : DomainBase
    {
        #region Atributos

        [LogicalAttribute("subjectid")]
        public Guid? ID { get; set; }
        [LogicalAttribute("title")]
        public String Nome { get; set; }        
        [LogicalAttribute("parentsubject")]
        public Lookup AssuntoPai { get; set;}
        [LogicalAttribute("description")]
        public string Descricao { get; set; }

        private Guid? assuntoPaiId;        
        private string assuntoPaiNome;
        
        public TipoDeAssunto TipoDeAssunto
        {
            get
            {
                if (this.Id == Guid.Empty)
                    return TipoDeAssunto.Vazio;
                else
                    return new RepositoryService().Assunto.BuscaTipoDeRelacaoPor(this);
            }
            set {  }
        }

        public string AssuntoPaiNome
        {
            get { return AssuntoPai != null ? AssuntoPai.Name : ""; }
            set { assuntoPaiNome = value; }
        }
        
        public Guid? AssuntoPaiId
        {
            get { return AssuntoPai != null ? AssuntoPai.Id : (Guid?) null; }
            set { assuntoPaiId = value; }
        }

        
        private bool? temFilho = null;
        public bool TemFilho
        {
            get
            {
                if (this.Id == Guid.Empty) return false;
                if (!temFilho.HasValue) temFilho = new RepositoryService().Assunto.TemAssuntoFilho(this.Id);
                return temFilho.Value;
            }
        }

        public string CampoRelacionadoNaOcorrencia
        {
            get {
                var campo = string.Empty;
                switch (this.TipoDeAssunto)
                {
                    case TipoDeAssunto.Motivo:
                        campo = "new_assunto_motivoid";
                        break;
                    case TipoDeAssunto.Parte:
                        campo = "new_assunto_parteid";
                        break;
                    case TipoDeAssunto.Problema:
                        campo = "new_assunto_problemaid";
                        break;
                    case TipoDeAssunto.Produto:
                        campo = "new_assunto_produtoid";
                        break;
                    case TipoDeAssunto.Serie:
                        campo = "new_assunto_serieid";
                        break;
                    case TipoDeAssunto.TipoDeProduto:
                        campo = "new_assunto_tipoid";
                        break;
                    case TipoDeAssunto.UnidadeDeNegocio:
                        campo = "new_assunto_unidadeid";
                        break;
                }

                return campo;
            }   
        }

        #endregion

        #region Contrutores

        private RepositoryService RepositoryService { get; set; }

        public Assunto() { }

        public Assunto(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Assunto(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Metodos

        public List<Assunto> EstruturaDoAssunto()
        {
            List<Assunto> listaAssunto = new List<Assunto>();
            if (this.Id == Guid.Empty) return listaAssunto;
            Assunto assunto = new RepositoryService().Assunto.Retrieve(this.Id);
            if (assunto == null) return listaAssunto;
            listaAssunto.Add(assunto);
            while (assunto.AssuntoPaiId.HasValue)
            {
                assunto = new RepositoryService().Assunto.Retrieve(assunto.AssuntoPaiId.Value);
                listaAssunto.Add(assunto);
            }
            return listaAssunto;
        }

        public string EstruturaDoAssuntoNome()
        {
            string resultado = string.Empty;
            if (this.Id == Guid.Empty) return resultado;

            List<Assunto> listaAssunto = this.EstruturaDoAssunto();

            for (var i = (listaAssunto.Count - 1); i >= 0; i--)
                resultado += " -> " + listaAssunto[i].Nome;

            resultado = resultado.Substring(3);

            return resultado;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;

            return this.ID == ((Assunto)obj).Id;
        }
        #endregion
    }
}
