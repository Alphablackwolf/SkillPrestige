using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SkillPrestige.Extensions;

namespace SkillPrestige.Menus.Elements
{
    /// <summary>
    /// Evenly distributes elements across an area in rows.
    /// </summary>
    internal static class ElementArranger
    {

        public static void Arrange(Rectangle canvasArea, IList<IArrangeableElement> elements, int horizontalPadding = 0, int verticalPadding = 0)
        {
            var rowElementCountList = GetNumberOfElementsPerRow(canvasArea, elements, horizontalPadding).ToList();

            var rows = GetRows(elements, rowElementCountList);
            var totalHeight = GetTotalHeight(rows, verticalPadding);
            var yLocation = canvasArea.Y;
            if (totalHeight >= canvasArea.Height)
            {
                yLocation += verticalPadding;
                foreach (var row in rows)
                {
                    var centerPointXSpacing = canvasArea.Width / (row.Count + 1);
                    var nextElementXCenter = centerPointXSpacing;
                    var rowHeight = 0;
                    foreach (var element in row)
                    {
                        var xLocation = canvasArea.X + nextElementXCenter - element.CalculateWidth() / 2;
                        var elementHeight = element.CalculateHeight();
                        if (elementHeight > rowHeight) rowHeight = elementHeight;
                        element.SetBounds(new Rectangle(xLocation, yLocation, element.CalculateWidth(), elementHeight));
                        nextElementXCenter += centerPointXSpacing;
                    }
                    yLocation += rowHeight + verticalPadding;
                }
            }
            else
            {
                var centerPointYSpacing = canvasArea.Height / (rows.Count + 1);
                var nextRowYCenter = centerPointYSpacing;
                foreach (var row in rows)
                {
                    var centerPointXSpacing = canvasArea.Width / (row.Count + 1);
                    var nextElementXCenter = centerPointXSpacing;
                    foreach (var element in row)
                    {
                        var xLocation = canvasArea.X + nextElementXCenter - element.CalculateWidth() / 2;
                        var elementHeight = element.CalculateHeight();
                        var elementYLocation = yLocation + nextRowYCenter - elementHeight / 2;
                        element.SetBounds(new Rectangle(xLocation, elementYLocation, element.CalculateWidth(), elementHeight));
                        nextElementXCenter += centerPointXSpacing;
                    }
                    nextRowYCenter += centerPointYSpacing;
                }
            }
        }

        private static IEnumerable<int> GetNumberOfElementsPerRow(Rectangle canvasArea, ICollection<IArrangeableElement> elements, int horizontalPadding = 0)
        {
            var totalWidth = elements.Sum(x => x.CalculateWidth()) + (elements.Count + 1) * horizontalPadding;
            var numberOfRows = ((float)totalWidth / canvasArea.Width).Ceiling();
            int numberOfElementsPerRow;
            var numberOfExtraElements = 0;
            if (numberOfRows >= elements.Count)
            {
                numberOfRows = elements.Count;
                numberOfElementsPerRow = 1;
            }
            else
            {
                numberOfElementsPerRow = ((float)elements.Count / numberOfRows).Floor();
                numberOfExtraElements = elements.Count % numberOfRows;
            }
            var totalRemainingElements = elements.Count;
            var currentRow = 1;
            var startingRowRemainingSpace = canvasArea.Width - horizontalPadding;
            var currentRowRemainingSpace = startingRowRemainingSpace;
            var elementsInCurrentRow = 0;
            var rowElementCountList = new List<int>();
            foreach (var element in elements)
            {
                if (element.CalculateWidth() + horizontalPadding > currentRowRemainingSpace)
                {
                    //button won't fit in row.
                    if (elementsInCurrentRow == 0)
                    {
                        //buttin is only element in the row.
                        numberOfExtraElements += numberOfElementsPerRow - 1;
                        rowElementCountList.Add(1);
                        currentRow++;
                        totalRemainingElements--;
                        continue;
                    }
                    //element is not the only element in the row, but does not fit. move it down to the next row.
                    rowElementCountList.Add(elementsInCurrentRow);
                    currentRow++;
                    numberOfExtraElements += numberOfElementsPerRow - elementsInCurrentRow;
                    elementsInCurrentRow = 1;
                    totalRemainingElements--;
                    currentRowRemainingSpace = startingRowRemainingSpace - (element.CalculateWidth() + horizontalPadding);
                    //if this was the last element, add it as the final row.
                    if (totalRemainingElements <= 0)
                    {
                        rowElementCountList.Add(elementsInCurrentRow);
                    }
                    continue;
                }
                //if the element will fit in the row.
                if (elementsInCurrentRow >= numberOfElementsPerRow)
                {
                    //if there is already a full row.

                    if (numberOfExtraElements > 0)
                    {
                        //if there are extra elements to add.
                        var remainingRows = numberOfRows - (currentRow - 1);
                        var elementsToAddPerRemainingRow = ((float)remainingRows / numberOfExtraElements).Ceiling();
                        if (numberOfElementsPerRow + elementsToAddPerRemainingRow - elementsInCurrentRow > 0)
                        {
                            //if there is a need to add extra elements to this row
                            numberOfExtraElements--;
                            elementsInCurrentRow++;
                            totalRemainingElements--;
                            currentRowRemainingSpace -= element.CalculateWidth() + horizontalPadding;
                            //if this was the last element, add it as the final row.
                            if (totalRemainingElements <= 0)
                            {
                                rowElementCountList.Add(elementsInCurrentRow);
                            }
                            continue;
                        }
                    }
                    //if there is no need to add it to this row, add it to the next.
                    currentRow++;
                    rowElementCountList.Add(elementsInCurrentRow);
                    currentRowRemainingSpace = startingRowRemainingSpace - (element.CalculateWidth() + horizontalPadding);
                    elementsInCurrentRow = 1;
                    totalRemainingElements--;
                    //if this was the last element, add it as the final row.
                    if (totalRemainingElements <= 0)
                    {
                        rowElementCountList.Add(elementsInCurrentRow);
                    }
                    continue;
                }
                //if the element fits in the row, and the row is not full
                currentRowRemainingSpace -= element.CalculateWidth() + horizontalPadding;
                elementsInCurrentRow++;
                totalRemainingElements--;
                //if this was the last element, add it as the final row.
                if (totalRemainingElements <= 0)
                {
                    rowElementCountList.Add(elementsInCurrentRow);
                }
            }
            return rowElementCountList;
        }


        private static IList<IList<IArrangeableElement>> GetRows(ICollection<IArrangeableElement> elements, IEnumerable<int> elementsPerRow)
        {
            var rows = new List<IList<IArrangeableElement>>();
            foreach (var numberOfElements in elementsPerRow)
            {
                rows.Add(new List<IArrangeableElement>(elements.Skip(rows.Sum(x => x.Count)).Take(numberOfElements)));
            }
            return rows;
        }
        private static int GetTotalHeight(IEnumerable<IList<IArrangeableElement>> rows, int verticalPadding)
        {
            return rows.Sum(x => x.Max(y => y.CalculateHeight()) + verticalPadding) + verticalPadding;
        }
    }
}
