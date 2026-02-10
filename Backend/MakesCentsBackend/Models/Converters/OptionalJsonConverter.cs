/*
 * Gianna Ross
 * Makes Cents
 * Sources: https://chatgpt.com/c/697ef15b-bf50-832d-87fb-4b27cae268fd
 */
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MakesCentsBackend.Models.Converters
{
    /// <summary>
    /// JSON converter for Optional<T>.
    /// JSON converter for Optional<T>.
    /// Handles serialization and deserialization of Optional<T> so that:
    /// - HasValue = false when property is missing in JSON
    /// - HasValue = true when property is present (even if null)
    /// </summary>
    /// <typeparam name="T">The type of the value wrapped in Optional, e.g., decimal?, string, etc.</typeparam>
    public class OptionalJsonConverter<T> : JsonConverter<Optional<T>>
    {
        /// <summary>
        /// Deserialize JSON into Optional<T>
        /// </summary>
        /// <param name="reader">
        /// The Utf8JsonReader positioned at the value to deserialize.
        /// It could be a number, string, null, etc.
        /// </param>
        /// <param name="typeToConvert">
        /// The type being deserialized.
        /// For this converter, it will always be Optional<T>.
        /// </param>
        /// <param name="options">
        /// The JsonSerializerOptions in effect, which may include other converters, naming policies, etc.
        /// </param>
        /// <returns>
        /// An Optional<T> object:
        /// - HasValue = true if JSON property was present
        /// - Value = deserialized value (or null if JSON explicitly null)
        /// </returns>
        public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Deserialize the JSON value into T? using the default serializer
            // Handles null automatically
            T? value = JsonSerializer.Deserialize<T>(ref reader, options);

            // Wrap it in Optional<T>. HasValue = true because the property was present in JSON
            return new Optional<T>(value);
        }

        /// <summary>
        /// Serialize Optional<T> into JSON
        /// </summary>
        /// <param name="writer">
        /// Utf8JsonWriter to write JSON output.
        /// </param>
        /// <param name="value">
        /// The Optional<T> object being serialized.
        /// - HasValue = false → property missing
        /// - HasValue = true → property present (Value may be null or actual value)
        /// </param>
        /// <param name="options">
        /// Current JsonSerializerOptions, which may contain naming policies, converters, etc.
        /// </param>
        public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                // Property was present in the model
                // Serialize its actual value
                // If value.Value is null, JSON will contain "null"
                JsonSerializer.Serialize(writer, value.Value, options);
            }
        }
    }

    /// <summary>
    /// Generic factory that dynamically provides OptionalJsonConverter<T> for any Optional<T>.
    /// This is needed because System.Text.Json cannot automatically handle generic converters.
    /// </summary>
    public class OptionalJsonConverterFactory : JsonConverterFactory
    {
        /// <summary>
        /// Determines whether this factory can create a converter for a given type.
        /// </summary>
        /// <param name="typeToConvert">
        /// The type being converted.
        /// Return true if it is Optional<T> for some T.
        /// </param>
        /// <returns>
        /// True if the type is Optional<T>, otherwise false.
        /// </returns>
        public override bool CanConvert(Type typeToConvert)
        {
            // Check if the type is generic (e.g., Optional<decimal?>)
            // and its generic type definition is Optional<>
            return typeToConvert.IsGenericType
                   && typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>);
        }

        /// <summary>
        /// Create a JsonConverter for a specific Optional<T> type
        /// </summary>
        /// <param name="typeToConvert">
        /// The specific type to convert, e.g., Optional<decimal?> or Optional<string>.
        /// </param>
        /// <param name="options">
        /// Current JsonSerializerOptions in use.
        /// </param>
        /// <returns>
        /// A JsonConverter instance capable of converting Optional<T> for the specific T.
        /// </returns>
        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            // Extract T from Optional<T>
            Type valueType = typeToConvert.GetGenericArguments()[0];

            // Construct OptionalJsonConverter<T> for that specific T
            Type converterType = typeof(OptionalJsonConverter<>).MakeGenericType(valueType);

            // Use reflection to instantiate the converter
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }
    }
}
