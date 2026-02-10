/*
 * Gianna Ross
 * Makes Cents
 * Sources: https://chatgpt.com/c/697ef15b-bf50-832d-87fb-4b27cae268fd (Optional<T>)
 */
namespace MakesCentsBackend.Models
{
    /// <summary>
    /// A base response model with only a status and message
    /// </summary>
    public class BaseResponse
    {
        // Class Level Properties
        public int HttpStatus { get; set; } = 0;
        public string Message { get; set; } = "";

        /// <summary>
        /// Parameterized constructor for a base response
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        public BaseResponse(int httpStatus, string message)
        {
            HttpStatus = httpStatus;
            Message = message;
        }
    }

    /// <summary>
    /// Base Response for a create or edit function
    /// </summary>
    public class BaseIdResponse : BaseResponse
    {
        // Class Level Properties
        public int Id { get; set; } = -1;

        /// <summary>
        /// Parameterized constructor for BaseIdResponse
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        /// <param name="id"></param>
        public BaseIdResponse(int httpStatus, string message, int? id) : base(httpStatus, message)
        {
            if (id != null)
            {
                Id = id.Value;
            }
            else
            {
                Id = -1;
            }
        }

        /// <summary>
        /// Parameterized constructor for BaseIdResponse
        /// </summary>
        /// <param name="httpStatus"></param>
        /// <param name="message"></param>
        /// <param name="id"></param>
        public BaseIdResponse(int httpStatus, string message) : base(httpStatus, message)
        {
            Id = -1;
        }
    }

    /// <summary>
    /// Optional class for creating and updating nulls
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct Optional<T>
    {
        /// <summary>
        /// Checks if the value was sent
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// The value that was sent
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// Parameterized constructor to set Value
        /// </summary>
        /// <param name="value"></param>
        public Optional(T? value)
        {
            HasValue = true;
            Value = value;
        }

        /// <summary>
        /// Allows Optional<decimal?> amount = 100m instead of amount = new Optional<decimal?>(100m)
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Optional<T>(T? value) => new(value);
    }

}
