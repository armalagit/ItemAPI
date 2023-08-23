using ItemAPI.EnumStructure;

namespace ItemAPI.EnumConverters {

    public static class BaseTypeVariableEnumConverter {

        public static BaseVariableStruct ConvertBaseVariable(this short flagTypeId) {

            Dictionary<short, string> enumDictionary = new() {
                { 11, "Physical Attack" },
                { 12, "Magical Attack" },
                { 13, "Physical Accuracy" },
                { 14, "Attack Speed" },
                { 15, "Physical Defence" },
                { 16, "Magical Defence" },
                { 17, "Evasion" },
                { 18, "Move Speed" },
                { 19, "Block Per" },
                { 20, "Maximum Weight" },
                { 21, "Block Def." },
                { 22, "Cast speed" },
                { 23, "Magical Accuracy" },
                { 24, "Magical Resistance" },
                { 25, "Skill cooldown reduce" },
                { 26, "Belt slot count" },
                { 27, "[ BaseVarEnum error: 27 ]" },
                { 30, "Maximum HP" },
                { 31, "Maximum MP" },
                { 33, "Mana Regeneration" },
                { 35, "Perfect block chance" },
                { 36, "Ignore Physical Defence" },
                { 37, "Ignore Magical Defence" },
                { 38, "Penetration of Physical Defence" },
                { 39, "Penetration of Magical Defence" },
                { 95, "Rental only" },
                { 96, "+ to parameter A type(common states)" },
                { 97, "+ to parameter B type" },
                { 98, "+ % to parameter A type(common states)" },
                { 99, "+ % to parameter B type" },
                { 130, "Soul Stone socket slot" },
                { 131, "Soul Stone socket slot" },
                { 133, "Apply continuous effect type performance; IDK if it works" },
                { 140, "Increasing double attack ratio (Like assassin impact)" },
                { 141, "Decreasing cooldown (Should be similiar to 25)" },
                { 142, "Increase cast speed (Should be similiar to 22)" },
                { 143, "Resistance to all control-type effects (Stun, fear, nightmare, etc)" },
                { 144, "Increasing creature stats" }
            };

            if (enumDictionary.TryGetValue(flagTypeId, out string? enumStringValue))
                return new BaseVariableStruct() {
                    Name = enumStringValue ?? $"[ BaseVarEnum error: {flagTypeId} ]",
                    BitFlag = flagTypeId
                };

            // Fallback value
            return new BaseVariableStruct() {
                Name = $"[ BaseVarEnum error: {flagTypeId} ]",
                BitFlag = flagTypeId
            };

        }

    }

}