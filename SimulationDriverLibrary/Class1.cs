using System;

namespace SimulationDriverLibrary
{
    public static class SimulationDriver
    {
        public static double ReturnValue(string address)
        {
            // U ovoj implementaciji simulacionog driver-a, adrese su opisne (po uzoru na iFIX), odnosni string vrednost
            // S - sine
            // C - cosine
            // R - ramp
            // Ovaj deo možete izmeniti u skladu sa svojim potrebama (adrese mogu biti i int, char, itd.)
            if (address == "S") return Sine();
            else if (address == "C") return Cosine();
            else if (address == "R") return Ramp();
            else return -1000;
        }

        private static double Sine()
        {
            return 10 * Math.Sin((double)DateTime.Now.Second / 60 * Math.PI);
        }

        private static double Cosine()
        {
            return 10 * Math.Cos((double)DateTime.Now.Second / 60 * Math.PI);
        }

        private static double Ramp()
        {
            return 10 * DateTime.Now.Second / 60;
        }
    }
}
