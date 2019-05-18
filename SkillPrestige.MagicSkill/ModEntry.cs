using System;
using System.Collections.Generic;
using System.Linq;
using Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.MagicSkill.Framework;
using SkillPrestige.Menus;
using SkillPrestige.Mods;
using SkillPrestige.Professions;
using SkillPrestige.SkillTypes;
using SpaceCore;
using SpaceCore.Interface;
using StardewModdingAPI;
using StardewValley;

namespace SkillPrestige.MagicSkill
{
    /// <summary>The mod entry class.</summary>
    public class ModEntry : StardewModdingAPI.Mod, ISkillMod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The cooking skill icon.</summary>
        private Texture2D IconTexture;

        /// <summary>The cooking skill type.</summary>
        private SkillType MagicSkillType;

        /// <summary>Whether the Cooking Skill mod is loaded.</summary>
        private bool IsCookingSkillModLoaded;

        /// <summary>Whether the Luck Skill mod is loaded.</summary>
        private bool IsLuckSkillModLoaded;


        /*********
        ** Accessors
        *********/
        /// <summary>The name to display for the mod in the log.</summary>
        public string DisplayName { get; } = "Magic Skill";

        /// <summary>Whether the mod is found in SMAPI.</summary>
        public bool IsFound { get; private set; }

        /// <summary>The skills added by this mod.</summary>
        public IEnumerable<Skill> AdditionalSkills => this.GetAddedSkills();

        /// <summary>The prestiges added by this mod.</summary>
        public IEnumerable<Prestige> AdditonalPrestiges => this.GetAddedPrestiges();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.IconTexture = helper.Content.Load<Texture2D>("icon.png");
            this.MagicSkillType = new SkillType("Magic", 8);
            this.IsFound = helper.ModRegistry.IsLoaded("spacechase0.Magic");
            this.IsCookingSkillModLoaded = helper.ModRegistry.IsLoaded("Alphablackwolf.CookingSkillPrestigeAdapter");
            this.IsLuckSkillModLoaded = helper.ModRegistry.IsLoaded("alphablackwolf.LuckSkillPrestigeAdapter");

            ModHandler.RegisterMod(this);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the skills added by this mod.</summary>
        private IEnumerable<Skill> GetAddedSkills()
        {
            if (!this.IsFound)
                yield break;

            int skillPos = 6;
            if (this.IsLuckSkillModLoaded)
                ++skillPos;
            if (this.IsCookingSkillModLoaded)
                ++skillPos;
            
            yield return new Skill
            {
                Type = this.MagicSkillType,
                SkillScreenPosition = skillPos, // fix potential conflict with order due to luck skill mod and cooking skill mod
                SourceRectangleForSkillIcon = new Rectangle(0, 0, 16, 16),
                SkillIconTexture = this.IconTexture,
                Professions = this.GetAddedProfessions(),
                SetSkillLevel = x => { }, // no set necessary, as the level isn't stored independently from the experience
                GetSkillLevel = this.GetMagicLevel,
                SetSkillExperience = SetMagicExperience,
                OnPrestige = OnPrestige,
                LevelUpManager = new LevelUpManager
                {
                    MenuType = typeof(SkillLevelUpMenu),
                    GetLevel = () => Game1.player.GetCustomSkillLevel(SpaceCore.Skills.GetSkill("spacechase0.Magic")),
                    GetSkill = () => Skill.AllSkills.Single(x => x.Type == this.MagicSkillType),
                    CreateNewLevelUpMenu = (skill, level) => new LevelUpMenuDecorator<SkillLevelUpMenu>(skill, level, new SkillLevelUpMenu("spacechase0.Magic", level),
                        "professionsToChoose", "leftProfessionDescription", "rightProfessionDescription", SkillLevelUpMenu.getProfessionDescription)
                }
            };
        }

        /// <summary>Get the prestiges added by this mod.</summary>
        private IEnumerable<Prestige> GetAddedPrestiges()
        {
            if (!this.IsFound)
                yield break;

            yield return new Prestige
            {
                SkillType = this.MagicSkillType
            };
        }

        /// <summary>Get the professions added by this mod.</summary>
        private IEnumerable<Profession> GetAddedProfessions()
        {
            var skill = SpaceCore.Skills.GetSkill("spacechase0.Magic");

            List<Profession> profs = new List<Profession>();
            List<TierOneProfession> tier1 = new List<TierOneProfession>();
            foreach ( var sprofLevel in skill.ProfessionsForLevels )
            {
                if (sprofLevel.Level == 5)
                {
                    var a = new TierOneProfession()
                    {
                        DisplayName = sprofLevel.First.GetName(),
                        Id = sprofLevel.First.GetVanillaId(),
                        EffectText = new[] { sprofLevel.First.GetDescription() },
                    };
                    profs.Add(a);
                    tier1.Add(a);

                    var b = new TierOneProfession()
                    {
                        DisplayName = sprofLevel.Second.GetName(),
                        Id = sprofLevel.Second.GetVanillaId(),
                        EffectText = new[] { sprofLevel.Second.GetDescription() },
                    };
                    profs.Add(b);
                    tier1.Add(b);
                }
                else if ( sprofLevel.Level == 10 )
                {
                    TierOneProfession req = null;
                    foreach ( var t1 in tier1 )
                    {
                        if ( t1.DisplayName == sprofLevel.Requires.GetName() )
                        {
                            req = t1;
                            break;
                        }
                    }

                    var a = new TierTwoProfession()
                    {
                        DisplayName = sprofLevel.First.GetName(),
                        Id = sprofLevel.First.GetVanillaId(),
                        EffectText = new[] { sprofLevel.First.GetDescription() },
                        TierOneProfession = req,
                    };
                    profs.Add(a);

                    var b = new TierTwoProfession()
                    {
                        DisplayName = sprofLevel.Second.GetName(),
                        Id = sprofLevel.Second.GetVanillaId(),
                        EffectText = new[] { sprofLevel.Second.GetDescription() },
                        TierOneProfession = req,
                    };
                    profs.Add(b);

                    req.TierTwoProfessions = new[] { a, b };
                }
            }

            foreach ( var prof in profs )
            {
                if (prof.DisplayName == "Mana Reserve")
                    prof.SpecialHandling = new ManaCapSpecialHandling(500);
                else if (prof.DisplayName == "Potential" || prof.DisplayName == "Prodigy")
                    prof.SpecialHandling = new UpgradePointSpecialHandling(2);
            }

            return profs;
        }

        /// <summary>Get the current cooking skill level.</summary>
        private int GetMagicLevel()
        {
            //this.FixExpLength();
            return Game1.player.GetCustomSkillLevel("spacechase0.Magic");
        }

        /// <summary>Set the current cooking skill XP.</summary>
        /// <param name="amount">The amount to set.</param>
        private static void SetMagicExperience(int amount)
        {
            var addedExperience = amount - Game1.player.GetCustomSkillExperience("spacechase0.Magic");
            Game1.player.AddCustomSkillExperience("spacechase0.Magic", addedExperience);
        }

        /// <summary>
        /// Reset the upgrade points of the player on prestige.
        /// Points from professions are handled in UpgradePointSpecialHandling.
        /// </summary>
        private static void OnPrestige()
        {
            var spells = Game1.player.getSpellBook();
            foreach ( var spell in new Dictionary<string, int>(spells.knownSpells) )
            {
                if ( spell.Value > 0 )
                {
                    Game1.player.forgetSpell(spell.Key, 1, sync: false);
                }
            }
            Game1.player.useSpellPoints(10, sync: true);
        }
    }
}
