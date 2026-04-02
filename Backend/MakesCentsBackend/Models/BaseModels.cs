/*
 * Gianna Ross
 * Makes Cents
 * Sources: https://chatgpt.com/c/697ef15b-bf50-832d-87fb-4b27cae268fd (Optional<T>)
 */
using TypeGen.Core.TypeAnnotations;

namespace MakesCentsBackend.Models
{
    /// <summary>
    /// A base response model with only a status and message
    /// </summary>
    [ExportTsInterface]
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

        /// <summary>
        /// Default constructor for a base response
        /// </summary>
        public BaseResponse() { }
    }

    /// <summary>
    /// Base Response for a create or edit function
    /// </summary>
    [ExportTsInterface]
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

        /// <summary>
        /// Default constructor for a base Id response
        /// </summary>
        public BaseIdResponse() : base() { }
    }


    [ExportTsClass]
    public interface IOptional
    {
        bool HasValue { get; }
        object? GetValue();
    }

    /// <summary>
    /// Optional class for creating and updating nulls
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ExportTsClass]
    public readonly struct Optional<T> : IOptional
    {
        /// <summary>
        /// Checks if the value was sent
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// The value that was sent
        /// </summary>
        [TsType("T | null")]
        public T? Value { get; }

        public object? GetValue() => Value;

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
        /// Parameterized constructor to bring in a HasValue value
        /// </summary>
        public Optional(bool hasValue)
        {
            HasValue = hasValue;
        }

        /// <summary>
        /// Parameterized constructor to bring in a HasValue and Value values
        /// </summary>
        public Optional(bool hasValue, T? value)
        {
            HasValue = hasValue;
            Value = value;
        }

        /// <summary>
        /// Allows Optional<decimal?> amount = 100m instead of amount = new Optional<decimal?>(100m)
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Optional<T>(T? value) => new(value);
    }

    public class BaseIdRequest
    {
        // Class properties
        public int UserId { get; set; } = 0;
        public int EntityId { get; set; } = 0;

        public BaseIdRequest() { }

        public BaseIdRequest(int userId, int entityId)
        {
            UserId = userId;
            EntityId = entityId;
        }
    }
}
