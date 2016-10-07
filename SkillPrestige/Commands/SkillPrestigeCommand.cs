using System;
using System.Collections.Generic;
using System.Linq;
using SkillPrestige.Logging;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace SkillPrestige.Commands
{
    /// <summary>
    /// Represents a command called in the SMAPI console interface.
    /// </summary>
    internal abstract class SkillPrestigeCommand
    {
        /// <summary>
        /// The name used to call the command in the console.
        /// </summary>
        private string Name { get; }

        /// <summary>
        /// The help description of the command.
        /// </summary>
        private string Description { get; }

        /// <summary>
        /// The help description of the aguments in the command.
        /// </summary>
        private IEnumerable<string> ArgumentDescriptions { get; }

        /// <summary>
        /// Whether or not the command is used only in test mode.
        /// </summary>
        protected abstract bool TestingCommand { get; }

        protected SkillPrestigeCommand(string name, string description, IEnumerable<string> argumentDescriptions = null)
        {
            Name = name;
            Description = description;
            ArgumentDescriptions = argumentDescriptions;
        }

        /// <summary>
        /// Registers a command with the SMAPI console.
        /// </summary>
        private void RegisterCommand()
        {
            Logger.LogInformation($"Registering {Name} command...");
            if (ArgumentDescriptions != null)
            {
                Command.RegisterCommand(Name, Description, ArgumentDescriptions.ToArray()).CommandFired += ApplyCommandEffect;
            }
            else
            {
                Command.RegisterCommand(Name, Description).CommandFired += ApplyCommandEffect;
            }
            Logger.LogInformation($"{Name} command registered.");
        }

        /// <summary>
        /// Applies the effect of a command when it is called from the console.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void ApplyCommandEffect(object sender, EventArgsCommand e);

        /// <summary>
        /// Registers all commands found in the system.
        /// </summary>
        /// <param name="testCommands">Whether or not you wish to only register testing commands.</param>
        public static void RegisterCommands(bool testCommands)
        {
            var concreteCommands = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypesSafely())
                .Where(x => x.IsSubclassOf(typeof(SkillPrestigeCommand)) && !x.IsAbstract);
            foreach (var commandType in concreteCommands)
            {
                var command = (SkillPrestigeCommand)Activator.CreateInstance(commandType);
                if (!(testCommands ^ command.TestingCommand)) command.RegisterCommand();
            }
        }
    }
}
