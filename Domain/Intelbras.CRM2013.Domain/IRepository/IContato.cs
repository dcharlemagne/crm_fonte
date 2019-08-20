using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using System.Xml;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IContato<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Conta conta);
        List<T> ListarPorEmail(String email, String cpfCnpj, bool apenasAtivos = true, params string[] columns);
        T ObterPor(string codigorepresentante);
        T ObterPorIntegracaoCrm4(string guiCrm40Contato);
        List<T> ListarPor(String cpfCnpj);
        List<T> ListarPorCodigoRepresentante(string codigoRepresentante);
        List<T> ListarPorCodigoRepresentante(string[] codigosDeRepresentante);
        bool AlterarStatus(Guid itbc_estadoid, int status);
        List<T> ListarKaRepresentantes(Guid? unidadenegocioId);
        List<T> ListarKaRepresentantes(Guid? unidadenegocioId, List<Guid> lstIdContatos);
        List<T> ListarKaRepresentantes(Guid? unidadenegocioId, List<Guid> lstIdContatos, int? pagina, int? contagem);
        List<T> ListarKaRepresentantesPotencial(Guid? unidadeNegocioId, int ano, params string[] columns);
        List<T> ListarTodos();
        List<T> ListarSemAcessoKonviva();
        List<T> ListarContatosSemMascara();

        //CRM4
        T ObterPor(string login, Guid contatoId);
        T ObterPor(string cpf, string email);
        List<T> ListarPorEmail(string email);
        //T Create(XmlDocument xml);
        List<T> ListarContatosPor(Contato contato);
        List<T> ListarContatosPor(Model.Conta cliente);
        List<T> ListarPor(int diaAniversario, int mesAniversario);
        List<T> ListarContatosComCep(DateTime ultimaDataModificacao);
        List<T> ListarContatosComCep();
        List<T> ListarComNFE();
        List<KeyValuePair<Guid, string>> ListarVendedorFidelidade(Guid clienteId);
        void UpdateEmailFBA(Contato contato);
        void EnviaEmailAcessoPortalCorporativo(Contato contato);
        void ExecutaWorkFlow(Contato contato, Guid WorkFlowId);
        string ObterSenha(Guid contatoid);
        int TipoAcessoPortal(string login);
        T ObterRepresentatePor(Domain.Model.Conta cliente);
        T ObterPorDuplicidade(string cpf, string login);
        List<T> ObterVendedores(Guid distribuidorId);
        List<T> ObterTodosComExtratoFidelidade(int quantidade, int pagina, string cookie);
        //CRM4
        List<T> ListarAssociadosA(string codigoConta);
        void AssociarAreasAtuacao(List<AreaAtuacao> areasAtuacao, Guid contatoid);
        void DesassociarAreasAtuacao(List<AreaAtuacao> areasAtuacao, Guid contatoid);
        void AssociarMarcas(List<Marca> marcas, Guid contatoid);
        void DesassociarMarcas(List<Marca> marcas, Guid contatoid);

    }
}
