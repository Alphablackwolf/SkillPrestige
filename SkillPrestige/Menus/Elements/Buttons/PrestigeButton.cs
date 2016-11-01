using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Logging;
using SkillPrestige.Menus.Dialogs;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Menus.Elements.Buttons
{
    /// <summary>
    /// Represents a prestige button inside the prestige menu.
    /// </summary>
    public class PrestigeButton : Button
    {

        public PrestigeButton(bool isDisabled)
        {
            TitleTextFont = Game1.dialogueFont;
            IsDisabled = isDisabled;
        }

        public Skill Skill { get; set; }
        private bool IsDisabled { get; }
        private Color DisplayColor => IsDisabled ? Color.Gray : Color.White;


        protected override string HoverText => IsDisabled ? $"You must reach level 10 in this skill and then{Environment.NewLine}sleep at least once in order to prestige this skill." : $"Click to prestige your {Skill?.Type?.Name} skill";
        protected override string Text => "Prestige";

        public override void Draw(SpriteBatch spriteBatch)  
        {
            spriteBatch.Draw(ButtonTexture, Bounds, DisplayColor);
            DrawTitleText(spriteBatch);
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
            //Magic numbers for tile size multipliers have been determined through trial and error.
            var dialogWidth = Game1.tileSize * 12;
            var dialogHeight = Game1.tileSize * 6;

            var viewport = Game1.graphics.GraphicsDevice.Viewport;
            var screenXCenter = (int)(viewport.Width * (1.0 / Game1.options.zoomLevel)) / 2;
            var screenYCenter = (int)(viewport.Height * (1.0 / Game1.options.zoomLevel)) / 2;
            var dialogXCenter = (dialogWidth + IClickableMenu.borderWidth * 2) / 2;
            var dialogYCenter = (dialogHeight + IClickableMenu.borderWidth * 2) / 2;
            var bounds = new Rectangle(screenXCenter - dialogXCenter, screenYCenter - dialogYCenter,
                dialogWidth + IClickableMenu.borderWidth * 2, dialogHeight + IClickableMenu.borderWidth * 2);
            Logger.LogVerbose($"{Skill.Type.Name} skill prestige attempted.");
            var message = $"Are you sure you wish to prestige your {Skill.Type.Name} skill? This cannot be undone and will revert you back to level 0 {Skill.Type.Name}. All associated benefits {(PerSaveOptions.Instance.ResetRecipesOnPrestige ? "and" : "except for")} crafting/cooking recipes will be lost.";
            Game1.activeClickableMenu.exitThisMenuNoSound();
            Game1.activeClickableMenu = new WarningDialog(bounds, message, () => { Prestige.PrestigeSkill(Skill); },
                () => { });
            
        }
    }
}