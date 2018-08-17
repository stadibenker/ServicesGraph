using System;
using System.Collections.Generic;
using System.IO;

namespace ServicesGraph
{
    //класс, описывающий сервисы (вершины графа)
    class Service
    {
        //словарь, содержащий № временных интервалов и количество взаимосвязей для каждого из них
        public Dictionary<int, int> activity = new Dictionary<int, int>();  
    }

    //структура, описывающая связи между сервисами (рёбра графа)
    struct Transaction
    {
        public string sourseService;            //имя сервиса-отправителя (from)
        public string recipientServive;         //имя сервиса-получателя  (to)
        public Dictionary<int, int> transfers;  //карта активности: ключ - № временного интервала, значение - количество транзакций для этого интервала
    }

    class GraphCreator
    {
        public int interval = 10;             //единица времени в секундах, на равные доли которой разбивается введённый промежуток времени.
                                              //если > общего промежутка времени, указанного при старте программы,
                                              //то расчёт кол-ва взаимодействий и передач между сервисами происходит для всего интервала без разбития на единицы времени.

        private DateTime startingPoint { get; set; }        //начальное время, которое будет вычитаться из времени прихода запроса для определения № временного интервала

        private int intervalNumber { get; set; }            //номер временного интервала (увеличивается, зависит от переменной interval и времени поступления запроса)

        private Dictionary<string, Service> services = new Dictionary<string, Service>();     //словарь сервисов
        private List<Transaction> transactions = new List<Transaction>();                     //список связей


        public void CreateGraph(string fileName, DateTime startInterval, DateTime endInterval, 
                                out Dictionary<string, Service> outServices, out List<Transaction> outTransactions)
        //функция для создания графа. на вход принимает имя файла, начальный и конечный интервал, для которого строить граф
        //возвращает словарь с сервисами (вершины) и список связей между ними (рёбра) 
        {
            string text;        //хранит строку, считанную с файла
            StreamReader read = new StreamReader(fileName);
            DateTime currentDate;                  //текущая дата, используется для определения временного интервала относительно начального времени startInterval
            startingPoint = startInterval;         //записываем стартовый интервал
            TimeSpan timeSpan;
            timeSpan = endInterval.Subtract(startInterval);   //определение величины интервала
            if (interval > (int)timeSpan.TotalSeconds) interval = (int)timeSpan.TotalSeconds;   //обрезаем interval, если он > величины интервала

            while ((text = read.ReadLine()) != null)
            {
                //вытаскиваем со строки дату и имена отправителя и получателя
                string s1 = " ";
                char []s2 = s1.ToCharArray();
                string[] part = text.Split(s2);
                string dateTime = part[0];
                string sourseService = part[1];
                string recipientService = part[2];
                if (sourseService == recipientService) continue;    //пропускаем петли (отправка сообщения самому себе)
                currentDate = DateTime.Parse(dateTime);
                if (currentDate < startInterval) continue;          //пропускаем времена, которые раньше указанного интервала
                if (currentDate > endInterval) break;               //если время выходит за интервал - завершаем чтение

                //вычисляем № временного интервала
                intervalNumber = GetIntervalNumber(currentDate);

                //если в графе нет сервиса отправителя или получателя, то создаём
                if (services.ContainsKey(sourseService) == false)
                {
                    CreateService(sourseService);
                }
                if (services.ContainsKey(recipientService) == false)
                {
                    CreateService(recipientService);
                }

                //увеличиваем число взаимосвязей для каждого из двух сервисов
                SetActivity(sourseService);
                SetActivity(recipientService);

                //связываем сервисы между собой и увеличиваем счётчик передач данных по ребру
                SetTransaction(sourseService, recipientService);
            }
            read.Close();

            outServices = services;
            outTransactions = transactions;
        }


        private int GetIntervalNumber(DateTime currentDate)
        //функция для определения номера временного интервала от стартового интервала до поступления очередного запроса
        {
            TimeSpan timeSpan;
            timeSpan = currentDate.Subtract(startingPoint);   //определение интервала
            int intervalNum = (int)timeSpan.TotalMilliseconds / (interval * 1000) + 1;     //расчёт номера интервала
            //if (intervalNum < 0) intervalNum = 1;
            return intervalNum;
        }


        private void CreateService(string serviceName)
        //функция создаёт новый сервис под именем, указанным в serviceName и добавляет его в словарь
        {
            Service newService = new Service();
            services.Add(serviceName, newService);
        }


        private void SetActivity(string serviceName)
        //функция для увеличения числа взаимосвязей для сервиса с именем, указанного в serviceName
        {
            //если указанный промежуток времени для данного сервиса существует в словаре, то увеличиваем количество взаимосвязей
            if (services[serviceName].activity.ContainsKey(intervalNumber) == true)
            {
                services[serviceName].activity[intervalNumber]++;
            }
            //иначе создаём новый для текущего промежутка времени и устанавливаем число связей 1
            else
            {
                services[serviceName].activity.Add(intervalNumber, 1);
            }
        }


        private void SetTransaction(string sourseService, string recipientServive)
        //функция для установки связи и увеличения счётчика взаимодействий между двумя сервисами, указанных в sourseService и recipientServive
        {
            bool isFind = false;    //флаг, указывающий, найдена ли транзакция в списке
            foreach (Transaction transact in transactions)
            {
                //если связь sourseService - recipientServive есть (в любом направлении), то увеличиваем количество передач для этой связи на 1
                if (((transact.sourseService == sourseService || transact.sourseService == recipientServive)
                    && (transact.recipientServive == sourseService || transact.recipientServive == recipientServive)) == true)
                {
                    //если уже есть счётчик связей для текущего интервала времени - увеличиваем его на 1
                    if (transact.transfers.ContainsKey(intervalNumber) == true)
                    {
                        transact.transfers[intervalNumber]++;
                    }
                    //если отсутсвует счётчик связей для текущего интервала времени - создаём и устанавливаем 1
                    else
                    {
                        transact.transfers.Add(intervalNumber, 1);
                    }
                    //устанавливаем, что нашли связь
                    isFind = true;
                    break;
                }
            }

            //если связи нет - создаём
            if (isFind == false)
            {
                Transaction newTransaction = new Transaction();
                newTransaction.sourseService = sourseService;
                newTransaction.recipientServive = recipientServive;
                newTransaction.transfers = new Dictionary<int, int>();
                newTransaction.transfers.Add(intervalNumber, 1);
                transactions.Add(newTransaction);       //добавляем связь в список
            }
        }
    }
}