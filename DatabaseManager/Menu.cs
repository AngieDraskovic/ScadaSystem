using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManager
{
    public delegate void Option();


    class MenuOption
    {
        public string Name { get; set; }
        public Option Callback { get; set; }

        public MenuOption() { }

        public MenuOption(string name, Option callback)
        {
            Name = name;
            Callback = callback;
        }
    }

    public class Menu
    {
        private List<MenuOption> _options;
        private string _stars = new String('*', 25);

        public Menu()
        {
            _options = new List<MenuOption>();
        }

        public void AddMenuOption(string name, Option callback)
        {
            MenuOption mo = new MenuOption(name, callback);
            _options.Add(mo);
        }

        public void ClearOptions()
        {
            _options.Clear();
        }

        public void ExecuteMenuOption(String optionNumber)
        {
            int optionNumberVal;

            try
            {
                optionNumberVal = Int32.Parse(optionNumber) - 1;
            }
            catch (Exception)
            {
                return;
            }

            if (optionNumberVal >= _options.Count() || optionNumberVal < 0)
            {
                return;
            }

            Console.Clear();
            Console.WriteLine(_stars + _options[optionNumberVal].Name + _stars);

            Validator.lowLimit = "";
            Validator.highLimit = "";
            _options[optionNumberVal].Callback();
        }

        public void WriteMenu()
        {
            Console.Clear();
            Console.WriteLine(_stars + "MENI" + _stars);
            Console.WriteLine("Dostupne opcije:");

            foreach (var item in _options.Select((value, i) => new { i, value }))
            {
                var value = item.value;
                Console.WriteLine($"{item.i + 1}) {value.Name}");
            }
        }
    }
}
