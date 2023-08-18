using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseManager.ServiceReference1;

namespace DatabaseManager
{
    internal class Program
    {
        private static string[] menuItems = { "Dodavanje taga", "Uklanjanje taga", "Registracije korisnika", "Upisivanje vrednosti",
                                              "Prikaz trenutnih vrednosti", "Podešavanje skeniranja ulaznih tagova", "Dodavanje alarma", "Log out" };

        private static readonly string INPUT_ERROR_MSG = "Unos nije validan, pokušajte ponovo.";
        static DBManagerServiceClient serviceClient = new DBManagerServiceClient();

        static void Main(string[] args)
        {
            while (true)
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
        }

        static void LogIn()
        {
            Console.Write("Korisničko ime >> ");
            string username = Console.ReadLine();
            Console.Write("Lozinka >> ");
            string password = Console.ReadLine();

            string result = serviceClient.LogIn(username, password);
            if (result != "Login neuspjesan")
            {
                Console.WriteLine("Uspješna prijava!");
                ShowMenu();
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
            while (true)
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
                        // Dodavanje taga
                        break;
                    case "2":
                        // Uklanjanje taga
                        break;
                    // ... Ostale opcije ...
                    case "8":
                        // Log out
                        Console.WriteLine("Odjavljeni ste.");
                        return;
                    default:
                        Console.WriteLine(INPUT_ERROR_MSG);
                        break;
                }
            }
        }
    }
}
