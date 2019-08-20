if (typeof (PoliticaComercial) == "undefined") { PoliticaComercial = {}; }

PoliticaComercial.Formulario = {

    OnLoad: function () {
        debugger;
        this.ShowAndHiden();
        switch (Xrm.Page.ui.getFormType()) {
            case 0: //Undefined
                break;
            case 1: //Create
                break;
            case 2: //Update
                this.DisabledEnabled(true);
                break;
        }
    },

    Teste: function (politicaId, url) {
        var SiteURL = url + "?politicaComercialId=" + politicaId;
        PoliticaComercial.Formulario.LaunchFullScreen(SiteURL);
    },

    ShowAndHiden: function () {

        if (Xrm.Page.getAttribute("itbc_aplicarpoliticapara").getValue() == 993520000) {
            //Canais
            Xrm.Page.ui.tabs.get("tab_5").sections.get("tab_5_section_2").setVisible(false);
            //Estados
            Xrm.Page.ui.tabs.get("tab_5").sections.get("tab_5_section_3").setVisible(true);

//            document.getElementById('{34477c1b-26cc-4955-a4d2-d81c67b3b03c}').style.visibility = 'visible';
        }
        else if (Xrm.Page.getAttribute("itbc_aplicarpoliticapara").getValue() == 993520001) {
            //          document.getElementById('{34477c1b-26cc-4955-a4d2-d81c67b3b03c}').style.visibility = 'hidden';
            //Canais
            Xrm.Page.ui.tabs.get("tab_5").sections.get("tab_5_section_2").setVisible(true);
            //Estados
            Xrm.Page.ui.tabs.get("tab_5").sections.get("tab_5_section_3").setVisible(false);
        }
        else {
            //Canais
            Xrm.Page.ui.tabs.get("tab_5").sections.get("tab_5_section_2").setVisible(true);
            //Estados
            Xrm.Page.ui.tabs.get("tab_5").sections.get("tab_5_section_3").setVisible(true);
        }
    },

    ConcatenarNome: function () {
        if (Xrm.Page.getAttribute("itbc_estabelecimentoid") != null && Xrm.Page.getAttribute("itbc_estabelecimentoid").getValue() != null) {
            Estabelecimento = PoliticaComercial.Formulario.ObterEstabelecimento();
            Xrm.Page.getAttribute("itbc_name").setValue(Estabelecimento.itbc_codigo_estabelecimento + " - " + Util.funcao.ContatenarCampos("itbc_tipodepolitica,itbc_businessunitid,itbc_classificacaoid,itbc_categoriaid", " - "));
        }
    },

    //false= habilita para edição/true desabilita para edicao
    DisabledEnabled: function (valor) {
        Xrm.Page.getControl("itbc_estabelecimentoid").setDisabled(valor);
        Xrm.Page.getControl("itbc_tipodepolitica").setDisabled(valor);
        Xrm.Page.getControl("itbc_aplicarpoliticapara").setDisabled(valor);
        Xrm.Page.getControl("itbc_businessunitid").setDisabled(valor);
        Xrm.Page.getControl("itbc_classificacaoid").setDisabled(valor);
        Xrm.Page.getControl("itbc_categoriaid").setDisabled(valor);
    },

    ForceFieldSave: function () {
        Xrm.Page.getAttribute("itbc_name").setSubmitMode("always");
    },

    OnSave: function (context) {
        var eventArgs = context.getEventArgs();

        switch (Xrm.Page.ui.getFormType()) {
            case 0: //Undefined
                break;
            case 1: //Create
                PoliticaComercial.Formulario.ConcatenarNome();
                break;
            case 2: //Update
                break;
        }
        //PoliticaComercial.Formulario.DisabledEnabled(false);

        //PoliticaComercial.Formulario.DisabledEnabled(true);
        //PoliticaComercial.Formulario.ForceFieldSave();
    },

    LaunchFullScreen: function (url) {

        var x = screen.width / 2 - 600 / 2;
        var y = screen.height / 2 - 500 / 2;

        params = 'width=600';
        params += ', height=500';
        params += ', top=' + y;
        params += ', left=' + x;
        params += ', resizable=yes';
        params += ', scrollbars=yes';
        params += ', location=0';
        params += ', toolbar=0';
        params += ', modal=yes';
        params += ', alwaysRaised=yes';

        //params = 'width=' + screen.width;
        //params += ', height=' + screen.height;
        //params += ', top=0, left=0';
        //params += ', fullscreen=yes';
        //params += ', resizable=yes';
        //params += ', scrollbars=yes';
        //params += ', location=yes';

        newwin = window.open(url, "_blank", params);
        if (window.focus) {
            newwin.focus()
        }
        return false;
    },

    ObterEstabelecimento: function () {
        var estabelecimentoId = Xrm.Page.getAttribute('itbc_estabelecimentoid').getValue()[0].id;
        if (estabelecimentoId == null || estabelecimentoId == '' || estabelecimentoId == undefined) return null;

        var retornoSucesso;
        var retornoErro;

        var Estabelecimento = SDKore.OData.Retrieve(estabelecimentoId, "itbc_estabelecimento");

        if (Estabelecimento == null || Estabelecimento == undefined || Estabelecimento.length <= 0)
            return null;
        else
            return Estabelecimento;
    }
}