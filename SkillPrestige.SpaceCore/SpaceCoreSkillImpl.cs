using SkillPrestige.Mods;
using SkillPrestige.Professions;
using SkillPrestige.SkillTypes;
using Microsoft.Xna.Framework;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;

namespace SkillPrestige.SpaceCore{
    internal class SpaceCoreSkillImpl : ISpaceCoreSkillMod
    {
        /// <summary>The skill type.</summary>
        private SkillType SkillType;

        /// <summary>The unique ID for the Skill mod.</summary>
        public string SpaceCoreSkillId { get; private set; }

        /// <summary>The name to display for the mod in the log.</summary>
        public string DisplayName { get; private set; }

        /// <summary>Whether the mod is found in SMAPI.</summary>
        public bool IsFound { get; private set; }

        /// <summary>The skills added by this mod.</summary>
        public IEnumerable<Skill> AdditionalSkills => this.GetAddedSkills();

        /// <summary>The prestiges added by this mod.</summary>
        public IEnumerable<Prestige> AdditionalPrestiges => this.GetAddedPrestiges();

        public SpaceCoreSkillImpl(string SpaceCoreSkillId, string DisplayName, string skillTypeName, int skillOrdinal){
            this.SkillType = new (skillTypeName, skillOrdinal);
            this.SpaceCoreSkillId = SpaceCoreSkillId;
            this.DisplayName = DisplayName;
            this.IsFound = true;
            
            ModHandler.RegisterMod(this);
        }

        /// <summary>Get the skills added by this mod.</summary>
        private IEnumerable<Skill> GetAddedSkills()
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

        /// <summary>Get the prestiges added by this mod.</summary>
        private IEnumerable<Prestige> GetAddedPrestiges()
        {
            yield return new Prestige
            {
                SkillType = this.SkillType
            };
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
}