using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbInserterApp
{
    public class TitleAliasInserter : BulkInserterBase
    {
        const char STX = '\u0002';

        private static int _aliasID = 0;
        private Dictionary<string, int> _aliasTypes;

        public TitleAliasInserter(SqlConnection con, string filePath) : base(con, filePath)
        {
            _connection = con;
            _filePath = filePath;
            _data = new List<string[]>();
            _aliasTypes = GetTypesFromDb(_connection);
        }

        protected override List<string[]> ReadPartialFromFile(string filePath)
        {
            IEnumerable<string[]> fromFile = new List<string[]>();
            List<string[]> partialList = new();
            if (_firstRead)
            {
                fromFile = File.ReadLines(filePath, Encoding.UTF8).Skip(1).Take(9_999_999).Select(line => line.Split("\t"));
                _firstRead = false;
            }
            else
            {
                fromFile = File.ReadLines(filePath, Encoding.UTF8).Skip(_linesRead).Take(10_000_000).Select(line => line.Split("\t"));
            }

            _linesRead += 10_000_000;
            return fromFile.ToList();
        }

        protected override void InsertDataIntoDatabase(List<string[]> dataChunk)
        {
            //TitleAliases (Aliasd, Tconst, Title, Region, Language, IsOriginalTitle)
            DataTable aliasTable = new DataTable("TitleAliases");
            aliasTable.Columns.Add("AliasId", typeof(int));
            aliasTable.Columns.Add("Tconst", typeof(string));
            aliasTable.Columns.Add("Title", typeof(string));
            aliasTable.Columns.Add("Region", typeof(string));
            aliasTable.Columns.Add("Language", typeof(string));
            aliasTable.Columns.Add("IsOriginalTitle", typeof(bool));
            aliasTable.Columns.Add("Attributes", typeof(string));
            
            DataTable titleAliasTable = new DataTable("TitleAliasesTypes");
            titleAliasTable.Columns.Add("AliasId", typeof(int));
            titleAliasTable.Columns.Add("AliasTypeId");
            
            foreach (var line in dataChunk)
            {
                int aliasID = ++_aliasID;
                DataRow aliasRow = aliasTable.NewRow();
                aliasRow["AliasId"] = aliasID;
                aliasRow["Tconst"] = line[0];
                aliasRow["Title"] = line[2];
                aliasRow["Region"] = line[3] == "\\N" ? DBNull.Value : line[3];
                aliasRow["Language"] = line[4] == "\\N" ? DBNull.Value : line[4];
                aliasRow["IsOriginalTitle"] = line[7] == "1";
                aliasRow["Attributes"] = line[6] == "\\N" ? DBNull.Value : line[6];
                aliasTable.Rows.Add(aliasRow);

                if (line[5] != "\\N")
                {
                    string type = line[5];
                    if (type.Contains(STX))
                    {
                        type = type.Replace(STX, ',');
                    }
                    foreach (var t in type.Split(","))
                    {
                        DataRow typeRow = titleAliasTable.NewRow();
                        typeRow["AliasId"] = aliasID;
                        typeRow["AliasTypeId"] = _aliasTypes[t];
                        titleAliasTable.Rows.Add(typeRow);
                    }
                }
            }

            SqlBulkCopy aliasCopy = new SqlBulkCopy(_connection,
                SqlBulkCopyOptions.KeepNulls & SqlBulkCopyOptions.KeepIdentity, null);
            aliasCopy.DestinationTableName = "TitleAliases";
            aliasCopy.WriteToServer(aliasTable);

            SqlBulkCopy typeCopy = new SqlBulkCopy(_connection,
                SqlBulkCopyOptions.KeepNulls & SqlBulkCopyOptions.KeepIdentity, null);
            typeCopy.DestinationTableName ="TitleAliasesTypes";
            typeCopy.WriteToServer(titleAliasTable);
        }

        private Dictionary<string, int> GetTypesFromDb(SqlConnection connection)
        {
            Dictionary<string, int> types = new Dictionary<string, int>();
            string query = "SELECT * FROM AliasTypes;";
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    types.Add(Convert.ToString(reader[1]), Convert.ToInt32(reader[0]));
                }
            }
            connection.Close();
            return types;
        }
    }
}
