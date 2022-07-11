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
    [RoutePrefix("admin")]
    public class AdminController : ApiController
    {
        [HttpGet]
        [Route("getAll")]
        public IHttpActionResult getAll()
        {
            try
            {
                AppDbContext db = new AppDbContext();
                List<AdminUserDTO> users = new List<AdminUserDTO>();
                if (db.tblAdmin != null)
                {
                    foreach (tblAdmin a in db.tblAdmin)
                    {
                        AdminUserDTO ad = new AdminUserDTO();
                        ad.UserName = a.UserName;
                        ad.Password = a.Password;
                        users.Add(ad);
                    }
                    return Content(HttpStatusCode.OK, users);
                }
                return Content(HttpStatusCode.NotFound, "No users to return!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "something wrong! " + ex.Message);
            }
        }
    }
}