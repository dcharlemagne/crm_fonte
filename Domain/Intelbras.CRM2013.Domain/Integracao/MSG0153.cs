using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0153 : Base, IBase<Message.Helper.MSG0153, Domain.Model.AcaoSubsidiadaVmc>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0153(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }
        #endregion

        #region trace
        private SDKore.Helper.Trace Trace { get; set; }
        public void DefinirObjetoTrace(SDKore.Helper.Trace trace)
        {
            this.Trace = trace;
        }
        #endregion

        #region Executar

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            List<Intelbras.Message.Helper.Entities.AcaoVMCItem> lstPolluxAcaoSubsidiadaVmc = new List<Pollux.Entities.AcaoVMCItem>();
            //SolicitacoesItens
            List<AcaoSubsidiadaVmc> lstAcaoSubsidiadaVmc = new Servicos.AcaoSubsidiadaVmcService(this.Organizacao, this.IsOffline).Listar();

            #region Lista

            if (lstAcaoSubsidiadaVmc != null && lstAcaoSubsidiadaVmc.Count > 0)
            {

                foreach (AcaoSubsidiadaVmc crmAcaoSubsidiadaVmc in lstAcaoSubsidiadaVmc)
                {
                    Pollux.Entities.AcaoVMCItem objPollux = new Pollux.Entities.AcaoVMCItem();
                    objPollux.CodigoAcaoSubsidiadaVMC = crmAcaoSubsidiadaVmc.ID.ToString();
                    objPollux.NomeAcaoSubsidiadaVMC = crmAcaoSubsidiadaVmc.Nome;
                    lstPolluxAcaoSubsidiadaVmc.Add(objPollux);
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0153R1>(numeroMensagem, retorno);
            }

            #endregion

            retorno.Add("AcaoVMCItens", lstPolluxAcaoSubsidiadaVmc);
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0153R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public AcaoSubsidiadaVmc DefinirPropriedades(Intelbras.Message.Helper.MSG0153 xml)
        {
            var crm = new Model.AcaoSubsidiadaVmc(this.Organizacao, this.IsOffline);
            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(AcaoSubsidiadaVmc objModel)
        {
            return String.Empty;
        }
        #endregion
    }
}
