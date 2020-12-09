using DadOrganizerRepo;
using DadOrganizerRepo.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DadOrganizer
{
    public partial class Default : System.Web.UI.Page
    {
        ScheduleRepo scheduler = new ScheduleRepo();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (awake.Value == "true") {
                scheduler.WriteSchedule();
                BindData();
            }
        }

        protected void awake_onclick(object sender, EventArgs e)
        {
            awake.Value = "true";
            scheduler.WriteSchedule();
            BindData();
        }
        protected void asleep_onclick(object sender, EventArgs e)
        {
            awake.Value = "false";

            foreach (var id in scheduler.GetSchedules(DateTime.Now.AddDays(-1)
                                            , DateTime.Now.AddDays(1)).Where(s => s.Administered == false).ToList())
            {

                scheduler.DeleteSchedule(id.Id);
                BindData();
            }
        }
        private void BindData()
        {
            List<Schedule> data = scheduler.GetSchedules(DateTime.Now.AddMinutes(-1), DateTime.Now.AddDays(1));
            rptSchedule.DataSource = data.Where(s => s.Administered == false).OrderBy(s => s.TreatmentTime);
            rptSchedule.DataBind();
        }

        protected void addTreatment_onclick(object sender, EventArgs e)
        {

        }
    }
}