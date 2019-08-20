if (typeof (PoliticaComercial) == "undefined") { PoliticaComercial = {}; }

PoliticaComercial.Formulario = {

    OnSave: function (context) {
        var eventArgs = context.getEventArgs();

        var datainicial = Xrm.Page.getAttribute("itbc_datainicial").getValue().getDate() + "/" + Xrm.Page.getAttribute("itbc_datainicial").getValue().getMonth() + "/" + Xrm.Page.getAttribute("itbc_datainicial").getValue().getFullYear()

        var datafinal = Xrm.Page.getAttribute("itbc_datafinal").getValue().getDate() + "/" + Xrm.Page.getAttribute("itbc_datafinal").getValue().getMonth() + "/" + Xrm.Page.getAttribute("itbc_datafinal").getValue().getFullYear()
        
        var nome = "De " + Xrm.Page.getAttribute("itbc_familiainicialid").getValue()[0].name + " até " + Xrm.Page.getAttribute("itbc_familiafinalid").getValue()[0].name + " Validade " + datainicial + " até " + datafinal;

        nome = nome.substr(0, 99);

        Xrm.Page.getAttribute("itbc_name").setValue(nome);
    }
}