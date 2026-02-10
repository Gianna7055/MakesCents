/*
 * Gianna Ross
 * Makes Cents
 * Sources: https://chatgpt.com/c/697b1f88-33e8-8332-8560-f5fa7b8c624e
 */
using Dapper;
using System.Data;

namespace MakesCentsBackend.Services.Utilities
{
    /// <summary>
    /// Class to handle date only values when dealing with the database
    /// </summary>
    public class DateOnlyHandler : SqlMapper.TypeHandler<DateOnly>
    {
        /// <summary>
        /// Method to take a DateOnly and set it to a DateTime
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.Value = value.ToDateTime(TimeOnly.MinValue); // send as DateTime to SQL
        }

        /// <summary>
        /// Method to take a DateTime object and return a DateOnly value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override DateOnly Parse(object value)
        {
            return DateOnly.FromDateTime((DateTime)value); // read DateTime from SQL, convert to DateOnly
        }
    }

}
