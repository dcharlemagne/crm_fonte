using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.IRepository;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class LeadService
    {

        private RepositoryService RepositoryService { get; set; }

        public LeadService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public LeadService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #region Atributos
        private ClientePotencial lead = null;

        #endregion

        public void Anexar(Anotacao anotacao)
        {
            if (null == lead)
                throw new ArgumentNullException("Lead", "O lead informado é nulo ou inválido");

            if (Guid.Empty == lead.ID)
                throw new ArgumentNullException("Lead ID", "O ID do lead informado é nulo ou inválido");

            if (null == anotacao)
                throw new ArgumentNullException("Anotação", "O anexo informado é nulo ou inválido.");

            RepositoryService.Oportunidade.CriarAnotacaoParaUmLead(lead.ID.Value, anotacao);
        }

        public Guid GerarLead()
        {
            lead.ID = RepositoryService.ClientePotencial.Create(this.lead);
            return lead.ID.Value;
        }

        public List<ClientePotencial> ListarProjetosPor(String CodigoRevenda, String CodigoDistribuidor, String CodigoExecutivo, String CNPJCliente, int? SituacaoProjeto, string CodigoSegmento, string CodigoUnidadeNegocio)
        {
            return RepositoryService.ClientePotencial.ListarProjetosPor(CodigoRevenda, CodigoDistribuidor, CodigoExecutivo, CNPJCliente, SituacaoProjeto, CodigoSegmento, CodigoUnidadeNegocio);
        }

        public List<ClientePotencial> ListarProjetosDuplicidade(String CNPJCliente,  String UnidadeNegocio)
        {
            return RepositoryService.ClientePotencial.ListarProjetosDuplicidade(CNPJCliente, UnidadeNegocio);
        }

        public ClientePotencial ObterPorNumeroProjeto(string numeroprojeto)
        {
            return RepositoryService.ClientePotencial.ObterPorNumeroProjeto(numeroprojeto);
        }

        public ClientePotencial Persistir(Model.ClientePotencial Objcliente)
        {
            List<Model.ClientePotencial> lstTempCliente = new List<ClientePotencial>();
            ClientePotencial tmpCliente = null;

            if (Objcliente.ID.HasValue)
            {
                tmpCliente = RepositoryService.ClientePotencial.Retrieve(Objcliente.ID.Value);

                if (tmpCliente != null)
                {
                    Objcliente.ID = tmpCliente.ID;
                    RepositoryService.ClientePotencial.Update(Objcliente);

                    return Objcliente;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                Objcliente.ID = RepositoryService.ClientePotencial.Create(Objcliente);
                return Objcliente;
            }
        }

        public int ObterUltimoNumeroProjeto(string Ano)
        {
            return RepositoryService.ClientePotencial.ObterUltimoNumeroProjeto(Ano);
        }
    }
}
