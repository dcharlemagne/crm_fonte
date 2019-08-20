if (typeof (Checklist) == "undefined") { Checklist = {}; }

Checklist = {

    OnLoad: function () {

        switch (Xrm.Page.ui.getFormType()) {

            case 1: //Create
                Xrm.Page.getAttribute('itbc_compartilhada').setValue(null);
                break;
        }
    },
}