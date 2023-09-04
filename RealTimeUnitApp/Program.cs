using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeUnitApp
{
    class Program
    {
        static RealTimeDriverService.RealTimeDriverClient rtdc = new RealTimeDriverService.RealTimeDriverClient();
        static CspParameters csp = null;
        static RSACryptoServiceProvider rsa = null;
        const string EXPORT_FOLDER = @"C:\public_key\";

        static void CreateAsmKeys()
        {
            csp = new CspParameters();
            rsa = new RSACryptoServiceProvider(csp);
        }

        static byte[] SignMessage(string message)
        {
            using (SHA256 sha = SHA256Managed.Create())
            {
                var hashValue = sha.ComputeHash(Encoding.UTF8.GetBytes(message));
                var formatter = new RSAPKCS1SignatureFormatter(rsa);
                formatter.SetHashAlgorithm("SHA256");
                return formatter.CreateSignature(hashValue);
            }

        }

        static string ExportPublicKey(string rtuId)
        {
            if (!(Directory.Exists(EXPORT_FOLDER)))
                Directory.CreateDirectory(EXPORT_FOLDER);

            string path = Path.Combine(EXPORT_FOLDER, $"javniKljuc{rtuId}.txt");

            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(rsa.ToXmlString(false));
            }

            return path;
        }

        static void Main(string[] args)
        {
            string rtuId;
            double lowLimit;
            double highLimit;
            string rtuAddress;
            while (true)
            {
                try
                {
                    Console.Write("Unesite id real time unita : ");
                    rtuId = Console.ReadLine();

                    Console.Write("Unesite adresu real time unita : ");
                    rtuAddress = Console.ReadLine();

                    if (rtuId == "" || rtuAddress == "")
                    {
                        Console.WriteLine("Ne smeju polja biti prazna!");
                        continue;
                    }

                    Console.Write("Unesite donju granicu za vrednost : ");
                    lowLimit = double.Parse(Console.ReadLine());

                    Console.Write("Unesite gornju granicu za vrednost : ");
                    highLimit = double.Parse(Console.ReadLine());

                    if (highLimit <= lowLimit)
                    {
                        Console.WriteLine("Gornja granica nije dobra!");
                        continue;
                    }

                    CreateAsmKeys();
                    string path = ExportPublicKey(rtuId);
                    if (!rtdc.InitRealTimeUnit(rtuId, path, rtuAddress))
                    {
                        Console.WriteLine("Id ili adresa već postoje!");
                        continue;
                    }
                    Console.WriteLine("Uspešno registrovan real time unit!");
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("Niste uneli broj!");
                }
            }

            Random rnd = new Random();

            while (true)
            {
                double value = rnd.NextDouble() * (highLimit - lowLimit) + lowLimit;
                string message = string.Format("{0:F6}", value);
                byte[] signature = SignMessage(message);
                rtdc.WriteValue(rtuId, message, rtuAddress, signature);
                Thread.Sleep(3000);
            }
        }
    }
}
