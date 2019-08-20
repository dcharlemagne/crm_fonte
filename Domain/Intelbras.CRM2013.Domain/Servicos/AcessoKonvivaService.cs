using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class AcessoKonvivaService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public AcessoKonvivaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public AcessoKonvivaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public AcessoKonvivaService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }
        #endregion

        #region Propriedades
        public Guid? organizacaoPadrao
        {
            get
            {
                Guid temp;
                if (_organizacaoPadrao == null)
                {
                    if (!Guid.TryParse(SDKore.Configuration.ConfigurationManager.GetSettingValue("organizacaoPadraoKonviva"), out temp))
                        throw new ArgumentException("(CRM) Variável organizacaoPadraoKonviva do SDKore.config não foi configurada corretamente.");

                    _organizacaoPadrao = temp; 
                }

                return _organizacaoPadrao;
            }
        }
        public Guid? _organizacaoPadrao { get; set; }
        #endregion

        #region Metodos

        public AcessoKonviva ObterPorContato(Guid contato, Domain.Enum.StateCode status)
        {
            return RepositoryService.AcessoKonviva.ObterPorContato(contato, status);
        }

        public void CriarAcessoKonvivaPadrao(Guid contatoId)
        {
            Contato contatoM = RepositoryService.Contato.Retrieve(contatoId);

            if (contatoM == null)
            {
                throw new ArgumentException("(CRM) Contato não existente");
            }

            AcessoKonviva acessoKonviva = RepositoryService.AcessoKonviva.ObterPorContato(contatoId, Domain.Enum.StateCode.Ativo);

            if (acessoKonviva != null)
            {
                throw new ArgumentException("(CRM) Usuário Konviva Duplicado!");
            }

            acessoKonviva = new AcessoKonviva(RepositoryService.NomeDaOrganizacao,RepositoryService.IsOffline);

            acessoKonviva.Contato = new Lookup(contatoM.ID.Value, "");

            //Nova regra de atribuição de Unidade Konviva
            if (contatoM.AssociadoA != null && contatoM.AssociadoA.Type == SDKore.Crm.Util.Utility.GetEntityName<Conta>())
            {
                acessoKonviva.Conta = new Lookup(contatoM.AssociadoA.Id, contatoM.AssociadoA.Type);
                acessoKonviva = new DeParaDeUnidadeDoKonvivaService(RepositoryService).ObterUnidadeKonvivaDeParaCom(acessoKonviva ,RepositoryService.Conta.Retrieve(acessoKonviva.Conta.Id), null);
            }
            else
            {
                acessoKonviva = new DeParaDeUnidadeDoKonvivaService(RepositoryService).ObterUnidadeKonvivaDeParaCom(acessoKonviva, null, contatoM);
            }

            if (acessoKonviva.UnidadeKonviva == null && this.organizacaoPadrao.HasValue)
            {
                acessoKonviva.UnidadeKonviva = new Lookup(this.organizacaoPadrao.Value, "");
                acessoKonviva.DeParaUnidadeKonviva = null;
            }

            //reseta o perfil para apenas aluno selecionado
            acessoKonviva = this.ResetarPerfilKonviva(acessoKonviva);

            //TODO NOME KONVIVA VER PADRAO DO DOCUMENTOS
            acessoKonviva.Nome = acessoKonviva.Contato.Name;

            RepositoryService.AcessoKonviva.Create(acessoKonviva);
        }

        public AcessoKonviva RetornarUnidadeCorreta(AcessoKonviva acessoKonviva)
        {
            var deParaService = new DeParaDeUnidadeDoKonvivaService(RepositoryService);

            if (acessoKonviva.Conta != null)
            {
                var canal = RepositoryService.Conta.Retrieve(acessoKonviva.Conta.Id);
                return deParaService.ObterUnidadeKonvivaDeParaCom(acessoKonviva ,canal, null);
            }
            else
            {
                var contato = RepositoryService.Contato.Retrieve(acessoKonviva.Contato.Id);
                return deParaService.ObterUnidadeKonvivaDeParaCom(acessoKonviva, null, contato);
            }
        }

        public AcessoKonviva ResetarPerfilKonviva(AcessoKonviva acessoKonviva)
        {
            if (acessoKonviva == null)
            {
                throw new ArgumentNullException("AcessoKonviva não fornecido");
            }

            acessoKonviva.PerfilAdministrador = false;
            acessoKonviva.PerfilAluno = true;
            acessoKonviva.PerfilAnalista = false;
            acessoKonviva.PerfilAutor = false;
            acessoKonviva.PerfilGestor = false;
            acessoKonviva.PerfilInstrutor = false;
            acessoKonviva.PerfilModerador = false;
            acessoKonviva.PerfilMonitor = false;
            acessoKonviva.PerfilTutor = false;

            return acessoKonviva;
        }

        /// <summary>
        /// Valida se o usuário que não é vinculado a intelbras possui um perfil que nao poderia ter
        /// </summary>
        /// <param name="acessoKonviva"></param>
        /// <returns>Retorna true se estiver Ok, e falso se estiver inválido</returns>
        public Boolean ValidarTipoAcesso(AcessoKonviva acessoKonviva)
        {
            string cnpjMatrizIntelbras = SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.Conta.CnpjMatriz");

            if (string.IsNullOrWhiteSpace(cnpjMatrizIntelbras))
            {
                throw new ArgumentException("(CRM) CNPJ da Matriz Intelbras não está configurado!");
            }

            Conta conta = new ContaService(RepositoryService).BuscaContaPorCpfCnpj(cnpjMatrizIntelbras);

            if (conta == null)
            {
                throw new ArgumentException("(CRM) Conta Intelbras não encontrada");
            }

            if (acessoKonviva.Contato == null)
            {
                throw new ArgumentException("(CRM) Acesso Konviva não possui contato");
            }

            Contato contato = RepositoryService.Contato.Retrieve(acessoKonviva.Contato.Id);

            if (contato == null)
            {
                throw new ArgumentException("(CRM) Contato não existente");
            }

            List<AcessoKonviva> listaAcessoKonviva = RepositoryService.AcessoKonviva.ListarPor(contato.ID.Value);

            if (listaAcessoKonviva.Count > 1)
            {
                throw new ArgumentException("(CRM) Acesso Konviva duplicado no sistema !");
            }

            if (contato.AssociadoA == null || contato.AssociadoA.Id != conta.ID.Value)
            {
                if (acessoKonviva.PerfilAdministrador == true)
                    return false;
            }

            return true;
        }

        public AcessoKonviva ObterPor(Guid acessokonvivaId)
        {
            return RepositoryService.AcessoKonviva.ObterPor(acessokonvivaId);
        }

        public AcessoKonviva Persistir(Model.AcessoKonviva objAcessoKonviva)
        {
            AcessoKonviva TmpAcessoKonviva = null;
            if (objAcessoKonviva.ID.HasValue)
            {
                TmpAcessoKonviva = RepositoryService.AcessoKonviva.ObterPor(objAcessoKonviva.ID.Value);

                if (TmpAcessoKonviva != null)
                {
                    RepositoryService.AcessoKonviva.Update(objAcessoKonviva);
                    //Altera Status - Se necessário
                    if (objAcessoKonviva.Status.HasValue && !TmpAcessoKonviva.Status.Equals(objAcessoKonviva.Status))
                        this.MudarStatus(TmpAcessoKonviva.ID.Value, objAcessoKonviva.Status.Value);
                    return TmpAcessoKonviva;
                }
                else
                    return null;
            }
            else
            {
                objAcessoKonviva.ID = RepositoryService.AcessoKonviva.Create(objAcessoKonviva);
                return objAcessoKonviva;
            }
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.AcessoKonviva.AlterarStatus(id, status);
        }

        public string IntegracaoBarramento(AcessoKonviva objKonviva)
        {
            Domain.Integracao.MSG0105 msgProdEstab = new Domain.Integracao.MSG0105(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            return msgProdEstab.Enviar(objKonviva);
        }

        public void MudarCanal(Contato contato, Guid? canalId)
        {
            AcessoKonviva acessoKonviva = this.ObterPorContato(contato.ID.Value, Enum.StateCode.Ativo);
            if (acessoKonviva != null)
            {
                acessoKonviva = this.ResetarPerfilKonviva(acessoKonviva);
                acessoKonviva.IntegrarNoPlugin = false;               
                
                if (canalId.HasValue)
                {
                    acessoKonviva.Conta = new SDKore.DomainModel.Lookup(canalId.Value, SDKore.Crm.Util.Utility.GetEntityName<Conta>());
                    acessoKonviva = new DeParaDeUnidadeDoKonvivaService(RepositoryService).ObterUnidadeKonvivaDeParaCom(acessoKonviva ,RepositoryService.Conta.Retrieve(canalId.Value), null);
                }
                else
                {
                    acessoKonviva.AddNullProperty("Conta");
                    acessoKonviva = new DeParaDeUnidadeDoKonvivaService(RepositoryService).ObterUnidadeKonvivaDeParaCom(acessoKonviva, null, contato);
                }

                this.Persistir(acessoKonviva);
            }
        }

        #endregion

    }
}
