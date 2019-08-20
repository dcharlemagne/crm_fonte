function verifyAlertChange(execObj) {

    var formType = Xrm.Page.ui.getFormType();
    var HabChanged = Xrm.Page.getAttribute("itbc_desconto_verde_habilitado").getIsDirty();
    var percChanged = Xrm.Page.getAttribute("itbc_percentual_desconto_verde").getIsDirty();

    var message = 'Ao alterar a configuração de desconto verde esta informação será replicada para todas as famílias de produto deste segmento. Salvar o registro mesmo assim?';

    if ((formType == 2) && (HabChanged || percChanged)) {

        Xrm.Utility.confirmDialog(message,
			function () {
			},
			function () {
			    execObj.getEventArgs().preventDefault();
			}
		);
    }
}