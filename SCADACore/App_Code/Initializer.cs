using SCADACore.Authentication;
using SCADACore.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace SCADACore.App_Code
{
    public class Initializer
    {
        public static void AppInitialize()
        {
            UserProcessing.LoadUsers();
            TagProcessing.LoadConfiguration();

            Thread t = new Thread(TagProcessing.PeriodicSaveConfiguration);
            t.Start();
        }
    }
}