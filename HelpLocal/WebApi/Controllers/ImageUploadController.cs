using DATA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebApi.Controllers
{
    [RoutePrefix("Files")]
    public class ImageUploadController : ApiController
    {
        [HttpPost]
        [Route("UploadImage")]
        public Task<HttpResponseMessage> UploadImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count < 1)
                {
                    return Task.FromResult(Request.CreateResponse(HttpStatusCode.BadRequest));
                }

                //length always 12 characters
                string dateTime = GetDateTime();
                string finalyName = "";
                string fileName = "";
                foreach (string file in httpRequest.Files)
                {
                    //ProfileImage-205648948.jpg
                    fileName = file;
                    string[] splitFileName = fileName.Split('.');
                    //ProfileImage-205648948
                    string name = splitFileName[0];
                    //jpg
                    string extension = splitFileName[1];
                    //finally name for file
                    finalyName = name + dateTime + '.' + extension;
                    var postedFile = httpRequest.Files[file];
                    var filePath = HttpContext.Current.Server.MapPath("~/ImageFiles/" + postedFile.FileName.Replace(postedFile.FileName, finalyName));
                    //update
                    postedFile.SaveAs(filePath);
                }
                return Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, finalyName));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Request.CreateResponse(HttpStatusCode.BadRequest, ex.InnerException));
            }
        }

        [HttpPost]
        [Route("UpdateImage")]
        public Task<HttpResponseMessage> UpdateImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count < 1)
                {
                    return Task.FromResult(Request.CreateResponse(HttpStatusCode.BadRequest));
                }
                string dateTime = GetDateTime();
                //file name to update to ImageFiels.
                string fileName = "";
                //full path of ImageFiles.
                var fullPathImageFiles = HttpContext.Current.Server.MapPath("~/ImageFiles/");
                //get all jpg files in ImageFiels directory in server.
                string[] allJpgFilesPath = Directory.GetFiles(fullPathImageFiles, "*.jpg");
                foreach (string newFileFromClient in httpRequest.Files)
                {
                    fileName = newFileFromClient;
                    //varible to insert the file.
                    var postedFile = httpRequest.Files[newFileFromClient];
                    //check if this super student have a profile picture.
                    foreach (string existsFile in allJpgFilesPath)
                    {
                        FileInfo existsFileInfo = new FileInfo(existsFile);
                        if (newFileFromClient == existsFileInfo.Name.Substring(0,22)+".jpg")
                        {
                            existsFileInfo.Delete();
                            break;
                        }
                    }
                    string [] splitFileName = fileName.Split('.');
                    string finalyPathToSaveImage = splitFileName[0]+ dateTime + "." + splitFileName[1];
                    postedFile.SaveAs(fullPathImageFiles +finalyPathToSaveImage);
                }
                string superId = fileName.Substring(13, 9);
                AppDbContext db = new AppDbContext();
                tblSuperStudent superStudentObj = db.tblSuperStudent.SingleOrDefault(s => s.StudentId == superId);
                superStudentObj.ImagePath = superId +dateTime;
                db.SaveChanges();

                return Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, fileName));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Request.CreateResponse(HttpStatusCode.BadRequest, ex.InnerException));
            }
        }

        [HttpPut]
        [Route("DeleteImage/{studentId}")]
        public Task<HttpResponseMessage> DeleteImage(string studentId)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                bool imageFoundToDelete = false;
                var fullPathImageFiles = HttpContext.Current.Server.MapPath("~/ImageFiles/");
                string[] allJpgFilesPath = Directory.GetFiles(fullPathImageFiles, "*.jpg");
                foreach (string fileFullPath in allJpgFilesPath)
                {
                    FileInfo fileToCheck = new FileInfo(fileFullPath);
                    if (fileToCheck.Name.Substring(13,9)== studentId)
                    {
                        fileToCheck.Delete();
                        tblSuperStudent superStudent = db.tblSuperStudent.SingleOrDefault(s => s.StudentId == studentId);
                        superStudent.ImagePath = "empty";
                        db.SaveChanges();
                        imageFoundToDelete = true;
                        break;
                    }
                }
                if (imageFoundToDelete)
                {
                    return Task.FromResult(Request.CreateResponse(HttpStatusCode.OK,$"Profile image for student with id:{studentId} deleted Successfully"));
                }
                else
                {
                    return Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, $"For student with id:{studentId} ,not have a profile picture"));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(Request.CreateResponse(HttpStatusCode.BadRequest, ex.InnerException));
            }
        }


        //returns date time to string with numbers. 
        private string GetDateTime()
        {
            string currentDateAndTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            string[] separateDateAndTime = currentDateAndTime.Split(' ');
            string[] dateOnly = separateDateAndTime[0].Split('/');
            string[] timeOnly = separateDateAndTime[1].Split(':');

            string dateWithNumbers = "";
            foreach (string item in dateOnly)
            {
                dateWithNumbers += item;
            }
            foreach (string item2 in timeOnly)
            {
                dateWithNumbers += item2;
            }
            return dateWithNumbers;
        }
    }
}
