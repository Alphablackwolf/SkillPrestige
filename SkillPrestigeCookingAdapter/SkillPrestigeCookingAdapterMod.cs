using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace SkillPrestigeCookingAdapter
{
    public class SkillPrestigeCookingAdapterMod : Mod
    {
        private static Texture2D _cookingSkillTexture;
        private const string IconFilePath = @"Mods\CookingSkill\iconA.png";
        public static Texture2D GetCookingSkillTexture()
        {
            if (_cookingSkillTexture != null) return _cookingSkillTexture;
            try
            {
                var fileStream = new FileStream(IconFilePath, FileMode.Open);
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
    }
}
