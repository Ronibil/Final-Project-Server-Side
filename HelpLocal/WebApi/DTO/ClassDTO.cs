using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class ClassDTO:IComparable
    {
        public int ClassCode { get; set; }
        public DateTime ClassDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string ClassName { get; set; }
        public string SuperStudentId { get; set; }
        public short NumOfParticipants { get; set; }
        public short NumOfRegistered { get; set; }
        public string ClassDescription { get; set; }
        public List<string> Tags { get; set; }
        public string SuperName { get; set; }
        public List<RankResultDTO> RankResults { get; set; }
        public int NumOfMatchs { get; set; }
        public short SuperStudentRank { get; set; }
        public short ClassRankAverage { get; set; }

        public int CompareTo(object obj)
        {
            ClassDTO c = (ClassDTO)obj;
            if (NumOfMatchs < c.NumOfMatchs)
            {
                return 1;
            }
            if (NumOfMatchs > c.NumOfMatchs)
            {
                return -1;
            }
            if (NumOfMatchs == c.NumOfMatchs && SuperStudentRank < c.SuperStudentRank)
            {
                return 1;
            }
            if (NumOfMatchs == c.NumOfMatchs && SuperStudentRank > c.SuperStudentRank)
            {
                return -1;
            }
            return 0;
        }
    }
}