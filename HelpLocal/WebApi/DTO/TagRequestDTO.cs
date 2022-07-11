using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class TagRequestDTO
    {
        public int TagRequestNum { get; set; }
        public List<string> Tags { get; set; }
        public string RequestStatus { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public string RequestDate { get; set; }
    }
}