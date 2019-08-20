using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_pontuacao")]
    public class Pontuacao : DomainBase
    {
        public Pontuacao(string organization, bool isOffline)
            : base(organization, isOffline)
        {

        }

        public Pontuacao()
        {

        }
        public List<Pontuacao> ListaPontuacaoValida
        {
            get
            {
                var pontuacao = GerenciadorCache.Get("LISTAPONTUACAO") as List<Pontuacao>;
                if (pontuacao == null)
                {
                    pontuacao = (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadePontuacao.ObterListaCompletaVigenciaValida();
                    if (pontuacao.Count > 0)
                        GerenciadorCache.Add("LISTAPONTUACAO", pontuacao, TimeSpan.FromHours(12));
                }

                return pontuacao;
            }
        }


        public Pontuacao this[Guid produtoId, Guid distribuidor, Model.Contato usuario]
        {
            get
            {
                List<Pontuacao> listaFiltrada = new List<Pontuacao>();

                listaFiltrada = (from p in ListaPontuacaoValida
                                 where p.Produto.Id == produtoId &&
                                       ((p.Distribuidor != null && p.Distribuidor.Id != null && p.Distribuidor.Id == distribuidor) || p.Distribuidor == null) &&
                                       ((p.Pais != null && p.Pais.Id != null && usuario.Endereco1Pais != null && usuario.Endereco1Pais.Id != null && p.Pais.Id == usuario.Endereco1Pais.Id) || p.Pais == null) &&
                                       ((p.Estado != null && p.Estado.Id != null && usuario.Endereco1Estadoid != null && usuario.Endereco1Estadoid.Id != null && p.Estado.Id == usuario.Endereco1Estadoid.Id) || p.Estado == null)
                                 orderby p.PontosRevenda descending
                                 select p).Take(1).ToList<Pontuacao>();

                if (listaFiltrada.Count > 0)
                    return listaFiltrada[0];
                else
                    return null;
            }
        }

        [LogicalAttribute("new_pontuacaoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("new_produtoId")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("new_ufid")]
        public Lookup Estado { get; set; }

        [LogicalAttribute("new_paisid")]
        public Lookup Pais { get; set; }

        [LogicalAttribute("new_distribuidorid")]
        public Lookup Distribuidor { get; set; }

        [LogicalAttribute("new_pontos_revenda")]
        public int? PontosRevenda { get; set; }

        [LogicalAttribute("new_pontos_vendedor")]
        public int? PontosVendedor { get; set; }

        [LogicalAttribute("new_data_inicio_vigencia")]
        public DateTime? DataInicioVigencia { get; set; }

        [LogicalAttribute("new_data_final_vigencia")]
        public DateTime? DataFinalVigencia { get; set; }
    }
}
