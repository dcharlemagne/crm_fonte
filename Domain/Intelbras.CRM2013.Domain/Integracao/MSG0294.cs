using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using Pollux = Intelbras.Message.Helper;
using System.Data;
using System.Linq;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0294 : Base, IBase<MSG0294, QuestionarioResposta>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0294(string org, bool isOffline) : base(org, isOffline)
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

        public string Executar(string mensagem, string numeroMensagem, Usuario usuario)
        {
            var xml = this.CarregarMensagem<Pollux.MSG0294>(mensagem);
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0294>(mensagem));

            if (resultadoPersistencia.Sucesso)
            {
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0294R1>(numeroMensagem, retorno);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0294R1>(numeroMensagem, retorno);
            }

            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0294R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public QuestionarioResposta DefinirPropriedades(Intelbras.Message.Helper.MSG0294 xml)
        {
            var crm = new Model.QuestionarioResposta(this.Organizacao, this.IsOffline);
            Conta conta = new Conta();
            string ReferenteServicoInstalacaoOpacao1 = SDKore.Configuration.ConfigurationManager.GetSettingValue("ReferenteServicoInstalacaoOpacao1");
            string ReferenteServicoInstalacaoOpacao2 = SDKore.Configuration.ConfigurationManager.GetSettingValue("ReferenteServicoInstalacaoOpacao2");
            string ReferenteServicoInstalacaoOpacao3 = SDKore.Configuration.ConfigurationManager.GetSettingValue("ReferenteServicoInstalacaoOpacao3");

            #region Opcoes de resposta

            List<QuestionarioOpcao> lstOpcoes = new Servicos.QuestionarioOpcaoServices(this.Organizacao, this.IsOffline).ListarPorContaId(new Guid(xml.Conta));
            foreach (QuestionarioOpcao questionarioOpcao in lstOpcoes)
            {
                var associada = xml.OpcoesResposta.Select(campo => campo.CodigoOpcao.ToString().ToUpper()).FirstOrDefault(c => c == questionarioOpcao.ID.ToString().ToUpper());
                QuestionarioResposta questionarioResposta = new QuestionarioRespostaServices(this.Organizacao, this.IsOffline).ObterPorOpcaoId((Guid)questionarioOpcao.ID, xml.Conta, false);
                if (associada == null)
                {
                    //Desativa no crm caso não esteja na lista da mensagem
                    new QuestionarioRespostaServices(this.Organizacao, this.IsOffline).Desativar((Guid)questionarioResposta.ID);
                }
                else
                {
                    //Ativa no crm caso esteja na lista da mensagem
                    new QuestionarioRespostaServices(this.Organizacao, this.IsOffline).Ativar((Guid)questionarioResposta.ID);
                }
            }

            foreach (var opcaoResposta in xml.OpcoesResposta)
            {
                var associada = lstOpcoes.Select(campo => campo.ID.ToString().ToUpper()).FirstOrDefault(c => c == opcaoResposta.CodigoOpcao.ToUpper());
                if (associada == null)
                {
                    //Cria o vínculo da Resposta com a conta
                    QuestionarioResposta questionarioRespostaCreate = new QuestionarioResposta();
                    questionarioRespostaCreate.QuestionarioOpcao = new Lookup(new Guid(opcaoResposta.CodigoOpcao), "itbc_questionarioresposta");
                    questionarioRespostaCreate.QuestionarioRespostaConta = new Lookup(new Guid(xml.Conta), "account");
                    questionarioRespostaCreate.Valor = opcaoResposta.ValorResposta;
                    new RepositoryService(this.Organizacao, this.IsOffline).QuestionarioResposta.Create(questionarioRespostaCreate);
                }
                else
                {
                    //Altera o vínculo da Resposta com a conta
                    QuestionarioResposta questionarioRespostaUpdate = new QuestionarioResposta();
                    QuestionarioResposta questionarioResposta = new QuestionarioRespostaServices(this.Organizacao, this.IsOffline).ObterPorOpcaoId(new Guid(opcaoResposta.CodigoOpcao), xml.Conta, false);
                    questionarioRespostaUpdate.QuestionarioOpcao = new Lookup(new Guid(opcaoResposta.CodigoOpcao), "itbc_questionarioresposta");
                    questionarioRespostaUpdate.ID = questionarioResposta.ID;
                    questionarioRespostaUpdate.Valor = opcaoResposta.ValorResposta;
                    new RepositoryService(this.Organizacao, this.IsOffline).QuestionarioResposta.Update(questionarioRespostaUpdate);
                }

                //Caso a opção de resposta seja “Instalamos os produtos independentemente de onde foram comprados” o campo Instalador da conta deve ser atualizado
                if (opcaoResposta.CodigoOpcao == ReferenteServicoInstalacaoOpacao2)
                    conta.Instalador = true;
                else if (opcaoResposta.CodigoOpcao == ReferenteServicoInstalacaoOpacao1 || opcaoResposta.CodigoOpcao == ReferenteServicoInstalacaoOpacao3)
                    conta.Instalador = false;
            }
            #endregion

            #region Segmentos Comerciais

            //Desassociar Segmento comercial à conta
            List<SegmentoComercial> listaAtual = new SegmentoComercialService(this.Organizacao, this.IsOffline).ListarSegmentoPorConta(xml.Conta);
            SegmentoComercialService segmentoComercialService = new SegmentoComercialService(this.Organizacao, this.IsOffline);
            if (listaAtual != null && listaAtual.Count > 0)
            {
                segmentoComercialService.DesassociarSegmentoComercial(listaAtual, new Guid(xml.Conta));
            }

            //Associar Segmento comercial à conta
            List<SegmentoComercial> listaSegmentoComercial = new List<SegmentoComercial>();
            foreach (var segmentosComercialPollux in xml.SegmentosComerciais)
            {
                SegmentoComercial segmentosComercialGuid = new SegmentoComercialService(this.Organizacao, this.IsOffline).ObterPorCodigo(segmentosComercialPollux.CodigoSegmentoComercial);
                listaSegmentoComercial.Add(segmentosComercialGuid);
            }
            if (listaSegmentoComercial != null && listaSegmentoComercial.Count > 0)
            {
                segmentoComercialService.AssociarSegmentoComercial(listaSegmentoComercial, new Guid(xml.Conta));
            }
            #endregion

            #region Marcas
            if (xml.Marcas != null)
            {
                conta.MarcasEnviadas = xml.Marcas;
            }
            else
            {
                conta.MarcasEnviadas = " ";
                //conta.AddNullProperty("itbc_marcas_enviadas");
            }
            conta.ID = new Guid(xml.Conta);
            conta.IntegrarNoPlugin = true;
            new RepositoryService(this.Organizacao, this.IsOffline).Conta.Update(conta);
            MarcasServices marcasService = new MarcasServices(this.Organizacao, this.IsOffline);
            List<Marcas> lstMarcas = new List<Marcas>();
            List<Marcas> lstMarcasAssociadas = new MarcasServices(this.Organizacao, this.IsOffline).ListarMarcasPorConta(xml.Conta);

            if (xml.Marcas != null)
            {
                string[] Sinonimos = xml.Marcas.Split(';');
                foreach (var marcaSplit in Sinonimos)
                {
                    Marcas marca = new MarcasServices(this.Organizacao, this.IsOffline).obterPorNome(marcaSplit);
                    if (marca != null)
                    {
                        var marcasDuplicadas = lstMarcas.Select(campo => campo.Id).FirstOrDefault(c => c == marca.Id);
                        if (marcasDuplicadas == Guid.Empty)
                        {
                            lstMarcas.Add(marca);
                            continue;
                        }
                    }
                    else if (new SinonimosMarcasServices(this.Organizacao, this.IsOffline).obterPorNome(marcaSplit) != null)
                    {
                        SinonimosMarcas sinonimosMarcas = new SinonimosMarcasServices(this.Organizacao, this.IsOffline).obterPorNome(marcaSplit);
                        if (sinonimosMarcas.Marca != null)
                        {
                            Marcas marcaAssociada = (new CRM2013.Domain.Servicos.RepositoryService()).Marcas.Retrieve((sinonimosMarcas.Marca.Id));
                            var vinculoConta = lstMarcas.Select(campo => campo.Id).FirstOrDefault(c => c == marcaAssociada.Id); //Verifica se a marca já está associada á conta
                            if (vinculoConta == Guid.Empty)
                            {
                                lstMarcas.Add(marcaAssociada);
                                continue;
                            }
                        }
                    }
                    else //Caso a marca ou sinônimo da marca enviada não exista, cria.
                    {
                        SinonimosMarcas sinonimosMarcasCreate = new SinonimosMarcas();
                        sinonimosMarcasCreate.Nome = marcaSplit;
                        new RepositoryService(this.Organizacao, this.IsOffline).SinonimosMarcas.Create(sinonimosMarcasCreate);
                    }
                }
                if (lstMarcas.Count > 0)
                {
                    marcasService.desassociarMarcas(lstMarcasAssociadas, new Guid(xml.Conta));
                    marcasService.associarMarcas(lstMarcas, new Guid(xml.Conta));
                }
            }
            else
            {
                marcasService.desassociarMarcas(lstMarcasAssociadas, new Guid(xml.Conta));
            }
            #endregion

            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(QuestionarioResposta objModel)
        {
            return String.Empty;
        }

        public QuestionarioResposta DefinirPropriedades(MSG0294 legado)
        {

            throw new NotImplementedException();
        }
        #endregion
    }
}
