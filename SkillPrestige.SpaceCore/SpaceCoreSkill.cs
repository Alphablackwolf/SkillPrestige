using Microsoft.Xna.Framework;
using SkillPrestige.Mods;
using SkillPrestige.Professions;
using SkillPrestige.SkillTypes;
using SpaceCore;
using StardewValley;

namespace SkillPrestige.SpaceCore;

public class SpaceCoreSkill : ISpaceCoreSkillMod
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private SkillType SkillType;
    public string DisplayName { get; }
    public IEnumerable<Skill> AdditionalSkills {
        get
        {
            yield return new Skill
            {
                Type = this.SkillType,
                SourceRectangleForSkillIcon = new Rectangle(0, 0, 16, 16),
                SkillIconTexture = Skills.GetSkillIcon(this.SpaceCoreSkillId),
                Professions = this.GetAddedProfessions(),
                GetSkillLevel = () => Skills.GetSkillLevel(Game1.player, this.SpaceCoreSkillId),
                SetSkillLevel = _ => { }, //is not set independently of the experience.
                GetSkillExperience = this.GetExperience,
                SetSkillExperience = this.SetExperience
            };
        }
    }

    public IEnumerable<Prestige> AdditionalPrestiges
    {
        get
        {
            yield return new Prestige
            {
                SkillType = this.SkillType
            };
        }
    }

    public bool IsFound { get; }
    public string SpaceCoreSkillId { get; }

    public SpaceCoreSkill(string spaceCoreSkillId, string displayName, string skillTypeName, int skillOrdinal)
    {
        this.SpaceCoreSkillId = spaceCoreSkillId;
        this.DisplayName = displayName;
        this.SkillType = new SkillType(skillTypeName, skillOrdinal);
        this.IsFound = true;
    }

    private IEnumerable<Profession> GetAddedProfessions()
        {
            var skill = Skills.GetSkill(this.SpaceCoreSkillId);
            IList<Profession> professions = new List<Profession>();
            IList<TierOneProfession> tierOne = new List<TierOneProfession>();
            foreach (var professionGroup in skill.ProfessionsForLevels)
                switch (professionGroup.Level)
                {
                    case 5:
                    {
                        var professionA = new SpaceCoreTierOneProfession(professionGroup.First);
                        var professionB = new SpaceCoreTierOneProfession(professionGroup.Second);
                        professions.Add(professionA);
                        professions.Add(professionB);
                        tierOne.Add(professionA);
                        tierOne.Add(professionB);
                        break;
                    }
                    case 10:
                    {
                        var requiredProfession = tierOne.First(p => p.DisplayName == professionGroup.Requires.GetName());
                        var professionA = new SpaceCoreTierTwoProfession(professionGroup.First)
                        {
                            TierOneProfession = requiredProfession,
                        };
                        var professionB = new SpaceCoreTierTwoProfession(professionGroup.Second)
                        {
                            TierOneProfession = requiredProfession,
                        };
                        professions.Add(professionA);
                        professions.Add(professionB);
                        requiredProfession.TierTwoProfessions = new[] { professionA, professionB };
                        break;
                    }
                }
            return professions;
        }
        /// <summary>Get the current skill XP.</summary>
        private int GetExperience()
        {
            return Skills.GetExperienceFor(Game1.player, this.SpaceCoreSkillId);
        }
        /// <summary>Set the current skill XP.</summary>
        /// <param name="amount">The amount to set.</param>
        private void SetExperience(int amount)
        {
            int addedExperience = amount - Game1.player.GetCustomSkillExperience(this.SpaceCoreSkillId);
            Game1.player.AddCustomSkillExperience(this.SpaceCoreSkillId, addedExperience);
        }
}
