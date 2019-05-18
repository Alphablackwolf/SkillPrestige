using Magic;
using SkillPrestige.Professions;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillPrestige.MagicSkill.Framework
{
    /// <summary>Special handling for adding to the mana cap from a Magic profession..</summary>
    internal class ManaCapSpecialHandling : IProfessionSpecialHandling
    {
        public readonly int Amount;

        public ManaCapSpecialHandling(int amount)
        {
            this.Amount = amount;
        }

        public void ApplyEffect()
        {
            Game1.player.setMaxMana(Game1.player.getMaxMana() + Amount);
        }

        public void RemoveEffect()
        {
            Game1.player.setMaxMana(Game1.player.getMaxMana() - Amount);
        }
    }
}
