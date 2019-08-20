using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class RetornaGuid : Base
    {

        #region Propriedades

        #endregion

        #region Construtor

        public RetornaGuid(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }
        #endregion


        public Guid UnidadeNegocio(string codigoUN, ref Pollux.Entities.Resultado resultadoPersistencia)
        {
            UnidadeNegocio unidadeNegocio = new Intelbras.CRM2013.Domain.Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocioPorChaveIntegracao(codigoUN);

            if (unidadeNegocio != null)
            {
                //crm.UnidadeNegocio = new Lookup((Guid)unidadeNegocio.ID, "");
                return (Guid)unidadeNegocio.ID;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador da Unidade de Negócio não encontrado.";
                return Guid.Empty;
            }
        }

        public Guid GrupoEstoque(int codigoGrupo, ref Pollux.Entities.Resultado resultadoPersistencia)
        {
            GrupoEstoque grupoEstoque = new Intelbras.CRM2013.Domain.Servicos.GrupoEstoqueService(this.Organizacao, this.IsOffline).ObterPor(codigoGrupo);

            if (grupoEstoque != null)
            {
                //crm.GrupoEstoque = new Lookup((Guid)grupoEstoque.ID, "");
                return (Guid)grupoEstoque.ID;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador da Grupo de Estoque não encontrado.";
                return Guid.Empty;
            }
        }

        public Guid Segmento(string codigoSegmento, ref Pollux.Entities.Resultado resultadoPersistencia)
        {
            Model.Segmento segmento = new Intelbras.CRM2013.Domain.Servicos.SegmentoService(this.Organizacao, this.IsOffline).BuscaSegmento(codigoSegmento);

            if (segmento != null && segmento.ID.HasValue)
            {
                //crm.Segmento = new Lookup(segmento.ID.Value, "");
                return (Guid)segmento.ID;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Segmento não encontrado!";
                return Guid.Empty;
            }
        }

        internal Guid FamiliaProduto(string codigoFamiliaProduto, ref Pollux.Entities.Resultado resultadoPersistencia)
        {
            Model.FamiliaProduto familia = new Intelbras.CRM2013.Domain.Servicos.FamiliaProdutoService(this.Organizacao, this.IsOffline).BuscaFamiliaProduto(codigoFamiliaProduto);

            if (familia != null && familia.ID.HasValue)
            {
                //crm.Segmento = new Lookup(segmento.ID.Value, "");
                return (Guid)familia.ID;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Família do Produto não encontrada!";
                return Guid.Empty;
            }
        }

        internal Guid SubfamiliaProduto(string codigoSubfamiliaProduto, ref Pollux.Entities.Resultado resultadoPersistencia)
        {
            Model.SubfamiliaProduto subfamilia = new Intelbras.CRM2013.Domain.Servicos.SubFamiliaProdutoService(this.Organizacao, this.IsOffline).BuscaSubfamiliaProduto(codigoSubfamiliaProduto);

            if (subfamilia != null && subfamilia.ID.HasValue)
            {
                //crm.Segmento = new Lookup(segmento.ID.Value, "");
                return (Guid)subfamilia.ID;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Subfamília do Produto não encontrada!";
                return Guid.Empty;
            }
        }

        internal Guid Origem(string codigoOrigem, ref Pollux.Entities.Resultado resultadoPersistencia)
        {
            Model.Origem origem = new Intelbras.CRM2013.Domain.Servicos.OrigemService(this.Organizacao, this.IsOffline).BuscaOrigem(codigoOrigem);

            if (origem != null && origem.ID.HasValue)
            {
                //crm.Segmento = new Lookup(segmento.ID.Value, "");
                return (Guid)origem.ID;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Origem não encontrada!";
                return Guid.Empty;
            }
        }

        internal Guid UnidadeMedida(string nomeUnidadeMedida, ref Pollux.Entities.Resultado resultadoPersistencia)
        {
            Model.Unidade unidadeMedida = new Intelbras.CRM2013.Domain.Servicos.UnidadeService(this.Organizacao, this.IsOffline).BuscaUnidadePorNome(nomeUnidadeMedida);

            if (unidadeMedida != null && unidadeMedida.ID.HasValue)
            {
                //crm.Segmento = new Lookup(segmento.ID.Value, "");
                return (Guid)unidadeMedida.ID;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Unidade de Medida não encontrada!";
                return Guid.Empty;
            }
        }

        internal Guid GrupoUnidadeMedida(string nomeGrupoUM, ref Pollux.Entities.Resultado resultadoPersistencia)
        {
            Model.GrupoUnidade grupoUM = new Intelbras.CRM2013.Domain.Servicos.GrupoUnidadeMedidaService(this.Organizacao, this.IsOffline).ObterPor(nomeGrupoUM);

            if (grupoUM != null && grupoUM.ID.HasValue)
            {
                return (Guid)grupoUM.ID;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Grupo de Unidade de Medida não encontrado!";
                return Guid.Empty;
            }
        }

        internal Guid FamiliaMaterial(string codigoFamiliaMaterial, ref Pollux.Entities.Resultado resultadoPersistencia)
        {
            Model.FamiliaMaterial familiamat = new Intelbras.CRM2013.Domain.Servicos.FamiliaMaterialService(this.Organizacao, this.IsOffline).ObterPor(codigoFamiliaMaterial);

            if (familiamat != null && familiamat.ID.HasValue)
            {
                return (Guid)familiamat.ID;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Família do Material não encontrada!";
                return Guid.Empty;
            }
        }

        internal Guid FamiliaComercial(string codigoFamiliaComercial, ref Pollux.Entities.Resultado resultadoPersistencia)
        {
            Model.FamiliaComercial familiaCom = new Intelbras.CRM2013.Domain.Servicos.FamiliaComercialService(this.Organizacao, this.IsOffline).ObterPor(codigoFamiliaComercial);

            if (familiaCom != null && familiaCom.ID.HasValue)
            {
                return (Guid)familiaCom.ID;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Família Comercial não encontrada!";
                return Guid.Empty;
            }
        }

        internal Guid ListaPreco(string nomeLista, ref Pollux.Entities.Resultado resultadoPersistencia)
        {
            Model.ListaPreco listaPreco = new Intelbras.CRM2013.Domain.Servicos.ListaPrecoService(this.Organizacao, this.IsOffline).BuscaListaPreco(nomeLista);

            if (listaPreco != null && listaPreco.ID.HasValue)
            {
                return (Guid)listaPreco.ID;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Lista de Preços não encontrada!";
                return Guid.Empty;
            }
        }

        internal Guid Moeda(string codigoMoeda, ref Pollux.Entities.Resultado resultadoPersistencia)
        {
            Moeda ObjMoeda = new Intelbras.CRM2013.Domain.Servicos.MoedaService(this.Organizacao, this.IsOffline).BuscaMoedaPorCodigo(codigoMoeda);

            if (ObjMoeda != null)
            {
                return (Guid)ObjMoeda.ID;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador da Moeda não encontrado.";
                return Guid.Empty;
            }
        }
    }
}
