using Serilog;
using System.Data.SqlClient;
using System.Data;
using ItemAPI.Settings;

namespace ItemAPI.HelperFunctions {

    public static class ConvertCodeToStringValue {

        public static string GetStringValue(this int stringCode) {

            SqlConnection? sqlConnection = null;
            SqlCommand? sqlCommand = null;
            SqlDataReader? reader = null;
            try {
                sqlConnection = new SqlConnection(DatabaseSettings.ConnectionString);
                sqlCommand = new SqlCommand(
                    $"SELECT [value] translatedCode FROM {DatabaseSettings.Arcadia}.dbo.{DatabaseSettings.StringResource} WHERE code = @stringCode;",
                    sqlConnection
                );
                // Add parameters to the command
                sqlCommand.Parameters.Add("@stringCode", SqlDbType.Int).Value = stringCode;

                // Open the connection and execute the reader
                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                        return reader.Convert<string>("translatedCode");

            } catch (Exception ex) {
                //Log the error
                Log.Error($"Error creating ItemAPI.HelperFunctions.GetStringValue: {ex.Message}\nStack Trace:{ex.StackTrace}");
            } finally {
                reader?.Dispose();
                sqlCommand?.Dispose();
                if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed) {
                    sqlConnection.Close();
                }
                sqlConnection?.Dispose();
            }

            return $"[ GetStringValue error: {stringCode} ]";

        }

    }

}
