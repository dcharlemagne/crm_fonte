using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class EnderecoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public EnderecoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public EnderecoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public Endereco ObterPor(Guid enderecoId)
        {
            return RepositoryService.Endereco.Retrieve(enderecoId);
        }

        public Endereco BuscaEndereco(Guid itbc_enderecoid)
        {
            Endereco endereco = RepositoryService.Endereco.Retrieve(itbc_enderecoid);
            if (endereco != null)
                return endereco;
            return null;
        }

        public List<Endereco> ContaPossuiEnderecoAdicional(Conta conta)
        {
            return RepositoryService.Endereco.ObterTodosOsEnderecosPor(conta);
        }

        public void AtualizaEnderecosAdicionaisDaConta(Conta conta)
        {
            var enderecos = RepositoryService.Endereco.ObterTodosOsEnderecosPor(conta);
            foreach (var end in enderecos)
            {
                Endereco endereco = new Endereco(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                endereco.Id = end.Id;

                if (end.AddressNumber == 1 || end.Nome == "Padrão")
                {
                    endereco.Nome = "Padrão";
                    endereco.TipoEndereco = 3; // Primário
                    endereco.Logradouro = conta.Endereco1Rua;
                    endereco.Complemento = conta.Endereco1Complemento;
                    endereco.Cep = conta.Endereco1CEP;
                    endereco.Numero = conta.Endereco1Numero;
                    endereco.Bairro = conta.Endereco1Bairro;
                    endereco.Cidade = conta.Endereco1Cidade;
                    endereco.SiglaEstado = conta.Endereco1Estado;
                    endereco.Pais = conta.Endereco1Pais1;

                    RepositoryService.Endereco.Update(endereco);
                }

                if (end.AddressNumber == 2 || end.Nome == "Cobrança")
                {   
                    endereco.Nome = "Cobrança";
                    endereco.TipoEndereco = 1; // Fatura
                    endereco.Logradouro = conta.Endereco1Rua;
                    endereco.Complemento = conta.Endereco2Complemento;
                    endereco.Cep = conta.Endereco2CEP;
                    endereco.Numero = conta.Endereco2Numero;
                    endereco.Bairro = conta.Endereco2Bairro;
                    if(conta.Endereco2Municipioid != null)
                        endereco.Cidade = conta.Endereco2Municipioid.Name;
                    endereco.SiglaEstado = conta.Endereco2Estado;
                    endereco.Pais = conta.Endereco2Pais2;

                    RepositoryService.Endereco.Update(endereco);
                }

                
            }
        }

        public Endereco Persistir(Endereco endereco)
        {
            Endereco enderecoAtual = new Endereco(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
            enderecoAtual = RepositoryService.Endereco.ObterPor(endereco.Conta.Id, endereco.CodigoEndereco);

            if(enderecoAtual == null)
            {
                endereco.ID = RepositoryService.Endereco.Create(endereco);
                return endereco;
            }

            endereco.ID = enderecoAtual.ID;
            RepositoryService.Endereco.Update(endereco);

            return endereco;
        }

        public Endereco Remover(Endereco endereco)
        {
            Endereco enderecoAtual = new Endereco(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
            enderecoAtual = RepositoryService.Endereco.ObterPor(endereco.Conta.Id, endereco.CodigoEndereco);
            if(enderecoAtual == null)
            {
                throw new ArgumentException("Não foi possível remover o endereço informado.");
            }
            enderecoAtual.IntegrarNoPlugin = false;
            // Lógica para não renviar a mensagem para o Totvs novamente quando o endereço é removido por mensagem
            RepositoryService.Endereco.Update(enderecoAtual); 

            RepositoryService.Endereco.Delete(enderecoAtual.ID.Value);

            return endereco;
        }

        public string IntegracaoBarramento(Endereco endereco)
        {
            Domain.Integracao.MSG0191 msg = new Domain.Integracao.MSG0191(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            return msg.Enviar(endereco);
        }

        #endregion
    }
}