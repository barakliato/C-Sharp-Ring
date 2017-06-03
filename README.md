# ConcurrentRing <br />
C# thread safe implementation for ring as a replacement for a consumer producer queue <br />
<br />
The class was built to support a connection buffer where one thread write from the connection and the other thread read from it. <br />
The implementation provide an alternative to a consumer producer queue where it required a contiguous block of information without locks. <br /><br />
an exmaple of use:<br />
<br />
```C#
ConcurrentRing<int> ring = new ConcurrentRing<int>(500);<br />
Action a1 = () =><br />
{<br />
    while (true)<br />
    {<br />
        ring.Write(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });<br />
        Task.Delay(25).Wait();<br />
    }<br />
};<br />
<br />
Action a2 = () =><br />
{<br />
    while (true)<br />
    {<br />
        var items = ring.Read(10);<br />
        foreach (var item in items)<br />
        Console.Write("{0} ", item);<br />
<br />
        Console.WriteLine();<br />
        Task.Delay(10).Wait();<br />
    }<br />
};<br />
<br />
var t1 = Task.Run(a1);<br />
var t2 = Task.Run(a2);<br />
Task.WaitAll(t1, t2);<br />
```
