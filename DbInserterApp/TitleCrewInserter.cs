namespace DbInserterApp
{
    public class TitleCrewInserter : BulkInserterBase
    {
        public TitleCrewInserter(SqlConnection con, string filePath) : base(con, filePath)
        {
            
        }

        public override void InsertDataIntoDatabase(List<string[]> dataChunk)
        {
            DataTable directorsTable = new DataTable("TitlesDirectors");
            directorsTable.Columns.Add("Tconst", typeof(string));
            directorsTable.Columns.Add("Nconst", typeof(string));

            DataTable writersTable = new DataTable("TitlesWriters");
            writersTable.Columns.Add("Tconst", typeof(string));
            writersTable.Columns.Add("Nconst", typeof(string));

            foreach (var crew in dataChunk)
            {
                if (crew[1] != "\\N")
                {
                    foreach (var dir in crew[1].Split(','))
                    {
                        DataRow dirRow = directorsTable.NewRow();
                        dirRow["Tconst"] = crew[0];
                        dirRow["Nconst"] = dir;
                        directorsTable.Rows.Add(dirRow);
                    }
                }

                if (crew[2] != "\\N")
                {
                    foreach (var wr in crew[2].Split(','))
                    {
                        DataRow wrRow = writersTable.NewRow();
                        wrRow["Tconst"] = crew[0];
                        wrRow["Nconst"] = wr;
                        writersTable.Rows.Add(wrRow);
                    }
                }
            }

            SqlBulkCopy directorsCopy = new SqlBulkCopy(_connection, SqlBulkCopyOptions.KeepNulls, null);
            directorsCopy.DestinationTableName = "TitlesDirectors";
            directorsCopy.WriteToServer(directorsTable);

            SqlBulkCopy writersCopy = new SqlBulkCopy(_connection, SqlBulkCopyOptions.KeepNulls, null);
            writersCopy.DestinationTableName = "TitlesWriters";
            writersCopy.WriteToServer(writersTable);
        }
    }
}
