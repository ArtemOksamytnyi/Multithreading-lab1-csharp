using System;
using System.Numerics;
using System.Threading;

class Program
{
    static bool[] stopFlags;
    static Thread[] workers;
    static int n = 5;
    static int[] timeDelay;
    static readonly object consoleLock = new object();

    static void Main()
    {
        stopFlags = new bool[n];
        workers = new Thread[n];
        timeDelay = new int[n];

        for (int i = 0; i < n; i++)
        {
            Console.Write($"Введіть час для виконання {i + 1} потоку (мс): ");
            timeDelay[i] = int.Parse(Console.ReadLine());
        }

        for (int i = 0; i < n; i++)
        {
            int id = i;
            workers[i] = new Thread(() => Work(id));
            workers[i].Start();
        }

        Thread controller = new Thread(Control);
        controller.Start();

        for (int i = 0; i < n; i++)
        {
            workers[i].Join();
        }

        lock (consoleLock)
        {
            Console.WriteLine("Усі потоки завершили роботу.");
        }
    }

    static void Work(int id)
    {
        BigInteger sum = 0;
        BigInteger current = 0;
        long count = 0;
        BigInteger step = id + 1;

        do
        {
            sum += current;
            current += step;
            count++;
        }
        while (!stopFlags[id]);

        lock (consoleLock)
        {
            Console.WriteLine($"Потік {id + 1}: сума = {sum}, кількість елементів = {count}");
        }
    }

    static void Control()
    {
        for (int i = 0; i < n; i++)
        {
            int id = i;
            new Thread(() =>
            {
                int delay = timeDelay[id];
                Thread.Sleep(delay);
                stopFlags[id] = true;
                lock (consoleLock)
                {
                    Console.WriteLine($"Контролер зупинив потік {id + 1} (delay={delay}ms)");
                }
            }).Start();
        }
    }
}