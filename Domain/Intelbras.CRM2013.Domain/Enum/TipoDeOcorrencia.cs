using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Enum
{
    public enum TipoDeOcorrencia
    {
        Instalacao = 200000,
        Desinstalacao = 200001,
        Reinstalacao = 200002,
        Consultoria = 200005,
        Vistoria = 200003,
        VisitaTecnica = 200006,
        Portabilidade = 200007,
        //        Sinistro = 200004,
        Sinistro = 200093,
        Improcedente = 200090,
        Manutencao = 200091,
        Reconfiguracao = 200092,
        Instalacao1 = 1,
        Reinstalacao1 = 3,

        Duvida = 300000,
        Elogios = 300001,
        Solicitação = 300002,
        Reclamação_Mau_Funcionamento = 300003,
        Sugestão = 300004,
        Atendimento_Avulso = 300005,
        Reclamação_Mau_Atendimento = 300006,
        Reclamação_Procon = 300007,
        Reclamação_Atraso_no_Conserto = 300008,
        Ordem_de_Serviço = 300009,
        Vazio = 0,
        ADefinir = 4,
        AnaliseDeDefeito = 300012,
        ReclamacaoImprocedente = 300013,
        Reclamacao_Falha_No_Processo = 300010,
        Reclamacao_Mau_Funcionamento = 300003,
        Reclamação_Nau_Funcionamento_Com_Solucao = 300015
    }
}
