<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="Intelbras.CRM2013.UI.Web.Error" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Intelbras - Erro</title>
    <style>
        .col-lg-12 {
            position: relative;
            min-height: 1px;
            padding-left: 15px;
            padding-right: 15px;
            float: left;
            width: 100%;
        }

        .text-center {
            text-align: center;
        }

        body {
            font-family: "Helvetica Neue",Helvetica,Arial,sans-serif;
            font-size: 14px;
            line-height: 1.42857143;
            color: #333;
        }

        h1, .h1 {
            font-size: 36px;
            margin-top: 20px;
            margin-bottom: 10px;
            font-family: inherit;
            font-weight: 500;
            line-height: 1.1;
            color: inherit;
            margin: .67em 0;
        }

        h6, .h6 {
            font-size: 12px;
            margin-top: 10px;
            margin-bottom: 10px;
            font-family: inherit;
            font-weight: 500;
            line-height: 1.1;
            color: inherit;
        }
    </style>
</head>
<body>
    <h1 class="text-center">Intelbras</h1>
    <div class="col-lg-12">
        <%--<h6 class="text-center">Oh, não!</h6>--%>
        <p class="text-center"><%=Mensagem %></p>
    </div>
</body>
</html>
