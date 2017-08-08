using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SkillPrestige.Menus.Elements.Buttons;

namespace SkillPrestige.Menus
{
    /// <summary>
    /// Handles the determination of where a collection of buttons should go on the prestige menu.
    /// </summary>
    internal static class PrestigeMenuButtonArranger
    {

        /// <summary>
        /// Sets button bounds distributed within the canvas area given.
        /// </summary>
        /// <param name="canvasArea"></param>
        /// <param name="buttons"></param>
        /// <param name="getHeight"></param>
        /// <param name="getWidth"></param>
        /// <param name="horizontalPadding"></param>
        /// <param name="verticalPadding"></param>
        /// <returns></returns>
        public static IDictionary<Button, Rectangle> GetButtonArrangement(Rectangle canvasArea, IEnumerable<Button> buttons, Func<string, int> getHeight, Func<string, int> getWidth, int horizontalPadding = 0, int verticalPadding = 0)
        {
            //TODO: Refactor the crap out of this method and class.

            var buttonList = buttons.ToList();
            var totalWidth = buttonList.Sum(x => getWidth(x.Text)) + (buttonList.Count + 1) * horizontalPadding;
            var numberOfRows = ((float)totalWidth / canvasArea.Width).Ceiling();
            int numberOfButtonsPerRow;
            var numberOfExtraButtons = 0;
            if (numberOfRows >= buttonList.Count)
            {
                numberOfRows = buttonList.Count;
                numberOfButtonsPerRow = 1;
            }
            else
            {
                numberOfButtonsPerRow = ((float)buttonList.Count / numberOfRows).Floor();
                numberOfExtraButtons = buttonList.Count % numberOfRows;
            }
            var totalRemainingButtons = buttonList.Count;
            var currentRow = 1;
            var startingRowRemainingSpace = canvasArea.Width - horizontalPadding;
            var currentRowRemainingSpace = startingRowRemainingSpace;
            var buttonsInCurrentRow = 0;
            var rowButtonCountList = new List<int>();
            foreach (var button in buttonList)
            {
                if (getWidth(button.Text) + horizontalPadding > currentRowRemainingSpace)
                {
                    //button won't fit in row.
                    if (buttonsInCurrentRow == 0)
                    {
                        //buttin is only button in the row.
                        numberOfExtraButtons += numberOfButtonsPerRow - 1;
                        rowButtonCountList.Add(1);
                        currentRow++;
                        totalRemainingButtons--;
                        continue;
                    }
                    //button is not the only button in the row, but does not fit. move it down to the next row.
                    rowButtonCountList.Add(buttonsInCurrentRow);
                    currentRow++;
                    numberOfExtraButtons += numberOfButtonsPerRow - buttonsInCurrentRow;
                    buttonsInCurrentRow = 1;
                    totalRemainingButtons--;
                    currentRowRemainingSpace = startingRowRemainingSpace - (getWidth(button.Text) + horizontalPadding);
                    //if this was the last button, add it as the final row.
                    if (totalRemainingButtons <= 0)
                    {
                        rowButtonCountList.Add(buttonsInCurrentRow);
                    }
                    continue;
                }
                //if the button will fit in the row.
                if (buttonsInCurrentRow >= numberOfButtonsPerRow)
                {
                    //if there is already a full row.

                    if (numberOfExtraButtons > 0)
                    {
                        //if there are extra buttons to add.
                        var remainingRows = numberOfRows - (currentRow - 1);
                        var buttonsToAddPerRemainingRow = ((float)remainingRows / numberOfExtraButtons).Ceiling();
                        if (numberOfButtonsPerRow + buttonsToAddPerRemainingRow - buttonsInCurrentRow > 0)
                        {
                            //if there is a need to add extra buttons to this row
                            numberOfExtraButtons--;
                            buttonsInCurrentRow++;
                            totalRemainingButtons--;
                            currentRowRemainingSpace -= getWidth(button.Text) + horizontalPadding;
                            //if this was the last button, add it as the final row.
                            if (totalRemainingButtons <= 0)
                            {
                                rowButtonCountList.Add(buttonsInCurrentRow);
                            }
                            continue;
                        }
                    }
                    //if there is no need to add it to this row, add it to the next.
                    currentRow++;
                    rowButtonCountList.Add(buttonsInCurrentRow);
                    currentRowRemainingSpace = startingRowRemainingSpace - (getWidth(button.Text) + horizontalPadding);
                    buttonsInCurrentRow = 1;
                    totalRemainingButtons--;
                    //if this was the last button, add it as the final row.
                    if (totalRemainingButtons <= 0)
                    {
                        rowButtonCountList.Add(buttonsInCurrentRow);
                    }
                    continue;
                }
                //if the button fits in the row, and the row is not full
                currentRowRemainingSpace -= getWidth(button.Text) + horizontalPadding;
                buttonsInCurrentRow++;
                totalRemainingButtons--;
                //if this was the last button, add it as the final row.
                if (totalRemainingButtons <= 0)
                {
                    rowButtonCountList.Add(buttonsInCurrentRow);
                }
            }
            var returnDictionary = new Dictionary<Button, Rectangle>();
            var yLocation = verticalPadding;
            foreach (var buttonsToAddToThisRow in rowButtonCountList)
            {
                var buttonsForThisRow = buttonList.Take(buttonsToAddToThisRow).ToList();
                var centerPointXSpacing = canvasArea.Width / (buttonsToAddToThisRow + 1);
                var nextButtonXCenter = centerPointXSpacing;
                var rowHeight = 0;
                foreach (var button in buttonsForThisRow)
                {
                    var xLocation = nextButtonXCenter - getWidth(button.Text) / 2;
                    var buttonHeight = getHeight(button.Text);
                    if (buttonHeight > rowHeight) rowHeight = buttonHeight;
                    returnDictionary.Add(button, new Rectangle(xLocation, yLocation, getWidth(button.Text), buttonHeight));
                    nextButtonXCenter += centerPointXSpacing;
                }
                yLocation += rowHeight + verticalPadding;
                buttonList = buttonList.Skip(buttonsToAddToThisRow).ToList();
            }
            return returnDictionary;
        }
    }
}
