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
using System.Web;
using System.IO;

namespace WebApi.Controllers
{
    [RoutePrefix("SuperStudent")]
    public class SuperStudentController : ApiController
    {
        [HttpGet]
        [Route("GetAllSuperUsers")]
        //return All SuperUsers {Email,Password}.
        public IHttpActionResult GetAllSuperUsers()
        {
            try
            {
                AppDbContext db = new AppDbContext();
                if (db.tblStudent != null)
                {
                    List<SuperStudentUserDTO> SuperUsersDTO = new List<SuperStudentUserDTO>();//List to return.
                    List<string> SuperIds = db.tblSuperStudent.Select(Super => Super.StudentId).ToList();
                    foreach (string IdSuper in SuperIds)
                    {
                        foreach (tblStudent S in db.tblStudent)
                        {
                            if (IdSuper == S.StudentId)
                            {
                                SuperStudentUserDTO SuperUserDTOtoAdd = new SuperStudentUserDTO()
                                {
                                    Email = S.Email,
                                    Password = S.Password
                                };
                                SuperUsersDTO.Add(SuperUserDTOtoAdd);
                                continue;
                            }
                        }
                    }
                    return Content(HttpStatusCode.OK, SuperUsersDTO);
                }
                return Content(HttpStatusCode.NoContent, "Sorry there are no SuperUsers Students in the System!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Sorry something wrong! " + ex.Message);
            }
        }

        [HttpPost]
        [Route("PostNewSuperStudent")]
        public IHttpActionResult PostNewSuperStudent(SuperStudentDTO FromClient)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                //Check if this student Upload a image.
                string path = HttpContext.Current.Server.MapPath("~/ImageFiles/");

                //varible for update ImagePath in DB.
                FileInfo fileInfo = null;
                string[] files = Directory.GetFiles(path, "*.jpg");
                bool imageExist = false;
                foreach (string fullFilePath in files)
                {
                    fileInfo = new FileInfo(fullFilePath);
                    string checkId = fileInfo.Name.Substring(13, 9);
                    if (checkId == FromClient.StudentId)
                    {
                        imageExist = true;
                        break;
                    }
                }

                if (FromClient != null && db.tblStudent.SingleOrDefault(s => s.StudentId == FromClient.StudentId) != null)
                {
                    tblSuperStudent NewSuperStudentToInsert = new tblSuperStudent()
                    {
                        StudentId = FromClient.StudentId,
                        DepartmentName = FromClient.DepartmentName,
                        StudyYear = FromClient.StudyYear,
                        Description = FromClient.Description,
                        NumOfRanks = 0,
                        RankAverage = 0,
                        CumulativeRank = 0
                    };
                    if (imageExist == true)
                    {
                        string temp = fileInfo.Name.Substring(13, 21);
                        NewSuperStudentToInsert.ImagePath = temp;
                    }
                    else
                    {
                        NewSuperStudentToInsert.ImagePath = "empty";
                    }
                    db.tblSuperStudent.Add(NewSuperStudentToInsert);
                    db.SaveChanges();
                    //return Created(new Uri(Request.RequestUri.AbsoluteUri + NewSuperStudentToInsert.StudentId), NewSuperStudentToInsert);
                    return Content(HttpStatusCode.OK, FromClient);
                }
                return Content(HttpStatusCode.NoContent, "Sorry the Object that you sended is null!");

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Sorry something wrong! " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetSuperLandingPageDetails")]
        public IHttpActionResult GetSuperLandingPageDetails(StudentUserDTO user)
        {
            try
            {
                List<ClassDTO> history = new List<ClassDTO>();
                List<ClassDTO> futre = new List<ClassDTO>();
                if (user != null)
                {

                    AppDbContext db = new AppDbContext();
                    tblStudent Student = db.tblStudent.SingleOrDefault(stu => stu.Email == user.Email);
                    SuperLandingPageDetailsDTO SuperDetailsToReturn = new SuperLandingPageDetailsDTO();

                    //From tblStuent
                    SuperDetailsToReturn.StudentId = Student.StudentId;
                    SuperDetailsToReturn.FullName = Student.FullName;
                    //Image=Student.tblSuperStudent.Image,
                    //From tblSuperStudent
                    SuperDetailsToReturn.Description = Student.tblSuperStudent.Description;
                    SuperDetailsToReturn.DepartmentName = Student.tblSuperStudent.DepartmentName;
                    SuperDetailsToReturn.StudyYear = Student.tblSuperStudent.StudyYear;
                    SuperDetailsToReturn.NumOfRanks = Student.tblSuperStudent.NumOfRanks.Value;
                    SuperDetailsToReturn.RankAverage = Convert.ToByte(Student.tblSuperStudent.RankAverage.Value);
                    SuperDetailsToReturn.ImagePath = Student.tblSuperStudent.ImagePath;

                    //Filling the lists Of Classes {FutreClasses/ClassesHistory}
                    foreach (tblClass ClassToAdd in Student.tblSuperStudent.tblClass)

                    {
                        ClassDTO ClassToAddDTO;
                        if (ClassToAdd.ClassDate >= DateTime.Now.Date)// חסר סינון על ידי שעה מדוייקת באותו יום.
                        {
                            //future class.
                            ClassToAddDTO = new ClassDTO()
                            {
                                ClassCode = ClassToAdd.ClassCode,
                                ClassDate = Convert.ToDateTime(ClassToAdd.ClassDate),
                                ClassDescription = ClassToAdd.ClassDescription,
                                StartTime = (TimeSpan)ClassToAdd.StartTime,
                                EndTime = (TimeSpan)ClassToAdd.EndTime,
                                ClassName = ClassToAdd.ClassName,
                                NumOfParticipants = (short)ClassToAdd.NumOfParticipants,
                                NumOfRegistered = (short)ClassToAdd.tblRegisteredTo.Count,
                                SuperStudentId = ClassToAdd.SuperStudentId,
                                SuperName = ClassToAdd.tblSuperStudent.tblStudent.FullName
                            };
                            List<string> tagsInClass = new List<string>();
                            foreach (tblTags Tag in ClassToAdd.tblTags)
                            {
                                tagsInClass.Add(Tag.TagName);
                            };
                            ClassToAddDTO.Tags = tagsInClass;
                            futre.Add(ClassToAddDTO);
                        }
                        else
                        {
                            //history class.
                            ClassToAddDTO = new ClassDTO()
                            {
                                ClassCode = ClassToAdd.ClassCode,
                                ClassDate = Convert.ToDateTime(ClassToAdd.ClassDate),
                                ClassDescription = ClassToAdd.ClassDescription,
                                StartTime = (TimeSpan)ClassToAdd.StartTime,
                                EndTime = (TimeSpan)ClassToAdd.EndTime,
                                ClassName = ClassToAdd.ClassName,
                                NumOfParticipants = (short)ClassToAdd.NumOfParticipants,
                                SuperStudentId = ClassToAdd.SuperStudentId,
                                SuperName = ClassToAdd.tblSuperStudent.tblStudent.FullName
                            };
                            List<string> tagsInClass = new List<string>();
                            foreach (tblTags Tag in ClassToAdd.tblTags)
                            {
                                tagsInClass.Add(Tag.TagName);
                            };
                            ClassToAddDTO.Tags = tagsInClass;
                            if (ClassToAdd.tblRegisteredTo.Count != 0)
                            {
                                //get the rank results from tblRegisteredTo.
                                int counter = 0;
                                int cumulativeRank = 0;
                                short result;
                                List<RankResultDTO> rankResults = new List<RankResultDTO>();
                                foreach (tblRegisteredTo registeredToObject in ClassToAdd.tblRegisteredTo)
                                {
                                    if (registeredToObject.StudentRank == 0)
                                    {
                                        continue;
                                    }
                                    RankResultDTO rankResult = new RankResultDTO()
                                    {
                                        RankValue = (short)registeredToObject.StudentRank,
                                        RankDescription = registeredToObject.RankDescription
                                    };
                                    rankResults.Add(rankResult);
                                    cumulativeRank += (short)registeredToObject.StudentRank;
                                    counter++;
                                }
                                if (counter != 0)
                                {
                                    result = Convert.ToInt16(Math.Ceiling((double)cumulativeRank / counter));
                                    ClassToAddDTO.ClassRankAverage = result;
                                }
                                ClassToAddDTO.RankResults = rankResults;
                            }
                            history.Add(ClassToAddDTO);
                        }

                    }
                    SuperDetailsToReturn.ClassesHistory = history;
                    SuperDetailsToReturn.FutreClasses = futre;
                    SuperDetailsToReturn.NumOfClass = history.Count;
                    return Content(HttpStatusCode.OK, SuperDetailsToReturn);
                }
                return Content(HttpStatusCode.NoContent, "Sorry invalid details!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Sorry something wrong! " + ex.Message);
            }
        }

        [HttpPut]
        [Route("UpdateClassDetailsSuperStudent")]
        public IHttpActionResult UpdateClassDetailsSuperStudent(ClassDTO ClassDetails)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                if (ClassDetails != null)
                {
                    tblClass ClassToUpdate = db.tblClass.SingleOrDefault(c => c.ClassCode == ClassDetails.ClassCode);
                    if (ClassToUpdate.tblRegisteredTo.Count > 0)
                    {
                        return Content(HttpStatusCode.BadRequest, $"You can not update this  class, because some students are registred in it");
                    }

                    ClassToUpdate.ClassName = ClassDetails.ClassName;
                    ClassToUpdate.ClassDate = ClassDetails.ClassDate;
                    ClassToUpdate.ClassDescription = ClassDetails.ClassDescription;
                    ClassToUpdate.NumOfParticipants = ClassDetails.NumOfParticipants;
                    ClassToUpdate.StartTime = ClassDetails.StartTime;
                    ClassToUpdate.EndTime = ClassDetails.EndTime;

                    if (ClassDetails.Tags != null)
                    {
                        ClassToUpdate.tblTags.Clear();
                        List<tblTags> ListToUpdate = new List<tblTags>();
                        foreach (string TagName in ClassDetails.Tags)
                        {
                            tblTags TagToAdd = db.tblTags.SingleOrDefault(t => t.TagName == TagName);
                            ListToUpdate.Add(TagToAdd);
                        }
                        ClassToUpdate.tblTags = ListToUpdate;
                        db.SaveChanges();
                    }
                    return Content(HttpStatusCode.OK, $"Class with Class Code:{ClassDetails.ClassCode} is Changed successful!");
                }
                return Content(HttpStatusCode.NoContent, "Sorry invalid details for update class!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Sorry something wrong! " + ex.Message);
            }
        }

        [HttpPut]
        [Route("UpdateSuperStudentProfileDescription")]
        public IHttpActionResult UpdateSuperStudentProfileDescription(UpdateProfileSuperStudentDTO UpdateProfile)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                tblStudent SuperStudentToUpdate = db.tblStudent.SingleOrDefault(s => s.Email == UpdateProfile.Email);
                SuperStudentToUpdate.tblSuperStudent.Description = UpdateProfile.Descreption;
                db.SaveChanges();
                return Content(HttpStatusCode.OK, $"Description for Super student with Email:{UpdateProfile.Email} changed successfuly to {UpdateProfile.Descreption}");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Sorry something wrong! " + ex.Message);
            }
        }

        [HttpPost]
        [Route("ShowSuperDetailsById/{id}")]
        public IHttpActionResult ShowSuperDetailsById(string id)
        {
            try
            {
                if (id != null)
                {
                    AppDbContext db = new AppDbContext();
                    tblStudent superStudent = db.tblStudent.SingleOrDefault(s => s.StudentId == id);
                    ShowSuperDetailsByIdDTO studentToReturn = new ShowSuperDetailsByIdDTO()
                    {
                        //from tblStudent
                        StudentId = superStudent.StudentId,
                        FullName = superStudent.FullName,
                        Phone = superStudent.Phone,
                        Email = superStudent.Email,

                        //from tblSuperStudent
                        ImagePath = superStudent.tblSuperStudent.ImagePath,
                        Description = superStudent.tblSuperStudent.Description,
                        DepartmentName = superStudent.tblSuperStudent.DepartmentName,
                        StudyYear = superStudent.tblSuperStudent.StudyYear,
                        NumOfRanks = (int)superStudent.tblSuperStudent.NumOfRanks,
                        RankAverage = (short)superStudent.tblSuperStudent.RankAverage
                    };
                    //if this student not yet classes.
                    if (superStudent.tblSuperStudent.tblClass.Count == 0)
                    {
                        studentToReturn.NumOfClass = 0;
                        studentToReturn.NumOfRanks = 0;
                        studentToReturn.FutreClasses = null;
                        return Content(HttpStatusCode.OK, studentToReturn);
                    }

                    List<ClassDTO> FutreClassesToAdd = new List<ClassDTO>();

                    foreach (tblClass c in superStudent.tblSuperStudent.tblClass)
                    {
                        if (c.ClassDate >= DateTime.Now.Date)
                        {
                            ClassDTO classToAdd = new ClassDTO()
                            {
                                ClassCode = c.ClassCode,
                                ClassDate = (DateTime)c.ClassDate,
                                StartTime = (TimeSpan)c.StartTime,
                                EndTime = (TimeSpan)c.EndTime,
                                ClassName = c.ClassName,
                                ClassDescription = c.ClassDescription,
                                SuperStudentId = c.tblSuperStudent.tblStudent.StudentId,
                                NumOfParticipants = (short)c.NumOfParticipants,
                                SuperName = c.tblSuperStudent.tblStudent.FullName,
                                SuperStudentRank = (short)superStudent.tblSuperStudent.RankAverage
                            };
                            List<string> TagsInClass = new List<string>();

                            foreach (tblTags tag in c.tblTags)
                            {
                                TagsInClass.Add(tag.TagName);
                            }
                            classToAdd.Tags = TagsInClass;
                            FutreClassesToAdd.Add(classToAdd);
                        }

                    }
                    studentToReturn.FutreClasses = FutreClassesToAdd;
                    studentToReturn.NumOfClass = studentToReturn.FutreClasses.Count;
                    return Content(HttpStatusCode.OK, studentToReturn);
                }
                return Content(HttpStatusCode.NoContent, "Sorry invalid id number. please try again.");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, $"Sorry something wrong!! {ex.Message}");
            }
        }
    }
}