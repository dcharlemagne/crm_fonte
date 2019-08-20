using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    /// <summary>
    /// Classe que controla o objeto da coleta na logística reversa.
    /// </summary>
    //[LogicalEntity("")] sem entidade no CRM
    public class ObjetoColeta : DomainBase
    {
        private RepositoryService RepositoryService { get; set; }

        public ObjetoColeta() { }

        public ObjetoColeta(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ObjetoColeta(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        /// <summary>
        /// Tag obrigatória. Apenas confirma o cadastro do objeto dentro da solicitação. Valor Fixo "1"
        /// </summary>
        public string Item { get { return "1"; } }

        /// <summary>
        /// Campo para preenchimento livre. É um valor para identificação do objeto junto ao cliente. Quando o 
        /// pedido é de coleta domiciliar e o pedido é processado com sucesso este valor é enviado no arquivo 
        /// de retorno gerado após o processamento. Exemplo: Número da nota fiscal.
        /// </summary>
        public string IdObj { get; set; }

        /// <summary>
        /// Descrição do objeto que será coletado   
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Número do objeto APENAS PARA PEDIDOS DE COLETA SIMULTÂNEA. 
        /// O contrato deve aceitar pedidos de coleta simultânea.   
        /// </summary>
        public string Entrega { get; set; }

        /// <summary>
        /// Número do objeto quando existe uma faixa numérica reservada para o cliente. 
        /// Esta opção ainda não é utilizada.   
        /// </summary>
        public string Numero { get; set; }

    }
}
