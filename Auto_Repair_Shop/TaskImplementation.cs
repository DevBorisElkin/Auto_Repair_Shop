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

        SellerFactory sellerFactory;

        public TaskImplementation()
        {
            Console.WriteLine($"chosen scenario: {chosenScenario}");
            Console.WriteLine($"Processors count: {Environment.ProcessorCount}");
            Console.WriteLine("Press any key to start");
            Console.ReadKey();

            sellerFactory = new SellerFactory(4, sellerWorkDuration);

            
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
            Seller seller = sellerFactory.GetSeller();
            Thread.Sleep(sellerWorkDuration);
            Console.WriteLine($"client {c.clientId} finished work, thread id:{Thread.CurrentThread.ManagedThreadId}, with seller [{seller.sellerId}]");
            seller.ReleaseTheSeller();

        }

        void ManageClient_GoToPickupPoint(Client c)
        {
            Thread.Sleep(clientMovementTimeToPickupPoint);
            Console.WriteLine($"client {c.clientId} moved to pickup point, thread id:{Thread.CurrentThread.ManagedThreadId}");
        }

    }
}
