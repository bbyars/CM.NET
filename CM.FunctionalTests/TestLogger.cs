using System;
using System.Collections.Generic;
using CM.Common;

namespace CM.FunctionalTests
{
    public class TestLogger : ILogger
    {
        private readonly List<string> logs = new List<string>();

        public void Info(string message, params object[] formatArgs)
        {
            logs.Add("INFO: " + string.Format(message, formatArgs));
        }

        public void Error(string message, params object[] formatArgs)
        {
            logs.Add("ERROR: " + string.Format(message, formatArgs));
        }

        public string Contents
        {
            get { return string.Join(Environment.NewLine, logs.ToArray()); }
        }
    }
}