using System.Diagnostics;

namespace DbInserterApp;

public abstract class BulkInserterBase
{
    protected SqlConnection _connection;
    protected List<string[]> _data;

    public BulkInserterBase()
    {
        
    }

    public BulkInserterBase(SqlConnection con, string filePath)
    {
        _connection = con;
        _data = ReadDataFromFile(filePath);
    }

    public virtual List<string[]> ReadDataFromFile(string filePath)
    {
        var data = File.ReadLines(filePath).Skip(1).Select(line => line.Split("\t"));
        return data.ToList();
    }

    public virtual void BulkInsertData(int batchSize)
    {
        Console.WriteLine($"Total rows in data file: {_data.Count}");
        int count = 0;
        _connection.Open();
        Stopwatch watch = new Stopwatch();
        watch.Start();
        while (count < _data.Count)
        {
            var batch = _data.Skip(count).Take(batchSize).ToList();
            InsertDataIntoDatabase(batch);
            count += batchSize;
            Console.Write($"\rRows inserted: {count} \t\t\t{watch.Elapsed.TotalSeconds} seconds");

        }
        watch.Stop();
        Console.WriteLine($"Total time: {watch.Elapsed.TotalSeconds} seconds");
        _connection.Close();
    }

    public abstract void InsertDataIntoDatabase(List<string[]> dataChunk);

}