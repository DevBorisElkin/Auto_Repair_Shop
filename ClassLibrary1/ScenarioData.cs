using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public struct ScenarioData
    {
        public int delayOrder;
        public int delayClient;
        public int delayProduction;

        public ScenarioData(int delayOrder, int delayClient, int delayProduction)
        {
            this.delayOrder = delayOrder;
            this.delayClient = delayClient;
            this.delayProduction = delayProduction;
        }

        public override string ToString() => $"delayOrder[{delayOrder}], delayClient[{delayClient}], delayProduction[{delayProduction}]";
    }
}
