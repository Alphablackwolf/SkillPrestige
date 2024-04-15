﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Extensions;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Menus.Elements.Buttons
{
    internal class Checkbox : Button
    {
        private const int PixelsWide = 9;
        private static int Width => PixelsWide*Game1.pixelZoom;
        private bool _isChecked;
        protected override string HoverText => string.Empty;
        protected override string Text { get; }

        public delegate void ClickCallback(bool isChecked);

        private readonly ClickCallback _onClick;

        public Checkbox(bool isChecked, string text, Rectangle bounds, ClickCallback onClickCallback)
        {
            _isChecked = isChecked;
            _onClick = onClickCallback;
            Bounds = bounds;
            Text = text;
        }

        protected override void OnMouseClick()
        {
            Game1.playSound("drumkit6");
            _isChecked = !_isChecked;
            _onClick.Invoke(_isChecked);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var location = new Vector2(Bounds.X, Bounds.Y);
            spriteBatch.Draw(Game1.mouseCursors, location, _isChecked ? OptionsCheckbox.sourceRectChecked : OptionsCheckbox.sourceRectUnchecked, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.4f);
            Utility.drawTextWithShadow(spriteBatch, Text, Game1.dialogueFont, new Vector2(location.X + Width + Game1.pixelZoom * 2, location.Y), Game1.textColor, 1f, 0.1f);
        }

        public override int CalculateWidth()
        {
            return Width + Game1.pixelZoom * 2 + (Game1.dialogueFont.MeasureString(Text).X).Ceiling();
        }

        public override int CalculateHeight()
        {
            return Width;
        }
    }
}
