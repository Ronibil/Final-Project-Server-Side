using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class HotTagDTO : IComparable
    {
        public string TagName { get; set; }
        public int NumOfRequirements { get; set; }
        public int NumOfUsesInClasses { get; set; }






        public int CompareTo(object obj)
        {
            HotTagDTO t = (HotTagDTO)obj;
            if (NumOfRequirements < t.NumOfRequirements)
            {
                return 1;
            }
            if (NumOfRequirements > t.NumOfRequirements)
            {
                return -1;
            }
            if (NumOfRequirements == t.NumOfRequirements && NumOfUsesInClasses < t.NumOfUsesInClasses)
            {
                return 1;
            }
            if (NumOfRequirements == t.NumOfRequirements && NumOfUsesInClasses > t.NumOfUsesInClasses)
            {
                return -1;
            }
            return 0;
        }
    }
}