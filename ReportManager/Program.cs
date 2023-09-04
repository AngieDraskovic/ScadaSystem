using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportManager
{
    class Program
    {
        static ReportManagerService.ReportManagerClient rmc = new ReportManagerService.ReportManagerClient();
        static string _stars = new String('*', 25);

        static DateTime InputDate(string message)
        {
            while (true)
            {
                Console.Write($"Unesite {message} : (format MM/dd/YYYY HH:mm:ss) ");

                DateTime userDateTime;
                if (DateTime.TryParse(Console.ReadLine(), out userDateTime))
                {
                    return userDateTime;
                }
                else
                {
                    Console.WriteLine("Nije dobro unet datum!");
                }
            }
        }

        static bool InputSortDirection()
        {
            Console.Write("Unesite da li hoćete da sortirate opadajuće ili rastuće (asc ili desc) : ");
            string type = Console.ReadLine();

            return type == "asc";
        }

        static void PrintAlarms(ReportManagerService.AlarmTime[] alarms)
        {
            var table = new ConsoleTable("Vreme", "Vrednost", "Granica", "Tip", "Prioritet", "Ime taga");

            foreach (var alarmTime in alarms)
            {
                table.AddRow(alarmTime.TimeStamp, alarmTime.Value, alarmTime.Limit, alarmTime.AlarmType ,
                    alarmTime.Priority, alarmTime.TagName);
            }

            table.Write();
        }

        static void PrintTagValues(ReportManagerService.TagValue[] tagValues)
        {
            var table = new ConsoleTable("Vreme", "Ime taga", "Vrednost");

            foreach (var tagValue in tagValues)
            {
                table.AddRow(tagValue.TimeStamp, tagValue.TagName, tagValue.Value);
            }

            table.Write();
        }


        static void GetAllAlarmsForDateRange()
        {
            DateTime startDate;
            DateTime endDate;
            string sortType;
            bool asc;

            while (true)
            {
                startDate = InputDate("početni datum");
                endDate = InputDate("krajnji datum");

                if (endDate < startDate)
                {
                    Console.WriteLine("Krajnji datum je pre početnog!");
                    continue;
                }

                break;
            }

            while (true)
            {
                Console.Write("Unesite polje po kojem se sortira (prioritet, vreme) : ");
                sortType = Console.ReadLine();

                if (sortType != "prioritet" && sortType != "vreme")
                {
                    Console.WriteLine("Niste dobro uneli!");
                    continue;
                }

                break;
            }

            asc = InputSortDirection();

            ReportManagerService.AlarmTime[] alarms = rmc.GetAllAlarmsForTime(startDate, endDate, sortType, asc);
            if (alarms.Length == 0)
            {
                Console.WriteLine("Nema rezultata!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine();
            Console.WriteLine(_stars + "PREGLED ALARMA" + _stars);
            PrintAlarms(alarms);
            Console.ReadKey();
        }

        static void GetAllAlarmsForPriority()
        {
            int priority;
            bool asc;

            while (true)
            {
                Console.Write("Unesite prioritet (1, 2 ili 3) : ");

                try
                {
                    priority = Int32.Parse(Console.ReadLine());
                    if (priority < 1 || priority > 3)
                    {
                        Console.WriteLine("Nije dobar unos!");
                        continue;
                    }

                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("Nije dobar unos!");
                }
            }

            asc = InputSortDirection();

            ReportManagerService.AlarmTime[] alarms = rmc.GetAllAlarmsForPriority(priority, asc);
            if (alarms.Length == 0)
            {
                Console.WriteLine("Nema rezultata!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine();
            Console.WriteLine(_stars + "PREGLED ALARMA" + _stars);
            PrintAlarms(alarms);
            Console.ReadKey();
        }

        static void GetAllTagValuesForDateRange()
        {
            DateTime startDate;
            DateTime endDate;
            bool asc;

            while (true)
            {
                startDate = InputDate("početni datum");
                endDate = InputDate("krajnji datum");

                if (endDate < startDate)
                {
                    Console.WriteLine("Krajnji datum je pre početnog!");
                    continue;
                }

                break;
            }

            asc = InputSortDirection();

            ReportManagerService.TagValue[] tagValues = rmc.GetAllTagValuesForTime(startDate, endDate, asc);
            if (tagValues.Length == 0)
            {
                Console.WriteLine("Nema rezultata!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine();
            Console.WriteLine(_stars + "PREGLED VREDNOSTI TAGOVA" + _stars);
            PrintTagValues(tagValues);
            Console.ReadKey();
        }

        static void GetAllTagValuesForType(string type)
        {
            bool asc = InputSortDirection();
            ReportManagerService.TagValue[] tagValues = rmc.GetAllTagValuesForType(type, asc);

            if (tagValues.Length == 0)
            {
                Console.WriteLine("Nema rezultata!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine();
            Console.WriteLine(_stars + "PREGLED VREDNOSTI TAGOVA" + _stars);
            PrintTagValues(tagValues);
            Console.ReadKey();
        }

        static void GetAllTagValuesForName()
        {
            Console.Write("Unesite ime taga za kojeg hoćete pregled vrednosti : ");
            string tagName = Console.ReadLine();
            bool asc = InputSortDirection();
            ReportManagerService.TagValue[] tagValues = rmc.GetAllTagValuesForName(tagName, asc);

            if (tagValues.Length == 0)
            {
                Console.WriteLine("Nema rezultata!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine();
            Console.WriteLine(_stars + "PREGLED VREDNOSTI TAGOVA" + _stars);
            PrintTagValues(tagValues);
            Console.ReadKey();
        }


        static void ExecuteOption(string option)
        {
            Console.Clear();

            if (option == "1")
            {
                GetAllAlarmsForDateRange();
            }
            else if (option == "2")
            {
                GetAllAlarmsForPriority();
            }
            else if (option == "3")
            {
                GetAllTagValuesForDateRange();
            }
            else if (option == "4")
            {
                GetAllTagValuesForType("AnalogInputTag");
            }
            else if (option == "5")
            {
                GetAllTagValuesForType("DigitalInputTag");
            }
            else
            {
                GetAllTagValuesForName();
            }
        }

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(_stars + "IZVEŠTAJI" + _stars);
                Console.WriteLine("Dostupni izveštaji:");

                Console.WriteLine("1) Prikaz svih alarma koji su se desili u određenom vremenskom periodu");
                Console.WriteLine("2) Prikaz svih alarma određenog prioriteta");
                Console.WriteLine("3) Prikaz vrednosti svih tagova u određenom vremenskom periodu");
                Console.WriteLine("4) Prikaz vrednosti svih analognih ulaznih tagova");
                Console.WriteLine("5) Prikaz vrednosti svih digitalnih ulaznih tagova");
                Console.WriteLine("6) Prikaz vrednosti taga sa određenim nazivom");

                Console.Write("\r\nOdaberite opciju: ");
                String option = Console.ReadLine();
                ExecuteOption(option);

            }
        }
    }
}
