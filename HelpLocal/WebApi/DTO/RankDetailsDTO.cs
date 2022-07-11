using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class RankDetailsDTO
    {
        public string StudentId { get; set; }       
        public int ClassCode { get; set; }
        public short RankValue { get; set; }
        public string RankDescription { get; set; }
    }
}