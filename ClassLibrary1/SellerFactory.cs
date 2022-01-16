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

        private int sellerProcessDelay;

        public SellerFactory(int amountOfSellers, int sellerProcessDelay)
        {
            this.sellerProcessDelay = sellerProcessDelay;
            autoResetEvent = new AutoResetEvent(true);
            sellers = new List<Seller>();
            //for (int i = 1; i <= amountOfSellers; i++)
            //    sellers.Add(new Seller(this, i));

            ClientReleased += GiveThreadASeller;
        }
        ~SellerFactory() { ClientReleased -= GiveThreadASeller; }

        void GiveThreadASeller(Seller a)
        {
            autoResetEvent.Set();
        }
        AutoResetEvent autoResetEvent;
        public Seller GetSeller()
        {
            bool foundFreeSeller = TryGetVacantSeller(out Seller seller);
            if (foundFreeSeller) return seller;

            autoResetEvent.WaitOne();

            foundFreeSeller = TryGetVacantSellerPersistent(out seller);
            if (foundFreeSeller) return seller;
            else Console.WriteLine("Error, couldn't find free seller althought there's should be at least one");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("____");
            sb.AppendLine($"Found no sellers, thread id[{Thread.CurrentThread.ManagedThreadId}]");
            foreach (var a in sellers)
                sb.AppendLine($"seller lock flag: [{a._lockFlag}]");
            sb.AppendLine("____");
            Console.WriteLine(sb);

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

        bool TryGetVacantSellerPersistent(out Seller seller)
        {
            seller = null;
            int iteration = 1;
            while (seller == null)
            {
                foreach (var a in sellers)
                {
                    if (Interlocked.CompareExchange(ref a._lockFlag, 1, 0) == 0)
                    {
                        Monitor.Enter(a);
                        seller = a;

                        Console.WriteLine($"Finished Persistent Seller aquasition on iteration [{iteration}], thread [{Thread.CurrentThread.ManagedThreadId}]");
                        return true;
                    }
                }
                iteration++;

                // this thing makes wonders happen. Without it iterations can get up to 4000.
                // With it still it's not perfect but at least I know that the app is running safe
                int chosenDelay = Math.Clamp(sellerProcessDelay, 1, 1000);
                Thread.Sleep(chosenDelay);
            }
            return false;
        }
    }
}
        
    

