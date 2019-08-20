namespace Intelbras.CRM2013.Domain.Integracao
{
    public interface IBase<T, D>
        where D : Domain.Model.DomainBase
        where T : class
    {
        string Enviar(D objModel);
        string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario);
        // comentado o processar por causa do problema das listas
        //void Processar(D lista);
        D DefinirPropriedades(T legado);
    }
}