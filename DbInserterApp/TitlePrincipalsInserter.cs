﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbInserterApp
{
    public class TitlePrincipalsInserter : BulkInserterBase
    {
        private static int _pID = 0;
        private static int _cID = 0;
        
        private string _filePath = String.Empty;

        private int _linesRead = 0;
        private bool _firstRead = true;

        public TitlePrincipalsInserter(SqlConnection con, string filePath)
        {
            _connection = con;
            _filePath = filePath;
            _data = new List<string[]>();
        }

        public void BeginInserting(int batchSize)
        {
            _data = ReadPartialFromFile(_filePath);

            while (_data.Any())
            {
                BulkInsertData(batchSize);

                _data = ReadPartialFromFile(_filePath);
            }
        }

        public List<string[]> ReadPartialFromFile(string filePath)
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

        protected override void BulkInsertData(int batchSize)
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

        protected override void InsertDataIntoDatabase(List<string[]> dataChunk)
        {
            Console.WriteLine("Creating tables");
            DataTable principalsTable = new DataTable("TitlePrincipals");
            principalsTable.Columns.Add("PrincipalId", typeof(int));
            principalsTable.Columns.Add("Tconst", typeof(string));
            principalsTable.Columns.Add("Nconst", typeof(string));
            principalsTable.Columns.Add("Category", typeof(string));
            principalsTable.Columns.Add("Job", typeof(string));

            DataTable charactersTable = new DataTable("PrincipalsCharacters");
            charactersTable.Columns.Add("CharId", typeof(int));
            charactersTable.Columns.Add("PrincipalId", typeof(int));
            charactersTable.Columns.Add("Character", typeof(string));

            foreach (var col in dataChunk)
            {
                int pID = ++_pID;
                DataRow pRow = principalsTable.NewRow();
                pRow["PrincipalId"] = pID;
                pRow["Tconst"] = col[0];
                pRow["Nconst"] = col[2];
                pRow["Category"] = col[3];
                pRow["Job"] = col[4];
                principalsTable.Rows.Add(pRow);

                if (col[5] != "\\N")
                {
                    string[] values = col[5].Split('"');

                    foreach (var c in values)
                    {
                        string trimmed = c.Trim();
                        if (string.IsNullOrWhiteSpace(trimmed)) continue;
                        var chars = trimmed.ToCharArray();
                        if (!char.IsLetter(chars[0])) continue;
                        int cID = ++_cID; 
                        DataRow charRow = charactersTable.NewRow();
                        charRow["CharId"] = cID;
                        charRow["PrincipalId"] = pID;
                        charRow["Character"] = trimmed;
                        charactersTable.Rows.Add(charRow);
                    }
                }
            }

            Console.WriteLine("Tables created");
            Console.WriteLine("Start copy");

            SqlBulkCopy princCopy = new SqlBulkCopy(_connection, 
                SqlBulkCopyOptions.KeepNulls & SqlBulkCopyOptions.KeepIdentity, null);
            princCopy.DestinationTableName = "TitlePrincipals";
            princCopy.WriteToServer(principalsTable);

            SqlBulkCopy charCopy = new SqlBulkCopy(_connection,
                SqlBulkCopyOptions.KeepNulls & SqlBulkCopyOptions.KeepIdentity, null);
            charCopy.DestinationTableName = "PrincipalsCharacters";
            charCopy.WriteToServer(charactersTable);
        }
    }
}
