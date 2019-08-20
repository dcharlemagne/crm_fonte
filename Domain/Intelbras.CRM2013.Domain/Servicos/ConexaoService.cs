using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ConexaoService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ConexaoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public ConexaoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion


        #region Métodos

        public Conexao Persistir(Guid usuarioId, Model.Conexao conexao)
        {

            if (conexao.ID != null)
            {

                Conexao conexaoAtual = new Conexao(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);

                conexaoAtual = RepositoryService.Conexao.ObterPor((Guid)conexao.ID);

                if (conexaoAtual != null)
                {
                    conexao.ID = conexaoAtual.ID;

                    RepositoryService.Conexao.Update(conexao, usuarioId);

                    if (conexao.Status != null && !conexaoAtual.Status.Equals(conexao.Status))
                        MudarStatus(conexao.ID.Value, conexao.Status.Value, usuarioId);

                    return conexao;
                }
                else
                    return null;
            }
            else
            {
                conexao.ID = RepositoryService.Conexao.Create(conexao, usuarioId);
                return conexao;
            }
        }


        public void MudarStatus(Guid conexaoId, int state, Guid usuarioId)
        {
            int statuscode = 0;

            switch (state)
            {
                case (int)Enum.StateCode.Ativo:
                    statuscode = (int)Enum.Conexao.Status.Ativo;
                    break;

                case (int)Enum.StateCode.Inativo:
                    statuscode = (int)Enum.Conexao.Status.Inativo;
                    break;
            }

            RepositoryService.Conexao.AlterarStatus(conexaoId, state, statuscode, usuarioId);
        }

        public List<FuncaoConexao> ListarFuncaoConexao(int? categoria)
        {
            return RepositoryService.FuncaoConexao.ListarPor(categoria);
        }

        public List<FuncaoConexao> ListarFuncaoConexaoPorNome(string nomeFuncao)
        {
            return RepositoryService.FuncaoConexao.ListarPorNome(nomeFuncao);
        }

        #endregion

    }
}
