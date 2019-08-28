namespace SkillPrestige.Framework.Commands
{
    /// <summary>A command that checks whether a mod is installed.</summary>
    // ReSharper disable once UnusedMember.Global - referenced via reflection
    internal class CheckForModCommand : SkillPrestigeCommand
    {
        public CheckForModCommand()
            : base("checkformod", "Checks for the existence of a mod.\n\nUsage: checkformod <uniqueId>\n- uniqueId: the mod's uniqueId as found in the manifest.") { }

        protected override bool TestingCommand => true;

        protected override void Apply(string[] args)
        {
            if (args.Length < 1)
            {
                ModEntry.LogMonitor.Log("<uniqueid> must be specified");
                return;
            }
            string uniqueIdArgument = args[0];
            ModEntry.LogMonitor.Log($"mod {uniqueIdArgument} {(ModEntry.ModRegistry.IsLoaded(uniqueIdArgument) ? string.Empty : "not ")}found.");
        }
    }
}
