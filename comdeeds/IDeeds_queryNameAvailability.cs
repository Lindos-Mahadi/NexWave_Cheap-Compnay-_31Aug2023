using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace comdeeds
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDeeds_queryNameAvailability" in both code and config file together.
    [ServiceContract]
    public interface IDeeds_queryNameAvailability
    {
        [OperationContract]
        void DoWork();
    }
}
