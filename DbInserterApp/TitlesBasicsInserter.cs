global using System.Data;
using System.Diagnostics;

namespace DbInserterApp
{
    public class TitlesBasicsInserter : BulkInserterBase
    {
        private SqlConnection _connection;
        private Dictionary<string, int> _types;
        private Dictionary<string, int> _genres;
        private List<string[]> _data;

        
        public TitlesBasicsInserter(SqlConnection con, string filePath) : base (con, filePath)
        {
            InitializeData();
        }

        private void InitializeData()
        {
            Console.WriteLine("Initializing data...");
            _types = GetTypesFromDb(_connection);
            _genres = GetGenresFromDb(_connection);
            
        }

        public void BulkInsertData()
        {
            int count = 0;
            int batchSize = 50_000;

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
            Console.WriteLine("Preparing data...");
            DataTable titlesTable = new DataTable("TitleBasics");
            titlesTable.Columns.Add("Tconst", typeof(string));
            titlesTable.Columns.Add("TitleType", typeof(string));
            titlesTable.Columns.Add("PrimaryTitle", typeof(string));
            titlesTable.Columns.Add("OriginalTitle", typeof(string));
            titlesTable.Columns.Add("IsAdult", typeof(bool));
            titlesTable.Columns.Add("StartYear", typeof(int));
            titlesTable.Columns.Add("EndYear", typeof(int));
            titlesTable.Columns.Add("Runtime", typeof(int));

            DataTable titlesGenresTable = new DataTable("TitlesGenres");
            titlesGenresTable.Columns.Add("TitleTconst", typeof(string));
            titlesGenresTable.Columns.Add("GenreId", typeof(int));

            foreach (var title in dataChunk)
            {
                DataRow titleRow = titlesTable.NewRow();
                titleRow["Tconst"] = title[0];
                titleRow["TitleType"] = _types[title[1]];
                titleRow["PrimaryTitle"] = title[2];
                titleRow["OriginalTitle"] = title[3];
                titleRow["IsAdult"] = title[4] == "1";
                titleRow["StartYear"] = title[5] == "\\N" ? DBNull.Value : Convert.ToInt32(title[5]);
                titleRow["EndYear"] = title[6] == "\\N" ? DBNull.Value : Convert.ToInt32(title[6]);
                titleRow["Runtime"] = title[7] == "\\N" ? DBNull.Value : Convert.ToInt32(title[7]);
                titlesTable.Rows.Add(titleRow);

                if (title[8] != "\\N")
                {
                    foreach (var genre in title[8].Split(','))
                    {
                        DataRow genreRow = titlesGenresTable.NewRow();
                        genreRow["TitleTconst"] = title[0];
                        genreRow["GenreId"] = _genres[genre];
                        titlesGenresTable.Rows.Add(genreRow);
                    }
                }
            }

            Console.WriteLine("Start copying...");

            SqlBulkCopy titlesCopy = new SqlBulkCopy(_connection, SqlBulkCopyOptions.KeepNulls, null);
            titlesCopy.DestinationTableName = "TitleBasics";
            titlesCopy.WriteToServer(titlesTable);

            SqlBulkCopy genresCopy = new SqlBulkCopy(_connection);
            genresCopy.DestinationTableName = "TitlesGenres";
            genresCopy.WriteToServer(titlesGenresTable);

            
        }

        public List<string[]> ReadDataFromFile(string filePath)
        {
            var data = File.ReadLines(filePath).Skip(1).Select(line => line.Split("\t"));
            return data.ToList();
        }

        private Dictionary<string, int> GetTypesFromDb(SqlConnection connection)
        {
            Dictionary<string, int> types = new Dictionary<string, int>();
            string query = "SELECT * FROM TitleTypes;";
            SqlCommand command = new SqlCommand(query, connection);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    types.Add(Convert.ToString(reader[1]), Convert.ToInt32(reader[0]));
                }
            }
            return types;
        }

        private Dictionary<string, int> GetGenresFromDb(SqlConnection connection)
        {
            Dictionary<string, int> genres = new Dictionary<string, int>();
            string query = "SELECT * FROM Genres;";
            SqlCommand command = new SqlCommand(query, connection);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    genres.Add(Convert.ToString(reader[1]), Convert.ToInt32(reader[0]));
                }
            }
            return genres;
        }
    }
}
