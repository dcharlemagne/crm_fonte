if (typeof (DocumentoParaCanaisExtranet) == "undefined") { DocumentoParaCanaisExtranet = {}; }

DocumentoParaCanaisExtranet = {

    RazaoDoStatus: {
        Rascunho: 993520000,
        PendenteAprovacao: 993520001,
        Aprovado: 993520002
    },

    subGridsCarregadas: false,

    OnLoad: function () {
        DocumentoParaCanaisExtranet.OnChangedTodosCanais();
    },

    OnSave: function () {
        DocumentoParaCanaisExtranet.ValidarRegistroClassificacoesCategorias();
    },

    OnChangedTodosCanais: function () {
        if (DocumentoParaCanaisExtranet.subGridsCarregadas) {
            var todosCanais = Xrm.Page.getAttribute("itbc_todoscanais").getValue();

            if (todosCanais) {
                Xrm.Page.getControl("itbc_subgrid_classificacao").setVisible(false);
                Xrm.Page.getControl("itbc_subgrid_categoria").setVisible(false);
            }
            else {
                Xrm.Page.getControl("itbc_subgrid_classificacao").setVisible(true);
                Xrm.Page.getControl("itbc_subgrid_categoria").setVisible(true);
            }
        }
        else {
            var subGrid = Xrm.Page.getControl("itbc_subgrid_categoria");

            if (subGrid != null) {
                DocumentoParaCanaisExtranet.subGridsCarregadas = true;
                DocumentoParaCanaisExtranet.OnChangedTodosCanais();
            }
            else
                setTimeout(DocumentoParaCanaisExtranet.OnChangedTodosCanais, 1500);
        }
    },

    ValidarRegistroClassificacoesCategorias: function () {
        var todosCanais = Xrm.Page.getAttribute("itbc_todoscanais").getValue();

        if (!todosCanais && Xrm.Page.ui.getFormType() == 2) {
            var linhasClassificacao = DocumentoParaCanaisExtranet.CapturarLinhasSubGrid("itbc_subgrid_classificacao");
            var linhasCategoria = DocumentoParaCanaisExtranet.CapturarLinhasSubGrid("itbc_subgrid_categoria");

            if (linhasCategoria.length > 0 && linhasClassificacao.length > 0) {
                //OK;
            }
            else {
                var statusAttribute = Xrm.Page.getAttribute("statuscode");

                Xrm.Page.ui.clearFormNotification('1');
                Xrm.Page.ui.setFormNotification('Atenção! Os documentos presentes neste registro necessitam de Classificação e Categoria para serem listados.', 'WARNING', '1');

                if (statusAttribute.getValue() == DocumentoParaCanaisExtranet.RazaoDoStatus.Aprovado)
                    statusAttribute.setValue(DocumentoParaCanaisExtranet.RazaoDoStatus.PendenteAprovacao);
            }
        }
    },

    //TO DO :> adicionar ao Util quando possível
    //Esse método pode ser melhorado quando a versão do CRM sofrer Update para o 2015
    //Varificar a seguinte referência: https://msdn.microsoft.com/en-us/library/dn932126.aspx
    CapturarLinhasSubGrid: function (subGridName) {
        var gridControl = document.getElementById(subGridName).control;
        return gridControl.get_allRecordIds();
    }
}