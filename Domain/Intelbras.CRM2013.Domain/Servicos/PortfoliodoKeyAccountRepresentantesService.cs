using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class PortfoliodoKeyAccountRepresentantesService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }
        public PortfoliodoKeyAccountRepresentantesService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }
        public PortfoliodoKeyAccountRepresentantesService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        public List<PortfoliodoKeyAccountRepresentantes> ListarPortfolioKARepresentantes(int? CodigoAssistente, string CodigoSupervisorEMS)
        {
            if (CodigoAssistente.HasValue)
            {
                Usuario assistente = RepositoryService.Usuario.ObterPorCodigoAssistente(CodigoAssistente.Value);

                if (assistente != null)
                    return RepositoryService.PortfoliodoKeyAccountRepresentantes.ListarPorRepresentanteECodigos(assistente.ID.Value);
            }
            if (!String.IsNullOrEmpty(CodigoSupervisorEMS))
            {
                Usuario supervisor = RepositoryService.Usuario.ObterPorCodigoSupervisorEMS(CodigoSupervisorEMS);

                if (supervisor != null)
                    return RepositoryService.PortfoliodoKeyAccountRepresentantes.ListarPorRepresentanteECodigos(supervisor.ID.Value);
            }

            throw new ArgumentException("Código Supervisor/Assistente inválido");
        }

        public void AtualizarCodigosAssistenteSupervisor(Usuario imagemPre, Usuario imagemPos)
        {
            //Verificamos se o campo nao esta sendo preenchido pela primeira vez,pois ai nao teria nada para atualizar
            if (!String.IsNullOrEmpty(imagemPre.CodigoSupervisorEMS) || imagemPre.CodigoAssistenteComercial.HasValue)
            {
                List<Domain.Model.PortfoliodoKeyAccountRepresentantes> lstPortfKARepre = new Intelbras.CRM2013.Domain.Servicos.PortfoliodoKeyAccountRepresentantesService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).ListarPortfolioKARepresentantes(imagemPos.CodigoAssistenteComercial, imagemPos.CodigoSupervisorEMS);
                foreach (Domain.Model.PortfoliodoKeyAccountRepresentantes portfolioKARepresentante in lstPortfKARepre)
                {
                    new Intelbras.CRM2013.Domain.Servicos.PortfoliodoKeyAccountRepresentantesService(RepositoryService.NomeDaOrganizacao,RepositoryService.IsOffline,RepositoryService.Provider).IntegracaoBarramento(portfolioKARepresentante);
                }
            }
        }
        public void AtualizarPortfolioKARepresentante(PortfoliodoKeyAccountRepresentantes PortfolioKA)
        {
            RepositoryService.PortfoliodoKeyAccountRepresentantes.Update(PortfolioKA);
        }

        public void VerificaDuplicidadePortforioKARepresentantes(PortfoliodoKeyAccountRepresentantes PortfolioKA)
        {
            List<PortfoliodoKeyAccountRepresentantes> lstPortFolioKa = new List<PortfoliodoKeyAccountRepresentantes>();

            if (PortfolioKA.KeyAccountRepresentante.Id == null)
                throw new ArgumentException("KeyAccount Representante é obrigatório.");

            if (PortfolioKA.UnidadedeNegocio == null)
                throw new ArgumentException("Unidade de Negócio é obrigatório.");


            lstPortFolioKa = RepositoryService.PortfoliodoKeyAccountRepresentantes.ListarPorUnidadeNegocioEsegmentoVazio(PortfolioKA.ID.Value, PortfolioKA.KeyAccountRepresentante.Id, PortfolioKA.UnidadedeNegocio.Id); //unidade de negócio, desde que o segmento não seja preenchido
            if (lstPortFolioKa.Count > 0)
                throw new ArgumentException("Ja existe um outro PortfolioKeyAccount com o mesmo KeyAccount Representante, Unidade de Negócio e Sem Segmento informado (atende todos os segmentos).");

            if (PortfolioKA.Segmento != null)
            {
                lstPortFolioKa = RepositoryService.PortfoliodoKeyAccountRepresentantes.ListarPorUnidadeNegocioEsegmento(PortfolioKA.ID.Value, PortfolioKA.KeyAccountRepresentante.Id, PortfolioKA.Segmento.Id, PortfolioKA.UnidadedeNegocio.Id);//segmento 

                if (lstPortFolioKa.Count > 0)
                    throw new ArgumentException("Ja existe um outro PortfolioKeyAccount com o mesmo KeyAccount Representante, Unidade de Negócio e Segmento.");
            }
            else
            {
                lstPortFolioKa = RepositoryService.PortfoliodoKeyAccountRepresentantes.ListarPorUnidadeNegocioEsegmentoNaoVazio(PortfolioKA.ID.Value, PortfolioKA.KeyAccountRepresentante.Id, PortfolioKA.UnidadedeNegocio.Id); //unidade de negócio, desde que o segmento não seja preenchido
                if (lstPortFolioKa.Count > 0)
                    throw new ArgumentException("Ja existe um outro PortfolioKeyAccount com o mesmo KeyAccount Representante, Unidade de Negócio e com algum Segmento informado. Não é possivel criar para todos os Segmentos");
            }

        }

        public string IntegracaoBarramento(PortfoliodoKeyAccountRepresentantes objPortfolioRepresentante)
        {
            Domain.Integracao.MSG0143 msgPortRepres = new Domain.Integracao.MSG0143(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            return msgPortRepres.Enviar(objPortfolioRepresentante);
        }

    }
}

