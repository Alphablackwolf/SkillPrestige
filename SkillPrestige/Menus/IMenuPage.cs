using Microsoft.Xna.Framework.Graphics;

namespace SkillPrestige.Menus
{
    internal interface IMenuPage
    {
        int PageIndex { get; set; }

        void Draw(SpriteBatch spriteBatch);

        void RegisterControls();

        void DeegisterControls();
    }
}
