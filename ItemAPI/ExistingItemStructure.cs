using Serilog;
using System.Data.SqlClient;
using System.Data;

using ItemAPI.EnumStructure;
using ItemAPI.HelperFunctions;
using ItemAPI.Settings;

namespace ItemAPI {

    public class ExistingItemStructure {

        /// <summary>
        /// Gets or sets the [Item] sid
        /// </summary>
        public long IdentifierCode { get; private set; }

        /// <summary>
        /// Gets or sets the [account_id] column
        /// </summary>
        public int WarehouseId { get; private set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public long Count { get; private set; }

        /// <summary>
        /// Gets or sets the Blacksmith level
        /// </summary>
        public LevelStruct Level { get; set; }

        /// <summary>
        /// Gets or sets the (cube) enhancement
        /// </summary>
        public int Enhancement { get; private set; }

        /// <summary>
        /// Gets or sets the durability
        /// </summary>
        public int Durability { get; private set; }

        /// <summary>
        /// Gets or sets the Soul Stone sockets
        /// </summary>
        public List<ItemStructure?> Socket { get; private set; } = new List<ItemStructure?> { };

        /// <summary>
        /// Gets or sets the remaining use time
        /// </summary>
        public int UseTimeLeft { get; private set; }

        /// <summary>
        /// Gets or sets the [elemental_effect_type] string (manual conversion)
        /// </summary>
        public string? ElementalJewelEffectName { get; private set; }

        /// <summary>
        /// Gets or sets the dbo.[RandomOption] variables for [awaken_sid] column in dbo.[Item]
        /// TODO: Are these stats only?
        /// </summary>
        public RandomOptionStruct? AwakenOptions { get; private set; }

        /// <summary>
        /// Gets or sets the [elemental_effect_attack_point]
        /// </summary>
        public int AdditionalPAtk { get; private set; }

        /// <summary>
        /// Gets or sets the [[elemental_effect_magic_point]]
        /// </summary>
        public int AdditionalMAtk { get; private set; }

        /// <summary>
        /// Gets or sets the dbo.[RandomOption] variables for [random_option_sid] column in dbo.[Item]
        /// </summary>
        public RandomOptionStruct? RandomOptions { get; private set; }

        #region Constructors

        // Dummy constructor
        public ExistingItemStructure() { }

        public ExistingItemStructure(long itemSid) {

            IdentifierCode = itemSid;

            SqlConnection? sqlConnection = null;
            SqlCommand? sqlCommand = null;
            SqlDataReader? reader = null;
            try {
                sqlConnection = new SqlConnection(DatabaseSettings.ConnectionString);
                sqlCommand = new SqlCommand(
                    $"SELECT * FROM {DatabaseSettings.Telecaster}.dbo.{DatabaseSettings.Item} WHERE [sid] = @identifierCode", 
                    sqlConnection
                );
                // Add parameters to the command
                sqlCommand.Parameters.Add("@identifierCode", SqlDbType.BigInt).Value = IdentifierCode;

                // Open the connection and execute the reader
                sqlConnection.Open();
                reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read()) {

                        WarehouseId = reader.Convert<int>("account_id");
                        Count = reader.Convert<long>("cnt");

                        int blacksmithLevel = reader.Convert<int>("level");
                        Level = new() {
                            Level = blacksmithLevel,
                            EnhanceSuccess = 0
                        };

                        Enhancement = reader.Convert<int>("enhance");
                        Durability = reader.Convert<int>("ethereal_durability");

                        for (int i = 0; i <= 3; i++) {
                            int socketItemResourceCode = reader.Convert<int>($"socket_{i}");

                            if (socketItemResourceCode == 0)
                                continue;

                            Socket.Add(new ItemStructure(socketItemResourceCode));
                        }

                        int awakenSid = reader.Convert<int>("awaken_sid");
                        if (awakenSid > 0)
                            AwakenOptions = awakenSid.GetRandomOptions();

                        AdditionalPAtk = reader.Convert<int>("elemental_effect_attack_point");
                        AdditionalMAtk = reader.Convert<int>("elemental_effect_magic_point");

                        int randomOptSid = reader.Convert<int>("random_option_sid");
                        if (randomOptSid > 0)
                            RandomOptions = randomOptSid.GetRandomOptions();

                        UseTimeLeft = reader.Convert<int>("remain_time");

                        int elementalEffectTypeId = reader.Convert<byte>("elemental_effect_type");
                        ElementalJewelEffectName = elementalEffectTypeId switch {
                            1 => "Scorching Jewel (Elite)",
                            2 => "Zephyr Jewel (Basic)",
                            3 => "Divine Jewel (Elite)",
                            4 => "Tidal Jewel (Elite)",
                            5 => "Whirlwind Jewel (Basic)",
                            6 => "Chaotic Jewel (Elite)",
                            7 => "Rage Stone (Expert)",
                            11 => "Galaxy Stone (Expert)",
                            _ => null
                        };

                    }

            } catch (Exception ex) {
                //Log the error
                Log.Error($"Error creating ItemStructure.ExistingItemStructure: {ex.Message}\nStack Trace:{ex.StackTrace}");
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
