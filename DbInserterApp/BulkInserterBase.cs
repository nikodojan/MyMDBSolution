using System.Diagnostics;

namespace DbInserterApp;

public abstract class BulkInserterBase
{
    protected SqlConnection _connection;
    protected List<string[]> _data;

    protected string _filePath = String.Empty;

    protected int _linesRead = 0;
    protected bool _firstRead = true;

    public BulkInserterBase()
    {
        
    }

    public BulkInserterBase(SqlConnection con, string filePath)
    {
        _connection = con;
        _filePath = filePath;
        _data = new List<string[]>();
    }

    public virtual void BeginInserting(int batchSize)
    {
        _data = ReadPartialFromFile(_filePath);

        while (_data.Any())
        {
            BulkInsertData(batchSize);

            _data = ReadPartialFromFile(_filePath);
        }
    }

    protected virtual List<string[]> ReadPartialFromFile(string filePath)
    {
        IEnumerable<string[]> fromFile = new List<string[]>();
        List<string[]> partialList = new();
        if (_firstRead)
        {
            fromFile = File.ReadLines(filePath).Skip(1).Take(9_999_999).Select(line => line.Split("\t"));
            _firstRead = false;
        }
        else
        {
            fromFile = File.ReadLines(filePath).Skip(_linesRead).Take(10_000_000).Select(line => line.Split("\t"));
        }

        _linesRead += 10_000_000;
        return fromFile.ToList();
    }

    protected virtual List<string[]> ReadDataFromFile(string filePath)
    {
        Console.WriteLine("Start reading file");
        var data = File.ReadLines(filePath).Skip(1).Select(line => line.Split("\t"));
        Console.WriteLine("File read to end");
        return data.ToList();
    }

    protected virtual void BulkInsertData(int batchSize)
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

    protected abstract void InsertDataIntoDatabase(List<string[]> dataChunk);

    ///// <summary>
    ///// Enables identity insert for the specified table (SET IDENTITY_INSERT ON)
    ///// </summary>
    ///// <param name="tableName">Name of the database table</param>
    //public void EnableIdentityInsert(string tableName)
    //{
    //    string commandText = $"SET IDENTITY_INSERT {tableName} ON;";
    //    using SqlCommand command = new SqlCommand(commandText, _connection);
    //    command.ExecuteNonQuery();
    //}

    ///// <summary>
    ///// Enables identity insert for the specified table (SET IDENTITY_INSERT OFF)
    ///// </summary>
    ///// <param name="tableName">Name of the database table</param>
    //public void DisableIdentityInsert(string tableName)
    //{
    //    string commandText = $"SET IDENTITY_INSERT [dbo].[{tableName}] OFF;";
    //    using SqlCommand command = new SqlCommand(commandText, _connection);
    //    command.ExecuteNonQuery();
    //}

}