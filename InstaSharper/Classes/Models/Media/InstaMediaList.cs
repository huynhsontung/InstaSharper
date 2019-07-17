using System.Collections.Generic;
using InstaSharper.Classes.Models.Other;

namespace InstaSharper.Classes.Models.Media
{
    public class InstaMediaList : List<InstaMedia>, IInstaBaseList
    {
        public int Pages { get; set; } = 0;
        public int PageSize { get; set; } = 0;
        public string NextMaxId { get; set; }
    }
}