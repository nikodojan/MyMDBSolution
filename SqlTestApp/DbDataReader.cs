using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTestApp
{
    public class DbDataReader
    {
        private const string ConnectionString = "Data Source=NIKOSNOTEBOOK;Initial Catalog=MyMDB;User ID=mymdbinserter;Password=password;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public List<TitleBasicInfo> GetTitleBasics(string title)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            List<TitleBasicInfo> results = new List<TitleBasicInfo>();
            string query = "SELECT * FROM [dbo].[FindTitleWithWildcard] (@title)";

            connection.Open();

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@title", title);

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                TitleBasicInfo info = new TitleBasicInfo();
                info.Tconst = (string)reader[0];
                info.Type = (string)reader[1];
                info.PrimaryTitle = (string)reader[2];
                info.OriginalTitle = (string)reader[3];
                info.IsAdult = reader[4] == "1";
                info.StartYear = reader[5] == DBNull.Value ? null : (Int32)reader[5];
                info.EndYear = reader[6] == DBNull.Value ? null : (Int32)reader[6];
                info.Runtime = reader[7] == DBNull.Value ? null : (Int32)reader[7];
                info.Genres = reader[8] == DBNull.Value ? null : (string)reader[8];
                results.Add(info);
            }

            return results;
        }
    }

    public class TitleBasicInfo
    {
        public string Tconst { get; set; }
        public string Type { get; set; }
        public string PrimaryTitle { get; set; }
        public string OriginalTitle { get; set; }
        public bool IsAdult { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public int? Runtime { get; set; }
        public string? Genres { get; set; }
    }
}
