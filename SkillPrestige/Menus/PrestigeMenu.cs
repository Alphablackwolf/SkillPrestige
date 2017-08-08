using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.InputHandling;
using SkillPrestige.Logging;
using SkillPrestige.Menus.Elements.Buttons;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Menus
{
    /// <summary>
    /// Represents a menu where players can choose to prestige a skill and select prestige awards.
    /// </summary>
    internal class PrestigeMenu : IClickableMenu
    {
        private readonly Skill _skill;
        private readonly Prestige _prestige;
        private Vector2 _prestigePointBonusLocation;
        private Vector2 _prestigePointsNumberLocation;
        private Vector2 _prestigePointTextLocation;
        private const string PointText = "Prestige Points:";
        private readonly IEnumerable<IMenuPage> _pages;
        private int _currentPageIndex;
        private IMenuPage CurrentPage => _pages.Single(x => x.PageIndex == _currentPageIndex);
        private int _debouceWaitTime;
        private PrestigeButton _prestigeButton;
        private TextureButton _settingsButton;
        private TextureButton _nextPageButton;
        private bool _menuLevelButtonsMouseEventRegistered;

        public PrestigeMenu(Rectangle bounds, Skill skill, Prestige prestige) : base(bounds.X, bounds.Y, bounds.Width, bounds.Height, true)
        {
            Logger.LogVerbose($"New {skill.Type.Name} Prestige Menu created.");
            _skill = skill;
            _prestige = prestige;
            _pages = new List<IMenuPage>
            {
                new PrestigeMenuProfessionsPage(skill, prestige, bounds),
                new PrestigeMenuBonusPage(skill, prestige, bounds)
            };
            SetPrestigePointLocation();
            InitiateSettingsButton();
            InitiatePrestigeButton();
            InitiateNextPageButton();
            exitFunction = DeregisterMouseEvents;
        }

        private void SetPrestigePointLocation()
        {
            var yOffset = 5 * Game1.pixelZoom;
            var yLocation = yPositionOnScreen + yOffset + (Game1.tileSize * 3.15).Floor();
            var xPadding = (Game1.pixelZoom * 4.25).Floor();
            _prestigePointTextLocation = new Vector2(xPositionOnScreen + spaceToClearSideBorder / 2 + borderWidth, yLocation);
            var prestigePointWidth = (NumberSprite.getWidth(PerSaveOptions.Instance.CostOfTierTwoPrestige) * 3.0).Ceiling();
            _prestigePointsNumberLocation = new Vector2(_prestigePointTextLocation.X + Game1.dialogueFont.MeasureString(PointText).X + xPadding + prestigePointWidth, _prestigePointTextLocation.Y + Game1.pixelZoom * 5);
            _prestigePointBonusLocation = new Vector2(_prestigePointsNumberLocation.X + prestigePointWidth + xPadding, yLocation);
        }

        private void InitiatePrestigeButton()
        {
            Logger.LogVerbose("Prestige menu - Initiating prestige button...");
            const int yOffset = 3;
            var buttonWidth = 100 * Game1.pixelZoom;
            var buttonHeight = 20 * Game1.pixelZoom;
            var rightEdgeOfDialog = xPositionOnScreen + width;
            var bounds = new Rectangle(rightEdgeOfDialog - spaceToClearSideBorder - buttonWidth,
                yPositionOnScreen + yOffset + (Game1.tileSize * 3.15).Floor(), buttonWidth, buttonHeight);

            var prestigeButtonDisabled = true;
            if (PerSaveOptions.Instance.PainlessPrestigeMode)
            {
                if (Game1.player.experiencePoints[_skill.Type.Ordinal] >= 15000 + PerSaveOptions.Instance.ExperienceNeededPerPainlessPrestige)
                {
                    prestigeButtonDisabled = false;
                }
            }
            else
            {
                if (_skill.GetSkillLevel() == 10)
                {
                    var newLevelForSkillExists = Game1.player.newLevels.Any(point => point.X == _skill.Type.Ordinal && point.Y > 0);
                    if (!newLevelForSkillExists)
                    {
                        prestigeButtonDisabled = false;
                    }
                }
            }

            _prestigeButton = new PrestigeButton(prestigeButtonDisabled, _skill)
            {
                Bounds = bounds,
            };
            Logger.LogVerbose("Prestige menu - Prestige button initiated.");
        }

        private void InitiateSettingsButton()
        {
            Logger.LogVerbose("Prestige menu - Initiating settings button...");
            var buttonWidth = 16 * Game1.pixelZoom;
            var buttonHeight = 16 * Game1.pixelZoom;
            var rightEdgeOfDialog = xPositionOnScreen + width;
            var bounds = new Rectangle(rightEdgeOfDialog - buttonWidth - Game1.tileSize, yPositionOnScreen, buttonWidth, buttonHeight);
            _settingsButton = new TextureButton(bounds, Game1.mouseCursors, new Rectangle(96, 368, 16, 16), OpenSettingsMenu, "Open Settings Menu");
            Logger.LogVerbose("Prestige menu - Settings button initiated.");
        }

        private void OpenSettingsMenu()
        {
            Logger.LogVerbose("Prestige Menu - Initiating Settings Menu...");
            var menuWidth = Game1.tileSize * 12;
            var menuHeight = Game1.tileSize * 10;
            var menuXCenter = (menuWidth + borderWidth * 2) / 2;
            var menuYCenter = (menuHeight + borderWidth * 2) / 2;
            var viewport = Game1.graphics.GraphicsDevice.Viewport;
            var screenXCenter = (int)(viewport.Width * (1.0 / Game1.options.zoomLevel)) / 2;
            var screenYCenter = (int)(viewport.Height * (1.0 / Game1.options.zoomLevel)) / 2;
            var bounds = new Rectangle(screenXCenter - menuXCenter, screenYCenter - menuYCenter,
                menuWidth + borderWidth * 2, menuHeight + borderWidth * 2);
            Game1.playSound("bigSelect");
            exitThisMenu(false);
            Game1.activeClickableMenu = new SettingsMenu(bounds);
            Logger.LogVerbose("Prestige Menu - Loaded Settings Menu.");
        }

        private void InitiateNextPageButton()
        {
            Logger.LogVerbose("Prestige menu - Initiating next page button...");
            var buttonWidth = 16 * Game1.pixelZoom;
            var buttonHeight = 16 * Game1.pixelZoom;
            var rightEdgeOfDialog = xPositionOnScreen + width;
            var yOffset = 5 * Game1.pixelZoom;
            var yPositon = yPositionOnScreen + yOffset + (Game1.tileSize * 3.15).Floor();
            var bounds = new Rectangle(rightEdgeOfDialog + Game1.tileSize, yPositon, buttonWidth, buttonHeight);
            _nextPageButton = new TextureButton(bounds, Game1.mouseCursors, new Rectangle(0, 192, 64, 64), MoveToNextPage, "Next Page");
            Logger.LogVerbose("Prestige menu - Next page button initiated.");
        }

        private void MoveToNextPage()
        {
            CurrentPage.DeegisterControls();
            if (_currentPageIndex >= _pages.Count() - 1)
            {
                _currentPageIndex = 0;
            }
            else
            {
                _currentPageIndex++;
            }
            CurrentPage.RegisterControls();
        }

        private void RegisterMouseEvents()
        {
            if (!_menuLevelButtonsMouseEventRegistered)
            {
                _menuLevelButtonsMouseEventRegistered = true;
                Logger.LogVerbose("Prestige menu - Registering mouse events...");
                Mouse.MouseMoved += _prestigeButton.CheckForMouseHover;
                Mouse.MouseMoved += _settingsButton.CheckForMouseHover;
                Mouse.MouseMoved += _nextPageButton.CheckForMouseHover;
                Mouse.MouseClicked += _prestigeButton.CheckForMouseClick;
                Mouse.MouseClicked += _settingsButton.CheckForMouseClick;
                Mouse.MouseClicked += _nextPageButton.CheckForMouseClick;
                Logger.LogVerbose("Prestige menu - Mouse events registered.");
            }
            CurrentPage.RegisterControls();
        }

        private void DeregisterMouseEvents()
        {
            Logger.LogVerbose("Prestige menu - Deregistering mouse events...");
            if (_menuLevelButtonsMouseEventRegistered)
            {
                Mouse.MouseMoved -= _prestigeButton.CheckForMouseHover;
                Mouse.MouseMoved -= _settingsButton.CheckForMouseHover;
                Mouse.MouseMoved -= _nextPageButton.CheckForMouseHover;
                Mouse.MouseClicked -= _prestigeButton.CheckForMouseClick;
                Mouse.MouseClicked -= _settingsButton.CheckForMouseClick;
                Mouse.MouseClicked -= _nextPageButton.CheckForMouseClick;
                _menuLevelButtonsMouseEventRegistered = false;
            }
            Logger.LogVerbose("Prestige menu - Mouse events deregistered.");

            foreach (var page in _pages)
            {
                page.DeegisterControls();
            }
        }
        
        public override void receiveRightClick(int x, int y, bool playSound = true) { }

        public override void draw(SpriteBatch spriteBatch)
        {
            if (_debouceWaitTime < 10)
            {
                _debouceWaitTime++;
            }
            else
            {
                RegisterMouseEvents();
            }

            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            upperRightCloseButton?.draw(spriteBatch);
            DrawHeader(spriteBatch);
            DrawPrestigePoints(spriteBatch);
            DrawPrestigePointBonus(spriteBatch);
            _prestigeButton.Draw(spriteBatch);
            DrawSettingsButton(spriteBatch);
            _nextPageButton.Draw(spriteBatch);
            CurrentPage.Draw(spriteBatch);
        }

        private void DrawHeader(SpriteBatch spriteBatch)
        {
            var title = $"{_skill.Type.Name} Prestige";
            DrawSkillIcon(spriteBatch, new Vector2(xPositionOnScreen + spaceToClearSideBorder + borderWidth, yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4));
            spriteBatch.DrawString(Game1.dialogueFont, title, new Vector2(xPositionOnScreen + width / 2 - Game1.dialogueFont.MeasureString(title).X / 2f, yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4), Game1.textColor);
            DrawSkillIcon(spriteBatch, new Vector2(xPositionOnScreen + width - spaceToClearSideBorder - borderWidth - Game1.tileSize, yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4));
            drawHorizontalPartition(spriteBatch, yPositionOnScreen + (Game1.tileSize * 2.5).Floor());
        }

        private void DrawSettingsButton(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Game1.mouseCursors, new Vector2(_settingsButton.Bounds.X, _settingsButton.Bounds.Y), _settingsButton.SourceRectangle, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.0001f);

        }

        private void DrawSkillIcon(SpriteBatch spriteBatch, Vector2 location)
        {
            Utility.drawWithShadow(spriteBatch, _skill.SkillIconTexture, location, _skill.SourceRectangleForSkillIcon, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 0.88f);
        }

        private void DrawPrestigePoints(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Game1.dialogueFont, PointText, _prestigePointTextLocation, Game1.textColor);
            NumberSprite.draw(_prestige.PrestigePoints, spriteBatch, _prestigePointsNumberLocation, Color.SandyBrown, 1f, .85f, 1f, 0);
        }

        private void DrawPrestigePointBonus(SpriteBatch spriteBatch)
        {
            if(PerSaveOptions.Instance.UseExperienceMultiplier) spriteBatch.DrawString(Game1.dialogueFont, $"({(_prestige.PrestigePoints * PerSaveOptions.Instance.ExperienceMultiplier * 100).Floor()}% XP bonus)", _prestigePointBonusLocation, Game1.textColor);
        }
    }
}
