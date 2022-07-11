using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DATA;
using WebApi.DTO;

namespace WebApi.Controllers
{
    [RoutePrefix("login")]
    public class LoginController : ApiController
    {
        [HttpPost]
        [Route("PostFindUser")]
        public IHttpActionResult PostFindUser([FromBody] StudentUserDTO user)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                tblAdmin checkAdmins = db.tblAdmin.SingleOrDefault(a => a.UserName == user.Email && a.Password == user.Password);
                if (checkAdmins != null)
                {
                    return Content(HttpStatusCode.OK, "admin");
                }
                else
                {
                    //tbl student empty.
                    if (db.tblStudent.Count() == 0)
                    {
                        return Content(HttpStatusCode.NotFound, $"Sorry, there is no user with this login details");
                    }
                    string checkIdStudent = db.tblStudent.SingleOrDefault(student => student.Email == user.Email).StudentId;
                    if (checkIdStudent != null)
                    {
                        tblSuperStudent checkSuperS = db.tblSuperStudent.SingleOrDefault(super => super.StudentId == checkIdStudent);
                        if (checkSuperS != null)
                        {
                            tblStudent stud = db.tblStudent.SingleOrDefault(s => s.StudentId == checkIdStudent);
                            if (stud.Email == user.Email && stud.Password == user.Password)
                            {
                                return Content(HttpStatusCode.OK, "superStudent");
                            }
                        }
                        else
                        {
                            tblStudent checkStudents = db.tblStudent.SingleOrDefault(student => student.Email == user.Email && student.Password == user.Password);
                            if (checkStudents != null)
                            {
                                return Content(HttpStatusCode.OK, "student");
                            }
                        }
                    }

                }
                return Content(HttpStatusCode.NotFound, $"Sorry, there is no user with this login details");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Something wrong! " + ex.Message);
            }
        }
    }
}