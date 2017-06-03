# ConcurrentRing <br />
C# thread safe implementation for ring as a replacement for a consumer producer queue <br />
<br />
The class was built to support a connection buffer where one thread write from the connection and the other thread read from it. <br />
The implementation provide an alternative to a consumer producer queue where it required a contiguous block of information without locks. <br /><br />
an exmaple of use:<br />
<br />
ConcurrentRing<int> ring = new ConcurrentRing<int>(500);<br />
Action a1 = () =><br />
{<br />
____while (true)<br />
    {<br />
_______ring.Write(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });<br />
_______Task.Delay(25).Wait();<br />
____}<br />
};<br />
<br />
Action a2 = () =><br />
{<br />
____while (true)<br />
____{<br />
________var items = ring.Read(10);<br />
________foreach (var item in items)<br />
________Console.Write("{0} ", item);<br />
<br />
________Console.WriteLine();<br />
________Task.Delay(10).Wait();<br />
____}<br />
};<br />
<br />
var t1 = Task.Run(a1);<br />
var t2 = Task.Run(a2);<br />
Task.WaitAll(t1, t2);<br />
