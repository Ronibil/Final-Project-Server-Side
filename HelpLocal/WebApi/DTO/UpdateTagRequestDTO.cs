using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class UpdateTagRequestDTO
    {
        public string TagName { get; set; }
        public string RequestStatus { get; set; }
    }
}