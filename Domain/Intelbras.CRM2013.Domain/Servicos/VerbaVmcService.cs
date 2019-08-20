using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class VerbaVmcService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public VerbaVmcService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public VerbaVmcService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public string EnviaVerbaFielo(Conta codigoMatriz, string CNPJ)
        {
            VerbaVMC verbaVmc = new VerbaVMC(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            verbaVmc.CodigoConta = codigoMatriz.ID.ToString();
            verbaVmc.CNPJ = CNPJ.Replace(".", "").Replace("/", "").Replace("-", "");

            var verba = RepositoryService.ParametroGlobal.ObterPor((int)Domain.Enum.TipoParametroGlobal.ValorVerbaVMCEnviadaParaFielo, null, null, codigoMatriz.Categoria.Id, null, null, null, null);
            if (verba != null)
            {
                verbaVmc.Verba = Convert.ToDecimal(verba.Valor);
            }
            else
            {
                return "false"; //"Não foi localizado o Parâmetro Global 88 {Valor da VerbaVMC Enviada Para Fielo}
            }

            string trimestre = "";
            if (DateTime.Now.Month >= 1 && DateTime.Now.Month <= 3)
            {
                trimestre = DateTime.Now.Year + "-T1";
            }
            else if (DateTime.Now.Month >= 4 && DateTime.Now.Month <= 6)
            {
                trimestre = DateTime.Now.Year + "-T2";
            }
            else if (DateTime.Now.Month >= 7 && DateTime.Now.Month <= 9)
            {
                trimestre = DateTime.Now.Year + "-T3";
            }
            if (DateTime.Now.Month >= 10 && DateTime.Now.Month <= 12)
            {
                trimestre = DateTime.Now.Year + "-T4";
            }

            verbaVmc.Trimestre = trimestre;
            var dias = RepositoryService.ParametroGlobal.ObterPor((int)Domain.Enum.TipoParametroGlobal.PrazoEmdiasResgatarVerbaFielo, null, null, null, null, null, null, null);
            verbaVmc.Validade = DateTime.Now.AddDays(Convert.ToDouble(dias.Valor));

            Domain.Integracao.MSG0185 msgVerbaVMC = new Domain.Integracao.MSG0185(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            return msgVerbaVMC.Enviar(verbaVmc);
        }

        #endregion
    }
}
