using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using TreatmentTrackerRepo;
using TreatmentTrackerRepo.DataClasses;

namespace TreatmentTracker
{
    public partial class Default : System.Web.UI.Page
    {
        readonly ScheduleRepo _scheduler = new ScheduleRepo();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (awake.Value != "true") return;
            _scheduler.WriteSchedule();
            BindData();
        }

        protected void awake_onclick(object sender, EventArgs e)
        {
            awake.Value = "true";
            _scheduler.WriteSchedule();
            BindData();
        }
        protected void asleep_onclick(object sender, EventArgs e)
        {
            awake.Value = "false";

            foreach (var id in _scheduler.GetSchedules(DateTime.Now.AddDays(-1)
                                            , DateTime.Now.AddDays(1)).Where(s => s.Administered == false).ToList())
            {
                _scheduler.DeleteSchedule(id.Id);
                BindData();
            }
        }
        private void BindData()
        {
            List<Schedule> data = _scheduler.GetSchedules(DateTime.Now.AddMinutes(-1), DateTime.Now.AddDays(1));
            rptSchedule.DataSource = data.Where(s => s.Administered == false).OrderBy(s => s.TreatmentTime);
            rptSchedule.DataBind();
        }

        protected void addTreatment_onclick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}