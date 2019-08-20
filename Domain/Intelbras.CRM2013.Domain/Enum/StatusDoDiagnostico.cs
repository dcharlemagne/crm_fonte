namespace Intelbras.CRM2013.Domain.Enum
{
    public enum StatusDoDiagnostico
    {
        // Ativo
        Vazio = 0,
        AguardandoPeca = 1,
        PedidoSolicitadoAoEms = 3,
        AguardandoConserto = 4,
        ConsertoRealizado = 5,
        IntervencaoTecnica = 6,
        Substituido = 7,

        // Inativo
        Cancelado = 2
    }
}
