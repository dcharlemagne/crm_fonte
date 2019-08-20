using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDKore.DomainModel
{
    public class DomainExecuteMultiple
    {
        public bool IsFaulted { get; set; }

        public DomainExecuteMultipleItem[] List { get; set; }
    }

    public class DomainExecuteMultipleItem
    {
        public Guid? ID { get; set; }

        public bool IsFaulted { get; set; }

        public string Message { get; set; }
    }
}