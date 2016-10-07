using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.InputHandling;
using SkillPrestige.Logging;
using SkillPrestige.Menus.Buttons;
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
        private Vector2 _professionButtonRowStartLocation;
        private readonly int _rowPadding = Game1.tileSize / 3;
        private int _leftProfessionStartingXLocation;
        private readonly IList<ProfessionButton> _professionButtons;
        private static int ProfessionButtonHeight => (int)Math.Ceiling(Game1.tileSize * 3.2);
        private static int ProfessionButtonWidth => (int)Math.Ceiling(Game1.tileSize * 5d);
        private const string CostText = "Cost:";
        private static int CostTextYOffset => (int)Math.Floor((double)ProfessionButtonHeight / 2 - Game1.dialogueFont.MeasureString(CostText).Y / 2);
        private float _levelFiveCostYLocation;
        private int _debouceWaitTime;
        private int _xSpaceAvailableForProfessionButtons;
        private static bool _buttonClickRegistered;

        public PrestigeMenu(Rectangle bounds, Skill skill, Prestige prestige) : base(bounds.X, bounds.Y, bounds.Width, bounds.Height, true)
        {
            Logger.LogVerbose("New Prestige Menu created.");
            _skill = skill;
            _prestige = prestige;
            InitiatePrestigeButton();
            _professionButtons = new List<ProfessionButton>();
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
            _prestigeButton = new PrestigeButton(Game1.player.getEffectiveSkillLevel(_skill.Type.Ordinal) < 10)
            {
                Bounds = bounds,
                Skill = _skill
            };
            Logger.LogVerbose("Prestige menu - Prestige button initiated.");
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
            Mouse.MouseClicked += _prestigeButton.CheckForMouseClick;
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
            Mouse.MouseClicked -= _prestigeButton.CheckForMouseClick;
            _buttonClickRegistered = false;
            Logger.LogVerbose("Prestige menu - Mouse events deregistered.");
        }

        private void InitiateLevelFiveProfessionButtons()
        {
            Logger.LogVerbose("Prestige menu - Initiating level 5 profession buttons...");
            var leftProfessionButtonXCenter = _leftProfessionStartingXLocation + _xSpaceAvailableForProfessionButtons / 4;
            var rightProfessionButtonXCenter = _leftProfessionStartingXLocation + (int)Math.Floor(_xSpaceAvailableForProfessionButtons * .75d);
            var firstProfession = _skill.Professions.OrderBy(x => x.Id).First(x => x is TierOneProfession);
            _professionButtons.Add(new ProfessionButton
            {

                Bounds = new Rectangle(leftProfessionButtonXCenter - (ProfessionButtonWidth / 2), (int)_professionButtonRowStartLocation.Y, ProfessionButtonWidth, ProfessionButtonHeight),
                CanBeAfforded = _prestige.PrestigePoints >= 1,
                IsObtainable = true,
                Selected = _prestige.PrestigeProfessionsSelected.Contains(firstProfession.Id),
                Profession = firstProfession
            });
            var secondProfession = _skill.Professions.OrderBy(x => x.Id).Skip(1).First(x => x is TierOneProfession);
            _professionButtons.Add(new ProfessionButton
            {

                Bounds = new Rectangle(rightProfessionButtonXCenter - (ProfessionButtonWidth / 2), (int)_professionButtonRowStartLocation.Y, ProfessionButtonWidth, ProfessionButtonHeight),
                CanBeAfforded = _prestige.PrestigePoints >= 1,
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
            var canBeAfforded = _prestige.PrestigePoints >= 2;
            foreach (var profession in _skill.Professions.OrderBy(x => x.Id).Where(x => x is TierTwoProfession)
            )
            {
                var tierTwoProfession = (TierTwoProfession)profession;
                _professionButtons.Add(new ProfessionButton
                {
                    Bounds = new Rectangle(_leftProfessionStartingXLocation + (int)Math.Floor(_xSpaceAvailableForProfessionButtons * (buttonCenterIndex / 8d)) - (ProfessionButtonWidth / 2), (int)_professionButtonRowStartLocation.Y + ProfessionButtonHeight + _rowPadding, ProfessionButtonWidth, ProfessionButtonHeight),
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
                button.CanBeAfforded = _prestige.PrestigePoints >= 2 || button.Profession is TierOneProfession && _prestige.PrestigePoints >= 1;
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
            var textLocation = new Vector2(xPositionOnScreen + spaceToClearSideBorder + borderWidth,
                yPositionOnScreen + yOffset + (int)Math.Floor(Game1.tileSize * 3.15));
            spriteBatch.DrawString(Game1.dialogueFont, pointText, textLocation, Game1.textColor);
            var pointNumberLocation = new Vector2(textLocation.X + Game1.dialogueFont.MeasureString(pointText).X + (int)Math.Floor(Game1.pixelZoom * 4.25) + NumberSprite.getWidth(_prestige.PrestigePoints), textLocation.Y + Game1.pixelZoom * 5);
            NumberSprite.draw(_prestige.PrestigePoints, spriteBatch, pointNumberLocation, Color.SandyBrown, 1f, .85f, 1f, 0);
            _professionButtonRowStartLocation = new Vector2(textLocation.X, textLocation.Y + Game1.dialogueFont.MeasureString(pointText).Y + _rowPadding);
        }

        private void DrawLevelFiveProfessionCost(SpriteBatch spriteBatch)
        {
            var costTextLocation = _professionButtonRowStartLocation;
            costTextLocation.Y += CostTextYOffset;
            _levelFiveCostYLocation = costTextLocation.Y;
            spriteBatch.DrawString(Game1.dialogueFont, CostText, costTextLocation, Game1.textColor);
            var pointNumberLocation = new Vector2(costTextLocation.X + Game1.dialogueFont.MeasureString(CostText).X + (int)Math.Floor(Game1.pixelZoom * 4.25), costTextLocation.Y + Game1.pixelZoom * 5);
            NumberSprite.draw(1, spriteBatch, pointNumberLocation, Color.SandyBrown, 1f, .85f, 1f, 0);
            if (_leftProfessionStartingXLocation == 0) _leftProfessionStartingXLocation = (int)pointNumberLocation.X + (int)Math.Ceiling((double)NumberSprite.digitWidth);
        }

        private void DrawLevelTenProfessionCost(SpriteBatch spriteBatch)
        {
            var costTextLocation = new Vector2(_professionButtonRowStartLocation.X, _levelFiveCostYLocation + ProfessionButtonHeight + _rowPadding);
            spriteBatch.DrawString(Game1.dialogueFont, CostText, costTextLocation, Game1.textColor);
            var pointNumberLocation = new Vector2(costTextLocation.X + Game1.dialogueFont.MeasureString(CostText).X + (int)Math.Floor(Game1.pixelZoom * 4.25), costTextLocation.Y + Game1.pixelZoom * 5);
            NumberSprite.draw(2, spriteBatch, pointNumberLocation, Color.SandyBrown, 1f, .85f, 1f, 0);
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
        }
    }
}
