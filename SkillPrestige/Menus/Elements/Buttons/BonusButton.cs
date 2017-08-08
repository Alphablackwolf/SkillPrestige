using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Bonuses;
using StardewValley;

namespace SkillPrestige.Menus.Elements.Buttons
{
    /// <summary>
    /// Represents a profession button on the prestige menu. 
    /// Used to allow the user to choose to permanently obtain a profession.
    /// </summary>
    public class BonusButton : Button
    {
        public BonusButton()
        {
            TitleTextFont = Game1.dialogueFont;
        }

        protected override Texture2D ButtonTexture
        {
            get => BonusButtonTexture;
            set => BonusButtonTexture = value;
        }

        public static Texture2D BonusButtonTexture { get; set; }

        public Bonus Bonus { get; set; }
        private bool IsMaxLevel => Bonus.Level == Bonus.Type.MaxLevel;
        public bool IsObtainable { private get; set; }
        public bool CanBeAfforded { private get; set; }
        private bool IsDisabled => IsMaxLevel || !IsObtainable || !CanBeAfforded;
        private Color DrawColor => IsDisabled ? Color.Gray : Color.White;

        private static int TextYOffset => 4 * Game1.pixelZoom;

        public override string HoverText => $"{HoverTextPrefix}{Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, Bonus.Type.EffectDescriptions)}";

        private string HoverTextPrefix => IsMaxLevel
            ? "You already have the maximum level for this Bonus."
            : IsObtainable
                ? CanBeAfforded
                    ? $"Click to increase your bonus level to {Bonus.Level + 1}.{Environment.NewLine}{Bonus.Type.GetNextLevelEffect(Bonus.Level)}"
                    : $"You cannot afford this bonus,{Environment.NewLine}you need {GetCost()} prestige point(s) in this skill to purchase it."
                : $"This bonus is not available to obtain until you have purchased all of the professions for the {Bonus.Type.SkillType.Name} skill.";

        public override string Text => string.Join(Environment.NewLine, Bonus.Type.Name.Split(' '));

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ButtonTexture, Bounds, DrawColor);
            DrawText(spriteBatch);
        }

        private void DrawText(SpriteBatch spriteBatch)
        {
            var buttonXCenter = Bounds.Width / 2;
            var textCenter = TitleTextFont.MeasureString(Text).X / 2;
            var textXLocationRelativeToButton = buttonXCenter - textCenter;
            var textYLocationRelativeToButton = TextYOffset * 2;
            var locationOfTextRelativeToButton = new Vector2(textXLocationRelativeToButton, textYLocationRelativeToButton);
            DrawTitleText(spriteBatch, locationOfTextRelativeToButton);
        }

        protected override void OnMouseHover()
        {
            base.OnMouseHover();
            if (!IsDisabled) Game1.playSound("smallSelect");
        }

        protected override void OnMouseClick()
        {
            if (IsDisabled) return;
            Game1.playSound("bigSelect");
            Prestige.AddPrestigeBonus(Bonus.Type);
        }

        private static int GetCost()
        {
            return PerSaveOptions.Instance.CostOfBonuses;
        }
    }
}
