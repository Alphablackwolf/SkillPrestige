using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Menus;
using SkillPrestige.Mods;
using SkillPrestige.Professions;
using SkillPrestige.SkillTypes;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige
{
    /// <summary>Represents a skill in Stardew Valley.</summary>
    public class Skill
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Metadata about a skill type.</summary>
        public SkillType Type { get; set; }

        /// <summary>The professions for this skill.</summary>
        public IEnumerable<Profession> Professions { get; set; }

        /// <summary>The location of the texture on the buffsIcons sprite sheet.</summary>
        public Rectangle SourceRectangleForSkillIcon { get; set; }

        /// <summary>The texture for the skill icon.</summary>
        public Texture2D SkillIconTexture { get; set; } = Game1.buffsIcons;

        /// <summary>The one-based index of where the skill appears on the screen (e.g. Farming is 1 and Fishing is 4). If your mod is creating a skill, you will need to detect where in the list your mod's skill(s) will be.</summary>
        public int SkillScreenPosition { get; set; }

        /// <summary>An action to set the skill's level. For the unmodded game, this sets the relevant player field (e.g. <see cref="Farmer.farmingLevel"/>). If you are implementing this class for your mod it should be whatever would be needed to set the skill level to a given integer.</summary>
        public Action<int> SetSkillLevel;

        /// <summary>A function to return the skill's level. For the unmodded game. this gets the relevant player field (e.g. <see cref="Farmer.farmingLevel"/>). If you are implementing this class for your mod it should be whatever would be needed to retrieve the player's current skill level.</summary>
        public Func<int> GetSkillLevel;

        /// <summary>An action to get the skill's experience. For the unmodded game, this updates the <see cref="Farmer.experiencePoints"/> array based on <see cref="SkillType.Ordinal"/>. If you are implementing this class for your mod it should be whatever would be needed to set the skill experience level to a given integer.</summary>
        // ReSharper disable once MemberCanBePrivate.Global used by other mods.
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public Action<int> SetSkillExperience { get; set; }

        /// <summary>An action to get the skill's experience. For the unmodded game, this reads the <see cref="Farmer.experiencePoints"/> array based on <see cref="SkillType.Ordinal"/>.</summary>
        public Func<int> GetSkillExperience { get; set; }

        /// <summary>An action triggered when prestiging is done. This allows extra handling if something else needs to be reset.</summary>
        public Action OnPrestige { get; set; }

        /// <summary>The management class for any level up menu.</summary>
        public LevelUpManager LevelUpManager { get; set; }

        /// <summary>The default skills available in the unmodded game.</summary>
        public static IEnumerable<Skill> DefaultSkills => new List<Skill>
        {
            new Skill
            {
                Type = SkillType.Farming,
                SkillScreenPosition = 1,
                SourceRectangleForSkillIcon = new Rectangle(0, 0, 16, 16),
                Professions = Profession.FarmingProfessions,
                SetSkillLevel = level => Game1.player.farmingLevel.Value = level,
                GetSkillLevel = () => Game1.player.farmingLevel.Value
            },
            new Skill
            {
                Type = SkillType.Fishing,
                SkillScreenPosition = 4,
                SourceRectangleForSkillIcon = new Rectangle(16, 0, 16, 16),
                Professions = Profession.FishingProfessions,
                SetSkillLevel = level => Game1.player.fishingLevel.Value = level,
                GetSkillLevel = () => Game1.player.fishingLevel.Value
            },
            new Skill
            {
                Type = SkillType.Foraging,
                SkillScreenPosition = 3,
                SourceRectangleForSkillIcon = new Rectangle(80, 0, 16, 16),
                Professions = Profession.ForagingProfessions,
                SetSkillLevel = level => Game1.player.foragingLevel.Value = level,
                GetSkillLevel = () => Game1.player.foragingLevel.Value
            },
            new Skill
            {
                Type = SkillType.Mining,
                SkillScreenPosition = 2,
                SourceRectangleForSkillIcon = new Rectangle(32, 0, 16, 16),
                Professions = Profession.MiningProfessions,
                SetSkillLevel = level => Game1.player.miningLevel.Value = level,
                GetSkillLevel = () => Game1.player.miningLevel.Value
            },
            new Skill
            {
                Type = SkillType.Combat,
                SkillScreenPosition = 5,
                SourceRectangleForSkillIcon = new Rectangle(128, 16, 16, 16),
                Professions = Profession.CombatProfessions,
                SetSkillLevel = level => Game1.player.combatLevel.Value = level,
                GetSkillLevel = () => Game1.player.combatLevel.Value
            }
        };

        /// <summary>Returns all skills loaded and registered into this mod, default and mod.</summary>
        public static IEnumerable<Skill> AllSkills
        {
            get
            {
                var skills = new List<Skill>(DefaultSkills);
                var addedSkills = ModHandler.GetAddedSkills().ToList();
                if (addedSkills.Any())
                    skills.AddRange(addedSkills);
                return skills;
            }
        }

        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public Skill()
        {
            this.GetSkillExperience = () => Game1.player.experiencePoints[this.Type.Ordinal];
            this.SetSkillExperience = exp =>
            {
                Game1.player.experiencePoints[this.Type.Ordinal] = 0;
                Game1.player.gainExperience(this.Type.Ordinal, exp);
            };
            this.LevelUpManager = new LevelUpManager
            {
                IsMenu = menu => {
                    if (menu.GetType() != typeof(LevelUpMenu))
                    {
                        return false;
                    }
                    int currentSkill = (int)(Game1.activeClickableMenu as LevelUpMenu).GetInstanceField("currentSkill");
                    if (currentSkill == this.Type.Ordinal)
                    {
                        return true;
                    }
                    return false;
                },
                GetLevel = () => (int)(Game1.activeClickableMenu as LevelUpMenu).GetInstanceField("currentLevel"),
                GetSkill = () => this,
                CreateNewLevelUpMenu = (skill, level) => new LevelUpMenuDecorator<LevelUpMenu>(
                    skill: skill,
                    level: level,
                    internalMenu: new LevelUpMenu(skill.Type.Ordinal, level),
                    professionsToChooseInternalName: "professionsToChoose",
                    leftProfessionDescriptionInternalName: "leftProfessionDescription",
                    rightProfessionDescriptionInternalName: "rightProfessionDescription",
                    getProfessionDescription: LevelUpMenu.getProfessionDescription
                )
            };
        }

        /// <summary>Get the unique IDs for the skill's professions.</summary>
        public IEnumerable<int> GetAllProfessionIds()
        {
            return this.Professions.Select(x => x.Id);
        }
    }
}
