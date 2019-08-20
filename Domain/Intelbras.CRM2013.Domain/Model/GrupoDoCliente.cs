using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Model
{

    [LogicalEntity("new_grupo_cliente")]
    public class GrupoDoCliente : DomainBase
    {

        private RepositoryService RepositoryService { get; set; }


        public GrupoDoCliente() { }

        public GrupoDoCliente(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public GrupoDoCliente(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }


        [LogicalAttribute("new_codigo_grupo_cliente")]
        public int? Codigo { get; set; }
        [LogicalAttribute("new_name")]
        public String Nome { get; set; }

        public const string chaveCache = "Grupos_Clientes_Intelbras";
        private List<GrupoDoCliente> _grupos;
        private List<GrupoDoCliente> Grupos
        {
            get
            {
                _grupos = GerenciadorCache.Get(chaveCache) as List<GrupoDoCliente>;
                if (_grupos == null)
                {
                    _grupos = (new Domain.Servicos.RepositoryService()).GrupoDoCliente.ListarTodos();
                    if (_grupos.Count > 0)
                        GerenciadorCache.Add(chaveCache, _grupos, 24);
                }

                _grupos.RemoveAll(obj => !obj.Codigo.HasValue);
                return _grupos;
            }
        }

        public bool IsDistribuir
        {
            get
            {
                var lista = this[SDKore.Configuration.ConfigurationManager.GetSettingValue("IdentificacaoDistribuidor").Split(';')];
                return (lista.Find(obj => obj.Id == this.Id) != null);
            }
            set { }
        }

        public bool IsRevenda
        {
            get
            {
                var lista = this[SDKore.Configuration.ConfigurationManager.GetSettingValue("IdentificacaoRevenda").Split(';')];
                return (lista.Find(obj => obj.Id == this.Id) != null);
            }
            set { }
        }

        public List<GrupoDoCliente> this[string[] codigos]
        {
            get
            {
                return codigos.Select(item => this.Grupos.Find(obj => obj.Codigo != null && obj.Codigo.Value == Convert.ToInt32(item))).Where(grupo => grupo != null).ToList();
            }
        }

    }
}
