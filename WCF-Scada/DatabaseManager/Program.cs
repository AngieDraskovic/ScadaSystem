using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseManager.ServiceReference1;



namespace DatabaseManager
{
    internal class Program
    {
        private static string[] menuItems = { "Dodavanje taga", "Uklanjanje taga", "Registracija korisnika", "Upisivanje vrijednosti",
                                      "Prikaz trenutnih vrijednosti", "Podešavanje skeniranja ulaznih tagova", "Dodavanje alarma", "Log out" };

        private static readonly string INPUT_ERROR_MSG = "Unos nije validan, pokušajte ponovo.";
        static DBManagerServiceClient serviceClient = new DBManagerServiceClient();
        static bool loggedIn = false;
        static string token = null;
        static void Main(string[] args)
        {
            while (true)
            {
                if (!loggedIn)
                {
                    Console.WriteLine("1. Prijavi se");
                    Console.WriteLine("2. Registruj se");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            LogIn();
                            break;
                        case "2":
                            Register();
                            break;
                        default:
                            Console.WriteLine(INPUT_ERROR_MSG);
                            break;
                    }
                }
                else
                {
                    ShowMenu();
                }
            }
        }

        static void LogIn()
        {
            Console.Write("Korisničko ime >> ");
            string username = Console.ReadLine();
            Console.Write("Lozinka >> ");
            string password = Console.ReadLine();

            string result = serviceClient.LogIn(username, password);
            if (result != null)
            {
                token = result; 
                Console.WriteLine("Uspješna prijava!");
                loggedIn = true;
            }
            else
            {
                Console.WriteLine("Pogrešno korisničko ime ili lozinka.");
            }
        }

        static void Register()
        {
            Console.Write("Korisničko ime >> ");
            string username = Console.ReadLine();
            Console.Write("Lozinka >> ");
            string password = Console.ReadLine();

            bool result = serviceClient.RegisterUser(username, password);
            if (result)
            {
                Console.WriteLine("Uspješna registracija!");
            }
            else
            {
                Console.WriteLine("Registracija nije uspjela. Pokušajte ponovo.");
            }
        }

        static void ShowMenu()
        {
            while (loggedIn)
            {
                Console.WriteLine("\nIzaberite opciju:");
                for (int i = 0; i < menuItems.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {menuItems[i]}");
                }

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddTag();
                        break;
                    case "2":
                        RemoveTag(token);
                        break;
                    // ... Ostale opcije ...
                    case "8":
                        LogOut();
                        break;
                    default:
                        Console.WriteLine(INPUT_ERROR_MSG);
                        break;
                }
            }
        }

        static void AddTag() {
            string[] tagTypes = { "Analog input", "Analog output", "Digital input", "Digital output" };
            Console.WriteLine("Tip taga: ");
            for (int i = 0; i < tagTypes.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {tagTypes[i]}");
            }
            Console.WriteLine(">> ");
            string type = Console.ReadLine().Trim();
            switch (type)
            {
                case "1":
                    AddAITag(token);        // analog input
                    break;
                case "2":
                    AddAOTag(token);        // analog output
                    break;
                case "3":
                    AddDITag(token);        // digital input
                    break;
                case "4":
                    AddDOTag(token);        // digital output
                    break;
                default:
                    Console.WriteLine(INPUT_ERROR_MSG);
                    break;
           }
        
        }

        static void AddAITag(string token)
        {
            AnalogInputTag tag = new AnalogInputTag();
            EnterGeneralTagProperties(tag);
            EnterInputTagProperties(tag);
            tag.LowLimit = EnterLimitProperty("Low");
            tag.HighLimit = EnterLimitProperty("High");
            tag.Units = EnterUnitsProperty();
            bool success = serviceClient.AddTag(tag, token);
            Console.WriteLine((success) ? "Tag je uspjesno dodat" : "Greska pri dodavanju taga");
        }

        private static void AddDITag(string token)
        {
            DigitalInputTag tag = new DigitalInputTag();
            EnterGeneralTagProperties(tag);
            EnterInputTagProperties(tag);
            bool success = serviceClient.AddTag(tag, token);
            Console.WriteLine((success) ? "Tag je uspešno dodat" : "Greška pri dodavanju taga");
        }


        static void AddAOTag(string token)
        {
            AnalogOutputTag tag = new AnalogOutputTag();
            EnterGeneralTagProperties(tag);
            EnterOutputTagProperties(tag);
            tag.LowLimit = EnterLimitProperty("Low");
            tag.HighLimit = EnterLimitProperty("High");
            tag.Units = EnterUnitsProperty();
            bool success = serviceClient.AddTag(tag, token);
            Console.WriteLine((success) ? "Tag je uspješno dodat" : "Greška pri dodavanju taga");
        }


        static void AddDOTag(string token)
        {
            DigitalOutputTag tag = new DigitalOutputTag();
            EnterGeneralTagProperties(tag);
            EnterOutputTagProperties(tag);
            bool success = serviceClient.AddTag(tag, token);
            Console.WriteLine((success) ? "Tag je uspješno dodat" : "Greška pri dodavanju taga");
        }

        private static void EnterGeneralTagProperties(Tag tag)
        {
            Console.Write("Tag name >> ");
            tag.Id = Console.ReadLine();
            Console.Write("Description >> ");
            tag.Description = Console.ReadLine();
            Console.Write("I/O address >> ");
            tag.IOAddress = Console.ReadLine();
        }

        private static void EnterInputTagProperties(InputTag tag)
        {
            EnterDriverProperty(tag);
            EnterScanTimeProperty(tag);
            EnterOnScanProperty(tag);
        }

        private static void EnterOutputTagProperties(OutputTag tag)
        {
            while (true)
            {
                Console.Write("Initial value >> ");
                string input = Console.ReadLine();
                if (double.TryParse(input, out double initialValue))
                {
                    if (initialValue >= 0)
                    {
                        tag.InitialValue = initialValue;
                        return;
                    }
                }
                Console.WriteLine(INPUT_ERROR_MSG);
            }
        }


        private static void EnterDriverProperty(InputTag tag)
        {
            Console.WriteLine("Driver: ");
            Console.WriteLine("1. Simulation Driver");
            Console.WriteLine("2. Real-Time Driver");
            while (true)
            {
                Console.Write(">> ");
                string input = Console.ReadLine().Trim();
                if (input == "1")
                {
                    tag.DriverType = DriverType.Simulation;
                    break;
                }
                else if (input == "2")
                {
                    tag.DriverType = DriverType.RealTime;
                    break;
                }
                Console.WriteLine(INPUT_ERROR_MSG);
            }
        }


        private static void EnterScanTimeProperty(InputTag tag)
        {
            while (true)
            {
                Console.Write("Scan time >> ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int scanTime))
                {
                    if (scanTime > 0)
                    {
                        tag.ScanTime = scanTime;
                        return;
                    }
                }
                Console.WriteLine(INPUT_ERROR_MSG);
            }
        }

        private static void EnterOnScanProperty(InputTag tag)
        {
            Console.WriteLine("On/Off scan: ");
            Console.WriteLine("1. On scan");
            Console.WriteLine("2. Off scan");
            while (true)
            {
                Console.Write(">> ");
                string input = Console.ReadLine().Trim();
                if (input == "1")
                {
                    tag.OnScan = true;
                    break;
                }
                else if (input == "2")
                {
                    tag.OnScan = false;
                    break;
                }
                Console.WriteLine(INPUT_ERROR_MSG);
            }
        }

        private static double EnterLimitProperty(string limitType)
        {
            while (true)
            {
                Console.Write($"{limitType} limit >> ");
                string input = Console.ReadLine();
                if (double.TryParse(input, out double limit))
                {
                    if (limit >= 0)
                    {
                        return limit;
                    }
                }
                Console.WriteLine(INPUT_ERROR_MSG);
            }
        }

        private static string EnterUnitsProperty()
        {
            Console.Write("Units >> ");
            return Console.ReadLine();
        }

        private static void RemoveTag(string token)
        {
            Console.Write("Naziv taga za brisanje >> ");
            string tagName = Console.ReadLine();
            bool success = serviceClient.RemoveTag(tagName, token);
            Console.WriteLine((success) ? "Tag je uspješno obrisan" : "Greška pri brisanju taga");
        }



        static void LogOut()
        {
           
            serviceClient.LogOut(token);  
            Console.WriteLine("Odjavljeni ste.");
            loggedIn = false;
            token = null;
             
          
        }
    }

  }
