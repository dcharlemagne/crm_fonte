using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk;
using SDKore.DomainModel;
using SDKore.Helper;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class BeneficioDoCanalService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public BeneficioDoCanalService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public BeneficioDoCanalService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public BeneficioDoCanalService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion
        public void validaAdesaoaoPrograma(Conta canal)
        {
            if (!canal.ParticipantePrograma.HasValue || canal.ParticipantePrograma.Value != (int)Enum.Conta.ParticipaDoPrograma.Sim)
                return;

            #region  Matriz e Apuração Centralizada na Matriz
            // Se Matriz e Apuração Centralizada na Matriz
            if (canal.TipoConta == (int)Enum.Conta.MatrizOuFilial.Matriz
                && canal.ApuracaoBeneficiosCompromissos == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Centralizada_Na_Matriz)
            {
                List<CategoriasCanal> lstCatCanal = RepositoryService.CategoriasCanal.ListarPor(canal.ID.Value, null, null, null, null);

                // Se o Canal não possui categorias, retorna.
                if (lstCatCanal.Count <= 0)
                    throw new ArgumentException("(CRM) O Canal deve conter Categoria para participar do programa.");
                this.validaParametroGlobal(canal, lstCatCanal);
            }
            #endregion

            #region Se Matriz e Apuração Por Filial
            // Se Matriz e Apuração Por Filial
            if (canal.TipoConta == (int)Enum.Conta.MatrizOuFilial.Matriz
                && canal.ApuracaoBeneficiosCompromissos == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Por_Filiais)
            {
                List<CategoriasCanal> lstCatCanal = RepositoryService.CategoriasCanal.ListarPor(canal.ID.Value, null, null, null, null);

                // Se o Canal não possui categorias, retorna.
                if (lstCatCanal.Count <= 0)
                    throw new ArgumentException("(CRM) O Canal deve conter Categoria para participar do programa.");
                this.validaParametroGlobal(canal, lstCatCanal);
            }
            #endregion

            #region Se Filial e Apuração Centralizada na Matriz
            // Se Filial e Apuração Centralizada na Matriz
            if (canal.TipoConta == (int)Enum.Conta.MatrizOuFilial.Filial
                && canal.ApuracaoBeneficiosCompromissos == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Centralizada_Na_Matriz)
            {
                if (canal.ContaPrimaria == null || canal.ContaPrimaria.Id == Guid.Empty)
                    throw new ArgumentException(string.Format("(CRM) Filial [{0}]-[{1}] não possui Matriz cadastrada.", canal.ID, canal.NomeFantasia));
            }
            #endregion

            #region Se Filial e Apuração Por Filial
            // Se Filial e Apuração Por Filial
            if (canal.TipoConta == (int)Enum.Conta.MatrizOuFilial.Filial
                && canal.ApuracaoBeneficiosCompromissos == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Por_Filiais)
            {
                List<CategoriasCanal> lstCatCanal = RepositoryService.CategoriasCanal.ListarPor(canal.ID.Value, null, null, null, null);

                // Se o Canal não possui categorias, retorna.
                if (lstCatCanal.Count <= 0)
                    throw new ArgumentException("(CRM) O Canal deve conter Categoria para participar do programa.");
                this.validaParametroGlobal(canal, lstCatCanal);
            }
            #endregion

        }

        public void validaParametroGlobal(Conta conta, List<CategoriasCanal> listCategoria)
        {
            foreach (var categoriaDoCanal in listCategoria)
            {
                var parametroGlobalService = new Intelbras.CRM2013.Domain.Servicos.ParametroGlobalService(RepositoryService);

                List<Perfil> lstPerfil = RepositoryService.Perfil.ListarPor(categoriaDoCanal.Classificacao.Id, categoriaDoCanal.UnidadeNegocios.Id, categoriaDoCanal.Categoria.Id, conta.Exclusividade);

                foreach (Perfil perfil in lstPerfil)
                {
                    List<Guid> Compromissos = new List<Guid>();

                    List<BeneficiosCompromissos> lstBeneficiosCompromissos = RepositoryService.BeneficioCompromisso.ListarPor(perfil.ID.Value);

                    foreach (BeneficiosCompromissos beneficiosCompromissos in lstBeneficiosCompromissos)
                    {
                        if (beneficiosCompromissos.Compromisso != null)
                            Compromissos.Add(beneficiosCompromissos.Compromisso.Id);
                    }

                    // Valida parametro Global para os Compromissos
                    foreach (var compromissoId in Compromissos)
                    {
                        var parametroGLobal = parametroGlobalService.ObterFrequenciaAtividadeChecklist(compromissoId);
                        if (parametroGLobal == null || !parametroGLobal.ID.HasValue || parametroGLobal.ID.Value == Guid.Empty || string.IsNullOrEmpty(parametroGLobal.Valor))
                        {
                            var compromisso = this.RepositoryService.CompromissosPrograma.ObterPor(compromissoId);
                            throw new ArgumentException(string.Format("(CRM) Parametro Global Frequencia Checklist não cadastrado para o compromisso [{0}]-[{1}].", compromisso.Nome, compromissoId));
                        }

                        // Atualiza Validade
                        int frequencia = 0;
                        if (!int.TryParse(parametroGLobal.Valor, out frequencia))
                        {
                            var compromisso = this.RepositoryService.CompromissosPrograma.ObterPor(compromissoId);
                            throw new ArgumentException(string.Format("(CRM) Parametro Global Frequencia Checklist inválido para o compromisso [{0}]-[{1}].", compromisso.Nome, compromissoId));
                        }
                    }
                }
            }

        }
        public void AdesaoAoPrograma(Conta canal)
        {
            if (!canal.ParticipantePrograma.HasValue || canal.ParticipantePrograma.Value != (int)Enum.Conta.ParticipaDoPrograma.Sim)
                return;

            #region  Matriz e Apuração Centralizada na Matriz
            // Se Matriz e Apuração Centralizada na Matriz
            if (canal.TipoConta == (int)Enum.Conta.MatrizOuFilial.Matriz
                && canal.ApuracaoBeneficiosCompromissos == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Centralizada_Na_Matriz)
            {
                List<CategoriasCanal> lstCatCanal = RepositoryService.CategoriasCanal.ListarPor(canal);

                // Se o Canal não possui categorias, retorna.
                if (lstCatCanal.Count <= 0)
                    throw new ArgumentException("(CRM) O Canal deve conter Categoria para participar do programa.");

                // Copia Apuração, Classificação e SubClassificação para as filiais
                var lstFiliais = this.RepositoryService.Conta.ListarContasFiliaisPorMatriz(canal.ID.Value);

                foreach (var filial in lstFiliais)
                {
                    // Copia tipo de Apuração de benefícios
                    if (filial.ApuracaoBeneficiosCompromissos != canal.ApuracaoBeneficiosCompromissos
                        || filial.Classificacao != canal.Classificacao
                        || filial.Subclassificacao != canal.Subclassificacao)
                    {
                        filial.ApuracaoBeneficiosCompromissos = canal.ApuracaoBeneficiosCompromissos;
                        filial.Classificacao = canal.Classificacao;
                        filial.Subclassificacao = canal.Subclassificacao;

                        this.RepositoryService.Conta.Update(filial);
                    }
                }
                // Executa a adesão das categorias.
                foreach (var categoriaDoCanal in lstCatCanal)
                {
                    this.AdesaoAoProgramaNovaCategoria(categoriaDoCanal, canal);
                }
            }
            #endregion

            #region Se Matriz e Apuração Por Filial
            // Se Matriz e Apuração Por Filial
            if (canal.TipoConta == (int)Enum.Conta.MatrizOuFilial.Matriz
                && canal.ApuracaoBeneficiosCompromissos == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Por_Filiais)
            {
                List<CategoriasCanal> lstCatCanal = RepositoryService.CategoriasCanal.ListarPor(canal.ID.Value, null, null, null, null);

                // Se o Canal não possui categorias, retorna.
                if (lstCatCanal.Count <= 0)
                    throw new ArgumentException("(CRM) O Canal deve conter Categoria para participar do programa.");

                // Copia tipo de Apuração para as filiais
                var lstFiliais = this.RepositoryService.Conta.ListarContasFiliaisPorMatriz(canal.ID.Value);

                foreach (var filial in lstFiliais)
                {
                    // Copia tipo de Apuração de benefícios
                    if (filial.ApuracaoBeneficiosCompromissos != canal.ApuracaoBeneficiosCompromissos)
                    {
                        filial.ApuracaoBeneficiosCompromissos = canal.ApuracaoBeneficiosCompromissos;

                        this.RepositoryService.Conta.Update(filial);
                    }
                }

                // Executa a adesão das categorias.
                foreach (var categoriaDoCanal in lstCatCanal)
                {
                    this.AdesaoAoProgramaNovaCategoria(categoriaDoCanal, canal);
                }
            }
            #endregion

            #region Se Filial e Apuração Centralizada na Matriz
            // Se Filial e Apuração Centralizada na Matriz
            if (canal.TipoConta == (int)Enum.Conta.MatrizOuFilial.Filial
                && canal.ApuracaoBeneficiosCompromissos == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Centralizada_Na_Matriz)
            {
                if (canal.ContaPrimaria == null || canal.ContaPrimaria.Id == Guid.Empty)
                    throw new ArgumentException(string.Format("(CRM) Filial [{0}]-[{1}] não possui Matriz cadastrada.", canal.ID, canal.NomeFantasia));

                var matriz = this.RepositoryService.Conta.Retrieve(canal.ContaPrimaria.Id);

                // Copia Apuração, Classificação e SubClassificação para as filiais
                if (canal.ApuracaoBeneficiosCompromissos != matriz.ApuracaoBeneficiosCompromissos
                    || canal.Classificacao != matriz.Classificacao
                    || canal.Subclassificacao != matriz.Subclassificacao)
                {
                    canal.ApuracaoBeneficiosCompromissos = matriz.ApuracaoBeneficiosCompromissos;
                    canal.Classificacao = matriz.Classificacao;
                    canal.Subclassificacao = matriz.Subclassificacao;

                    this.RepositoryService.Conta.Update(canal);
                }

                var categoriaDoCanalService = new CategoriaCanalService(this.RepositoryService);
                categoriaDoCanalService.CopiarCategoriasDoCanalDaMatrizParaFilial(canal.ContaPrimaria.Id, canal);
            }
            #endregion

            #region Se Filial e Apuração Por Filial
            // Se Filial e Apuração Por Filial
            if (canal.TipoConta == (int)Enum.Conta.MatrizOuFilial.Filial
                && canal.ApuracaoBeneficiosCompromissos == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Por_Filiais)
            {
                var categoriaDoCanalService = new CategoriaCanalService(this.RepositoryService);
                categoriaDoCanalService.CopiarCategoriasDoCanalDaMatrizParaFilial(canal.ContaPrimaria.Id, canal);

                List<CategoriasCanal> lstCatCanal = RepositoryService.CategoriasCanal.ListarPor(canal.ID.Value, null, null, null, null);

                // Se o Canal não possui categorias, retorna.
                if (lstCatCanal.Count <= 0)
                    throw new ArgumentException("(CRM) O Canal deve conter Categoria para participar do programa.");

                // Executa a adesão das categorias.
                foreach (var categoriaDoCanal in lstCatCanal)
                {
                    this.AdesaoAoProgramaNovaCategoria(categoriaDoCanal, canal);
                }
            }
            #endregion

            //DDT 2.2.6
            //Gerar tarefas de visita comercial aos key accounts representantes
            new Intelbras.CRM2013.Domain.Servicos.TarefaService(RepositoryService).GerarAtividadesVisitaComercial(canal);
        }
        public void AdesaoAoProgramaNovaCategoria(CategoriasCanal categoriaDoCanal, Conta canal = null)
        {
            //Verifica se o Canal Participa do Programa, se não participa retorna e não executa a rotina..
            if (canal == null)
                canal = RepositoryService.Conta.Retrieve(categoriaDoCanal.Canal.Id);

            if (canal.ParticipantePrograma.Value != (int)Enum.Conta.ParticipaDoPrograma.Sim)
                return;

            if (canal.TipoConta.Value == (int)Enum.Conta.MatrizOuFilial.Matriz
                && canal.ApuracaoBeneficiosCompromissos.Value == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Centralizada_Na_Matriz)
            {

                // Cria Benefícios e Compromissos para a Matriz.
                this.CriarBeneficiosECompromissosParaOCanal(canal, categoriaDoCanal);

                // Replica a categoria para as filiais participantes do programa.
                var lstFiliais = this.RepositoryService.Conta.ListarContasFiliaisPorMatriz(canal.ID.Value);

                foreach (var filial in lstFiliais)
                {
                    if (filial.ParticipantePrograma.HasValue
                        && filial.ParticipantePrograma.Value == (int)Enum.Conta.ParticipaDoPrograma.Sim)
                    {
                        // Se não existe a Categoria na Filial, inclui.
                        var lstCategoriasDoCanal = this.RepositoryService.CategoriasCanal.ListarPor(filial.ID.Value, categoriaDoCanal.UnidadeNegocios.Id, categoriaDoCanal.Classificacao.Id, categoriaDoCanal.SubClassificacao.Id, categoriaDoCanal.Categoria.Id);

                        if (lstCategoriasDoCanal.Count <= 0)
                        {
                            CategoriasCanal novaCategoriaDoCanal = new CategoriasCanal(categoriaDoCanal.OrganizationName, categoriaDoCanal.IsOffline);
                            novaCategoriaDoCanal.Canal = new Lookup(filial.ID.Value, "account");
                            novaCategoriaDoCanal.Categoria = categoriaDoCanal.Categoria;
                            novaCategoriaDoCanal.Classificacao = categoriaDoCanal.Classificacao;
                            novaCategoriaDoCanal.Nome = categoriaDoCanal.Nome;
                            novaCategoriaDoCanal.SubClassificacao = categoriaDoCanal.SubClassificacao;
                            novaCategoriaDoCanal.UnidadeNegocios = categoriaDoCanal.UnidadeNegocios;
                            novaCategoriaDoCanal.Status = categoriaDoCanal.Status;

                            novaCategoriaDoCanal.ID = this.RepositoryService.CategoriasCanal.Create(novaCategoriaDoCanal);
                        }
                    }
                }
                return;
            }

            if (canal.ApuracaoBeneficiosCompromissos.Value == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Por_Filiais)
            {
                // Cria Beneficios e Beneficios
                this.CriarBeneficiosECompromissosParaOCanal(canal, categoriaDoCanal);
                return;
            }
        }

        /// <summary>
        /// Fluxo Alternativo 1 - Criar Benefícios e Compromissos para o Canal
        /// </summary>
        /// <param name="categoriaDoCanal"></param>
        /// <param name="canal"></param>
        public void CriarBeneficiosECompromissosParaOCanal(Conta canal, CategoriasCanal categoriaDoCanal)
        {
            var parametroGlobalService = new Intelbras.CRM2013.Domain.Servicos.ParametroGlobalService(RepositoryService);

            List<Perfil> lstPerfil = RepositoryService.Perfil.ListarPor(categoriaDoCanal.Classificacao.Id, categoriaDoCanal.UnidadeNegocios.Id, categoriaDoCanal.Categoria.Id, canal.Exclusividade);

            foreach (Perfil perfil in lstPerfil)
            {
                List<Guid> Compromissos = new List<Guid>();
                List<Guid> Beneficios = new List<Guid>();

                List<BeneficiosCompromissos> lstBeneficiosCompromissos = RepositoryService.BeneficioCompromisso.ListarPor(perfil.ID.Value);

                foreach (BeneficiosCompromissos beneficiosCompromissos in lstBeneficiosCompromissos)
                {
                    if (beneficiosCompromissos.Compromisso != null)
                        Compromissos.Add(beneficiosCompromissos.Compromisso.Id);

                    if (beneficiosCompromissos.Beneficio != null)
                        Beneficios.Add(beneficiosCompromissos.Beneficio.Id);
                }

                // Valida parametro Global para os Compromissos
                foreach (var compromissoId in Compromissos)
                {
                    var parametroGLobal = parametroGlobalService.ObterFrequenciaAtividadeChecklist(compromissoId);
                    if (parametroGLobal == null || !parametroGLobal.ID.HasValue || parametroGLobal.ID.Value == Guid.Empty || string.IsNullOrEmpty(parametroGLobal.Valor))
                    {
                        var compromisso = this.RepositoryService.CompromissosPrograma.ObterPor(compromissoId);
                        throw new ArgumentException(string.Format("(CRM) Parametro Global Frequencia Checklist não cadastrado para o compromisso [{0}]-[{1}].", compromisso.Nome, compromissoId));
                    }

                    // Atualiza Validade
                    int frequencia = 0;
                    if (!int.TryParse(parametroGLobal.Valor, out frequencia))
                    {
                        var compromisso = this.RepositoryService.CompromissosPrograma.ObterPor(compromissoId);
                        throw new ArgumentException(string.Format("(CRM) Parametro Global Frequencia Checklist inválido para o compromisso [{0}]-[{1}].", compromisso.Nome, compromissoId));
                    }
                }

                //Cria um registro de Benefício do Canal para cada Beneficio e Compromisso do Programa.
                if (Compromissos.Count() > 0)
                    CriaCompromissoDoCanalCategorias(Compromissos, canal.ID.Value, categoriaDoCanal.UnidadeNegocios.Id);

                //Cria um registro de Compromisso do Canal para cada Beneficio e Compromisso do Programa.
                if (Beneficios.Count() > 0)
                    CriaBeneficioDoCanalCategorias(Beneficios, canal.ID.Value, categoriaDoCanal);
            }
        }
        public void VerificaDuplicidadeCategoria(Guid Canal, Guid UnidadeNegocios)
        {
            List<CategoriasCanal> lstCatCanal = RepositoryService.CategoriasCanal.ListarPor(Canal, UnidadeNegocios, null, null, null);

            if (lstCatCanal.Count() > 0)  // Deve ter um registro (ele mesmo), caso contrario deve dar erro.
                throw new ArgumentException("(CRM) O Sistema não permite mais de uma categorização por unidade de negócio para o mesmo Canal.");
        }

        public void VerificaConfiguracaoPerfilCategoria(Guid Classificacao, Guid UnidadeNegocios, Guid Categoria, Guid CanalId)
        {
            List<Perfil> lstPerfil = RepositoryService.Perfil.ListarPorConfigurado(Classificacao, UnidadeNegocios, Categoria, RepositoryService.Conta.Retrieve(CanalId).Exclusividade.Value);

            if (lstPerfil.Count() == 0)
                throw new ArgumentException("(CRM) A Categoria " + Categoria.ToString() + " informada não tem um Perfil ou o Perfil não esta 'Configurado'. Unidade de Negócio: " + UnidadeNegocios.ToString());

        }

        private void CriaCompromissoDoCanalCategorias(List<Guid> lstCompromisso, Guid canalId, Guid unidadeNegociosId)
        {
            List<Guid> lstCompromissoProg = lstCompromisso.Distinct().ToList();

            CompromissosDoCanal NovoCompromissosDoCanal = new CompromissosDoCanal(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
            try
            {
                StatusCompromissos statusCompromisso = this.RepositoryService.StatusCompromissos.ObterPor(Enum.CompromissoCanal.StatusCompromisso.Cumprido);

                if (!statusCompromisso.ID.HasValue || statusCompromisso.ID.Value == Guid.Empty)
                    throw new ArgumentException(string.Format("(CRM) Status Compromisso [{0}] não encontrado na entidade Status Compromissos.", Enum.CompromissoCanal.StatusCompromisso.Cumprido));

                List<CompromissosDoCanal> lstCompromissoCanal = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(RepositoryService).ListarPorListaCompromissosEcanal(lstCompromissoProg, canalId, unidadeNegociosId);

                var parametroGlobalService = new Intelbras.CRM2013.Domain.Servicos.ParametroGlobalService(RepositoryService);

                //Verificamos se ele ja possui registros desativados antes de cria-los
                foreach (CompromissosDoCanal compromissoCanal in lstCompromissoCanal)
                {
                    //if (compromissoCanal.Status.HasValue && compromissoCanal.Status.Value == (int)Enum.CompromissoCanal.Status.Desativado)
                    if (compromissoCanal.Status.HasValue && compromissoCanal.Status.Value == (int)Enum.CompromissoCanal.StateCode.Inativo)
                    {
                        //Ativa o compromisso
                        RepositoryService.CompromissosDoCanal.AtualizarStatus(compromissoCanal.ID.Value, (int)Enum.CompromissoCanal.StateCode.Ativo, (int)Enum.CompromissoCanal.Status.Ativo);
                    }

                    var parametroGLobal = parametroGlobalService.ObterFrequenciaAtividadeChecklist(compromissoCanal.Compromisso.Id);
                    if (parametroGLobal == null || !parametroGLobal.ID.HasValue || parametroGLobal.ID.Value == Guid.Empty || string.IsNullOrEmpty(parametroGLobal.Valor))
                        throw new ArgumentException(string.Format("(CRM) Parametro Global Frequencia Checklist não cadastrado para o compromisso [{0}].", compromissoCanal.Compromisso.Id));

                    // Atualiza Validade
                    int frequencia = 0;
                    if (int.TryParse(parametroGLobal.Valor, out frequencia))
                        compromissoCanal.Validade = DateTime.Now.Date.AddDays(frequencia);
                    else
                        throw new ArgumentException(string.Format("(CRM) Parametro Global Frequencia Checklist inválido para o compromisso [{0}].", compromissoCanal.Compromisso.Id));

                    compromissoCanal.StatusCompromisso = new Lookup(statusCompromisso.ID.Value, "itbc_statuscompromissos");

                    //Remove da lista temporária independente de estar ativo ou não
                    lstCompromissoProg.Remove(compromissoCanal.Compromisso.Id);
                }

                var compromissosDoCanalService = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(RepositoryService);
                var unidadeNegocioService = new Intelbras.CRM2013.Domain.Servicos.UnidadeNegocioService(RepositoryService);

                foreach (var compromissoId in lstCompromissoProg)
                {
                    string nome = string.Empty;
                    CompromissosDoPrograma compPrograma = compromissosDoCanalService.BuscarCompromissoDoPrograma(compromissoId);
                    if (compPrograma != null && !string.IsNullOrEmpty(compPrograma.Nome))
                    {
                        UnidadeNegocio uniNeg = unidadeNegocioService.BuscaUnidadeNegocio(unidadeNegociosId);
                        if (uniNeg == null)
                            throw new ArgumentException("(CRM) Unidade de negócios não encontrada");

                        nome = Helper.Truncate((compPrograma.Nome + "-" + uniNeg.Nome), 98);
                    }
                    else
                    {
                        nome = "Plugin";
                    }

                    var parametroGLobal = parametroGlobalService.ObterFrequenciaAtividadeChecklist(compromissoId);
                    if (parametroGLobal == null || !parametroGLobal.ID.HasValue || parametroGLobal.ID.Value == Guid.Empty || string.IsNullOrEmpty(parametroGLobal.Valor))
                        throw new ArgumentException(string.Format("(CRM) Parametro Global Frequencia Checklist não cadastrado para o compromisso [{0}].", compromissoId));

                    // Atualiza Validade
                    int frequencia = 0;
                    if (int.TryParse(parametroGLobal.Valor, out frequencia))
                        NovoCompromissosDoCanal.Validade = DateTime.Now.Date.AddDays(frequencia);
                    else
                        throw new ArgumentException(string.Format("(CRM) Parametro Global Frequencia Checklist inválido para o compromisso [{0}].", compromissoId));

                    NovoCompromissosDoCanal.Nome = nome;
                    NovoCompromissosDoCanal.Canal = new Lookup(canalId, "account");
                    NovoCompromissosDoCanal.Compromisso = new Lookup(compromissoId, "itbc_compromissos");
                    NovoCompromissosDoCanal.UnidadeDeNegocio = new Lookup(unidadeNegociosId, "businessunit");
                    NovoCompromissosDoCanal.StatusCompromisso = new Lookup(statusCompromisso.ID.Value, "itbc_statuscompromissos");

                    RepositoryService.CompromissosDoCanal.Create(NovoCompromissosDoCanal);
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException(string.Format("(CRM) Erro na criação do compromisso do canal [{0}] e Unidade de Negócios [{1}] ", canalId, unidadeNegociosId), e);
            }

        }

        private void CriaBeneficioDoCanalCategorias(List<Guid> lstBeneficios, Guid canalId, CategoriasCanal categoriaCanal)
        {
            List<Guid> lstBeneficioProg = lstBeneficios.Distinct().ToList();
            Guid unidadeNegociosId = categoriaCanal.UnidadeNegocios.Id;
            BeneficioDoCanal novoBeneficioDoCanal = new BeneficioDoCanal(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);

            //Verifica se possui beneficios desse canal e essa unidade de negocio ja criados
            List<BeneficioDoCanal> lstBeneficioCanal = RepositoryService.BeneficioDoCanal.ListarPor(lstBeneficios, canalId, unidadeNegociosId, categoriaCanal.Categoria.Id);

            String nomeTmp = String.Empty;

            while (lstBeneficioCanal.Count > 0)
            {
                BeneficioDoCanal beneficioDoCanal = lstBeneficioCanal[0];

                //Percorre lista dos beneficios do canal ja criados,e ativa e retira eles da futura lista de criação
                var beneficioAtivo = lstBeneficioCanal.Find(b => b.Beneficio.Id == beneficioDoCanal.Beneficio.Id && b.Status == (int)Enum.BeneficioDoCanal.StateCode.Ativo && beneficioDoCanal.Categoria.Id == categoriaCanal.Categoria.Id);

                if (beneficioAtivo == null)
                {
                    var beneficioInativo = lstBeneficioCanal.Find(b => b.Beneficio.Id == beneficioDoCanal.Beneficio.Id && b.Status == (int)Enum.BeneficioDoCanal.StateCode.Inativo && beneficioDoCanal.Categoria.Id == categoriaCanal.Categoria.Id);

                    if (beneficioInativo != null)
                    {
                        //Ativa o beneficio
                        RepositoryService.BeneficioDoCanal.AtualizarStatus(beneficioInativo.ID.Value, (int)Enum.BeneficioDoCanal.StateCode.Ativo, (int)Enum.BeneficioDoCanal.StatusCode.Ativo);

                        BeneficioDoCanal benefCanal = RepositoryService.BeneficioDoCanal.ObterPor(beneficioInativo.ID.Value);
                        benefCanal.Categoria = new Lookup(categoriaCanal.Categoria.Id, "");
                        benefCanal.AcumulaVerba = true;
                        RepositoryService.BeneficioDoCanal.Update(benefCanal);
                    }
                }

                //Remove da lista temporária independente de estar ativo ou não
                //Fiz um removeAll para testar pois ja fizemos 
                lstBeneficioProg.RemoveAll(valorGuid => valorGuid == beneficioDoCanal.Beneficio.Id);
                lstBeneficioCanal.RemoveAll(b => b.Beneficio.Id == beneficioDoCanal.Beneficio.Id);
            }

            var unidadeNegocioService = new Intelbras.CRM2013.Domain.Servicos.UnidadeNegocioService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
            foreach (Guid beneficioId in lstBeneficioProg)
            {
                UnidadeNegocio uniNeg = unidadeNegocioService.BuscaUnidadeNegocio(unidadeNegociosId);
                if (uniNeg == null)
                    throw new ArgumentException("(CRM) Unidade de negócios não encontrada.");

                Beneficio _Beneficio = RepositoryService.Beneficio.ObterPor(beneficioId);
                novoBeneficioDoCanal.Nome = (_Beneficio.Nome + "-" + uniNeg.Nome).Truncate(98);
                novoBeneficioDoCanal.Canal = new Lookup(canalId, "account");
                novoBeneficioDoCanal.UnidadeDeNegocio = new Lookup(unidadeNegociosId, "businessunit");
                novoBeneficioDoCanal.Beneficio = new Lookup(beneficioId, "itbc_beneficio");
                novoBeneficioDoCanal.Categoria = new Lookup(categoriaCanal.Categoria.Id, "itbc_categoria");


                //TODO: Adesão - Fixo no código para resolver problema com criação de revendas (Ricardo, Jessica e Faria). Ficou uma pendencia para abrir chamado e revisar esse fluxo.
                var statusBeneficioId = SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("stsbeneficiosuspenso");
                novoBeneficioDoCanal.StatusBeneficio = new Lookup(statusBeneficioId, SDKore.Crm.Util.Utility.GetEntityName<StatusBeneficios>());

                RepositoryService.BeneficioDoCanal.Create(novoBeneficioDoCanal);
            }
        }

        public void AdesaoAoProgramaAlteracaoCategoria(CategoriasCanal CategoriaPre, CategoriasCanal CategoriaPost)
        {
            LimpaCompromissos(CategoriaPre);
            LimpaBeneficios(CategoriaPre);

            AdesaoAoProgramaNovaCategoria(CategoriaPost);
        }

        public void AdesaoAoProgramaRemoveCategoria(CategoriasCanal CategoriaPre)
        {
            LimpaCompromissos(CategoriaPre);
            LimpaBeneficios(CategoriaPre);
        }

        private void LimpaBeneficios(CategoriasCanal CategoriaPre)
        {
            List<BeneficioDoCanal> lstBeneficios = RepositoryService.BeneficioDoCanal.ListarPor(CategoriaPre.Canal.Id, CategoriaPre.UnidadeNegocios.Id);

            foreach (BeneficioDoCanal beneficio in lstBeneficios)
            {
                RepositoryService.BeneficioDoCanal.AtualizarStatus(beneficio.ID.Value, (int)Enum.BeneficioDoCanal.StateCode.Inativo, (int)Enum.BeneficioDoCanal.StatusCode.Inativo);
            }
        }

        private void LimpaCompromissos(CategoriasCanal CategoriaPre)
        {
            List<CompromissosDoCanal> lstCompromissos = RepositoryService.CompromissosDoCanal.ListarPor(CategoriaPre.Canal.Id, CategoriaPre.UnidadeNegocios.Id);

            foreach (CompromissosDoCanal compromisso in lstCompromissos)
            {
                RepositoryService.CompromissosDoCanal.AtualizarStatus(compromisso.ID.Value, (int)Enum.CompromissoCanal.StateCode.Inativo, (int)Enum.CompromissoCanal.Status.Desativado);
            }
        }

        public void MudarEmpresa(Contato contatoPos)
        {
            List<ColaboradorTreinadoCertificado> lstColaboradorCertificado = RepositoryService.ColaboradorTreinadoCertificado.ListarPor(contatoPos.ID.Value);

            foreach (ColaboradorTreinadoCertificado _TreinamentoDoContato in lstColaboradorCertificado)
            {
                if (contatoPos.AssociadoA == null)
                {
                    _TreinamentoDoContato.AddNullProperty("Canal");
                }
                else
                    _TreinamentoDoContato.Canal = new Lookup(contatoPos.AssociadoA.Id, "");

                RepositoryService.ColaboradorTreinadoCertificado.Update(_TreinamentoDoContato);
            }
        }

        public BeneficioDoCanal BuscarBeneficioCanal(Guid beneficioPrograma, Guid unidadeNegocio, Guid canal)
        {
            return RepositoryService.BeneficioDoCanal.ObterPor(beneficioPrograma, unidadeNegocio, canal);
        }

        public void AlterarBeneficioCanal(BeneficioDoCanal objBeneficioCanal)
        {
            RepositoryService.BeneficioDoCanal.Update(objBeneficioCanal);
            //return objBeneficioCanal;
        }

        public BeneficioDoCanal ObterPor(Guid beneficioCanalId)
        {
            return RepositoryService.BeneficioDoCanal.Retrieve(beneficioCanalId);
        }

        public List<BeneficioDoCanal> ListarPorConta(Guid accountId)
        {
            return RepositoryService.BeneficioDoCanal.ListarPorConta(accountId);
        }

        public List<BeneficioDoCanal> ListarPorContaUnidadeNegocio(Guid accountId, Guid? unidadeNegocioId)
        {
            return RepositoryService.BeneficioDoCanal.ListarPorContaUnidadeNegocio(accountId, unidadeNegocioId);
        }

        public List<BeneficioDoCanal> ListarPorContaUnidadeNegocioPlanilha()
        {
            return RepositoryService.BeneficioDoCanal.ListarPorContaUnidadeNegocioPlanilha();
        }
        public void Atualizar(BeneficioDoCanal beneficioCanal)
        {
            RepositoryService.BeneficioDoCanal.Update(beneficioCanal);
        }
        /// <summary>
        /// para ativar passe true e suspender passe false
        /// 
        /// </summary>
        /// <param name="beneficioprogId"></param>
        /// <param name="ativar"></param>
        public void AtivaOuDesativaTodos(Guid beneficioprogId, bool ativar)
        {
            List<BeneficioDoCanal> lstBeneficiodoCanal = RepositoryService.BeneficioDoCanal.ListarPorBeneficioProg(beneficioprogId);
            foreach (BeneficioDoCanal item in lstBeneficiodoCanal)
            {
                if (ativar)
                    item.StatusBeneficio = new Lookup(Guid.Parse(SDKore.Configuration.ConfigurationManager.GetSettingValue("stsbeneficioativo")), "itbc_statusbeneficios");
                else
                    item.StatusBeneficio = new Lookup(Guid.Parse(SDKore.Configuration.ConfigurationManager.GetSettingValue("stsbeneficiosuspenso")), "itbc_statusbeneficios");

                RepositoryService.BeneficioDoCanal.Update(item);

            }

        }

        public Entity ConsolidarSaldoBeneficioCanal(Entity target, Entity imagem, BeneficioDoCanal beneficioCanal, BeneficioDoCanal beneficioCanalImagem)
        {
            try
            {
                if (target == null)
                    throw new Exception();

                decimal verbaCalculada, verbaReembolsada, verbaBruta, solicitacoes, verbaLiquida;

                if (imagem == null)
                {
                    verbaCalculada = target.Contains("itbc_verbaperiodoatual") ? ((Money)target.Attributes["itbc_verbaperiodoatual"]).Value : 0;


                    verbaReembolsada = target.Contains("itbc_verbareembolsada") ? ((Money)target.Attributes["itbc_verbareembolsada"]).Value : 0;
                    solicitacoes = target.Contains("itbc_totalsolicitacoesaprovadas") ? ((Money)target.Attributes["itbc_totalsolicitacoesaprovadas"]).Value : 0;
                    verbaBruta = verbaCalculada - verbaReembolsada;
                    if (verbaBruta < 0)
                        verbaBruta = 0;
                    verbaLiquida = verbaBruta > 0 ? verbaBruta - solicitacoes : 0;
                }
                else
                {
                    //Verifica se o campo Verba calculada foi preenchido ou nao
                    if (target.Contains("itbc_verbaperiodoatual"))
                        if (target.Attributes["itbc_verbaperiodoatual"] == null)
                            verbaCalculada = 0;
                        else
                            verbaCalculada = ((Money)target.Attributes["itbc_verbaperiodoatual"]).Value;
                    else if (imagem.Contains("itbc_verbaperiodoatual"))
                        if (imagem.Attributes["itbc_verbaperiodoatual"] == null)
                            verbaCalculada = 0;
                        else
                            verbaCalculada = ((Money)imagem.Attributes["itbc_verbaperiodoatual"]).Value;
                    else
                        verbaCalculada = 0;

                    //Verifica se o campo Verba Reembolsada foi preenchido ou nao
                    if (target.Contains("itbc_verbareembolsada"))
                        if (target.Attributes["itbc_verbareembolsada"] == null)
                            verbaReembolsada = 0;
                        else
                            verbaReembolsada = ((Money)target.Attributes["itbc_verbareembolsada"]).Value;


                    else if (imagem.Contains("itbc_verbareembolsada"))
                        if (imagem.Attributes["itbc_verbareembolsada"] == null)
                            verbaReembolsada = 0;
                        else
                            verbaReembolsada = ((Money)imagem.Attributes["itbc_verbareembolsada"]).Value;
                    else
                        verbaReembolsada = 0;

                    //Verifica se o campo solicitacoes foi preenchido ou nao
                    if (target.Contains("itbc_totalsolicitacoesaprovadas"))
                        if (target.Attributes["itbc_totalsolicitacoesaprovadas"] == null)
                            solicitacoes = 0;
                        else
                            solicitacoes = ((Money)target.Attributes["itbc_totalsolicitacoesaprovadas"]).Value;
                    else if (imagem.Contains("itbc_totalsolicitacoesaprovadas"))
                        if (imagem.Attributes["itbc_totalsolicitacoesaprovadas"] == null)
                            solicitacoes = 0;
                        else
                            solicitacoes = ((Money)imagem.Attributes["itbc_totalsolicitacoesaprovadas"]).Value;
                    else
                        solicitacoes = 0;

                    verbaBruta = verbaCalculada - verbaReembolsada;

                    if (verbaBruta < 0)
                        verbaBruta = 0;

                    verbaLiquida = verbaBruta > 0 ? verbaBruta - solicitacoes : 0;



                }

                //Calculo referente ddt 4.1.2 - soma das solicitações aprovadas e não pagas
                decimal valorTotalSolNaoPagas = 0;

                List<SolicitacaoBeneficio> lstSolNaoPagas = RepositoryService.SolicitacaoBeneficio.ListarPorBeneficioCanal(beneficioCanal.ID.Value);


                foreach (var item in lstSolNaoPagas)
                {
                    if ((item.StatusSolicitacao != (int?)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Criada
                        && item.StatusSolicitacao != (int?)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.EmAnalise
                        && item.StatusSolicitacao != (int?)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado
                        && item.StatusSolicitacao != (int?)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.NaoAprovada
                        && item.StatusSolicitacao != (int?)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Cancelada)
                        && (item.ValorAbater.HasValue == true))
                    {
                        valorTotalSolNaoPagas += item.ValorAbater.Value;
                    }
                }

                if (valorTotalSolNaoPagas < 0)
                    throw new Exception("Valor total solicitações aprovadas :" + valorTotalSolNaoPagas.ToString() + " menor que 0,operação cancelada.");

                if (target.Contains("itbc_totalsolicitacoesaprovadas"))
                    target.Attributes["itbc_totalsolicitacoesaprovadas"] = new Money(valorTotalSolNaoPagas);
                else
                    target.Attributes.Add("itbc_totalsolicitacoesaprovadas", new Money(valorTotalSolNaoPagas));

                // fim 4.1.2

                if (target.Contains("itbc_verbabrutaperiodoatual"))
                    target.Attributes["itbc_verbabrutaperiodoatual"] = new Money(verbaBruta);
                else
                    target.Attributes.Add("itbc_verbabrutaperiodoatual", new Money(verbaBruta));


                if (verbaBruta < 0)
                    verbaBruta = 0;

                verbaLiquida = verbaBruta > 0 ? verbaBruta - valorTotalSolNaoPagas : 0;

                if (target.Contains("itbc_verbadisponivel"))
                    target.Attributes["itbc_verbadisponivel"] = new Money(verbaLiquida);
                else
                    target.Attributes.Add("itbc_verbadisponivel", new Money(verbaLiquida));

                return target;
            }
            catch (Exception e)
            {
                throw;
            }
        }



        #region Integracao

        public string IntegracaoBarramento(CategoriasCanal objCategoriaCanal)
        {
            Domain.Integracao.MSG0122 msgCategoriaCanal = new Domain.Integracao.MSG0122(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            return msgCategoriaCanal.Enviar(objCategoriaCanal);
        }
        #endregion


        public void ValidarDescredenciamentoAoPrograma(Conta canal)
        {
            if (canal.ApuracaoBeneficiosCompromissos == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Centralizada_Na_Matriz
                && canal.TipoConta == (int)Enum.Conta.MatrizOuFilial.Filial)
            {
                if (canal.ContaPrimaria != null && canal.ContaPrimaria.Id != Guid.Empty)
                {
                    var matriz = this.RepositoryService.Conta.Retrieve(canal.ContaPrimaria.Id);

                    if (matriz.ParticipantePrograma == (int)Enum.Conta.ParticipaDoPrograma.Sim)
                    {
                        throw new ArgumentException("(CRM) Para descredenciar este canal você deve primeiro descredenciar a sua matriz.");
                    }
                }
            }

            var listaBeneficiosComSaldo = RepositoryService.BeneficioDoCanal.ListarComSaldoAtivos(canal.ID.Value);
            SDKore.Helper.Error.Create(listaBeneficiosComSaldo.Count + "", System.Diagnostics.EventLogEntryType.Error);

            if (listaBeneficiosComSaldo.Count > 0)
            {
                throw new ArgumentException("(CRM) Para descredenciar este canal você deve primeiro zerar o saldo de todos os benefícios.");
            }
        }

        public void DescredenciamentoAoPrograma(Conta canal)
        {
            if (canal.Classificacao.Name == "Revendas")
            {
                canal.IntegraIntelbrasPontua = true;
            }

            if (canal.ParticipantePrograma.Value == (int)Enum.Conta.ParticipaDoPrograma.Sim)
            {
                canal.ParticipantePrograma = (int)Domain.Enum.Conta.ParticipaDoPrograma.Descredenciado;
                this.RepositoryService.Conta.Update(canal);
            }

            // Se Canal=Matriz e Apuração=Centralizado na Matriz
            if (canal.TipoConta.Value == (int)Enum.Conta.MatrizOuFilial.Matriz
                && canal.ApuracaoBeneficiosCompromissos.Value == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Centralizada_Na_Matriz)
            {

                // Descredencia as Filiais
                var lstFiliais = this.RepositoryService.Conta.ListarContasFiliaisPorMatriz(canal.ID.Value);

                foreach (var filial in lstFiliais)
                {
                    if (filial.ParticipantePrograma == (int)Enum.Conta.ParticipaDoPrograma.Sim)
                    {
                        filial.ParticipantePrograma = (int)Enum.Conta.ParticipaDoPrograma.Descredenciado;
                        this.RepositoryService.Conta.Update(filial);
                        this.InativarBeneFiciosECompromissosCategoriasEAcessoAExtranet(filial);
                    }
                }

                this.InativarBeneFiciosECompromissosCategoriasEAcessoAExtranet(canal);

                return;
            }

            // Se Centralizado na matriz e Filial
            if (canal.ApuracaoBeneficiosCompromissos.Value == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Centralizada_Na_Matriz
                && canal.TipoConta.Value == (int)Enum.Conta.MatrizOuFilial.Filial)
            {
                this.ValidarDescredenciamentoAoPrograma(canal);
                return;
            }

            // Se Por Filial
            if (canal.ApuracaoBeneficiosCompromissos.Value == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Por_Filiais)
            {
                this.InativarBeneFiciosECompromissosCategoriasEAcessoAExtranet(canal);
                return;
            }
        }

        /// <summary>
        /// Fluxo alternativo 2 - Inativação de Benefícios e COmpromissos para Canal, Categoria do Canal e "Acesso a Extranet dos Contatos".
        /// </summary>
        /// <param name="canal"></param>
        public void InativarBeneFiciosECompromissosCategoriasEAcessoAExtranet(Conta canal)
        {
            var lstBeneficiosDoCanal = this.RepositoryService.BeneficioDoCanal.ListarPorConta(canal.ID.Value);

            foreach (var beneficioDoCanal in lstBeneficiosDoCanal)
            {
                if (!beneficioDoCanal.VerbaBrutaPeriodoAtual.HasValue || beneficioDoCanal.VerbaBrutaPeriodoAtual.Value == 0)
                {
                    if (beneficioDoCanal.Status == (int)Enum.BeneficioDoCanal.StateCode.Ativo)
                        this.RepositoryService.BeneficioDoCanal.AtualizarStatus(beneficioDoCanal.ID.Value, (int)Enum.BeneficioDoCanal.StateCode.Inativo, (int)Enum.BeneficioDoCanal.StatusCode.Inativo);
                }
                else
                {
                    beneficioDoCanal.AcumulaVerba = false;
                    this.RepositoryService.BeneficioDoCanal.Update(beneficioDoCanal);
                }
            }

            var compromissosDoCanalService = new CompromissosDoCanalService(this.RepositoryService);
            compromissosDoCanalService.InativarCompromissosDoCanal(canal);

            var categoriaCanalService = new CategoriaCanalService(this.RepositoryService);
            categoriaCanalService.InativarCategoriaDoCanal(canal);

            var acessoExtranetContatoService = new AcessoExtranetContatoService(this.RepositoryService);
            acessoExtranetContatoService.InativarAcessoExtranetContato(canal);
        }

        public void InativaBeneficioDoCanal(BeneficioDoCanal beneficioCanal)
        {
            if (beneficioCanal.Canal == null || beneficioCanal.Canal.Id == Guid.Empty)
                return;

            var canal = this.RepositoryService.Conta.Retrieve(beneficioCanal.Canal.Id);

            if (canal.ParticipantePrograma.HasValue
                && (canal.ParticipantePrograma.Value == (int)Enum.Conta.ParticipaDoPrograma.Nao
                || canal.ParticipantePrograma.Value == (int)Enum.Conta.ParticipaDoPrograma.Descredenciado))
            {
                this.RepositoryService.BeneficioDoCanal.AtualizarStatus(beneficioCanal.ID.Value, (int)Enum.BeneficioDoCanal.StateCode.Inativo, (int)Enum.BeneficioDoCanal.StatusCode.Inativo);
            }
        }

        public List<Conta> ListarContasParticipantes()
        {
            var contas = this.RepositoryService.Conta.ListarContasParticipantesApuracaoPorFilial();
            contas.AddRange(this.RepositoryService.Conta.ListarMatrizesParticipantesApuracaoCentralizadaNaMatriz());

            return contas;
        }

        public void AdesaoAoProgramaTodosOsCanais()
        {
            var lstMatrizParticipante = this.RepositoryService.Conta.ListarMatrizesParticipantes();

            int count = 0;
            foreach (var matriz in lstMatrizParticipante)
            {
                count++;
                this.AdesaoAoPrograma(matriz);
            }
        }

        public Boolean validaIntegraPontuaFielo(Conta canalPre, Conta canalMerge)
        {
            //conta antes alteração deve ser enviado para pontua
            if (validaCategoriaPontua(canalPre)) { return true; }

            //conta antes alteração não deve ir para pontua, mas depois da alteração deve ir pontua
            if (!validaCategoriaPontua(canalPre) && validaCategoriaPontua(canalMerge)) { return true; }

            //conta antes alteração não deve ir para pontua, mas depois da alteração não deve ir pontua
            if (!validaCategoriaPontua(canalPre) && (!validaCategoriaPontua(canalMerge))) { return false; }

            return false;
        }

        public Boolean validaCategoriaPontua(Conta Conta)
        {
            if (Conta != null)
            {
                if (Conta.ParticipantePrograma.Value == (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim)
                {
                    switch (Conta.Classificacao.Name)
                    {
                        case Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Revendas:                    
                            if (Conta.Categoria != null && (Conta.Categoria.Id == SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Categoria.Ouro")
                                || Conta.Categoria.Id == SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Categoria.Prata")
                                || Conta.Categoria.Id == SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Categoria.Bronze")))
                                return true;
                            break;  
                        case Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Dist_VAD:
                        case Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Dist_BoxMover:
                            return true;
                        case Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Atac_Dist:
                            if(Conta.Subclassificacao != null && Conta.Subclassificacao.Name == Intelbras.CRM2013.Domain.Enum.Conta.SubClassificacao.Atac_Distribuidor)
                                if(Conta.Categoria != null && Conta.Categoria.Name == Intelbras.CRM2013.Domain.Enum.Conta.CategoriaConta.Completo)
                                    return true;
                            break;
                        case Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Rev_Sol:
                            string[] subclassificacoes = {Intelbras.CRM2013.Domain.Enum.Conta.SubClassificacao.Rev_Integrador, 
                                                            Intelbras.CRM2013.Domain.Enum.Conta.SubClassificacao.Rev_Bancaria, 
                                                            Intelbras.CRM2013.Domain.Enum.Conta.SubClassificacao.Rev_Incendio,
                                                            Intelbras.CRM2013.Domain.Enum.Conta.SubClassificacao.Rev_Monitoramento,
                                                            Intelbras.CRM2013.Domain.Enum.Conta.SubClassificacao.Rev_Revendas,
                                                            Intelbras.CRM2013.Domain.Enum.Conta.SubClassificacao.Rev_Telecom};

                            if(Conta.Subclassificacao != null && (subclassificacoes.Contains(Conta.Subclassificacao.Name)))
                                if(Conta.Categoria != null && Conta.Categoria.Name == Intelbras.CRM2013.Domain.Enum.Conta.CategoriaConta.Rev_Sol)
                                    return true;
                            break;
                        case Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Provedores:
                            if(Conta.Subclassificacao != null && Conta.Subclassificacao.Name == Intelbras.CRM2013.Domain.Enum.Conta.SubClassificacao.Provedores)
                                if(Conta.Categoria != null && Conta.Categoria.Name == Intelbras.CRM2013.Domain.Enum.Conta.CategoriaConta.Provedores)
                                return true;
                            break;
                        default:
                            return false;
                    }
                }
            }
            return false;
        }
    }
}