using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class StudentDTO
    {
        public string StudentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string RegistrationDate { get; set; }
    }
}