using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0036 : Base, IBase<Pollux.MSG0036, Model.FamiliaComercial>
    {

        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private Domain.Model.Usuario usuarioIntegracao;

        #endregion

        #region Construtor

        public MSG0036(string org, bool isOffline)
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
            usuarioIntegracao = usuario;
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0036>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0036R1>(numeroMensagem, retorno);
            }

            objeto = new Intelbras.CRM2013.Domain.Servicos.FamiliaComercialService(this.Organizacao, this.IsOffline).Persistir(objeto);
            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro ao persistir Família Comercial!";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0036R1>(numeroMensagem, retorno);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            }

            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0036R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public FamiliaComercial DefinirPropriedades(Intelbras.Message.Helper.MSG0036 xml)
        {
            var crm = new FamiliaComercial(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(xml.Nome))
                crm.Nome = xml.Nome;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Nome não enviado.";
                return crm;
            }
            if (!String.IsNullOrEmpty(xml.CodigoFamiliaComercial))
                crm.Codigo = xml.CodigoFamiliaComercial;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Código Familia Comercial não enviado.";
                return crm;
            }
            #region Services
            //Segmento
            if (!String.IsNullOrEmpty(xml.Segmento))
            {
                Model.Segmento segmento = new Model.Segmento(this.Organizacao, this.IsOffline);
                segmento = new Intelbras.CRM2013.Domain.Servicos.SegmentoService(this.Organizacao, this.IsOffline).BuscaSegmento(xml.Segmento);

                if (segmento != null && segmento.ID.HasValue)
                    crm.Segmento = new Lookup(segmento.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Segmento não encontrado!";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Segmento não enviado.";
                return crm;
            }
            //Familia
            if (!String.IsNullOrEmpty(xml.Familia))
            {
                Model.FamiliaProduto familia = new Model.FamiliaProduto(this.Organizacao, this.IsOffline);
                familia = new Intelbras.CRM2013.Domain.Servicos.FamiliaProdutoService(this.Organizacao, this.IsOffline).BuscaFamiliaProduto(xml.Familia);

                if (familia != null && familia.ID.HasValue)
                    crm.Familia = new Lookup(familia.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "FamiliaProduto não encontrado!";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "FamiliaProduto não enviado.";
                return crm;
            }

            //SubfamiliaProduto
            if (!String.IsNullOrEmpty(xml.SubFamilia))
            {
                Model.SubfamiliaProduto subFamilia = new Model.SubfamiliaProduto(this.Organizacao, this.IsOffline);
                subFamilia = new Intelbras.CRM2013.Domain.Servicos.SubFamiliaProdutoService(this.Organizacao, this.IsOffline).BuscaSubfamiliaProduto(xml.SubFamilia);

                if (subFamilia != null && subFamilia.ID.HasValue)
                    crm.Subfamilia = new Lookup(subFamilia.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "SubfamiliaProduto não encontrado!";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "SubfamiliaProduto não enviado.";
                return crm;
            }
            //Origem
            if (!String.IsNullOrEmpty(xml.Origem))
            {
                Model.Origem origem = new Model.Origem(this.Organizacao, this.IsOffline);
                origem = new Intelbras.CRM2013.Domain.Servicos.OrigemService(this.Organizacao, this.IsOffline).BuscaOrigem(xml.Origem);

                if (origem != null && origem.ID.HasValue)
                    crm.Origem = new Lookup(origem.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Origem não encontrado!";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Origem não enviado.";
                return crm;
            }


            crm.Status = xml.Situacao;
            crm.IntegradoEm = DateTime.Now;
            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            crm.UsuarioIntegracao = xml.LoginUsuario;

            #endregion

            #endregion

            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(FamiliaComercial objModel)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
