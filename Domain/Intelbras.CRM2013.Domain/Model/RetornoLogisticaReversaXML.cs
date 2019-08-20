using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Domain.Model
{
    /// <summary>
    /// Autor: Marcelo Ferreira de Láias (MSBS Tridea)
    /// Data: 02/09/2011
    /// Descrição: Classe para ler o XML com base no leiaute do arquivo versão 4.0 que é utilizado para retorno gerado após o processamento dos pedidos de
    /// coleta contidos no arquivo XML de solicitação. 
    /// </summary>
    //[LogicalEntity("")] sem entidade no CRM
    public class RetornoLogisticaReversaXML : DomainBase
    {

        #region Atributos
        /// <summary>
        /// Identifica qual o tipo do arquivo dentro do sistema “Logística Reversa”. Permite a distinção entre os
        /// diversos tipos de arquivos que o sistema pode processar.
        /// Para este leiaute deverá ser preenchido: RetornoPostagem
        /// </summary>
        public string TipoArquivo { get { return "RetornoPostagem"; } }

        /// <summary>
        /// Identifica a versão do arquivo XML. Para esta versão deverá ser preenchido: 4.0
        /// </summary>
        public string VersaoArquivo { get { return "4.0"; } }

        /// <summary>
        /// Data para agendar o processamento do arquivo. Se informada o sistema processa o arquivo apenas na data indicada.
        /// </summary>
        public StatusDoProcessamentoLogisticaReversa StatusProcessamento { get; set; }

        /// <summary>
        /// Data do processamento do arquivo.
        /// </summary>
        public DateTime DataProcessamento { get; set; }

        /// <summary>
        /// Hora do processamento do arquivo. 
        /// </summary>
        public DateTime HoraProcessamento { get; set; }

        /// <summary>
        /// Código do erro gerado. Se o arquivo foi processado corretamente o sistema atribui o valor “00” para esta tag.
        /// Verificar a tabela de erros constante neste documento.
        /// </summary>
        public string CodidoErro { get; set; }

        /// <summary>
        /// Descrição do erro caso ocorra.
        /// </summary>
        public string MsgErro { get; set; }

        private List<ObjetoPostal> resultadoSolicitacao;
        public List<ObjetoPostal> ResultadoSolicitacao
        {
            get { return resultadoSolicitacao; }
        }

        #endregion

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public RetornoLogisticaReversaXML() { }

        public RetornoLogisticaReversaXML(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public RetornoLogisticaReversaXML(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Metodos
        #endregion

    }
}
