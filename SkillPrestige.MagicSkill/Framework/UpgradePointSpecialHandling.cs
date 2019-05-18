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
    internal class UpgradePointSpecialHandling : IProfessionSpecialHandling
    {
        public readonly int Amount;

        public UpgradePointSpecialHandling(int amount)
        {
            this.Amount = amount;
        }

        public void ApplyEffect()
        {
            Game1.player.useSpellPoints(-Amount, true);
        }

        public void RemoveEffect()
        {
            Game1.player.useSpellPoints(Amount, true);
        }
    }
}
