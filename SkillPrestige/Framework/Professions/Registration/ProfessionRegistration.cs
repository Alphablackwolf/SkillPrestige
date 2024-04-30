using SkillPrestige.Professions;

namespace SkillPrestige.Framework.Professions.Registration
{
    internal abstract class ProfessionRegistration : Profession, IProfessionRegistration
    {
        /// <summary>Returns a level available at of 0, as this class is used solely to handle registration of static members of it's base class that are all declared in partial classes.</summary>
        public override int LevelAvailableAt => 0;

        /// <summary>Register available professions with the profession class.</summary>
        public abstract void RegisterProfessions();
    }
}
