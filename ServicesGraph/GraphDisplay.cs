using System;
using System.Collections.Generic;

namespace ServicesGraph
{
    class GraphDisplay
    {
        private Dictionary<string, Service> services;
        private List<Transaction> transactions;
        private DateTime startPoint;
        private int interval;

        public GraphDisplay(Dictionary<string, Service> services, List<Transaction> transactions, DateTime startPoint, int interval)
        {
            this.services = services;
            this.transactions = transactions;
            this.startPoint = startPoint;
            this.interval = interval;
        }
        
        public void Display()
        {
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Graph structure:");
            Console.ResetColor();
            //вывод структуры графа
            DisplayGraphStructure();
            //вывод статистики по вершинам и рёбрам
            DisplayTransferActivity();
        }

        
        private void DisplayGraphStructure()
        //функция для вывода статистики всех сервисов коллекции services и их транзакций
        {
            foreach (KeyValuePair<string, Service> service in services)
            {
                Console.WriteLine($"\nService \"{service.Key}\" has link with:");
                //перебираем все рёбра, связанные с текущей вершиной перебираемой коллекции
                foreach(Transaction transact in transactions)
                {
                    if(transact.sourseService == service.Key)
                    {
                        Console.WriteLine($"\t- {transact.recipientServive}");
                    }
                    if (transact.recipientServive == service.Key)
                    {
                        Console.WriteLine($"\t- {transact.sourseService}");
                    }
                }
            }
        }


        private void DisplayTransferActivity()
        //функция для вывода статистики передач между сервисами
        {
            Console.WriteLine();
            foreach (KeyValuePair<string, Service> service in services)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{service.Key} activity:");
                Console.ResetColor();
                foreach (int intervalNumber in service.Value.activity.Keys)
                {
                    Console.WriteLine($"From {startPoint.AddSeconds(interval * (intervalNumber - 1))} " +
                        $"to {startPoint.AddSeconds(interval * intervalNumber)}" +
                        $"\tInteractions quantity = {service.Value.activity[intervalNumber]}");
                    Console.WriteLine("Transactions quantity:");
                    foreach (Transaction transact in transactions)
                    {
                        if (transact.transfers.ContainsKey(intervalNumber) == true)
                        {
                            if (transact.sourseService == service.Key)
                            {

                                Console.WriteLine($"{service.Key} " +
                                    $"---- {transact.recipientServive} " +
                                    $"= {transact.transfers[intervalNumber]}");

                            }
                            if (transact.recipientServive == service.Key)
                            {
                                Console.WriteLine($"{service.Key} ---- " +
                                    $"{transact.sourseService} = " +
                                    $"{transact.transfers[intervalNumber]}");
                            }
                        }
                    }
                }
            }
        }
    }
}