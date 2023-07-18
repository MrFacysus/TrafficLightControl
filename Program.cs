using System;
using System.Collections.Generic;

namespace TrafficLightControl
{
    internal class Program
    {
        class Car
        {
            public int ArivalTime { get; set; }
            public int DepartureTime { get; set; }
            public char Direction { get; set; }

            public Car(int arivalTime, char direction)
            {
                ArivalTime = arivalTime;
                Direction = direction;
            }
        }
        class TrafficLight
        {
            public char Color { get; set; }
            public char Direction { get; set; }
            public List<Car> Cars { get; set; }
            public int CarsPassed { get; set; }
            public int LargestQueue { get; set; }
            public List<Car> CarMax { get; set; }

            public TrafficLight(char direction)
            {
                Color = 'R';
                Direction = direction;
                Cars = new List<Car>();
                CarsPassed = 0;
                LargestQueue = 0;
                CarMax = new List<Car>();
            }

            public float CalculateAvgWaitTime()
            {
                float avgWaitTime = 0;
                foreach (var car in CarMax)
                {
                    if (car.DepartureTime != 0)
                    {
                        avgWaitTime += (float)car.DepartureTime - (float)car.ArivalTime;
                    }
                }
                return avgWaitTime / CarMax.Count;
            }
        }
        static void Main(string[] args)
        {
            string[] lines = System.IO.File.ReadAllLines(@"HW4_Data.txt");
            List<char> Directions = new List<char>() { 'W', 'E', 'N', 'S' };
            List<TrafficLight> InterceptionLights = new List<TrafficLight>();
            for (int i = 0; i < 4; i++)
            {
                InterceptionLights.Add(new TrafficLight(Directions[i]));
            }
            List<Car> cars = new List<Car>();
            foreach (string line in lines)
            {
                int arivalTime = int.Parse(line.Substring(1));
                char direction = line[0];
                Car car = new Car(arivalTime, direction);
                cars.Add(car);
            }
            int time = 1;
            int maxTime = 240;
            while (time < maxTime)
            {
                foreach (Car car in cars)
                {
                    if (car.DepartureTime == time)
                    {
                        Console.WriteLine("Car {0} arrived at {1} and left at {2} after waiting {3} seconds", cars.IndexOf(car), car.ArivalTime, car.DepartureTime, car.DepartureTime - car.ArivalTime);
                    }
                }
                for (int i = 0; i < cars.Count; i++)
                {
                    if (cars[i].ArivalTime == time)
                    {
                        for (int j = 0; j < InterceptionLights.Count; j++)
                        {
                            if (InterceptionLights[j].Direction == cars[i].Direction)
                            {
                                InterceptionLights[j].Cars.Add(cars[i]);
                                InterceptionLights[j].CarMax.Add(cars[i]);
                                break;
                            }
                        }
                    }
                }
                for (int i = 0; i < InterceptionLights.Count; i++)
                {
                    int modul = time % 8;
                    if (modul < 3)
                    {
                        if (InterceptionLights[i].Direction == 'S' || InterceptionLights[i].Direction == 'W')
                        {
                            InterceptionLights[i].Color = 'R';
                            Console.WriteLine("Light {0} turned red at {1}", InterceptionLights[i].Direction, time);
                        }
                        else
                        {
                            InterceptionLights[i].Color = 'G';
                            Console.WriteLine("Light {0} turned green at {1}", InterceptionLights[i].Direction, time);

                        }
                    }
                    else if (modul >= 3 && modul < 6)
                    {
                        InterceptionLights[i].Color = 'Y';
                        Console.WriteLine("Light {0} turned yellow at {1}", InterceptionLights[i].Direction, time);
                    }
                    else
                    {
                        if (InterceptionLights[i].Direction == 'S' || InterceptionLights[i].Direction == 'W')
                        {
                            InterceptionLights[i].Color = 'G';
                            Console.WriteLine("Light {0} turned green at {1}", InterceptionLights[i].Direction, time);
                        }
                        else
                        {
                            InterceptionLights[i].Color = 'R';
                            Console.WriteLine("Light {0} turned red at {1}", InterceptionLights[i].Direction, time);
                        }
                    }
                }
                for (int i = 0; i < InterceptionLights.Count; i++)
                {
                    if (InterceptionLights[i].Cars.Count > 0)
                    {
                        if (InterceptionLights[i].Color == 'G')
                        {
                            InterceptionLights[i].Cars[0].DepartureTime = time + 1;
                            InterceptionLights[i].Cars.RemoveAt(0);
                            InterceptionLights[i].CarsPassed++;
                        }
                    }
                    if (InterceptionLights[i].Cars.Count > InterceptionLights[i].LargestQueue)
                    {
                        InterceptionLights[i].LargestQueue = InterceptionLights[i].Cars.Count;
                    }
                }
                time++;
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("\nTime: {0}\n", time);
            }
            for (int i = 0; i < InterceptionLights.Count; i++)
            {
                Console.WriteLine("The {0} light passed {1} cars", InterceptionLights[i].Direction, InterceptionLights[i].CarsPassed);
                Console.WriteLine("The {0} light had the largest queue of {1} cars", InterceptionLights[i].Direction, InterceptionLights[i].LargestQueue);
                Console.WriteLine("The {0} light had an average wait time of {1} seconds", InterceptionLights[i].Direction, InterceptionLights[i].CalculateAvgWaitTime());
            }
            Console.ReadLine();
        }
    }
}
