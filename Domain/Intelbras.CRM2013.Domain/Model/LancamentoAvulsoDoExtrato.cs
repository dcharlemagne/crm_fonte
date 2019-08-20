using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_lancamento_avulso")]
    public class LancamentoAvulsoDoExtrato : DomainBase
    {
        [LogicalAttribute("new_extratoid")]
        public Lookup ExtratoId { get; set; }
        private Extrato extrato = null;
        public Extrato Extrato
        {
            get { return extrato; }
            set { extrato = value; }
        }
        [LogicalAttribute("new_clienteid")]
        public Lookup PostoAutorizadoId { get; set; }
        private Model.Conta postoAutorizado = null;
        public Model.Conta PostoAutorizado
        {
            get { return postoAutorizado; }
            set { postoAutorizado = value; }
        }
        [LogicalAttribute("new_valor")]
        public Decimal? Valor { get; set; }

        private RepositoryService RepositoryService { get; set; }

        public LancamentoAvulsoDoExtrato() { }

        public LancamentoAvulsoDoExtrato(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public LancamentoAvulsoDoExtrato(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

    }
}
