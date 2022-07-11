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
    [RoutePrefix("RegisteredTo")]
    public class RegisteredToController : ApiController
    {
        [HttpPut]
        [Route("StudentRank")]
        public IHttpActionResult StudentRank(RankDetailsDTO DetailsFromClient)
        {
            try
            {
                if (DetailsFromClient != null)
                {
                    AppDbContext db = new AppDbContext();
                    //stop procces if the class is future.
                    int ClassCode = DetailsFromClient.ClassCode;
                    tblClass CheckClass = db.tblClass.SingleOrDefault(c => c.ClassCode == ClassCode);
                    if (CheckClass.ClassDate > DateTime.Now)
                    {
                        return Content(HttpStatusCode.BadRequest, "It is not possible to rank a future class.");
                    }
                    //Convert the Rank from 20,40,60,80,100 to 1,2,3,4,5.
                    short StudentRank = (short)((DetailsFromClient.RankValue / 10) / 2);
                    // get RegisteredTo object by {StudentId,ClassCode}.
                    string StudentId = DetailsFromClient.StudentId;                    
                    string RankDescription = DetailsFromClient.RankDescription;
                    tblRegisteredTo registeredToUpdate = db.tblRegisteredTo.SingleOrDefault(r => r.ClassCode == ClassCode && r.StudentId == StudentId);
                    //Stop the process if the student has made a ranking for this Class.
                    if (registeredToUpdate.StudentRank != 0)
                    {
                        return Content(HttpStatusCode.BadRequest, "This class has been ranked in the past");
                    }
                    //Update registeredTo table.                    
                    registeredToUpdate.StudentRank = StudentRank;
                    if (RankDescription.Length == 0)
                    {
                        registeredToUpdate.RankDescription = ".מצטערים, תוכן הדירוג לא צויין";
                    }
                    else
                    {
                        registeredToUpdate.RankDescription = RankDescription;
                    }

                    //Update super student.
                    tblSuperStudent SuperToUpdate = db.tblClass.SingleOrDefault(c => c.ClassCode == ClassCode).tblSuperStudent;
                    int cumulativeRank = (int)SuperToUpdate.CumulativeRank + StudentRank;
                    SuperToUpdate.CumulativeRank = cumulativeRank;
                    // +1 to num of ranks.                     
                    int numOfRanks = (int)SuperToUpdate.NumOfRanks+1;
                    SuperToUpdate.NumOfRanks = numOfRanks;
                    //Calculating the average
                    //Math.Ceiling = Round up for example: 1.6=>2 | 1.4=>2
                    short resultRankAverage = Convert.ToInt16(Math.Ceiling((double)cumulativeRank / numOfRanks));
                    //update RankAverage
                    SuperToUpdate.RankAverage = resultRankAverage;
                    db.SaveChanges();
                    return Content(HttpStatusCode.OK, "The update was completed successfully");
                }                
                return Content(HttpStatusCode.BadRequest, "Invalid Details.");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, $"Sorry something wrong. Exception message:{ex.Message}");
            }
        }
    }
}
