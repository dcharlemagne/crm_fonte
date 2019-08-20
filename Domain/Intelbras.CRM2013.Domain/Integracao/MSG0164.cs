using Intelbras.CRM2013.Util.CustomException;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0164 : Base
    {
        #region Propriedades

        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = null;

        #endregion

        #region Construtor

        public MSG0164(string org, bool isOffline)
            : base(org, isOffline)
        {
            resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        }

        #endregion

        public Domain.ViewModels.SefazViewModel Enviar(Domain.ViewModels.SefazViewModel sefazViewModel)
        {
            var msg0164 = InstanciarValidarObjeto(sefazViewModel);

            string resposta;
            var integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);

            if (integracao.EnviarMensagemBarramento(msg0164.GenerateMessage(true), "1", "1", out resposta))
            {
                var msg0164r1 = CarregarMensagem<Pollux.MSG0164R1>(resposta);
                
                if (!msg0164r1.Resultado.Sucesso)
                {
                    throw new BarramentoException(msg0164r1.Resultado.Mensagem, msg0164r1.Resultado.CodigoErro);
                }

                return InstanciarValidarObjeto(msg0164r1);
            }
            else
            {
                var erro001 = CarregarMensagem<Pollux.ERR0001>(resposta);
                throw new ArgumentException("(CRM) " + erro001.GenerateMessage(false));
            }
        }

        private Pollux.MSG0164 InstanciarValidarObjeto(Domain.ViewModels.SefazViewModel sefazViewModel)
        {
            if (sefazViewModel == null)
            {
                throw new ArgumentNullException("Domain.ViewModels.SefazViewModel", "SefazViewModel não pode ser vazia.");
            }

            if (string.IsNullOrWhiteSpace(sefazViewModel.UF))
            {
                throw new ArgumentNullException("Domain.ViewModels.SefazViewModel.UF", "A UF não pode ser vazia!");
            }

            if (string.IsNullOrWhiteSpace(sefazViewModel.CPF) && string.IsNullOrWhiteSpace(sefazViewModel.CNPJ))
            {
                throw new ArgumentNullException("Domain.ViewModels.SefazViewModel.CPJ || CNPJ", "O campo CPF ou CNPJ é obrigatório!");
            }

            string numeroOperacao = (string.IsNullOrWhiteSpace(sefazViewModel.CNPJ)) ? sefazViewModel.CPF : sefazViewModel.CNPJ;

            var msg0164 = new Pollux.MSG0164(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), numeroOperacao)
            {
                InscricaoEstadual = sefazViewModel.InscricaoEstadual,
                CPF = sefazViewModel.CPF,
                CNPJ = sefazViewModel.CNPJ,
                UF = sefazViewModel.UF
            };

            return msg0164;
        }

        private Domain.ViewModels.SefazViewModel InstanciarValidarObjeto(Pollux.MSG0164R1 msg0164r1)
        {
            var sefazViewModel = new Domain.ViewModels.SefazViewModel()
            {
                CNPJ = msg0164r1.InformacaoFiscal.CNPJ,
                CPF = msg0164r1.InformacaoFiscal.CPF,
                InscricaoEstadual = msg0164r1.InformacaoFiscal.InscricaoEstadual,
                UF = msg0164r1.InformacaoFiscal.UF,
                CNAE = msg0164r1.InformacaoFiscal.CNAE,
                DataBaixa = msg0164r1.InformacaoFiscal.DataBaixa,
                DataInicioAtividade = msg0164r1.InformacaoFiscal.DataInicioAtividade,
                DataModificacaoSituacao = msg0164r1.InformacaoFiscal.DataModificacaoSituacao,
                InscricaoEstadualAtual = msg0164r1.InformacaoFiscal.InscricaoEstadualAtual,
                InscricaoEstadualUnica = msg0164r1.InformacaoFiscal.InscricaoEstadualUnica,
                Nome = msg0164r1.InformacaoFiscal.Nome,
                NomeFantasia = msg0164r1.InformacaoFiscal.NomeFantasia,
                ContribuinteIcms = msg0164r1.InformacaoFiscal.Situacao,
                RegimeApuracao = msg0164r1.InformacaoFiscal.RegimeApuracao,
                SituacaoCredenciamentoCTE = msg0164r1.InformacaoFiscal.SituacaoCredenciamentoCTE,
                SituacaoCredenciamentoNFE = msg0164r1.InformacaoFiscal.SituacaoCredenciamentoNFE
            };
            
            if (msg0164r1.InformacaoFiscal.EnderecoContribuinte != null)
            {
                sefazViewModel.EnderecoContribuinte = new ViewModels.SefazEnderecoContribuinteViewModel()
                {
                    Bairro = msg0164r1.InformacaoFiscal.EnderecoContribuinte.Bairro,
                    CEP = msg0164r1.InformacaoFiscal.EnderecoContribuinte.CEP,
                    CodigoIBGE = msg0164r1.InformacaoFiscal.EnderecoContribuinte.CodigoIBGE,
                    Complemento = msg0164r1.InformacaoFiscal.EnderecoContribuinte.Complemento,
                    Logradouro = msg0164r1.InformacaoFiscal.EnderecoContribuinte.Logradouro,
                    NomeCidade = msg0164r1.InformacaoFiscal.EnderecoContribuinte.NomeCidade,
                    Numero = msg0164r1.InformacaoFiscal.EnderecoContribuinte.Numero
                };
            }

            return sefazViewModel;
        }
    }
}