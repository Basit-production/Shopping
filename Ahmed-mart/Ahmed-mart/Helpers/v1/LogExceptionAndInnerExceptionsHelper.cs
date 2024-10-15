using Ahmed_mart.Repository.v1;
using System.Net;

namespace Ahmed_mart.Helpers.v1
{
    public static class LogExceptionAndInnerExceptionsHelper
    {
        public static ServiceResponse<T> LogExceptionAndInnerExceptions<T>(ILogger logger, Exception ex, HttpStatusCode statusCode, string defaultMessage = "An error occurred.")
        {
            // Log the exception with stack trace
            logger.LogError(ex, $"An unhandled exception occurred: {ex}");

            // Log inner exceptions with stack trace and type information
            foreach (var innerException in InnerExceptionsHelper.GetAllInnerExceptions(ex))
            {
                logger.LogError(innerException, $"Inner Exception Type: {innerException.GetType().FullName}");
            }

            // Create a ServiceResponse<T> with the exception details
            var errorResponse = new ServiceResponse<T>
            {
                Data = default,
                Success = false,
                StatusCode = statusCode,
                Message = $"{defaultMessage}",
                Errors = { ex.Message }
            };

            // Include inner exceptions in the error response
            foreach (var innerException in InnerExceptionsHelper.GetAllInnerExceptions(ex))
            {
                errorResponse.Errors.Add(innerException.Message);
            }

            return errorResponse;
        }
    }
}
