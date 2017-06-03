# ConcurrentRing <br />
C# implementation for ring <br />
<br />
I was initially built it to support a connection buffer that recieve information in high rate. <br />
It was built with porpuse to provide an alternative to a consumer producer queue in order to have a fixed size buffer while always
<br />
reading the last information discarding information too old. <br />
an exmaple of use:<br />
<br />
```C#
ConcurrentRing<int> ring = new ConcurrentRing<int>(500);
Action a1 = () =>
{
    while (true)
    {
        ring.Write(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        Task.Delay(25).Wait();
    }
};

Action a2 = () =>
{
    while (true)
    {
        var items = ring.Read(10);
        foreach (var item in items)
            Console.Write("{0} ", item);

        Console.WriteLine();
        Task.Delay(10).Wait();
    }
};

var t1 = Task.Run(a1);
var t2 = Task.Run(a2);
Task.WaitAll(t1, t2);

/*
 * ----------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" (Revision 42):
 * Barak Liato wrote this file. As long as you retain this notice you
 * can do whatever you want with this stuff. If we meet some day, and you think
 * this stuff is worth it, you can buy me a beer in return.      Barak Liato
 * ----------------------------------------------------------------------------
 */
```
