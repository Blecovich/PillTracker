using System;
using TreatmentTrackerRepo;
using TreatmentTrackerRepo.DataClasses; 

namespace TreatmentTracker
{
    public partial class API : System.Web.UI.Page
    {
        readonly ScheduleRepo _scheduler = new ScheduleRepo();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["action"] != "administer") return;
            try
            {
                Response.Write(AdministerTreatment(Convert.ToInt32((Request.QueryString["id"]))));
            }
            catch (Exception exc)
            {
                Response.Write(exc.Message);
            }
        }

        protected string AdministerTreatment(int id)
        {
            try
            {
                _scheduler.AdministerTreatment(id);  
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "Success";
        }
    }
}