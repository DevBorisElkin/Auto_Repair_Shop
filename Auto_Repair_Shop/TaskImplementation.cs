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

        int sellerWorkDuration = 10; //100
        int clientMovementTimeToPickupPoint = 15; //100

        int sellersCount = 4;

        AbstractFactory<Seller> sellerFactory;

        public TaskImplementation()
        {
            Console.WriteLine($"chosen scenario: {chosenScenario}");
            Console.WriteLine($"Processors count: {Environment.ProcessorCount}");
            Console.WriteLine("Press any key to start");
            Console.ReadKey();

            List<Seller> sellers = new List<Seller>();
            sellerFactory = new AbstractFactory<Seller>(sellers, sellerWorkDuration);
            for (int i = 1; i <= sellersCount; i++)
                sellers.Add(new Seller(sellerFactory, i));

            
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
            Seller seller = sellerFactory.GetEntity();
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
        }

    }
}
