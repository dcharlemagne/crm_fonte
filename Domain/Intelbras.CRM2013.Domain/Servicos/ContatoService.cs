using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using SDKore.Helper;
using Intelbras.CRM2013.Domain.Exceptions;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ContatoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ContatoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public ContatoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public ContatoService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Métodos

        public Contato Persistir(Model.Contato Objcontato, string codigoRepresentante)
        {
            var contato = this.BuscaContatoPorCodigoRepresentante(codigoRepresentante);
            if(contato != null)
            {
                Objcontato.ID = contato.ID;
            }
            return this.Persistir(Objcontato);
        }

        public Contato Persistir(Contato Objcontato)
        {
            List<Contato> lstTempContato = new List<Contato>();
            Contato tmpContato = null;

            if (Objcontato.ID.HasValue)
            {
                tmpContato = RepositoryService.Contato.Retrieve(Objcontato.ID.Value);
                if (tmpContato == null)
                {
                    return tmpContato; // retorna NULL
                }

                // Atualiza o contato pelo GUID
                return this.AtualizaDadosContato(Objcontato, tmpContato);
            }
            
            // Verifica o Contato pelo campo CPF/CNPJ - itbc_cpfoucnpj
            if (!String.IsNullOrEmpty(Objcontato.CpfCnpj))
            {
                lstTempContato = RepositoryService.Contato.ListarPor(Objcontato.CpfCnpj);
                if (lstTempContato.Count == 1)
                {
                    return this.AtualizaDadosContato(Objcontato, lstTempContato.First<Contato>());
                }
                else if (lstTempContato.Count > 1)
                {
                    throw new ChaveIntegracaoContatoException("Foram encontrados contatos duplicados com o CPF/CNPJ informado.");
                }
            }

            // Cria o contato 
            Objcontato.ID = RepositoryService.Contato.Create(Objcontato);
            return Objcontato;
        }

        public void ColocarMascara()
        {
            List<Domain.Model.Contato> contatos = new List<Domain.Model.Contato>();
            int count;

            do
            {
                contatos = RepositoryService.Contato.ListarContatosSemMascara();
                count = 0;
                foreach (var item in contatos)
                {
                    if (item.CpfCnpj.Length >= 11)
                    {
                        item.CpfCnpj = item.CpfCnpj.InputMask();
                        if (item.CpfCnpj.Contains("."))
                        {
                            this.Persistir(item);
                            count++;
                        }
                    }
                }
            } while (contatos.Count >= 5000 || count > 0);
        }
        public void PersistirAreasAtuacao(Contato contato, List<AreaAtuacao> listaAreasAtuacao)
        {
            
            List<AreaAtuacao> listaAtual = RepositoryService.AreaAtuacao.ListarPorContato(contato.ID.Value);
            if (listaAtual != null && listaAtual.Count > 0)
            {
                RepositoryService.Contato.DesassociarAreasAtuacao(listaAtual, contato.ID.Value);
            }
            if (listaAreasAtuacao != null && listaAreasAtuacao.Count > 0)
            {
                RepositoryService.Contato.AssociarAreasAtuacao(listaAreasAtuacao, contato.ID.Value);
            }
        }
        public void PersistirMarcas(Contato contato, List<Marca> listaMarcas)
        {
            
            List<Marca> listaAtual = RepositoryService.Marca.ListarPorContato(contato.ID.Value);
            if (listaAtual != null && listaAtual.Count > 0)
            {
                RepositoryService.Contato.DesassociarMarcas(listaAtual, contato.ID.Value);
            }
            if (listaMarcas != null && listaMarcas.Count > 0)
            {
                RepositoryService.Contato.AssociarMarcas(listaMarcas, contato.ID.Value);
            }
        }

        private Contato AtualizaDadosContato(Contato contato, Contato tmpContato)
        {
            contato.ID = tmpContato.ID;
            RepositoryService.Contato.Update(contato);

            if (contato.Status != null && (!tmpContato.Status.Equals(contato.Status)))
                this.MudarStatus(tmpContato.ID.Value, contato.Status.Value);

            return contato;
        }

        public Contato BuscaContato(Guid contato)
        {
            return RepositoryService.Contato.Retrieve(contato);
        }

        public void AlteraTipoRelacao(Contato contato)
        {
            contato.TipoRelacao = (int)Enum.Contato.TipoRelacao.Outro;
            Persistir(contato);
        }

        public void EnviaContatoFielo(Contato contato, bool integraIntelbrasPontua)
        {
            contato.IntegraIntelbrasPontua = integraIntelbrasPontua;
            IntegracaoBarramento(contato);
        }

        public void ValidarDadosContato(Contato contato)
        {
            if (!string.IsNullOrEmpty(contato.CpfCnpj))
            {
                List<Contato> lista = RepositoryService.Contato.ListarPorEmail("", contato.CpfCnpj, true, new string[] { "contactid", "itbc_cpfoucnpj" });

                foreach (var item in lista)
                {
                    //Se o contato.ID não tiver valor veio de um create do plugin, se tiver valor verifica se o ID é o mesmo do update
                    if (!contato.ID.HasValue || (contato.ID.HasValue && item.ID.Value != contato.ID.Value))
                    {
                        if (item.CpfCnpj.GetOnlyNumbers() == contato.CpfCnpj.GetOnlyNumbers())
                        {
                            throw new ArgumentException(string.Format("(CRM) Duplicidade encontrada, verificar se existe outro contato com o mesmo CPF ou CNPJ [{0}].", item.CpfCnpj));
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(contato.CodigoRepresentante))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(contato.CodigoRepresentante, @"^\d+$"))
                {
                    throw new ArgumentException("(CRM) Código Representante deve ser composto apenas por números.");
                }
            }
        }

        public List<Contato> ListarTodosContatos()
        {
            return RepositoryService.Contato.ListarTodos();
        }

        public List<Contato> ListarAssociadosa()
        {
            return RepositoryService.Contato.ListarTodos();
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.Contato.AlterarStatus(id, status);
        }

        public Contato BuscaContatoPorCodigoRepresentante(string codigoRepresentante)
        {
            List<Contato> lstContato = RepositoryService.Contato.ListarPorCodigoRepresentante(codigoRepresentante);

            if (lstContato.Count > 0)
                return lstContato.First<Contato>();
            return null;
        }

        public List<Contato> BuscaContatoPorCodigoRepresentante(string[] codigosDeRepresentante)
        {
            return RepositoryService.Contato.ListarPorCodigoRepresentante(codigosDeRepresentante);
        }

        public string IntegracaoBarramento(Contato objContato)
        {
            Domain.Integracao.MSG0058 msgContato = new Domain.Integracao.MSG0058(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            return msgContato.Enviar(objContato);
        }

        public string IntegracaoFotoBarramento(Contato objContato)
        {
            Domain.Integracao.MSG0276 msgContato = new Domain.Integracao.MSG0276(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            return msgContato.Enviar(objContato);
        }

        public Contato BuscarPorIntegracaoCrm4(string guidCrm40)
        {
            return RepositoryService.Contato.ObterPorIntegracaoCrm4(guidCrm40);
        }
        
        public Contato BuscarContatoIntegracaoCrm4(string guidCrm40)
        {
           return RepositoryService.Contato.ObterPorIntegracaoCrm4(guidCrm40);
        }

        public bool ContatoPossuiTodosCamposParaIntegracao(Contato contato)
        {
            // Método criado para verificar se o contato possui todos os campos necessários para integração
            // necessários para o envio da MSG0058 - Contatos antigos do CRM4 não possuem todos
            // os dados, com isso não serão integrados no barramento
            if(contato.CpfCnpj == null || 
               contato.Email1 == null || 
               contato.Endereco1Municipioid == null ||
               contato.Endereco1Estadoid == null ||
               contato.Endereco1Pais == null)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
