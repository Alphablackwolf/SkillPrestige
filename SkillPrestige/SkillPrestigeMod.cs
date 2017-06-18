using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Commands;
using SkillPrestige.Logging;
using SkillPrestige.Menus;
using SkillPrestige.Menus.Elements.Buttons;
using SkillPrestige.Mods;
using SkillPrestige.Professions;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using static SkillPrestige.InputHandling.Mouse;

namespace SkillPrestige
{
    /// <summary>The Skill Prestige Mod by Alphablackwolf. Enjoy!</summary>
    public class SkillPrestigeMod : Mod
    {
        private static bool IsFirstUpdate = true;

        public static string ModPath { get; private set; }

        public static string OptionsPath { get; private set; }

        public static IMonitor LogMonitor { get; private set; }

        public static string CurrentSaveOptionsPath { get; private set; }

        public static string PerSaveOptionsDirectory { get; private set; }

        public static Texture2D PrestigeIconTexture { get; private set; }

        public static Texture2D CheckmarkTexture { get; private set; }

        public static IModHelper Helper;

        public override void Entry(IModHelper helper)
        {
            // initialise
            SkillPrestigeMod.Helper = helper;
            LogMonitor = Monitor;
            ModPath = helper.DirectoryPath;
            PerSaveOptionsDirectory = Path.Combine(ModPath, "psconfigs/");
            OptionsPath = Path.Combine(ModPath, "config.json");
            Logger.LogInformation("Detected game entry.");

            // read data
            PrestigeSaveData.Instance.Read();

            // check for mod conflicts
            if (helper.ModRegistry.IsLoaded("community.AllProfessions"))
            {
                Logger.LogCriticalWarning("Conflict Detected. This mod cannot work with AllProfessions. Skill Prestige disabled.");
                Logger.LogDisplay("Skill Prestige Mod: If you wish to use this mod in place of AllProfessions, remove the AllProfessions mod and run the player_resetallprofessions command.");
                return;
            }

            // load sprites
            LoadSprites();

            // register events
            ControlEvents.MouseChanged += MouseChanged;
            LocationEvents.CurrentLocationChanged += LocationChanged;
            GraphicsEvents.OnPostRenderGuiEvent += PostRenderGuiEvent;
            GameEvents.UpdateTick += GameUpdate;

            Logger.LogDisplay($"{this.ModManifest.Name} version {this.ModManifest.Version} by {this.ModManifest.Author} Initialized.");
        }

        private static void MouseChanged(object sender, EventArgsMouseStateChanged args)
        {
            HandleState(args);
        }

        private static void LocationChanged(object sender, EventArgs args)
        {
            Logger.LogVerbose("Location change detected.");
            CurrentSaveOptionsPath = Path.Combine(ModPath, "psconfigs/", $@"{Game1.player.name.RemoveNonAlphanumerics()}_{Game1.uniqueIDForThisGame}.json");
            PrestigeSaveData.Instance.UpdateCurrentSaveFileInformation();
            PerSaveOptions.Instance.Check();
            Profession.AddMissingProfessions();
        }

        private static void PostRenderGuiEvent(object sender, EventArgs args)
        {
            SkillsMenuExtension.AddPrestigeButtonsToMenu();
        }

        private static void LoadSprites()
        {
            Logger.LogInformation("Loading sprites...");
            Button.DefaultButtonTexture = Game1.content.Load<Texture2D>(@"LooseSprites\DialogBoxGreen");
            MinimalistProfessionButton.ProfessionButtonTexture = Game1.content.Load<Texture2D>(@"LooseSprites\boardGameBorder");

            var prestigeIconFilePath = Path.Combine(ModPath, @"PrestigeIcon.png");
            Logger.LogInformation($"Prestige Icon Path: {prestigeIconFilePath}");
            var prestigeIconFileStream = new FileStream(prestigeIconFilePath, FileMode.Open);
            PrestigeIconTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, prestigeIconFileStream);

            var checkmarkFilePath = Path.Combine(ModPath, @"Checkmark.png");
            Logger.LogInformation($"Checkmark Path: {checkmarkFilePath}");
            var checkmarkFileStream = new FileStream(checkmarkFilePath, FileMode.Open);
            CheckmarkTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, checkmarkFileStream);
            Logger.LogInformation("Sprites loaded.");
        }

        private static void GameUpdate(object sender, EventArgs args)
        {
            if (IsFirstUpdate)
            {
                AfterModsInitialised();
                IsFirstUpdate = false;
            }

            CheckForGameSave();
            CheckForLevelUpMenu();
        }

        private static void AfterModsInitialised()
        {
            // load mods
            ModHandler.RegisterLoadedMods();

            // register commands
            if (Options.Instance.TestingMode)
                RegisterTestingCommands();
            RegisterCommands();
        }

        private static void CheckForGameSave()
        {
            if (!Game1.newDay || Game1.fadeToBlackAlpha <= 0.95f) return;
            Logger.LogInformation("New game day detected.");
            PrestigeSaveData.Instance.Save();
        }

        private static void CheckForLevelUpMenu()
        {
            var levelUpMenu = Game1.activeClickableMenu as LevelUpMenu;
            var currentLevel = (int?)levelUpMenu?.GetInstanceField("currentLevel");
            if (currentLevel % 5 != 0) return;
            Logger.LogInformation("Level up menu as profession chooser detected.");
            var currentSkill = (int)levelUpMenu.GetInstanceField("currentSkill");
            Game1.activeClickableMenu = new LevelUpMenuDecorator(currentSkill, currentLevel.Value);
            Logger.LogInformation("Replaced level up menu with custom menu.");
        }

        private static void RegisterTestingCommands()
        {
            Logger.LogInformation("Registering Testing commands...");
            SkillPrestigeCommand.RegisterCommands(SkillPrestigeMod.Helper.ConsoleCommands, true);
            Logger.LogInformation("Testing commands registered.");
        }

        private static void RegisterCommands()
        {
            Logger.LogInformation("Registering commands...");
            SkillPrestigeCommand.RegisterCommands(SkillPrestigeMod.Helper.ConsoleCommands, false);
            Logger.LogInformation("Commands registered.");
        }
    }
}