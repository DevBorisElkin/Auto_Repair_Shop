using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Mechanic : Entity<Mechanic>
    {
        public Mechanic(AbstractFactory<Mechanic> factory, int entityId) : base(factory, entityId) { }
    }
}
