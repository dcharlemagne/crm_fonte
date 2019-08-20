using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_estrutura_produto")]
    public class EstruturaProduto:DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public EstruturaProduto() { }

        public EstruturaProduto(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public EstruturaProduto(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }


        #endregion

        #region Atributos
        [LogicalAttribute("itbc_estrutura_produtoid")]
        public Guid? ID { get; set; }
        
        [LogicalAttribute("itbc_atualizado_viamultiplanta")]
        public Boolean AtualizadoViaMultiplanta { get; set; }

        [LogicalAttribute("itbc_chave_integracao")]
        public String ChaveIntegracao { get; set; }

        [LogicalAttribute("itbc_check_sum")]
        public String CheckSum { get; set; }

        [LogicalAttribute("itbc_concentracao")]
        public Decimal? Concentracao { get; set; }

        [LogicalAttribute("itbc_concentracao_max")]
        public Decimal? ConcentracaoMaxima { get; set; }

        [LogicalAttribute("itbc_concentracao_minima")]
        public Decimal? ConcentracaoMinima { get; set; }

        [LogicalAttribute("itbc_data_inicio")]
        public DateTime? DataInicio { get; set; }

        [LogicalAttribute("itbc_data_termino")]
        public DateTime? DataTermino { get; set; }

        [LogicalAttribute("itbc_deposito")]
        public String Deposito { get; set; }

        [LogicalAttribute("itbc_engenharia")]
        public Int32? EngenhariaResponsavel { get; set; }

        [LogicalAttribute("itbc_fantasma")]
        public Boolean Fantasma { get; set; }

        [LogicalAttribute("itbc_fator_perda")]
        public Decimal? FatorPerda { get; set; }

        [LogicalAttribute("itbc_lista_componentes")]
        public String ListaComponentes { get; set; }

        [LogicalAttribute("itbc_local_montagem")]
        public String LocalMontagem { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_observacao")]
        public String Observacao { get; set; }

        [LogicalAttribute("itbc_operacao")]
        public int? Operacao { get; set; }

        [LogicalAttribute("itbc_porcentagem_processo")]
        public Decimal? PorcentagemProcesso { get; set; }

        [LogicalAttribute("itbc_posicao")]
        public String Posicao { get; set; }

        [LogicalAttribute("itbc_proporcao")]
        public Decimal? Proporcao { get; set; }

        [LogicalAttribute("itbc_quantidade")]
        public Decimal? Quantidade { get; set; }

        [LogicalAttribute("itbc_quantidade_componente")]
        public Decimal? QuantidadeComponente { get; set; }

        [LogicalAttribute("itbc_quantidade_item")]
        public Decimal? QuantidadeItem { get; set; }

        [LogicalAttribute("itbc_quantidade_liquida")]
        public Decimal? QuantidadeLiquida { get; set; }

        [LogicalAttribute("itbc_reaprova_desmontagem")]
        public Boolean ReaprovaDesmontagem { get; set; }

        [LogicalAttribute("itbc_refer_estrutura")]
        public Boolean ReferEstruturaDisponivel { get; set; }

        [LogicalAttribute("itbc_referencia")]
        public String Referencia { get; set; }

        [LogicalAttribute("itbc_rendimento")]
        public Decimal? Rendimento { get; set; }

        [LogicalAttribute("itbc_revisao")]
        public String Revisao { get; set; }

        [LogicalAttribute("itbc_roteiro")]
        public String Roteiro { get; set; }

        [LogicalAttribute("itbc_sequencia")]
        public Int32? Sequencia { get; set; }

        [LogicalAttribute("itbc_serie_final")]
        public String SerieFinal { get; set; }

        [LogicalAttribute("itbc_serie_inicial")]
        public String SerieInicial { get; set; }

        [LogicalAttribute("itbc_tempo_reserva")]
        public Int32? TempoReserva { get; set; }

        [LogicalAttribute("itbc_tipo_sobra")]
        public Int32? TipoSobra { get; set; }

        [LogicalAttribute("itbc_todas_referencias_item")]
        public Boolean TodasReferenciasItem { get; set; }

        [LogicalAttribute("itbc_variacao_proporcao")]
        public Decimal? VariacaoProporcao { get; set; }

        [LogicalAttribute("itbc_variacao_quantidade")]
        public Int32? VariacaoQuantidade { get; set; }

        #endregion  
    }
}
