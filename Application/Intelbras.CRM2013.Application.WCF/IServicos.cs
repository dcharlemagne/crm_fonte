using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Intelbras.CRM2013.Application.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServicos" in both code and config file together.
    [ServiceContract]
    public interface IServicos
    {
        //[OperationContract]
        //string DoWork();

        //[OperationContract]
        //string Pollux();


        [OperationContract]
        bool Ping();

        [OperationContract]
        bool Postar(string requisicao, out string resposta);
    }
}
