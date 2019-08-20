using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    /// <summary>
    /// Autor: Marcelo Ferreira de Láias (MSBS Tridea)
    /// Data: 05/09/2011
    /// Descrição: Classe para controlar a coleta (Dados da Solicitação). 
    /// </summary>
    //[LogicalEntity("")] sem entidade no CRM
    public class Solicitacao : DomainBase
    {

        #region Atributos
        /// <summary>
        /// Indica se a solicitação é de coleta domiciliária e/ou uma autorização de postagem.
        /// </summary>
        public Enum.TipoDeSolicitacaoCorreios Tipo { get; set; }

        /// <summary>
        /// Número da Autorização de Postagem. Usado quando o cliente já possui uma faixa numérica 
        /// reservada desse tipo de solicitação. Esse número será encaminhado no arquivo de retorno.
        /// </summary>
        public string Numero { get; set; }

        /// <summary>
        /// Campo para preenchimento livre. É um valor para identificação da solicitação junto ao cliente. 
        /// Este valor é enviado no arquivo de retorno gerado após o processamento.
        /// </summary>
        public string IdCliente { get; set; }

        /// <summary>
        /// Coleta domiciliar: Data para agendamento da coleta. Se informado o pedido fica retido no
        /// sistema e a primeira tentativa de coleta é feita apenas na data informada. O sistema aceita 
        /// apenas datas com mais de cinco dias corridos a partir da data de processamento do pedido. 
        /// Autorização de Postagem: Indica a quantidade de dias de validade da autorização. A validade 
        /// deve ser de no mínimo 5 e no máximo 30 dias. Se não for informada, a validade da autorização 
        /// será de 10 (dez) dias corridos a partir da data do processamento do pedido.
        /// </summary>
        public DateTime AG { get; set; }

        /// <summary>
        /// Número do cartão de postagem para ser usado no faturamento dos valores do serviço realizado.
        /// Caso seja informado para essa solicitação, a tag <cartao> do cabeçalho será ignorada.
        /// </summary>
        public string Cartao { get; set; }

        /// <summary>
        /// Somatório de todos os valores declarados dos objetos da coleta. Exemplo: 520.70
        /// </summary>
        public double ValorDeclarado { get; set; }

        /// <summary>
        /// Códigos de serviços adicionais separados por vírgula.
        /// </summary>
        public string ServicoAdicional { get; set; }

        /// <summary>
        /// Descrição / instruções para coleta.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Indica se é para solicitar Aviso de Recebimento para as encomendas cadastradas. Usado apenas
        /// para pedidos de Autorização de Postagem. Colocar 0 ou 1.
        /// </summary>
        public bool AR { get; set; }

        /// <summary>
        /// Indica que serão impressas vias de checklist. Apenas clientes previamente habilitados podem 
        /// utilizar essa opção. Código fornecido pela ECT.
        /// </summary>
        public string CKList { get; set; }

        private EnderecoXML remetente;
        public EnderecoXML Remetente
        {
            get { return remetente; }
            //set { remetente = value; }
        }

        private List<ObjetoColeta> objetos;
        public List<ObjetoColeta> Objetos
        {
            get { return objetos; }
            //set { objetos = value; }
        }

        #endregion

        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public Solicitacao() { }

        public Solicitacao(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Solicitacao(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

    }
}
