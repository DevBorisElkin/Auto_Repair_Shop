using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class SellerFactory
    {
        private List<Seller> sellers;

        public Action<Seller> ClientReleased;

        public SellerFactory(int amountOfSellers)
        {
            sellers = new List<Seller>();
            for (int i = 0; i < amountOfSellers; i++)
                sellers.Add(new Seller(this));

            ClientReleased += GiveThreadASeller;
        }

        void GiveThreadASeller(Seller a)
        {
            autoResetEvent.Set();
        }
        AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        public Seller GetSeller()
        {
            bool foundFreeSeller = TryGetVacantSeller(out Seller seller);
            if (foundFreeSeller) return seller;

            autoResetEvent.WaitOne();

            foundFreeSeller = TryGetVacantSeller(out seller);
            if (foundFreeSeller) return seller;
            else Console.WriteLine("Error, couldn't find free seller althought there's should be at least one");
            
            return null;
        }

        private bool TryGetVacantSeller(out Seller seller)
        {
            seller = null;
            foreach (var a in sellers)
            {
                if (Interlocked.CompareExchange(ref a._lockFlag, 1, 0) == 0)
                {
                    Monitor.Enter(a);
                    seller = a;
                    return true;
                }
            }
            return false;
        }
    }
}
        
    

