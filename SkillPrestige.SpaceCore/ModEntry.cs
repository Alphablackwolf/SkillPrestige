using Microsoft.Xna.Framework;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;

namespace SkillPrestige.SpaceCore
{
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
                var cookingSkill = new SpaceCoreSkillImpl("moonslime.Cooking", "Yet Another Cooking Skill", "Cooking", 6);
                this.skillList.Add(cookingSkill);
            }
        }

    }
}
