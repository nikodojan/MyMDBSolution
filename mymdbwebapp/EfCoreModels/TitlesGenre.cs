﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace mymdbwebapp.EfCoreModels
{
    public partial class TitlesGenre
    {
        public string TitleTconst { get; set; }
        public int GenreId { get; set; }

        public virtual Genre Genre { get; set; }
        public virtual TitleBasic TitleTconstNavigation { get; set; }
    }
}