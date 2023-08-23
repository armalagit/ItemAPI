
namespace ItemAPI.EnumStructure {

    public struct WearTypeStruct {
        public string Name;
        public short TypeId;
    }

    // Base variables only!
    public struct ItemStatsStruct {
        public string Name;
        public short BitFlag;
        public decimal IncreaseValue;
        public decimal IncreasePerLevel;
    }

    public struct BaseVariableStruct {
        public string Name;
        public short BitFlag;
    }

    public struct OptionVariableStruct {
        public short TypeId;
        public string Name;
        public decimal IncreaseValue;
    }

    public struct SetBonusStruct {
        public int SetId;
        public string Name;
        public string Tooltip;
        public List<ItemStatsStruct> BaseVariables;
        public List<OptionVariableStruct> OptionVariables;
        // TODO [SetItemEffectResource] > [effect_id]
    }

    public struct EnhanceEffectStruct {
        public int EffectId;
        public Dictionary<int, List<decimal>> IncreaseValues;
    }

    public struct EffectStruct {
        public int EffectId;
        public int OrdinalId;
        public string Tooltip;
        public List<OptionVariableStruct> OptionVariables;
        public List<string> SkillBoost;
    }

    public struct RandomOptionStruct {
        public int OptionId;
        public List<OptionVariableStruct> OptionVariables;
        public List<EffectStruct> Effects;
        public short IncreaseSoulstoneSlot;
        public List<string> ExtraAwakenOptions;
    }

    public struct LevelStruct {
        public int Level;
        public double EnhanceSuccess;
    }

}
