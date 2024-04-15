using SkillPrestige.Logging;
using SkillPrestige.Skills.Types;
using SkillType = SkillPrestige.Skills.Types.SkillType;

namespace SkillPrestige.Mods.MyLuckSkill
{
    // ReSharper disable once UnusedMember.Global - referenced through reflection.
    public sealed class LuckSkillTypeRegistration : SkillType, ISkillTypeRegistration
    {
        public void RegisterSkillTypes()
        {
            Logger.LogInformation("Registering Luck skill type.");
            Luck = new SkillType("Luck", 5);
            Logger.LogInformation("Luck skill type registered.");
        }
    }
}

