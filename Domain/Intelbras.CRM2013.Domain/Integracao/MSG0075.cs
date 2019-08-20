using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0075 : Base, IBase<Pollux.MSG0075, Model.RegiaoAtuacao>
    {

        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        #endregion

        #region Construtor

        public MSG0075(string org, bool isOffline)
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
            usuarioIntegracao = usuario;
            var xml = this.CarregarMensagem<Pollux.MSG0075>(mensagem);

            var objeto = this.DefinirPropriedades(xml);

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0075R1>(numeroMensagem, retorno);
            }

            if (objeto.Canal.HasValue)
            {
                List<RegiaoAtuacao> objRegiaoAtuacao = new Servicos.RegiaoAtuacaoServices(this.Organizacao, this.IsOffline).ListarRegiaoAtuacao(objeto.Canal.Value);


                if (objRegiaoAtuacao != null && objRegiaoAtuacao.Count > 0)
                {
                    List<Pollux.Entities.Regiao> lstRegiao = new List<Pollux.Entities.Regiao>();
                    foreach (RegiaoAtuacao regiaoAtuacaoItem in objRegiaoAtuacao)
                    {
                        Pollux.Entities.Regiao regiaoItem = new Pollux.Entities.Regiao();
                        if (regiaoAtuacaoItem.MunicipioId.HasValue)
                        {
                            Municipio objMunicipio = new Servicos.EnderecoServices(this.Organizacao, this.IsOffline).ObterMunicipio(regiaoAtuacaoItem.MunicipioId.Value);

                            regiaoItem.ChaveIntegracaoCidade = objMunicipio.ChaveIntegracao;
                            regiaoItem.NomeCidade = objMunicipio.Nome;
                            lstRegiao.Add(regiaoItem);
                        }
                    }
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
                    retorno.Add("RegioesItens", lstRegiao);
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Relacionamento não encontrado no CRM.";
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Indentificador do Canal não enviado";
            }

            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0075R1>(numeroMensagem, retorno);

        }
        #endregion

        #region Definir Propriedades
        public RegiaoAtuacao DefinirPropriedades(Intelbras.Message.Helper.MSG0075 xml)
        {

            var crm = new RegiaoAtuacao(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(xml.CodigoConta))
            {
                Model.Conta conta = new Model.Conta(this.Organizacao, this.IsOffline);
                conta = new Intelbras.CRM2013.Domain.Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(new Guid(xml.CodigoConta));
                if (conta != null)
                    crm.Canal = conta.ID.Value;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Canal não encontrado!";
                return crm;
            }

            #endregion

            return crm;

        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(RegiaoAtuacao objModel)
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
