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
    [RoutePrefix("city")]
    public class CityController : ApiController
    {
        [HttpGet]
        [Route("getAll")]
        public IHttpActionResult getAll()
        {
            try
            {
                AppDbContext db = new AppDbContext();
                List<CityDTO> cities = new List<CityDTO>();
                if (db.tblCity != null)
                {
                    foreach (tblCity c in db.tblCity)
                    {
                        CityDTO cd = new CityDTO
                        {
                            CityName = c.CityName
                        };
                        cities.Add(cd);
                    }
                    return Content(HttpStatusCode.OK, cities);
                }
                return Content(HttpStatusCode.NotFound, "Sorry the tblCity is empty!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Something wrong! " + ex.Message);
            }
        }
        [HttpGet]
        [Route("getCitiesByInput/{input}")]
        public IHttpActionResult getCitiesByInput(string input)
        {
            try
            {
                AppDbContext db = new AppDbContext();
                List<CityDTO> cities = new List<CityDTO>();
                if (db.tblCity != null)
                {
                    foreach (tblCity c in db.tblCity)
                    {
                        if (input != "" && c.CityName.Contains(input))
                        {
                            CityDTO cd = new CityDTO
                            {
                                CityName = c.CityName
                            };
                            cities.Add(cd);
                        }
                    }
                    return Content(HttpStatusCode.OK, cities);
                }
                return Content(HttpStatusCode.NotFound, "Sorry the tblCity is empty!");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Something wrong! " + ex.Message);
            }
        }
    }
}