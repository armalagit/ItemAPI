using Serilog;
using System.Data.SqlClient;
using System.Data;

using ItemAPI.EnumStructure;
using ItemAPI.EnumConverters;
using ItemAPI.Settings;

namespace ItemAPI.HelperFunctions {

    public static class CreateSetBonusStruct {

        public static SetBonusStruct GetSetBonus(this int setId) {

            SetBonusStruct setBonus = new() {
                SetId = setId,
                Name = string.Empty,
                BaseVariables = new List<ItemStatsStruct>() { },
                OptionVariables = new List<OptionVariableStruct>() { }
            };

            SqlConnection? sqlConnection = null;
            SqlCommand? sqlCommand = null;
            SqlDataReader? reader = null;
            try {
                sqlConnection = new SqlConnection(DatabaseSettings.ConnectionString);
                sqlCommand = new SqlCommand(
                    $"SELECT * FROM {DatabaseSettings.Arcadia}.dbo.{DatabaseSettings.SetItemEffectResource} WHERE set_id = @setIdentifier;", 
                    sqlConnection
                );
                // Add parameters to the command
                sqlCommand.Parameters.Add("@setIdentifier", SqlDbType.Int).Value = setId;

                // Open the connection and execute the reader
                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read()) {

                        setBonus.Name = reader.Convert<int>("text_id").GetStringValue();
                        setBonus.Tooltip = reader.Convert<int>("tooltip_id").GetStringValue();

                        for (int i = 0; i <= 3; i++) {

                            short flagValue = reader.Convert<short>($"base_type_{i}");

                            if (flagValue == 0)
                                continue;

                            BaseVariableStruct baseVariableStruct = flagValue.ConvertBaseVariable();
                            setBonus.BaseVariables.Add(new() {
                                Name = baseVariableStruct.Name,
                                BitFlag = baseVariableStruct.BitFlag,
                                IncreaseValue = reader.Convert<decimal>($"base_var1_{i}"),
                                IncreasePerLevel = reader.Convert<decimal>($"base_var2_{i}")
                            });

                        }

                        for (int i = 0; i <= 3; i++) {

                            short typeId = reader.Convert<short>($"opt_type_{i}");

                            if (typeId == 0)
                                continue;

                            long optVarFlag = (long)reader.Convert<decimal>($"opt_var1_{i}");

                            List<string> optionVariableStructs =
                                OptionTypeVariableEnumConverter.Convert(typeId, optVarFlag);

                            foreach (string optionVariableName in optionVariableStructs)
                                setBonus.OptionVariables.Add(new OptionVariableStruct() {
                                    Name = optionVariableName,
                                    TypeId = typeId,
                                    IncreaseValue = reader.Convert<decimal>($"opt_var2_{i}")
                                });

                        }

                    }

            } catch (Exception ex) {
                //Log the error
                Log.Error($"Error creating ItemAPI.HelperFunctions.GetSetBonus: {ex.Message}\nStack Trace:{ex.StackTrace}");
            } finally {
                reader?.Dispose();
                sqlCommand?.Dispose();
                if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed) {
                    sqlConnection.Close();
                }
                sqlConnection?.Dispose();
            }

            return setBonus;

        }

    }

}
