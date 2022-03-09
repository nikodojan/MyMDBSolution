using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbInserterApp
{
    public class TitlePrincipalsInserter : BulkInserterBase
    {
        private int _id = 0;

        public override void InsertDataIntoDatabase(List<string[]> dataChunk)
        {
            DataTable principalsTable = new DataTable("TitlePrincipals");
            principalsTable.Columns.Add("PrincipalId", typeof(int));
            principalsTable.Columns.Add("Tconst", typeof(string));
            principalsTable.Columns.Add("Nconst", typeof(string));
            principalsTable.Columns.Add("Category", typeof(string));
            principalsTable.Columns.Add("Job", typeof(string));

            DataTable charactersTable = new DataTable("PrincipalsCharacters");
            charactersTable.Columns.Add("PrincipalId", typeof(int));
            charactersTable.Columns.Add("Character", typeof(string));

            foreach (var col in dataChunk)
            {
                int pID = ++_id;
                DataRow pRow = principalsTable.NewRow();
                pRow["PrincipalId"] = pID;
                pRow["Tconst"] = col[0];
                pRow["Nconst"] = col[2];
                pRow["Category"] = col[3];
                pRow["Job"] = col[4];

                if (col[5] != "\\N")
                {
                    foreach (var c in col[5].Split(','))
                    {
                        DataRow charRow = charactersTable.NewRow();
                        charRow["PrincipalId"] = pID;
                        charRow["Character"] = c;
                    }
                }
            }

            // identity insert on

            // bulk insert

            // identity insert off

        }

        private void EnableIdentityInsert()
        {
            // implement
        }

        private void DisableIdentityInsert()
        {
            // implement
        }

    }
}
