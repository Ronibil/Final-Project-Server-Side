using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class UpdatePasswordDTO
    {
        public string StudentId { get; set; }
        public string Password { get; set; }
    }
}