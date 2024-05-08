using System;
using System.Collections.Generic;
using System.Linq;
using SkillPrestige.Logging;
using SkillPrestige.Mods;
using SkillPrestige.SkillTypes;

namespace SkillPrestige.Framework
{
    /// <summary>Represents a set of prestiges that are tied to skills.</summary>
    [Serializable]
    internal class PrestigeSet
    {
        // ReSharper disable once InconsistentNaming
        private static PrestigeSet _instance;

        public static PrestigeSet Instance
        {
            get {
                if (_instance is null && Read is not null) _instance = Read();
                return _instance;
            }
        }
        public static Action Save;
        public static Func<PrestigeSet> Read;

        public static void Load()
        {
            _instance ??= Read();
        }
        public static void UnLoad()
        {
            _instance = null;
        }
        public IEnumerable<Prestige> Prestiges { get; set; }

        /// <summary>the default prestige set that contains prestiges for each of the skills in the unmodded version Stardew Valley.</summary>
        private static List<Prestige> DefaultPrestiges =>
            new()
            {
                new Prestige
                {
                    SkillType = SkillType.Farming
                },
                new Prestige
                {
                    SkillType = SkillType.Mining
                },
                new Prestige
                {
                    SkillType = SkillType.Fishing
                },
                new Prestige
                {
                    SkillType = SkillType.Foraging
                },
                new Prestige
                {
                    SkillType = SkillType.Combat
                }
            };

        /// <summary>Returns all prestige set loaded and registered into this mod, default and mod.</summary>
        public static PrestigeSet CompleteEmptyPrestigeSet()
        {
                var prestiges = DefaultPrestiges;
                var addedPrestiges = ModHandler.GetAddedEmptyPrestiges().ToList();
                if (addedPrestiges.Any())
                    prestiges.AddRange(addedPrestiges);
                return new PrestigeSet
                {
                    Prestiges = prestiges
                };
        }
    }
}
