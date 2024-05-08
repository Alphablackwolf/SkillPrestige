using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Framework;
using SkillPrestige.Framework.Commands;
using SkillPrestige.Framework.InputHandling;
using SkillPrestige.Framework.Menus;
using SkillPrestige.Framework.Menus.Elements.Buttons;
using SkillPrestige.Logging;
using SkillPrestige.Menus;
using SkillPrestige.Mods;
using SkillPrestige.Professions;
using SpaceCore;
using SpaceCore.Interface;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

// ReSharper disable MemberCanBeMadeStatic.Local

namespace SkillPrestige
{
    /// <summary>The mod entry class.</summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    internal class ModEntry : Mod
    {
        private bool SaveIsLoaded { get; set; }

        public static string ModPath { get; private set; }

        public static ModConfig Config { get; private set; }

        public static IMonitor LogMonitor { get; private set; }

        public static string CurrentSaveOptionsPath => Path.Combine(ModPath, "psconfigs/", Constants.SaveFolderName!);

        public static string PerSaveOptionsDirectory { get; private set; }

        public static Texture2D CheckmarkTexture { get; private set; }

        private static string PrestigeSaveKey => "SkillPrestigeData";

        public static IModRegistry ModRegistry { get; private set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // init
            LogMonitor = this.Monitor;
            ModPath = helper.DirectoryPath;
            ModRegistry = helper.ModRegistry;
            PerSaveOptionsDirectory = Path.Combine(ModPath, "psconfigs/");
            Config = helper.ReadConfig<ModConfig>();

            // disable mod if All Professions is installed
            if (this.Helper.ModRegistry.IsLoaded("community.AllProfessions"))
            {
                Logger.LogCriticalWarning("Conflict Detected. This mod cannot work with AllProfessions. Skill Prestige disabled.");
                Logger.LogDisplay("Skill Prestige Mod: If you wish to use this mod in place of AllProfessions, remove the AllProfessions mod and run the player_resetallprofessions command.");
                return;
            }

            if (!this.Helper.ModRegistry.IsLoaded("CinderGarde.SkillPrestige.CP"))
            {
                Logger.LogCriticalWarning("SkillPrestige [CP] not found, prestige statue will not be added to crafting recipes or textures");
                Logger.LogDisplay("Skill Prestige Mod: If you wish to use this mod, please include the SkillPrestige [CP] folder from the download.");
            }

            // hook events
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.Saved += this.OnSaved;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Input.CursorMoved += this.OnCursorMoved;
            helper.Events.Display.RenderedActiveMenu += this.OnRenderedActiveMenu;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {


            if (Game1.activeClickableMenu is null
                && this.SaveIsLoaded
                && e.Button.IsOneOf(SButton.MouseRight, SButton.ControllerA))
            {
                var tileToCheck = e.Button switch
                {
                    SButton.MouseRight => Game1.currentCursorTile,
                    SButton.ControllerA => Game1.player.GetGrabTile(),
                    _ => Vector2.Zero
                };
                if(Game1.currentLocation.objects.ContainsKey(tileToCheck)
                    && Game1.currentLocation.objects[tileToCheck].name == "Prestige Statue")
                    this.OpenSelectionMenu();
            }
            bool isClick = e.Button.IsOneOf(SButton.MouseLeft, SButton.ControllerA);
            // ReSharper disable once InvertIf
            if (Game1.activeClickableMenu is not null && Game1.activeClickableMenu is IInputHandler handler)
                    handler.OnButtonPressed(e, isClick);
        }

        private void OpenSelectionMenu(bool playSound = true)
        {
            Logger.LogVerbose("Opening Selection Menu...");
            const int menuWidth = Game1.tileSize * 10;
            const int menuHeight = Game1.tileSize * 10;

            int menuXCenter = (menuWidth + IClickableMenu.borderWidth * 2) / 2;
            int menuYCenter = (menuHeight + IClickableMenu.borderWidth * 2) / 2;
            int screenXCenter = Game1.uiViewport.Width / 2;
            int screenYCenter = Game1.uiViewport.Height / 2;
            var bounds = new Rectangle(screenXCenter - menuXCenter, screenYCenter - menuYCenter, menuWidth + IClickableMenu.borderWidth * 2, menuHeight + IClickableMenu.borderWidth * 2);
            if(playSound) Game1.playSound("bigSelect");
            Game1.activeClickableMenu = new SelectionMenu(bounds);
        }

        /// <summary>Raised after the player moves the in-game cursor.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnCursorMoved(object sender, CursorMovedEventArgs e)
        {
            if (Game1.activeClickableMenu is IInputHandler handler)
                handler.OnCursorMoved(e);
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
#pragma warning disable CS0612 // Type or member is obsolete, required for backwards compatability
            PrestigeSaveData.Instance.UpdateCurrentSaveFileInformation();

            PrestigeSet.Load();
            PrestigeSaveData.MigrateData();

#pragma warning restore CS0612 // Type or member is obsolete

            PerSaveOptions.Instance.Check();
            Profession.AddMissingProfessions();
            RecipeHandler.LoadRecipes();

            var currentPrestiges = PrestigeSet.Instance.Prestiges.ToList();
            foreach (var entry in ModHandler.GetAddedEmptyPrestiges())
            {
                if (currentPrestiges.Any(x => x.SkillType.Name == entry.SkillType.Name))
                    continue;
                Logger.LogInformation($"Adding empty prestige set for skill {entry.SkillType.Name}");
                PrestigeSet.Instance.Prestiges = PrestigeSet.Instance.Prestiges.Append(entry);
            }
            this.SaveIsLoaded = true;
        }

        /// <summary>Raised after the game returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
#pragma warning disable CS0612 // Type or member is obsolete, required for backwards compatability
            PrestigeSaveData.Read();
#pragma warning restore CS0612 // Type or member is obsolete
            this.SaveIsLoaded = false;
            PrestigeSet.UnLoad();
            Logger.LogInformation("Return To Title.");
            PerSaveOptions.ClearLoadedPerSaveOptionsFile();
            ExperienceHandler.ResetExperience();
            RecipeHandler.ResetRecipes();
        }

        private bool SelectionMenuPendingReopen;
        private bool PrestigeMenuPendingReopen;
        /// <summary>When a menu is open (<see cref="Game1.activeClickableMenu"/> isn't null), raised after that menu is drawn to the sprite batch but before it's rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (this.SelectionMenuPendingReopen && Game1.activeClickableMenu is SelectionMenu)
            {
                this.OpenSelectionMenu(false);
                this.SelectionMenuPendingReopen = false;
            }

            if (this.PrestigeMenuPendingReopen && Game1.activeClickableMenu is PrestigeMenu)
            {
                var currentPrestigeMenu = (PrestigeMenu)Game1.activeClickableMenu;
                var currentPrestigeMenuBounds = new Rectangle(currentPrestigeMenu.xPositionOnScreen, currentPrestigeMenu.yPositionOnScreen, currentPrestigeMenu.width, currentPrestigeMenu.height);
                var prestige = PrestigeSet.Instance.Prestiges.Single(x => x.SkillType == currentPrestigeMenu.Skill.Type);
                Game1.activeClickableMenu = new PrestigeMenu(currentPrestigeMenuBounds, currentPrestigeMenu.Skill, prestige);
                this.PrestigeMenuPendingReopen = false;
            }
            switch (Game1.nextClickableMenu.FirstOrDefault())
            {
                case SelectionMenu:
                    this.SelectionMenuPendingReopen = true;
                    break;
                case PrestigeMenu:
                    this.PrestigeMenuPendingReopen = true;
                    break;
            }
        }

        private void LoadSprites()
        {
            Logger.LogInformation("Loading sprites...");
            Button.DefaultButtonTexture = Game1.content.Load<Texture2D>(@"LooseSprites\DialogBoxGreen");
            MinimalistProfessionButton.ProfessionButtonTexture = Game1.content.Load<Texture2D>(@"LooseSprites\boardGameBorder");

            string checkmarkFilePath = Path.Combine(ModPath, "assets", "checkmark.png");
            Logger.LogInformation($"Checkmark Path: {checkmarkFilePath}");
            var checkmarkFileStream = new FileStream(checkmarkFilePath, FileMode.Open);
            CheckmarkTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, checkmarkFileStream);
            Logger.LogInformation("Sprites loaded.");
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // init mod
            this.LoadSprites();
#pragma warning disable CS0612 // Type or member is obsolete - need to interact with obsolete element to ensure backwards compatibility.
            PrestigeSaveData.Read();
#pragma warning restore CS0612 // Type or member is obsolete
            PrestigeSet.Save = () => this.Helper.Data.WriteSaveData(PrestigeSaveKey, PrestigeSet.Instance);
            PrestigeSet.Read = () => this.Helper.Data.ReadSaveData<PrestigeSet>(PrestigeSaveKey) ?? PrestigeSet.CompleteEmptyPrestigeSet();
            ModHandler.Initialize();
            // register commands
            if (Config.TestingMode) this.RegisterTestingCommands();
            this.RegisterCommands();
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if(e.IsMultipleOf(30)) ModHandler.RegisterPendingSpaceCoreMods();

            this.CheckForLevelUpMenu();
            if (e.IsOneSecond && this.SaveIsLoaded)
                ExperienceHandler.UpdateExperience(); //one second tick for this, as the detection of changed experience can happen as infrequently as possible. a 10 second tick would be well within tolerance.
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            PrestigeSet.Save();
        }

        private void OnSaved(object sender, SavedEventArgs e)
        {
#pragma warning disable CS0612 // Type or member is obsolete, required for backwards compatability
            PrestigeSaveData.CleanupDataFile();
#pragma warning restore CS0612 // Type or member is obsolete
        }


        private void CheckForLevelUpMenu()
        {
            switch (Game1.activeClickableMenu)
            {
                case null:
                    return;
                case LevelUpMenu:
                    int level = (int)Game1.activeClickableMenu.GetInstanceField("currentLevel");
                    if (level % 5 != 0) return;
                    int currentSkillId = (int)Game1.activeClickableMenu.GetInstanceField("currentSkill");

                    var skill = Skill.AllSkills.SingleOrDefault(x => x.Type.Ordinal == currentSkillId);
                    if (skill == null)
                    {
                        Logger.LogCritical($"Unable to load skill id {currentSkillId}, skipping level up menu decoration.");
                    }
                    Logger.LogInformation("Level up menu as profession chooser detected.");
                    Game1.activeClickableMenu = new LevelUpMenuDecorator(skill, level, Game1.activeClickableMenu);
                    Logger.LogInformation("Replaced level up menu with custom menu.");
                    break;
                case SkillLevelUpMenu:
                {
                    int spaceCoreLevel = (int)Game1.activeClickableMenu.GetInstanceField("currentLevel");
                    if (spaceCoreLevel % 5 != 0) return;
                    string currentSkillName = Skills.GetSkill((string)Game1.activeClickableMenu.GetInstanceField("currentSkill")).GetName(); //overwrite name with it's display name

                    var spaceCoreSkill = Skill.AllSkills.SingleOrDefault(x => x.Type.Name == currentSkillName);
                    if (spaceCoreSkill == null)
                    {
                        Logger.LogCritical($"Unable to load skill {currentSkillName}, skipping level up menu decoration.");
                    }
                    Logger.LogInformation("Level up menu as profession chooser detected.");
                    Game1.activeClickableMenu = new LevelUpMenuDecorator(spaceCoreSkill, spaceCoreLevel, Game1.activeClickableMenu);
                    Logger.LogInformation("Replaced level up menu with custom menu.");
                    break;
                }
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!this.SaveIsLoaded) return;
            PrestigeSet.Load();
            RecipeHandler.CheckForAndHandleAddedRecipes();
        }

        private void RegisterTestingCommands()
        {
            Logger.LogInformation("Registering Testing commands...");
            SkillPrestigeCommand.RegisterCommands(this.Helper.ConsoleCommands, true);
            Logger.LogInformation("Testing commands registered.");
        }

        private void RegisterCommands()
        {
            Logger.LogInformation("Registering commands...");
            SkillPrestigeCommand.RegisterCommands(this.Helper.ConsoleCommands, false);
            Logger.LogInformation("Commands registered.");
        }
    }
}
