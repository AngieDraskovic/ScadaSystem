using ConsoleTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager
{
    public enum UserType
    {
        ADMIN,
        USER,
        NOT_LOGGED
    }

    class Program
    {
        static DatabaseManagerService.AuthenticationClient ac = new DatabaseManagerService.AuthenticationClient();
        static DatabaseManagerService.TagWorkClient twc = new DatabaseManagerService.TagWorkClient();
        static Menu menu = new Menu();
        static UserType userType = UserType.NOT_LOGGED;
        static string token = "";
        static bool exit = false;

        static bool IsExit(string field)
        {
            if (field.ToLower() == "x")
            {
                return true;
            }

            return false;
        }

        static void LogIn()
        {
            String username;
            String password;
            Validator.ValidationType = ValidationType.NOT_EMPTY;

            username = InputField("korisničko ime");
            if (IsExit(username))
            {
                return;
            }

            password = InputField("lozinku");
            if (IsExit(password))
            {
                return;
            }

            string newToken = ac.Login(username, password);
            if (newToken == "")
            {
                Console.WriteLine("\nNeuspešna prijava!");
            }
            else
            {
                bool isAdmin = ac.IsUserAdmin(newToken);
                userType = UserType.USER;
                if (isAdmin)
                {
                    userType = UserType.ADMIN;
                }
                token = newToken;
                Console.WriteLine("\nUspešna prijava!");
            }

            AddOptionsBasedOnUserType();
            ContinueConsole();
        }

        static void Logout()
        {
            ac.Logout(token);
            userType = UserType.NOT_LOGGED;
            token = "";
            Console.WriteLine("\nUspešna odjava sa sistema!");

            AddOptionsBasedOnUserType();
            ContinueConsole();
        }

        static void RegisterUser()
        {
            string username;
            string password;
            Validator.ValidationType = ValidationType.NOT_EMPTY;

            username = InputField("korisničko ime");
            if (IsExit(username))
            {
                return;
            }

            password = InputField("lozinku");
            if (IsExit(password))
            {
                return;
            }

            if (!ac.RegisterUser(username, password, token))
            {
                Console.WriteLine("\nNeuspešna registracija, korisničko ime već postoji!");
            }
            else
            {
                Console.WriteLine("\nUspešna registracija!");
            }
            ContinueConsole();
            AddOptionsBasedOnUserType();
        }

        static bool InputAlarms(string tagName, List<DatabaseManagerService.Alarm> alarms)
        {
            Validator.ValidationType = ValidationType.RANGE_NUMBER;
            Validator.lowLimit = "0";

            while (true)
            {
                DatabaseManagerService.Alarm alarm = InputAlarm(tagName);
                if (alarm == null)
                {
                    return false;
                }
                alarms.Add(alarm);

                Console.Write("Unesite Da ako želite da dodate još alarma : ");
                string choice = Console.ReadLine();
                if (choice.ToLower() != "da")
                {
                    break;
                }
            }

            for (int i = 0; i < alarms.Count; i++)
            {
                alarms[i].AlarmId = i;
            }

            return true;
        }

        static DatabaseManagerService.Alarm InputAlarm(string tagName)
        {
            Validator.highLimit = "1";

            string alarmType = InputField("tip alarma (0 - LOW, 1 - HIGH)");
            if (IsExit(alarmType))
            {
                return null;
            }

            Validator.highLimit = "3";

            string alarmPriority = InputField("prioritet alarma (1, 2 ili 3)");
            if (IsExit(alarmPriority))
            {
                return null;
            }

            return new DatabaseManagerService.Alarm
            {
                AlarmType = alarmType == "0" ? DatabaseManagerService.AlarmType.LOW : DatabaseManagerService.AlarmType.HIGH,
                Priority = Int32.Parse(alarmPriority),
                TagName = tagName
            };
        }

        static void InputAnalogInputTag()
        {
            DatabaseManagerService.InputTag inTag = InputInputTag();
            if (inTag == null)
            {
                return;
            }

            Validator.ValidationType = ValidationType.ANALOG_NUMBER;
            Validator.lowLimit = "";
            Validator.highLimit = "";

            string lowLimit = InputField("donju granicu za vrednost");
            if (IsExit(lowLimit))
            {
                return;
            }

            Validator.lowLimit = lowLimit;

            string highLimit = InputField("gornju granicu za vrednost");
            if (IsExit(highLimit))
            {
                return;
            }

            Validator.ValidationType = ValidationType.NOT_EMPTY;

            string unit = InputField("merna jedinica");
            if (IsExit(unit))
            {
                return;
            }

            List<DatabaseManagerService.Alarm> alarms = new List<DatabaseManagerService.Alarm>();
            Console.Write("Da li želite da unesete alarme? (da) : ");
            string choice = Console.ReadLine();
            if (choice.ToLower() == "da")
            {
                bool ok = InputAlarms(inTag.TagName, alarms);
                if (!ok)
                {
                    return;
                }
            }

            string message = twc.AddTag(new DatabaseManagerService.AnalogInputTag
            {
                TagName = inTag.TagName,
                Description = inTag.Description,
                Driver = inTag.Driver,
                IoAddress = inTag.IoAddress,
                ScanTime = inTag.ScanTime,
                ScanOn = inTag.ScanOn,
                LowLimit = double.Parse(lowLimit),
                HighLimit = double.Parse(highLimit),
                Alarms = alarms.ToArray(),
                Unit = unit
            }, token);

            if (message == "")
            {
                Console.WriteLine("\nUspešno dodavanje analognog ulaznog taga!");
            }
            else
            {
                Console.WriteLine("\nNeuspešno dodavanje, " + message);
            }

            ContinueConsole();
        }

        static void InputDigitalInputTag()
        {
            DatabaseManagerService.InputTag inTag = InputInputTag();

            if (inTag == null)
            {
                return;
            }

            string message = twc.AddTag(new DatabaseManagerService.DigitalInputTag
            {
                TagName = inTag.TagName,
                Description = inTag.Description,
                Driver = inTag.Driver,
                IoAddress = inTag.IoAddress,
                ScanTime = inTag.ScanTime,
                ScanOn = inTag.ScanOn
            }, token);

            if (message == "")
            {
                Console.WriteLine("\nUspešno dodavanje digitalnog ulaznog taga!");
            }
            else
            {
                Console.WriteLine("\nNeuspešno dodavanje, " + message);
            }

            ContinueConsole();
        }

        static DatabaseManagerService.InputTag InputInputTag()
        {
            Validator.ValidationType = ValidationType.NOT_EMPTY;

            string tagName = InputField("ime ulaza");
            if (IsExit(tagName))
            {
                return null;
            }

            string description = InputField("kratak opis");
            if (IsExit(description))
            {
                return null;
            }

            Validator.ValidationType = ValidationType.RANGE_NUMBER;
            Validator.lowLimit = "0";
            Validator.highLimit ="1";

            string driver = InputField("koji hoćete drajver (0 - SimulationDriver, 1 - RealTimeDriver)");
            if (IsExit(driver))
            {
                return null;
            }

            string ioAddress;

            if (driver == "0")
            {
                Validator.ValidationType = ValidationType.SIMULATION_ADDRESS;
                ioAddress = InputField("adresu na koju će se vezati vrednost ulaza (S - sinus,  C - kosinus, R - rampa)");
            }
            else
            {
                Validator.ValidationType = ValidationType.NOT_EMPTY;
                ioAddress = InputField("adresu real time drivera na koju će se vezati vrednost ulaza");
            }
            if (IsExit(ioAddress))
            {
                return null;
            }

            Validator.ValidationType = ValidationType.RANGE_NUMBER;
            Validator.lowLimit = "0";
            Validator.highLimit = "";

            string scanTime = InputField("broj sekundi na koje će se vrednost izlaza osvežavati");
            if (IsExit(scanTime))
            {
                return null;
            }

            Validator.lowLimit = "0";
            Validator.highLimit = "1";

            string scanOn = InputField("da li hoćete da se osvežavanje vrednosti prikazuje na trending aplikaciji (0 - ne, 1 - da)");
            if (IsExit(scanOn))
            {
                return null;
            }

            return new DatabaseManagerService.InputTag
            {
                TagName = tagName,
                Description = description,
                Driver = driver == "0"? DatabaseManagerService.DriverType.SIMULATION_DRIVER : DatabaseManagerService.DriverType.REAL_TIME_DRIVER,
                IoAddress = ioAddress,
                ScanTime = Int32.Parse(scanTime),
                ScanOn = scanOn != "0",
            };
        }

        static DatabaseManagerService.Tag InputOutputTag()
        {
            string tagName;
            string description;
            string ioAddress;
            Validator.ValidationType = ValidationType.NOT_EMPTY;

            tagName = InputField("ime izlaza");
            if (IsExit(tagName))
            {
                return null;
            }

            description = InputField("kratak opis");
            if (IsExit(description))
            {
                return null;
            }

            ioAddress = InputField("adresu na koju hoćete da upišete vrednost izlaza");
            if (IsExit(ioAddress))
            {
                return null;
            }

            return new DatabaseManagerService.Tag
            {
                TagName = tagName,
                Description = description,
                IoAddress = ioAddress
            };
        }

        static void InputDigitalOutputTag()
        {
            DatabaseManagerService.Tag outTag = InputOutputTag();
            if (outTag == null)
            {
                return;
            }

            string initialValue;
            Validator.ValidationType = ValidationType.DIGITAL_NUMBER;

            initialValue = InputField("inicijalnu vrednost izlaza(0 ili 1)");
            if (IsExit(initialValue))
            {
                return;
            }

            string message = twc.AddTag(new DatabaseManagerService.DigitalOutputTag
            {
                TagName = outTag.TagName,
                Description = outTag.Description,
                IoAddress = outTag.IoAddress,
                InitialValue = double.Parse(initialValue)
            }, token);

            if (message == "")
            {
                Console.WriteLine("\nUspešno dodavanje digitalnog izlaznog taga!");
            }
            else
            {
                Console.WriteLine("\nNeuspešno dodavanje, " + message);
            }

            ContinueConsole();
        }

        static void InputAnalogOutputTag()
        {
            DatabaseManagerService.Tag outTag = InputOutputTag();
            if (outTag == null)
            {
                return;
            }

            string lowLimit;
            string highLimit;
            string initialValue;
            Validator.ValidationType = ValidationType.ANALOG_NUMBER;

            lowLimit = InputField("donju granicu za vrednost");
            if (IsExit(lowLimit))
            {
                return;
            }

            Validator.lowLimit = lowLimit;

            highLimit = InputField("gornju granicu za vrednost");
            if (IsExit(highLimit))
            {
                return;
            }

            Validator.ValidationType = ValidationType.NOT_EMPTY;

            string unit = InputField("merna jedinica");
            if (IsExit(unit))
            {
                return;
            }

            Validator.highLimit = highLimit;

            initialValue = InputField("inicijalnu vrednost izlaza");
            if (IsExit(initialValue))
            {
                return;
            }

            string message = twc.AddTag(new DatabaseManagerService.AnalogOutputTag
            {
                TagName = outTag.TagName,
                Description = outTag.Description,
                IoAddress = outTag.IoAddress,
                InitialValue = double.Parse(initialValue),
                LowLimit = double.Parse(lowLimit),
                HighLimit = double.Parse(highLimit),
                Unit = unit
            }, token);

            if (message == "")
            {
                Console.WriteLine("\nUspešno dodavanje analognog izlaznog taga!");
            }
            else
            {
                Console.WriteLine("\nNeuspešno dodavanje, " + message);
            }

            ContinueConsole();
        }

        static string InputField(string fieldName)
        {
            while (true)
            {
                Console.Write("Unesite " + fieldName + " (X za izlaz) : ");
                string field = Console.ReadLine();

                if (field.ToLower() == "x")
                {
                    return field;
                }

                if (Validator.ValidateField(field))
                {
                    return field;
                }
            }

        }

        static List<DatabaseManagerService.TagTransfer> ReadOutputTagValues(Type type)
        {
            DatabaseManagerService.TagTransfer[] tags = twc.GetOutputTagValues(token);
            if (tags == null)
            {
                return null;
            }

            List<DatabaseManagerService.TagTransfer> digitalTags = tags.ToList().Where(tag => tag.Tag.GetType() == type).ToList();
            return digitalTags;
        }

        static void PrintDigitalOutputTagValues()
        {
            List<DatabaseManagerService.TagTransfer> digitalTags = ReadOutputTagValues(typeof(DatabaseManagerService.DigitalOutputTag));
            if (digitalTags == null)
            {
                return;
            }

            var table = new ConsoleTable("Vrednost", "Ime taga", "Opis", "I/O adresa", "Inicijalna vrednost");

            foreach (var transfer in digitalTags)
            {
                DatabaseManagerService.DigitalOutputTag tag = transfer.Tag as DatabaseManagerService.DigitalOutputTag;
                table.AddRow(transfer.Value, tag.TagName, tag.Description, tag.IoAddress, tag.InitialValue);
            }

            table.Write();
        }

        static void PrintAnalogOutputTagValues()
        {
            List<DatabaseManagerService.TagTransfer> analogTags = ReadOutputTagValues(typeof(DatabaseManagerService.AnalogOutputTag));
            if (analogTags == null)
            {
                return;
            }
            var table = new ConsoleTable("Vrednost", "Merna jedinica", "Ime taga", "Opis", "I/O adresa", "Inicijalna vrednost", "Donja granica", "Gornja granica");

            foreach (var transfer in analogTags)
            {
                DatabaseManagerService.AnalogOutputTag tag = transfer.Tag as DatabaseManagerService.AnalogOutputTag;
                table.AddRow(transfer.Value, tag.Unit, tag.TagName, tag.Description, tag.IoAddress, tag.InitialValue, tag.LowLimit, tag.HighLimit);
            }

            table.Write();
        }

        static void PrintDigitalInputValues()
        {
            DatabaseManagerService.Tag[] digitalTags = twc.GetTags(token, typeof(DatabaseManagerService.DigitalInputTag).Name);
            if (digitalTags == null)
            {
                return;
            }

            var table = new ConsoleTable("Ime taga", "Opis", "Drajver", "I/O adresa", "Vreme skeniranja", "Uključeno skeniranje");

            foreach (var tag in digitalTags)
            {
                DatabaseManagerService.DigitalInputTag inTag = tag as DatabaseManagerService.DigitalInputTag;
                table.AddRow(inTag.TagName, inTag.Description, inTag.Driver, inTag.IoAddress, inTag.ScanTime, inTag.ScanOn? "Da" : "Ne");
            }

            table.Write();
        }

        static void PrintAnalogInputTags()
        {
            DatabaseManagerService.Tag[] analogTags = twc.GetTags(token, typeof(DatabaseManagerService.AnalogInputTag).Name);
            if (analogTags == null)
            {
                return;
            }

            var table = new ConsoleTable("Ime taga", "Opis", "Drajver", "I/O adresa", "Vreme skeniranja", "Uključeno skeniranje", "Donja granica", "Gornja granica", "Merna jedinica");

            foreach (var tag in analogTags)
            {
                DatabaseManagerService.AnalogInputTag inTag = tag as DatabaseManagerService.AnalogInputTag;
                table.AddRow(inTag.TagName, inTag.Description, inTag.Driver, inTag.IoAddress, inTag.ScanTime, inTag.ScanOn ? "Da" : "Ne", inTag.LowLimit, inTag.HighLimit, inTag.Unit);
            }

            table.Write();
        }

        static void ShowAnalogOutputTagValues()
        {
            PrintAnalogOutputTagValues();
            ContinueConsole();
        }

        static void ShowDigitalutputTagValues()
        {
            PrintDigitalOutputTagValues();
            ContinueConsole();
        }

        static void PrintTags(Type type)
        {
            if (type == typeof(DatabaseManagerService.DigitalOutputTag))
            {
                PrintDigitalOutputTagValues();
            }
            else if (type == typeof(DatabaseManagerService.AnalogOutputTag))
            {
                PrintAnalogOutputTagValues();
            }
            else if (type == typeof(DatabaseManagerService.DigitalInputTag))
            {
                PrintDigitalInputValues();
            }
            else if (type == typeof(DatabaseManagerService.AnalogInputTag))
            {
                PrintAnalogInputTags();
            }
        }

        static void ChangeDigitalOutputTagValue() {
            ChangeOutputTagValue(typeof(DatabaseManagerService.DigitalOutputTag));

        }

        static void ChangeAnalogOutputTagValue()
        {
            ChangeOutputTagValue(typeof(DatabaseManagerService.AnalogOutputTag));
        }

        static void ChangeOutputTagValue(Type type)
        {
            if (TagsEmpty(type))
            {
                return;
            }

            PrintTags(type);

            Validator.ValidationType = ValidationType.NOT_EMPTY;
            string tagName = InputField("ime izlaznog taga za izmenu");
            if (IsExit(tagName))
            {
                return;
            }

            Validator.ValidationType = ValidationType.ANALOG_NUMBER;
            string newValue = InputField("novu vrednost");
            if (IsExit(newValue))
            {
                return;
            }

            if (!twc.ChangeOutputTagValue(tagName, type.Name, token, double.Parse(newValue)))
            {
                Console.WriteLine("Došlo je do greške prilikom izmene vrednosti!");
            }
            else
            {
                Console.WriteLine("Uspešno izmenjena vrednost!");
            }

            ContinueConsole();
        }

        static void DeleteTag(Type type)
        {
            if (TagsEmpty(type))
            {
                return;
            }

            PrintTags(type);

            Validator.ValidationType = ValidationType.NOT_EMPTY;
            string tagName = InputField("ime taga za brisanje");
            if (IsExit(tagName))
            {
                return;
            }

            if (twc.DeleteTag(tagName, type.Name, token))
            {
                Console.WriteLine("Uspešno obrisan tag!");
            }
            else
            {
                Console.WriteLine("Došlo je do greške prilikom brisanja taga!");
            }

            ContinueConsole();
        }

        static void DeleteAnalogOutputTag()
        {
            DeleteTag(typeof(DatabaseManagerService.AnalogOutputTag));
        }

        static void DeleteDigitalOutputTag()
        {
            DeleteTag(typeof(DatabaseManagerService.DigitalOutputTag));
        }

        static void DeleteDigitalInputTag()
        {
            DeleteTag(typeof(DatabaseManagerService.DigitalInputTag));
        }

        static void DeleteAnalogInputTag()
        {
            DeleteTag(typeof(DatabaseManagerService.AnalogInputTag));
        }

        static void ChangeScanInputTag(Type type, bool scanOn, string message)
        {
            if (TagsEmpty(type))
            {
                return;
            }

            PrintTags(type);

            Validator.ValidationType = ValidationType.NOT_EMPTY;
            string tagName = InputField($"ime taga za {message}");
            if (IsExit(tagName))
            {
                return;
            }

            if (twc.ChangeTagScan(token, tagName, scanOn))
            {
                Console.WriteLine("Uspešno izmenjeno skeniranje!");
            }
            else
            {
                Console.WriteLine("Došlo je do greške prilikom izmene skeniranja!");
            }

            ContinueConsole();
        }

        static void AddAlarmForAnalogInputTag()
        {
            if (TagsEmpty(typeof(DatabaseManagerService.AnalogInputTag)))
            {
                return;
            }

            PrintTags(typeof(DatabaseManagerService.AnalogInputTag));

            Validator.ValidationType = ValidationType.NOT_EMPTY;
            string tagName = InputField($"ime taga za dodavanje alarma");
            if (IsExit(tagName))
            {
                return;
            }

            while (true)
            {
                DatabaseManagerService.Alarm alarm = InputAlarm(tagName);
                if (twc.AddAlarm(tagName, alarm, token))
                {
                    Console.WriteLine("Alarm je uspešno dodat!");
                }
                else
                {
                    Console.WriteLine("Greška prilikom dodavanja alarma!");
                    break;
                }

                Console.Write("Da li želite da unesete još alarma? (da) : ");
                string choice = Console.ReadLine();
                if (choice.ToLower() != "da")
                {
                    break;
                }
            }

            ContinueConsole();
        }

        static void PrintAlarms(DatabaseManagerService.Alarm[] alarms)
        {
            var table = new ConsoleTable("Tip", "Prioritet", "Ime taga", "Id");

            foreach (var alarm in alarms)
            {
                table.AddRow(alarm.AlarmType, alarm.Priority, alarm.TagName, alarm.AlarmId);
            }

            table.Write();
        }

        static void DeleteAlarmForAnalogInputTag()
        {
            if (TagsEmpty(typeof(DatabaseManagerService.AnalogInputTag)))
            {
                return;
            }

            PrintTags(typeof(DatabaseManagerService.AnalogInputTag));

            Validator.ValidationType = ValidationType.NOT_EMPTY;
            string tagName = InputField($"ime taga za brisanje alarma");
            if (IsExit(tagName))
            {
                return;
            }

            DatabaseManagerService.Alarm[] alarms;

            while (true)
            {
                try
                {
                    alarms = (twc.GetTags(token, (typeof(DatabaseManagerService.AnalogInputTag).Name)).Where(t => t.TagName == tagName).First() as DatabaseManagerService.AnalogInputTag).Alarms;
                }
                catch (Exception)
                {
                    Console.WriteLine("Greška, tag sa unetim imenom ne postoji!");
                    ContinueConsole();
                    return;
                }

                if (alarms.Length == 0)
                {
                    Console.WriteLine("Nema alarma za brisanje!");
                    ContinueConsole();
                    return;
                }

                PrintAlarms(alarms);

                Validator.ValidationType = ValidationType.RANGE_NUMBER;
                Validator.lowLimit = (alarms[0].AlarmId).ToString();
                Validator.highLimit = (alarms[alarms.Length-1].AlarmId).ToString();

                string id = InputField($"id alarma za brisanje");
                if (IsExit(id))
                {
                    return;
                }

                if (twc.DeleteAlarm(tagName, Int32.Parse(id), token))
                {
                    Console.WriteLine("Alarm je uspešno obrisan!");
                }
                else
                {
                    Console.WriteLine("Greška prilikom brisanja alarma!");
                    break;
                }

                Console.Write("Da li želite da obrišete još alarma? (da) : ");
                string choice = Console.ReadLine();
                if (choice.ToLower() != "da")
                {
                    break;
                }
            }

            ContinueConsole();
        }

        static void ScanOffDigitalInputTag()
        {
            ChangeScanInputTag(typeof(DatabaseManagerService.DigitalInputTag), false, "isključivanje");
        }

        static void ScanOnDigitalInputTag()
        {
            ChangeScanInputTag(typeof(DatabaseManagerService.DigitalInputTag), true, "uključivanje");
        }

        static void ScanOffAnalogInputTag()
        {
            ChangeScanInputTag(typeof(DatabaseManagerService.AnalogInputTag), false, "isključivanje");
        }

        static void ScanOnAnalogInputTag()
        {
            ChangeScanInputTag(typeof(DatabaseManagerService.AnalogInputTag), true, "uključivanje");
        }

        static bool TagsEmpty(Type type)
        {
            DatabaseManagerService.Tag[] tags = twc.GetTags(token, type.Name);
            if (tags.Length == 0)
            {
                Console.WriteLine("Nema tagova za prikaz.");
                ContinueConsole();
                return true;
            }

            return false;

        }

        static void ContinueConsole()
        {
            Console.WriteLine("Pritisnite enter za nastavak.");
            Console.ReadKey();
        }

        static void Exit()
        {
            exit = true;
        }

        static void Back()
        {
            AddOptionsBasedOnUserType();
        }

        static void AddOptionsBasedOnUserType()
        {
            menu.ClearOptions();

            if (ac.UsersEmpty())
            {
                AddOptionsForEmptyUsers();
            }
            else if (userType == UserType.NOT_LOGGED)
            {
                AddOptionsForNotLoggedIn();
            }
            else if (userType == UserType.USER)
            {
                AddOptionsForUser();
            }
            else if (userType == UserType.ADMIN)
            {
                AddOptionsForAdmin();
            }
        }

        static void AddOptionsForEmptyUsers()
        {
            menu.AddMenuOption("Registracija kao admin na sistem", RegisterUser);
            menu.AddMenuOption("EXIT", Exit);
        }

        static void AddOptionsForNotLoggedIn()
        {
            menu.AddMenuOption("Prijava na sistem", LogIn);
            menu.AddMenuOption("EXIT", Exit);
        }

        static void AddOptionsForTagInput()
        {
            menu.ClearOptions();
            menu.AddMenuOption("Dodavanje digitalnog izlaznog taga", InputDigitalOutputTag);
            menu.AddMenuOption("Dodavanje digitalnog ulaznog taga", InputDigitalInputTag);
            menu.AddMenuOption("Dodavanje analognog izlaznog taga", InputAnalogOutputTag);
            menu.AddMenuOption("Dodavanje analognog ulaznog taga", InputAnalogInputTag);
            menu.AddMenuOption("Nazad", Back);
        }

        static void AddOptionsForTagView()
        {
            menu.ClearOptions();
            menu.AddMenuOption("Prikaz vrednosti digitalnih izlaznih tagova", ShowDigitalutputTagValues);
            menu.AddMenuOption("Prikaz vrednosti analognih izlaznih tagova", ShowAnalogOutputTagValues);
            menu.AddMenuOption("Nazad", Back);
        }

        static void AddOptionsForTagValueChange()
        {
            menu.ClearOptions();
            menu.AddMenuOption("Izmena vrednosti digitalnih izlaznih tagova", ChangeDigitalOutputTagValue);
            menu.AddMenuOption("Izmena vrednosti analognih izlaznih tagova", ChangeAnalogOutputTagValue);
            menu.AddMenuOption("Nazad", Back);
        }

        static void AddOptionsForTagDelete()
        {
            menu.ClearOptions();
            menu.AddMenuOption("Brisanje digitalnog izlaznog taga", DeleteDigitalOutputTag);
            menu.AddMenuOption("Brisanje digitalnog ulaznog taga", DeleteDigitalInputTag);
            menu.AddMenuOption("Brisanje analognog izlaznog taga", DeleteAnalogOutputTag);
            menu.AddMenuOption("Brisanje analognog ulaznog taga", DeleteAnalogInputTag);
            menu.AddMenuOption("Nazad", Back);
        }

        static void AddOptionsForScan()
        {
            menu.ClearOptions();
            menu.AddMenuOption("Uključivanje digitalnog ulaznog taga", ScanOnDigitalInputTag);
            menu.AddMenuOption("Isključivanje digitalnog ulaznog taga", ScanOffDigitalInputTag);
            menu.AddMenuOption("Uključivanje analognog ulaznog taga", ScanOnAnalogInputTag);
            menu.AddMenuOption("Isključivanje analognog ulaznog taga", ScanOffAnalogInputTag);
            menu.AddMenuOption("Nazad", Back);
        }

        static void AddOptionsForAlarms()
        {
            menu.ClearOptions();
            menu.AddMenuOption("Dodavanje alarma za analogni ulazni tag", AddAlarmForAnalogInputTag);
            menu.AddMenuOption("Brisanje alarma za analogni ulazni tag", DeleteAlarmForAnalogInputTag);
            menu.AddMenuOption("Nazad", Back);
        }

        static void AddOptionsForUser()
        {
            menu.AddMenuOption("Dodavanje taga", AddOptionsForTagInput);
            menu.AddMenuOption("Brisanje taga", AddOptionsForTagDelete);
            menu.AddMenuOption("Izmena skeniranja", AddOptionsForScan);
            menu.AddMenuOption("Rad sa alarmima", AddOptionsForAlarms);
            menu.AddMenuOption("Prikaz vrednosti izlaznih tagova", AddOptionsForTagView);
            menu.AddMenuOption("Izmena vrednosti izlaznih tagova", AddOptionsForTagValueChange);
            menu.AddMenuOption("Odjava sa sistema", Logout);
            menu.AddMenuOption("EXIT", Exit);
        }

        static void AddOptionsForAdmin()
        {
            menu.AddMenuOption("Registracija novog korisnika", RegisterUser);
            AddOptionsForUser();
        }

        static void Main(string[] args)
        {
            AddOptionsBasedOnUserType();

            while (true)
            {
                menu.WriteMenu();

                Console.Write("\r\nOdaberite opciju: ");
                String option = Console.ReadLine();
                menu.ExecuteMenuOption(option);

                if (exit)
                {
                    break;
                }
            }
        }
    }
}
