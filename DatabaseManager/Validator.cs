using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager
{
    public enum ValidationType
    {
        NOT_EMPTY,
        DIGITAL_NUMBER,
        ANALOG_NUMBER,
        RANGE_NUMBER,
        SIMULATION_ADDRESS
    }

    public static class Validator
    {
        public static ValidationType ValidationType { get; set; }
        public static string lowLimit = "";
        public static string highLimit = "";

        public static bool ValidateField(string field)
        {
            if (ValidationType == ValidationType.NOT_EMPTY)
            {
                return ValidateEmpty(field);
            }
            else if (ValidationType == ValidationType.DIGITAL_NUMBER)
            {
                return ValidateDigitalNumber(field);
            }
            else if (ValidationType == ValidationType.ANALOG_NUMBER)
            {
                return ValidateAnalogNumber(field);
            }
            else if (ValidationType == ValidationType.RANGE_NUMBER)
            {
                return ValidateRangeNumber(field);
            } 
            else if (ValidationType == ValidationType.SIMULATION_ADDRESS)
            {
                return ValidateSimulationAddress(field);
            }

            return false;
        }

        private static bool ValidateEmpty(string field)
        {
            if (field == "")
            {
                Console.WriteLine("\nPolje ne sme biti prazno!");
                return false;
            }

            return true;
        }

        private static bool ValidateDigitalNumber(string field)
        {
            if (field != "1" && field != "0")
            {
                Console.WriteLine("\nMoguće vrednosti su 0 ili 1!");
                return false;
            }

            return true;
        }

        private static bool ValidateAnalogNumber(string field)
        {
            try
            {
                double value = double.Parse(field);

                if (lowLimit != "" && double.Parse(lowLimit) > value)
                {
                    Console.WriteLine("\nNe može vrednost biti manja od donje granice!");
                    return false;
                }

                if (highLimit != "" && double.Parse(highLimit) < value)
                {
                    Console.WriteLine("\nNe može vrednost biti veća od gornje granice!");
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool ValidateRangeNumber(string field)
        {
            try
            {
                int value = Int32.Parse(field);

                if (lowLimit != "" && Int32.Parse(lowLimit) > value)
                {
                    Console.WriteLine("\nBroj nije dobro unet!");
                    return false;
                }

                if (highLimit != "" && Int32.Parse(highLimit) < value)
                {
                    Console.WriteLine("\nBroj nije dobro unet!");
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool ValidateSimulationAddress(string field)
        {
            if (field != "S" && field != "C" && field != "R")
            {
                Console.WriteLine("\nMoguće vrednosti su S, C ili R");
                return false;
            }

            return true;
        }
    }
}
