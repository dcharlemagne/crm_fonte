using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("relationshiprole")]
    public class FuncaoRelacionamento : DomainBase
    {
        public FuncaoRelacionamento() { }

        public FuncaoRelacionamento(string organization, bool isOffline)
            : base(organization, isOffline)
        {

        }

        [LogicalAttribute("name")]
        public string Nome { get; set; }

        [LogicalAttribute("relationshiproleid")]
        public Guid? ID { get; set; }

        public const string ChaveCache = "FUNCOES_RELACIONAMENTO";

        private List<FuncaoRelacionamento> _funcoes;
        private List<FuncaoRelacionamento> Funcoes
        {
            get
            {
                _funcoes = GerenciadorCache.Get(ChaveCache) as List<FuncaoRelacionamento>;
                if (_funcoes == null)
                {
                    _funcoes = (new CRM2013.Domain.Servicos.RepositoryService()).FuncaoRelacionamento.ListarTodas();
                    if (_funcoes.Count > 0)
                        GerenciadorCache.Add(ChaveCache, _funcoes, 24);
                }

                return _funcoes;
            }
        }


        public  FuncaoRelacionamento this[Enum.Contato.TipoAcesso tipoAcesso]
        {
            get
            {
                return this.Funcoes.FirstOrDefault(c => c.Nome == tipoAcesso.ToString());
            }
        }
    }
}
