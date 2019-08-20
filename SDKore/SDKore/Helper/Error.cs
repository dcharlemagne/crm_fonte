using System;
using System.Diagnostics;
using System.Text;
using System.Web.Services.Protocols;

namespace SDKore.Helper
{
    public class Error
    {
        private static readonly string _eventName = "CRM 2013";
        private static readonly int _eventId = 5554;
        private static readonly string _userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

        public static string GetMessageError(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            while (ex != null)
            {
                sb.AppendFormat("UserName  : {0}{1}", _userName, Environment.NewLine);
                sb.AppendFormat("Exception : {0}{1}", ex.GetType().FullName, Environment.NewLine);
                sb.AppendFormat("Message   : {0}{1}", ex.Message, Environment.NewLine);
                sb.AppendFormat("StackTrace: {0}{1}", ex.StackTrace, Environment.NewLine);
                sb.AppendFormat("HelpLink  : {0}{1}", ex.HelpLink, Environment.NewLine);
                sb.AppendFormat("Extension : {0}{1}", GetMessageExtension(ex), Environment.NewLine);
                sb.AppendFormat("-----------------------------------------------------------{0}", Environment.NewLine);
                ex = ex.InnerException;
            }

            return sb.ToString();
        }

        public static string Handler(Exception ex)
        {
            switch (ex.GetType().FullName)
            {
                case "System.ArgumentException":
                case "System.ArgumentNullException":
                case "System.NullReferenceException":
                case "System.ServiceModel.FaultException":
                case "System.ServiceModel.FaultException`1[[Microsoft.Xrm.Sdk.OrganizationServiceFault, Microsoft.Xrm.Sdk, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35]]":
                    return GetMessage(ex);

                case "System.ApplicationException":
                case "Microsoft.Xrm.Sdk.InvalidPluginExecutionException":
                    Create(GetMessageError(ex), EventLogEntryType.Warning);
                    return GetMessage(ex);

                default:
                    if (ex is System.ServiceModel.FaultException)
                    {
                        return GetMessage(ex);
                    }
                    else
                    {
                        Create(GetMessageError(ex), EventLogEntryType.Error);
                        return GetMessage(ex);
                    }
            }
        }

        public static bool Create(Exception ex, EventLogEntryType type)
        {
            string message = GetMessageError(ex);

            return Create(message, type);
        }

        public static bool Create(string message, EventLogEntryType type)
        {
            try
            {
                EventLog.WriteEntry(_eventName, (message.Length > 32000 ? message.Substring(0, 32000) : message), type, _eventId);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static string GetMessage(Exception ex)
        {
            switch (ex.GetType().FullName)
            {
                case "Microsoft.Xrm.Sdk.InvalidPluginExecutionException":
                case "System.ApplicationException":
                case "System.ArgumentException":
                case "System.ArgumentNullException":
                case "System.NullReferenceException":
                case "System.ServiceModel.FaultException":
                    return ex.Message;

                case "System.Web.Services.Protocols.SoapException":
                    return ((SoapException)ex).Detail.InnerText;

                default:
                    if (ex is System.ServiceModel.FaultException
                       || ex is System.ServiceModel.FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)
                    {
                        return ex.Message;
                    }
                    else
                    {
                        return "(CRM) Erro inesperado.";
                    }
            }
        }

        private static string GetMessageExtension(Exception ex)
        {
            string messageError = string.Empty;

            #region System.Net.WebException
            if (ex is System.Net.WebException)
            {
                var webExtension = ex as System.Net.WebException;

                if (webExtension.Response != null && webExtension.Response.ResponseUri != null)
                {
                    messageError += "AbsoluteUri: " + webExtension.Response.ResponseUri.AbsoluteUri;
                    messageError += Environment.NewLine + "UserInfo: " + webExtension.Response.ResponseUri.UserInfo;
                }

                return messageError;
            }
            #endregion

            #region System.Runtime.InteropServices.COMException
            if (ex is System.Runtime.InteropServices.COMException)
            {
                var comException = ex as System.Runtime.InteropServices.COMException;

                messageError += Environment.NewLine + "ErrorCode   :" + comException.ErrorCode;

                return messageError;
            }
            #endregion


            return messageError;
        }
    }
}
