using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Auto_Repair_Shop.UserInput;

namespace Auto_Repair_Shop
{
    public class TaskImplementation
    {
        int amountOfClients = 20;
        int maxThreads = 20;

        int sellerWorkDuration = 100; //100
        int clientMovementTimeToPickupPoint = 1; //100
        int mechanicWorkDuration = 100; //100

        int sellersCount = 4;
        int mechanicsCount = 5;

        AbstractFactory<Seller> sellersFactory;
        AbstractFactory<Mechanic> mechanicsFactory;
        ThreadFactory threadFactory;
        CarPickupPoint carPickupPoint;

        public TaskImplementation()
        {
            Console.WriteLine($"chosen scenario: {chosenScenario}");
            Console.WriteLine($"Processors count: {Environment.ProcessorCount}");
            Console.WriteLine("Press any key to start");
            Console.ReadKey();

            List<Seller> sellers = new List<Seller>();
            sellersFactory = new AbstractFactory<Seller>(sellers, sellerWorkDuration);
            for (int i = 1; i <= sellersCount; i++)
                sellers.Add(new Seller(sellersFactory, i));

            List<Mechanic> mechanics = new List<Mechanic>();
            mechanicsFactory = new AbstractFactory<Mechanic>(mechanics, mechanicWorkDuration);
            for (int i = 0; i < mechanicsCount; i++)
                mechanics.Add(new Mechanic(mechanicsFactory, i));

            threadFactory = new ThreadFactory(5, 1);
            carPickupPoint = new CarPickupPoint();

            ThreadPool.SetMaxThreads(maxThreads, maxThreads);

            for (int i = 1; i <= amountOfClients; i++)
            {
                Client client = new Client(i);

                ThreadPool.QueueUserWorkItem(
                    new WaitCallback(x =>
                    {
                        ManageClient(x);
                    }), client
                );
            }

            Console.ReadLine();
        }

        void ManageClient(object obj)
        {
            Client c = (Client)obj;
            ManageClient_RequestStage(c);
            ManageClient_GoToPickupPoint(c);
        }

        void ManageClient_RequestStage(Client c)
        {
            Seller seller = sellersFactory.GetEntity();
            Thread.Sleep(sellerWorkDuration);
            Console.WriteLine($"client {c.clientId} finished work, thread id:{Thread.CurrentThread.ManagedThreadId}, with seller [{seller.entityId}]");
            seller.ReleaseTheEntity();
            SendClientRequestToTheWorkshop(c);
        }

        void ManageClient_GoToPickupPoint(Client c)
        {
            Thread.Sleep(clientMovementTimeToPickupPoint);
            Console.WriteLine($"client {c.clientId} moved to pickup point, thread id:{Thread.CurrentThread.ManagedThreadId}");

            if (carPickupPoint.CheckAndPickTheCar(c))
                Console.WriteLine($"client {c.clientId} FINALLY RECEIVED THE CAR, thread id:{Thread.CurrentThread.ManagedThreadId}");
            else carPickupPoint.CarWasIssuedForWaitingClient += PickTheCarAfterItsArriaval; 
        }

        void PickTheCarAfterItsArriaval(Client c)
        {
            if (carPickupPoint.CheckAndPickTheCar(c))
                Console.WriteLine($"client {c.clientId} FINALLY RECEIVED THE CAR, thread id:{Thread.CurrentThread.ManagedThreadId}");
            else
                Console.WriteLine($"client {c.clientId} FOR UNKNOWN REASON COULDN'T GET HIS CAR:{Thread.CurrentThread.ManagedThreadId}");
        }

        // running out of time
        void SendClientRequestToTheWorkshop(Client c)
        {
            Thread thread = new Thread(() => 
            {
                var threadInstance = threadFactory.GetFreeThread();
                threadInstance.SetWork(FixClientsCar, c, threadInstance.ReleaseTheThread);
                threadInstance.LaunchThread();
            });
        }

        void FixClientsCar(object client)
        {
            Client c = (Client)client;
            Mechanic mechanic = mechanicsFactory.GetEntity();
            Console.WriteLine($"mechanic {mechanic.entityId} STARTED work for client[{c.clientId}], thread id:{Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(mechanicWorkDuration);
            Console.WriteLine($"mechanic {mechanic.entityId} finished work for client[{c.clientId}], thread id:{Thread.CurrentThread.ManagedThreadId}");
            carPickupPoint.AddCarToPickupPoint(c.clientId);
            mechanic.ReleaseTheEntity();
        }
    }
}
