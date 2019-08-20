using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class PoliticaComercialService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PoliticaComercialService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public PoliticaComercialService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        public bool VerificarExistenciaPoliticaComercial(PoliticaComercial politicaComercial)
        {

            List<PoliticaComercial> lstPoliticaComercial;

            if (politicaComercial.AplicarPoliticaPara.Value == (int)Enum.PoliticaComercial.AplicarPolíticaPara.PerfilDeCanais)
            {
                if (politicaComercial.Estabelecimento == null || !politicaComercial.ID.HasValue
                    || politicaComercial.UnidadeNegocio == null || politicaComercial.Classificacao == null || politicaComercial.Categoria == null
                    || !politicaComercial.TipoDePolitica.HasValue || !politicaComercial.DataInicio.HasValue || !politicaComercial.DataFim.HasValue)
                    throw new ArgumentException("Campos incompletos no formulário, preencha corretamente.");

                lstPoliticaComercial = RepositoryService.PoliticaComercial.ListarPor(politicaComercial.Estabelecimento.Id, politicaComercial.ID, politicaComercial.UnidadeNegocio.Id, politicaComercial.Classificacao.Id, politicaComercial.Categoria.Id, politicaComercial.TipoDePolitica.Value, politicaComercial.DataInicio.Value, politicaComercial.DataFim.Value);

                if (lstPoliticaComercial.Count() > 0)
                    return true;
            }

            return false;
        }

        public List<Guid> ListarEstadosDaPoliticaComercial(Guid politicaComercialId)
        {
            List<Guid> lstRetorno = new List<Guid>();
            List<PoliticaComercialXEstado> lstPoliticaEstado = RepositoryService.PoliticaComercialXEstado.ListarPor(politicaComercialId, null);

            if (lstPoliticaEstado.Count == 0)
                return lstRetorno;

            foreach (var item in lstPoliticaEstado)
            {
                if (item.EstadoId.HasValue)
                    lstRetorno.Add(item.EstadoId.Value);
            }
            return lstRetorno;
        }

        public List<Guid> ListarCanaisDaPoliticaComercial(Guid politicaComercialId)
        {
            List<Guid> lstRetorno = new List<Guid>();
            List<PoliticaComercialXConta> lstPoliticaConta = RepositoryService.PoliticaComercialXConta.ListarPor(politicaComercialId, null);

            if (lstPoliticaConta.Count == 0)
                return lstRetorno;

            foreach (var item in lstPoliticaConta)
            {
                if (item.ContaId.HasValue)
                    lstRetorno.Add(item.ContaId.Value);
            }
            return lstRetorno;
        }

        public List<ProdutoPoliticaComercial> ListarProdutoDaPoliticaComercial(Guid politicaComercialId)
        {
            List<ProdutoPoliticaComercial> lstProdutoPolitica = RepositoryService.ProdutoPoliticaComercial.ListarPor(politicaComercialId);

            return lstProdutoPolitica;
        }

        public List<FamiliaPoliticaComercial> ListarFamiliaDaPoliticaComercial(Guid politicaComercialId)
        {
            List<FamiliaPoliticaComercial> lstFamiliaPolitica = RepositoryService.FamiliaPoliticaComercial.ListarTodas();

            return lstFamiliaPolitica;
        }

        public PoliticaComercial ObterPor(Guid id)
        {
            return RepositoryService.PoliticaComercial.ObterPor(id);
        }

        /// <summary>
        /// Retorna true para registro duplicado e false para nao
        /// </summary>
        /// <param name="IgnorarPoliticaPropria">True ignora o uso da politica propria e false não ignora</param>
        /// <returns></returns>
        public bool VerificarDuplicidadePoliticaRegistros(PoliticaComercial politicaComercial, List<Guid> lstRegistros, string tipoDuplicidade, Boolean IgnorarPoliticaPropria)
        {
            Guid? politicaId = IgnorarPoliticaPropria ? null : politicaComercial.ID;

            switch (tipoDuplicidade)
            {
                case "estado":
                    if (!politicaComercial.TipoDePolitica.HasValue || !politicaComercial.AplicarPoliticaPara.HasValue || politicaComercial.Estabelecimento == null ||
                        politicaComercial.UnidadeNegocio == null || !politicaComercial.DataInicio.HasValue || !politicaComercial.DataFim.HasValue || politicaComercial.Classificacao == null || politicaComercial.Categoria == null || lstRegistros.Count == 0)
                        throw new ArgumentException("(CRM) Dados incompletos.Necessário o preenchimento dos campos Tipo de Política,Aplicar Política Para,Estabelecimento,Unidade de Negócio,Classificação,Categoria");

                    if (RepositoryService.PoliticaComercial.ListarPorEstado(politicaId, politicaComercial.TipoDePolitica.Value, politicaComercial.AplicarPoliticaPara.Value, politicaComercial.Estabelecimento.Id, politicaComercial.UnidadeNegocio.Id, politicaComercial.Classificacao.Id, politicaComercial.Categoria.Id, lstRegistros, politicaComercial.DataInicio.Value, politicaComercial.DataFim.Value).Count > 0)
                        return true;

                    break;
                case "conta":
                    if (!politicaComercial.TipoDePolitica.HasValue || !politicaComercial.AplicarPoliticaPara.HasValue || politicaComercial.Estabelecimento == null ||
                        politicaComercial.UnidadeNegocio == null || !politicaComercial.DataInicio.HasValue || !politicaComercial.DataFim.HasValue || lstRegistros.Count == 0)
                        throw new ArgumentException("(CRM) Dados incompletos.Necessário o preenchimento dos campos Tipo de Política,Aplicar Política Para,Estabelecimento,Unidade de Negócio");

                    if (RepositoryService.PoliticaComercial.ListarPor(politicaId, politicaComercial.TipoDePolitica.Value, politicaComercial.AplicarPoliticaPara.Value, politicaComercial.Estabelecimento.Id, politicaComercial.UnidadeNegocio.Id, lstRegistros, politicaComercial.DataInicio.Value, politicaComercial.DataFim.Value).Count > 0)
                        return true;

                    break;
            }
            return false;
        }
        public bool VerificarIntervaloQtd(ProdutoPoliticaComercial produtoPoliticaComercial)
        {

            List<ProdutoPoliticaComercial> lstProdutoPoliticaComercial = RepositoryService.ProdutoPoliticaComercial.ListarPor(produtoPoliticaComercial.PoliticaComercial.Id, produtoPoliticaComercial.Produto.Id, produtoPoliticaComercial.ID, produtoPoliticaComercial.DataInicioVigencia.Value, produtoPoliticaComercial.DataFimVigencia.Value, produtoPoliticaComercial.QtdInicial.Value, produtoPoliticaComercial.QtdFinal.Value);

            foreach (ProdutoPoliticaComercial prod in lstProdutoPoliticaComercial)
            {
                if (NoIntervalo(produtoPoliticaComercial.QtdInicial, prod.QtdInicial, prod.QtdFinal))
                {
                    return true;
                }

                if (NoIntervalo(produtoPoliticaComercial.QtdFinal, prod.QtdInicial, prod.QtdFinal))
                {
                    return true;
                }

                if (NoIntervalo(prod.QtdInicial, produtoPoliticaComercial.QtdInicial, produtoPoliticaComercial.QtdFinal))
                {
                    return true;
                }

                if (NoIntervalo(prod.QtdFinal, produtoPoliticaComercial.QtdInicial, produtoPoliticaComercial.QtdFinal))
                {
                    return true;
                }
            }

            return false;
        }

        public bool NoIntervalo(int? valor, int? minimo, int? maximo)
        {
            if (!valor.HasValue)
                valor = 0;

            if (!minimo.HasValue)
                minimo = 0;

            if (!maximo.HasValue)
                maximo = 0;

            return (minimo.Value <= valor.Value && valor.Value <= maximo.Value);
        }

        public void InsereProdutosporFamiliaComercial(string politicaComercialId, string familiasIds, int qtdIni, int qtdFin, double Fator)
        {
            try
            {
                FamiliaComercialService fcs = new FamiliaComercialService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                List<Product> produtos = fcs.ProdutosPorFamilias(familiasIds);

                PoliticaComercial InfoPoliticaComercial = RepositoryService.PoliticaComercial.Retrieve(new Guid(politicaComercialId));

                foreach (Product produto in produtos)
                {
                    ProdutoPoliticaComercial prodPolCom = new ProdutoPoliticaComercial(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);

                    prodPolCom.Produto = new Lookup((Guid)produto.ID, "");
                    prodPolCom.QtdInicial = qtdIni;
                    prodPolCom.QtdFinal = qtdFin;
                    prodPolCom.Nome = InfoPoliticaComercial.Nome + " - " + produto.Nome;
                    if (produto.FamiliaProduto != null)
                        prodPolCom.FamiliaProduto = new Lookup(produto.FamiliaProduto.Id, "");

                    prodPolCom.PoliticaComercial = new Lookup(new Guid(politicaComercialId), "");
                    prodPolCom.Fator = Fator;
                    prodPolCom.DataInicioVigencia = InfoPoliticaComercial.DataInicio;
                    prodPolCom.DataFimVigencia = InfoPoliticaComercial.DataFim;



                    RepositoryService.ProdutoPoliticaComercial.Create(prodPolCom);
                }
            }

            catch (Exception ex)
            {
                //trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "Account", DateTime.Now));
                //trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new ArgumentException(ex.Message);
            }
        }

        public void Copiar(string organizationName, bool copiarProdutos, bool copiarEstados, DateTime dataInicialVigencia, DateTime dataFinalVigencia, string[] lstRegitrosId)
        {
            PoliticaComercialService politicaDomainService = new PoliticaComercialService(organizationName, false);
            foreach (var id in lstRegitrosId)
            {
                var guidPoliticaAntiga = new Guid(id);
                var politicaTmp = politicaDomainService.ObterPor(guidPoliticaAntiga);

                politicaTmp.ID = null;
                politicaTmp.Id = Guid.Empty;
                politicaTmp.DataInicio = dataInicialVigencia;
                politicaTmp.DataFim = dataFinalVigencia;
                politicaTmp.Status = (int)Enum.PoliticaComercial.Status.Ativo;
                politicaTmp.RazaoStatus = (int)Enum.PoliticaComercial.RazaoStatus.Rascunho;

                Guid newPoliticaGuid = RepositoryService.PoliticaComercial.Create(politicaTmp);

                var listaCanaisPolitica = politicaDomainService.ListarCanaisDaPoliticaComercial(guidPoliticaAntiga);

                foreach (var canaisTmp in listaCanaisPolitica)
                {
                    RepositoryService.PoliticaComercial.CriarAssociacaoCanal(newPoliticaGuid, canaisTmp);
                }

                if (copiarEstados)
                {
                    var listaEstados = politicaDomainService.ListarEstadosDaPoliticaComercial(guidPoliticaAntiga);

                    foreach (var estadoTmp in listaEstados)
                    {
                        RepositoryService.PoliticaComercial.CriarAssociacaoEstado(newPoliticaGuid, estadoTmp);
                    }
                }

                if (copiarProdutos)
                {
                    var listaProdutoPoliticaComercial = politicaDomainService.ListarProdutoDaPoliticaComercial(guidPoliticaAntiga);

                    foreach (var produtoPoliticaTmp in listaProdutoPoliticaComercial)
                    {
                        produtoPoliticaTmp.ID = null;
                        produtoPoliticaTmp.Id = Guid.Empty;
                        produtoPoliticaTmp.PoliticaComercial = new Lookup(newPoliticaGuid, "");
                        produtoPoliticaTmp.DataInicioVigencia = dataInicialVigencia;
                        produtoPoliticaTmp.DataFimVigencia = dataFinalVigencia;

                        RepositoryService.ProdutoPoliticaComercial.Create(produtoPoliticaTmp);
                    }


                    var listaFamiliaPoliticaComercial = politicaDomainService.ListarFamiliaDaPoliticaComercial(guidPoliticaAntiga);

                    foreach (var familiaPoliticaTmp in listaFamiliaPoliticaComercial)
                    {
                        familiaPoliticaTmp.ID = null;
                        familiaPoliticaTmp.Id = Guid.Empty;
                        familiaPoliticaTmp.PoliticaComercial = new Lookup(newPoliticaGuid, "");
                        familiaPoliticaTmp.DataInicial = dataInicialVigencia;
                        familiaPoliticaTmp.DataFinal = dataFinalVigencia;

                        RepositoryService.FamiliaPoliticaComercial.Create(familiaPoliticaTmp);
                    }
                }
            }
        }

    }
}
