using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0111 : Base, IBase<Intelbras.Message.Helper.MSG0111, Domain.Model.ParametroGlobal>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        #region Parametros Services
        int tipoParamentroCod;
        Guid? unidadeNegocioId = null;
        Guid? classificacaoId = null;
        Guid? categoriaId = null;
        Guid? nivelPosVendaId = null;
        Guid? compromissoId = null;
        Guid? beneficioId = null;


        #endregion
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0111(string org, bool isOffline)
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
            Intelbras.Message.Helper.MSG0111 xml = this.CarregarMensagem<Pollux.MSG0111>(mensagem);
            ParametroGlobal objeto = this.DefinirPropriedades(xml);

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0111R1>(numeroMensagem, retorno);
            }

            objeto = new Domain.Servicos.ParametroGlobalService(this.Organizacao, this.IsOffline).ObterPor(tipoParamentroCod, unidadeNegocioId, classificacaoId, categoriaId, nivelPosVendaId, compromissoId, beneficioId,null);

            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Registro não encontrado!";
            }
            else
            {
                Pollux.Entities.ParametroGlobal objRetorno = new Pollux.Entities.ParametroGlobal();
                objRetorno.TipoDado = objeto.TipoDado;
                objRetorno.Valor = objeto.Valor;

                retorno.Add("ParametroGlobal", objRetorno);

                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            }

            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0111R1>(numeroMensagem, retorno);
        }
        //public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        //{
        //     try
        //    {
        //    //Trace.Add("Mensagem " + numeroMensagem + " XML: {0}", mensagem);
        //    usuarioIntegracao = usuario;
        //    Intelbras.Message.Helper.MSG0111 xml = this.CarregarMensagem<Pollux.MSG0111>(mensagem);
        //    ParametroGlobal objeto = this.DefinirPropriedades(xml);

        //    if (!resultadoPersistencia.Sucesso)
        //    {
        //        retorno.Add("Resultado", resultadoPersistencia);
        //        return CriarMensagemRetorno<Pollux.MSG0111R1>(numeroMensagem, retorno);
        //    }

        //    objeto = new Domain.Servicos.ParametroGlobalService(this.Organizacao, this.IsOffline).ObterPor(tipoParamentroCod, unidadeNegocioId, classificacaoId, categoriaId, nivelPosVendaId, compromissoId, beneficioId,null);

        //    if (objeto == null)
        //    {
        //        resultadoPersistencia.Sucesso = true;
        //        resultadoPersistencia.Mensagem = "Registro não encontrado!";
        //    }
        //    else
        //    {
        //        Pollux.Entities.ParametroGlobal objRetorno = new Pollux.Entities.ParametroGlobal();
        //        objRetorno.TipoDado = objeto.TipoDado;
        //        objRetorno.Valor = objeto.Valor;

        //        retorno.Add("ParametroGlobal", objRetorno);

        //        resultadoPersistencia.Sucesso = true;
        //        resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
        //    }

        //    retorno.Add("Resultado", resultadoPersistencia);
        //    return CriarMensagemRetorno<Pollux.MSG0111R1>(numeroMensagem, retorno);
        //    }
        //     catch (Exception e)
        //     {
        //         resultadoPersistencia.Sucesso = false;
        //         resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(e);
        //         retorno.Add("Resultado", resultadoPersistencia);
        //         return CriarMensagemRetorno<Pollux.MSG0111R1>(numeroMensagem, retorno);
        //         //throw new ArgumentException(e.Message);
        //     }
        //}

        #endregion

        #region Definir Propriedades

        public ParametroGlobal DefinirPropriedades(Intelbras.Message.Helper.MSG0111 xml)
        {
            ParametroGlobal crm = new ParametroGlobal(this.Organizacao, this.IsOffline);

            //Obrigatorio
            TipodeParametroGlobal tipoParamentro = new Servicos.TipodeParametroGlobalService(this.Organizacao, this.IsOffline).ObterPor(xml.TipoParametroGlobal);
            if (tipoParamentro != null && tipoParamentro.Codigo.HasValue)
            {
                tipoParamentroCod = tipoParamentro.Codigo.Value;
                crm.TipoParametro = new Lookup(tipoParamentro.ID.Value, "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "TipoParametroGlobal/Codigo não cadastrada no Crm.";
                return crm;
            }

            //Não Obrigatório
            if (!String.IsNullOrEmpty(xml.CodigoBeneficio))
            {
                if (xml.CodigoBeneficio.Length == 36)
                    crm.Beneficio = new Lookup((Guid)(TransformaGuid(xml.CodigoBeneficio)), "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Identificador CodigoBeneficio fora do padrão (Guid).";
                    return crm;
                }
            }

            //Não obrigatorio
            if (!String.IsNullOrEmpty(xml.CodigoClassificacao))
            {
                Classificacao classificacao = new Servicos.ClassificacaoService(this.Organizacao, this.IsOffline).BuscaClassificacao(new Guid(xml.CodigoClassificacao));
                if (classificacao != null)
                {
                    crm.Classificacao = new Lookup(classificacao.ID.Value, "");
                    classificacaoId = classificacao.ID.Value;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Classificação não cadastrada no Crm.";
                    return crm;
                }
            }

            //Não obrigatorio
            if (!String.IsNullOrEmpty(xml.CodigoCategoria))
            {
                Categoria categoria = new Servicos.CategoriaService(this.Organizacao, this.IsOffline).ObterPor(new Guid(xml.CodigoCategoria));
                if (categoria != null)
                {
                    crm.Categoria = new Lookup(categoria.ID.Value, "");
                    categoriaId = categoria.ID.Value;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Categoria não cadastrada no Crm.";
                    return crm;
                }
            }

            //Não Obrigatorio
            if (!String.IsNullOrEmpty(xml.CodigoCompromisso))
            {
                if (xml.CodigoCompromisso.Length == 36)
                {
                    crm.Compromisso = new Lookup(new Guid(xml.CodigoCompromisso), "");
                    compromissoId = new Guid(xml.CodigoCompromisso);
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Identificador CodigoCompromisso fora do padrão (Guid).";
                    return crm;
                }
            }


            //Não Obrigatorio
            if (!String.IsNullOrEmpty(xml.CodigoBeneficio))
            {
                if (xml.CodigoBeneficio.Length == 36)
                {
                    crm.Beneficio = new Lookup(new Guid(xml.CodigoBeneficio), "");
                    beneficioId = new Guid(xml.CodigoBeneficio);
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Identificador CodigoBeneficio fora do padrão (Guid).";
                    return crm;
                }
            }

            //Não Obrigatorio
            if (!String.IsNullOrEmpty(xml.CodigoUnidadeNegocio))
            {
                UnidadeNegocio unidadeNeg = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocioPorChaveIntegracao(xml.CodigoUnidadeNegocio);
                if (unidadeNeg != null)
                {
                    crm.UnidadeNegocio = new Lookup(unidadeNeg.ID.Value, "");
                    unidadeNegocioId = unidadeNeg.ID.Value;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "UnidadeNegocio não cadastrada no Crm.";
                    return crm;
                }
            }
            //Não Obrigatorio
            if (!String.IsNullOrEmpty(xml.CodigoNivelPosVenda))
            {
                if (xml.CodigoNivelPosVenda.Length == 36)
                {
                    crm.NivelPosVenda = new Lookup(new Guid(xml.CodigoNivelPosVenda), "");
                    nivelPosVendaId = new Guid(xml.CodigoNivelPosVenda);
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Identificador NivelPosVenda fora do padrão (Guid).";
                    return crm;
                }

            }

            return crm;

        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(ParametroGlobal objModel)
        {
            throw new NotImplementedException();
        }

        private Guid? TransformaGuid(string valor)
        {
            if (!String.IsNullOrEmpty(valor))
            {
                return new Guid(valor);
            }

            return null;
        }
        #endregion
    }
}