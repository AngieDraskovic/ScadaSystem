using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using WCF_Scada.Context;
using WCF_Scada.models;

namespace WCF_Scada.Processing
{
    public class TagProcessing
    {

        static List<Tag> tags = new List<Tag>(); 
        static Dictionary<string, double> currentValues = new Dictionary<string, double>();

        public static bool AddTag(Tag tag)
        {

            using (var db = new TagContext())
            {
                try
                {
                 
                    db.Tags.Add(tag);
                    db.SaveChanges();
                    return true;
                 
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception: " + ex.ToString());
                    return false;
                }
            }
        }


        public static bool RemoveTag(string Id)
        {
            using (var context = new TagContext())
            {
                var tag = context.Tags.FirstOrDefault(t => t.Id == Id);

                if (tag == null)
                {
                    return false; 
                }

                context.Tags.Remove(tag);
                context.SaveChanges();
                return true;
            }
        }

    }
}