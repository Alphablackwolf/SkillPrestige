using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.InputHandling;
using SkillPrestige.Logging;
using SkillPrestige.Menus.Elements.Buttons;
using SkillPrestige.Professions;
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
        private PrestigeButton _prestigeButton;
        private TextureButton _settingsButton;
        private Vector2 _professionButtonRowStartLocation;
        private readonly int _rowPadding = Game1.tileSize / 3;
        private int _leftProfessionStartingXLocation;
        private readonly IList<MinimalistProfessionButton> _professionButtons;
        private static int Offset => 4*Game1.pixelZoom;

        private static int ProfessionButtonHeight(Profession profession)
        {
                var iconHeight = profession.IconSourceRectangle.Height*Game1.pixelZoom;
                var textHeight = (int) Math.Ceiling(Game1.dialogueFont.MeasureString(string.Join(Environment.NewLine, profession.DisplayName.Split(' '))).Y);
                return Offset*3 + iconHeight + textHeight;
        }

        private int GetRowHeight<T>() where T : Profession
        {
            return _skill.Professions.Where(x => x is T).Select(ProfessionButtonHeight).Max();
        }

        private int CostTextYOffset<T>() where T : Profession
        {
            return (int)Math.Floor((double)GetRowHeight<T>() / 2 - Game1.dialogueFont.MeasureString(CostText).Y / 2);
        } 

        private const string CostText = "Cost:";
        private int _debouceWaitTime;
        private int _xSpaceAvailableForProfessionButtons;
        private static bool _buttonClickRegistered;

        public PrestigeMenu(Rectangle bounds, Skill skill, Prestige prestige) : base(bounds.X, bounds.Y, bounds.Width, bounds.Height, true)
        {
            Logger.LogVerbose($"New {skill.Type.Name} Prestige Menu created.");
            _skill = skill;
            _prestige = prestige;
            InitiatePrestigeButton();
            InitiateSettingsButton();
            _professionButtons = new List<MinimalistProfessionButton>();
            exitFunction = DeregisterMouseEvents;
        }

        private void InitiatePrestigeButton()
        {
            Logger.LogVerbose("Prestige menu - Initiating prestige button...");
            const int yOffset = 3;
            var buttonWidth = 100 * Game1.pixelZoom;
            var buttonHeight = 20 * Game1.pixelZoom;
            var rightEdgeOfDialog = xPositionOnScreen + width;
            var bounds = new Rectangle(rightEdgeOfDialog - spaceToClearSideBorder - buttonWidth,
                yPositionOnScreen + yOffset + (int)Math.Floor(Game1.tileSize * 3.15), buttonWidth, buttonHeight);
            _prestigeButton = new PrestigeButton(_skill.GetSkillLevel() < 10)
            {
                Bounds = bounds,
                Skill = _skill
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
                menuWidth + borderWidth*2, menuHeight + borderWidth*2);
            Game1.playSound("bigSelect");
            exitThisMenu(false);
            Game1.activeClickableMenu = new SettingsMenu(bounds);
            Logger.LogVerbose("Prestige Menu - Loaded Settings Menu.");
        }

        private void InitiateProfessionButtons()
        {
            Logger.LogVerbose("Prestige menu - Initiating profession buttons...");
            _xSpaceAvailableForProfessionButtons = xPositionOnScreen + width - spaceToClearSideBorder * 2 - _leftProfessionStartingXLocation;
            InitiateLevelFiveProfessionButtons();
            InitiateLevelTenProfessionButtons();
            Logger.LogVerbose("Prestige menu - Profession button initiated.");
        }

        private void RegisterMouseEvents()
        {
            if (_buttonClickRegistered) return;
            _buttonClickRegistered = true;
            Logger.LogVerbose("Prestige menu - Registering mouse events...");
            foreach (var button in _professionButtons)
            {
                Mouse.MouseMoved += button.CheckForMouseHover;
                Mouse.MouseClicked += button.CheckForMouseClick;
            }
            Mouse.MouseMoved += _prestigeButton.CheckForMouseHover;
            Mouse.MouseMoved += _settingsButton.CheckForMouseHover;
            Mouse.MouseClicked += _prestigeButton.CheckForMouseClick;
            Mouse.MouseClicked += _settingsButton.CheckForMouseClick;
            Logger.LogVerbose("Prestige menu - Mouse events registered.");
        }

        private void DeregisterMouseEvents()
        {
            Logger.LogVerbose("Prestige menu - Deregistering mouse events...");
            if (!_buttonClickRegistered) return;
            foreach (var button in _professionButtons)
            {
                Mouse.MouseMoved -= button.CheckForMouseHover;
                Mouse.MouseClicked -= button.CheckForMouseClick;
            }
            Mouse.MouseMoved -= _prestigeButton.CheckForMouseHover;
            Mouse.MouseMoved -= _settingsButton.CheckForMouseHover;
            Mouse.MouseClicked -= _prestigeButton.CheckForMouseClick;
            Mouse.MouseClicked -= _settingsButton.CheckForMouseClick;
            _buttonClickRegistered = false;
            Logger.LogVerbose("Prestige menu - Mouse events deregistered.");
        }

        private static int ProfessionButtonWidth(Profession profession)
        {
            return (int)Math.Ceiling(Game1.dialogueFont.MeasureString(string.Join(Environment.NewLine, profession.DisplayName.Split(' '))).X) + Offset * 2;
        }

        private void InitiateLevelFiveProfessionButtons()
        {
            Logger.LogVerbose("Prestige menu - Initiating level 5 profession buttons...");
            var leftProfessionButtonXCenter = _leftProfessionStartingXLocation + _xSpaceAvailableForProfessionButtons / 4;
            var rightProfessionButtonXCenter = _leftProfessionStartingXLocation + (int)Math.Floor(_xSpaceAvailableForProfessionButtons * .75d);
            var firstProfession = _skill.Professions.OrderBy(x => x.Id).First(x => x is TierOneProfession);

            _professionButtons.Add(new MinimalistProfessionButton
            {

                Bounds = new Rectangle(leftProfessionButtonXCenter - ProfessionButtonWidth(firstProfession) / 2, (int)_professionButtonRowStartLocation.Y, ProfessionButtonWidth(firstProfession), ProfessionButtonHeight(firstProfession)),
                CanBeAfforded = _prestige.PrestigePoints >= PerSaveOptions.Instance.CostOfTierOnePrestige,
                IsObtainable = true,
                Selected = _prestige.PrestigeProfessionsSelected.Contains(firstProfession.Id),
                Profession = firstProfession
            });
            var secondProfession = _skill.Professions.OrderBy(x => x.Id).Skip(1).First(x => x is TierOneProfession);
            _professionButtons.Add(new MinimalistProfessionButton
            {

                Bounds = new Rectangle(rightProfessionButtonXCenter - ProfessionButtonWidth(secondProfession) / 2, (int)_professionButtonRowStartLocation.Y, ProfessionButtonWidth(secondProfession), ProfessionButtonHeight(secondProfession)),
                CanBeAfforded = _prestige.PrestigePoints >= PerSaveOptions.Instance.CostOfTierOnePrestige,
                IsObtainable = true,
                Selected = _prestige.PrestigeProfessionsSelected.Contains(secondProfession.Id),
                Profession = secondProfession
            });
            Logger.LogVerbose("Prestige menu - Level 5 profession buttons initiated.");
        }

        private void InitiateLevelTenProfessionButtons()
        {
            Logger.LogVerbose("Prestige menu - Initiating level 10 profession buttons...");
            var buttonCenterIndex = 1;
            var canBeAfforded = _prestige.PrestigePoints >= PerSaveOptions.Instance.CostOfTierTwoPrestige;
            foreach (var profession in _skill.Professions.OrderBy(x => x.Id).Where(x => x is TierTwoProfession)
            )
            {
                var tierTwoProfession = (TierTwoProfession)profession;
                _professionButtons.Add(new MinimalistProfessionButton
                {
                    Bounds = new Rectangle(_leftProfessionStartingXLocation + (int)Math.Floor(_xSpaceAvailableForProfessionButtons * (buttonCenterIndex / 8d)) - ProfessionButtonWidth(profession) / 2, (int)_professionButtonRowStartLocation.Y + GetRowHeight<TierOneProfession>() + _rowPadding, ProfessionButtonWidth(profession), ProfessionButtonHeight(profession)),
                    CanBeAfforded = canBeAfforded,
                    IsObtainable = _prestige.PrestigeProfessionsSelected.Contains(tierTwoProfession.TierOneProfession.Id),
                    Selected = _prestige.PrestigeProfessionsSelected.Contains(tierTwoProfession.Id),
                    Profession = tierTwoProfession
                });
                buttonCenterIndex += 2;
            }
            Logger.LogVerbose("Prestige menu - Level 10 profession buttons initiated.");
        }

        private void UpdateProfessionButtonAvailability()
        {
            foreach (var button in _professionButtons)
            {
                button.CanBeAfforded = _prestige.PrestigePoints >= PerSaveOptions.Instance.CostOfTierTwoPrestige || button.Profession is TierOneProfession && _prestige.PrestigePoints >= PerSaveOptions.Instance.CostOfTierOnePrestige;
                button.IsObtainable = button.Profession is TierOneProfession || _prestige.PrestigeProfessionsSelected.Contains(((TierTwoProfession)button.Profession).TierOneProfession.Id);
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
            DrawSettingsButton(spriteBatch);
            DrawHeader(spriteBatch);
            DrawPrestigePoints(spriteBatch);
            _prestigeButton.Draw(spriteBatch);
            DrawLevelFiveProfessionCost(spriteBatch);
            if (!_professionButtons.Any()) InitiateProfessionButtons();
            UpdateProfessionButtonAvailability();
            DrawProfessionButtons(spriteBatch);
            DrawLevelTenProfessionCost(spriteBatch);
            DrawButtonHoverText(spriteBatch);
            Mouse.DrawCursor(spriteBatch);
        }

        private void DrawSettingsButton(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Game1.mouseCursors, new Vector2(_settingsButton.Bounds.X, _settingsButton.Bounds.Y), _settingsButton.SourceRectangle, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.0001f);

        }

        private void DrawHeader(SpriteBatch spriteBatch)
        {
            var title = $"{_skill.Type.Name} Prestige";
            DrawSkillIcon(spriteBatch, new Vector2(xPositionOnScreen + spaceToClearSideBorder + borderWidth, yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4));
            spriteBatch.DrawString(Game1.dialogueFont, title, new Vector2(xPositionOnScreen + width / 2 - Game1.dialogueFont.MeasureString(title).X / 2f, yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4), Game1.textColor);
            DrawSkillIcon(spriteBatch, new Vector2(xPositionOnScreen + width - spaceToClearSideBorder - borderWidth - Game1.tileSize, yPositionOnScreen + spaceToClearTopBorder + Game1.tileSize / 4));
            drawHorizontalPartition(spriteBatch, yPositionOnScreen + (int)Math.Floor(Game1.tileSize * 2.5));
        }

        private void DrawSkillIcon(SpriteBatch spriteBatch, Vector2 location)
        {
            Utility.drawWithShadow(spriteBatch, _skill.SkillIconTexture, location, _skill.SourceRectangleForSkillIcon, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 0.88f);
        }

        private void DrawPrestigePoints(SpriteBatch spriteBatch)
        {
            const string pointText = "Prestige Points:";
            var yOffset = 5 * Game1.pixelZoom;
            var textLocation = new Vector2(xPositionOnScreen + spaceToClearSideBorder / 2 + borderWidth,
                yPositionOnScreen + yOffset + (int)Math.Floor(Game1.tileSize * 3.15));
            spriteBatch.DrawString(Game1.dialogueFont, pointText, textLocation, Game1.textColor);
            var pointNumberLocation = new Vector2(textLocation.X + Game1.dialogueFont.MeasureString(pointText).X + (int)Math.Floor(Game1.pixelZoom * 4.25) + NumberSprite.getWidth(_prestige.PrestigePoints), textLocation.Y + Game1.pixelZoom * 5);
            NumberSprite.draw(_prestige.PrestigePoints, spriteBatch, pointNumberLocation, Color.SandyBrown, 1f, .85f, 1f, 0);
            _professionButtonRowStartLocation = new Vector2(textLocation.X, textLocation.Y + Game1.dialogueFont.MeasureString(pointText).Y + _rowPadding);
        }

        private void DrawLevelFiveProfessionCost(SpriteBatch spriteBatch)
        {
            var costTextLocation = _professionButtonRowStartLocation;
            costTextLocation.Y += CostTextYOffset<TierOneProfession>();
            spriteBatch.DrawString(Game1.dialogueFont, CostText, costTextLocation, Game1.textColor);
            var pointNumberLocation = new Vector2(costTextLocation.X + Game1.dialogueFont.MeasureString(CostText).X + (int)Math.Ceiling(NumberSprite.getWidth(PerSaveOptions.Instance.CostOfTierOnePrestige) * 3.0), costTextLocation.Y + Game1.pixelZoom * 5);
            NumberSprite.draw(PerSaveOptions.Instance.CostOfTierOnePrestige, spriteBatch, pointNumberLocation, Color.SandyBrown, 1f, .85f, 1f, 0);
            if (_leftProfessionStartingXLocation == 0) _leftProfessionStartingXLocation = (int)pointNumberLocation.X + (int)Math.Ceiling((double)NumberSprite.digitWidth);
        }

        private void DrawLevelTenProfessionCost(SpriteBatch spriteBatch)
        {
            var firstRowBottomLocation = _professionButtonRowStartLocation.Y + GetRowHeight<TierOneProfession>();
            var costTextLocation = new Vector2(_professionButtonRowStartLocation.X, firstRowBottomLocation + CostTextYOffset<TierTwoProfession>() + _rowPadding);
            spriteBatch.DrawString(Game1.dialogueFont, CostText, costTextLocation, Game1.textColor);
            var pointNumberLocation = new Vector2(costTextLocation.X + Game1.dialogueFont.MeasureString(CostText).X + (int)Math.Ceiling(NumberSprite.getWidth(PerSaveOptions.Instance.CostOfTierTwoPrestige) *3.0), costTextLocation.Y + Game1.pixelZoom * 5);
            NumberSprite.draw(PerSaveOptions.Instance.CostOfTierTwoPrestige, spriteBatch, pointNumberLocation, Color.SandyBrown, 1f, .85f, 1f, 0);
        }

        private void DrawProfessionButtons(SpriteBatch spriteBatch)
        {
            foreach (var button in _professionButtons)
            {
                button.Draw(spriteBatch);
            }
        }

        private void DrawButtonHoverText(SpriteBatch spriteBatch)
        {
            foreach (var button in _professionButtons)
            {
                button.DrawHoverText(spriteBatch);
            }
            _prestigeButton.DrawHoverText(spriteBatch);
            _settingsButton.DrawHoverText(spriteBatch);
        }
    }
}
