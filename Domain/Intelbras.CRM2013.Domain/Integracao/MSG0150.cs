using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0150 : Base, IBase<Message.Helper.MSG0150, Domain.Model.Parecer>
    {

        #region Construtor

        public MSG0150(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }

        #endregion

        #region Propriedades
        //Dictionary que sera enviado como resposta do request

        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };


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
            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";

            List<Pollux.Entities.Parecer> lstObjeto = this.DefinirRetorno(this.CarregarMensagem<Pollux.MSG0150>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0150R1>(numeroMensagem, retorno);
            }

            Pollux.MSG0150R1 resposta = new Pollux.MSG0150R1(mensagem, numeroMensagem);
            resposta.ParecerItens = new List<Pollux.Entities.Parecer>();

            if (lstObjeto.Count > 0)
            {
                resposta.ParecerItens = lstObjeto;
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
                retorno.Add("ParecerItens", resposta.ParecerItens);
                retorno.Add("Resultado", resultadoPersistencia);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros no Crm.";
                retorno.Add("Resultado", resultadoPersistencia);

            }
            return CriarMensagemRetorno<Pollux.MSG0150R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public Parecer DefinirPropriedades(Intelbras.Message.Helper.MSG0150 xml)
        {
            Parecer retorno = new Parecer(this.Organizacao, this.IsOffline);
            return retorno;
        }
        public List<Pollux.Entities.Parecer> DefinirRetorno(Intelbras.Message.Helper.MSG0150 xml)
        {
            #region Propriedades Crm->Xml
            List<Pollux.Entities.Parecer> lstRetorno = new List<Pollux.Entities.Parecer>();

            if (!string.IsNullOrEmpty(xml.CodigoTarefa))
            {
                List<Parecer> lstParecer = new Intelbras.CRM2013.Domain.Servicos.ParecerService(this.Organizacao, this.IsOffline).ListarPor(new Guid(xml.CodigoTarefa));
                if (lstParecer.Count > 0)
                {
                    foreach (Parecer registro in lstParecer)
                    {
                        Pollux.Entities.Parecer tmpParecer = new Pollux.Entities.Parecer();

                        tmpParecer.CodigoParecer = registro.ID.Value.ToString();

                        if (String.IsNullOrEmpty(registro.Nome))
                            tmpParecer.NomeParecer = (String)this.PreencherAtributoVazio("string");
                        else
                            tmpParecer.NomeParecer = registro.Nome;

                        if (registro.TipodoParecer.HasValue)
                        {
                            tmpParecer.TipoParecer = registro.TipodoParecer.Value;
                            tmpParecer.NomeTipoParecer = new Intelbras.CRM2013.Domain.Enum.Parecer().TipoParecerNome(registro.TipodoParecer.Value);
                        }
                        else
                        {
                            tmpParecer.TipoParecer = (int)this.PreencherAtributoVazio("int");
                            tmpParecer.NomeTipoParecer = (string)this.PreencherAtributoVazio("string");
                        }


                        if (registro.KeyAccountouRepresentante != null)
                        {
                            Contato objContato = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(registro.KeyAccountouRepresentante.Id);
                            if (objContato != null)
                            {
                                tmpParecer.CodigoRepresentante = Convert.ToInt32(objContato.CodigoRepresentante);
                                tmpParecer.NomeRepresentante = registro.KeyAccountouRepresentante.Name;
                            }
                        }
                        else
                        {
                            tmpParecer.CodigoRepresentante = (int)this.PreencherAtributoVazio("int");
                            tmpParecer.NomeRepresentante = (String)this.PreencherAtributoVazio("string");
                        }

                        if (registro.Canal != null)
                        {
                            tmpParecer.CodigoConta = registro.Canal.Id.ToString();
                            tmpParecer.NomeConta = registro.Canal.Name;
                        }
                        else
                        {
                            tmpParecer.CodigoConta = (string)this.PreencherAtributoVazio("string");
                            tmpParecer.NomeConta = (String)this.PreencherAtributoVazio("string");
                        }

                        if (registro.Status.HasValue)
                        {
                            tmpParecer.Situacao = registro.Status.Value;
                            switch (registro.Status.Value)
                            {
                                case 0:
                                    tmpParecer.NomeSituacao = "Ativo";
                                    break;
                                case 1:
                                    tmpParecer.NomeSituacao = "Inativo";
                                    break;
                            }
                        }
                        else
                        {
                            tmpParecer.NomeSituacao = (String)this.PreencherAtributoVazio("string");
                            tmpParecer.Situacao = (Int32)this.PreencherAtributoVazio("int");
                        }

                        lstRetorno.Add(tmpParecer);
                    }

                    return lstRetorno;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Valor do parâmetro 'Código Tarefa' não existe";
                    return lstRetorno;
                }

            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Valor do parâmetro 'Código Tarefa' é obrigatório";
                return lstRetorno;
            }
            #endregion
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Parecer objModel)
        {
            string resposta = string.Empty;

            return resposta;
        }

        #endregion

        Parecer IBase<Pollux.MSG0150, Parecer>.DefinirPropriedades(Pollux.MSG0150 legado)
        {
            throw new NotImplementedException();
        }
    }
}
