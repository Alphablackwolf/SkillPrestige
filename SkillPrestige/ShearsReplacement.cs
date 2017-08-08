using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace SkillPrestige
{
    /// <summary>
    /// A holder to contain replacement routines for the Stardew Valley Crop class.
    /// </summary>
    public class ShearsReplacement : Shears
    {
        /// <summary>
        /// Pulled from the decompiled Stardew Valley code, *slightly* reworked for readability. 
        /// Added quantity adjustment.
        /// </summary>
        public override void DoFunction(GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            base.DoFunction(location, x, y, power, who);
            who.Stamina -= 4f;
            playSnip(who);
            currentParentTileIndex = 7;
            indexOfMenuItemView = 7;
            var animalBeingSheared = (FarmAnimal)this.GetInstanceField("animal");
            if (animalBeingSheared != null && animalBeingSheared.currentProduce > 0 && animalBeingSheared.age >= animalBeingSheared.ageWhenMature && animalBeingSheared.toolUsedForHarvest.Equals(name))
            {
                var farmer = who;
                var @object = new Object(Vector2.Zero, animalBeingSheared.currentProduce, null, false, true, false, false)
                {
                    quality = animalBeingSheared.produceQuality
                };
                if (AnimalProduceHandler.ShouldAnimalProductQuanityIncrease()) @object.addToStack(1);
                if (farmer.addItemToInventoryBool(@object))
                {
                    animalBeingSheared.currentProduce = -1;
                    Game1.playSound("coin");
                    animalBeingSheared.friendshipTowardFarmer = Math.Min(1000, animalBeingSheared.friendshipTowardFarmer + 5);
                    if (animalBeingSheared.showDifferentTextureWhenReadyForHarvest)
                        animalBeingSheared.sprite.Texture = Game1.content.Load<Texture2D>("Animals\\Sheared" + animalBeingSheared.type);
                    who.gainExperience(0, 5);
                }
            }
            else
            {
                var dialogue = "";
                if (animalBeingSheared != null && !animalBeingSheared.toolUsedForHarvest.Equals(name))
                    dialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Shears.cs.14245", animalBeingSheared.displayName as object);
                if (animalBeingSheared != null && animalBeingSheared.isBaby() && animalBeingSheared.toolUsedForHarvest.Equals(name))
                    dialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Shears.cs.14246", animalBeingSheared.displayName as object);
                if (animalBeingSheared != null && animalBeingSheared.age >= animalBeingSheared.ageWhenMature && animalBeingSheared.toolUsedForHarvest.Equals(name))
                    dialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Shears.cs.14247", animalBeingSheared.displayName as object);
                if (dialogue.Length > 0)
                    Game1.drawObjectDialogue(dialogue);
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