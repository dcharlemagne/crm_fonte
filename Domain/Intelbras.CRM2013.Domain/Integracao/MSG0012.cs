using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain;


namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0012 : Base, IBase<Intelbras.Message.Helper.MSG0012, Domain.Model.Municipio>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private Domain.Model.Usuario usuarioIntegracao;

        #endregion

        #region Construtor
        public MSG0012(string org, bool isOffline)
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

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            try
            {
                usuarioIntegracao = usuario;
                var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0012>(mensagem));

                objeto = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).Persistir(objeto);

                if (objeto == null)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Registro não encontrado!";
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                }

                retorno.Add("Resultado", resultadoPersistencia);

                return CriarMensagemRetorno<Pollux.MSG0012R1>(numeroMensagem, retorno);
            }
            catch (ArgumentException ex)
            {
                resultadoPersistencia.Mensagem = ex.Message;
                resultadoPersistencia.Sucesso = false;

                return CriarMensagemRetorno<Pollux.MSG0012R1>(numeroMensagem, retorno);
            }
        }

        #region Definir Propriedades

        public Municipio DefinirPropriedades(Intelbras.Message.Helper.MSG0012 xml)
        {
            if (string.IsNullOrWhiteSpace(xml.Nome))
            {
                throw new ArgumentException("(CRM) Nome não pode ser vazio");
            }

            if (string.IsNullOrWhiteSpace(xml.Estado))
            {
                throw new ArgumentException("(CRM) Estado não pode ser vazio");
            }

            if (string.IsNullOrWhiteSpace(xml.ChaveIntegracao))
            {
                throw new ArgumentException("(CRM) ChaveIntegracao não pode ser vazio");
            }

            var estado = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaEstado(xml.Estado);

            if (estado == null)
            {
                throw new ArgumentException("(CRM) Estado não encontrado no CRM");
            }

            #region Propriedades Crm->Xml

            var crm = new Municipio(this.Organizacao, this.IsOffline);
            crm.Nome = xml.Nome;
            crm.Estadoid = new Lookup(estado.ID.Value, "");
            crm.ChaveIntegracao = xml.ChaveIntegracao;
            crm.State = xml.Situacao;
            crm.IntegradoEm = DateTime.Now;
            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            crm.UsuarioIntegracao = xml.LoginUsuario;
            crm.CodigoIbge = xml.CodigoIBGE;

            if (xml.CodigoIBGE.HasValue)
                crm.CodigoIbge = xml.CodigoIBGE;
            else
                crm.AddNullProperty("CodigoIbge");

            #endregion

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Municipio objModel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
