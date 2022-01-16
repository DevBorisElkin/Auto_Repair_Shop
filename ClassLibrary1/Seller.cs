using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Seller : Entity<Seller>
    {
        public Seller(AbstractFactory<Seller> factory, int entityId) : base(factory, entityId) { }
    }
}
