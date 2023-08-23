using Serilog;
using System.Data.SqlClient;
using System.Data;
using ItemAPI.Settings;

namespace ItemAPI.HelperFunctions {

    public static class ConvertSkillIdToName {

        public static string GetSkillStringName(this int skillId) {

            SqlConnection? sqlConnection = null;
            SqlCommand? sqlCommand = null;
            SqlDataReader? reader = null;
            try {
                sqlConnection = new SqlConnection(DatabaseSettings.ConnectionString);
                sqlCommand = new SqlCommand(
                    $"SELECT StrRes.[value] skillName FROM {DatabaseSettings.Arcadia}.dbo.{DatabaseSettings.SkillResource} AS SkiRes INNER JOIN {DatabaseSettings.Arcadia}.dbo.{DatabaseSettings.StringResource} AS StrRes ON StrRes.code = SkiRes.text_id WHERE SkiRes.id = @skillId;", 
                    sqlConnection
                );
                // Add parameters to the command
                sqlCommand.Parameters.Add("@skillId", SqlDbType.Int).Value = skillId;

                // Open the connection and execute the reader
                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                        return reader.Convert<string>("skillName");

            } catch (Exception ex) {
                //Log the error
                Log.Error($"Error creating ItemAPI.HelperFunctions.GetSkillStringName: {ex.Message}\nStack Trace:{ex.StackTrace}");
            } finally {
                reader?.Dispose();
                sqlCommand?.Dispose();
                if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed) {
                    sqlConnection.Close();
                }
                sqlConnection?.Dispose();
            }

            return $"[ GetSkillStringName error: {skillId} ]";

        }

    }

}
