
using SDKore.Crm.Util;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_processamentofidelidade")]
    public class ProcessamentoFidelidade : DomainBase
    {
        public ProcessamentoFidelidade()
        {

        }

        public ProcessamentoFidelidade(string organization, bool isOffline)
            : base(organization, isOffline)
        {

        }

        [LogicalAttribute("new_name")]
        public string Nome { get; set; }

        [LogicalAttribute("new_mensagem")]
        public string Mensagem { get; set; }

        [LogicalAttribute("statuscode")]
        public new int? Status { get; set; }
    }
}
