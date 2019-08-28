namespace SkillPrestige.Bonuses.TypeRegistration
{
    public abstract class BonusTypeRegistration : BonusType, IBonusTypeRegistration
    {
        /// <summary>Register available professions with the bonus type class.</summary>
        public abstract void RegisterBonusTypes();
    }
}
