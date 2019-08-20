using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.Crm.Util;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0123 : Base,IBase<Message.Helper.MSG0123,Domain.Model.EmpresasColigadas>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0123(string org, bool isOffline)
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
            //Trace.Add("Mensagem " + numeroMensagem + " XML: {0}", mensagem);
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0123>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0123R1>(numeroMensagem, retorno);
            }

            //Checa dentro da service se ele tentou mudar o proprietario,se positivo recusa e retorna erro
            bool mudancaProprietario = false;

            objeto = new Intelbras.CRM2013.Domain.Servicos.EmpresasColigadasService(this.Organizacao, this.IsOffline).Persistir(objeto,ref mudancaProprietario);

            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Registro não encontrado!";
            }
            else
            {
                if (mudancaProprietario == true)
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso, não houve alteração do proprietário.";
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                    retorno.Add("CodigoEmpresaColigada", objeto.ID.Value.ToString());
                }
                
            }
            retorno.Add("Resultado", resultadoPersistencia);
            
            return CriarMensagemRetorno<Pollux.MSG0123R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public EmpresasColigadas DefinirPropriedades(Intelbras.Message.Helper.MSG0123 xml)
        {
            var crm = new EmpresasColigadas(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(xml.CodigoEmpresaColigada))
                crm.ID = new Guid(xml.CodigoEmpresaColigada);

            if(!String.IsNullOrEmpty(xml.Nome))
                crm.RazaoSocial = xml.Nome;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador do Nome não encontrado!";
                return crm;
            }

            if (!string.IsNullOrEmpty(xml.Conta))
                crm.Conta = new Lookup( new Guid(xml.Conta), "");
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador do Canal não encontrado!";
                return crm;
            }


            crm.CpfCnpj = !String.IsNullOrEmpty(xml.CPF) ? xml.CPF : !String.IsNullOrEmpty(xml.CNPJ) ? xml.CNPJ : string.Empty;

            if (!String.IsNullOrEmpty(crm.CpfCnpj))
                crm.CpfCnpj = xml.CNPJ;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador do Cpf/Cnpf não encontrado!";
                return crm;
            }
            

            if(!String.IsNullOrEmpty(xml.Nacionalidade))
                crm.Nacionalidade = xml.Nacionalidade;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador da Nacionalidade não encontrado!";
                return crm;
            }

            crm.PorcentagemCapital = xml.PorcentagemCapital;

            crm.Status = xml.Situacao;

            #endregion

            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(EmpresasColigadas objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
