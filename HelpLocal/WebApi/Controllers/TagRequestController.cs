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
    [RoutePrefix("tagRequest")]
    public class TagRequestController : ApiController
    {
        [HttpGet]
        [Route("getAll")]
        //returns all requests for student. kedar
        public IHttpActionResult GetAll()
        {
            try
            {
                AppDbContext db = new AppDbContext();
                List<tblTagsRequests> dbRequests = db.tblTagsRequests.ToList();
                List<TagRequestDTO> requests = new List<TagRequestDTO>();
                List<string> tags = new List<string>();
                if (db.tblTagsRequests != null)
                {
                    dbRequests = db.tblTagsRequests.Where(r => r.RequestStatus == "onHold").ToList();
                    if (dbRequests != null)
                    {
                        requests = dbRequests.GroupBy(r => new { r.StudentId, r.ClassName, r.StudentName, r.RequestDate }).Select(item => new TagRequestDTO()
                        {
                            Tags = item.Select(t => t.TagName).ToList(),
                            RequestStatus = "onHold",
                            StudentId = item.Key.StudentId,
                            StudentName = item.Key.StudentName,
                            ClassName = item.Key.ClassName,
                            RequestDate = item.Key.RequestDate.ToString()
                        }).ToList();
                        return Content(HttpStatusCode.OK, requests);
                    }
                }
                return Content(HttpStatusCode.NoContent, "Sorry the tblRequestToJoin is empty!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Something wrong! " + ex.Message);
            }
        }
        [HttpPost]
        [Route("NewRequest")]
        public IHttpActionResult PostNewTagRequest([FromBody] TagRequestDTO NewRequestFromClient)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                List<string> newTags = new List<string>();
                List<string> invalidTags = new List<string>();
                List<tblTags> tags = db.tblTags.ToList();
                string res = "";
                if (NewRequestFromClient != null)
                {
                    foreach (string tag in NewRequestFromClient.Tags)
                    {

                        tblTags dbTag = db.tblTags.SingleOrDefault(t => t.TagName == tag);
                        if (dbTag != null)
                        {
                            invalidTags.Add(tag);
                        }
                        else
                        {
                            newTags.Add(tag);
                        }
                    }
                    if (invalidTags.Count() > 0)
                    {
                        foreach (string t in invalidTags)
                        {
                            res += t + " - ";
                        }
                        return Content(HttpStatusCode.Forbidden, "תגיות אלו כבר קיימות(" + invalidTags.Count() + ") - " + res);
                    }
                    else
                    {
                        foreach (string t in newTags)
                        {
                            tblTagsRequests RequestToInsert = new tblTagsRequests()
                            {
                                TagName = t,
                                RequestStatus = NewRequestFromClient.RequestStatus,
                                StudentId = NewRequestFromClient.StudentId,
                                StudentName = NewRequestFromClient.StudentName,
                                ClassName = NewRequestFromClient.ClassName,
                                RequestDate = DateTime.Parse(NewRequestFromClient.RequestDate).Date
                            };
                            db.tblTagsRequests.Add(RequestToInsert);
                            res += t + " - ";
                        }
                        db.SaveChanges();
                        return Content(HttpStatusCode.Created, "CREATED");
                    }
                }
                return Content(HttpStatusCode.NoContent, "Sorry the Object that you sended is null!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Something wrong! " + ex.Message);
            }
        }
        [HttpPut]
        [Route("updateTagStatus")]
        public IHttpActionResult PutUpdateTagRequestStatus([FromBody] UpdateTagRequestDTO FromClient)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                tblTagsRequests request = db.tblTagsRequests.SingleOrDefault(r => r.TagName == FromClient.TagName);
                if (request != null)
                {
                    request.RequestStatus = FromClient.RequestStatus;
                    db.SaveChanges();
                    return Content(HttpStatusCode.OK, FromClient);
                }
                return Content(HttpStatusCode.NotFound, $"Sorry there is no Request with name:{FromClient.TagName}");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Something wrong! " + ex.Message);
            }
        }
    }
}