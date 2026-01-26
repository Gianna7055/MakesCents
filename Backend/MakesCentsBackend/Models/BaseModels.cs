namespace MakesCentsBackend.Models
{
    /// <summary>
    /// A base response model with only a status and message
    /// </summary>
    public class BaseResponse
    {
        // CLass Level Properties
        public int Status { get; set; } = 0;
        public string Message { get; set; } = "";

        /// <summary>
        /// Parameterized constructor for a base response
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        public BaseResponse(int status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}
