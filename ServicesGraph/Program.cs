using System;
using System.Collections.Generic;

namespace ServicesGraph
{
    class Program
    {
        static void Main(string[] args)
        //ввод данных по аргументам коммандной строки или в ходе выполнения программы.
        //1й аргумент - имя файла с исходными данными
        //2й аргумент - 
        //3й аргумент - 
        {
            GraphCreator graphCreator = new GraphCreator();     //класс для создания графа
            GraphDisplay graphDisplay = new GraphDisplay();     //класс для вывода графа
            Dictionary<string, Service> graph;                  //словарь, содержащий граф
            string fileName;                          //имя файла, содержащего исходные данные
            DateTime startInterval, endInterval;      //начальный и конечный интервал, для которого построить граф

            //блок ввода начальных данных
            /*if (args.Length != 3)
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
            }*/

            fileName = @"D:\Архив\Работы\Projects\ServicesGraph\ServicesGraph\input.txt";
            startInterval = DateTime.Parse("2018-08-15T00:42:01.1156");
            endInterval = DateTime.Parse("2018-08-15T00:42:01.1156");

            //вызов функции создания графа
            graph = graphCreator.CreateGraph(fileName, startInterval, endInterval);
            //вызов функции вывода графа
            graphDisplay.Display(graph, graphCreator.startingPoint, graphCreator.interval);
            Console.ReadKey();
        }
    }
}
