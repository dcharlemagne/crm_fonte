using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Domain.Model
{

    /// <summary>
    /// Autor: Marcelo Ferreira de Láias (MSBS Tridea)
    /// Data: 05/09/2011
    /// Descrição:  Classe que controla o objeto da coleta na logística reversa.
    /// </summary>
    //[LogicalEntity("")] sem entidade no CRM
    public class ObjetoPostal : DomainBase
    {
        private RepositoryService RepositoryService { get; set; }

        public ObjetoPostal() { }

        public ObjetoPostal(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ObjetoPostal(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }


        /// <summary>
        /// Indica se a solicitação é de coleta domiciliária e/ou uma autorização de postagem.
        /// </summary>
        public Enum.TipoDeSolicitacaoCorreios Tipo { get; set; }

        /// <summary>
        /// Campo para preenchimento livre. É um valor para identificação da solicitação junto ao cliente. 
        /// Este valor é enviado no arquivo de retorno gerado após o processamento.
        /// </summary>
        public string IdCliente { get; set; }

        /// <summary>
        /// Número do pedido de coleta se for uma coleta domiciliar ou o
        /// Código da Autorização de Postagem se for uma autorização de
        /// postagem. O número da coleta é composto por 7 algarismos e o código da
        /// autorização de postagem é formado por 9 algarismos.
        /// </summary>
        public string NumeroColeta { get; set; }


        /// <summary>
        /// Registro da etiqueta de cada objeto cadastrado no arquivo de
        /// solicitação (apenas coleta domiciliar). Exemplo: LV123456789BR
        /// </summary>
        public string NumeroEtiqueta { get; set; }

        /// <summary>
        /// Mesmo valor informado na tag <id_obj> do arquivo de
        /// solicitação. Enviados apenas nos pedidos de Coleta Domiciliar.
        /// Não é enviado no arquivo de retorno quando ocorre algum
        /// erro na solicitação.
        /// </summary>
        public string IdObj { get; set; }

        /// <summary>
        /// Indica se o objeto foi cadastrado corretamente.
        /// 01 – cadastrado corretamente
        /// 00 – Erro no cadastro do objeto
        /// </summary>
        public StatusDoObjetoPostal StatusObjeto { get; set; }

        /// <summary>
        /// Coleta Domiciliar:
        /// Prazo para a primeira tentativa de coleta.
        /// Autorização de Postagem:
        /// Data de validade do código de autorização
        /// </summary>
        public DateTime DataProcessamento { get; set; }

        /// <summary>
        /// Data do cadastro do pedido de coleta
        /// </summary>
        public DateTime DataSolicitacao { get; set; }

        /// <summary>
        /// Hora da solicitação da coleta
        /// </summary>
        public DateTime HoraSolicitacao { get; set; }

        /// <summary>
        /// Código do erro caso ocorra durante o cadastramento do pedido
        /// de coleta. Verificar tabela de erros.
        /// </summary>
        public string CodigoErro { get; set; }

        /// <summary>
        /// Descrição do erro caso ocorra.
        /// </summary>
        public string DescricaoErro { get; set; }

    }
}
