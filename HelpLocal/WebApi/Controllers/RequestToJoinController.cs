using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DATA;
using WebApi.DTO;
using System.Data.Entity;
using System.Web.Http.Cors;
using System.Net.Mail;
using WebApi.Extention;

namespace WebApi.Controllers
{
    [RoutePrefix("requestToJoin")]
    public class RequestToJoinController : ApiController
    {
        [HttpGet]
        [Route("getAll")]
        //returns all requests for student. kedar
        public IHttpActionResult GetAll()
        {
            try
            {
                AppDbContext db = new AppDbContext();
                List<RequestToJoinDTO> requests = new List<RequestToJoinDTO>();
                if (db.tblRequestToJoin != null)
                {
                    foreach (tblRequestToJoin rtj in db.tblRequestToJoin)
                    {
                        RequestToJoinDTO requestDTO = new RequestToJoinDTO
                        {
                            RequsetNum = rtj.RequsetNum,
                            StudentId = rtj.StudentId,
                            FullName = rtj.FullName,
                            Email = rtj.Email,
                            BirthDate = rtj.BirthDate.ToString(),
                            Gender = rtj.Gender,
                            City = rtj.City,
                            Phone = rtj.Phone,
                            RequestDate = rtj.RequestDate.ToString(),
                            RequestStatus = rtj.RequestStatus
                        };
                        requests.Add(requestDTO);
                    }
                    return Content(HttpStatusCode.OK, requests);
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
        //Insert into tblRequestToJoin a New RequestToJoin for Student. kedar
        public IHttpActionResult PostNewRequest([FromBody] RequestToJoinDTO NewRequestFromClient)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                if (NewRequestFromClient != null)
                {
                    tblRequestToJoin RequestToInsert = new tblRequestToJoin()
                    {
                        StudentId = NewRequestFromClient.StudentId,
                        FullName = NewRequestFromClient.FullName,
                        Email = NewRequestFromClient.Email,
                        BirthDate = DateTime.Parse(NewRequestFromClient.BirthDate),
                        Gender = NewRequestFromClient.Gender,
                        City = NewRequestFromClient.City,
                        Phone = NewRequestFromClient.Phone,
                        RequestDate = DateTime.Parse(NewRequestFromClient.RequestDate),
                        RequestStatus = NewRequestFromClient.RequestStatus
                    };
                    db.tblRequestToJoin.Add(RequestToInsert);
                    db.SaveChanges();
                    mailSender.sendNewRequestToAdmin(NewRequestFromClient);
                    return Created(new Uri(Request.RequestUri.AbsoluteUri + RequestToInsert.RequsetNum), RequestToInsert);
                }
                return Content(HttpStatusCode.NoContent, "Sorry the Object that you sended is null!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Something wrong! " + ex.Message);
            }
        }

        [HttpPost]
        [Route("isEmailExist")] 
        public IHttpActionResult PostFindEmail([FromBody] StudentUserDTO email)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                tblRequestToJoin request = db.tblRequestToJoin.SingleOrDefault(r => r.RequestStatus == "onHold" && r.Email == email.Email);
                if (request != null)
                {
                    return Content(HttpStatusCode.OK, "found");
                }
                else
                {
                    tblStudent student = db.tblStudent.SingleOrDefault(s => s.Email == email.Email);
                    if (student != null)
                    {
                        return Content(HttpStatusCode.OK, "found");
                    }
                    else
                    {
                        return Content(HttpStatusCode.OK, "not found");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Something wrong! " + ex.Message);
            }
        }

        [HttpPut]
        [Route("updateRequestStatus")]
        //update Request status from onHold to - (approved/Rejected). kedar
        public IHttpActionResult PutUpdateRequestStatus([FromBody] RequestStatusDTO FromClient)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                //Search the Request object by StudentId.
                tblRequestToJoin req = db.tblRequestToJoin.SingleOrDefault(r => r.StudentId == FromClient.StudentId);
                if (req != null)
                {
                    req.RequestStatus = FromClient.RequestStatus;
                    db.SaveChanges();
                    tblRequestToJoin user = db.tblRequestToJoin.SingleOrDefault(x => x.StudentId == FromClient.StudentId);
                    mailSender.sendEmailVerify(user, FromClient.RequestStatus);
                    //return Content(HttpStatusCode.OK, FromClient);
                    return Created(new Uri(Request.RequestUri.AbsoluteUri + "/" + FromClient.StudentId), $"The Request status with RequestNum:{req.RequsetNum} has been changed successfuly to: {FromClient.RequestStatus}.");
                }
                return Content(HttpStatusCode.NotFound, $"Sorry there is no request for Student with id:{FromClient.StudentId}");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Something wrong! " + ex.Message);
            }
        }
    }
}