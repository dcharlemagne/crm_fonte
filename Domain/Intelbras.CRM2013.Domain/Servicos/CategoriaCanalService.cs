using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using System.Text.RegularExpressions;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class CategoriaCanalService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public CategoriaCanalService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public CategoriaCanalService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public CategoriaCanalService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Métodos

        public List<CategoriasCanal> ListarPor(Guid? canalId, Guid? unidadenegocioId)
        {
            return RepositoryService.CategoriasCanal.ListarPor(canalId, unidadenegocioId, null, null, null);
        }
        public List<CategoriasCanal> ListarPor(Conta canal)
        {
            return RepositoryService.CategoriasCanal.ListarPor(canal);
        }

        public Lookup ObterCategoriaPrincipalDoCanal(Conta canal)
        {
            List<CategoriasCanal> listaCategorias = RepositoryService.CategoriasCanal.ListarPor(canal);
            if (listaCategorias.Count > 0)
            {
                CategoriasCanal categoriaPrincipal = listaCategorias.First();

                if (categoriaPrincipal.Categoria.Name == "Ouro")
                    return categoriaPrincipal.Categoria;

                ObterCategoriaPrincipal(listaCategorias, ref categoriaPrincipal);

                return categoriaPrincipal.Categoria;
            }
            else
            {
                return null;
            }

        }

        public CategoriasCanal ObterPor(Guid CategoriaCanalId)
        {
            return RepositoryService.CategoriasCanal.ObterPor(CategoriaCanalId);
        }

        private void ObterCategoriaPrincipal(List<CategoriasCanal> listaCategoriasDoCanal, ref CategoriasCanal categoriaPrincipal)
        {
            foreach (var item in listaCategoriasDoCanal)
                item.Categoria.Name = EnumerarCategoria(item.Categoria.Name);

            var IsNumber = new Regex("^[0-9]");
            foreach (var novo in listaCategoriasDoCanal)
            {
                if (IsNumber.IsMatch(novo.Categoria.Name))
                {
                    if (Convert.ToInt32(categoriaPrincipal.Categoria.Name) < Convert.ToInt32(novo.Categoria.Name))
                    {
                        categoriaPrincipal = novo;
                    }
                }
            }

            categoriaPrincipal.Categoria.Name = EnumerarCategoria(categoriaPrincipal.Categoria.Name, true);
        }

        private string EnumerarCategoria(string categoria, bool reclassificar = false)
        {
            if (reclassificar)
                return categoria == "4" ? "Ouro" :
                    categoria == "3" ? "Prata" :
                        categoria == "2" ? "Bronze" :
                            categoria == "1" ? "Registrado" :
                                categoria;

            return categoria == "Ouro" ? "4" :
                    categoria == "Prata" ? "3" :
                        categoria == "Bronze" ? "2" :
                            categoria == "Registrado" ? "1" :
                                categoria;
        }

        /// <summary>
        /// Copia as categorias da Matriz para a Filial
        /// </summary>
        /// <param name="matrizId"></param>
        /// <param name="filialId"></param>
        public void CopiarCategoriasDoCanalDaMatrizParaFilial(Guid matrizId, Conta filial)
        {
            List<CategoriasCanal> lstCatCanalMatriz = RepositoryService.CategoriasCanal.ListarPor(matrizId, null, null, null, null);
            List<CategoriasCanal> lstCatCanalFilial = RepositoryService.CategoriasCanal.ListarPor(filial);

            foreach (CategoriasCanal catCanal in lstCatCanalMatriz)
            {
                var catFilial = lstCatCanalFilial.Find(c => c.Categoria.Id == catCanal.Categoria.Id && c.UnidadeNegocios.Id == catCanal.UnidadeNegocios.Id);

                if (catFilial == null)
                {
                    catCanal.ID = null;
                    catCanal.Canal.Id = filial.Id;
                    catCanal.Id = Guid.Empty;

                    RepositoryService.CategoriasCanal.Create(catCanal);
                }
                else
                {
                    RepositoryService.CategoriasCanal.AtualizarStatus(catFilial.Id, (int)Enum.CategoriaCanal.StateCode.Ativado, (int)Enum.CategoriaCanal.StatusCode.Ativo);
                }
            }
        }

        /// <summary>
        /// Verifica se o canal da categoria de canal
        /// </summary>
        /// <param name="categoriasCanal"></param>
        /// <returns></returns>
        public bool CanalEhFilialEApuracaoCentralizadaNaMatrizOuMatrizNaoPossuiACategoria(CategoriasCanal categoriasCanal)
        {
            if (categoriasCanal.Canal == null || categoriasCanal.Canal.Id == Guid.Empty)
                return false;

            var canal = this.RepositoryService.Conta.Retrieve(categoriasCanal.Canal.Id);

            if (!canal.TipoConta.HasValue || !canal.ApuracaoBeneficiosCompromissos.HasValue)
                return false;

            if (canal.ParticipantePrograma == (int)Enum.Conta.ParticipaDoPrograma.Sim
                && canal.TipoConta.Value == (int)Enum.Conta.MatrizOuFilial.Filial
                && canal.ApuracaoBeneficiosCompromissos.Value == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Centralizada_Na_Matriz)
            {
                // Verifica se Existe a mesma categoria de canal na Matriz
                if (canal.ContaPrimaria != null)
                {
                    var categoriaNaMatriz = this.RepositoryService.CategoriasCanal.ListarPor(canal.ContaPrimaria.Id, categoriasCanal.UnidadeNegocios.Id, categoriasCanal.Classificacao.Id, categoriasCanal.SubClassificacao.Id, categoriasCanal.Categoria.Id);

                    if (categoriaNaMatriz == null || categoriaNaMatriz.Count == 0)
                        return false;
                }
                return true;
            }
            return false;
        }

        public void InativarCategoriaDoCanal(CategoriasCanal categoriaDoCanal)
        {
            if (this.CanalEhFilialEApuracaoCentralizadaNaMatrizOuMatrizNaoPossuiACategoria(categoriaDoCanal))
                return;

            var canal = this.RepositoryService.Conta.Retrieve(categoriaDoCanal.Canal.Id);

            // Se Matriz e Apuração centralizada na Matriz
            if ((canal.TipoConta.HasValue && canal.TipoConta.Value == (int)Enum.Conta.MatrizOuFilial.Matriz)
                && (canal.ApuracaoBeneficiosCompromissos.HasValue && canal.ApuracaoBeneficiosCompromissos.Value == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Centralizada_Na_Matriz))
            {
                this.InativarBeneficiosECompromissosPorCanaleUnidadeDeNegocios(categoriaDoCanal, canal);

                // Inativa a categoria nas Filiais.
                var lstFilias = this.RepositoryService.Conta.ListarContasFiliaisPorMatriz(canal.ID.Value);

                foreach (var filial in lstFilias)
                {
                    var lstCategoriasDaFilial = this.ListarPor(filial.ID.Value, categoriaDoCanal.UnidadeNegocios.Id);

                    foreach (var categoriaDaFilial in lstCategoriasDaFilial)
                    {
                        if (categoriaDaFilial.Status.Value == (int)Enum.CategoriaCanal.StateCode.Ativado)
                        {
                            this.RepositoryService.CategoriasCanal.AtualizarStatus(categoriaDaFilial.ID.Value, (int)Enum.CategoriaCanal.StateCode.Desativado, (int)Enum.CategoriaCanal.StatusCode.Inativo);
                        }
                    }
                }

                return;
            }

            // Se Matriz e Apuração por filial
            if ((canal.TipoConta.HasValue && canal.TipoConta.Value == (int)Enum.Conta.MatrizOuFilial.Matriz)
                && (canal.ApuracaoBeneficiosCompromissos.HasValue && canal.ApuracaoBeneficiosCompromissos.Value == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Por_Filiais))
            {
                this.InativarBeneficiosECompromissosPorCanaleUnidadeDeNegocios(categoriaDoCanal, canal);
            }

            // Se Filial e Apuração por filial
            if ((canal.TipoConta.HasValue && canal.TipoConta.Value == (int)Enum.Conta.MatrizOuFilial.Filial)
                && (canal.ApuracaoBeneficiosCompromissos.HasValue && canal.ApuracaoBeneficiosCompromissos.Value == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Por_Filiais))
            {
                this.InativarBeneficiosECompromissosPorCanaleUnidadeDeNegocios(categoriaDoCanal, canal);
            }
        }

        private void InativarBeneficiosECompromissosPorCanaleUnidadeDeNegocios(CategoriasCanal categoriaDoCanal, Conta canal)
        {
            var lstBeneficiosDoCanal = this.RepositoryService.BeneficioDoCanal.ListarPorContaUnidadeNegocio(canal.ID.Value, categoriaDoCanal.UnidadeNegocios.Id);
            var lstCompromissosDoCanal = this.RepositoryService.CompromissosDoCanal.ListarPorContaUnidade(canal.ID.Value, categoriaDoCanal.UnidadeNegocios.Id);

            foreach (var beneficioDoCanal in lstBeneficiosDoCanal)
            {
                if (!beneficioDoCanal.VerbaBrutaPeriodoAtual.HasValue || beneficioDoCanal.VerbaBrutaPeriodoAtual.Value == 0)
                {
                    if (beneficioDoCanal.Status.Value == (int)Enum.BeneficioDoCanal.StateCode.Ativo)
                        this.RepositoryService.BeneficioDoCanal.AtualizarStatus(beneficioDoCanal.ID.Value, (int)Enum.BeneficioDoCanal.StateCode.Inativo, (int)Enum.BeneficioDoCanal.StatusCode.Inativo);
                }
                else
                {
                    beneficioDoCanal.AcumulaVerba = false;
                    this.RepositoryService.BeneficioDoCanal.Update(beneficioDoCanal);
                }
            }

            foreach (var compromissoDoCanal in lstCompromissosDoCanal)
            {
                if (compromissoDoCanal.Status == (int)Enum.CompromissoCanal.Status.Ativo)
                {
                    this.RepositoryService.CompromissosDoCanal.AtualizarStatus(compromissoDoCanal.ID.Value, (int)Enum.CompromissoCanal.StateCode.Inativo, (int)Enum.CompromissoCanal.Status.Desativado);
                }
            }

        }

        public void InativarCategoriaDoCanal(Conta canal)
        {
            var lstCategoriasDoCanal = this.RepositoryService.CategoriasCanal.ListarPor(canal.ID.Value, null, null, null, null);
            foreach (var categoriaDoCanal in lstCategoriasDoCanal)
            {
                if (categoriaDoCanal.Status.Value == (int)Enum.CategoriaCanal.StateCode.Ativado)
                {
                    this.RepositoryService.CategoriasCanal.AtualizarStatus(categoriaDoCanal.ID.Value, (int)Enum.CategoriaCanal.StateCode.Desativado, (int)Enum.CategoriaCanal.StatusCode.Inativo);
                }
            }
        }
        public CategoriasCanal AtivarCategoriaDoCanal(Conta conta)
        {
            List<CategoriasCanal> categorias = this.RepositoryService.CategoriasCanal.ListarPor(conta);
            foreach (var c in categorias)
            {
                if (c.UnidadeNegocios.Name.Contains("ADMINISTRATIVO"))
                {
                    this.RepositoryService.CategoriasCanal.AtualizarStatus(c.ID.Value, (int)Enum.CategoriaCanal.StateCode.Ativado, (int)Enum.CategoriaCanal.StatusCode.Ativo);
                    c.Categoria.Id = conta.Categoria.Id;
                    c.RazaoStatus = (int)Enum.CategoriaCanal.StatusCode.Ativo;
                    c.Status = (int)Enum.CategoriaCanal.StateCode.Ativado;
                    this.RepositoryService.CategoriasCanal.Update(c);
                    return c;
                }
            }
            return null;
        }

        #endregion
    }
}