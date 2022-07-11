using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DATA;
using WebApi.DTO;
using System.Web.Http.Cors;
using System.Text;
using WebApi.Extention;

namespace WebApi.Controllers
{
    [RoutePrefix("Class")]
    public class ClassController : ApiController
    {
        [HttpGet]
        [Route("GetAllClasses")]
        public IHttpActionResult GetAllClasses()
        {
            try
            {
                AppDbContext db = new AppDbContext();
                if (db.tblClass != null)
                {
                    List<ClassDTO> classList = new List<ClassDTO>(); //List of Classes;
                    List<tblClass> AllClasses = db.tblClass.ToList();// get Data from db.
                    foreach (tblClass c in AllClasses)
                    {
                        List<string> ClassTags = new List<string>();//List of tags insert to ClassDTO.
                        //The class must have a Tag. so no need check if the List empty!
                        foreach (tblTags Tag in c.tblTags)
                        {
                            ClassTags.Add(Tag.TagName);
                        }
                        ClassDTO classDTO = new ClassDTO()
                        {
                            ClassCode = c.ClassCode,
                            ClassDate = (DateTime)c.ClassDate,
                            StartTime = (TimeSpan)c.StartTime,
                            EndTime = (TimeSpan)c.EndTime,
                            ClassName = c.ClassName,
                            SuperStudentId = (c.SuperStudentId),
                            NumOfParticipants = (short)c.NumOfParticipants,
                            ClassDescription = c.ClassDescription,
                            SuperName=c.tblSuperStudent.tblStudent.FullName,
                            Tags = ClassTags
                        };
                        classList.Add(classDTO);
                    }
                    return Content(HttpStatusCode.OK, classList);
                }
                return Content(HttpStatusCode.NoContent, "Sorry tblClass is empty!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Sorry something wrong! " + ex.Message);

            }
        }

        [HttpPost]
        [Route("PostNewClass")]
        public IHttpActionResult PostNewClass([FromBody] ClassDTO newClass)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                if (newClass != null)
                {
                    tblClass NewClassToAdd = new tblClass()
                    {
                        ClassDate = newClass.ClassDate.Date,
                        StartTime = newClass.StartTime,
                        EndTime = newClass.EndTime,
                        ClassName = newClass.ClassName,
                        SuperStudentId = newClass.SuperStudentId.ToString(),
                        NumOfParticipants = Convert.ToInt16(newClass.NumOfParticipants),
                        ClassDescription = newClass.ClassDescription
                    };
                    List<tblTags> AllTags = db.tblTags.ToList();//All tags in db.
                    foreach (string TagFromClient in newClass.Tags)
                    {
                        tblTags TagToAdd = AllTags.SingleOrDefault(T => T.TagName == TagFromClient);
                        NewClassToAdd.tblTags.Add(TagToAdd);
                    }
                    db.tblClass.Add(NewClassToAdd);
                    db.SaveChanges();
                    //Check if there are any students who are registered to see these tags notifications!
                    mailSender.SendMailNotificationToStudent(newClass);
                    return Created(new Uri(Request.RequestUri.AbsoluteUri + "/" + NewClassToAdd.ClassCode), newClass);
                }
                return Content(HttpStatusCode.NotFound, "Sorry Invalid details!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "The request can't be proccessed " + ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteClassByClassCode/{classCode}")]
        public IHttpActionResult DeleteClassByClassCode(int classCode)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                tblClass classToRemove = db.tblClass.SingleOrDefault(x => x.ClassCode == classCode);
                if (classToRemove != null)
                {
                    //חיפוש סטודנטים רשומים ושליחת מייל
                    if (classToRemove.tblRegisteredTo.Count > 0)//בודק שיש רשומים
                    {
                        foreach (tblRegisteredTo Participant in classToRemove.tblRegisteredTo)//עובר על כל הרשומים לשיעור
                        {
                            tblStudent student = db.tblStudent.SingleOrDefault(s => s.StudentId == Participant.StudentId);
                            if (student != null)//בודק אם הסטודנט קיים במערכת ולא נמחק מהאפלקצייה
                            {
                                mailSender.sendEmailStudentUpdateOnCanceledClass(student, classToRemove);// המייל נשלח בהצלחה
                            }
                        }
                    }
                    db.tblClass.Remove(classToRemove);
                    db.SaveChanges();
                    return Content(HttpStatusCode.OK, "The class removed");
                }
                return Content(HttpStatusCode.NotFound, "The class is not found");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "something wrong" + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetClassesByTags/{StudentId}")]
        public IHttpActionResult GetClassesByTags(List<TagsDTO> TagsFromClient, string StudentId)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                if (TagsFromClient != null)
                {
                    //tblStudent student = db.tblStudent.SingleOrDefault(s => s.StudentId == StudentId);
                    List<tblClass> AllClasses = db.tblClass.ToList();
                    List<ClassDTO> ClassesToReturn = new List<ClassDTO>();

                    foreach (tblClass Class in AllClasses)
                    {
                        //SuperStudent object to get Super student rank.
                        tblSuperStudent SuperStudent = Class.tblSuperStudent;
                        if (Class.ClassDate >= DateTime.Now.Date)
                        {
                            //Check if student registred to this class.
                            List<tblRegisteredTo> registredStudent = Class.tblRegisteredTo.ToList();
                            //Check if this class have Student registred.
                            if (registredStudent.SingleOrDefault(r => r.StudentId == StudentId) != null)
                            {
                                // Go to next class.
                                continue;
                            }

                            foreach (TagsDTO TagName in TagsFromClient)
                            {
                                tblTags TagToCheck = db.tblTags.SingleOrDefault(t => t.TagName == TagName.TagName);
                                if (Class.tblTags.Contains(TagToCheck))
                                {
                                    
                                    ClassDTO ClassToAdd = new ClassDTO()
                                    {
                                        ClassCode = Class.ClassCode,
                                        ClassDate = Convert.ToDateTime(Class.ClassDate),
                                        StartTime = (TimeSpan)Class.StartTime,
                                        EndTime = (TimeSpan)Class.EndTime,
                                        ClassName = Class.ClassName,
                                        SuperStudentId = Class.SuperStudentId,
                                        NumOfParticipants = (short)Class.NumOfParticipants,
                                        NumOfRegistered = (short)Class.tblRegisteredTo.Count,
                                        ClassDescription = Class.ClassDescription,
                                        SuperName = Class.tblSuperStudent.tblStudent.FullName,
                                        SuperStudentRank = (short)SuperStudent.RankAverage                                      
                                    };
                                    List<string> Tags = new List<string>();
                                    foreach (tblTags TagObject in Class.tblTags)
                                    {
                                        string TagToAdd = TagObject.TagName;
                                        Tags.Add(TagToAdd);
                                    }
                                    ClassToAdd.Tags = Tags;
                                    //Count how much matching tags.
                                    int NumOfMatchingTags = 0;
                                    foreach (TagsDTO TagToCount in TagsFromClient)
                                    {
                                        if (Tags.Contains(TagToCount.TagName))
                                        {
                                            NumOfMatchingTags++;
                                        }
                                    }
                                    // update NumOfMatchingTags for this class.
                                    ClassToAdd.NumOfMatchs = NumOfMatchingTags;
                                    ClassesToReturn.Add(ClassToAdd);
                                    break;
                                }
                            }
                        }
                    }
                    if (ClassesToReturn.Count == 0)
                    {
                        return Content(HttpStatusCode.BadRequest, "Sorry there are still no classes with tags that you sended. please try another tags.");
                    }
                    else
                    {
                        //Sort the list by NumOfMacthingTags and Super student rank.
                        ClassesToReturn.Sort();
                        return Content(HttpStatusCode.OK, ClassesToReturn);
                    }
                }
                return Content(HttpStatusCode.NoContent, "Sorry, invalid details!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Sorry something wrong! " + ex.Message);
            }
        }

        [HttpGet]
        [Route("GetSuggestionsClasses/{StudentId}")]
        public IHttpActionResult GetSuggestionsClasses(string StudentId)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                if (StudentId != null)
                {
                    //All tblRegisteredTo objects for this student.
                    tblStudent student = db.tblStudent.SingleOrDefault(s => s.StudentId == StudentId);
                    //get all registeredTo object from last 30 days.
                    List<tblRegisteredTo> registeredTo = student.tblRegisteredTo.Where(c => c.RegistrationDate >= DateTime.Now.AddDays(-30)).ToList();
                    // if this student not registered for any class.
                    if (registeredTo.Count == 0)
                    {
                        return Content(HttpStatusCode.NotFound, "student is not registered for any class yet.");
                    }
                    List<TagsDTO> TagsDto = new List<TagsDTO>();
                    List<string> TagsFromClasses = new List<string>();
                    foreach (tblRegisteredTo registered in registeredTo)
                    {
                        //All tags in one class.
                        List<tblTags> Tags = registered.tblClass.tblTags.ToList();
                        foreach (tblTags Tag in Tags)
                        {
                            if (TagsFromClasses.Contains(Tag.TagName))
                            {
                                continue;
                            }
                            else
                            {
                                TagsFromClasses.Add(Tag.TagName);
                            }
                        }
                    }
                    foreach (string TagNameString in TagsFromClasses)
                    {
                        TagsDTO t = new TagsDTO()
                        {
                            TagName = TagNameString
                        };
                        TagsDto.Add(t);
                    }
                    //starting to return classes
                    List<tblClass> AllClasses = db.tblClass.ToList();
                    List<ClassDTO> ClassesToReturn = new List<ClassDTO>();
                    foreach (tblClass Class in AllClasses)
                    {
                        if (Class.ClassDate >= DateTime.Now.Date)
                        {
                            //Check if student registred to this class.
                            List<tblRegisteredTo> registredStudents = Class.tblRegisteredTo.ToList();
                            //Check if this class have Student registred.
                            if (registredStudents.SingleOrDefault(r => r.StudentId == StudentId) != null)
                            {
                                // Go to next class.
                                continue;
                            }

                            foreach (TagsDTO TagName in TagsDto)
                            {
                                tblTags TagToCheck = db.tblTags.SingleOrDefault(t => t.TagName == TagName.TagName);
                                if (Class.tblTags.Contains(TagToCheck))
                                {

                                    ClassDTO ClassToAdd = new ClassDTO()
                                    {
                                        ClassCode = Class.ClassCode,
                                        ClassDate = Convert.ToDateTime(Class.ClassDate),
                                        StartTime = (TimeSpan)Class.StartTime,
                                        EndTime = (TimeSpan)Class.EndTime,
                                        ClassName = Class.ClassName,
                                        SuperStudentId = Class.SuperStudentId,
                                        NumOfParticipants = (short)Class.NumOfParticipants,
                                        NumOfRegistered = (short)Class.tblRegisteredTo.Count,
                                        ClassDescription = Class.ClassDescription,
                                        SuperName = Class.tblSuperStudent.tblStudent.FullName,
                                        SuperStudentRank= (short)Class.tblSuperStudent.RankAverage
                                    };
                                    List<string> Tags = new List<string>();
                                    foreach (tblTags TagObject in Class.tblTags)
                                    {
                                        string TagToAdd = TagObject.TagName;
                                        Tags.Add(TagToAdd);
                                    }
                                    ClassToAdd.Tags = Tags;
                                    ClassesToReturn.Add(ClassToAdd);
                                    break;
                                }
                            }
                        }
                    }
                    if (ClassesToReturn.Count == 0)
                    {
                        return Content(HttpStatusCode.NotFound, "Sorry there are still no classes with tags that you sended. please try another tags.");
                    }
                    else
                    {
                        return Content(HttpStatusCode.OK, ClassesToReturn);
                    }
                }
                return Content(HttpStatusCode.NoContent, "Sorry, invalid details!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, $"Sorry something wrong!. exception message:{ex.Message}");
            }
        }

    }
}