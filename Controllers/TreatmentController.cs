using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TreatmentTrackerRepo;
using TreatmentTrackerRepo.DataClasses;

namespace TreatmentTracker.Controllers
{
    public class TreatmentController : ApiController
    {
        private readonly ScheduleRepo _scheduler = new ScheduleRepo();

        public IEnumerable<Treatment> Get()
        {
            return _scheduler.GetTreatments();
        }

        public HttpResponseMessage Put(int id)
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
