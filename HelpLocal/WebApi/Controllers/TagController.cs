using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DATA;
using WebApi.DTO;
using System.Web.Http.Cors;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [RoutePrefix("Tags")]
    public class TagController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        [HttpGet]
        [Route("getAll")]
        public IHttpActionResult getAllTags()
        {
            try
            {
                AppDbContext db = new AppDbContext();
                if (db.tblTags != null)
                {
                    List<string> tagsList = new List<string>();
                    List<tblTags> tags = db.tblTags.ToList();
                    foreach (tblTags tag in tags)
                    {
                        string tg = tag.TagName;
                        tagsList.Add(tg);
                    }
                    return Content(HttpStatusCode.OK, tagsList);
                }
                return Content(HttpStatusCode.NoContent, "Sorry tblTags is empty!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Something Went Wrong... " + ex.Message);
            }
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        [Route("PostTagsToDB")]
        public IHttpActionResult PostTags([FromBody] string[] value)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                if (value != null)
                {
                    foreach (string item in value)
                    {
                        tblTags t = new tblTags() { TagName = item };
                        db.tblTags.Add(t);

                    }
                    db.SaveChangesAsync();
                    return Content(HttpStatusCode.OK, "added");
                }
                return Content(HttpStatusCode.NotFound, "array not found");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }


        }


        [HttpPost]
        [Route("PostNewTag")]
        public IHttpActionResult PostNewTag([FromBody] string newTag)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                if (newTag != null)
                {
                    tblTags newTagToAdd = new tblTags()
                    {
                        TagName = newTag
                    };
                    db.tblTags.Add(newTagToAdd);
                    db.SaveChanges();

                    return Created(new Uri(Request.RequestUri.AbsoluteUri + "/" + newTagToAdd.TagName), newTag);
                }
                return Content(HttpStatusCode.NotFound, "request not found");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Invalid request " + ex.Message);
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}