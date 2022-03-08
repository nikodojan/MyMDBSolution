using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbInserterApp
{
    public class Inserter
    {
        SqlConnection sqlCon = new SqlConnection("server=localhost;database=MyMDB;user id=mymdbuser;password=password");

        public List<string[]> ReadDataFromFile(string filePath)
        {
            var data = File.ReadLines(filePath).Skip(1).Select(line=>line.Split("\t"));
            return data.ToList();
        }



    }
}
