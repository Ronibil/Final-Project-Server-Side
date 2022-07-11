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
    [RoutePrefix("HotTags")]
    public class HotTagsController : ApiController
    {
        [HttpGet]
        [Route("GetTop5HotTags")]
        public IHttpActionResult GetTop5HotTags()
        {
            try
            {
                AppDbContext db = new AppDbContext();
                //.OrderByDescending(t => t.tblStudent.Count).Take(10).ToList()
                List<tblTags> allTagsInTblTagsNotifications = db.tblTags.Where(t => t.tblStudent.Count > 0).ToList();
                if (allTagsInTblTagsNotifications.Count==0)
                {
                    return Content(HttpStatusCode.BadRequest, "There are no hot tags yet");
                }
                //Create list to return.
                List<HotTagDTO> hotTagsList = new List<HotTagDTO>();
                foreach (tblTags tag in allTagsInTblTagsNotifications)
                {
                    HotTagDTO hotTagToAdd = new HotTagDTO()
                    {
                        TagName = tag.TagName,
                        NumOfRequirements = tag.tblStudent.Count,
                        NumOfUsesInClasses = tag.tblClass.Count
                    };
                    hotTagsList.Add(hotTagToAdd);
                }
                //Sorting List by IComparable.
                hotTagsList.Sort();
                if (hotTagsList.Count >5)
                {                   
                    return Content(HttpStatusCode.OK, hotTagsList.Take(5));
                }
                else
                {
                    return Content(HttpStatusCode.OK, hotTagsList);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, $"Sorry something wrong! exception message:{ex.Message}");
            }
        }
    }
}
