using System;

namespace SmapiManifestCreator
{
    /// <summary>
    /// Interface for any SMAPI mod that you wish to create a Manifest file name from.
    /// And yes, I decided on Manifestable. Deal with it.
    /// </summary>
    public interface ISmapiModManifestable
    {
        string ModName { get; }
        string Authour { get; }
        Version Version { get; }
        string Description { get; }
        Guid UniqueId { get; }
        bool PerSaveConfigs { get; }
        string EntryDll { get; }
    }
}
