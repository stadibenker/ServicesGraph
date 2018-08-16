using System;
using System.Collections.Generic;
using System.Text;

namespace ServicesGraph
{
    class GraphDisplay
    {
        public void Display(Dictionary<string, Service> services, DateTime startPoint, int interval)
        {
            int intervalNumber = 0;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Graph structure:");
            Console.ResetColor();
            //вывод структуры графа
            DisplayGraphStructure(services);
            DisplayTransferActivity(services, startPoint, interval);
        }


        private void DisplayGraphStructure(Dictionary<string, Service> services)
        //функция для вывода статистики всех сервисов коллекции services и их транзакций
        {
            foreach (KeyValuePair<string, Service> service in services)
            {
                Console.WriteLine($"\nService \"{service.Key}\" has link with:");
                //перебираем все рёбра, связанные с текущей вершиной перебираемой коллекции
                foreach (KeyValuePair<string, Transaction> transaction in service.Value.transactions)
                {
                    Console.WriteLine($"\t- { transaction.Key}");
                }
            }
        }


        private void DisplayTransferActivity(Dictionary<string, Service> services, DateTime startPoint, int interval)
        //функция для вывода статистики передач между сервисами
        {
            int intervalNumber = 1;
            int count = 0;
            string sourseService, recipientServive;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nServices activity:");
            Console.ResetColor();
            //вывод статистики
            foreach (KeyValuePair<string, Service> service in services)
            {
                sourseService = service.Key;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{sourseService} activity:");
                Console.ResetColor();

                foreach (KeyValuePair<string, Transaction> transaction in service.Value.transactions)
                {
                    Service linkService = transaction.Value.relatedService;
                    recipientServive = transaction.Key;

                    if(transaction.Value.transfers.ContainsKey(intervalNumber) == true)
                    {
                        count += linkService.transactions[sourseService].transfers[intervalNumber];
                    }
                    foreach (KeyValuePair<int, int> act in transaction.Value.transfers)
                    {
                        Console.WriteLine(act.Value + linkService.transactions[sourseService].transfers[intervalNumber]);
                    }
                }
            }
        }
    }
}