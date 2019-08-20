using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0105 : Base, IBase<Message.Helper.MSG0105, Domain.Model.AcessoKonviva>
    {

        #region Construtor
        
        public MSG0105(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0105>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0105R1>(numeroMensagem, retorno);
            }

            objeto = new Servicos.AcessoKonvivaService(this.Organizacao, this.IsOffline).Persistir(objeto);

            if (objeto != null)
            {
                retorno.Add("CodigoAcessoKonviva", objeto.ID.Value.ToString());
                retorno.Add("IdentificadorUnidadeKonviva", objeto.IdUnidadeAcessoKonviva);
                retorno.Add("Proprietario", usuarioIntegracao.ID.Value.ToString());
                retorno.Add("TipoProprietario", "systemuser");
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";
                retorno.Add("Resultado", resultadoPersistencia);
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro de Persistência!";
                retorno.Add("Resultado", resultadoPersistencia);
            }
            return CriarMensagemRetorno<Pollux.MSG0105R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public AcessoKonviva DefinirPropriedades(Intelbras.Message.Helper.MSG0105 xml)
        {
            var crm = new AcessoKonviva(this.Organizacao, this.IsOffline);
            Guid idClassificacaoKonviva = Guid.Empty;
            Lookup classificacaoKonviva;
            Guid organizacaoPadrao;

            Guid conta = Guid.Empty;
            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(xml.NomeAcessoKonviva))
            {
                crm.Nome = xml.NomeAcessoKonviva;
            }
            else 
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "NomeAcessoKonviva não enviado!";
                return crm;
            }

            xml.Situacao = (crm.Status.HasValue ? crm.Status.Value : 0);
            //if (xml.Situacao == 1 || xml.Situacao == 0)
            //    crm.Status = xml.Situacao;
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Situação não enviada!";
            //    return crm;
            //}

            if (!String.IsNullOrEmpty(xml.CodigoAcessoKonviva))
            {
                Guid konviva;
                if (Guid.TryParse(xml.CodigoAcessoKonviva, out konviva))
                    crm.ID = konviva;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Guid AcessoKonviva Formato incorreto";
                    return crm;
                }
            }
            //Seção comentada enquanto não é decidido a documentação
            //Conta não é obrigatório então caso nao tenha preenchido a classificação assume um default
            //if (String.IsNullOrEmpty(xml.CodigoConta))
            //{
            //    if (false == Guid.TryParse(SDKore.Configuration.ConfigurationManager.GetSettingValue("ClassificacaoPadraoKonviva"), out idClassificacaoKonviva))
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Guid Classificação Konviva padrão em formato incorreto";
            //        return crm;
            //    }

            //    Classificacao classif = new Intelbras.CRM2013.Domain.Servicos.ClassificacaoService(this.Organizacao,this.IsOffline).BuscaClassificacao(idClassificacaoKonviva);
                
            //    if(classif == null)
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Classificação do Canal não encontrada.";
            //        return crm;
            //    }

            //    classificacaoKonviva = new Lookup(classif.ID.Value,classif.Nome,"");
            //}
            //else
            //{
            //    if (false == Guid.TryParse(xml.CodigoConta,out conta))
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Guid conta em formato incorreto";
            //        return crm;
            //    }

            //    Model.Conta mConta = new Intelbras.CRM2013.Domain.Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(conta);
            //    if (conta == null)
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Conta não encontrada.";
            //        return crm;
            //    }

            //    if (mConta.Classificacao == null)
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Classificação da conta obrigatório.";
            //        return crm;
            //    }
            //    classificacaoKonviva = mConta.Classificacao;
            //}


            if (!Guid.TryParse(SDKore.Configuration.ConfigurationManager.GetSettingValue("organizacaoPadraoKonviva"), out organizacaoPadrao))
                    throw new Exception("Variável organizacaoPadraoKonviva do SDKore.config não foi configurada corretamente.");

            UnidadeKonviva unKonviva = new Intelbras.CRM2013.Domain.Servicos.UnidadeKonvivaService(this.Organizacao, this.IsOffline).ObterPor(organizacaoPadrao);

            if (unKonviva == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Unidade Konviva não encontrada.";
                return crm;
            }
            crm.UnidadeKonviva = new Lookup(unKonviva.ID.Value, "");

            if(unKonviva.IdInterno.HasValue)
                crm.IdUnidadeAcessoKonviva = unKonviva.IdInterno;

            //if (xml.IdentificadorUnidadeKonviva.HasValue)
            //{
            //    UnidadeKonviva unidadeKonviva = new Servicos.UnidadeKonvivaService(this.Organizacao, this.IsOffline).ObterPor(xml.IdentificadorUnidadeKonviva.Value);

            //    if (unidadeKonviva != null)
            //    {
            //        crm.UnidadeKonviva = new Lookup(unidadeKonviva.ID.Value, "");
            //        crm.IdUnidadeAcessoKonviva = xml.IdentificadorUnidadeKonviva;
            //    }
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "UnidadeKonviva não cadastrado no Crm!";
            //        return crm;
            //    }
            //}
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "IdentificadorUnidadeKonviva não enviado!";
            //    return crm;
            //}

            if (!String.IsNullOrEmpty(xml.CodigoConta))
            {
                crm.Conta = new Lookup(new Guid(xml.CodigoConta), "");
            }

            if (!String.IsNullOrEmpty(xml.CodigoContato) && xml.CodigoContato.Length == 36)
            {
                crm.Contato = new Lookup(new Guid(xml.CodigoContato), "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoContato inválido ou não enviado!";
                return crm;
            }

            if (xml.PerfilAluno.HasValue)
            {
                crm.PerfilAluno = xml.PerfilAluno;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "PerfilAluno não enviado!";
                return crm;
            }
            if (xml.PerfilGestor.HasValue)
            {
                crm.PerfilGestor = xml.PerfilGestor;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "PerfilGestor não enviado!";
                return crm;
            }

            if (xml.PerfilAutor.HasValue)
            {
                crm.PerfilAutor = xml.PerfilAutor;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "PerfilAutor não enviado!";
                return crm;
            }

            if (xml.PerfilAdministrador.HasValue)
            {
                crm.PerfilAdministrador = xml.PerfilAdministrador;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "PerfilAdministrador não enviado!";
                return crm;
            }

            if (xml.PerfilMonitor.HasValue)
            {
                crm.PerfilMonitor = xml.PerfilMonitor;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "PerfilMonitor não enviado!";
                return crm;
            }

            if (xml.PerfilModerador.HasValue)
            {
                crm.PerfilModerador = xml.PerfilModerador;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "PerfilModerador não enviado!";
                return crm;
            }

            if (xml.PerfilInstrutor.HasValue)
            {
                crm.PerfilInstrutor = xml.PerfilInstrutor;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "PerfilInstrutor não enviado!";
                return crm;
            }

            if (xml.PerfilAnalista.HasValue)
            {
                crm.PerfilAnalista = xml.PerfilAnalista;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "PerfilAnalista não enviado!";
                return crm;
            }

            if (xml.PerfilTutor.HasValue)
            {
                crm.PerfilTutor = xml.PerfilTutor;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "PerfilTutor não enviado!";
                return crm;
            }

            crm.IntegrarNoPlugin = true;
            
            #endregion

            return crm;
        }

        public Pollux.MSG0105 DefinirPropriedades(AcessoKonviva objModel)
        {
            #region Propriedades Crm->Xml

            if(String.IsNullOrEmpty(objModel.Nome))
                objModel.Nome = (string)this.PreencherAtributoVazio("string");

            Pollux.MSG0105 msg0105 = new Pollux.MSG0105(itb.RetornaSistema(itb.Sistema.CRM), Intelbras.CRM2013.Domain.Servicos.Helper.Truncate(objModel.Nome, 40));

            if (objModel.Conta != null)
            {
                msg0105.CodigoConta = objModel.Conta.Id.ToString();
            }

            if (objModel.Contato != null)
            {
                msg0105.CodigoContato = objModel.Contato.Id.ToString();
            }

            msg0105.CodigoAcessoKonviva = objModel.ID.Value.ToString();

            if (objModel.UnidadeKonviva != null)
            {
                Model.UnidadeKonviva uniAcessoKonviva = new Intelbras.CRM2013.Domain.Servicos.UnidadeKonvivaService(this.Organizacao, this.IsOffline).ObterPor(objModel.UnidadeKonviva.Id);
                
                if(uniAcessoKonviva != null && uniAcessoKonviva.IdInterno.HasValue)
                    msg0105.IdentificadorUnidadeKonviva = uniAcessoKonviva.IdInterno.Value;
            }

            if (!string.IsNullOrEmpty(objModel.Nome))
                msg0105.NomeAcessoKonviva = objModel.Nome;
            else
                msg0105.NomeAcessoKonviva = (string)this.PreencherAtributoVazio("string");

            if(objModel.PerfilAdministrador.HasValue)
                msg0105.PerfilAdministrador = objModel.PerfilAdministrador;

            if (objModel.PerfilAluno.HasValue)
                msg0105.PerfilAluno = objModel.PerfilAluno;

            if (objModel.PerfilAnalista.HasValue)
                msg0105.PerfilAnalista = objModel.PerfilAnalista;

            if (objModel.PerfilAutor.HasValue)
                msg0105.PerfilAutor = objModel.PerfilAutor;

            if (objModel.PerfilGestor.HasValue)
                msg0105.PerfilGestor = objModel.PerfilGestor;

            if (objModel.PerfilInstrutor.HasValue)
                msg0105.PerfilInstrutor = objModel.PerfilInstrutor;

            if (objModel.PerfilModerador.HasValue)
                msg0105.PerfilModerador = objModel.PerfilModerador;

            if (objModel.PerfilMonitor.HasValue)
                msg0105.PerfilMonitor = objModel.PerfilMonitor;

            if (objModel.PerfilTutor.HasValue)
                msg0105.PerfilTutor = objModel.PerfilTutor;

            if (usuarioIntegracao != null)
            {
                msg0105.Proprietario = usuarioIntegracao.ID.Value.ToString();
                msg0105.TipoProprietario = "systemuser";
            }else
            {
                msg0105.Proprietario = "259A8E4F-15E9-E311-9420-00155D013D39";
                msg0105.TipoProprietario = "systemuser";
            }
            
            msg0105.Situacao = (objModel.Status.HasValue ? objModel.Status.Value : (int)Enum.StateCode.Ativo);

            #endregion

            return msg0105;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(AcessoKonviva objModel)
        {
            string retMsg = String.Empty;

            Intelbras.Message.Helper.MSG0105 mensagem = this.DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out retMsg))
            {
                Intelbras.Message.Helper.MSG0105R1 retorno = CarregarMensagem<Pollux.MSG0105R1>(retMsg);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new ArgumentException("(CRM) " + string.Concat(retorno.Resultado.Mensagem));
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(retMsg);
                throw new ArgumentException("(CRM) " + string.Concat("Erro de Integração \n", erro001.GenerateMessage(false)));
            }
            return retMsg;
        }

        #endregion

    }
}
