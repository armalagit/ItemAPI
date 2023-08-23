using ItemAPI.EnumStructure;

namespace ItemAPI.HelperFunctions {

    public static class ConsolidateOptionVariables {

        public static List<OptionVariableStruct> ConsolidateOptVars(this List<OptionVariableStruct> optionVariables) {

            return optionVariables
                .GroupBy(o => new { o.IncreaseValue, o.TypeId })
                .Select(g => new OptionVariableStruct {
                    IncreaseValue = g.Key.IncreaseValue,
                    Name = string.Join(", ", g.Select(o => o.Name)),
                    TypeId = g.Select(o => o.TypeId).FirstOrDefault() // Set the TypeId from the first element in the group
                })
                .ToList();

        }

    }

}
