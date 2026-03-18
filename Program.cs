using System;
using System.Numerics;
using System.Threading;
using System.Diagnostics;

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
        bool[] stopped = new bool[n];
        int stoppedCount = 0;

        Stopwatch stopwatch = Stopwatch.StartNew();

        while (stoppedCount < n)
        {
            for (int i = 0; i < n; i++)
            {
                if (!stopped[i] && stopwatch.ElapsedMilliseconds >= timeDelay[i])
                {
                    stopFlags[i] = true;
                    stopped[i] = true;
                    stoppedCount++;

                    lock (consoleLock)
                    {
                        Console.WriteLine($"Контролер зупинив потік {i + 1} (delay = {timeDelay[i]} мс)");
                    }
                }
            }

            Thread.Sleep(1);
        }
    }
    
}