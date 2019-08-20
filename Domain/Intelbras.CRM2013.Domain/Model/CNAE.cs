using SDKore.Crm.Util;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_cnae")]
    public class CNAE : DomainBase
    {
        public CNAE() { }

        public CNAE(string organization, bool isOffline) : base(organization, isOffline) { }

        public CNAE(string organization, bool isOffline, object provider) : base(organization, isOffline, provider) { }

        [LogicalAttribute("itbc_cnaeid")]
        public Guid? ID { get; set; }



        // String
        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_denominacao")]
        public string Denominacao { get; set; }

        [LogicalAttribute("itbc_classe")]
        public string Classe { get; set; }

        [LogicalAttribute("itbc_Subclasse")]
        public string SubClasse { get; set; }

        [LogicalAttribute("itbc_grupo")]
        public string Grupo { get; set; }

        [LogicalAttribute("itbc_divisao")]
        public string Divisao { get; set; }

        [LogicalAttribute("itbc_secao")]
        public string Secao { get; set; }
    }
}