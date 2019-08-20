
namespace Intelbras.CRM2013.Domain.Enum
{
    /// <summary>
    /// Indica se a solicitação é de coleta domiciliária e/ou uma autorização de postagem.
    /// </summary>
    public enum TipoDeSolicitacaoCorreios
    {
        Vazio = 0,

        /// <summary>
        /// CA - Caso não exista coleta domiciliar na localidade o sistema transforma
        /// automaticamente o pedido em uma AUTORIZAÇÃO DE POSTAGEM.
        /// </summary>
        ColetaDomiciliar = 1,

        /// <summary>
        /// C - Caso não exista a coleta no local indicado, o sistema ignora a solicitação
        /// </summary>
        ColetaDomiciliaria = 2,

        /// <summary>
        /// A - Caso nenhum valor seja passado nessa tag, o sistema entende que é uma 
        /// solicitação de coleta domiciliária.
        /// </summary>
        AutorizacaoDePostagem = 3

    }
}
