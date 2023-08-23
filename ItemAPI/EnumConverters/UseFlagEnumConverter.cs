
namespace ItemAPI.EnumConverters {

    public static class UseFlagEnumConverter {

        public static List<string> ConvertUseFlag(this int useFlag) {

            // Create a list of readable flags
            List<string> flagList = new() { 
                "Can't donate", 
                "Can't store", 
                "Can't enhance", 
                "Consumable", 
                "Is card", 
                "Is socket",                 
                "Stackable", 
                "Needs target", 
                "Warp", 
                "Untradeable", 
                "Unsellable", 
                "Quest Item", 
                "Can't Use Overweight", 
                "Cash Item", 
                "Can't use while on a mount", 
                "Non-droppable", 
                "Can't use while moving", 
                "Quest Distribute", 
                "Can't use while sitting", 
                "Can't Use in Raid", 
                "Can't Use in Hidden Village", 
                "Can't Use in Event Area", 
                "Can't Use in Ursa", 
                "Can Only Use in Ursa", 
                "Can't Use in Death Match", 
                "Can Only Use in Death Match", 
                "Can't Erase", 
                "Can Only Use While Sitting", 
                "Can't Use in Secret Dungeon", 
                "Can't Use in Battle Arena", 
                "Can't Decompose", 
                "Is Identifiable" 
            };

            List<string> plainTextFlagList = new() { };
            for (int index = 0; index < flagList.Count; index++)
                if (((useFlag >> index) & 1) == 1)
                    plainTextFlagList.Add(flagList[index]);

            return plainTextFlagList;

        }

    }
}
