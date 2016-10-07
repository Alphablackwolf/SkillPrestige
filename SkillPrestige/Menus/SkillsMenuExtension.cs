using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.InputHandling;
using SkillPrestige.Logging;
using SkillPrestige.Menus.Buttons;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Menus
{
    /// <summary>
    /// Extends the skills menu in Stardew Valley to add prestige buttons next to the skills.
    /// </summary>
    internal static class SkillsMenuExtension
    {
        private static bool _skillsMenuInitialized;
        private static readonly IDictionary<Skill, SkillsMenuPrestigeButton> PrestigeButtons = new Dictionary<Skill, SkillsMenuPrestigeButton>();

        // ReSharper disable once SuggestBaseTypeForParameter - we specifically want the skills page here, even if our usage could work against IClickableMenu.
        private static void IntializeSkillsMenu(SkillsPage skillsPage)
        {
            Logger.LogVerbose("Initializing Skills Menu...");
            _skillsMenuInitialized = true;
            foreach (var skill in Skill.AllSkills)
            {
                var width = 45 * Game1.pixelZoom;
                var height = 12 * Game1.pixelZoom;
                var yOffset = (int)Math.Floor(Game1.tileSize / 7.1);
                var yPadding = (int)Math.Floor(Game1.tileSize / 1.1);
                var xOffset = skillsPage.width + Game1.tileSize / 2;
                var bounds = new Rectangle(skillsPage.xPositionOnScreen + xOffset, skillsPage.yPositionOnScreen + yPadding + yOffset * skill.SkillScreenPosition + skill.SkillScreenPosition * height, width, height);
                var prestigeButton = new SkillsMenuPrestigeButton
                {
                    Skill = skill,
                    Bounds = bounds
                };
                PrestigeButtons.Add(skill, prestigeButton);
                Mouse.MouseMoved += prestigeButton.CheckForMouseHover;
                Mouse.MouseClicked += prestigeButton.CheckForMouseClick;
                Logger.LogVerbose("Skills Menu initialized.");
            }
        }

        private static void UnloadSkillsPageAdditions()
        {
            Logger.LogVerbose("Unloading Skills Menu.");
            _skillsMenuInitialized = false;
            foreach (var button in PrestigeButtons)
            {
                Mouse.MouseMoved -= button.Value.CheckForMouseHover;
                Mouse.MouseClicked -= button.Value.CheckForMouseClick;
            }
            PrestigeButtons.Clear();
            Logger.LogVerbose("Skills Menu unloaded.");
        }

        public static void AddPrestigeButtonsToMenu()
        {
            var activeClickableMenu = Game1.activeClickableMenu as GameMenu;
            if (activeClickableMenu == null || activeClickableMenu.currentTab != 1)
            {
                if (_skillsMenuInitialized) UnloadSkillsPageAdditions();
            }
            else
            {
                var skillsPage = (SkillsPage)((List<IClickableMenu>)activeClickableMenu.GetInstanceField("pages"))[1];
                if (!_skillsMenuInitialized) IntializeSkillsMenu(skillsPage);
                var spriteBatch = Game1.spriteBatch;
                DrawPrestigeButtons(spriteBatch);
                DrawPrestigeButtonsHoverText(spriteBatch);
                Mouse.DrawCursor(spriteBatch);
            }
        }

        private static void DrawPrestigeButtons(SpriteBatch spriteBatch)
        {
            foreach (var prestigeButton in PrestigeButtons)
            {
                prestigeButton.Value.Draw(spriteBatch);
            }
        }

        private static void DrawPrestigeButtonsHoverText(SpriteBatch spriteBatch)
        {
            foreach (var prestigeButton in PrestigeButtons)
            {
                prestigeButton.Value.DrawHoverText(spriteBatch);
            }
        }

        internal static void OpenPrestigeMenu(Skill skill)
        {
            Logger.LogVerbose("Skills Menu - Setting up Prestige Menu...");
            var menuWidth = Game1.tileSize * 24;
            var menuHeight = Game1.tileSize * 11;

            var menuXCenter = (menuWidth + IClickableMenu.borderWidth * 2) / 2;
            var menuYCenter = (menuHeight + IClickableMenu.borderWidth * 2) / 2;
            var viewport = Game1.graphics.GraphicsDevice.Viewport;
            var screenXCenter = viewport.Width / 2;
            var screenYCenter = viewport.Height / 2;
            var bounds = new Rectangle(screenXCenter - menuXCenter, screenYCenter - menuYCenter,
                menuWidth + IClickableMenu.borderWidth * 2, menuHeight + IClickableMenu.borderWidth * 2);
            Game1.playSound("bigSelect");
            Logger.LogVerbose("Getting currently loaded prestige data...");
            var prestige = PrestigeSaveData.CurrentlyLoadedPrestigeSet.Prestiges.Single(x => x.SkillType == skill.Type);
            Game1.activeClickableMenu = new PrestigeMenu(bounds, skill, prestige);
            Logger.LogVerbose("Skills Menu - Loaded Prestige Menu.");
        }
    }
}
