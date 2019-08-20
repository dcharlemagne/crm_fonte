using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Xml.Linq;
using Intelbras.Message.Helper;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using System.ServiceModel;
using System.Collections.Generic;
//using SDKore.Configuration;
//using Intelbras.CRM2013.Application.Barramento;



namespace Intelbras.CRM2013.Domain.Servicos
{
    public class TabelaPrecoExtranetService
    {
        public string Organizacao { get; set; }
        public bool IsOffline { get; set; }
        private BasicHttpBinding myBinding { get; set; }
        private EndpointAddress endPointAddress { get; set; }
        private TabelaPrecoExtranet.SellOutServiceService ConexaoTabelaPrecoExtranet { get; set; }


        public TabelaPrecoExtranetService(string org, bool isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));

            this.ConexaoTabelaPrecoExtranet = new TabelaPrecoExtranet.SellOutServiceService();
            //Usamos o SellOutAutentificacaoUrl porque esta no mesmo webservice do sellout
            this.ConexaoTabelaPrecoExtranet.Url = SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutAutentificacaoUrl");
        }
        public TabelaPrecoExtranet.Resposta IncluirPlanilhaTabelaPreco(Guid conta, bool gerarPSD, bool gerarPP, bool gerarPSCF)
        {
            TabelaPrecoExtranet.Resposta resposta = this.ConexaoTabelaPrecoExtranet.IncluirPlanilhaTabelaPreco(conta.ToString(), gerarPSD, gerarPP, gerarPSCF, true);

            return resposta;
        }
        public TabelaPrecoExtranet.TabelaPrecoPlanilhaResposta ListarPlanilhasTabelaPreco(Guid conta)
        {
            TabelaPrecoExtranet.TabelaPrecoPlanilhaResposta resposta = this.ConexaoTabelaPrecoExtranet.ListarPlanilhasTabelaPreco(conta.ToString());

            return resposta;
        }
    }
}
