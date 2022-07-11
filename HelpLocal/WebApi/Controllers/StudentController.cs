using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DATA;
using WebApi.DTO;
using System.Web.Http.Cors;
using WebApi.Extention;
using System.Web.Security;
using System.Text.RegularExpressions;

namespace WebApi.Controllers
{
    [RoutePrefix("Student")]
    public class StudentController : ApiController
    {
        [HttpGet]
        [Route("GetAllStudentUsers")]
        //שולח את כל המשתמשים
        public IHttpActionResult GetAllStudentUsers()
        {
            try
            {
                AppDbContext db = new AppDbContext();
                List<StudentUserDTO> AllStudentsUsers = new List<StudentUserDTO>();
                if (db.tblStudent != null)
                {
                    foreach (tblStudent S in db.tblStudent)
                    {
                        StudentUserDTO UserDTO = new StudentUserDTO();
                        UserDTO.Email = S.Email;
                        UserDTO.Password = S.Password;
                        AllStudentsUsers.Add(UserDTO);
                    }
                    return Content(HttpStatusCode.OK, AllStudentsUsers);
                }
                else
                {
                    return Content(HttpStatusCode.NoContent, "Sorry there are no Users in the system!");
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Sorry something wrong! " + ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllStudentsDetails")]
        public IHttpActionResult GetAllStudentsDetails()
        {
            try
            {
                AppDbContext db = new AppDbContext();
                if (db.tblStudent != null)
                {
                    List<StudentDTO> AllStudentsDetails = new List<StudentDTO>();
                    foreach (tblStudent S in db.tblStudent)
                    {
                        StudentDTO StuDTO = new StudentDTO()
                        {
                            StudentId = S.StudentId,
                            FullName = S.FullName,
                            Email = S.Email,
                            BirthDate = S.BirthDate.ToString(),
                            Gender = S.Gender,
                            City = S.City,
                            Phone = S.Phone,
                            RegistrationDate = S.RegistrationDate.ToString()
                        };
                        AllStudentsDetails.Add(StuDTO);
                    }
                    return Content(HttpStatusCode.OK, AllStudentsDetails);
                }
                return Content(HttpStatusCode.NoContent, "Sorry tblStudent is empty!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Sorry something wrong! " + ex.Message);
            }
        }

        [HttpGet]
        [Route("getStudentById/{id}")]
        public IHttpActionResult GetStudentById(string id)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                tblStudent student = db.tblStudent.SingleOrDefault(s => s.StudentId == id);
                if (student != null)
                {
                    StudentDTO studentDTO = new StudentDTO()
                    {
                        StudentId = student.StudentId,
                        FullName = student.FullName,
                        Email = student.Email,
                        Password = student.Password,
                        BirthDate = student.BirthDate.ToString(),
                        Gender = student.Gender,
                        City = student.City,
                        Phone = student.Phone
                    };
                    return Content(HttpStatusCode.OK, studentDTO);
                }
                else
                {
                    return Content(HttpStatusCode.NotFound, $"Sorry, there is no user with this ID");
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Sorry something wrong! " + ex.Message);
            }
        }

        [HttpPost]
        [Route("PostNewStudent")]
        public IHttpActionResult PostNewStudent([FromBody] StudentDTO fromClient)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                if (fromClient != null)
                {
                    tblStudent s = new tblStudent()
                    {
                        StudentId = fromClient.StudentId,
                        FullName = fromClient.FullName,
                        Email = fromClient.Email,
                        Password = fromClient.Password,//The password will be the Student id.
                        BirthDate = DateTime.Parse(fromClient.BirthDate),
                        Gender = fromClient.Gender,
                        City = fromClient.City,
                        Phone = fromClient.Phone,
                        RegistrationDate = DateTime.Parse(fromClient.RegistrationDate)
                    };
                    db.tblStudent.Add(s);
                    db.SaveChanges();
                    return Created(new Uri(Request.RequestUri.AbsoluteUri + s.StudentId), s);
                }
                return Content(HttpStatusCode.NoContent, "Sorry the Object that you sended is null!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Sorry something wrong! " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetStudentLandingPageDetails")]
        public IHttpActionResult GetStudentLandingPageDetails([FromBody] StudentUserDTO user)
        {
            try
            {
                List<ClassDTO> history = new List<ClassDTO>();
                List<ClassDTO> futre = new List<ClassDTO>();
                if (user != null)
                {
                    AppDbContext db = new AppDbContext();
                    tblStudent StudentRegTo = db.tblStudent.SingleOrDefault(stu => stu.Email == user.Email);
                    StudentLandingPageDetailsDTO StudentDetailsToReturn = new StudentLandingPageDetailsDTO();

                    //From tblStuent
                    StudentDetailsToReturn.StudentId = StudentRegTo.StudentId;
                    StudentDetailsToReturn.FullName = StudentRegTo.FullName;

                    //Filling the lists Of Classes {FutreClasses/ClassesHistory}
                    foreach (tblRegisteredTo RegisteredToClass in StudentRegTo.tblRegisteredTo)//2
                    {
                        ClassDTO ClassToAddDTO;
                        if (RegisteredToClass.tblClass.ClassDate >= DateTime.Now.Date)
                        {
                            ClassToAddDTO = new ClassDTO()
                            {
                                ClassCode = RegisteredToClass.tblClass.ClassCode,
                                ClassDate = Convert.ToDateTime(RegisteredToClass.tblClass.ClassDate),
                                ClassDescription = RegisteredToClass.tblClass.ClassDescription,
                                StartTime = (TimeSpan)RegisteredToClass.tblClass.StartTime,
                                EndTime = (TimeSpan)RegisteredToClass.tblClass.EndTime,
                                ClassName = RegisteredToClass.tblClass.ClassName,
                                NumOfParticipants = (short)RegisteredToClass.tblClass.NumOfParticipants,
                                SuperStudentId = RegisteredToClass.tblClass.SuperStudentId,
                                SuperName = RegisteredToClass.tblClass.tblSuperStudent.tblStudent.FullName,
                                SuperStudentRank = (short)RegisteredToClass.tblClass.tblSuperStudent.RankAverage
                            };
                            List<string> tagsInClass = new List<string>();
                            foreach (tblTags Tag in RegisteredToClass.tblClass.tblTags)
                            {
                                tagsInClass.Add(Tag.TagName);
                            };
                            ClassToAddDTO.Tags = tagsInClass;
                            futre.Add(ClassToAddDTO);
                        }
                        else
                        {
                            //History classes.
                            ClassToAddDTO = new ClassDTO()
                            {
                                ClassCode = RegisteredToClass.tblClass.ClassCode,
                                ClassDate = Convert.ToDateTime(RegisteredToClass.tblClass.ClassDate),
                                ClassDescription = RegisteredToClass.tblClass.ClassDescription,
                                StartTime = (TimeSpan)RegisteredToClass.tblClass.StartTime,
                                EndTime = (TimeSpan)RegisteredToClass.tblClass.EndTime,
                                ClassName = RegisteredToClass.tblClass.ClassName,
                                NumOfParticipants = (short)RegisteredToClass.tblClass.NumOfParticipants,
                                SuperStudentId = RegisteredToClass.tblClass.SuperStudentId,
                                SuperName = RegisteredToClass.tblClass.tblSuperStudent.tblStudent.FullName,
                                SuperStudentRank = (short)RegisteredToClass.tblClass.tblSuperStudent.RankAverage
                            };
                            //if student submit rank.
                            List<RankResultDTO> listRankResult = new List<RankResultDTO>();
                            RankResultDTO rankResultForStudent = new RankResultDTO();
                            if (RegisteredToClass.StudentRank > 0)
                            {
                                rankResultForStudent.RankValue = (short)RegisteredToClass.StudentRank;
                                rankResultForStudent.RankDescription = RegisteredToClass.RankDescription;
                            }
                            else
                            {
                                rankResultForStudent.RankValue = 0;
                                rankResultForStudent.RankDescription = "עדיין לא דירגת שיעור זה";
                            }
                            listRankResult.Add(rankResultForStudent);
                            ClassToAddDTO.RankResults = listRankResult;
                            List<string> tagsInClass = new List<string>();
                            foreach (tblTags Tag in RegisteredToClass.tblClass.tblTags)
                            {
                                tagsInClass.Add(Tag.TagName);
                            };

                            ClassToAddDTO.Tags = tagsInClass;
                            history.Add(ClassToAddDTO);
                        }
                    }
                    StudentDetailsToReturn.ClassesHistory = history;
                    StudentDetailsToReturn.FutreClasses = futre;
                    return Content(HttpStatusCode.OK, StudentDetailsToReturn);
                }
                return Content(HttpStatusCode.NoContent, "Sorry invalid details!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Sorry something wrong! " + ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteStudentFromClass")]
        public IHttpActionResult DeleteStudentFromClass([FromBody] StudentIdClassCodeDTO student)
        {
            try
            {
                if (student != null)
                {
                    AppDbContext db = new AppDbContext();
                    tblStudent stud = db.tblStudent.SingleOrDefault(s => s.StudentId == student.StudentId);
                    stud.tblRegisteredTo.Remove(stud.tblRegisteredTo.SingleOrDefault(x => x.ClassCode == student.ClassCode));
                    tblClass ClassUpdateNumOfParticipant = db.tblClass.SingleOrDefault(C => C.ClassCode == student.ClassCode);
                    //Num of participans -1
                    //ClassUpdateNumOfParticipant.NumOfParticipants--;
                    db.SaveChanges();
                    //שליחת מייל לסופר סטודנט שהסטודנט נמחק מהשיעור
                    tblClass c = db.tblClass.SingleOrDefault(x => x.ClassCode == student.ClassCode);//מביא את השיעור המתאים כדי לקבל את פרטי הסופר סטודנט
                    if (c != null)
                    {
                        string Email = c.tblSuperStudent.tblStudent.Email;// מייל של הסופר סטודנט
                        string superSName = c.tblSuperStudent.tblStudent.FullName;// השם של הסופר סטודנט כדי שנוכל לשלוח לו מיי ולפנות אליו בשם שלו
                        mailSender.sendEmailToSuperStudentUpdateDelete(superSName, Email, stud, c);// המייל נשלח בהצלחה
                    }
                    return Content(HttpStatusCode.OK, $"Class with Class Code:{student.ClassCode} deleted succesfully to Student with StudentId={student.StudentId}");
                }
                return Content(HttpStatusCode.NoContent, "Sorry the Details has Invalid!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Sorry something wrong! " + ex.Message);
            }

        }

        [HttpPost]
        [Route("PostStudentToClass")]
        public IHttpActionResult PostNewStudentToClass([FromBody] StudentIdClassCodeDTO student)
        {
            try
            {
                if (student != null)
                {
                    AppDbContext db = new AppDbContext();
                    tblStudent stud = db.tblStudent.SingleOrDefault(s => s.StudentId == student.StudentId);
                    tblClass Class = db.tblClass.SingleOrDefault(c => c.ClassCode == student.ClassCode);
                    tblRegisteredTo registerTo = new tblRegisteredTo()
                    {
                        StudentId = student.StudentId,
                        ClassCode = student.ClassCode,
                        RegistrationDate = DateTime.Now,
                        StudentRank = 0,
                        IsDone = false,
                        RankDescription = null,
                        tblClass = Class,
                        tblStudent = stud
                    };
                    stud.tblRegisteredTo.Add(registerTo);
                    Class.tblRegisteredTo.Add(registerTo);

                    //שליחת מייל לסופר סטודנט שהסטודנט נרשם לשיעור
                    tblSuperStudent superStudent = db.tblSuperStudent.SingleOrDefault(s => s.StudentId == Class.SuperStudentId);// מביא פרטים של סופר סטודנט כדי לשלוח מייל מסודר
                    if (superStudent != null)
                    {
                        mailSender.sendEmailToSuperStudentUpdateRegistered(superStudent, Class, stud);//המייל נשלח בהצלחה
                    }
                    db.SaveChanges();
                    return Content(HttpStatusCode.OK, $"Student with Id={student.StudentId}, registerd succesfully to Class with Class code={student.ClassCode}");
                }
                return Content(HttpStatusCode.NoContent, "Sorry the details invaild!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Sorry something wrong! " + ex.Message);
            }
        }

        [HttpPost]
        [Route("NewPassword/{id}")]
        public IHttpActionResult generateNewPassword(string id)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                tblStudent student = db.tblStudent.SingleOrDefault(s => s.StudentId == id);
                if (student != null)
                {
                    string email = student.Email;
                    string password = Membership.GeneratePassword(8, 0);
                    password = Regex.Replace(password, @"[^a-zA-Z0-9]", m => "9");
                    student.Password = password;
                    db.SaveChanges();
                    mailSender.sendNewPasswordToUser(student.FullName, password, student.Email);
                    return Content(HttpStatusCode.OK, email + "-" + password);
                }
                else
                {
                    return Content(HttpStatusCode.NotFound, $"Sorry, there is no user with this ID");
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Something wrong! " + ex.Message);
            }
        }
        [HttpPut]
        [Route("UpdatePassword")]
        public IHttpActionResult PutUpdatePassword([FromBody] UpdatePasswordDTO FromClient)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                tblStudent s = db.tblStudent.SingleOrDefault(stud => stud.StudentId == FromClient.StudentId);
                if (s != null)
                {
                    tblSuperStudent sp = db.tblSuperStudent.SingleOrDefault(super => super.StudentId == FromClient.StudentId);
                    s.Password = FromClient.Password;
                    db.SaveChanges();
                    if (sp != null)
                    {
                        return Content(HttpStatusCode.OK, "superStudent");
                    }
                    else
                    {
                        return Content(HttpStatusCode.OK, "student");
                    }
                }
                return Content(HttpStatusCode.NotFound, $"Sorry there is no Student with id:{FromClient.StudentId}");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Something wrong! " + ex.Message);
            }
        }

    }
}