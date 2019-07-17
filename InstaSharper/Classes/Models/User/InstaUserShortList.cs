using System.Collections.Generic;
using InstaSharper.Classes.Models.Other;

namespace InstaSharper.Classes.Models.User
{
    public class InstaUserShortList : List<InstaUserShort>, IInstaBaseList
    {
        public string NextMaxId { get; set; }
    }
}