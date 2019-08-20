using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using Pollux = Intelbras.Message.Helper;
using System.Data;
using System.Linq;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Text;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0306 : Base, IBase<Message.Helper.MSG0306, Domain.Model.SegmentoComercial>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private List<string> listaErros = new List<string>();
        private Domain.Model.Usuario usuarioIntegracao;
        #endregion

        #region Construtor
        public MSG0306(string org, bool isOffline) : base(org, isOffline)
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
            usuarioIntegracao = usuario;
            var xml = this.CarregarMensagem<Pollux.MSG0306>(mensagem);
            var lstOrdenada = xml.SegmentosComerciais.OrderBy(s => s.CodigoSegmentoComercialPai).ThenBy(s => s.TipoSegmentoComercial).ToList();

            try
            {
                foreach (var item in lstOrdenada)
                {
                    SegmentoComercial segmentoComercial = new Servicos.SegmentoComercialService(this.Organizacao, this.IsOffline).ObterPorCodigo(item.CodigoSegmentoComercial);

                    if (segmentoComercial != null)
                    {
                        if (item.Situacao == (int)Enum.StateCode.Inativo && segmentoComercial.Status == (int)Enum.StateCode.Ativo)
                        {
                            try
                            {
                                new SegmentoComercialService(this.Organizacao, this.IsOffline).Desativar((Guid)segmentoComercial.ID);
                            }
                            catch (Exception ex)
                            {
                                listaErros.Add("Erro ao desativar o Segmento Comercial: " + item.CodigoSegmentoComercial + " Nome: " + item.NomeSegmentoComercial);
                                continue;
                            }
                        }
                        else
                        {
                            if (item.Situacao == (int)Enum.StateCode.Ativo && segmentoComercial.Status == (int)Enum.StateCode.Inativo)
                            {
                                try
                                {
                                    new SegmentoComercialService(this.Organizacao, this.IsOffline).Ativar((Guid)segmentoComercial.ID);
                                    segmentoComercial = new Servicos.SegmentoComercialService(this.Organizacao, this.IsOffline).ObterPorCodigo(item.CodigoSegmentoComercial);
                                }
                                catch (Exception ex)
                                {
                                    listaErros.Add("Erro ao ativar o Segmento Comercial: " + item.CodigoSegmentoComercial + " Nome: " + item.NomeSegmentoComercial);
                                    continue;
                                }
                            }

                            //Atualiza registro
                            try
                            {
                                segmentoComercial.CodigoSegmento = Convert.ToInt32(item.CodigoSegmentoComercial);

                                if (!string.IsNullOrEmpty(item.CodigoSegmentoComercialPai))
                                {
                                    try
                                    {
                                        SegmentoComercial obj = new Servicos.SegmentoComercialService(this.Organizacao, this.IsOffline).ObterPorCodigo(item.CodigoSegmentoComercialPai);
                                        segmentoComercial.CodigoSegmentoPai = new SDKore.DomainModel.Lookup(obj.Id, "itbc_segmentocomercial");
                                    }
                                    catch (Exception ex)
                                    {
                                        listaErros.Add("Erro ao obter o Segmento Comercial Pai: " + item.CodigoSegmentoComercial + " Nome: " + item.NomeSegmentoComercial);
                                        continue;
                                    }
                                }

                                segmentoComercial.Nome = item.NomeSegmentoComercial;
                                segmentoComercial.Ordem = item.Ordem;
                                segmentoComercial.TipoSegmento = item.TipoSegmentoComercial;

                                new RepositoryService(this.Organizacao, this.IsOffline).SegmentoComercial.Update(segmentoComercial);
                            }
                            catch (Exception ex)
                            {
                                listaErros.Add("Erro ao atualizar o Segmento Comercial Pai: " + item.CodigoSegmentoComercial + " Nome: " + item.NomeSegmentoComercial);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            SegmentoComercial obj = new SegmentoComercial();

                            if (!string.IsNullOrEmpty(item.CodigoSegmentoComercialPai))
                            {
                                try
                                {
                                    SegmentoComercial objSegmentoComercialPai = new Servicos.SegmentoComercialService(this.Organizacao, this.IsOffline).ObterPorCodigo(item.CodigoSegmentoComercialPai);
                                    obj.CodigoSegmentoPai = new SDKore.DomainModel.Lookup(objSegmentoComercialPai.Id, "itbc_segmentocomercial");
                                }
                                catch (Exception ex)
                                {
                                    listaErros.Add("Erro ao obter o Segmento Comercial Pai: " + item.CodigoSegmentoComercial + " Nome: " + item.NomeSegmentoComercial);
                                    continue;
                                }
                            }

                            obj.Nome = item.NomeSegmentoComercial;
                            obj.Ordem = item.Ordem;
                            obj.TipoSegmento = item.TipoSegmentoComercial;
                            obj.CodigoSegmento = Convert.ToInt32(item.CodigoSegmentoComercial);

                            new RepositoryService(this.Organizacao, this.IsOffline).SegmentoComercial.Create(obj);
                        }
                        catch (Exception ex)
                        {
                            listaErros.Add("Erro ao criar o Segmento Comercial Pai: " + item.CodigoSegmentoComercial + " Nome: " + item.NomeSegmentoComercial);
                            continue;
                        }
                    }
                }

                List<SegmentoComercial> lstSeguimentosComerciaisItens = new Servicos.SegmentoComercialService(this.Organizacao, this.IsOffline).ListarTodos();

                if (lstSeguimentosComerciaisItens != null && lstSeguimentosComerciaisItens.Count > 0)
                {
                    foreach (SegmentoComercial crmItem in lstSeguimentosComerciaisItens)
                    {
                        if (lstOrdenada.Exists(o => o.CodigoSegmentoComercial == crmItem.CodigoSegmento.ToString()) == false)
                        {
                            try
                            {
                                new SegmentoComercialService(this.Organizacao, this.IsOffline).Desativar((Guid)crmItem.ID);
                            }
                            catch (Exception ex)
                            {
                                listaErros.Add("Erro ao desativar o Segmento Comercial: " + crmItem.CodigoSegmento + " Nome: " + crmItem.Nome);
                                continue;
                            }
                        }
                    }
                }

                if (listaErros.Count() == 0)
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Erros apresentados nesta requisição: ");

                    foreach (var mensagemErro in listaErros)
                    {
                        sb.AppendLine(mensagemErro + ";");
                    }
                    resultadoPersistencia.Mensagem = sb.ToString();
                }

                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0306R1>(numeroMensagem, retorno);
            }
            catch (Exception e)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(e);
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0306R1>(numeroMensagem, retorno);
            }
        }
        #endregion

        #region Definir Propriedades
        public SegmentoComercial DefinirPropriedades(Intelbras.Message.Helper.MSG0306 xml)
        {
            var crm = new Model.SegmentoComercial(this.Organizacao, this.IsOffline);
            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(SegmentoComercial objModel)
        {
            return String.Empty;
        }
        #endregion
    }
}
