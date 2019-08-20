/// <reference path="d:\intelbras2\canais\branch\2015-07-23\adesaomonitoramento\customization\intelbras.crm2013.customization\util\util.js" />
/// <reference path="d:\intelbras2\canais\branch\2015-07-23\adesaomonitoramento\customization\intelbras.crm2013.customization\sdkorebase\sdkore.js" />

if (typeof (CategoriasdoCanal) == "undefined") { CategoriasdoCanal = {}; }

CategoriasdoCanal = {
    MatrizFilial: { Matriz: 993520000, Filial: 993520001 },
    ApuracaoDeBeneFiciosCompromissos: { Centralizada_na_Matriz: 993520000, Por_Filiais: 993520001 },
    FORMTYPE_CREATE: 1,
    FORMTYPE_UPDATE: 2,

    OnLoad: function () {
        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");
        this.EstabelecerFiltroDeCategoria();

        var formType = Xrm.Page.ui.getFormType();

        if (formType == this.FORMTYPE_CREATE) {
            CategoriasdoCanal.ValidaCanal();
        }

        if (formType == this.FORMTYPE_CREATE || formType == this.FORMTYPE_UPDATE) {
            Xrm.Page.getAttribute("itbc_canalid").addOnChange(CategoriasdoCanal.itbc_canalid_OnChange);
        }
    },

    //false= habilita para edição/true desabilita para edicao
    DisabledEnabled: function (valor) {
        Xrm.Page.getControl("itbc_name").setDisabled(valor);
    },

    ForceFieldSave: function () {
        Xrm.Page.getAttribute("itbc_name").setSubmitMode("always");
    },

    OnSave: function (context) {
        var eventArgs = context.getEventArgs();

        if (!CategoriasdoCanal.ValidaCanal()) {
            eventArgs.preventDefault();
            return;
        }

        CategoriasdoCanal.DisabledEnabled(false);

        var nome = Xrm.Page.getAttribute("itbc_name").getValue();
        if (nome == null || nome == "") {
            Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_canalid,itbc_categoria", " - "));
        }

        CategoriasdoCanal.DisabledEnabled(true);
        CategoriasdoCanal.ForceFieldSave();
    },

    EstabelecerFiltroDeCategoria: function () {
        var ViewColumn = CustomView.MakeStruct("SchemaName Width");
        var customViewId = "{" + CustomView.Guid() + "}";
        var customViewName = "Visualização de Categorias filtradas";
        var lookupFieldName = "itbc_categoria";
        var entityName = "itbc_categoria";
        var primaryKeyName = "itbc_categoriaid";
        var primaryFieldName = "itbc_name";
        var orderBy = "itbc_name";
        var viewColumns = [new ViewColumn("itbc_name", 250)];

        var meuFiltro = "";
        var classificacao = Xrm.Page.getAttribute("itbc_classificacaoid").getValue();

        if (classificacao != null) {
            if (classificacao[0].name.indexOf("Distribuidor") > -1) {
                meuFiltro += "<filter type='and'>";
                meuFiltro += "<condition attribute='itbc_name' operator='like' value='%Distribuidor%' />";
                meuFiltro += "</filter>";
            } else if (classificacao[0].name.indexOf("Revenda") > -1) {
                meuFiltro += "<filter type='and'>";
                meuFiltro += "<filter type='or'>";
                meuFiltro += "<condition attribute='itbc_name' operator='like' value='%Registrada%' />";
                meuFiltro += "<condition attribute='itbc_name' operator='like' value='%Bronze%' />";
                meuFiltro += "<condition attribute='itbc_name' operator='like' value='%Prata%' />";
                meuFiltro += "<condition attribute='itbc_name' operator='like' value='%Ouro%' />";
                meuFiltro += "</filter>";
                meuFiltro += "</filter>";
            }
        } else {
            meuFiltro += "<filter type='and'>";
            meuFiltro += "<condition attribute='itbc_name' operator='eq' value='00000000-0000-0000-0000-000000000000' />";
            meuFiltro += "</filter>";
        }

        CustomView.FilterGlobal(customViewId, customViewName, lookupFieldName, entityName, primaryKeyName, primaryFieldName, orderBy, null, viewColumns, meuFiltro, onload);
    },

    itbc_canalid_OnChange: function () {
        this.ValidaCanal();
    },

    /// Valida se Canal permite a inclusão de categorias do canal
    ValidaCanal: function () {
        var canal = Util.Xrm.ObterValor("itbc_canalid");
        if (canal == null)
            return true;

        SDKore.OData.setBaseUrl();
        SDKore.OData.configurarParametros("$select=itbc_MatrizouFilial,itbc_ApuracaoDeBeneficiosECompromissos");

        var conta = SDKore.OData.Retrieve(canal[0].id, "Account");

        if (conta.itbc_MatrizouFilial == null || conta.itbc_ApuracaoDeBeneficiosECompromissos == null)
            return true;

        if (conta.itbc_MatrizouFilial.Value == this.MatrizFilial.Filial && conta.itbc_ApuracaoDeBeneficiosECompromissos.Value == this.ApuracaoDeBeneFiciosCompromissos.Centralizada_na_Matriz) {
            alert("(CRM) Não é possivel criar/alterar categorias de canal para filiais com apuração de benefícios e compromissos centralizado na matriz.");
            return false;
        }

        return true;
    }

}