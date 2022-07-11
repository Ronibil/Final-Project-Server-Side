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
    [RoutePrefix("tip")]
    public class TipController : ApiController
    {
        [HttpGet]
        [Route("GetRandomTip")]
        public IHttpActionResult GetRandomTip()
        {
            try
            {
                AppDbContext db = new AppDbContext();
                //Number of tips in db.
                int countOfTips = db.tblTips.Count();
                Random rnd = new Random();
                int tipId = rnd.Next(1,countOfTips);
                //get Tip from db.
                tblTips tipObject = db.tblTips.SingleOrDefault(t => t.TipId == tipId);
                TipDTO tipToReturn = new TipDTO()
                {
                    TipId = tipObject.TipId,
                    TipContent = tipObject.TipContent
                };
                return Content(HttpStatusCode.OK, tipToReturn);              
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, $"Sorry something wrong. Exception message:{ex.Message}");
            }
        }
    }
}
