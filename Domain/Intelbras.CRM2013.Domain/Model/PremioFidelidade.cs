using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_premio_fidelidade")]
    public class PremioFidelidade : DomainBase
    {
        public PremioFidelidade() { }

        public PremioFidelidade(string organization, bool isOffline)
            : base(organization, isOffline)
        {

        }

        [LogicalAttribute("new_premio_fidelidadeid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("new_resgate_premio_fidelidadeid")]
        public Lookup ResgatePremioFidelidade { get; set; }

        [LogicalAttribute("new_name")]
        public string Nome { get; set; }

        [LogicalAttribute("new_caminho_imagem")]
        public string Imagem { get; set; }

        [LogicalAttribute("new_pontos_necessarios")]
        public int? PontosNecessarios { get; set; }

        [LogicalAttribute("new_grupo_premioid")]
        public Lookup Grupo { get; set; }

        [LogicalAttribute("new_descricao")]
        public string Descricao { get; set; }

        public List<PremioFidelidade> PremiosFidelidade
        {
            get
            {
                var lista = GerenciadorCache.Get("LISTAPREMIOSFIDELIDADE") as List<PremioFidelidade>;
                if (lista == null)
                {
                    lista = (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadePremio.ListarTodos();
                    if (lista.Count > 0)
                        GerenciadorCache.Add("LISTAPREMIOSFIDELIDADE", lista, TimeSpan.FromHours(12));
                }

                return lista;
            }
        }
    }
}
