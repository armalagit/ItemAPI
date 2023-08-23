using Serilog;
using System.Data.SqlClient;
using System.Data;

using ItemAPI.EnumStructure;
using ItemAPI.EnumConverters;
using ItemAPI.Settings;

namespace ItemAPI.HelperFunctions {

    public static class CreateRandomOptionStruct {

        public static RandomOptionStruct GetRandomOptions(this int randomOptionId) {

            RandomOptionStruct randomOptionStruct = new() {
                OptionId = randomOptionId,
                OptionVariables = new() { },
                ExtraAwakenOptions = new() { }
            };

            SqlConnection? sqlConnection = null;
            SqlCommand? sqlCommand = null;
            SqlDataReader? reader = null;
            try {
                sqlConnection = new SqlConnection(DatabaseSettings.ConnectionString);
                sqlCommand = new SqlCommand(
                    $"SELECT * FROM {DatabaseSettings.Telecaster}.dbo.{DatabaseSettings.Telecaster} WHERE [sid] = @randomOptionId", 
                    sqlConnection
                );
                // Add parameters to the command
                sqlCommand.Parameters.Add("@randomOptionId", SqlDbType.Int).Value = randomOptionId;

                // Open the connection and execute the reader
                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read()) {

                        for (int i = 1; i <= 10; i++) {

                            short typeId = Convert.ToInt16(reader.Convert<int>($"type_{i:D2}"));

                            if (typeId == 0)
                                continue;

                            int optVarFlag = (int)reader.Convert<decimal>($"value1_0{i}");
                            decimal typeValue = reader.Convert<decimal>($"value2_0{i}");

                            if (new List<int>() { 96, 97, 98, 99}.Contains(typeId)) {

                                List<string> optionVariableStructs =
                                    OptionTypeVariableEnumConverter.Convert(typeId, optVarFlag);

                                foreach (string optionVariableName in optionVariableStructs)
                                    randomOptionStruct.OptionVariables.Add(new OptionVariableStruct() {
                                        Name = optionVariableName,
                                        TypeId = typeId,
                                        IncreaseValue = typeValue
                                    });

                            }

                            if (new List<int>() { 130, 131 }.Contains(typeId))
                                randomOptionStruct.IncreaseSoulstoneSlot += 1;

                            // optVarFlag 1008, 1009, 1010 = Increase the level of all skills (Helmet item awaken)
                            // TODO: [effectIdType == 203 && effectLevel == 0] means skill increase, this is handled in GetEffectStruct()
                            if (new List<int>() { 133 }.Contains(typeId) && !new List<int>() { 1008, 1009, 1010 }.Contains(optVarFlag)) {
                                randomOptionStruct.Effects ??= new List<EffectStruct>() { };
                                randomOptionStruct.Effects.AddRange(
                                    optVarFlag.GetEffectStruct()
                                );
                            }

                            if (new List<int>() { 133 }.Contains(typeId) && new List<int>() { 1008, 1009, 1010 }.Contains(optVarFlag))
                                randomOptionStruct.ExtraAwakenOptions.Add($"Increase the level of all skills {typeValue.ToString().TrimEnd('0').TrimEnd('.')}%");

                            // Increase the probability of double attacks during basic attacks (Gloves)
                            if (typeId == 140)
                                randomOptionStruct.ExtraAwakenOptions.Add($"Increase the probability of double attacks during basic attacks {typeValue.ToString().TrimEnd('0').TrimEnd('.')}%");

                            // Decrease the cooldown time of all skills (Helmet)
                            if (typeId == 141)
                                randomOptionStruct.ExtraAwakenOptions.Add($"Decrease the cooldown time of all skills {typeValue.ToString().TrimEnd('0').TrimEnd('.')}%");

                            // Decrease the casting time of all skills (Gloves)
                            if (typeId == 142)
                                randomOptionStruct.ExtraAwakenOptions.Add($"Decrease the casting time of all skills {typeValue.ToString().TrimEnd('0').TrimEnd('.')}%");

                            // Decrease the duration of all debuffs (Boots)
                            if (typeId == 143)
                                randomOptionStruct.ExtraAwakenOptions.Add($"Decrease the duration of all debuffs {typeValue.ToString().TrimEnd('0').TrimEnd('.')}%");

                            // Increase creature stats (Boots)
                            if (typeId == 144)
                                randomOptionStruct.ExtraAwakenOptions.Add($"Increase creature stats {typeValue.ToString().TrimEnd('0').TrimEnd('.')}%");

                            // Amplify ability of equipped boss card (Belt)
                            if (typeId == 145)
                                randomOptionStruct.ExtraAwakenOptions.Add($"Amplify ability of equipped boss card {typeValue.ToString().TrimEnd('0').TrimEnd('.')}%");

                        }

                    }

            } catch (Exception ex) {
                //Log the error
                Log.Error($"Error creating ItemAPI.HelperFunctions.GetRandomOptions: {ex.Message}\nStack Trace:{ex.StackTrace}");
            } finally {
                reader?.Dispose();
                sqlCommand?.Dispose();
                if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed) {
                    sqlConnection.Close();
                }
                sqlConnection?.Dispose();
            }

            return randomOptionStruct;
        }

    }

}
