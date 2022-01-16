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
        int amountOfClients = 100;
        int maxThreads = 20;

        int sellerWorkDuration = 500; //100
        int clientMovementTimeToPickupPoint = 15; //100
        int mechanicWorkDuration = 1000; //100

        int sellersCount = 4;
        int mechanicsCount = 5;

        AbstractFactory<Seller> sellersFactory;
        AbstractFactory<Mechanic> mechanicsFactory;

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
            Thread.Sleep(sellerWorkDuration); // immitation of DoWork();
            Console.WriteLine($"client {c.clientId} finished work, thread id:{Thread.CurrentThread.ManagedThreadId}, with seller [{seller.entityId}]");
            seller.ReleaseTheEntity();
            SendClientRequestToTheWorkshop(c);
        }

        void ManageClient_GoToPickupPoint(Client c)
        {
            Thread.Sleep(clientMovementTimeToPickupPoint);
            Console.WriteLine($"client {c.clientId} moved to pickup point, thread id:{Thread.CurrentThread.ManagedThreadId}");
        }

        void SendClientRequestToTheWorkshop(Client c)
        {
            // here we start a new thread and searching for mechanic for that specific car
            // then he does the work and sends the car to the pickup point

            ThreadPool.QueueUserWorkItem(
                    new WaitCallback(x =>
                    {
                        FixClientsCar(x);
                    }), c
                );
        }

        void FixClientsCar(object client)
        {
            Client c = (Client)client;
            Mechanic mechanic = mechanicsFactory.GetEntity();
            Thread.Sleep(mechanicWorkDuration);
            Console.WriteLine($"mechanic {mechanic.entityId} finished work for client[{c.clientId}], thread id:{Thread.CurrentThread.ManagedThreadId}");
            mechanic.ReleaseTheEntity();
        }

    }
}
