using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class StudentLandingPageDetailsDTO
    {
        public string StudentId { get; set; }
        public string FullName { get; set; }
        public List<ClassDTO> FutreClasses { get; set; }
        public List<ClassDTO> ClassesHistory { get; set; }
    }
}