using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbInserterApp.Models
{
    public class TitleBasics
    {
        public string Tconst { get; set; }
        public string TitleType { get; set; }
        public string PrimaryTitle { get; set; }
        public string OriginalTitle { get; set; }
        public bool IsAdult { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public int? RunTimeMinutes { get; set; }

        

        public TitleBasics()
        {
        }

        public TitleBasics(string[] movie)
        {
            Tconst = movie[0];
            TitleType = movie[1];
            PrimaryTitle = movie[2];
            OriginalTitle = movie[3];
            IsAdult = movie[4] == "1";
            StartYear = movie[5] == "\\N" ? null : Convert.ToInt32(movie[5]);
            EndYear = movie[6] == "\\N" ? null : Convert.ToInt32(movie[6]);
            RunTimeMinutes = movie[7] == "\\N" ? null : Convert.ToInt32(movie[7]);
        }
    }
}
