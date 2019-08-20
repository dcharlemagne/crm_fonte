if (typeof (AcessoExtranetContato) == "undefined") { AcessoExtranetContato = {}; }

AcessoExtranetContato = {

    OnLoad: function () {
        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");
    },

    //false= habilita para edição/true desabilita para edicao
    DisabledEnabled: function (valor) {
        Xrm.Page.getControl("itbc_name").setDisabled(valor);
    },

    ForceFieldSave: function () {
        Xrm.Page.getAttribute("itbc_name").setSubmitMode("always");
    },

    FiltrarAcessoExtranetPorTipo: function () {
        try {
            if (!Util.Xrm.HasValue("itbc_tipodeacesso")) return;

            var tipodeacessoId = Xrm.Page.getAttribute("itbc_tipodeacesso").getValue()[0].id;

            if (tipodeacessoId == "{" + Config.Entidade.TipoAcessoExtranet.Canal.id + "}") {
                Xrm.Page.getControl("itbc_acessoextranetid").addPreSearch(this.AdicionarFiltroAcessoExtranet);
            } else {
                Xrm.Page.getControl("itbc_acessoextranetid").removePreSearch(this.AdicionarFiltroAcessoExtranet);
            }
        } catch (e) {
            Xrm.Page.ui.setFormNotification('Não foi possível filtrar os acessos a extranet: ' + e, 'WARNING', 'FiltrarAcessoExtranetPorTipo');
        }
    },

    AdicionarFiltroAcessoExtranet: function () {
        try {
            if (!Util.Xrm.HasValue("itbc_tipodeacesso")) return;
            if (!Util.Xrm.HasValue("itbc_canal")) return;

            var canalId = Xrm.Page.getAttribute("itbc_canal").getValue()[0].id;
            var tipodeacessoId = Xrm.Page.getAttribute("itbc_tipodeacesso").getValue()[0].id;

            SDKore.OData.configurarParametros("$select=itbc_ClassificacaoId");
            var classificacao = SDKore.OData.Retrieve(canalId, "Account").itbc_ClassificacaoId;

            if (classificacao == null) {
                throw "Não foi encontrado classificação para a canal selecionado.";
            }

            SDKore.OData.configurarParametros("$select=itbc_categoria&$filter=statecode/Value%20eq%200%20and%20itbc_CanalId/Id%20eq%20(guid%27" + canalId + "%27)");
            var categorias = SDKore.OData.RetrieveMultiple("itbc_categoriasdocanal");

            /*if (categorias.length == 0) {
                throw "Não foi encontrado categoria para a canal selecionado.";
            }*/

            var fetch = "<filter type=\"and\">";
            fetch += "<condition attribute=\"statecode\" operator=\"eq\" value=\"0\" />";
            fetch += "<condition attribute=\"itbc_tipodeacesso\" operator=\"eq\" uitype=\"itbc_tipoacessoextranet\" value=\"" + tipodeacessoId + "\" />";
            fetch += "<condition attribute=\"itbc_classificacaoid\" operator=\"eq\" uitype=\"itbc_classificacao\" value=\"{" + classificacao.Id + "}\" />";
            if (categorias.length > 0) {
                fetch += "<condition attribute=\"itbc_categoriaid\" operator=\"in\">";

                for (i = 0; i < categorias.length; i++) {
                    fetch += "<value uiname=\"" + categorias[i].itbc_categoria.Name + "\" uitype=\"itbc_categoria\">{" + categorias[i].itbc_categoria.Id + "}</value>";
                }
                fetch += "</condition>";
            }

            fetch += "</filter>";

            Xrm.Page.getControl("itbc_acessoextranetid").addCustomFilter(fetch);
        } catch (e) {
            Xrm.Page.ui.setFormNotification('Não foi possível filtrar os acessos a extranet: ' + e, 'ERROR', 'FiltrarAcessoExtranetPorTipo');
            Xrm.Page.getControl("itbc_acessoextranetid").setDisabled(true);
            Xrm.Page.getAttribute("itbc_acessoextranetid").setValue(null);
        }
    },

    OnSave: function (context) {
        var eventArgs = context.getEventArgs();
        Xrm.Page.getAttribute("itbc_acaocrm").setValue(false);
        AcessoExtranetContato.DisabledEnabled(false);
        Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_contactid", " - Acesso à Extranet"));
        AcessoExtranetContato.DisabledEnabled(true);
        AcessoExtranetContato.ForceFieldSave();
    }
}