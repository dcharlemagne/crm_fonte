using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Intelbras.CRM2013.Application.WCF
{
    [ServiceContract]
    public interface IServicoBarramento
    {
        [OperationContract]
        bool Postar(string mensagem, out string resposta);
        [OperationContract]
        bool Ping();

    }
}
