using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SkillPrestige.Professions;
using StardewValley;

namespace SkillPrestige.Mods.LuckSkill
{
    /// <summary>
    /// The Luck Skill Mod's representation in SkillPrestige.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global - referenced through reflection.
    public class LuckSkillMod : SkillMod
    {
        public override string DisplayName => "Luck Skill";
        protected override string Namespace => "LuckSkill";
        protected override string ClassName => "LuckSkillMod";

        public override IEnumerable<Skill> AdditionalSkills => IsFound
            ? new List<Skill>
            {
                new Skill
                {
                    Type = SkillTypes.SkillType.Luck,
                    SkillScreenPosition = 6,
                    SourceRectangleForSkillIcon = new Rectangle(64, 0, 16, 16),
                    Professions = Profession.LuckProfessions,
                    SetSkillLevel = x => Game1.player.luckLevel = x,
                    GetSkillLevel = () => Game1.player.luckLevel
                }
            }
            : null;

        public override IEnumerable<Prestige> AdditonalPrestiges => IsFound ? new List<Prestige>
        {
            new Prestige
            {
                SkillType = SkillTypes.SkillType.Luck
            }
        } : null;
    }
}
