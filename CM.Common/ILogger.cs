namespace CM.Common
{
    public interface ILogger
    {
        void Info(string message, params object[] formatArgs);
        void Error(string message, params object[] formatArgs);
    }
}