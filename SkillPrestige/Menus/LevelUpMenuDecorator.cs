using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.InputHandling;
using SkillPrestige.Logging;
using SkillPrestige.Menus.Buttons;
using SkillPrestige.Menus.Dialogs;
using SkillPrestige.Professions;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Menus
{
    /// <summary>
    /// Decorates the Level Up Menu with a prestiged! indicator on prestiged professions.
    /// </summary>
    internal class LevelUpMenuDecorator : LevelUpMenu
    {
        private readonly int _currentSkillId;
        private readonly int _currentLevel;
        private bool _isRightSideOfTree;
        private bool _uiInitiated;
        private bool _drawToggleSwitch;
        private bool _drawLeftPrestigedIndicator;
        private bool _drawRightPrestigedIndicator;
        private TextureButton _levelTenToggleButton;

        private Rectangle MessageDialogBounds
        {
            get
            {
                var viewport = Game1.graphics.GraphicsDevice.Viewport;
                var screenXCenter = viewport.Width / 2;
                var screenYCenter = viewport.Height / 2;
                var dialogWidth = Game1.tileSize*10;
                var dialogHeight = Game1.tileSize*8;
                var xLocation = screenXCenter - width/2;
                var yLocation = screenYCenter - height/2;
                return new Rectangle(xLocation, yLocation, dialogWidth, dialogHeight);
            }
        }
        private Rectangle ExtraTallMessageDialogBounds
        {
            get
            {
                var extraTallRectangle = MessageDialogBounds;
                extraTallRectangle.Height += Game1.tileSize*4;
                return extraTallRectangle;
            }
        }


        public LevelUpMenuDecorator(int skill, int level) : base(skill, level)
        {
            _currentSkillId = skill;
            _currentLevel = level;
            exitFunction = DeregisterMouseEvents;
        }

        private void RegisterMouseEvents()
        {
            Logger.LogInformation("Level Up Menu - Registering mouse events...");
            Mouse.MouseMoved += _levelTenToggleButton.CheckForMouseHover;
            Mouse.MouseClicked += _levelTenToggleButton.CheckForMouseClick;
            Logger.LogInformation("Level Up Menu - Mouse events registered.");
        }

        private void DeregisterMouseEvents()
        {
            Logger.LogInformation("Level Up Menu - Deregistering mouse events...");
            if (_levelTenToggleButton == null) return;
            Mouse.MouseMoved += _levelTenToggleButton.CheckForMouseHover;
            Mouse.MouseClicked += _levelTenToggleButton.CheckForMouseClick;
            Logger.LogInformation("Level Up Menu - Mouse events deregistered.");
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);
            if (!_uiInitiated) InitiateUi();
            DecorateUi(spriteBatch);
            drawMouse(spriteBatch);
        }

        private void InitiateUi()
        {
            if (_uiInitiated) return;
            _uiInitiated = true;
                Logger.LogVerbose("Level Up Menu - initializing UI...");
                var skill = Skill.AllSkills.Single(x => x.Type.Ordinal == _currentSkillId);
                var prestigeData = PrestigeSaveData.CurrentlyLoadedPrestigeSet.Prestiges.Single(x => x.SkillType == skill.Type);
                var prestigedProfessionsForThisSkillAndLevel =
                    skill.Professions.Where(
                        x =>
                            prestigeData.PrestigeProfessionsSelected.Contains(x.Id) &&
                            x.LevelAvailableAt == _currentLevel).ToList();
                var professionsToChooseFrom = skill.Professions.Where(x => x.LevelAvailableAt == _currentLevel).ToList();

                if (_currentLevel == 5)
                {
                    if (!prestigedProfessionsForThisSkillAndLevel.Any())
                    {
                        Logger.LogVerbose(
                            "Level Up Menu - No prestiged professions found for this skill/level combination.");
                        return;
                    }
                    if (prestigedProfessionsForThisSkillAndLevel.Count == 1)
                    {
                        Logger.LogInformation(
                            "Level Up Menu - One level 5 prestiged profession found, automatically selecting the other.");
                        var professionToAdd =
                            professionsToChooseFrom.First(x => !prestigedProfessionsForThisSkillAndLevel.Contains(x));
                        Game1.player.professions.Add(professionToAdd.Id);
                        professionToAdd.SpecialHandling?.ApplyEffect();
                        exitThisMenu(false);
                        Game1.activeClickableMenu = new LevelUpMessageDialogWithProfession(MessageDialogBounds,
                            $"You levelled your {skill.Type.Name} skill to level {_currentLevel} and gained a profession!",
                            skill, professionToAdd);
                        return;
                    }
                    if (prestigedProfessionsForThisSkillAndLevel.Count >= 2)
                    {
                        Logger.LogInformation(
                            "Level Up Menu - Both available level 5 professions are already prestiged.");
                        exitThisMenu(false);
                        Game1.activeClickableMenu = new LevelUpMessageDialog(MessageDialogBounds,
                            $"You levelled your {skill.Type.Name} skill to level {_currentLevel}!", skill);
                        return;
                    }
                }
                if (_currentLevel != 10) return;

                var levelFiveProfessionsCount =
                    Game1.player.professions.Intersect(
                        skill.Professions.Where(x => x is TierOneProfession).Select(x => x.Id)).Count();
                if (levelFiveProfessionsCount == 1)
                {
                    if (!prestigedProfessionsForThisSkillAndLevel.Any())
                    {
                        Logger.LogVerbose(
                            "Level Up Menu - No prestiged professions found for this skill/level combination.");
                        return;
                    }
                    if (prestigedProfessionsForThisSkillAndLevel.Count == 1)
                    {
                        Logger.LogInformation(
                            "Level Up Menu - One level 10 prestiged profession found for only one available level 5 skill (cheater!), automatically selecting the other.");
                        var tierOneProfession =
                            ((TierTwoProfession) prestigedProfessionsForThisSkillAndLevel.First()).TierOneProfession;
                        var professionToAdd =
                            professionsToChooseFrom.First(
                                x =>
                                    (x as TierTwoProfession)?.TierOneProfession == tierOneProfession &&
                                    !prestigedProfessionsForThisSkillAndLevel.Contains(x));
                        Game1.player.professions.Add(professionToAdd.Id);
                        professionToAdd.SpecialHandling?.ApplyEffect();
                        exitThisMenu(false);
                        Game1.activeClickableMenu = new LevelUpMessageDialogWithProfession(ExtraTallMessageDialogBounds,
                            $"You levelled your {skill.Type.Name} skill to level {_currentLevel} and gained a profession! {Environment.NewLine} You may now prestige this skill again!",
                            skill, professionToAdd);
                        return;
                    }
                    if (prestigedProfessionsForThisSkillAndLevel.Count < 2) return;
                    Logger.LogInformation(
                        "Level Up Menu - Only one level 5 profession found with both level 10 professions already prestiged (cheater!).");
                    exitThisMenu(false);
                    Game1.activeClickableMenu = new LevelUpMessageDialog(MessageDialogBounds,
                        $"You levelled your {skill.Type.Name} skill to level {_currentLevel}!  {Environment.NewLine} You may now prestige this skill again!",
                        skill);
                }
                else
                {
                    if (prestigedProfessionsForThisSkillAndLevel.Count <= 2)
                    {
                        Logger.LogInformation(
                            "Level Up Menu - Two or less prestiged level 10 professions found for this skill, with more than one level 5 profession found.");
                        if (prestigedProfessionsForThisSkillAndLevel.Intersect(professionsToChooseFrom.Take(2)).Count() == 2)
                        {
                            Logger.LogInformation("Level Up Menu - All of one level 10 profession branch found, switching to remaining menu.");
                            ToggleLevelTenMenu();
                            return;
                        }
                    if (prestigedProfessionsForThisSkillAndLevel.Intersect(professionsToChooseFrom.Skip(2).Take(2)).Count() == 2)
                    {
                        Logger.LogInformation("Level Up Menu - All of one level 10 profession branch found, leaving at default menu.");
                        return;
                    }
                    Logger.LogInformation(
                            "Level Up Menu - Both level up menus found as viable, enabling user side toggle.");
                        SetupLevelTenToggleButton();
                        _drawToggleSwitch = true;
                        _drawLeftPrestigedIndicator =
                            prestigedProfessionsForThisSkillAndLevel.Contains(
                                professionsToChooseFrom.Skip(_isRightSideOfTree == false ? 0 : 2).First());
                        _drawRightPrestigedIndicator =
                            prestigedProfessionsForThisSkillAndLevel.Contains(
                                professionsToChooseFrom.Skip(_isRightSideOfTree == false ? 1 : 3).First());
                        return;
                    }
                    if (prestigedProfessionsForThisSkillAndLevel.Count == 3)
                    {
                        Logger.LogInformation(
                            "Level Up Menu - All but one level 10 profession found, selecting remaining profession.");
                        var professionToAdd =
                            professionsToChooseFrom.First(x => !prestigedProfessionsForThisSkillAndLevel.Contains(x));
                        Game1.player.professions.Add(professionToAdd.Id);
                        professionToAdd.SpecialHandling?.ApplyEffect();
                        exitThisMenu(false);
                        Game1.activeClickableMenu = new LevelUpMessageDialogWithProfession(ExtraTallMessageDialogBounds,
                            $"You levelled your {skill.Type.Name} skill to level {_currentLevel} and gained a profession!  {Environment.NewLine} You may now prestige this skill again!",
                            skill, professionToAdd);
                        return;
                    }
                    if (prestigedProfessionsForThisSkillAndLevel.Count < 4) return;
                    Logger.LogInformation("Level Up Menu - All professions already prestiged for this skill.");
                    exitThisMenu(false);
                Game1.activeClickableMenu = new LevelUpMessageDialog(ExtraTallMessageDialogBounds,
                        $"You levelled your {skill.Type.Name} skill to level {_currentLevel}!  {Environment.NewLine} Congratulations! You have prestiged all of your professions and reached level 10 again! You may continue to earn prestige points if you wish, as more prestige options are coming soon!",
                        skill);
            }
        }

        private void DecorateUi(SpriteBatch spriteBatch)
        {
            if (_drawToggleSwitch) _levelTenToggleButton.Draw(spriteBatch);
            DrawPrestigedIndicators(spriteBatch, _drawLeftPrestigedIndicator, _drawRightPrestigedIndicator);

        }

        private void DrawPrestigedIndicators(SpriteBatch spriteBatch, bool left, bool right)
        {
            const string text = "Prestiged!";
            var textPadding = Game1.tileSize;
            var yPositionOfText = yPositionOnScreen + height + textPadding;
            if (left) spriteBatch.DrawString(Game1.dialogueFont, text, new Vector2(xPositionOnScreen + width / 4 - Game1.dialogueFont.MeasureString(text).X / 2, yPositionOfText), Color.LimeGreen);
            if (right) spriteBatch.DrawString(Game1.dialogueFont, text, new Vector2(xPositionOnScreen + width * 3 / 4 - Game1.dialogueFont.MeasureString(text).X / 2, yPositionOfText), Color.LimeGreen);
        }

        private void SetupLevelTenToggleButton()
        {
            if (_levelTenToggleButton != null) return;
            Logger.LogInformation("Level Up Menu - initiating level 10 toggle button...");
            var position = new Vector2(xPositionOnScreen + width + Game1.tileSize, yPositionOnScreen);
            var bounds = new Rectangle((int)Math.Floor(position.X), (int)Math.Floor(position.Y), Game1.tileSize, Game1.tileSize);
            _levelTenToggleButton = new TextureButton(bounds, Game1.mouseCursors, new Rectangle(0, 192, 64, 64), ToggleLevelTenMenu, "More professions...");
            RegisterMouseEvents();
            Logger.LogInformation("Level Up Menu - Level 10 toggle button initiated.");
        }

        private void ToggleLevelTenMenu()
        {
            Logger.LogInformation("Toggling level 10 menu...");
            _isRightSideOfTree = !_isRightSideOfTree;
            var offsetForLevelTenProfessions = 2;
            if (_isRightSideOfTree)
            {
                Logger.LogInformation("Moving to right side of tree...");
                offsetForLevelTenProfessions += 2;
            }
            var professionsToChoose = new List<int> { _currentSkillId * 6 + offsetForLevelTenProfessions, _currentSkillId * 6 + offsetForLevelTenProfessions + 1 };
            this.SetInstanceFieldOfBase("professionsToChoose", professionsToChoose);
            this.SetInstanceFieldOfBase("leftProfessionDescription", getProfessionDescription(professionsToChoose[0]));
            this.SetInstanceFieldOfBase("rightProfessionDescription", getProfessionDescription(professionsToChoose[1]));
            var skill = Skill.AllSkills.Single(x => x.Type.Ordinal == _currentSkillId);
            var prestigeData = PrestigeSaveData.CurrentlyLoadedPrestigeSet.Prestiges.Single(x => x.SkillType == skill.Type);
            var prestigedProfessionsForThisSkillAndLevel = skill.Professions.Where(x => prestigeData.PrestigeProfessionsSelected.Contains(x.Id) && x.LevelAvailableAt == _currentLevel).ToList();
            var professionsToChooseFrom = skill.Professions.Where(x => x.LevelAvailableAt == _currentLevel).ToList();
            _drawLeftPrestigedIndicator = prestigedProfessionsForThisSkillAndLevel.Contains(professionsToChooseFrom.Skip(_isRightSideOfTree == false ? 0 : 2).First());
            _drawRightPrestigedIndicator = prestigedProfessionsForThisSkillAndLevel.Contains(professionsToChooseFrom.Skip(_isRightSideOfTree == false ? 1 : 3).First());
        }

    }
}
