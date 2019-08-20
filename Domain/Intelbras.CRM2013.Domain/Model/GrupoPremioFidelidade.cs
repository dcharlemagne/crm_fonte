using SDKore.Crm.Util;
using System;
using System.Collections.Generic;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_grupo_premio_fidelidade")]
    public class GrupoFidelidade : DomainBase
    {
        public GrupoFidelidade()
        {

        }
        public GrupoFidelidade(string organization, bool isOffline)
            : base(organization, isOffline)
        {

        }

        [LogicalAttribute("new_grupo_premio_fidelidadeid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("new_name")]
        public string Nome { get; set; }

        public List<GrupoFidelidade> Grupos
        {
            get
            {
                var lista = GerenciadorCache.Get("LISTAGRUPOSFIDELIDADE") as List<GrupoFidelidade>;
                if (lista == null)
                {
                    lista = (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeGrupo.ListarTodos();
                    if (lista.Count > 0)
                        GerenciadorCache.Add("LISTAGRUPOSFIDELIDADE", lista, TimeSpan.FromHours(12));

                }

                return lista;
            }
        }
    }
}
