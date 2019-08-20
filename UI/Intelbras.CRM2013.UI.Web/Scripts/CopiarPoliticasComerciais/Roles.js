$().ready(function () {
    $("#btnCopiarPoliticas").click(function () {
        $("#carregando").toggle("slow");
        parametros = {
            organizationName: $("#NomeOrganizacao").val(),
            copiarProdutos: $("#copiarProdutos").attr("checked"),
            copiarEstados: $("#copiarEstados").attr("checked"),
            dataInicialVigencia: $("#dataInicioVigencia").val(),
            dataFinalVigencia: $("#dataFinalVigencia").val(),
            idDosRegistros: window.opener.colecaoDeRegistros,
        };
        CallPageMethod("CopiarPoliticas", parametros, function (sucesso, res) {
            if (sucesso) {
                $("#carregando").toggle("slow");
                if (res == "null") {
                    alert("Erro ao copiar os registros favor contactar o administrador.");
                } else {
                    $.each(JSON.parse(res), function (name, value) {
                        if (name == "resultado" && value == true) {
                            $("#mensagemRetorno").show();
                        }
                        else {
                            if (name == "mensagem")
                                alert(value);
                        }
                    });
                }
            } else {
                ativarCampos(true);
                $("#carregando").toggle("slow");
                alert("Erro ao Gerar a planilha,tente novamente mais tarde");
            }

        });
    });
});