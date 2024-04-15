using Microsoft.Xna.Framework;

namespace SkillPrestige.Menus.Elements
{
    internal interface IArrangeableElement
    {
        int CalculateWidth();

        int CalculateHeight();

        void SetBounds(Rectangle bounds);
    }
}