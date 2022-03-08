namespace DbInserterApp
{
    public class NameBasicsInserter : BulkInserterBase
    {
        public NameBasicsInserter(SqlConnection con, string filePath) : base (con, filePath)
        {

        }

        public override void InsertDataIntoDatabase(List<string[]> dataChunk)
        {
            DataTable namesTable = new DataTable("NameBasics");
            namesTable.Columns.Add("Nconst", typeof(string));
            namesTable.Columns.Add("PrimaryName", typeof(string));
            namesTable.Columns.Add("BirthYear", typeof(int));
            namesTable.Columns.Add("DeathYear", typeof(int));

            DataTable nameProfessions = new DataTable("NameProfessions");
            nameProfessions.Columns.Add("Nconst", typeof(string));
            nameProfessions.Columns.Add("Profession", typeof(string));

            DataTable knownFor = new DataTable("NamesKnownForTitles");
            knownFor.Columns.Add("Tconst", typeof(string));
            knownFor.Columns.Add("Nconst", typeof(string));

            foreach (var name in dataChunk)
            {
                DataRow nameRow = namesTable.NewRow();
                nameRow["Nconst"] = name[0];
                nameRow["PrimaryName"] = name[1];
                nameRow["BirthYear"] = name[2] == "\\N" ? DBNull.Value : Convert.ToInt32(name[2]);
                nameRow["DeathYear"] = name[3] == "\\N" ? DBNull.Value : Convert.ToInt32(name[3]);
                namesTable.Rows.Add(nameRow);

                if (name[4] != "\\N")
                {
                    foreach (var prof in name[4].Split(','))
                    {
                        DataRow profRow = nameProfessions.NewRow();
                        profRow["Nconst"] = name[0];
                        profRow["Profession"] = prof;
                        nameProfessions.Rows.Add(profRow);
                    }
                }

                if (name[5] != "\\N")
                {
                    foreach (var movie in name[5].Split(','))
                    {
                        DataRow movieRow = knownFor.NewRow();
                        movieRow["Tconst"] = movie;
                        movieRow["Nconst"] = name[0];
                        knownFor.Rows.Add(movieRow);
                    }
                }

            }
            
            SqlBulkCopy namesCopy = new SqlBulkCopy(_connection, SqlBulkCopyOptions.KeepNulls, null);
            namesCopy.DestinationTableName = "NameBasics";
            namesCopy.WriteToServer(namesTable);

            SqlBulkCopy profCopy = new SqlBulkCopy(_connection);
            profCopy.DestinationTableName = "NamesProfessions";
            profCopy.WriteToServer(nameProfessions);

            SqlBulkCopy moviesCopy = new SqlBulkCopy(_connection);
            moviesCopy.DestinationTableName = "NamesKnownForTitles";
            moviesCopy.WriteToServer(knownFor);
        }
    }
}
