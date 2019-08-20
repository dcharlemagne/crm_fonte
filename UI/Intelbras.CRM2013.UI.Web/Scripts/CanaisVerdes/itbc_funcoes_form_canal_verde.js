function CheckName() {
    var UtilName = Xrm.Page.getAttribute("itbc_name").getValue();
    if (UtilName == null) {
        Xrm.Page.getAttribute("itbc_name").setValue("-");
    }
}

function SetName() {
    var newName;

    var lookupFam = Xrm.Page.getAttribute("itbc_familia_produto_id");
    var nameFam;
    var lookupSeg = Xrm.Page.getAttribute("itbc_segmento");
    var nameSeg;
    var lookupCanal = Xrm.Page.getAttribute("itbc_canalid");
    var nameCanal;

    if (lookupFam != null) {
        var lookupFamValue = lookupFam.getValue();
        if ((lookupFamValue != null)) {
            nameFam = lookupFamValue[0].name;
        }
    }

    if (lookupSeg != null) {
        var lookupSegValue = lookupSeg.getValue();
        if ((lookupSegValue != null)) {
            nameSeg = lookupSegValue[0].name;
        }
    }

    if (lookupCanal != null) {
        var lookupCanalValue = lookupCanal.getValue();
        if ((lookupCanalValue != null)) {
            nameCanal = lookupCanalValue[0].name;
        }
    }

    newName = nameFam
    newName += " | ";
    newName += nameSeg;
    newName += " | ";
    newName += nameCanal;

    Xrm.Page.getAttribute("itbc_name").setValue(newName);
}