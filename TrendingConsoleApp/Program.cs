using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TrendingConsoleApp.TrendingConsoleAppService;

namespace TrendingConsoleApp
{
    public class Callback : ITrendingConsoleAppCallback
    {
        public void TagValueChanged(double value, Tag tag)
        {
            string toWrite = $"Vrednost : {value} , Ime taga : {tag.TagName} , Opis : {tag.Description}, I/O Adresa : {tag.IoAddress}";
            if (tag is AnalogInputTag)
            {
                AnalogInputTag ait = tag as AnalogInputTag;
                toWrite += $", Donja granica : {ait.LowLimit} , Gornja granica : {ait.HighLimit}";
            }

            Console.WriteLine(toWrite);
        }
    }

    class Program
    {
        static TrendingConsoleAppClient tcac;

        static void Main(string[] args)
        {
            InstanceContext ic = new InstanceContext(new Callback());
            tcac = new TrendingConsoleAppClient(ic);

            tcac.TrendingConsoleAppInit();

            Console.ReadKey();
        }
    }
}
