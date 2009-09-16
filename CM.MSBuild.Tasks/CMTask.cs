using System;
using System.Collections;
using Microsoft.Build.Utilities;

namespace CM.MSBuild.Tasks
{
    public abstract class CMTask : Task
    {
        private bool useDebugLogger = false;
        private ArrayList debugLog = new ArrayList();
        
        protected abstract void DoExecute();
        
        public override bool Execute()
        {
            try
            {
                DoExecute();
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
            return true;
        }
        
        public void UseDebugLog()
        {
            useDebugLogger = true;
        }
        
        public ArrayList DebugLog
        {
            get { return debugLog; }   
        }
        
        protected virtual void LogMessage(string message, params object[] messageArgs)
        {
            if (useDebugLogger)
            {
                debugLog.Add(string.Format(message, messageArgs));
            }
            else
            {
                Log.LogMessage(message, messageArgs);
            }
        }
        
        protected virtual void LogException(Exception e)
        {
            if (useDebugLogger)
            {
                debugLog.Add(e.Message);
            }
            else
            {
                Log.LogErrorFromException(e);
            }
        }

        protected virtual void LogWarning(string message, params object[] messageArgs)
        {
            if (useDebugLogger)
            {
                debugLog.Add(string.Format(message, messageArgs));
            }
            else
            {
                Log.LogWarning(message, messageArgs);
            }
        }
    }
}