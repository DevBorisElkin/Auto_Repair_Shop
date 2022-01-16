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

            var waitingClient = waitingClients.Where(a => a.clientId == id).FirstOrDefault();
            if (waitingClient != null)
                CarWasIssuedForWaitingClient?.Invoke(waitingClient);
        }

        public bool CheckAndPickTheCar(Client c)
        {
            Monitor.Enter(locker);
            if (availableCarsById.Contains(c.clientId))
            {
                availableCarsById.Remove(c.clientId);
                return true;
            }
            else
            {
                waitingClients.Add(c);
            }

            Monitor.Exit(locker);
            return false;
        }
    }
}
