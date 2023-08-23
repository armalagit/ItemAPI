using System.Data.SqlClient;
using System.ComponentModel;

namespace ItemAPI.HelperFunctions {

    static class Conversion {

        public static T Convert<T>(this SqlDataReader sqlDataReader, string identifier) {

            // Read the column ordinal number
            int columnOrdinal = sqlDataReader.GetOrdinal(identifier);

            // Read integer value by ordinal number
            object columnValue = sqlDataReader.GetValue(columnOrdinal);

            // Check if value null
            if (columnValue == null) {
                return default;
            }

            // If the value is already of the correct type, return it directly
            if (columnValue is T t) {
                return t;
            }

            // Try to convert the value to the correct type
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFrom(columnValue);

        }

    }

}