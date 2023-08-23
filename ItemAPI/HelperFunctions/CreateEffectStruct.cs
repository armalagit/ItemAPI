using Serilog;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

using ItemAPI.EnumStructure;
using ItemAPI.EnumConverters;
using ItemAPI.Settings;

namespace ItemAPI.HelperFunctions {

    public static class CreateEffectStruct {
        
        public static List<EffectStruct> GetEffectStruct(this int effectId) {
            List<EffectStruct> effectStructList = new();

            SqlConnection? sqlConnection = null;
            SqlCommand? sqlCommand = null;
            SqlDataReader? reader = null;
            try {
                sqlConnection = new SqlConnection(DatabaseSettings.ConnectionString);
                sqlCommand = new SqlCommand(
                    $"SELECT * FROM {DatabaseSettings.Arcadia}.dbo.{DatabaseSettings.ItemEffectResource} WHERE id = @effectId", 
                    sqlConnection
                );
                // Add parameters to the command
                sqlCommand.Parameters.Add("@effectId", SqlDbType.Int).Value = effectId;

                // Open the connection and execute the reader
                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read()) {

                        int ordinalId = reader.Convert<int>("ordinal_id");
                        short effectIdType = reader.Convert<short>("effect_id");
                        short effectLevel = reader.Convert<short>("effect_level");

                        EffectStruct effectStruct = new() {
                            EffectId = effectId,
                            OrdinalId = ordinalId,
                            Tooltip = reader.Convert<int>("tooltip_id").GetStringValue(),
                            OptionVariables = new() { }
                        };

                        if (effectIdType == 203 && effectLevel == 0)
                            effectStruct.SkillBoost = new() { };

                        for (int i = 0; i <= 10; i += 3) {

                            int typeId = (int)reader.Convert<decimal>($"value_{i}");
                            long optVarFlag = (long)reader.Convert<decimal>($"value_{i + 1}");

                            if (effectIdType == 203 && effectLevel == 0 && optVarFlag > 0) {
                                string skillName = Convert.ToInt32(typeId).GetSkillStringName();
                                skillName = Regex.Replace(skillName, "<.*?>", string.Empty);

                                if (effectStruct.SkillBoost == null)
                                    effectStruct.SkillBoost = new() { };

                                effectStruct.SkillBoost.Add(
                                    $"{skillName} +{optVarFlag}"
                                );
                            }

                            // TODO: [effectIdType == 203 && effectLevel == 0] means skill increase
                            if (new List<int>() { 96, 97, 98, 99 }.Contains(typeId)) {

                                List<string> optionVariableStructs =
                                    OptionTypeVariableEnumConverter.Convert((short)typeId, optVarFlag);

                                foreach (string optionVariableName in optionVariableStructs)
                                    effectStruct.OptionVariables.Add(new OptionVariableStruct() {
                                        Name = optionVariableName,
                                        TypeId = (short)typeId,
                                        IncreaseValue = reader.Convert<decimal>($"value_{i + 2}")
                                    });

                            }

                        }

                        effectStructList.Add(effectStruct);

                    }

            } catch (Exception ex) {
                //Log the error
                Log.Error($"Error creating ItemAPI.HelperFunctions.GetEffectStruct: {ex.Message}\nStack Trace:{ex.StackTrace}");
            } finally {
                reader?.Dispose();
                sqlCommand?.Dispose();
                if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed) {
                    sqlConnection.Close();
                }
                sqlConnection?.Dispose();
            }

            return effectStructList;
        }

    }

}
