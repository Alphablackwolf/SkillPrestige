using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;

namespace SkillPrestige
{
    /// <summary>
    /// A holder to contain replacement routines for the Stardew Valley Crop class.
    /// </summary>
    public class MilkPailReplacement : MilkPail
    {
        /// <summary>
        /// Pulled from the decompiled Stardew Valley code, *slightly* reworked for readability. 
        /// Added quantity adjustment.
        /// </summary>
        public override void DoFunction(GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            base.DoFunction(location, x, y, power, who);
            who.Stamina -= 4f;
            currentParentTileIndex = 6;
            indexOfMenuItemView = 6;
            var animalBeingMilked = (FarmAnimal)this.GetInstanceField("animal");
            if (animalBeingMilked != null && animalBeingMilked.currentProduce > 0 && animalBeingMilked.age >= animalBeingMilked.ageWhenMature && animalBeingMilked.toolUsedForHarvest.Equals(name))
            {
                var farmer = who;
                var @object = new Object(Vector2.Zero, animalBeingMilked.currentProduce, null, false, true, false, false)
                {
                    quality = animalBeingMilked.produceQuality
                };
                if (AnimalProduceHandler.ShouldAnimalProductQuanityIncrease()) @object.addToStack(1);
                if (farmer.addItemToInventoryBool(@object))
                {
                    Game1.playSound("coin");
                    animalBeingMilked.currentProduce = -1;
                    if (animalBeingMilked.showDifferentTextureWhenReadyForHarvest)
                        animalBeingMilked.sprite.Texture = Game1.content.Load<Texture2D>("Animals\\Sheared" + animalBeingMilked.type);
                    who.gainExperience(0, 5);
                }
            }
            this.SetInstanceField("animal", (FarmAnimal)null);
            if (Game1.activeClickableMenu == null)
                who.CanMove = true;
            else
                who.Halt();
            who.usingTool = false;
            who.canReleaseTool = true;
        }
    }
}