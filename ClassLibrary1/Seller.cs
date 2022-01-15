using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Seller
    {
        SellerFactory sellerFactory;
        public int sellerId;
        public Seller(SellerFactory factory, int sellerId)
        {
            sellerFactory = factory;
            this.sellerId = sellerId;
        }

        public int _lockFlag = 0;

        public void ReleaseTheSeller()
        {
            //int oldLockFlag = _lockFlag;
            Interlocked.Decrement(ref _lockFlag);
            Monitor.Exit(this);
            //Console.WriteLine($"Releasing the seller, old lock flag[{oldLockFlag}], current[{_lockFlag}]");
            sellerFactory.ClientReleased?.Invoke(this);
        }
    }
}
