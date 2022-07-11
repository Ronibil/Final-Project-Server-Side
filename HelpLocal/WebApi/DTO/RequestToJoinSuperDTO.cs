using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class RequestToJoinSuperDTO
    {
        public int RequsetNum { get; set; }
        public string DepartmentName { get; set; }
        public string StudyYear { get; set; }
        public string Description { get; set; }
    }
}