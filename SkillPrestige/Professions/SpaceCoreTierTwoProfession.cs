using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;

namespace SkillPrestige.Professions;

public class SpaceCoreTierTwoProfession : TierTwoProfession
{
    public SpaceCoreTierTwoProfession() {}

    public SpaceCoreTierTwoProfession(Skills.Skill.Profession spaceCoreProfession)
    {
        this.DisplayName = spaceCoreProfession.GetName();
        this.EffectText = new List<string> { spaceCoreProfession.GetDescription() };
        this.Id = spaceCoreProfession.GetVanillaId();
        this.Texture = spaceCoreProfession.Icon;
    }

    public sealed override Texture2D Texture { get; set; }

    public override Rectangle IconSourceRectangle => new (0, 0, 16, 16);
}
