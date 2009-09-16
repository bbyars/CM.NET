using System.Collections.Generic;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace CM.Nant.Tasks
{
    [TaskName("register")]
    public class RegisterTask : Task
    {
        public static List<Registration> Registrations = new List<Registration>();

        [TaskAttribute("baseTarget")]
        public string BaseTarget { get; set; }

        [TaskAttribute("filter")]
        public Filter Filter { get; set; }

        [TaskAttribute("addTarget")]
        public string AddTarget { get; set; }

        protected override void ExecuteTask()
        {
            Registrations.Add(new Registration(BaseTarget, Filter, AddTarget));
        }
    }
}