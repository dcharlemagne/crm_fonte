using System;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Crm.Util;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.ValueObjects
{
    [LogicalEntity("incident")]
    public class ImpressaoOcorrenciaIsolVO
    {
        [LogicalAttribute("incidentid")]
        public Guid OcorrenciaId { get; set; } // incidentid 
        [LogicalAttribute("ticketnumber")]
        public string NumeroOcorrencia { get; set; } // ticketnumber
        [LogicalAttribute("new_os_cliente")]
        public string OsCliente { get; set; } // new_os_cliente
        [LogicalAttribute("new_solicitante_portal")]
        public string SolicitantePortal { get; set; } // new_solicitante_portal
        [LogicalAttribute("new_kilometragem_percorrida")]
        public string KilometragemPercorrida { get; set; } // new_kilometragem_percorrida
        [LogicalAttribute("description")]
        public string DefeitoAlegado { get; set; } // description
        [LogicalAttribute("productserialnumber")]
        public string ProdutosDoCliente { get; set; } // productserialnumber
        [LogicalAttribute("new_atividade_executada")]
        public string AtividadeExecutada { get; set; } // new_atividade_executada
        [LogicalAttribute("new_contato_visita")]
        public string ContatoVisita { get; set; } // new_contato_visita
        [LogicalAttribute("new_numero_nf_consumidor")]
        public string NumeroNotaFiscalConsumidor { get; set; } // new_numero_nf_consumidor
        [LogicalAttribute("createdby")]
        public Lookup CriadoPor { get; set; } // createdby
        [LogicalAttribute("new_empresa_executanteid")]
        public Lookup EmpresaExecutante { get; set; } // new_empresa_executanteid
        public Lookup NotaFiscalRemessa { get; set; } // ####### Analisar

        // Id = new_solicitanteid
        // Nome = fullname
        // TelefoneComercial = telephone1
        public Contato Solicitante { get; set; }

        // Id = incident.new_tecnico_visitaid
        // Nome = fullname
        // TelefoneComercial = telephone1
        // Rg = new_rg
        public Contato TecnicoDaVisita { get; set; }

        // Id = incident.new_tecnico_responsavelid
        // Nome = fullname
        // TelefoneComercial = telephone1
        // Rg = new_rg
        public Contato TecnicoResponsavel { get; set; }

        // Id = incident.customerId
        // Nome = name
        // NomeFantasia = new_nome_fantasia
        // TelefonePrincipal.Numero = telephone1
        public Domain.Model.Conta Cliente { get; set; }

        // Id = contractId
        // ObservacaoOs = contractlanguage
        // NotaFiscalRemessa = new_nfremessaid
        public Contrato Contrato { get; set; }

        // Id = new_guid_endereco
        // UF = stateorprovince
        // Cidade = city
        // Cep = postalcode
        // Bairro = line2
        // Numero = new_numero_endereco
        // Logradouro = line1
        // Complemento = line3
        public Endereco Endereco { get; set; }
        [LogicalAttribute("new_guid_endereco")]
        public Guid EnderecoId { get; set; } 

        public PicklistVO TipoOcorrencia { get; set; } // casetypecode
        [LogicalAttribute("new_data_origem")]
        public DateTime DataOrigem { get; set; } // new_data_origem
        [LogicalAttribute("new_data_hora_prevista_visita")]
        public DateTime DataPrevistaParaVisita { get; set; } // new_data_hora_prevista_visita
    }
}
