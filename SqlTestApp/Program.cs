// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using SqlTestApp;

Console.WriteLine("Hello, World!");

DbDataReader dbReader = new DbDataReader();
Stopwatch watch = new Stopwatch();
watch.Start();
var results = dbReader.GetTitleBasics("Batman");
watch.Stop();

foreach (var t in results)
{
    Console.WriteLine($"{t.PrimaryTitle} ({t.StartYear}), {t.Genres}");
}

Console.WriteLine(watch.Elapsed.TotalSeconds);

Console.WriteLine("done");
Console.ReadKey();
