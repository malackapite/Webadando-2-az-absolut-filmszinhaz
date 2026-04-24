namespace Backend
{
    public static class Utils
    {
        public static string GetInnerMostExceptionMessage(this Exception exception) => exception.InnerException?.GetInnerMostExceptionMessageInternal() ?? exception.Message;

        static string GetInnerMostExceptionMessageInternal(this Exception innerException)
        {
            while (innerException.InnerException is not null)
            {
                innerException = innerException.InnerException;
            }
            return innerException.Message;
        }
    }
}
