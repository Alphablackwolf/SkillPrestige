using System;

namespace SkillPrestige.Commands
{
    /// <summary>A command that checks whether a mod is installed.</summary>
    /// // ReSharper disable once UnusedMember.Global - referenced via reflection
    internal class CheckForModCommand : SkillPrestigeCommand
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public CheckForModCommand()
            : base("checkformod", "Checks for the existence of a mod.\n\nUsage: checkformod <namespace> <modname>\n- namespace: the mod namespace.\n- mod: the mod name.", isTestCommand: true) { }

        /// <summary>Invokes the command when called from the console.</summary>
        /// <param name="args">The command-line arguments.</param>
        public override void Apply(string[] args)
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
