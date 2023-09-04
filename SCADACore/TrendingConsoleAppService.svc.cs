using SCADACore.Tags;
using SCADACore.TrendingConsoleAppInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SCADACore
{
    public class TrendingConsoleAppService : ITrendingConsoleApp
    {
        public void TrendingConsoleAppInit()
        {
            TagProcessing.TrendingConsoleAppInit();
        }
    }
}
