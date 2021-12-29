using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.FactLogic;
using System;
using static SmallTricks.TTTUtilities;

namespace SmallTricks
{
    class NewFeats
    {
        static public BlueprintFeature shield_brace;
        static public BlueprintFeature unhindering_shield;
        static public BlueprintFeature jouster;
        static public BlueprintFeature backgroundJouster;
        static public BlueprintUnit CohortUnit;
        static public BlueprintFeature LeadershipFeature;


        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool Initialized;

            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;

                Main.LogHeader("Patching Feats");
                try
                {

                    CreateShieldBrace();
                    CreateUnhinderingShield();
                    CreateJouster();
                    NewBackgrounds.CreateJousterBackground();
                    Leadership.CreateLeadershipFeature();
                }
                catch (Exception e)
                {

                    Main.Error(e, "Blah");
                }

                Main.LogHeader("Feats Patched");
            }

        }


        static void CreateShieldBrace()
        {
            var fighter = Resources.GetBlueprint<BlueprintCharacterClass>("48ac8db94d5de7645906c7d0ad3bcfbd");
            var shield_focus = Resources.GetBlueprint<BlueprintFeature>("ac57069b6bf8c904086171683992a92a");
            //var armor_training = Resources.GetBlueprint<BlueprintFeature>("");


            shield_brace = Helpers.CreateBlueprint<BlueprintFeature>("ShieldBraceFeature", bp =>
            {
                bp.SetName("Shield Brace");
                bp.SetDescription("You can use a two-handed weapon sized appropriately for you from the polearm or spears weapon group" +
                    " while also using a light, heavy, or tower shield with which you are proficient. The shield’s armor check penalty " +
                    "(if any) applies to attacks made with the weapon.");
                bp.Ranks = 1;
                bp.ReapplyOnLevelUp = true;
                bp.IsClassFeature = true;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Feat, FeatureGroup.CombatFeat };
                bp.AddPrerequisiteFeature(shield_focus);
                bp.AddPrerequisite<PrerequisiteStatValue>(c =>
                {
                    c.Stat = StatType.BaseAttackBonus;
                    c.Value = 3;
                });
                bp.AddPrerequisite<PrerequisiteClassLevel>(c =>
                {
                    c.m_CharacterClass = fighter.ToReference<BlueprintCharacterClassReference>();
                    c.Level = 1;
                });
                bp.AddComponent(Helpers.Create<HandHolding.ShieldBrace>());
            });
            FeatTools.AddAsFeat(shield_brace);
        }

        static void CreateUnhinderingShield()
        {
            var fighter = Resources.GetBlueprint<BlueprintCharacterClass>("48ac8db94d5de7645906c7d0ad3bcfbd");
            var shield_focus = Resources.GetBlueprint<BlueprintFeature>("ac57069b6bf8c904086171683992a92a");
            //var armor_training = Resources.GetBlueprint<BlueprintFeature>("");

            unhindering_shield = Helpers.CreateBlueprint<BlueprintFeature>("UnhinderingShieldFeature", bp =>
            {
                bp.SetName("Unhindering Shield");
                bp.SetDescription("You still gain a buckler’s bonus to AC even if you use your shield hand for some other purpose." +
                " When you wield a buckler, your shield hand is considered free for the purposes of casting spells, wielding two -" +
                " handed weapons, and using any other abilities that require you to have a free hand or interact with your shield," +
                " such as the swashbuckler’s precise strike deed or the Weapon Finesse feat.");
                bp.Ranks = 1;
                bp.ReapplyOnLevelUp = true;
                bp.IsClassFeature = true;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Feat, FeatureGroup.CombatFeat };
                bp.AddPrerequisiteFeature(shield_focus);
                bp.AddPrerequisite<PrerequisiteStatValue>(c =>
                {
                    c.Stat = StatType.BaseAttackBonus;
                    c.Value = 6;
                });
                bp.AddPrerequisite<PrerequisiteClassLevel>(c =>
                {
                    c.m_CharacterClass = fighter.ToReference<BlueprintCharacterClassReference>();
                    c.Level = 4;
                });
                bp.AddComponent(Helpers.Create<HandHolding.UnhinderingShield>());
            });
            FeatTools.AddAsFeat(unhindering_shield);

        }


        static void CreateJouster()
        {



            jouster = Helpers.CreateBlueprint<BlueprintFeature>("ShieldJousterFeature", bp =>
            {
                bp.SetName("Jouster");
                bp.SetDescription("You can use a longspear sized appropriately for you" +
                    " while also using a light, heavy, or tower shield with which you are proficient.");
                bp.Ranks = 1;
                bp.ReapplyOnLevelUp = true;
                bp.IsClassFeature = true;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Feat, FeatureGroup.CombatFeat };
                bp.AddComponent(Helpers.Create<HandHolding.Jouster>());
            });
            //FeatTools.AddAsFeat(shield_brace);
        }


        class NewBackgrounds
        {
            public static void CreateJousterBackground()
            {

                //var AOCIcon = AssetLoader.LoadInternal("Backgrounds", "Icon_AOC.png"
                var BackgroundNobleSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("7b11f589e81617a46b3e5eda3632508d");
                backgroundJouster = Helpers.CreateBlueprint<BlueprintFeature>("BackgroundJousterFeature", bp =>
                {
                    bp.SetName("Jouster");
                    bp.SetDescription("Jouster adds {g|Encyclopedia:Mobility}Mobility{/g} to the list of her class {g|Encyclopedia:Skills}" +
                        " skills{/g}. She also becomes proficient with longspears and can use them with shields.\nIf the" +
                        " character already has the class skill," +
                        " {g|Encyclopedia:Weapon_Proficiency}weapon proficiency {/g}or armor proficiency granted by the selected background" +
                        " from her class during character creation, then the corresponding {g|Encyclopedia:Bonus}bonuses{/g} from background change" +
                        " to a + 1 competence bonus in case of skills, a +1 enhancement bonus in case of weapon proficiency and a -1 Armor" +
                        " {g|Encyclopedia:Check}Check {/g}{g|Encyclopedia:Penalty}Penalty{/g}reduction in case of armor proficiency."



                        );
                    //bp.SetDescription("Jouster is proficient and can use longspears in one hand with shield. ");
                    //bp.m_Icon = AOCIcon;
                    bp.Ranks = 1;
                    bp.IsClassFeature = true;
                    bp.m_DescriptionShort = Helpers.CreateString("$BackgroundJouster.DescriptionShort", "Jouster is proficient with longspears.Better able to make mobility checks and can use longspears in one hand.");
                    bp.AddComponent<AddClassSkill>(c =>
                    {
                        c.Skill = Kingmaker.EntitySystem.Stats.StatType.SkillMobility;
                    });
                    bp.AddComponent<AddBackgroundClassSkill>(c =>
                    {
                        c.Skill = Kingmaker.EntitySystem.Stats.StatType.SkillMobility;
                    });/*
                    bp.AddComponent<AddStatBonus>(c =>
                    {
                        c.Descriptor = Kingmaker.Enums.ModifierDescriptor.UntypedStackable;
                        c.Stat = Kingmaker.EntitySystem.Stats.StatType.SkillMobility;
                        c.Value = 1;
                    });*/
                    bp.AddComponent(Helpers.Create<HandHolding.Jouster>());
                    var LsProficiency = Resources.GetBlueprint<BlueprintFeature>("9c7b33404dc0ac1449ee2732657b722a");
                    bp.AddComponent<AddBackgroundWeaponProficiency>(c =>
                    {
                        c.Proficiency = WeaponCategory.Longspear;
                        c.StackBonusType = ModifierDescriptor.Enhancement;
                        c.StackBonus = 1;
                    });

                    bp.AddComponent<AddFacts>(c =>
                    {
                        c.m_Facts = new BlueprintUnitFactReference[]
                        {
                            LsProficiency.ToReference<BlueprintUnitFactReference>()

                        };
                    });
                    //addFacts.m_Facts = addFacts.m_Facts.AppendToArray(LsProficiency.ToReference<BlueprintUnitFactReference>());

                });
                BackgroundNobleSelection.m_AllFeatures = BackgroundNobleSelection.m_AllFeatures.AppendToArray(backgroundJouster.ToReference<BlueprintFeatureReference>());
                Main.LogPatch("Patched", backgroundJouster);
            }
        }

        public class Leadership
        {


            public static void CreateLeadershipFeature()

            {
                Main.LogHeader("Creating Leadership Feature");
                var defaulthuman = Resources.GetBlueprint<BlueprintUnit>("07381437a02832f41882bb55701b40d3");
                //var defaulthuman = Helpers.Create<BlueprintUnit>(a => { a = defaulthumanpreset; });

                //CohortUnit = Helpers.CreateBlueprint<BlueprintUnit>(bp => { bp = defaulthuman; }) ;
                LeadershipFeature = Helpers.CreateBlueprint<BlueprintFeature>("LeadershipFeature", bp =>
                {
                    bp.SetName("Leadership");
                    bp.SetDescription("You attract followers to your cause and a companion to join you on your adventures.");
                    //bp.m_Icon = HavocIcon;
                    bp.Ranks = 1;
                    bp.Groups = new FeatureGroup[]
                    {
                        FeatureGroup.AnimalCompanion
                    };
                    bp.ReapplyOnLevelUp = true;
                    bp.IsClassFeature = true;
                    bp.AddComponent<SmallTricks.Wrath.AddCohort>(c =>
                    {
                        c.m_Cohort = defaulthuman.ToReference<BlueprintUnitReference>();
                    });
                });
                Main.LogHeader("Leadership Feature Created");
            }
        }
    }
}