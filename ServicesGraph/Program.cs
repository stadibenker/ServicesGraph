using System;
using System.Collections.Generic;

namespace ServicesGraph
{
    class Program
    {
        static void Main(string[] args)
        //ввод данных по аргументам коммандной строки или в ходе выполнения программы.
        //1й аргумент - имя файла с исходными данными
        //2й аргумент - начало интервала в формате YYYY-MM-DD HH:MI:SS.milliseconds
        //3й аргумент - конец интервала в формате YYYY-MM-DD HH:MI:SS.milliseconds
        {
            GraphCreator graphCreator = new GraphCreator();     //класс для создания графа
            Dictionary<string, Service> nodes;        //словарь вершин, для хранения результата функции CreateGraph
            List<Transaction> edges;                  //список рёбер, для хранения результата функции CreateGraph
            string fileName;                          //имя файла, содержащего исходные данные
            DateTime startInterval, endInterval;      //начальный и конечный интервал, для которого построить граф

            //блок ввода начальных данных
            if (args.Length != 3)
            {
                Console.Write("Enter file name:");
                fileName = Console.ReadLine();
                Console.Write("Enter start of interval:");
                startInterval = DateTime.Parse(Console.ReadLine());
                Console.Write("Enter end of interval:");
                endInterval = DateTime.Parse(Console.ReadLine());
            }
            else
            {
                fileName = args[0];
                startInterval = DateTime.Parse(args[1]);
                endInterval = DateTime.Parse(args[2]);
            }

            //fileName = "input.txt";
            //startInterval = DateTime.Parse("2018-08-15T00:42:01.1156");
            //endInterval = DateTime.Parse("2018-08-15T00:42:05.1156");

            //вызов функции создания графа
            graphCreator.CreateGraph(fileName, startInterval, endInterval, out nodes, out edges);
            
            GraphDisplay graphDisplay = new GraphDisplay(nodes, edges, startInterval, graphCreator.interval);     //класс для вывода графа
            //вызов функции вывода графа
            graphDisplay.Display();
            Console.ReadKey();
        }
    }
}
