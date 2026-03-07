using Dapper;
using System.Data;

namespace MakesCentsBackend.Models.Converters
{
    public class OptionalDBConverter<T> : SqlMapper.TypeHandler<Optional<T>>
    {
        public override Optional<T> Parse(object value)
        {
            if (value == null || value is DBNull)
            {
                return new Optional<T> (false);
            }
            return new Optional<T>(true, (T)value);
        }

        public override void SetValue(IDbDataParameter parameter, Optional<T> value)
        {
            if (value.HasValue)
            {
                parameter.Value = value.Value ?? (object)DBNull.Value;
            }
            else
            {
                parameter.Value = DBNull.Value;
            }
        }
    }
}
