using AlarmDisplayApp.AlarmDisplayService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AlarmDisplayApp
{
    public class Callback : IAlarmDisplayCallback
    {
        public void AlarmHappened(AlarmTime alarmTime)
        {
            string toWrite = $"Vreme : {alarmTime.TimeStamp} , Vrednost : {alarmTime.Value} , Limit : {alarmTime.Limit} , " +
                        $"Tip alarma : {alarmTime.AlarmType} , Prioritet : {alarmTime.Priority} , Ime taga : {alarmTime.TagName}";

            for (int i = 0; i < alarmTime.Priority; i++)
            {
                Console.WriteLine(toWrite);
            }
        }
    }

    class Program
    {
        static AlarmDisplayClient adc;

        static void Main(string[] args)
        {
            InstanceContext ic = new InstanceContext(new Callback());
            adc = new AlarmDisplayClient(ic);

            adc.AlarmDisplayInit();

            Console.ReadKey();
        }
    }
}
