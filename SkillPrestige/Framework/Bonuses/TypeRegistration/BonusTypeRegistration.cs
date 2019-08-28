using SkillPrestige.Bonuses;

namespace SkillPrestige.Framework.Bonuses.TypeRegistration
{
    internal abstract class BonusTypeRegistration : BonusType, IBonusTypeRegistration
    {
        /// <summary>Register available professions with the bonus type class.</summary>
        public abstract void RegisterBonusTypes();
    }
}
