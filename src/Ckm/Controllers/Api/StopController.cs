namespace Ckm.Controllers.Web
{
    using Ckm.Models;
    using Ckm.ViewModels;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Extensions.Logging;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    [Authorize]
    [Route("api/trips/{tripName}/stops")]
    public class StopController : Controller
    {
        private readonly CoordService coordService;
        private readonly ILogger<TripController> logger;
        private readonly ICkmRepo repo;

        public StopController(ICkmRepo repo, ILogger<TripController> logger, CoordService coordService)
        {
            this.repo = repo;
            this.logger = logger;
            this.coordService = coordService;
        }

        [HttpGet("")]
        public JsonResult Get(string tripName)
        {
            try
            {
                var trip =
                    repo
                    //.GetTripByName(tripName, User.Identity.Name);
                    .GetTripByName(tripName, User.Identity.Name);

                if (trip == null)
                {
                    return Json(null);
                }

                var stops =
                    Startup
                    .Mapper
                    .Map<IEnumerable<Stop>, IEnumerable<StopViewModel>>(
                        trip.Stops.OrderBy(x => x.Order).ToList()
                    );

                return Json(stops);
            }
            catch (Exception ex)
            {
                logger.LogError($"failed to get stops for trip {tripName} ", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Error occurred finding trip name");
            }
        }

        [HttpPost("")]
        public async Task<JsonResult> Post(string tripName, [FromBody]StopViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // map to entity
                    var newStop = Startup.Mapper.Map<Stop>(vm);

                    // lookup geos
                    var coordResult = await this.coordService.Lookup(newStop.Name);

                    if (!coordResult.Success)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json(coordResult);
                    }

                    newStop.Longitude = coordResult.Longitude;
                    newStop.Latitude = coordResult.Latitude;

                    // save to db
                    var userName = User.Identity.Name;
                    repo.AddStop(tripName, userName, newStop);

                    if (repo.SaveAll())
                    {
                        Response.StatusCode = (int)HttpStatusCode.OK;
                        return Json(Startup.Mapper.Map<StopViewModel>(newStop));
                    }
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to save new stop");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Failed to save new Stop");
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json("Validation failed on new Stop");
        }
    }
}