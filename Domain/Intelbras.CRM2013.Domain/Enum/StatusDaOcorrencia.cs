using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Enum
{
    [Serializable]
    public enum StatusDaOcorrencia
    {
        Vazio = 0,
        A_Definir1 = 1,
        A_Definir2 = 2,
        A_Definir3 = 3,
        A_Definir4 = 4,
        Resolvido = 5,
        CanceladaSistema = 6,
        A_Definir7 = 7,
        A_Definir8 = 8,
        A_Definir9 = 9,
        A_Definir10 = 10,
        A_Definir11 = 11,
        Aberta = 200000,
        Em_Andamento = 200001,
        Pendente = 200002,
        Cancelada = 200003,
        Fechada = 200004,
        AguardandoAprovacao = 200005,
        AguardandoFechamento = 200006,
        A_Definir200007 = 200007,
        A_Definir200008 = 200008,
        A_Definir200009 = 200009,

        /* Logística Reversa */
        Aguardando_Tratativa = 200010,
        A_Definir200011 = 200011,
        A_Definir200012 = 200012,
        A_Definir200013 = 200013,
        A_Definir200014 = 200014,
        A_Definir200015 = 200015,
        A_Definir200016 = 200016,
        A_Definir200017 = 200017,
        A_Definir200018 = 200018,
        A_Definir200019 = 200019,
        A_Definir200020 = 200020,
        A_Definir200021 = 200021,
        A_Definir200022 = 200022,
        A_Definir200023 = 200023,
        A_Definir200024 = 200024,
        A_Definir200025 = 200025,
        CallCenter1 = 200026,
        CallCenter2 = 200027,
        CallCenter3 = 200028,
        CallCenter4 = 200029,
        CallCenter5 = 200030,
        CallCenter6 = 200031,
        CallCenter7 = 200032,
        CallCenter8 = 200033,
        CallCenter9 = 200034,

        Aguardando_Analise = 200035,
        Aguardando_Peça = 200036,
        Aguardando_Conserto = 200037,
        Conserto_Realizado = 200038,
        Fechada_Retirado_Cliente = 200039,
        Auditoria = 200040,
        Ajuste_Posto_de_Serviço = 200041,
        Aprovada = 200042,
        Reprovada = 200043,
        Encerrada = 200044,
        Pedido_Solicitado = 200045,

        Status200046 = 200046,
        Status200047 = 200047,
        Status200048 = 200048,
        Status200049 = 200049,
        Status200050 = 200050,

        /* Logística Reversa */
        Produto_Entregue = 200051,
        Aguardando_Envio_Produto = 200052,
        Logistica_Reversa_Recusada = 200054,
        Logistica_Reversa_Expirada = 200055,
        Aguardando_Visita_Tecnica = 993520001,
        Aguardando_Confirmacao = 993520002,
        Atendimento_Confirmado = 993520003,
        Atendimento_Rejeitado = 993520004,
        Visita_Concluida = 993520005,
        Pendente_Operadora = 993520006,
        Redirecionado_Grupo_Telefonia_IP = 993520007,
        Redirecionado_Grupo_Voz_Video = 993520008,
        Aguardando_Analise_Orcamento = 993520009,
        Fechamento_Cobrado = 993520010
    }
}
