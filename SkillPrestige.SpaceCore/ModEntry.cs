using SkillPrestige.Mods;
using StardewModdingAPI;

namespace SkillPrestige.SpaceCore;

public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        foreach (var skill in SpaceCoreSkillRegistrations.AllSkillsRegistered.Where(skill => this.Helper.ModRegistry.IsLoaded(skill.SpaceCoreSkillId)))
            ModHandler.RegisterMod(skill);
    }
}
