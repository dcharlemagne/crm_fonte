using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class UsuarioService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public UsuarioService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }


        public UsuarioService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        public Usuario BuscaUnidadePorNome(String usuario)
        {
            return RepositoryService.Usuario.ObterPor(usuario);
        }

        public Usuario BuscaPorCodigoAssistente(int itbc_codigodoassistcoml)
        {
            return RepositoryService.Usuario.ObterPorCodigoAssistente(itbc_codigodoassistcoml);
        }

        public Usuario BuscaPorCodigoSupervisorEMS(String codigoSupervisorEMS)
        {
            return RepositoryService.Usuario.ObterPorCodigoSupervisorEMS(codigoSupervisorEMS);
        }


        public Usuario ObterPor(Guid userId)
        {
            return RepositoryService.Usuario.ObterPor(userId);
        }

        public List<Usuario> ListarPor(Guid userId)
        {
            return RepositoryService.Usuario.ListarPor(userId);
        }

        public void DesativaUsuario()
        {
            DataTable dtUsuarios = RepositoryService.Usuario.ListarUsuarioSemAcessar40Dias();

            bool enviarEmail = false;
            String corpoEmail = "";

            foreach (DataRow item in dtUsuarios.Rows)
            {
                Usuario usuario = RepositoryService.Usuario.ObterPor(item.Field<Guid>("systemuserid"));

                if (usuario == null)
                {
                    continue;
                }
                else
                {
                    corpoEmail += "Usuário desabilitado: " + usuario.NomeUsuario + " - " + usuario.NomeCompleto + ". Data do último acesso: " + item.Field<DateTime>("lastaccesstime") + "\n";

                    RepositoryService.Usuario.AlterarStatus(usuario.Id, false);                    
                    enviarEmail = true;
                }
            }

            if (enviarEmail)
            {
                RepositoryService.Email.EnviaEmailComUsuariosDesativados(corpoEmail, "Usuários desabilitados - sem acessar a mais de 40 dias");
            }

        }

        public Boolean DesabilitaUsuarioDesligado(string matricula)
        {
            Usuario usuario = RepositoryService.Usuario.ObterUsuarioPorMatricula(matricula);
            if(usuario != null)
            {
                RepositoryService.Usuario.AlterarStatus(usuario.Id, false);
            }
            return true;
        }

        public Boolean DesabilitaUsuarioTrocaDepartamento(string matricula, string departamento)
        {
            Usuario usuario = RepositoryService.Usuario.ObterUsuarioPorMatricula(matricula);
            if (usuario != null && usuario.DescricaoDepartamento != departamento)
            {
                usuario.DescricaoDepartamento = departamento;
                RepositoryService.Usuario.Update(usuario);
                RepositoryService.Usuario.AlterarStatus(usuario.Id, false);
            }
            return true;
        }
        public Usuario BuscarProprietario(string entidadeRelacionada, string parametroIdEntidadeRelacionada, Guid idEntidadeRelacionada)
        {
            return RepositoryService.Usuario.BuscarProprietario(entidadeRelacionada, parametroIdEntidadeRelacionada, idEntidadeRelacionada);
        }
        //public Unidade Persistir(Unidade objUnidade)
        //{
        //    Unidade tmpUnidade = null;
        //    if (objUnidade.ID.HasValue)
        //    {
        //        tmpUnidade = RepositoryService.Unidade.ObterPor(objUnidade.ID.Value);

        //        if (tmpUnidade != null)
        //        {
        //            objUnidade.ID = tmpUnidade.ID;
        //            RepositoryService.Unidade.Update(objUnidade);

        //            return tmpUnidade;
        //        }
        //        else
        //            return null;
        //    }
        //    else
        //    {
        //        objUnidade.ID = RepositoryService.Unidade.Create(objUnidade);
        //        return objUnidade;
        //    }
        //}                
    }
}
