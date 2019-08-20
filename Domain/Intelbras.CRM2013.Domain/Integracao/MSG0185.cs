using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0185 : Base, IBase<Pollux.MSG0185, Model.VerbaVMC>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };


        #endregion

        #region Construtor
        public MSG0185(string org, bool isOffline) : base(org, isOffline)
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

            resultadoPersistencia.Sucesso = false;
            resultadoPersistencia.Mensagem = "(Integração não permitida!";

            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0185R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public VerbaVMC DefinirPropriedades(Intelbras.Message.Helper.MSG0185 xml)
        {
            throw new NotImplementedException();
        }

        private Intelbras.Message.Helper.MSG0185 DefinirPropriedades(VerbaVMC crm)
        {
            Intelbras.Message.Helper.MSG0185 xml = new Pollux.MSG0185(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.CNPJ, 40));
            Pollux.Entities.RegistraVMC registraVMC = new Pollux.Entities.RegistraVMC();

            registraVMC.CodigoConta = crm.CodigoConta;
            registraVMC.CNPJ = crm.CNPJ;
            registraVMC.Verba = crm.Verba;
            registraVMC.Trimestre = crm.Trimestre;
            registraVMC.Validade = crm.Validade;
            xml.RegistraVMC = registraVMC;
            return xml;
        }
        #endregion

        #region Métodos Auxiliares
        public string Enviar(VerbaVMC objModel)
        {
            string resposta;

            Intelbras.Message.Helper.MSG0185 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            try
            {

                if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
                {
                    Intelbras.Message.Helper.MSG0185R1 retorno = CarregarMensagem<Pollux.MSG0185R1>(resposta);
                    if (!retorno.Resultado.Sucesso)
                    {
                        return resposta;
                    }
                }
                else
                {
                    Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(resposta);
                    throw new Exception(erro001.GenerateMessage(false));
                }
                return resposta;
            }
            catch (Exception ex)
            {
                return "erro";
            }
            
        }
        #endregion        

    }
}
