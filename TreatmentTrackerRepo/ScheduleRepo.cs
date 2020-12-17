using System;
using System.Collections.Generic;
using System.Linq;
using TreatmentTrackerRepo.DataClasses;
using System.Data.Entity;

namespace TreatmentTrackerRepo
{


    public class ScheduleRepo
    {
        public List<Schedule> GetSchedules(int id)
        {
            using (var context = new TreatmentEntities())
            {
                var query = from r in context.schedule_DL
                            join t in context.treatment_DL
                            on r.treatment_id equals t.id
                            where r.id >= id
                            select new Schedule
                            {
                                Id = r.id,
                                TreatmentTime = (DateTime)r.administered_date,
                                TreatmentId = (int)r.treatment_id,
                                TreatmentName = (string)t.treatment_description,
                                Administered = (bool)r.administered
                            };
                return query.ToList();
            }
        }
        public List<Schedule> GetSchedules(DateTime startDate, DateTime endDate)
        {
            using (var context = new TreatmentEntities())
            {
                var query = from r in context.schedule_DL
                            join t in context.treatment_DL
                            on r.treatment_id equals t.id
                            where r.administered_date >= startDate
                            select new Schedule
                            {
                                Id = r.id,
                                TreatmentTime = (DateTime)r.administered_date,
                                TreatmentId = (int)r.treatment_id,
                                TreatmentName = (string)t.treatment_description,
                                Administered = (bool)r.administered
                            };
                return query.ToList();
            }
        }
        public List<Schedule> GetSchedules(DateTime startDate, string treatmentName)
        {
            using (var context = new TreatmentEntities())
            {
                var query = from r in context.schedule_DL
                            join t in context.treatment_DL
                            on r.treatment_id equals t.id
                            where t.treatment_description.Contains(treatmentName) &&
                                DateTime.Compare((DateTime)r.administered_date, startDate) >= 0
                            select new Schedule
                            {
                                Id = r.id,
                                TreatmentTime = (DateTime)r.administered_date,
                                TreatmentId = (int)r.treatment_id,
                                TreatmentName = (string)t.treatment_description,
                                Administered = (bool)r.administered
                            };
                return query.ToList();
            }
        }
        public List<Treatment> GetTreatments()
        {
            using (var context = new TreatmentEntities())
            {
                var query = context.treatment_DL.Select(r => new Treatment
                {
                    Id = r.id,
                    Description = r.treatment_description,
                    Frequency = (decimal) r.every_x_hours,
                    MaxPerDay = (int) r.max_per_day,
                    StartTimeTicks = r.start_time * 10000000 ?? 0
                });
                var returnList = query.ToList();
                returnList.ForEach(t =>
                    t.StartTime = new TimeSpan(t.StartTimeTicks));
                return returnList;
            }
        }

        public void AdministerTreatment(int ID)
        {
            Schedule schedule = GetSchedules(ID).First();


            using (var context = new TreatmentEntities())
            {
                var result = context.schedule_DL.SingleOrDefault(s => s.id == ID);
                if (result != null)
                {
                    result.administered = true;
                    result.administered_date = DateTime.Now;
                    context.SaveChanges();
                }
            }
        }

        public void WriteSchedule()
        {
            List<Schedule> outputSchedule = new List<Schedule>();

            //required starting information.
            DateTime now = DateTime.Now;
            List<Treatment> treatment_DL = GetTreatments();

            //no reason to care. We can just rebuild.
            GetSchedules(now.AddDays(-7), now.AddDays(2))
                .Where(s => s.Administered == false)
                .ToList()
                .ForEach(s => DeleteSchedule(s.Id));

            //build out schedule for each treatment.
            foreach (Treatment t in treatment_DL)
            {
                //this is our starting cursor
                DateTime cursor = now;

                // if below minimum start time, let's bump it up to minimum start time.
                if (DateTime.Compare(cursor, DateTime.Now.Date.Add(t.StartTime)) < 0)
                {
                    cursor = DateTime.Now.Date.Add(t.StartTime);
                }

                // let's see when it was last given
                Schedule lastAdministeredNullable = GetSchedules(DateTime.Now.AddDays(-7), t.Description)
                                                .Where(s => s.Administered == true)
                                                .OrderByDescending(s => s.TreatmentTime)
                                                .FirstOrDefault();


                DateTime lastAdministered = lastAdministeredNullable == null ? DateTime.Now.AddDays(-8) : lastAdministeredNullable.TreatmentTime;

                // if administered too recently, bump it out appropriately
                if (DateTime.Compare(cursor, lastAdministered.AddMinutes((double)(t.Frequency * 60))) < 0)
                {
                    cursor = lastAdministered.AddMinutes((double)(t.Frequency * 60));
                }

                //how many treatment_DL have we done today?
                int completedtreatment_DL = GetSchedules(DateTime.Now.Date, t.Description)
                    .Count(s => s.Administered == true);


                while (cursor < now.Date.AddHours(30) && completedtreatment_DL < t.MaxPerDay)
                {
                    outputSchedule.Add(new Schedule
                    {
                        Id = -1,
                        TreatmentTime = cursor,
                        TreatmentId = t.Id,
                        Administered = false
                    });
                    cursor = cursor.AddMinutes(Convert.ToDouble(t.Frequency * 60));
                    completedtreatment_DL++;
                }
            }

            using (var context = new TreatmentEntities())
            {
                foreach (Schedule s in outputSchedule)
                {
                    context.schedule_DL.Add(new schedule_DL
                    {
                        administered = s.Administered,
                        treatment_id = s.TreatmentId,
                        administered_date = s.TreatmentTime
                    });
                }
                context.SaveChanges();
            }
        }

        public void DeleteSchedule(int id)
        {
            using (var context = new TreatmentEntities())
            {
                var forRemoving = context.schedule_DL.Find(id);
                if (forRemoving != null && forRemoving.administered == true)
                {
                    throw new InvalidOperationException("Cannot delete record when treatment has been administered");
                }

                if (forRemoving != null) context.schedule_DL.Remove(forRemoving);
                context.SaveChanges();
            }
        }
    }
}
