using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Bonuses;
using SkillPrestige.Extensions;
using SkillPrestige.Options;
using SkillPrestige.PrestigeFramework;
using StardewValley;

namespace SkillPrestige.Menus.Elements.Buttons
{
    /// <summary>
    /// Represents a profession button on the prestige menu. 
    /// Used to allow the user to choose to permanently obtain a profession.
    /// </summary>
    public class BonusButton : Button
    {
        public BonusButton(Bonus bonus)
        {
            TitleTextFont = Game1.dialogueFont;
            Bonus = bonus;
        }

        protected override Texture2D ButtonTexture
        {
            get => BonusButtonTexture;
            set => BonusButtonTexture = value;
        }

        public static Texture2D BonusButtonTexture { private get; set; }

        private Bonus Bonus { get; }
        private bool IsMaxLevel => Bonus.Level == Bonus.Type.MaxLevel;
        public bool IsObtainable { private get; set; }
        public bool CanBeAfforded { private get; set; }
        private bool IsDisabled => IsMaxLevel || !IsObtainable || !CanBeAfforded;
        private Color DrawColor => IsDisabled ? Color.Gray : Color.White;
        private static int Offset => 4 * Game1.pixelZoom;

        private string LevelText => $"{Bonus.Level}/{Bonus.Type.MaxLevel?.ToString() ?? "∞"}";

        private static int TextYOffset => 4 * Game1.pixelZoom;

        protected override string HoverText => $"{HoverTextPrefix}{Environment.NewLine}{Environment.NewLine}{(Bonus.Level == Bonus.Type.MaxLevel ? $"MAX LEVEL:{Environment.NewLine}{Bonus.Type.GetLevelEffect(Bonus.Level)}" : $"Next Level({ Bonus.Level + 1}):{Environment.NewLine}{Bonus.Type.GetLevelEffect(Bonus.Level + 1)}")}";

        private string HoverTextPrefix => IsMaxLevel
            ? "You already have the maximum level for this Bonus."
            : IsObtainable
                ? CanBeAfforded
                    ? $"Grants{Environment.NewLine}{string.Join(Environment.NewLine, Bonus.Type.EffectDescriptions)}"
                    : $"You cannot afford this bonus,{Environment.NewLine}you need {GetCost()} prestige point(s) in this skill to purchase it."
                : $"This bonus is not available to obtain until you have purchased all of the professions for the {Bonus.Type.SkillType.Name} skill.";

        protected override string Text => string.Join(Environment.NewLine, Bonus.Type.Name.Split(' '));

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ButtonTexture, Bounds, DrawColor);
            DrawText(spriteBatch);
        }

        public override int CalculateWidth()
        {
            return Game1.dialogueFont.MeasureString(string.Join(Environment.NewLine, Bonus.Type.Name.Split(' '))).X.Ceiling() + Offset * 2;
        }

        public override int CalculateHeight()
        {
            var textHeight = Game1.dialogueFont.MeasureString(string.Join(Environment.NewLine, Bonus.Type.Name.Split(' '))).Y.Ceiling();
            var smallTextHeight = Game1.smallFont.MeasureString(LevelText).Y.Ceiling();
            return Offset * 3 + textHeight + smallTextHeight;
        }

        private void DrawText(SpriteBatch spriteBatch)
        {
            var buttonXCenter = Bounds.Width / 2;
            var textCenter = TitleTextFont.MeasureString(Text).X / 2;
            var textXLocationRelativeToButton = buttonXCenter - textCenter;
            var textYLocationRelativeToButton = TextYOffset * 2;
            var locationOfTextRelativeToButton = new Vector2(textXLocationRelativeToButton, textYLocationRelativeToButton);
            DrawTitleText(spriteBatch, locationOfTextRelativeToButton);
            var levelTextYLocation = Bounds.Bottom - TextYOffset - Game1.smallFont.MeasureString(LevelText).Y; 
            var levelTextXLocation = Bounds.Right - Offset - Game1.smallFont.MeasureString(LevelText).X;
            spriteBatch.DrawString(Game1.smallFont, LevelText, new Vector2(levelTextXLocation, levelTextYLocation), Game1.textColor);
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
