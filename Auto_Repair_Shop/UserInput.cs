using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Repair_Shop
{
    public static class UserInput
    {
        public static ScenarioData chosenScenario;

        private static int chosenScenarioInt;
        private static string wrongInput = "Please, enter a single digit from 1 to 5, for example, '1' and then press 'Enter' key";
        private static Dictionary<int, ScenarioData> scenarioDatas;

        public static void GetScenarioDataByUserInput()
        {
            InitScenarioDictionary();
            ScenarioDescription();
            ManageInput();
        }

        static void InitScenarioDictionary()
        {
            scenarioDatas = new Dictionary<int, ScenarioData>();
            scenarioDatas.Add(1, new ScenarioData(0, 0, 0));
            scenarioDatas.Add(2, new ScenarioData(1, 2, 0));
            scenarioDatas.Add(3, new ScenarioData(0, 0, 1));
            scenarioDatas.Add(4, new ScenarioData(0, 1, 1));
            scenarioDatas.Add(5, new ScenarioData(1, 0, 1));
        }
        private static void ScenarioDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Choose scenario by entering single digit from 1 to 5 and then pressing 'Enter':");
            sb.AppendLine("1) tЗаявка = 0, tКлиент = 0, tПроизводство = 0");
            sb.AppendLine("2) tЗаявка = 1мс, tКлиент = 2мс, tПроизводство = 0");
            sb.AppendLine("3) tЗаявка = 0, tКлиент = 0, tПроизводство = 1");
            sb.AppendLine("4) tЗаявка = 0, tКлиент = 1, tПроизводство = 1");
            sb.AppendLine("5) tЗаявка = 1, tКлиент = 0, tПроизводство = 1");
            Console.WriteLine(sb);
        }
        static void ManageInput()
        {
            ScenarioData data;

            while (true)
            {
                bool result = ScenarioChoice();
                if (!result && scenarioDatas.TryGetValue(chosenScenarioInt, out data)) break;
            }

            Console.WriteLine($"User chosen scenario {chosenScenarioInt}");
            chosenScenario = data;
        }
        private static bool ScenarioChoice()
        {
            string key = Console.ReadLine();
            if (key.Length > 1)
            {
                Console.WriteLine(wrongInput);
                return true;
            }

            bool correctInput = Int32.TryParse(key, out chosenScenarioInt);
            if (!correctInput)
            {
                Console.WriteLine(wrongInput);
                return true;
            }

            if(chosenScenarioInt < 1 || chosenScenarioInt > 5)
            {
                Console.WriteLine(wrongInput);
                return true;
            }

            return false;
        }
    }
}
