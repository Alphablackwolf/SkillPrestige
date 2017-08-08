using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Logging;
using SkillPrestige.Menus.Elements.Buttons;
using SkillPrestige.Professions;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Menus
{
    internal class PrestigeMenuProfessionsPage : MenuPage
    {
        private Vector2 _professionButtonRowStartLocation;
        private Vector2 _tierOneCostTextLocation;
        private Vector2 _tierOneCostPointNumberLocation;
        private int _leftProfessionStartingXLocation;
        private readonly int _rowPadding = Game1.tileSize / 3;
        private int _xSpaceAvailableForProfessionButtons;
        private static int Offset => 4 * Game1.pixelZoom;
        private const string PointText = "Prestige Points:";
        private const string CostText = "Cost:";
        private readonly Skill _skill;
        private readonly Prestige _prestige;

        public PrestigeMenuProfessionsPage(Skill skill, Prestige prestige, Rectangle dialogBounds)
        {
            PageIndex = 0;
            _skill = skill;
            _prestige = prestige;
            SetLocationVariables(dialogBounds);
            InitiateProfessionButtons(dialogBounds);
            BeforeControlsDrawnBehindControlsDrawFunctions.Add(x => UpdateProfessionButtonAvailability());
            BeforeControlsDrawnBehindControlsDrawFunctions.Add(DrawLevelFiveProfessionCost);
            BeforeControlsDrawnBehindControlsDrawFunctions.Add(DrawLevelTenProfessionCost);
        }

        private void InitiateProfessionButtons(Rectangle dialogBounds)
        {
            Logger.LogVerbose("Prestige menu - Initiating profession buttons...");
            _xSpaceAvailableForProfessionButtons = dialogBounds.X + dialogBounds.Width - IClickableMenu.spaceToClearSideBorder * 2 - _leftProfessionStartingXLocation;
            InitiateLevelFiveProfessionButtons();
            InitiateLevelTenProfessionButtons();
            Logger.LogVerbose("Prestige menu - Profession button initiated.");
        }

        private void InitiateLevelFiveProfessionButtons()
        {
            Logger.LogVerbose("Prestige menu - Initiating level 5 profession buttons...");
            var leftProfessionButtonXCenter = _leftProfessionStartingXLocation + _xSpaceAvailableForProfessionButtons / 4;
            var rightProfessionButtonXCenter = _leftProfessionStartingXLocation + (_xSpaceAvailableForProfessionButtons * .75d).Floor();
            var firstProfession = _skill.Professions.OrderBy(x => x.Id).First(x => x is TierOneProfession);

            Controls.Add(new MinimalistProfessionButton
            {

                Bounds = new Rectangle(leftProfessionButtonXCenter - ProfessionButtonWidth(firstProfession) / 2, (int)_professionButtonRowStartLocation.Y, ProfessionButtonWidth(firstProfession), ProfessionButtonHeight(firstProfession)),
                CanBeAfforded = _prestige.PrestigePoints >= PerSaveOptions.Instance.CostOfTierOnePrestige,
                IsObtainable = true,
                Selected = _prestige.PrestigeProfessionsSelected.Contains(firstProfession.Id),
                Profession = firstProfession
            });
            var secondProfession = _skill.Professions.OrderBy(x => x.Id).Skip(1).First(x => x is TierOneProfession);
            Controls.Add(new MinimalistProfessionButton
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
                Controls.Add(new MinimalistProfessionButton
                {
                    Bounds = new Rectangle(_leftProfessionStartingXLocation + (_xSpaceAvailableForProfessionButtons * (buttonCenterIndex / 8d)).Floor() - ProfessionButtonWidth(profession) / 2, (int)_professionButtonRowStartLocation.Y + GetRowHeight<TierOneProfession>() + _rowPadding, ProfessionButtonWidth(profession), ProfessionButtonHeight(profession)),
                    CanBeAfforded = canBeAfforded,
                    IsObtainable = _prestige.PrestigeProfessionsSelected.Contains(tierTwoProfession.TierOneProfession.Id),
                    Selected = _prestige.PrestigeProfessionsSelected.Contains(tierTwoProfession.Id),
                    Profession = tierTwoProfession
                });
                buttonCenterIndex += 2;
            }
            Logger.LogVerbose("Prestige menu - Level 10 profession buttons initiated.");
        }

        private void SetLocationVariables(Rectangle dialogBounds)
        {
            var yOffset = 5 * Game1.pixelZoom;
            var yLocation = dialogBounds.Y + yOffset + (Game1.tileSize * 3.15).Floor();
            var prestigePointTextLocation = new Vector2(dialogBounds.X + IClickableMenu.spaceToClearSideBorder / 2 + IClickableMenu.borderWidth, yLocation);
            _professionButtonRowStartLocation = new Vector2(prestigePointTextLocation.X, prestigePointTextLocation.Y + Game1.dialogueFont.MeasureString(PointText).Y + _rowPadding);
            var costTextLocation = _professionButtonRowStartLocation;
            costTextLocation.Y += CostTextYOffset<TierOneProfession>();
            _tierOneCostTextLocation = costTextLocation;
            _tierOneCostPointNumberLocation = new Vector2(_tierOneCostTextLocation.X + Game1.dialogueFont.MeasureString(CostText).X + (NumberSprite.getWidth(PerSaveOptions.Instance.CostOfTierOnePrestige) * 3.0).Ceiling(), _tierOneCostTextLocation.Y + Game1.pixelZoom * 5);
            _leftProfessionStartingXLocation = _tierOneCostPointNumberLocation.X.Ceiling() + NumberSprite.digitWidth;
        }

        private int CostTextYOffset<T>() where T : Profession
        {
            return ((double)GetRowHeight<T>() / 2 - Game1.dialogueFont.MeasureString(CostText).Y / 2).Floor();
        }

        private int GetRowHeight<T>() where T : Profession
        {
            return _skill.Professions.Where(x => x is T).Select(ProfessionButtonHeight).Max();
        }

        private static int ProfessionButtonHeight(Profession profession)
        {
            var iconHeight = profession.IconSourceRectangle.Height * Game1.pixelZoom;
            var textHeight = Game1.dialogueFont.MeasureString(string.Join(Environment.NewLine, profession.DisplayName.Split(' '))).Y.Ceiling();
            return Offset * 3 + iconHeight + textHeight;
        }

        private static int ProfessionButtonWidth(Profession profession)
        {
            return Game1.dialogueFont.MeasureString(string.Join(Environment.NewLine, profession.DisplayName.Split(' '))).X.Ceiling() + Offset * 2;
        }

        private void DrawLevelFiveProfessionCost(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Game1.dialogueFont, CostText, _tierOneCostTextLocation, Game1.textColor);
            NumberSprite.draw(PerSaveOptions.Instance.CostOfTierOnePrestige, spriteBatch, _tierOneCostPointNumberLocation, Color.SandyBrown, 1f, .85f, 1f, 0);
        }

        private void DrawLevelTenProfessionCost(SpriteBatch spriteBatch)
        {
            var firstRowBottomLocation = _professionButtonRowStartLocation.Y + GetRowHeight<TierOneProfession>();
            var costTextLocation = new Vector2(_professionButtonRowStartLocation.X, firstRowBottomLocation + CostTextYOffset<TierTwoProfession>() + _rowPadding);
            spriteBatch.DrawString(Game1.dialogueFont, CostText, costTextLocation, Game1.textColor);
            var pointNumberLocation = new Vector2(costTextLocation.X + Game1.dialogueFont.MeasureString(CostText).X + (NumberSprite.getWidth(PerSaveOptions.Instance.CostOfTierTwoPrestige) * 3.0).Ceiling(), costTextLocation.Y + Game1.pixelZoom * 5);
            NumberSprite.draw(PerSaveOptions.Instance.CostOfTierTwoPrestige, spriteBatch, pointNumberLocation, Color.SandyBrown, 1f, .85f, 1f, 0);
        }

        private void UpdateProfessionButtonAvailability()
        {
            foreach (var button in Controls.OfType<MinimalistProfessionButton>())
            {
                button.CanBeAfforded = _prestige.PrestigePoints >= PerSaveOptions.Instance.CostOfTierTwoPrestige || button.Profession is TierOneProfession && _prestige.PrestigePoints >= PerSaveOptions.Instance.CostOfTierOnePrestige;
                button.IsObtainable = button.Profession is TierOneProfession || _prestige.PrestigeProfessionsSelected.Contains(((TierTwoProfession)button.Profession).TierOneProfession.Id);
            }
        }
    }
}
