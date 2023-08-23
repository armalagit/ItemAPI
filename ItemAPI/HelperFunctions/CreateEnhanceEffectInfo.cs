using Serilog;
using System.Data.SqlClient;
using System.Data;

using ItemAPI.EnumStructure;
using ItemAPI.Settings;

namespace ItemAPI.HelperFunctions {

    public static class CreateEnhanceEffectInfo {

        public static EnhanceEffectStruct GetEnhanceEffectBonus(this int enhanceEffectId) {
            EnhanceEffectStruct enhanceEffect = new() {
                EffectId = enhanceEffectId,
                IncreaseValues = new Dictionary<int, List<decimal>>() { }
            };

            SqlConnection? sqlConnection = null;
            SqlCommand? sqlCommand = null;
            SqlDataReader? reader = null;
            try {
                sqlConnection = new SqlConnection(DatabaseSettings.ConnectionString);
                sqlCommand = new SqlCommand(
                    $"SELECT * FROM {DatabaseSettings.Arcadia}.dbo.{DatabaseSettings.EnhanceEffectResource} WHERE [sid] = @enhanceEffectId", 
                    sqlConnection
                );
                // Add parameters to the command
                sqlCommand.Parameters.Add("@enhanceEffectId", SqlDbType.Int).Value = enhanceEffectId;

                // Open the connection and execute the reader
                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read()) {

                        int effectId = Convert.ToInt32(reader.Convert<short>("effect_id"));
                        if (!enhanceEffect.IncreaseValues.ContainsKey(effectId))
                            enhanceEffect.IncreaseValues.Add(effectId, new List<decimal>() { });

                        for (int i = 1; i <= 25; i++) {

                            decimal increaseValue = reader.Convert<decimal>($"value_{i:D2}");

                            if (increaseValue == 0)
                                continue;

                            enhanceEffect.IncreaseValues[effectId].Add(increaseValue);
                        }
                    }

            } catch (Exception ex) {
                //Log the error
                Log.Error($"Error creating ItemAPI.HelperFunctions.GetEnhanceEffectBonus: {ex.Message}\nStack Trace:{ex.StackTrace}");
            } finally {
                reader?.Dispose();
                sqlCommand?.Dispose();
                if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed) {
                    sqlConnection.Close();
                }
                sqlConnection?.Dispose();
            }

            return enhanceEffect;
        }

    }

}
