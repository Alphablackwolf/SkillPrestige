namespace SkillPrestige.SpaceCore;

public class SpaceCoreSkillRegistrations
{
    public static List<SpaceCoreSkill> AllSkillsRegistered => new List<SpaceCoreSkill>
    {
        new LuckSkill(),
        new CookingSkill(),
        new BinningSkill(),
        new SocializingSkill(),
        new SlimingSkill(),
        new TravellingSkill(),
        new ArchaeologySkill()
    };
}

    internal class LuckSkill : SpaceCoreSkill
    {
        public LuckSkill() : base("moonslime.Luck", "Luck Skill", "Luck", 5) {}
    }
    internal class CookingSkill : SpaceCoreSkill
    {
        public CookingSkill() : base("moonslime.Cooking", "Yet Another Cooking Skill", "Cooking", 6) {}
    }
    internal class BinningSkill : SpaceCoreSkill
    {
        public BinningSkill() : base("drbirbdev.Binning", "Binning Skill", "Binning", 7) {}
    }
    internal class SocializingSkill : SpaceCoreSkill
    {
        public SocializingSkill() :  base("drbirbdev.Socializing", "Socializing Skill", "Socializing", 8) {}
    }
    internal class SlimingSkill : SpaceCoreSkill
    {
        public SlimingSkill() : base("drbirbdev.Sliming", "Sliming Skill", "Sliming", 9)  {}
    }
    internal class TravellingSkill : SpaceCoreSkill
    {
        public TravellingSkill() : base("Achtuur.Travelling", "Stardew Travel Skill", "Travelling", 10) {}
    }
    internal class ArchaeologySkill : SpaceCoreSkill
    {
        public ArchaeologySkill() : base("moonslime.Archaeology", "Archaeology Skill", "Archaeology", 11) {}
    }
    internal class SpookySkill : SpaceCoreSkill
    {
        public SpookySkill() : base("moonslime.SpookySkill", "Scaring/Thieving Skill", "Scaring", 12) {}
    }
