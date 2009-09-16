using System;

namespace CM.Nant.Tasks
{
    public class Registration
    {
        public string BaseTarget;
        public Filter Filter;
        public string AddTarget;

        public Registration(string target, Filter filter, string advice)
        {
            BaseTarget = target;
            Filter = filter;
            AddTarget = advice;
        }

        public bool Matches(string target, Filter filter)
        {
            return BaseTarget.Equals(target, StringComparison.InvariantCultureIgnoreCase)
                   && Filter == filter;
        }
    }
}