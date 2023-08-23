namespace ItemAPI.Settings {

    internal class DatabaseSettings {

        /// <summary>
        /// Gets or sets the database connection string
        /// </summary>
        public static string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the [Arcadia] database name
        /// </summary>
        public static string Arcadia { get; set; } = "Arcadia";

        /// <summary>
        /// Gets or sets the [Telecaster] database name
        /// </summary>
        public static string Telecaster { get; set; } = "Telecaster";

        /// <summary>
        /// Gets or sets the [EnhanceEffectResource] table name
        /// </summary>
        public static string EnhanceEffectResource { get; set; } = "EnhanceEffectResource";

        /// <summary>
        /// Gets or sets the [ItemEffectResource] table name
        /// </summary>
        public static string ItemEffectResource { get; set; } = "ItemEffectResource";

        /// <summary>
        /// Gets or sets the [RandomOption] table name
        /// </summary>
        public static string RandomOption { get; set; } = "RandomOption";

        /// <summary>
        /// Gets or sets the [SetItemEffectResource] table name
        /// </summary>
        public static string SetItemEffectResource { get; set; } = "SetItemEffectResource";

        /// <summary>
        /// Gets or sets the [SkillResource] table name
        /// </summary>
        public static string SkillResource { get; set; } = "SkillResource";

        /// <summary>
        /// Gets or sets the [StringResource] table name
        /// </summary>
        public static string StringResource { get; set; } = "StringResource";

    }

}
