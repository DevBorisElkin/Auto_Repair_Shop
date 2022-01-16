using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class CarPickupPoint
    {
        public List<int> availableCarsById;
        public List<Client> waitingClients;

        public object locker = new object();
        public object locker_clients = new object();

        public Action<Client> CarWasIssuedForWaitingClient;

        public CarPickupPoint()
        {
            availableCarsById = new List<int>();
            waitingClients = new List<Client>();
        }

        public void AddCarToPickupPoint(int id)
        {
            Monitor.Enter(locker);
            availableCarsById.Add(id);
            Monitor.Exit(locker);

            Monitor.Enter(locker_clients);
            var waitingClient = waitingClients.Where(a => a.clientId == id).FirstOrDefault();
            if (waitingClient != null)
            {
                waitingClient.PickCarAfterItsArrival?.Invoke(id);
            }
            Monitor.Exit(locker_clients);
        }

        public bool CheckAndPickTheCar(Client c)
        {
            Monitor.Enter(locker);
            if (availableCarsById.Contains(c.clientId))
            {
                availableCarsById.Remove(c.clientId);
                Monitor.Exit(locker);
                return true;
            }
            else
            {
                Monitor.Enter(locker_clients);
                waitingClients.Add(c);
                Monitor.Exit(locker_clients);
            }

            Monitor.Exit(locker);
            return false;
        }
    }
}
