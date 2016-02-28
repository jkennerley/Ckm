namespace Ckm.Controllers.Web
{
    using AutoMapper;
    using Ckm.Models;
    using Ckm.ViewModels;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    [Authorize]
    [Route("api/trips")]
    public class TripController : Controller
    {
        private readonly ILogger<TripController> logger;
        private readonly ICkmRepo repo;

        public TripController(ICkmRepo repo, ILogger<TripController> logger)
        {
            this.repo = repo;
            this.logger = logger;
        }

        [HttpGet("")]
        public JsonResult Get()
        {
            // Code
            var tripsAll =
                repo
                .GetAllTripsWithStops()
                .OrderBy(x => x.UserName)
                .ToList();

            var trips =
                repo
                .GetUserTripsWithStops(User.Identity.Name)
                .OrderBy(x => x.UserName)
                .ToList();

            var vms =
                Mapper
                .Map<IEnumerable<TripViewModel>>(
                    trips);

            //IMapper mapper = config.CreateMapper();
            //var source = new Source();
            //var dest = mapper.Map<Source, Dest>(source);
            //Startup.

            // Return
            return Json(vms);
        }

        [HttpPost("")]
        public JsonResult Post([FromBody] TripViewModel vm)
        {
            #region

            //{
            //"id": 1,
            //"name": "US Trip",
            //"created": "2016-02-14T14:38:51.2032826",
            //"username": "",
            //"stops": null
            //}

            #endregion

            try
            {
                if (ModelState.IsValid)
                {
                    var trip = Mapper.Map<Trip>(vm);

                    trip.UserName = User.Identity.Name;

                    logger.LogInformation("attempting to save new trip");

                    repo.AddTrip(trip);

                    if (repo.SaveAll())
                    {
                        Response.StatusCode = (int)HttpStatusCode.Created;

                        return Json(Mapper.Map<TripViewModel>(trip));
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json(new { ok = false, ModelState = ModelState, trip = vm });
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { ok = false, ModelState = ModelState, trip = vm });
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to save new trip", ex.Message);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { ok = false });
            }
        }
    }
}