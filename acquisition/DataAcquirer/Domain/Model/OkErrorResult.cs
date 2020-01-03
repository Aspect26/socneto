using System;

namespace Domain.Model
{
    public class OkErrorResult
    {

        public bool IsSuccessful { get; }
        public string ErrorMessage { get; }
        private OkErrorResult()
        {
            IsSuccessful = true;
            ErrorMessage = null;
        }

        public OkErrorResult(string error)
        {
            IsSuccessful = false;
            ErrorMessage = error;
        }

        public static OkErrorResult Successful()
        {
            return new OkErrorResult();
        }
        public static OkErrorResult Error(string error)
        {
            error = error ?? throw new ArgumentNullException(nameof(error));
            return new OkErrorResult(error);
        }
    }
}
