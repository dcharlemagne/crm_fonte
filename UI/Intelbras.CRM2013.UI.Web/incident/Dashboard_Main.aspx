<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard_Main.aspx.cs" Inherits="Intelbras.CRM2013.UI.Web.Pages.incident.Dashboard_Main" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="margin: 0px; padding: 0px;" >
    <form id="form1" runat="server">
                <script language="javascript">
                    function resizeFrames() {
                        var x;
                        var y;
                        //--- all except Explorer ---
                        if (self.innerHeight) {
                            x = self.innerWidth;
                            y = self.innerHeight;
                        }

                        //--- Explorer 6 Strict Mode ---
                        else if (document.documentElement && document.documentElement.clientHeight) {
                            x = document.documentElement.clientWidth;
                            y = document.documentElement.clientHeight;
                        }

                        //--- other Explorers ---
                        else if (document.body) {
                            x = document.body.clientWidth;
                            y = document.body.clientHeight;
                        }

                        var obj = document.getElementById("principalIFrame");
                        obj.style.width = x;
                        obj.style.height = Math.max(0, y - obj.offsetTop + 1);
                    }
                </script>
                <IFRAME id="principalIFrame" style="LEFT: 0px; WIDTH: 100%; POSITION: absolute; TOP: 0px; HEIGHT: 100%" name="principal" src="DashBoard.aspx" frameBorder="0" width="100%" scrolling="auto"></IFRAME>
                <SCRIPT type="text/javascript">resizeFrames();</SCRIPT>





    </form>
</body>
</html>
