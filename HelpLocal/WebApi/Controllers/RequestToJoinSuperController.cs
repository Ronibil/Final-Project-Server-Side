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
    [RoutePrefix("RequestToJoinSuper")]
    public class RequestToJoinSuperController : ApiController
    {
        [HttpGet]
        [Route("GetAll")]
        public IHttpActionResult GetAll()
        {
            try
            {
                AppDbContext db = new AppDbContext();
                List<RequestToJoinSuperDTO> requests = new List<RequestToJoinSuperDTO>();
                if (db.tblRequestToJoinSuperStudent != null)
                {
                    foreach (tblRequestToJoinSuperStudent rtj in db.tblRequestToJoinSuperStudent)
                    {
                        RequestToJoinSuperDTO requestDTO = new RequestToJoinSuperDTO
                        {
                            RequsetNum = rtj.RequsetNum,
                            DepartmentName = rtj.DepartmentName,
                            StudyYear = rtj.StudyYear,
                            Description = rtj.Description
                        };
                        requests.Add(requestDTO);
                    }
                    return Content(HttpStatusCode.OK, requests);                   
                }
                return Content(HttpStatusCode.NoContent, "Sorry the tblRequestToJoinSuperStudent is empty!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Sorry something wrong! " + ex.Message);
            }

        }

        [HttpPost]
        [Route("NewSuperRequest")]
        public IHttpActionResult PostNewSuperRequest([FromBody] RequestToJoinSuperDTO FromClient)//INSERT INTO...
        {
            try
            {
                AppDbContext db = new AppDbContext();
                //check if this student exists in tblRequestToJoin
                if (db.tblRequestToJoin.SingleOrDefault(r => r.RequsetNum==FromClient.RequsetNum)!= null)
                {
                    tblRequestToJoinSuperStudent superRequest = new tblRequestToJoinSuperStudent()
                    {
                        RequsetNum = FromClient.RequsetNum,
                        DepartmentName = FromClient.DepartmentName,
                        StudyYear = FromClient.StudyYear,
                        Description = FromClient.Description
                    };
                    db.tblRequestToJoinSuperStudent.Add(superRequest);
                    db.SaveChanges();
                    return Created(new Uri(Request.RequestUri.AbsoluteUri + "/" + superRequest.RequsetNum), FromClient);
                }
                return Content(HttpStatusCode.NoContent, "Sorry this student not exists in tblRequestToJoin");

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest,"Sorry something wrong!! "+ex.Message);
            }
        }
    }
}