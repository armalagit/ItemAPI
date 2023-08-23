using Serilog;
using System.Data.SqlClient;
using System.Data;

using ItemAPI.EnumStructure;
using ItemAPI.EnumConverters;
using ItemAPI.HelperFunctions;
using ItemAPI.Settings;

namespace ItemAPI {

    public class ItemStructure {

        /// <summary>
        /// Gets or sets the [ItemResource] id
        /// </summary>
        public int ResourceCode { get; private set; }

        /// <summary>
        /// Gets or sets the [Item] entry
        /// </summary>
        public ExistingItemStructure? AttachedItem { get; private set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the tooltip text
        /// </summary>
        public string Tooltip { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the [ItemResource] [class] column
        /// </summary>
        public string ClassDefinition { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the item wear type
        /// ItemAPI.EnumStructure > WearTypeStruct
        /// </summary>
        public WearTypeStruct WearType { get; private set; }

        /// <summary>
        /// Gets or sets the [ItemResource] set bonus
        /// </summary>
        public SetBonusStruct? SetBonus { get; private set; }

        /// <summary>
        /// Gets or sets the item socket
        /// </summary>
        public int SoulstoneSlotCount { get; private set; }

        /// <summary>
        /// Gets or sets the job type limit
        /// </summary>
        public bool FighterEnabled { get; private set; } = true;
        public bool HunterEnabled { get; private set; } = true;
        public bool MageEnabled { get; private set; } = true;
        public bool SummonerEnabled { get; private set; } = true;

        /// <summary>
        /// Gets or sets the minimum and maximum use level
        /// </summary>
        public int MinUseLevel { get; private set; } = 0;
        public int MaxUseLevel { get; private set; } = 301;

        /// <summary>
        /// Gets or sets the default durability
        /// </summary>
        public int Durability { get; private set; }

        /// <summary>
        /// Gets or sets the use flags
        /// </summary>
        public List<string> UseFlags { get; private set; } = new List<string> { };

        /// <summary>
        /// Gets or sets the use timer
        /// </summary>
        public int UseTime { get; private set; }

        /// <summary>
        /// Gets or sets the [ItemResource] base variables
        /// NOTE: BaseVariables are always flat values (?)
        /// </summary>
        public List<ItemStatsStruct> BaseVariables { get; private set; } = new List<ItemStatsStruct> { };

        /// <summary>
        /// Gets or sets the [ItemResource] option variables
        /// </summary>
        public List<OptionVariableStruct> OptionVariables { get; private set; } = new List<OptionVariableStruct> { };

        /// <summary>
        /// Gets or sets the enhance effects for [enhance_id] column in dbo.[ItemResource]
        /// </summary>
        public EnhanceEffectStruct? EnhanceEffect { get; private set; }

        /// <summary>
        /// Gets or sets the dbo.[ItemEffectResource] variables for [effect_id] column in dbo.[ItemResource]
        /// </summary>
        public List<EffectStruct>? EffectOptions { get; private set; }

        /// <summary>
        /// Gets or sets the icon file
        /// </summary>
        public string? IconFile { get; private set; }

        #region Class constructor

        // Dummy constructor
        public ItemStructure() { }

        public ItemStructure(int resourceCode, long existingIdentifier = -1) {

            if (existingIdentifier > 0) {
                AttachedItem = new(existingIdentifier);
                if (AttachedItem.RandomOptions != null)
                    SoulstoneSlotCount += AttachedItem.RandomOptions.Value.IncreaseSoulstoneSlot;
            }

            ResourceCode = resourceCode;

            SqlConnection? sqlConnection = null;
            SqlCommand? sqlCommand = null;
            SqlDataReader? reader = null;
            try {
                sqlConnection = new SqlConnection(DatabaseSettings.ConnectionString);
                sqlCommand = new SqlCommand(
                    $"SELECT * FROM {DatabaseSettings.Arcadia}.dbo.{DatabaseSettings.ItemResource} WHERE id = @resourceCode", 
                    sqlConnection
                );
                // Add parameters to the command
                sqlCommand.Parameters.Add("@resourceCode", SqlDbType.Int).Value = ResourceCode;

                // Open the connection and execute the reader
                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read()) {

                        Name = reader.Convert<int>("name_id").GetStringValue();
                        Tooltip = reader.Convert<int>("tooltip_id").GetStringValue();

                        ClassDefinition = reader.Convert<int>("class").ConvertClassType();

                        short wearTypeId = Convert.ToInt16(reader.Convert<int>("wear_type"));
                        WearType = wearTypeId.ConvertWearType();

                        // Convert weapon blacksmith levels
                        if (AttachedItem != null && AttachedItem.Level.Level > 40 && new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 24, 25, 99 }.Contains(wearTypeId)) {
                            int actualLevel = AttachedItem.Level.Level % 100;
                            double enhanceSuccess = (AttachedItem.Level.Level - actualLevel) / 1000.00;

                            AttachedItem.Level = new() {
                                Level = actualLevel,
                                EnhanceSuccess = enhanceSuccess
                            };
                        }

                        int setBonusId = reader.Convert<int>("set_id");
                        if (setBonusId > 0)
                            SetBonus = setBonusId.GetSetBonus();

                        SoulstoneSlotCount = reader.Convert<int>("socket");
                        if (AttachedItem != null && AttachedItem.RandomOptions != null)
                            SoulstoneSlotCount += AttachedItem.RandomOptions.Value.IncreaseSoulstoneSlot;

                        FighterEnabled = reader.Convert<int>("limit_fighter") == 1;
                        HunterEnabled = reader.Convert<int>("limit_hunter") == 1;
                        MageEnabled = reader.Convert<int>("limit_magician") == 1;
                        SummonerEnabled = reader.Convert<int>("limit_summoner") == 1;

                        MinUseLevel = reader.Convert<int>("use_min_level");
                        MaxUseLevel = reader.Convert<int>("use_max_level");
                        Durability = reader.Convert<int>("ethereal_durability");
                        UseFlags = reader.Convert<int>("item_use_flag").ConvertUseFlag();
                        UseTime = reader.Convert<int>("available_period");

                        // Base variables
                        for (int i = 0; i <= 3; i++) {

                            short flagValue = reader.Convert<short>($"base_type_{i}");

                            if (flagValue == 0)
                                continue;

                            BaseVariableStruct baseVariableStruct = flagValue.ConvertBaseVariable();
                            ItemStatsStruct baseVar = new() {
                                Name = baseVariableStruct.Name,
                                BitFlag = baseVariableStruct.BitFlag,
                                IncreaseValue = reader.Convert<decimal>($"base_var1_{i}"),
                                IncreasePerLevel = reader.Convert<decimal>($"base_var2_{i}")
                            };

                            BaseVariables.Add(baseVar);

                        }

                        // Option variables
                        for (int i = 0; i <= 3; i++) {

                            short typeId = reader.Convert<short>($"opt_type_{i}");

                            if (typeId == 0)
                                continue;

                            long optVarFlag = (long)reader.Convert<decimal>($"opt_var1_{i}");

                            List<string> optionVariableStructs =
                                OptionTypeVariableEnumConverter.Convert(typeId, optVarFlag);

                            foreach (string optionVariableName in optionVariableStructs)
                                OptionVariables.Add(new OptionVariableStruct() {
                                    Name = optionVariableName,
                                    TypeId = typeId,
                                    IncreaseValue = reader.Convert<decimal>($"opt_var2_{i}")
                                });

                        }

                        int effectId = reader.Convert<int>("effect_id");
                        if (effectId > 0)
                            EffectOptions = effectId.GetEffectStruct();

                        int enhanceId = reader.Convert<int>("enhance_id");
                        if (enhanceId > 0)
                            EnhanceEffect = enhanceId.GetEnhanceEffectBonus();

                        IconFile = reader.Convert<string>("icon_file_name") + ".jpg";

                    }

            } catch (Exception ex) {
                //Log the error
                Log.Error($"Error creating ItemStructure: {ex.Message}\nStack Trace:{ex.StackTrace}");
            } finally {
                reader?.Dispose();
                sqlCommand?.Dispose();
                if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed) {
                    sqlConnection.Close();
                }
                sqlConnection?.Dispose();
            }

        }

        #endregion

    }

}
