namespace Socneto.Api.Models
{
    public class SuccessResponse
    {
        public bool Success { get; set; }

        public static SuccessResponse True()
        {
            return new SuccessResponse { Success = true };
        }

        public static SuccessResponse False()
        {
            return new SuccessResponse {Success = false };
        }
    }
}