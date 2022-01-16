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
        int amountOfClients = 1000;
        int maxThreads = 20;

        int sellerWorkDuration = 0;                 // e.g. order
        int clientMovementTimeToPickupPoint = 0;    // e.g. client
        int mechanicWorkDuration = 0;               // e.g. production

        int sellersCount = 4;
        int mechanicsCount = 5;

        AbstractFactory<Seller> sellersFactory;
        AbstractFactory<Mechanic> mechanicsFactory;
        ThreadFactory threadFactory;
        CarPickupPoint carPickupPoint;

        public TaskImplementation()
        {
            // comment out these 3 lines to insert your custom values above
            sellerWorkDuration = chosenScenario.delayOrder;
            clientMovementTimeToPickupPoint = chosenScenario.delayClient;
            mechanicWorkDuration = chosenScenario.delayProduction;

            Console.WriteLine($"chosen scenario: {chosenScenario}");
            Console.WriteLine($"Processors count: {Environment.ProcessorCount}");
            Console.WriteLine("Press any key to start");
            Console.ReadKey();

            List<Seller> sellers = new List<Seller>();
            sellersFactory = new AbstractFactory<Seller>(sellers, sellerWorkDuration);
            for (int i = 1; i <= sellersCount; i++) sellers.Add(new Seller(sellersFactory, i));

            List<Mechanic> mechanics = new List<Mechanic>();
            mechanicsFactory = new AbstractFactory<Mechanic>(mechanics, mechanicWorkDuration);
            for (int i = 0; i < mechanicsCount; i++) mechanics.Add(new Mechanic(mechanicsFactory, i));

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

            //while(true) // if you need it to not stop after pressing enter again
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

            // running out of time, this produces too much garbage, as well as Thread factory, which starts new threads
            void SendClientRequestToTheWorkshop(Client c)
            {
                Thread thread = new Thread(() =>
                {
                    var threadInstance = threadFactory.GetFreeThread();
                    threadInstance.SetWork(FixClientsCar, c, threadInstance.ReleaseTheThread);
                    threadInstance.LaunchThread();
                });
                thread.Start();
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

        void ManageClient_GoToPickupPoint(Client c)
        {
            Thread.Sleep(clientMovementTimeToPickupPoint);
            Console.WriteLine($"client {c.clientId} moved to pickup point, thread id:{Thread.CurrentThread.ManagedThreadId}");

            if (carPickupPoint.CheckAndPickTheCar(c))
                Console.WriteLine($"client {c.clientId} FINALLY RECEIVED THE CAR, thread id:{Thread.CurrentThread.ManagedThreadId}");
            else c.PickCarAfterItsArrival += PickTheCarAfterItsArriaval;
        }

        void PickTheCarAfterItsArriaval(int id)
        {
            Console.WriteLine($"client {id} FINALLY RECEIVED THE CAR, BUT CAR ARRIVED A BIT LATER, thread id:{Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
