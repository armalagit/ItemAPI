namespace ItemAPI.EnumConverters {

    internal class OptionTypeVariableEnumConverter {

        public static List<string> Convert(short typeId, long flagValue) {

            List<string> flagList_96_98 = new() {
                "Strength",
                "Vitality",
                "Agility",
                "Dexterity",
                "Intelligence",
                "Wisdom",
                "Luck",
                "Physical Attack",
                "Magical Attack",
                "Physical Defense",
                "Magical Defense",
                "Attack Speed",
                "Cast Speed",
                "Movement Speed",
                "Accuracy",
                "Magical Accuracy",
                "Critical Rate",
                "Block Per.",
                "Block Def.",
                "Evasion",
                "Magical Resistance",
                "Max HP",
                "Max MP",
                "Max Stamina",
                "Health Regeneration",
                "Mana Regeneration",
                "Stamina Regeneration",
                "Health Regeneration [%]",
                "Mana Regeneration [%]",
                "Critical Power",
                "Maximum Weight",
                "Received Damage Reduction"
            };
            List<string> flagList_97_99 = new() {
                "Non-Elemental Resistance",
                "Fire Resistance",
                "Water Resistance",
                "Air Resistance",
                "Earth Resistance",
                "Holy Resistance",
                "Darkness Resistance",
                "[ OptVarEnum error ]",
                "[ OptVarEnum error ]",
                "Perf. Block ratio",
                "P. Ignore",
                "M. Ignore",
                "P. Pierce",
                "M. Pierce",
                "Non-Elemental Lasting Damage",
                "Fire Lasting Damage",
                "Water Lasting Damage",
                "Air Lasting Damage",
                "Earth Lasting Damage",
                "Holy Lasting Damage",
                "Shade Lasting Damage",
                "Non-Elemental Additional Damage",
                "Fire Additional Damage",
                "Water Additional Damage",
                "Air Additional Damage",
                "Earth Additional Damage",
                "Holy Additional Damage",
                "Shade Additional Damage",
                "Critical Power",
                "HP Regeneration stop",
                "MP Regeneration stop"
            };

            List<string> flagListToUse = flagList_96_98;
            if (typeId == 97 || typeId == 99)
                flagListToUse = flagList_97_99;

            List<string> returnList = new() { };
            for (int index = 0; index < flagListToUse.Count; index++)
                if (((flagValue >> index) & 1) == 1)
                    returnList.Add(flagListToUse[index]);

            return returnList;

        }

    }

}