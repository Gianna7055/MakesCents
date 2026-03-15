using System.Text.Json;
using System.Text.Json.Serialization;

namespace MakesCentsBackend.Models.Converters
{
    /// <summary>
    /// A generic JSON converter that safely deserializes a property.
    /// Returns the default value if the input cannot be converted to the target type.
    /// </summary>
    public class SafeDefaultConverter<T> : JsonConverter<T>
    {
        /// <summary>
        /// Reads JSON and converts it to the target type.
        /// Returns fallback for invalid input.
        /// </summary>
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                // Deserialize normally; null-forgiving operator (!) suppresses compiler warning
                return JsonSerializer.Deserialize<T>(ref reader, options)!;
            }
            catch
            {
                // If the type is an enum, return 1 as the fallback
                if (typeof(T).IsEnum)
                {
                    // Convert 1 to the enum type
                    return (T)Enum.ToObject(typeof(T), 1);
                }

                // For all other types, return default
                return default!;
            }
        }

        /// <summary>
        /// Writes the property value to JSON.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            // Null-forgiving operator (!) ensures compiler does not warn about null reference
            JsonSerializer.Serialize(writer, value!, options);
        }
    }
}
