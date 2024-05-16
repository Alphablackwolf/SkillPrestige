using System;
using System.Collections.Generic;
using SkillPrestige.Logging;
using StardewValley;

namespace SkillPrestige.Framework;

internal class SaveInformation : Dictionary<long, PrestigeSet>
{
    public static SaveInformation Instance { get; set; }

    public static void MigrateLoadedPrestigeSet()
    {
        Instance = new SaveInformation { { Game1.player.UniqueMultiplayerID, PrestigeSet.Instance } };
    }

    public static Action Save;
    public static Func<SaveInformation> Read;

    public static bool TryLoad()
    {
        try
        {
            Instance ??= Read();
            return true;
        }
        catch(Exception exception)
        {
            Logger.LogInformation($"attempted data read for multiplayer data, read failed: {Environment.NewLine} {exception.Message} {Environment.NewLine} {exception.StackTrace}");
            return false;
        }
    }

    public static void UnLoad()
    {
        Instance = null;
    }
}
