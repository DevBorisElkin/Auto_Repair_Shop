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

        int sellerWorkDuration = 1; //100

        SellerFactory sellerFactory;

        public TaskImplementation()
        {
            Console.WriteLine($"chosen scenario: {chosenScenario}");
            Console.WriteLine($"Processors count: {Environment.ProcessorCount}");

            sellerFactory = new SellerFactory(4);

            Thread.Sleep(3000);
            
            ThreadPool.SetMaxThreads(maxThreads, maxThreads);

            for (int i = 1; i <= amountOfClients; i++)
            {
                Client client = new Client(i);

                ThreadPool.QueueUserWorkItem(
                    new WaitCallback(x =>
                    {
                        ManageClient_RequestStage(x);
                    }), client
                );
            }

            Console.ReadLine();
        }

        void ManageClient_RequestStage(object obj)
        {
            Client c = (Client) obj;

            Seller seller = sellerFactory.GetSeller();
            Thread.Sleep(sellerWorkDuration);
            seller.ReleaseTheSeller();

            Console.WriteLine($"client {c.clientId} finished work, thread id:{Thread.CurrentThread.ManagedThreadId}");
        }

    }
}
