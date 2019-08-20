using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    /// <summary>
    /// Autor: Marcelo Ferreira de Láias (MSBS Tridea)
    /// Data: 05/09/2011
    /// Descrição: Classe para dados de endereço 
    /// </summary>
    [Serializable]
    public class EnderecoXML
    {
        /// <summary>
        /// Nome do Remetente, Cliente ou Razão Social
        /// </summary>
        public string Nome { get; set; }

        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }
        public string Referencia { get; set; }
        public string DDD { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
    }
}
