using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;
using SDKore.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0182 : Base, IBase<Intelbras.Message.Helper.MSG0182, Domain.Model.CNAE>
    {
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        public MSG0182(string org, bool isOffline) : base(org, isOffline) { }

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            try
            {
                retorno.Add("Resultado", this.resultadoPersistencia);

                var xml = this.CarregarMensagem<Pollux.MSG0182>(mensagem);
                var objeto = this.DefinirPropriedades(xml);

                new Servicos.CnaeService(Organizacao, IsOffline).Update(objeto);

                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            }
            catch (Exception ex)
            {
                resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(ex);
                resultadoPersistencia.Sucesso = false;
            }

            return CriarMensagemRetorno<Pollux.MSG0182R1>(numeroMensagem, retorno);
        }

        public Model.CNAE DefinirPropriedades(Intelbras.Message.Helper.MSG0182 xml)
        {
            if (string.IsNullOrEmpty(xml.Classe))
            {
                throw new ArgumentException("(CRM) O campo 'Classe' é obrigatório!");
            }

            if (string.IsNullOrEmpty(xml.Subclasse))
            {
                throw new ArgumentException("(CRM) O campo 'SubClasse' é obrigatório!");
            }

            if (string.IsNullOrEmpty(xml.Denominacao))
            {
                throw new ArgumentException("(CRM) O campo 'Denominacao' é obrigatório!");
            }

            if (string.IsNullOrEmpty(xml.Grupo))
            {
                throw new ArgumentException("(CRM) O campo 'Grupo' é obrigatório!");
            }

            if (string.IsNullOrEmpty(xml.Secao))
            {
                throw new ArgumentException("(CRM) O campo 'Secao' é obrigatório!");
            }

            if (string.IsNullOrEmpty(xml.Nome))
            {
                throw new ArgumentException("(CRM) O campo 'Nome' é obrigatório!");
            }

            if (string.IsNullOrEmpty(xml.CnaeId))
            {
                throw new ArgumentException("(CRM) O campo 'CnaeId' é obrigatório!");
            }

            var cnae = new CNAE(this.Organizacao, this.IsOffline)
            {
                ID = new Guid(xml.CnaeId),
                Nome = xml.Nome,
                Classe = xml.Classe,
                SubClasse = xml.Subclasse,
                Denominacao = xml.Denominacao,
                Divisao = xml.Divisao,
                Secao = xml.Secao,
                Grupo = xml.Grupo
            };

            return cnae;
        }

        public Intelbras.Message.Helper.MSG0182 DefinirPropriedades(CNAE cnae)
        {
            var xml = new Intelbras.Message.Helper.MSG0182(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), cnae.Nome.Truncate(40));

            xml.Nome = cnae.Nome;
            xml.CnaeId = cnae.ID.Value.ToString();
            xml.Denominacao = cnae.Denominacao;
            xml.Secao = cnae.Secao;
            xml.Divisao = cnae.Divisao;
            xml.Classe = cnae.Classe;
            xml.Subclasse = cnae.SubClasse;
            xml.Grupo = cnae.Grupo;

            return xml;
        }

        public string Enviar(CNAE objModel)
        {
            var mensagem = DefinirPropriedades(objModel);
            var integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            string retMsg = string.Empty;

            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out retMsg))
            {
                var retorno = CarregarMensagem<Pollux.MSG0182R1>(retMsg);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new ArgumentException("(CRM) " + retorno.Resultado.Mensagem);
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(retMsg);
                throw new ArgumentException(string.Concat("(CRM) Erro de Integração \n", erro001.GenerateMessage(false)));
            }
            return retMsg;
        }
    }
}