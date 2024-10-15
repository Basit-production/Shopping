namespace Ahmed_mart.Helpers.v1
{
    public static class InnerExceptionsHelper
    {
        public static IEnumerable<Exception> GetAllInnerExceptions(Exception ex)
        {
            var innerException = ex.InnerException;
            while (innerException != null)
            {
                yield return innerException;
                innerException = innerException.InnerException;
            }
        }
    }
}
