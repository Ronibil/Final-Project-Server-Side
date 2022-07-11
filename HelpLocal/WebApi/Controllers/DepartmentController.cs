using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DATA;
using WebApi.DTO;
using System.Web.Http.Cors;

namespace WebApi.Controllers
{
    [RoutePrefix("department")]
    public class DepartmentController : ApiController
    {
        [HttpGet]
        [Route("getAll")]
        public IHttpActionResult GetAll()
        {
            try
            {
                AppDbContext db = new AppDbContext();
                List<DepartmentDTO> departments = new List<DepartmentDTO>();
                if (db.tblDepartment != null)
                {
                    foreach (tblDepartment d in db.tblDepartment)
                    {
                        DepartmentDTO dp = new DepartmentDTO
                        {
                            DepartmentName = d.DepartmentName
                        };
                        departments.Add(dp);
                    }
                    return Content(HttpStatusCode.OK, departments);
                }
                return Content(HttpStatusCode.NotFound, "Sorry the tblDepartment is empty!");

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Sorry something wrong! " + ex.Message);
            }
 

        }
    }
}