if (typeof (Email) == "undefined") { Email = {}; }

Email = {

    OnLoad: function () {

        $('body').on('keydown', '#descriptionIFrame', function (e) {
            if ((this.scrollHeight - 5) > parseInt(this.style.height.replace("px", ""))) {
                this.style.height = (this.scrollHeight) + "px";
            }
        });

        //$('#descriptionIFrame').attr('scrolling', 'auto');

        //$('#descriptionIFrame').each(function () {
        //    $(this).height($(this).prop('scrollHeight'));
        //});

        crmForm.ObtemOcorrencia = function () {

            if (Xrm.Page.getAttribute("regardingobjectid").getValue() != null
                && Xrm.Page.getAttribute("regardingobjectid").getValue()[0].typename == "incident") {

                resultado = crmForm.ObterEmpresa(Xrm.Page.getAttribute("regardingobjectid").getValue()[0].id);
                if (resultado != null) {
                    var lookupData = new Array();
                    lookupData[0] = resultado;
                    Xrm.Page.getAttribute("to").setValue(lookupData);
                }
            }
            else {
                alert("Para localizar a empresa é preciso selecionar uma ocorrência.");
            }
        }

        crmForm.ObterEmpresa = function (ocorrenciaId) {
            Cols = ["new_tecnico_responsavelid"];
            var Retrieved = XrmServiceToolkit.Soap.Retrieve("incident", ocorrenciaId, Cols);

            var lookupItem = new Object();
            if (Retrieved.attributes["new_tecnico_responsavelid"] != null) {
                lookupItem.id = Retrieved.attributes["new_tecnico_responsavelid"].id;
                lookupItem.entityType = "contact";
                lookupItem.name = Retrieved.attributes["new_tecnico_responsavelid"].name;
            }
            return lookupItem;
        }

        if (Xrm.Page.ui.getFormType() == 1) {
            if (window.top.opener != undefined && window.top.opener != null && window.top.opener.Xrm.Page.data.entity != null && window.top.opener.Xrm.Page.data.entity.getEntityName() == 'incident') {
                crmForm.ObtemOcorrencia();
            }
        }
    },

    OnSave: function () {
        if (Xrm.Page.getAttribute("subject").getValue() != null) {
            Xrm.Page.getAttribute("subject").setValue(Xrm.Page.getAttribute("subject").getValue() + ".");
        }
    },

    regardingobjectid_onchange: function () {

    }
}
