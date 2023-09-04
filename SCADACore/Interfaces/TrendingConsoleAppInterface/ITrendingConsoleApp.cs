using SCADACore.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SCADACore.TrendingConsoleAppInterface
{
    [ServiceContract(CallbackContract = typeof(ITrendingConsoleAppCallback))]
    public interface ITrendingConsoleApp
    {
        [OperationContract(IsOneWay = true)]
        void TrendingConsoleAppInit();
    }

    public interface ITrendingConsoleAppCallback
    {
        [OperationContract(IsOneWay = true)]
        void TagValueChanged(double value, Tag tag);
    }
}
