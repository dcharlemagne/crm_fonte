using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    /// <summary>
    /// Autor: Marcelo Ferreira de Láias (MSBS Tridea)
    /// Data: 05/09/2011
    /// Descrição: Classe para gerar o XML com base no leiaute do arquivo versão 4.0 que é utilizado para cadastro de
    /// pedidos de coleta e autorização de postagem para o serviço de Logística Reversa.
    /// </summary> 
    [Serializable]
    public class SolicitacaoLogisticaReversaXML : DomainBase
    {

        #region Atributos

        /// <summary>
        /// Esse valor será fixo e definido pela ECT
        /// </summary>
        public string Reg { get { return "LRP0584X"; } }

        /// <summary>
        /// Identifica a versão do arquivo XML. Para esta versão deverá ser preenchido: 4.0
        /// </summary>
        public string VersaoArquivo { get { return "4.0"; } }

        /// <summary>
        /// Data para agendar o processamento do arquivo. Se informada o sistema processa o arquivo apenas na data indicada.
        /// </summary>
        public DateTime DataProcessamento { get; set; }

        /// <summary>
        /// Data de agendamento de coleta de todos os pedidos do arquivo. O sistema aceita apenas datas com mais de cinco dias
        /// corridos a partir da data de processamento do arquivo. Caso contrário o processamento do arquivo inteiro será cancelado.
        /// </summary>
        public DateTime Agendamento { get; set; }

        /// <summary>
        /// Código Administrativo do cliente.
        /// </summary>
        public string CodigoAdministrativo { get; set; }

        /// <summary>
        /// Número do contrato do cliente.
        /// </summary>
        public string Contrato { get; set; }

        /// <summary>
        /// Código do serviço que será utilizado. O código
        /// será fornecido pela ECT.
        /// </summary>
        public string CodigoServico { get; set; }

        /// <summary>
        /// Número do cartão de postagem do cliente que será usado para a cobrança das taxas do serviço realizado.
        /// </summary>
        public string Cartao { get; set; }

        private EnderecoXML destinatario;
        public EnderecoXML Destinatario
        {
            get { return destinatario; }
            //set { remetente = value; }
        }

        private List<Solicitacao> coletasSolicitadas;
        public List<Solicitacao> ColetasSolicitadas
        {
            get { return coletasSolicitadas; }
            //set { coletasSolicitadas = value; }
        }

        #endregion

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SolicitacaoLogisticaReversaXML() { }

        public SolicitacaoLogisticaReversaXML(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public SolicitacaoLogisticaReversaXML(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Monta o XML com base no leiaute do arquivo versão 4.0
        /// </summary>
        /// <returns>Retorna um string XML</returns>
        public override string ToString()
        {
            MemoryStream ms = this.GetMemoryStream();
            ms.Position = 0;
            string str = new StreamReader(ms).ReadToEnd();

            return str;
        }

        /// <summary>
        /// Monta o XML com base no leiaute do arquivo versão 4.0
        /// </summary>
        /// <returns>Retorna um string XML</returns>
        public MemoryStream GetMemoryStream()
        {
            //StringWriter memoryStream = new StringWriter();
            MemoryStream memoryStream = new MemoryStream();
            XmlTextWriter xmlWR = new XmlTextWriter(memoryStream, Encoding.GetEncoding("ISO-8859-1"));

            xmlWR.Formatting = Formatting.Indented;
            xmlWR.WriteStartDocument();

            xmlWR.WriteComment(string.Format("Leiaute do arquivo XML versão {0}", this.VersaoArquivo.Trim()));
            xmlWR.WriteComment(string.Format("Gerado a partir da classe {0}", this.GetType().FullName.Trim()));
            xmlWR.WriteComment(string.Format("Criado em: {0}", DateTime.Now.ToString()));
            xmlWR.WriteStartElement("logisticareversa");

            xmlWR.WriteStartElement("reg");
            xmlWR.WriteString(this.Reg.Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("versao_arquivo");
            xmlWR.WriteString(this.VersaoArquivo.Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("data_processamento");
            if (this.DataProcessamento != DateTime.MinValue)
                xmlWR.WriteString(this.DataProcessamento.ToString("dd/MM/yyyy"));
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("agendamento");
            if (this.Agendamento != DateTime.MinValue)
                xmlWR.WriteString(this.Agendamento.ToString("dd/MM/yyyy"));
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("codigo_administrativo");
            xmlWR.WriteString(this.CodigoAdministrativo.ToString().Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("contrato");
            xmlWR.WriteString(this.Contrato.ToString().Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("codigo_servico");
            xmlWR.WriteString(this.CodigoServico.ToString().Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("cartao");
            xmlWR.WriteString(this.Cartao.ToString().Trim());
            xmlWR.WriteEndElement();


            xmlWR.WriteStartElement("destinatario");

            xmlWR.WriteStartElement("nome");
            xmlWR.WriteString(this.destinatario.Nome.PadRight(60, (char)' ').Substring(0, 60).Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("logradouro");
            xmlWR.WriteString(this.destinatario.Logradouro.PadRight(150, (char)' ').Substring(0, 150).Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("numero");
            if (this.destinatario.Numero.Trim() == string.Empty)
                xmlWR.WriteString("S/N");
            else
                xmlWR.WriteString(this.destinatario.Numero.PadRight(8, (char)' ').Substring(0, 8).Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("complemento");
            xmlWR.WriteString(this.destinatario.Complemento.PadRight(30, (char)' ').Substring(0, 30).Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("bairro");
            xmlWR.WriteString(this.destinatario.Bairro.PadRight(50, (char)' ').Substring(0, 50).Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("referencia");
            xmlWR.WriteString(this.destinatario.Referencia.PadRight(60, (char)' ').Substring(0, 60).Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("cidade");
            xmlWR.WriteString(this.destinatario.Cidade.PadRight(36, (char)' ').Substring(0, 36).Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("uf");
            xmlWR.WriteString(this.destinatario.UF.PadRight(2, (char)' ').Substring(0, 2).Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("cep");
            xmlWR.WriteString(this.destinatario.CEP.PadRight(60, (char)' ').Substring(0, 60).Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("ddd");
            xmlWR.WriteString(this.destinatario.DDD.PadRight(3, (char)' ').Substring(0, 3).Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("telefone");
            xmlWR.WriteString(this.destinatario.Telefone.PadRight(12, (char)' ').Substring(0, 12).Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteStartElement("email");
            xmlWR.WriteString(this.destinatario.Email.PadRight(72, (char)' ').Substring(0, 72).Trim());
            xmlWR.WriteEndElement();

            xmlWR.WriteEndElement(); //Fechando tag <destinatario>


            xmlWR.WriteStartElement("coletas_solicitadas");

            foreach (Solicitacao item in this.coletasSolicitadas)
            {
                xmlWR.WriteStartElement("coleta");

                xmlWR.WriteStartElement("tipo");
                xmlWR.WriteString(ObterTipoDeColeta(item.Tipo).Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("numero");
                xmlWR.WriteString(item.Numero.ToString().Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("id_cliente");
                xmlWR.WriteString(item.IdCliente.PadRight(30, (char)' ').Substring(0, 30).Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("ag");
                if (item.AG != DateTime.MinValue)
                    xmlWR.WriteString(item.AG.ToString("dd/MM/yyyy"));
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("cartao");
                xmlWR.WriteString(item.Cartao.ToString().Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("valor_declarado");
                xmlWR.WriteString(item.ValorDeclarado.ToString().Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("servico_adicional");
                xmlWR.WriteString(item.ServicoAdicional.PadRight(20, (char)' ').Substring(0, 20).Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("descricao");
                xmlWR.WriteString(item.Descricao.PadRight(255, (char)' ').Substring(0, 255).Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("ar");
                xmlWR.WriteString(item.AR == true ? "1" : "0");
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("cklist");
                xmlWR.WriteString(item.CKList.Trim());
                xmlWR.WriteEndElement();



                xmlWR.WriteStartElement("remetente");

                xmlWR.WriteStartElement("nome");
                xmlWR.WriteString(item.Remetente.Nome.PadRight(60, (char)' ').Substring(0, 60).Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("logradouro");
                xmlWR.WriteString(item.Remetente.Logradouro.PadRight(150, (char)' ').Substring(0, 150).Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("numero");
                if (item.Remetente.Numero.Trim() == string.Empty)
                    xmlWR.WriteString("S/N");
                else
                    xmlWR.WriteString(item.Remetente.Numero.PadRight(8, (char)' ').Substring(0, 8).Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("complemento");
                xmlWR.WriteString(item.Remetente.Complemento.PadRight(30, (char)' ').Substring(0, 30).Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("bairro");
                xmlWR.WriteString(item.Remetente.Bairro.PadRight(50, (char)' ').Substring(0, 50).Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("cidade");
                xmlWR.WriteString(item.Remetente.Cidade.PadRight(36, (char)' ').Substring(0, 36).Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("uf");
                xmlWR.WriteString(item.Remetente.UF.PadRight(2, (char)' ').Substring(0, 2).Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("cep");
                xmlWR.WriteString(item.Remetente.CEP.PadRight(60, (char)' ').Substring(0, 60).Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("referencia");
                xmlWR.WriteString(item.Remetente.Referencia.PadRight(60, (char)' ').Substring(0, 60).Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("ddd");
                xmlWR.WriteString(item.Remetente.DDD.PadRight(3, (char)' ').Substring(0, 3).Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("telefone");
                xmlWR.WriteString(item.Remetente.Telefone.PadRight(12, (char)' ').Substring(0, 12).Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteStartElement("email");
                xmlWR.WriteString(item.Remetente.Email.PadRight(72, (char)' ').Substring(0, 72).Trim());
                xmlWR.WriteEndElement();

                xmlWR.WriteEndElement(); //Fechando tag <remetente>


                xmlWR.WriteStartElement("obj");

                foreach (ObjetoColeta objItem in item.Objetos)
                {
                    xmlWR.WriteStartElement("obj");


                    xmlWR.WriteStartElement("item");
                    xmlWR.WriteString(objItem.Item.Trim());
                    xmlWR.WriteEndElement();

                    xmlWR.WriteStartElement("id");
                    xmlWR.WriteString(objItem.IdObj.PadRight(30, (char)' ').Substring(0, 30).Trim());
                    xmlWR.WriteEndElement();

                    xmlWR.WriteStartElement("desc");
                    xmlWR.WriteString(objItem.Descricao.PadRight(255, (char)' ').Substring(0, 255).Trim());
                    xmlWR.WriteEndElement();

                    xmlWR.WriteStartElement("entrega");
                    xmlWR.WriteString(objItem.Entrega.ToString().PadRight(13, (char)' ').Substring(0, 13).Trim());
                    xmlWR.WriteEndElement();

                    xmlWR.WriteStartElement("num");
                    xmlWR.WriteString(objItem.Numero.PadRight(13, (char)' ').Substring(0, 13).Trim());
                    xmlWR.WriteEndElement();

                    xmlWR.WriteEndElement(); //Fechando tag <obj>

                }

                xmlWR.WriteEndElement(); //Fechando tag <obj_col>

                xmlWR.WriteStartElement("produto");
                xmlWR.WriteEndElement();

                xmlWR.WriteEndElement(); //Fechando tag <coleta>
            }


            xmlWR.WriteEndElement(); //Fechando tag <coletas_solicitadas>


            xmlWR.WriteEndElement(); //Fechando tag <logisticareversa>
            xmlWR.WriteEndDocument();

            ////xmlWR.Flush();
            ////xmlWR.Close();


            ////var memoryStream = new StreamWriter(ms);
            ////memoryStream.WriteLine("Hello World");
            ////memoryStream.Flush();

            //memoryStream.Position = 0;
            //string str = new StreamReader(memoryStream).ReadToEnd();


            xmlWR.Flush();
            //xmlWR.Close();

            //return str;

            ////return memoryStream.ToString();

            return memoryStream;
        }


        private string ObterTipoDeColeta(Enum.TipoDeSolicitacaoCorreios tipoDeColeta)
        {
            switch (tipoDeColeta)
            {
                case Enum.TipoDeSolicitacaoCorreios.ColetaDomiciliar:
                    return "CA";

                case Enum.TipoDeSolicitacaoCorreios.ColetaDomiciliaria:
                    return "C";

                case Enum.TipoDeSolicitacaoCorreios.AutorizacaoDePostagem:
                    return "A";

                default:
                    return "";
            }
        }

        #endregion
    }
}
