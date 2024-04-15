using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SkillPrestige.Extensions
{
    /// <summary>
    /// Display Extension methods created and used for SkillPrestige.
    /// </summary>
    public static class DisplayExtensions
    {
        /// <summary>
        /// Word-wraps text for XNA sprite fonts
        /// </summary>
        /// <param name="text">the text that will be word-wrapped</param>
        /// <param name="font">the XNA Sprite Font to measure the string with.</param>
        /// <param name="maxLineWidth">The maximum line width in pixels before a word is wrapped.</param>
        /// <returns></returns>
        public static string WrapText(this string text, SpriteFont font, float maxLineWidth)
        {
            var words = text.Split(' ');
            var stringBuilder = new StringBuilder();
            var lineWidth = 0f;
            var spaceWidth = font.MeasureString(" ").X;

            foreach (var word in words)
            {
                var size = font.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    stringBuilder.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    if (size.X > maxLineWidth)
                    {
                        if (stringBuilder.ToString() == string.Empty)
                        {
                            stringBuilder.Append((word.Insert(word.Length / 2, " ") + " ").WrapText(font, maxLineWidth));
                        }
                        else
                        {
                            stringBuilder.Append("\n" + (word.Insert(word.Length / 2, " ") + " ").WrapText(font, maxLineWidth));
                        }
                    }
                    else
                    {
                        stringBuilder.Append("\n" + word + " ");
                        lineWidth = size.X + spaceWidth;
                    }
                }
            }
            return stringBuilder.ToString();
        }
        
        /// <summary>
        /// Removes all non-alphanumeric characters.
        /// </summary>
        /// <param name="value">The string to manipulate.</param>
        /// <returns>The value string given, sans any non-alphanumeric characters.</returns>
        public static string RemoveNonAlphanumerics(this string value)
        {
            foreach (var character in value)
            {
                if (!char.IsLetterOrDigit(character))
                    value = value.Replace(character.ToString(), "");
            }
            return value;
        }
    }
}
