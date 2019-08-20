using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Intelbras.CRM2013.Application.WebServices.UsersService
{
    /// <summary>
    /// Summary description for UserService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class UserService : WebServiceBase
    {
        [WebMethod]
        public Usuario ObterUsuarioPor(string usuarioId)
        {
            Guid id;

            try { id = new Guid(usuarioId); }
            catch { throw new ArgumentException("O id do usuário está em branco ou é nulo", "usuarioId"); }
            
            return (new Domain.Servicos.RepositoryService()).Usuario.Retrieve(id);
        }

        [WebMethod]
        public Contato ObterContatoPor(string contatoId)
        {
            //try
            //{
            if (string.IsNullOrEmpty(contatoId))
                throw new Exception("O id do usuário está em branco ou é nulo!");
            
            return (new Domain.Servicos.RepositoryService()).Contato.Retrieve(new Guid(contatoId));
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        [WebMethod]
        public Contato ObterContatoPorLogin(string login)
        {
            Contato contato = null;

            try
            {
                contato = (new Domain.Servicos.RepositoryService()).Contato.ObterPor(login, Guid.Empty);
            }
            catch (Exception ex)
            {
                //LogService.GravaLog(ex, TipoDeLog.WSUsersService_UserService, "Contato ObterContatoPorLogin(string login, string organizacao)"); 
            }

            return contato;
        }

        [WebMethod]
        public Contato ObterContatoDaOSPor(string cpf, string nome)
        {
            //try
            //{
            if (string.IsNullOrEmpty(cpf))
                throw new Exception("O CPF do contato está em branco ou é nulo!");

            return (new Domain.Servicos.RepositoryService()).Contato.ObterPor(cpf);

            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        [WebMethod]
        public List<Contato> ListarContatosPor(string cpf, string nome)
        {
            var usuario = new Contato(nomeOrganizacao, false);
            usuario.Id = Guid.Empty;
            usuario.CpfCnpj = cpf;
            usuario.Nome = nome;
            return usuario.ListarContatosPor(usuario);
        }

        [WebMethod]
        public List<Contato> ListarContatosPelo(Domain.Model.Conta cliente)
        {
            return (new Domain.Servicos.RepositoryService()).Contato.ListarContatosPor(cliente);
        }

        [WebMethod]
        public List<Municipio> ListarCidadesPor(string regionalId)
        {
            try
            {
                if (string.IsNullOrEmpty(regionalId))
                    throw new Exception("O id da regional do usuário está em branco é ou nulo");

                var regional = new Regional(nomeOrganizacao, false) { Id = new Guid(regionalId) };
                return (new Domain.Servicos.RepositoryService()).Municipio.ListarCidadesPor(regional);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public List<Pais> ListarPaises()
        {
            return (new Domain.Servicos.RepositoryService()).Pais.ListarTodos();
        }

        [WebMethod]
        public List<Estado> ListarEstadosPor(string paisId)
        {
            return (new Domain.Servicos.RepositoryService()).Estado.ListarPor(new Pais(nomeOrganizacao, false) { Id = new Guid(paisId) });
            //return new List<Domain.Model.Localidades.UF>();
        }

        [WebMethod]
        public List<Municipio> ListarMunicipiosPor(string ufId)
        {
            return (new Domain.Servicos.RepositoryService()).Municipio.ListarPor(new Estado(nomeOrganizacao, false) { Id = new Guid(ufId) });
        }

        [WebMethod]
        public Regional PesquisarRegionalPor(string cidadeId)
        {
            Municipio cidade = new Municipio(nomeOrganizacao, false) { Id = new Guid(cidadeId) };
            return cidade.Regional;
        }

        [WebMethod]
        public string ValidaUsuarioNoCRMPor(string cpf, string email)
        {
            if (string.IsNullOrEmpty(cpf) && string.IsNullOrEmpty(email))
                throw new ArgumentException("Os paramentros cpf ou email devem ser preenchidos!");

            Contato contato = (new Domain.Servicos.RepositoryService()).Contato.ObterPor(cpf, email);

            if (contato == null)
                return string.Empty;

            if (!string.IsNullOrEmpty(email) && contato.Email1.Equals(email))
                return "Email já cadastrado!\\nEm caso de Dúvidas procure o instrutor técnico de sua região";

            if (!string.IsNullOrEmpty(cpf) && contato.CpfCnpj.Equals(cpf))
                return "CPF já cadastrado!\\nEm caso de Dúvidas procure o instrutor técnico de sua região";

            return string.Empty;
        }

        /// <summary>
        /// Usado para Criar ou Atualizar um contato no CRM.
        /// Usado pela pagina de cadastro da ITEC.
        /// </summary>
        /// <param name="contato"></param>
        /// <param name="organizacao"></param>
        /// <returns></returns>
        [WebMethod]
        public Guid SalvarContatoNoCRM(Contato contato)
        {
            if (contato.Id == Guid.Empty)
            {
                //contato.Origem = Domain.Model.Enum.OrigemDoContato.Portal;
                return (new Domain.Servicos.RepositoryService()).Contato.Create(contato);
            }
            else
            {
                (new Domain.Servicos.RepositoryService()).Contato.Update(contato);
                return contato.Id;
            }
        }

        [WebMethod]
        public bool AtivarDesativarUsuario(string nomeDeDominio, bool ativar)
        {
            bool retorno = false;
            Usuario usuario = (new Domain.Servicos.RepositoryService()).Usuario.ObterPor(nomeDeDominio);
            if (usuario != null)
            {
                (new Domain.Servicos.RepositoryService()).Usuario.AlterarStatus(usuario.Id, true);
                retorno = true;
            }

            return retorno;
        }
    }
}