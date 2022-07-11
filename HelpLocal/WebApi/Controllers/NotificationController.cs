using DATA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.DTO;

namespace WebApi.Controllers
{
    [RoutePrefix("notification")]
    public class NotificationController : ApiController
    {
        [HttpGet]
        [Route("GetNotificationsTags/{StudentId}")]
        public IHttpActionResult GetNotificationsTags(string StudentId)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                tblStudent student = db.tblStudent.SingleOrDefault(s => s.StudentId == StudentId);
                if (student.tblTags.Count==0)
                {
                    return Content(HttpStatusCode.BadRequest, $"for student with id:{StudentId} has not yet registered to notifications!");
                }
                List<TagsDTO> allTagsNotifications = new List<TagsDTO>();
                foreach (tblTags existsTag in student.tblTags)
                {
                    TagsDTO tagToAdd = new TagsDTO()
                    {
                        TagName = existsTag.TagName
                    };
                    allTagsNotifications.Add(tagToAdd);
                }
                return Content(HttpStatusCode.OK, allTagsNotifications);         
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, $"Sorry something wrong. exception message:{ex.Message}");
            }
        }

        [HttpPost]
        [Route("UpdateTagsNotifications/{StudentId}")]
        public IHttpActionResult UpdateTagsNotifications(List<TagsDTO> TagsNotifications,string StudentId)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                tblStudent student = db.tblStudent.SingleOrDefault(s => s.StudentId == StudentId);
                //this student not have TagsNotifications
                if (student.tblTags.Count==0)
                {
                    //All tags in db.
                    List<tblTags> allTags = db.tblTags.ToList();
                    foreach (TagsDTO tagNotificationFromClient in TagsNotifications)
                    {
                        tblTags tagToAdd = allTags.SingleOrDefault(t => t.TagName == tagNotificationFromClient.TagName);
                        student.tblTags.Add(tagToAdd);
                    }
                    db.SaveChanges();
                    return Content(HttpStatusCode.OK, TagsNotifications);
                }
                else
                {
                    //Clear all Tags Notifications.
                    student.tblTags.Clear();
                    //All tags in db.
                    List<tblTags> allTags = db.tblTags.ToList();
                    foreach (TagsDTO tagNotificationFromClient in TagsNotifications)
                    {
                        tblTags tagToAdd = allTags.SingleOrDefault(t => t.TagName == tagNotificationFromClient.TagName);
                        student.tblTags.Add(tagToAdd);
                    }
                    db.SaveChanges();
                    return Content(HttpStatusCode.OK, TagsNotifications);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, $"Sorry something wrong. exception message:{ex.Message}");
            }
        }
    }
}
