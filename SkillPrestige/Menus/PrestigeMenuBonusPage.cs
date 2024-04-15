using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Bonuses;
using SkillPrestige.Extensions;
using SkillPrestige.Logging;
using SkillPrestige.Menus.Elements;
using SkillPrestige.Menus.Elements.Buttons;
using SkillPrestige.Options;
using SkillPrestige.PrestigeFramework;
using SkillPrestige.Skills;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Menus
{
    internal class PrestigeMenuBonusPage : MenuPage
    {
        private readonly int _rowPadding = Game1.tileSize / 3;
        private readonly Skill _skill;
        private readonly Prestige _prestige;
        private const string CostText = "Cost:";
        private Vector2 _costTextLocation;
        private Vector2 _pointNumberLocation;

        public PrestigeMenuBonusPage(Skill skill, Prestige prestige, Rectangle dialogBounds)
        {
            PageIndex = 1;
            _skill = skill;
            _prestige = prestige;
            dialogBounds = SetCostTextLocation(dialogBounds);
            SetupButtons(dialogBounds);
            BeforeControlsDrawnFunctions.Add(x => UpdateBonusButtonAvailability());
            BeforeControlsDrawnFunctions.Add(DrawCost);
        }

        private void SetupButtons(Rectangle dialogBounds)
        {
            Logger.LogInformation($"Setting up bonus page for {_skill.Type.Name}.");
            foreach (var bonusType in _skill.AvailableBonusTypes)
            {
                Logger.LogInformation($"Setting up bonus button for {bonusType.Name} bonus.");
                var bonus = _prestige.Bonuses.FirstOrDefault(x => x.BonusTypeCode == bonusType.Code);
                if (bonus == null)
                {
                    bonus = new Bonus
                    {
                        BonusTypeCode = bonusType.Code,
                        Level = 0
                    };
                    _prestige.Bonuses.Add(bonus);
                }
                Controls.Add(new BonusButton(bonus)
                {
                    IsObtainable = bonus.Level < bonus.Type.MaxLevel && (PerSaveOptions.Instance.DropBonusRequirements || _skill.Professions.All(x => _prestige.PrestigeProfessionsSelected.Contains(x.Id))),
                    CanBeAfforded = _prestige.PrestigePoints >= PerSaveOptions.Instance.CostOfBonuses
                });
            }
            ElementArranger.Arrange(dialogBounds, Controls.Cast<IArrangeableElement>().ToList(), Game1.pixelZoom, _rowPadding);
        }

        private Rectangle SetCostTextLocation(Rectangle dialogBounds)
        {
            var textHeight = new[] { Game1.dialogueFont.MeasureString(CostText).Y, NumberSprite.digitHeight }.Max();
            _costTextLocation = new Vector2(dialogBounds.X + IClickableMenu.spaceToClearSideBorder / 2 + IClickableMenu.borderWidth, dialogBounds.Y + dialogBounds.Height / 2 - textHeight / 2);
            _pointNumberLocation = new Vector2(_costTextLocation.X + Game1.dialogueFont.MeasureString(CostText).X + (NumberSprite.getWidth(PerSaveOptions.Instance.CostOfBonuses) * 3.0).Ceiling(), _costTextLocation.Y + Game1.pixelZoom * 5);
            var controlAreaXLocation = _pointNumberLocation.X.Ceiling() + NumberSprite.digitWidth;
            var removedWidth = controlAreaXLocation - dialogBounds.X;
            var returnBounds = new Rectangle(controlAreaXLocation, dialogBounds.Y, dialogBounds.Width - removedWidth, dialogBounds.Height);
            return returnBounds;
        }

        private void DrawCost(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Game1.dialogueFont, CostText, _costTextLocation, Game1.textColor);
            NumberSprite.draw(PerSaveOptions.Instance.CostOfBonuses, spriteBatch, _pointNumberLocation, Color.SandyBrown, 1f, .85f, 1f, 0);
        }

        private void UpdateBonusButtonAvailability()
        {
            foreach (var button in Controls.OfType<BonusButton>())
            {
                button.CanBeAfforded = _prestige.PrestigePoints >= PerSaveOptions.Instance.CostOfBonuses;
            }
        }
    }
}
