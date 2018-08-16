using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServicesGraph
{
    //класс, описывающий сервисы (вершины графа)
    class Service
    {
        //словарь, содержащий № временных интервалов и количество взаимосвязей для каждого из них
        public Dictionary<int, int> activity = new Dictionary<int, int>();  
        //словарь, содержащий имена сервисов, с которыми есть связь и ссылки на них
        public Dictionary<string, Transaction> transactions = new Dictionary<string, Transaction>();    
    }

    //класс, описывающий связи между сервисами (рёбра графа)
    class Transaction
    {
        //cсылка на связанный сервис
        public Service relatedService = new Service();                 
        //словарь с количеством транзакций для данного направления для временных интервалов intervalNumber
        public Dictionary<int, int> transfers = new Dictionary<int, int>();       
    }




    class GraphCreator
    {
        public readonly int interval = 10;    //единица времени в секундах, на равные доли которой разбивается введённый промежуток времени.
                                              //если = 0 или > общего промежутка времени, указанного при старте программы,
                                              //то расчёт кол-ва взаимодействий и передач между сервисами происходит для всего интервала без разбития на единицы времени.

        public DateTime startingPoint;        //начальное время, которое будет вычитаться из времени прихода запроса для определения № временного интервала

        private int intervalNumber;           //номер временного интервала (увеличивается, зависит от переменной interval и времени поступления запроса)

        private Dictionary<string, Service> services = new Dictionary<string, Service>();     //словарь сервисов с транзакциями
        
        public Dictionary<string, Service> CreateGraph(string fileName, DateTime startInterval, DateTime endInterval)
        {
            string text;
            StreamReader read = new StreamReader(fileName);
            DateTime currentDate;
            startingPoint = startInterval;         //записываем стартовый интервал
            
            while ((text = read.ReadLine()) != null)
            {
                string[] part = text.Split(" ");
                string dateTime = part[0];
                string sourseService = part[1];
                string recipientService = part[2];
                currentDate = DateTime.Parse(dateTime);

                //вычисляем № временного интервала
                intervalNumber = GetIntervalNumber(currentDate);

                //если в графе нет сервиса отправителя или получателя, то создаём
                if (services.ContainsKey(sourseService) == false)
                {
                    CreateService(sourseService);
                }
                if(services.ContainsKey(recipientService) == false)
                {
                    CreateService(recipientService);
                }

                //увеличиваем число взаимосвязей для каждого из двух сервисов
                SetActivity(sourseService);
                SetActivity(recipientService);

                //связываем сервисы между собой и увеличиваем счётчик передач данных по ребру
                SetTransaction(sourseService, recipientService);
                //устанавливаем обратную свять, счётчик не трогаем
                SetReverseTransaction(sourseService, recipientService);
            }

            return services;
        }


        private int GetIntervalNumber(DateTime currentDate)
        //функция для определения номера временного интервала от стартового интервала до поступления очередного запроса
        {
            TimeSpan timeSpan;
            timeSpan = currentDate.Subtract(startingPoint);   //определение интервала
            int intervalNum = (int)timeSpan.TotalMilliseconds / (interval * 1000) + 1;     //расчёт номера интервала
            if (intervalNum < 0) intervalNum = 1;
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
            //если сервисы ещё не связаны между собой, то создаём связь и устанавливаем число взаимодействий между сервисами = 1
            if (services[sourseService].transactions.ContainsKey(recipientServive) == false)
            {
                Transaction newTransaction = new Transaction();
                newTransaction.relatedService = services[recipientServive];
                newTransaction.transfers.Add(intervalNumber, 1);
                services[sourseService].transactions.Add(recipientServive, newTransaction);
            }
            //если связь есть, то
            else
            {
                //проверяем, есть ли для текущего временного интвервала счётчик взаимодействий. если нет,
                //то создаём и устанавливаем 1, если есть, то увеличиваем на 1.
                if (services[sourseService].transactions[recipientServive].transfers.ContainsKey(intervalNumber) == false)
                {
                    services[sourseService].transactions[recipientServive].transfers.Add(intervalNumber, 1);
                }
                else
                {
                    services[sourseService].transactions[recipientServive].transfers[intervalNumber]++;
                }
            }
        }


        private void SetReverseTransaction(string sourseService, string recipientServive)
        //функция для установки обратной связи между источником и получаетелем
        {
            //создаём для сервиса recipientServive обратную связь на sourseService, если такова отсутсвует
            if (services[recipientServive].transactions.ContainsKey(sourseService) == false)
            {
                Transaction newTransaction = new Transaction();
                newTransaction.relatedService = services[sourseService];
                services[recipientServive].transactions.Add(sourseService, newTransaction);
            }
        }
    }
}