using ItemAPI.EnumStructure;

namespace ItemAPI.EnumConverters {

    public static class WearTypeEnumConverter {

        public static WearTypeStruct ConvertWearType(this short wearType) {
            WearTypeStruct convertedType = new() {
                Name = $"[ BaseVarEnum error: {wearType} ]",
                TypeId = wearType
            };

            convertedType.Name = wearType switch {
                -2 => "Warehouse",
                0 => "Main hand",
                1 => "Shield",
                2 => "Armor",
                3 => "Helmet",
                4 => "Gloves",
                5 => "Boots",
                6 => "Belt",
                7 => "Cloak, mantle",
                8 => "Necklace",
                9 or 10 => "Ring",
                11 => "Earrings",
                12 => "Decorative mask",
                13 => "Decorative hairstyle",
                14 => "Decorative weapon",
                15 => "Decorative shield",
                16 => "Decorative armor",
                17 => "Decorative helmet",
                18 => "Decorative gloves",
                19 => "Decorative boots",
                20 => "Decorative cloak",
                21 => "Wings",
                22 => "Mountable",
                23 => "Bag",
                24 => "One-Handed weapon",
                25 => "Shield",
                26 => "Decorative One-Handed weapon",
                27 => "Decorative shield",
                28 or 29 => "Booster",
                198 => "Belt slot wearable",
                199 => "Decorative Two-Handed weapon",
                90 or 91 or 92 or 93 or 94 => "Mask",
                99 => "Two-Handed weapon",
                100 => "Skill",
                200 => "Creature wearable",
                1000 or 1001 or 1002 or 1003 or 1004 or 1005 => "Belt slot wearable",
                _ => "Other",
            };

            return convertedType;
        }

    }

}
