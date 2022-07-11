using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class SuperLandingPageDetailsDTO
    {
        public string StudentId { get; set; }
        public string FullName { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string DepartmentName { get; set; }
        public string StudyYear { get; set; }
        public int NumOfRanks { get; set; }
        public int RankAverage { get; set; }
        public int NumOfClass { get; set; }
        public string ImagePath { get; set; }
        public List<ClassDTO> FutreClasses { get; set; }
        public List<ClassDTO> ClassesHistory { get; set; }
    }
}