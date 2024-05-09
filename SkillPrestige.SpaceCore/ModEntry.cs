using Microsoft.Xna.Framework;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;

namespace SkillPrestige.SpaceCore
{
    internal class CookingSkillImpl : SpaceCoreSkillImpl{
        public CookingSkillImpl () : base("moonslime.Cooking", "Yet Another Cooking Skill", "Cooking", 6) {}
    }

    internal class BinningSkillImpl : SpaceCoreSkillImpl{
        public BinningSkillImpl () : base("drbirbdev.Binning", "BinningSkill", "Binning", 7) {}
    }

    internal class SocializingSkillImpl : SpaceCoreSkillImpl{
        public SocializingSkillImpl () : base("drbirbdev.Socializing", "SocializingSkill", "Socializing", 8) {}
    }

    internal class SlimingSkillImpl : SpaceCoreSkillImpl{
        public SlimingSkillImpl () : base("drbirbdev.Sliming", "SlimingSkill", "Sliming", 9) {}
    }

    internal class TravelSkillImpl : SpaceCoreSkillImpl{
        public TravelSkillImpl () : base("Achtuur.Travelling", "Stardew Travel Skill", "Travelling", 10) {}
    }

    internal class ArchaeologySkillImpl : SpaceCoreSkillImpl{
        public ArchaeologySkillImpl () : base("moonslime.Archaeology", "ArchaeologySkill", "Archaeology", 11) {}
    }

    /// <summary>The mod entry class.</summary>
    internal class ModEntry : Mod
    {

        private List<SpaceCoreSkillImpl> skillList;
        
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.skillList = new List<SpaceCoreSkillImpl>();

            if (helper.ModRegistry.IsLoaded("moonslime.CookingSkill")){
                this.skillList.Add(new CookingSkillImpl());
            }

            if (helper.ModRegistry.IsLoaded("drbirbdev.BinningSkill")){
                this.skillList.Add(new BinningSkillImpl());
            }

            if (helper.ModRegistry.IsLoaded("drbirbdev.SocializingSkill")){
                this.skillList.Add(new SocializingSkillImpl());
            }

            if (helper.ModRegistry.IsLoaded("drbirbdev.SlimingSkill")){
                this.skillList.Add(new SlimingSkillImpl());
            }

            if (helper.ModRegistry.IsLoaded("Achtuur.StardewTravelSkill")){
                this.skillList.Add(new TravelSkillImpl());
            }

            if (helper.ModRegistry.IsLoaded("moonslime.ArchaeologySkill")){
                this.skillList.Add(new ArchaeologySkillImpl());
            }


        }

    }
}
