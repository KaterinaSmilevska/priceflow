using PriceFlowApp.Exceptions;

namespace PriceFlowApp.Helpers
{
    public static class ValidationHelper
    {
        public static void ValidateRequiredField(string value, string fieldName, string errorCode)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ValidationException(errorCode, $"{fieldName} cannot be null or empty.");
        }
    }
}
