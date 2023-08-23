using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WCF_Scada.models;

namespace WCF_Scada.Processing
{
    public class TagProcessing
    {

        static List<Tag> tags = new List<Tag>();
        static Dictionary<string, double> currentValues = new Dictionary<string, double>();

        static readonly object tagsLock = new object();
        public static bool AddTag(Tag tag) {
            return false;

        }
    }
}