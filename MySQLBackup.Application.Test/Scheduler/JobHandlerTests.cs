using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quartz;
using Quartz.Impl;

namespace MySQLBackup.Application.Scheduler.Tests
{
    [TestClass()]
    public class JobHandlerTests
    {
        [TestMethod()]
        public void ScheduleJobsTest()
        {
            using (JobHandler jobHandler = new JobHandler())
            {
                jobHandler.ScheduleJobs();
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                Assert.IsTrue(scheduler.CheckExists(new JobKey("Delete_Old_Job", "Cleanup")));
            }
        }
    }
}
