using System;
using System.Threading;

public class ThreadBase
{
    public static void Main()
    {

        System.Threading.WaitCallback waitCallback = new WaitCallback(MyThreadWork);

        //ThreadPool.QueueUserWorkItem(waitCallback, "第一个线程");
        //ThreadPool.QueueUserWorkItem(waitCallback, "第二个线程");
        //ThreadPool.QueueUserWorkItem(waitCallback, "第三个线程");
        //ThreadPool.QueueUserWorkItem(waitCallback, "第四个线程");
        for (int i = 0; i < 10; i++)
        {
            ThreadPool.QueueUserWorkItem((state) => {
                Console.WriteLine("线程现在开始启动…… {0}", (int)state);                
                Thread.Sleep((int)state * 1000);
                Console.WriteLine("运行结束…… {0}", (int)state);
            },i);
        }        
        Console.ReadLine();
    }

    public static void MyThreadWork(object state)
    {
        Console.WriteLine("线程现在开始启动…… {0}", (string)state);
        Thread.Sleep(10000);
        Console.WriteLine("运行结束…… {0}", (string)state);
    }
}