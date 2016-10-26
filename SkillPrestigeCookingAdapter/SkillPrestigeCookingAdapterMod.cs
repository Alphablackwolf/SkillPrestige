using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using SkillPrestige;

namespace SkillPrestigeCookingAdapter
{
    public class SkillPrestigeCookingAdapterMod : Mod
    {
        private static Texture2D _cookingSkillTexture;
        private const string IconFileName = @"iconA.png";
        public static Texture2D GetCookingSkillTexture()
        {
            if (_cookingSkillTexture != null) return _cookingSkillTexture;
            try
            {
                var cookingSkillPath = GetCookingSkillModPath();
                var fileStream = new FileStream(Path.Combine(cookingSkillPath, IconFileName), FileMode.Open);
                _cookingSkillTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, fileStream);
            }
            catch (Exception ex)
            {
                Log.Async("[Skill Prestige Cooking Adapter] failed to load icon: " + ex);
                _cookingSkillTexture = new Texture2D(Game1.graphics.GraphicsDevice, 16, 16);
                _cookingSkillTexture.SetData(Enumerable.Range(0, 256).Select(i => new Color(225, 168, byte.MaxValue)).ToArray());
            }
            return _cookingSkillTexture;
        }

        private static string GetCookingSkillModPath()
        {
            var type = typeof(Program);
            var info = type.GetField("_modPaths", BindingFlags.NonPublic | BindingFlags.Static);
            var modPaths = (List<string>)info.GetValue(null);
            foreach (var modPath in modPaths)
            {
                foreach (var directory in Directory.GetDirectories(modPath))
                {
                    foreach (var file in Directory.GetFiles(directory, "manifest.json"))
                    {
                        if (file.Contains("StardewInjector")) continue;
                        var baseConfig = new Manifest();
                        Manifest manifest;
                        try
                        {
                            if (string.IsNullOrEmpty(File.ReadAllText(file)))
                            {
                                continue;
                            }
                            manifest = baseConfig.InitializeConfig(file);
                            if (string.IsNullOrEmpty(manifest.EntryDll))
                            {
                                continue;
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        var directoryName = Path.GetDirectoryName(file);
                        if (manifest.UniqueID == "CookingSkill")
                        {
                            return directoryName;
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}
