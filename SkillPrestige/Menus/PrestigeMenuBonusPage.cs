using System;
using Microsoft.Xna.Framework;
using SkillPrestige.Bonuses;
using SkillPrestige.Professions;
using StardewValley;

namespace SkillPrestige.Menus
{
    internal class PrestigeMenuBonusPage : MenuPage
    {
        private readonly int _rowPadding = Game1.tileSize / 3;
        private static int Offset => 4 * Game1.pixelZoom;
        private const string PointText = "Prestige Points:";
        private readonly Skill _skill;
        private readonly Prestige _prestige;

        public PrestigeMenuBonusPage(Skill skill, Prestige prestige, Rectangle dialogBounds)
        {
            PageIndex = 1;
            _skill = skill;
            _prestige = prestige;
            SetupButtons();
        }

        private void SetupButtons()
        {
            
        }

        private static int BonusButtonWidth(BonusType bonus)
        {
            return Game1.dialogueFont.MeasureString(string.Join(Environment.NewLine, bonus.Name.Split(' '))).X.Ceiling() + Offset * 2;
        }

        private static int BonusButtonHeight(BonusType bonus)
        {
            var textHeight = Game1.dialogueFont.MeasureString(string.Join(Environment.NewLine, bonus.Name.Split(' '))).Y.Ceiling();
            return Offset * 3 + textHeight;
        }
    }
}
