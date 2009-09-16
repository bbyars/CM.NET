using System.Linq;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace CM.Nant.Tasks
{
    [TaskName("call-registered")]
    public class CallRegisteredTask : Task
    {
        protected override void ExecuteTask()
        {
            Execute(Filter.Before);
            Execute(Filter.During);
            Execute(Filter.After);
        }

        private void Execute(Filter filter)
        {
            var targets = RegisterTask.Registrations.Where(r => r.Matches(Project.CurrentTarget.Name, filter)).ToList();
            targets.ForEach(target => Project.Execute(target.AddTarget, true));
        }
    }
}