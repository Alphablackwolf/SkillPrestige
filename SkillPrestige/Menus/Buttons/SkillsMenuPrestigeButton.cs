using StardewValley;

namespace SkillPrestige.Menus.Buttons
{
    /// <summary>
    /// represents a prestige button on the skills page of Stardew Valley that opens the prestige menu.
    /// </summary>
    public class SkillsMenuPrestigeButton : Button
    {
        public SkillsMenuPrestigeButton()
        {
            TitleTextFont = Game1.smallFont;
        }

        public Skill Skill { get; set; }

        protected override string HoverText => "Click to open the Prestige menu.";
        protected override string Text => "Prestige";

        protected override void OnMouseClick()
        {
            SkillsMenuExtension.OpenPrestigeMenu(Skill);
        }

        protected override void OnMouseHover()
        {
            base.OnMouseHover();
            Game1.playSound("smallSelect");
        }
    }
}