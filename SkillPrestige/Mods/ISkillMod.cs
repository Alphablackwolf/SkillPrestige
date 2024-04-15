using System.Collections.Generic;
using SkillPrestige.PrestigeFramework;
using SkillPrestige.Skills;

namespace SkillPrestige.Mods
{
    /// <summary>
    /// Interface that all skill mods need to implement in order to register with Skill Prestige
    /// </summary>
    public interface ISkillMod
    {
        /// <summary>
        /// The name to display for the mod in the log.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Skills to add to the prestige system.
        /// </summary>
        IEnumerable<Skill> AdditionalSkills { get; }

        /// <summary>
        /// Empty objects that contain a skill type for the saved data.
        /// </summary>
        IEnumerable<Prestige> AdditonalPrestiges { get; }

        bool IsFound { get; }
    }
}
