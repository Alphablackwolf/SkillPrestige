using System;

namespace SkillPrestige.Commands
{
    /// <summary>
    /// A command that checks wehther a mod is installed
    /// </summary>
    /// // ReSharper disable once UnusedMember.Global - referenced via reflection
    internal class CheckForModCommand : SkillPrestigeCommand
    {

        public CheckForModCommand() : base("checkformod", "Checks for the existence of a mod.\n\nUsage: checkformod <namespace> <modname>\n- namespace: the mod namespace.\n- mod: the mod name.") { }

        protected override bool TestingCommand => true;

        protected override void Apply(string[] args)
        {
            if (args.Length <= 1)
            {
                SkillPrestigeMod.LogMonitor.Log("<namespace> and <modname> must be specified");
                return;
            }
            var namespaceArgument = args[0];
            var modNameArgument = args[1];
            SkillPrestigeMod.LogMonitor.Log($"mod {modNameArgument} {(Type.GetType($"{namespaceArgument}.{modNameArgument}, {namespaceArgument}") == null ? "not " : string.Empty)}found.");
        }
    }
}
