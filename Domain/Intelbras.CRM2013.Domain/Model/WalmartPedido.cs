using System;
using System.Xml.Serialization;


namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [XmlRoot(ElementName = "Pedido")]
    public class WalmartPedido
    {
        public int IdPedido { get; set; }

        public string CNPJClienteB2B { get; set; }

        public decimal ValorPago { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorTotalPedido { get; set; }

        public string Status { get; set; }

        public string CodigoStatus { get; set; }

        public int? PrazoEntrega { get; set; }

        public string DataEntrega { get; set; }

        public string DataAgendada { get; set; }

        public string TurnoAgendado { get; set; }

        [XmlArray("Itens")]
        [XmlArrayItem("ItemPedido", typeof(WalmartItemPedido))]
        public WalmartItemPedido[] ItemPedido { get; set; }
        
        public WalmartClientePedido Cliente { get; set; }

        public WalmartEnderecoPedido EnderecoEntrega { get; set; }

        [XmlIgnore]
        public Enum.Resgate.RazaoDoStatus StatusResgate
        {
            get
            {
                switch(this.CodigoStatus)
                {
                    case "_CAM":
                    case "_CANM":
                    case "_CANNM":
                    case "AVT":
                    case "CAN":
                    case "CLN":
                    case "MDP":
                    case "NLC":
                    case "PFE":
                    case "PNA":
                    case "QPE":
                    case "REC":
                    case "RRP":
                        return Enum.Resgate.RazaoDoStatus.Cancelado;

                    case "ENT":
                    case "_EIS":
                    case "CEC":
                    case "_CEV":
                    case "FCE":
                    case "IVE":
                        return Enum.Resgate.RazaoDoStatus.Finalizado;

                    default:
                        return Enum.Resgate.RazaoDoStatus.Andamento;
                }
            }
        }

        public override string ToString()
        {
            string newLine = "\n";

            var texto = "DADOS DO PEDIDO";
            texto += newLine + "CPF/CNPJ do Cliente: " + CNPJClienteB2B;
            texto += newLine + "Status: " + Status;
            texto += newLine + "Prazo de Entrega: " + (PrazoEntrega.HasValue ? PrazoEntrega.Value.ToString() : "");
            texto += newLine + "Data Entrega: " + DataEntrega;
            texto += newLine + "Data Agendada: " + DataAgendada;
            texto += newLine + "Turno Agendado: " + TurnoAgendado;

            if (Cliente != null)
            {
                texto += newLine + newLine + "DADOS DO CLIENTE";
                texto += newLine + "Nome Completo: " + Cliente.NomeCompleto;
                texto += newLine + "CPF: " + Cliente.Cpf;
                texto += newLine + "Data Nascimento: " + Cliente.DataNascimento;
                texto += newLine + "Email: " + Cliente.Email;
                texto += newLine + "Genero: " + Cliente.Genero;
                texto += newLine + "Telefone Celular: " + Cliente.TelefoneCelular;
                texto += newLine + "Telefone Residencial: " + Cliente.TelefoneResidencial;
            }

            if (EnderecoEntrega != null)
            {
                texto += newLine + newLine + "DADOS DO ENDEREÇO";
                texto += newLine + "Logradouro: " + EnderecoEntrega.Logradouro;
                texto += newLine + "Número: " + EnderecoEntrega.Numero;
                texto += newLine + "Complemente: " + EnderecoEntrega.Complemento;
                texto += newLine + "Ponto de Referência: " + EnderecoEntrega.PontoReferencia;
                texto += newLine + "Bairro: " + EnderecoEntrega.Bairro;
                texto += newLine + "Cidade: " + EnderecoEntrega.Cidade;
                texto += newLine + "Estado: " + EnderecoEntrega.Estado;
                texto += newLine + "Nome do Destinatário: " + EnderecoEntrega.NomeDestinatario;
                texto += newLine + "Tipo do Logradouro: " + EnderecoEntrega.TipoLogradouro;
            }

            foreach (var item in ItemPedido) {
                texto += newLine + newLine + "DADOS DO ITEM";
                texto += newLine + "IdSku: " + item.IdSku;
                texto += newLine + "Nome: " + item.Nome;
                texto += newLine + "Prazo de Entrega: " + item.PrazoEntrega;
                texto += newLine + "Preço Venda: " + item.PrecoVenda;
                texto += newLine + "Quantidade de Pontos: " + item.QtdPontos;
                texto += newLine + "Quantidade: " + item.Quantidade;
                texto += newLine + "Status: " + item.Status;
                texto += newLine + "Embalar Presente: " + item.EmbalarPresente;
                texto += newLine + "Valor EmbalagemPresente: " + (item.ValorEmbalagemPresente.HasValue ? item.ValorEmbalagemPresente.Value.ToString() : "");
            }

            return texto;
        }
    }
}
