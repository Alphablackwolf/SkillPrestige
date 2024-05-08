namespace SkillPrestige.Mods
{
    /// <summary>Interface that all skill mods need to implement in order to register with Skill Prestige.</summary>
    public interface ISpaceCoreSkillMod : ISkillMod
    {
        //ID to lookup space core skill
        string SpaceCoreSkillId { get; }
    }
}
