using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    public static void Main()

    {
        int[] nums = Enumerable.Range(0, 1000000).ToArray<int>();

        long total = 0;

        ParallelLoopResult result = Parallel.For<long>(0, nums.Length,
             () => { return 0; },
             (j, loop, subtotal) =>
             {
                 // 延长任务时间，更方便观察下面得出的结论

                 Thread.SpinWait(200);

                 Console.WriteLine("当前线程ID为：{0},j为{1}，subtotal为：{2}。"

                     , Thread.CurrentThread.ManagedThreadId, j.ToString(), subtotal.ToString());

                 if (j == 23)

                     loop.Break();

                 if (j > loop.LowestBreakIteration)
                 {
                     Thread.Sleep(4000);
                     Console.WriteLine("j为{0},等待4s种，用于判断已开启且大于阻断迭代是否会运行完。", j.ToString());
                 }
                 Console.WriteLine("j为{0},LowestBreakIteration为：{1}", j.ToString(), loop.LowestBreakIteration);
                 subtotal += nums[j];
                 return subtotal;
             },
             (finalResult) => Interlocked.Add(ref total, finalResult)
        );

        Console.WriteLine("total值为：{0}", total.ToString());
        if (result.IsCompleted)
        {
            Console.WriteLine("循环执行完毕");
        }
        else
        {
            Console.WriteLine("{0}", result.LowestBreakIteration.HasValue ? "调用了Break()阻断循环." : "调用了Stop()终止循环.");
        }

    }
}