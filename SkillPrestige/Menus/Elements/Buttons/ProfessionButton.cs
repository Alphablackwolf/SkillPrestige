using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Logging;
using SkillPrestige.Professions;
using StardewValley;

namespace SkillPrestige.Menus.Elements.Buttons
{
    /// <summary>
    /// Represents a profession button on the prestige menu. 
    /// Used to allow the user to choose to permanently obtain a profession.
    /// </summary>
    public class ProfessionButton : Button
    {
        public ProfessionButton()
        {
            TitleTextFont = Game1.dialogueFont;
            EffectTextFont = Game1.smallFont;
        }

        protected override Texture2D ButtonTexture
        {
            get { return ProfessionButtonTexture; }
            set { ProfessionButtonTexture = value; }
        }

        public static Texture2D ProfessionButtonTexture { get; set; }

        public Profession Profession { get; set; }
        public bool Selected { get; set; }
        public bool IsObtainable { get; set; }
        public bool CanBeAfforded { get; set; }
        private bool IsDisabled => Selected || !IsObtainable || !CanBeAfforded;
        private Color DrawColor => IsDisabled ? Color.Gray : Color.White;
        private SpriteFont EffectTextFont { get; set; }
        private static int TextXOffset => 4 * Game1.pixelZoom;
        private static int TextYOffset => 4 * Game1.pixelZoom;
        private Vector2 _iconLocation;

        protected override string HoverText => Selected
                                            ? $"You already permanently have the {Profession.DisplayName} profession."
                                            : IsObtainable
                                                ? CanBeAfforded
                                                    ? $"Click to permanently obtain the {Profession.DisplayName} profession."
                                                    : $"You cannot afford this profession, you need {GetPrestigeCost()} prestige point(s) in this skill to purchase it."
                                                : $"This profession is not available to obtain permanently until the {(Profession as TierTwoProfession)?.TierOneProfession.DisplayName} profession has been permanently obtained.";

        protected override string Text => Profession.DisplayName;

        public override void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(ButtonTexture, Bounds, DrawColor);
            DrawTitleText(spriteBatch, new Vector2(TextXOffset, TextYOffset));
            DrawIcon(spriteBatch);
            DrawEffectText(spriteBatch);
        }

        private void DrawIcon(SpriteBatch spriteBatch)
        {
            var locationOfIconRelativeToButton = new Vector2(Bounds.Width - (TextXOffset + Profession.IconSourceRectangle.Width * Game1.pixelZoom), TextYOffset);
            var buttonLocation = new Vector2(Bounds.X, Bounds.Y);
            _iconLocation = buttonLocation + locationOfIconRelativeToButton;
            spriteBatch.Draw(Profession.Texture, _iconLocation, Profession.IconSourceRectangle, DrawColor, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        }

        private void DrawEffectText(SpriteBatch spriteBatch)
        {

            var padding = 2 * Game1.pixelZoom;
            var bottomOfIcon = _iconLocation.Y + Profession.IconSourceRectangle.Height * Game1.pixelZoom;
            var nextLineLocation = new Vector2(Bounds.X + TextXOffset, bottomOfIcon + padding);
            spriteBatch.DrawString(EffectTextFont, string.Join("\n", Profession.EffectText.Select(effect => effect.WrapText(EffectTextFont, Bounds.Width - TextXOffset * 2))), nextLineLocation, Game1.textColor);
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
            Prestige.AddPrestigeProfession(Profession.Id);
            Selected = true;
        }

        private int GetPrestigeCost()
        {
            var tier = Profession.LevelAvailableAt / 5;
            switch (tier)
            {
                case 1:
                    return PerSaveOptions.Instance.CostOfTierOnePrestige;
                case 2:
                    return PerSaveOptions.Instance.CostOfTierTwoPrestige;
                default:
                    Logger.LogWarning("Tier for profession not found, defaulting to a cost of 1.");
                    return 1;
            }
        }
    }
}
