using System;
using System.Linq;
using SkillPrestige.Logging;
using StardewModdingAPI;

namespace SkillPrestige.Commands
{
    /// <summary>A command called in the SMAPI console.</summary>
    internal abstract class SkillPrestigeCommand
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name used to call the command in the console.</summary>
        private string Name { get; }

        /// <summary>The help description of the command.</summary>
        private string Description { get; }

        /// <summary>Whether the command is used only in test mode.</summary>
        protected bool IsTestCommand { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Registers all commands found in the system.</summary>
        /// <param name="helper">The SMAPI console command helper.</param>
        /// <param name="testCommands">Whether or not you wish to only register testing commands.</param>
        public static void RegisterCommands(ICommandHelper helper, bool testCommands)
        {
            var concreteCommands = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypesSafely())
                .Where(x => x.IsSubclassOf(typeof(SkillPrestigeCommand)) && !x.IsAbstract);
            foreach (var commandType in concreteCommands)
            {
                var command = (SkillPrestigeCommand)Activator.CreateInstance(commandType);
                if (!(testCommands ^ command.IsTestCommand)) command.RegisterCommand(helper);
            }
        }

        /// <summary>Invokes the command when called from the console.</summary>
        /// <param name="args">The command-line arguments.</param>
        public abstract void Apply(string[] args);


        /*********
        ** Protected methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="name">The name used to call the command in the console.</param>
        /// <param name="description">The help description of the command.</param>
        /// <param name="isTestCommand">Whether the command is used only in test mode.</param>
        protected SkillPrestigeCommand(string name, string description, bool isTestCommand)
        {
            Name = name;
            Description = description;
            this.IsTestCommand = isTestCommand;
        }

        /// <summary>Registers a command with the SMAPI console.</summary>
        /// <param name="helper">The SMAPI command helper.</param>
        private void RegisterCommand(ICommandHelper helper)
        {
            Logger.LogInformation($"Registering {Name} command...");
            helper.Add(Name, Description, (name, args) => Apply(args));
            Logger.LogInformation($"{Name} command registered.");
        }
    }
}
