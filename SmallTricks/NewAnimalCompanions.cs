using HarmonyLib;
using Kingmaker.AI.Blueprints;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.View;
using System;

namespace SmallTricks
{
    class NewAnimalCompanions
    {
        public static readonly BlueprintFeature AnimalCompanionRank = Resources.GetBlueprint<BlueprintFeature>("1670990255e4fe948a863bafd5dbda5d");
        static public BlueprintFeature FrogPetUpgradeFeature;
        static public BlueprintUnit FrogUnit;
        static public BlueprintFeature FrogPetFeature;
        static public BlueprintUnit Frog2Unit;
        static public BlueprintFeature Frog2PetFeature;
        static public BlueprintUnit OwlbearUnit;
        static public BlueprintFeature OwlbearPetFeature;


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

                    CreateACFrog();
                    CreateACOwlbear();

                }
                catch (Exception e)
                {

                    Main.Error(e, "Blah");
                }

                Main.LogHeader("Feats Patched");
            }

        }


        static void CreateACFrog()
        {
            //Frog unit bd241d52f6622e44b8775a34b7027f8f
            var UnitDog = Resources.GetBlueprint<BlueprintUnit>("918939943bf32ba4a95470ea696c2ba5");
            var DragonAzataCompanionFeature = Resources.GetBlueprint<BlueprintFeature>("cf36f23d60987224696f03be70351928");
            //var TrueFrogUnit = Resources.GetBlueprint<BlueprintUnit>("bd241d52f6622e44b8775a34b7027f8f");
            var DragonAzataCompanionRank = Resources.GetBlueprint<BlueprintFeature>("2780764bf33c46745b11f0e1d2d20092");

            var SwallowWhole = Resources.GetBlueprint<BlueprintFeature>("dee864aec4a0d344b913dd27a4b504cb");



            //var HavocIcon = AssetLoader.LoadInternal("Skills", "Icon_Havoc.png");
            FrogPetUpgradeFeature = Helpers.CreateBlueprint<BlueprintFeature>("FrogPetUpgradeFeature", bp =>
            {
                bp.SetName("Frog - Upgrade ");
                bp.SetDescription(" ");
                //bp.m_Icon = HavocIcon;
                bp.Ranks = 1;
                bp.ReapplyOnLevelUp = true;
                bp.IsClassFeature = true;
                bp.AddComponent<PrerequisiteCharacterLevel>(a => { a.Level = 4; });
                bp.AddComponent<AddStatBonus>(a =>
                {
                    a.Stat = StatType.Strength;
                    a.Value = 2;
                });
                bp.AddComponent<AddStatBonus>(a =>
                {
                    a.Stat = StatType.Dexterity;
                    a.Value = 2;
                });


            });
            Main.LogHeader("Frog Pet Upgrade Feature Created");


            var CharacterBrain = Resources.GetBlueprint<BlueprintBrain>("cf986dd7ba9d4ec46ad8a3a0406d02ae");

            var FrogNPC = Resources.GetBlueprint<BlueprintUnit>("f834867d15633294ea70579f3616af21");
            //CR1_GiantFrogStandard.0534fa89c1027064eb0f42e3102c6855
            var FrogNPC2 = Resources.GetBlueprint<BlueprintUnit>("0534fa89c1027064eb0f42e3102c6855");


            FrogUnit = Helpers.CreateBlueprint<BlueprintUnit>("FrogUnit", bp =>
             {
                 //bp.m_Portrait = UnitDog.m_Portrait;
                 bp.MaxHP = UnitDog.MaxHP;
                 bp.PortraitSafe = FrogNPC.PortraitSafe;
                 bp.m_Faction = UnitDog.m_Faction;
                 bp.m_Brain = UnitDog.m_Brain;
                 bp.m_AddFacts = UnitDog.m_AddFacts;
                 bp.Gender = UnitDog.Gender;
                 bp.LocalizedName = FrogNPC.LocalizedName;
                 bp.Size = Kingmaker.Enums.Size.Medium;
                 //bp.Color = UnitDog.Color;
                 bp.Alignment = Kingmaker.Enums.Alignment.TrueNeutral;
                 bp.Prefab = FrogNPC.Prefab;
                 bp.Visual = FrogNPC.Visual;
                 bp.FactionOverrides = UnitDog.FactionOverrides;
                 bp.Body = FrogNPC.Body;
                 bp.Strength = 15;
                 bp.Dexterity = 13;
                 bp.Constitution = 16;
                 bp.Intelligence = 1;
                 bp.Wisdom = 9;
                 bp.Charisma = 6;
                 bp.Speed = UnitDog.Speed;
                 bp.Speed.m_Value = 30;
                 bp.Skills = UnitDog.Skills;
                 bp.m_DisplayName = Helpers.CreateString("$FrogUnit.Name", "Frog");
                 bp.m_Description = Helpers.CreateString("$FrogUnit.Description", "");
                 bp.m_DescriptionShort = Helpers.CreateString("$FrogUnit.DescriptionShort", "");
                 bp.ComponentsArray = UnitDog.ComponentsArray;
                 bp.AddComponent<AddFacts>(c =>
                 {
                     c.m_Facts = new BlueprintUnitFactReference[]
{
                            FrogPetUpgradeFeature.ToReference<BlueprintUnitFactReference>(),


};
                 });

             });

            Main.LogHeader("Frog Unit Created");

            Frog2Unit = Helpers.CreateBlueprint<BlueprintUnit>("Frog2Unit", bp =>
            {

                bp.MaxHP = UnitDog.MaxHP;
                //bp.m_Portrait = UnitDog.m_Portrait;
                bp.PortraitSafe = FrogNPC2.PortraitSafe;
                bp.m_Faction = UnitDog.m_Faction;
                bp.m_Brain = UnitDog.m_Brain;
                bp.m_AddFacts = UnitDog.m_AddFacts;
                bp.Gender = UnitDog.Gender;
                bp.LocalizedName = FrogNPC.LocalizedName;
                bp.Size = Kingmaker.Enums.Size.Medium;
                //bp.Color = UnitDog.Color;
                bp.Alignment = Kingmaker.Enums.Alignment.TrueNeutral;
                bp.FactionOverrides = UnitDog.FactionOverrides;
                bp.Strength = 15;
                bp.Dexterity = 13;
                bp.Constitution = 16;
                bp.Intelligence = 1;
                bp.Wisdom = 9;
                bp.Charisma = 6;
                bp.Speed = UnitDog.Speed;
                bp.Speed.m_Value = 30;
                bp.Skills = UnitDog.Skills;
                bp.m_DisplayName = Helpers.CreateString("$FrogUnit.Name", "Frog");
                bp.m_Description = Helpers.CreateString("$FrogUnit.Description", "");
                bp.m_DescriptionShort = Helpers.CreateString("$FrogUnit.DescriptionShort", "");
                bp.ComponentsArray = UnitDog.ComponentsArray;
                bp.AddComponent<AddFacts>(c =>
                {
                    c.m_Facts = new BlueprintUnitFactReference[]
{
                            FrogPetUpgradeFeature.ToReference<BlueprintUnitFactReference>(),


};
                });
                bp.Prefab = FrogNPC2.Prefab;
                bp.Visual = FrogNPC2.Visual;
                bp.Body = FrogNPC2.Body;


            });

            Main.LogHeader("Frog Unit 2 Created");
            /*
            string minppath = @"\Assets\Portraits\Frog\Small.png";
            string majppath = @"\Assets\Portraits\Frog\Medium.png";
            NewPortraitsPatch.CreatePortrait(minppath, majppath, FrogUnit);
            NewPortraitsPatch.CreatePortrait(minppath, majppath, Frog2Unit);
            */



            //var HavocIcon = AssetLoader.LoadInternal("Skills", "Icon_Havoc.png");
            FrogPetFeature = Helpers.CreateBlueprint<BlueprintFeature>("FrogPetFeature", bp =>
            {
                bp.SetName("Frog Companion - Yellow");
                bp.SetDescription("This large dragon’s scales and insectile wings dance with color, while its whiplike tail waves as if stirred by an unseen breeze. " +
                "\nDespite their best intentions, the appropriately named havoc dragons often cause collateral damage as they develop whimsical wonderlands of revelry and relaxation.");
                bp.m_DescriptionShort = Helpers.CreateString("HavocDragonPet.DescriptionShort", "This dragon’s scales and insectile wings dance with color, while its whiplike tail waves as if stirred by an unseen breeze. " +
                "\nDespite their best intentions, the appropriately named havoc dragons often cause collateral damage as they develop whimsical wonderlands of revelry and relaxation.");
                //bp.m_Icon = HavocIcon;
                bp.Ranks = 1;
                bp.Groups = new FeatureGroup[] { FeatureGroup.AnimalCompanion };
                bp.ReapplyOnLevelUp = true;
                bp.IsClassFeature = true;
                bp.AddComponent<AddPet>(c =>
                {
                    c.m_Pet = FrogUnit.ToReference<BlueprintUnitReference>();
                    c.m_LevelRank = AnimalCompanionRank.ToReference<BlueprintFeatureReference>();
                });
                bp.AddComponent<PrerequisitePet>(c => { c.NoCompanion = true; });
            });
            Main.LogHeader("Frog Pet Feature Created");

            Frog2PetFeature = Helpers.CreateBlueprint<BlueprintFeature>("Frog2PetFeature", bp =>
            {
                bp.SetName("Frog Companion - Green");
                bp.SetDescription("This large dragon’s scales and insectile wings dance with color, while its whiplike tail waves as if stirred by an unseen breeze. " +
                "\nDespite their best intentions, the appropriately named havoc dragons often cause collateral damage as they develop whimsical wonderlands of revelry and relaxation.");
                bp.m_DescriptionShort = Helpers.CreateString("HavocDragonPet.DescriptionShort", "This dragon’s scales and insectile wings dance with color, while its whiplike tail waves as if stirred by an unseen breeze. " +
                "\nDespite their best intentions, the appropriately named havoc dragons often cause collateral damage as they develop whimsical wonderlands of revelry and relaxation.");
                //bp.m_Icon = HavocIcon;
                bp.Ranks = 1;
                bp.Groups = new FeatureGroup[] { FeatureGroup.AnimalCompanion };
                bp.ReapplyOnLevelUp = true;
                bp.IsClassFeature = true;
                bp.AddComponent<AddPet>(c =>
                {
                    c.m_Pet = Frog2Unit.ToReference<BlueprintUnitReference>();
                    c.m_LevelRank = AnimalCompanionRank.ToReference<BlueprintFeatureReference>();
                });
                bp.AddComponent<PrerequisitePet>(c => { c.NoCompanion = true; });
            });
            Main.LogHeader("Frog2 Pet Feature Created");

            AddAsAc(FrogPetFeature);
            AddAsAc(Frog2PetFeature);
        }

        static void CreateACOwlbear()
        {
            //Frog unit bd241d52f6622e44b8775a34b7027f8f
            var UnitDog = Resources.GetBlueprint<BlueprintUnit>("918939943bf32ba4a95470ea696c2ba5");
            var DragonAzataCompanionFeature = Resources.GetBlueprint<BlueprintFeature>("cf36f23d60987224696f03be70351928");
            //var TrueFrogUnit = Resources.GetBlueprint<BlueprintUnit>("bd241d52f6622e44b8775a34b7027f8f");
            var DragonAzataCompanionRank = Resources.GetBlueprint<BlueprintFeature>("2780764bf33c46745b11f0e1d2d20092");



            var CharacterBrain = Resources.GetBlueprint<BlueprintBrain>("cf986dd7ba9d4ec46ad8a3a0406d02ae");

            var OwlbearNPC = Resources.GetBlueprint<BlueprintUnit>("d42bbb8a2c328874683048849412aa80");
            //OwlbearNPC.Size = Kingmaker.Enums.Size.Diminutive;
            var OwlbearAdvanced = Resources.GetBlueprint<BlueprintUnit>("7d3bd11169778c845b2631d22d27d465");

            var prefabOwlbear = OwlbearNPC.Prefab;
            UnitEntityView view = prefabOwlbear.Load();
            view.GetSizeScale();


            OwlbearUnit = Helpers.CreateBlueprint<BlueprintUnit>("OwlbearUnit", bp =>
            {
                bp.m_Portrait = UnitDog.m_Portrait;
                bp.m_Faction = UnitDog.m_Faction;

                bp.m_Brain = UnitDog.m_Brain;
                bp.m_AddFacts = UnitDog.m_AddFacts;
                bp.Gender = UnitDog.Gender;
                bp.LocalizedName = UnitDog.LocalizedName;
                bp.Size = Kingmaker.Enums.Size.Medium;
                bp.Color = UnitDog.Color;
                bp.Alignment = Kingmaker.Enums.Alignment.TrueNeutral;
                bp.Prefab = OwlbearNPC.Prefab;
                bp.Visual = OwlbearNPC.Visual;
                bp.FactionOverrides = UnitDog.FactionOverrides;
                bp.Body = OwlbearNPC.Body;
                bp.Strength = UnitDog.Strength;
                bp.Dexterity = UnitDog.Dexterity;
                bp.Constitution = UnitDog.Constitution;
                bp.Intelligence = UnitDog.Intelligence;
                bp.Wisdom = UnitDog.Wisdom;
                bp.Charisma = UnitDog.Charisma;
                bp.Speed = UnitDog.Speed;
                bp.Skills = UnitDog.Skills;
                bp.m_DisplayName = UnitDog.m_DisplayName;
                bp.m_Description = UnitDog.m_Description;
                bp.m_DescriptionShort = UnitDog.m_DescriptionShort;
                bp.ComponentsArray = UnitDog.ComponentsArray;
            });

            Main.LogHeader("Owlbear Unit Created");


            //var HavocIcon = AssetLoader.LoadInternal("Skills", "Icon_Havoc.png");
            OwlbearPetFeature = Helpers.CreateBlueprint<BlueprintFeature>("OwlbearPetFeature", bp =>
            {
                bp.SetName("Owlbear Companion - Large");
                bp.SetDescription("This large dragon’s scales and insectile wings dance with color, while its whiplike tail waves as if stirred by an unseen breeze. " +
                "\nDespite their best intentions, the appropriately named havoc dragons often cause collateral damage as they develop whimsical wonderlands of revelry and relaxation.");
                bp.m_DescriptionShort = Helpers.CreateString("HavocDragonPet.DescriptionShort", "This dragon’s scales and insectile wings dance with color, while its whiplike tail waves as if stirred by an unseen breeze. " +
                "\nDespite their best intentions, the appropriately named havoc dragons often cause collateral damage as they develop whimsical wonderlands of revelry and relaxation.");
                //bp.m_Icon = HavocIcon;
                bp.Ranks = 1;
                bp.Groups = new FeatureGroup[] { FeatureGroup.AnimalCompanion };
                bp.ReapplyOnLevelUp = true;
                bp.IsClassFeature = true;
                bp.AddComponent<AddPet>(c =>
                {
                    c.m_Pet = OwlbearUnit.ToReference<BlueprintUnitReference>();
                    c.m_LevelRank = AnimalCompanionRank.ToReference<BlueprintFeatureReference>();
                });
                bp.AddComponent<PrerequisitePet>(c => { c.NoCompanion = true; });
            });
            Main.LogHeader("Owlbear Pet Feature Created");


            AddAsAc(OwlbearPetFeature);
        }



        static void AddAsAc(BlueprintFeature PetFeature)
        {
            var AnimalCompanionSelectionBase = Resources.GetBlueprint<BlueprintFeatureSelection>("90406c575576aee40a34917a1b429254");
            var AnimalCompanionSelectionDomain = Resources.GetBlueprint<BlueprintFeatureSelection>("2ecd6c64683b59944a7fe544033bb533");
            var AnimalCompanionSelectionDruid = Resources.GetBlueprint<BlueprintFeatureSelection>("571f8434d98560c43935e132df65fe76");
            var AnimalCompanionSelectionHunter = Resources.GetBlueprint<BlueprintFeatureSelection>("715ac15eb8bd5e342bc8a0a3c9e3e38f");
            var AnimalCompanionSelectionMadDog = Resources.GetBlueprint<BlueprintFeatureSelection>("738b59d0b58187f4d846b0caaf0f80d7");
            var AnimalCompanionSelectionRanger = Resources.GetBlueprint<BlueprintFeatureSelection>("ee63330662126374e8785cc901941ac7");
            var AnimalCompanionSelectionSacredHuntsmaster = Resources.GetBlueprint<BlueprintFeatureSelection>("2995b36659b9ad3408fd26f137ee2c67");
            var AnimalCompanionSelectionSylvanSorcerer = Resources.GetBlueprint<BlueprintFeatureSelection>("a540d7dfe1e2a174a94198aba037274c");
            var AnimalCompanionSelectionUrbanHunter = Resources.GetBlueprint<BlueprintFeatureSelection>("257375cd139800e459d69ccfe4ca309c");
            var AnimalCompanionSelectionWildlandShaman = Resources.GetBlueprint<BlueprintFeatureSelection>("164c875d6b27483faba479c7f5261915");
            var ArcaneRiderMountSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("82c791d4790c45dcac6038ef6932c3e3");
            var BeastRiderMountSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("f7da602dae8944d499f00074c973c28a");
            var BloodriderMountSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("c81bb2aa48c113c4ba3ee8582eb058ea");
            var CavalierMountSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("0605927df6e2fdd42af6ee2424eb89f2");
            var NomadMountSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("8e1957da5a8144d1b0fcf8875ac6bab7");
            var OracleRevelationBondedMount = Resources.GetBlueprint<BlueprintFeatureSelection>("0234d0dd1cead22428e71a2500afa2e1");
            var PaladinDivineMountSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("e2f0e0efc9e155e43ba431984429678e");
            var ShamanNatureSpiritTrueSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("e7f4cfcd7488ac14bbc3e09426b59fd0");
            var SoheiMonasticMountHorseSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("918432cc97b146a4b93e2b3060bdd1ed");

            var Animal = PetFeature.ToReference<BlueprintFeatureReference>();

            AnimalCompanionSelectionBase.m_AllFeatures = AnimalCompanionSelectionBase.m_AllFeatures.AppendToArray(Animal);
            AnimalCompanionSelectionDomain.m_AllFeatures = AnimalCompanionSelectionDomain.m_AllFeatures.AppendToArray(Animal);
            AnimalCompanionSelectionDruid.m_AllFeatures = AnimalCompanionSelectionDruid.m_AllFeatures.AppendToArray(Animal);
            AnimalCompanionSelectionHunter.m_AllFeatures = AnimalCompanionSelectionHunter.m_AllFeatures.AppendToArray(Animal);
            AnimalCompanionSelectionMadDog.m_AllFeatures = AnimalCompanionSelectionMadDog.m_AllFeatures.AppendToArray(Animal);
            AnimalCompanionSelectionRanger.m_AllFeatures = AnimalCompanionSelectionRanger.m_AllFeatures.AppendToArray(Animal);
            AnimalCompanionSelectionSacredHuntsmaster.m_AllFeatures = AnimalCompanionSelectionSacredHuntsmaster.m_AllFeatures.AppendToArray(Animal);
            AnimalCompanionSelectionSylvanSorcerer.m_AllFeatures = AnimalCompanionSelectionSylvanSorcerer.m_AllFeatures.AppendToArray(Animal);
            AnimalCompanionSelectionUrbanHunter.m_AllFeatures = AnimalCompanionSelectionUrbanHunter.m_AllFeatures.AppendToArray(Animal);
            AnimalCompanionSelectionWildlandShaman.m_AllFeatures = AnimalCompanionSelectionWildlandShaman.m_AllFeatures.AppendToArray(Animal);
            BeastRiderMountSelection.m_AllFeatures = BeastRiderMountSelection.m_AllFeatures.AppendToArray(Animal);
            BloodriderMountSelection.m_AllFeatures = BloodriderMountSelection.m_AllFeatures.AppendToArray(Animal);
            NomadMountSelection.m_AllFeatures = NomadMountSelection.m_AllFeatures.AppendToArray(Animal);
            OracleRevelationBondedMount.m_AllFeatures = OracleRevelationBondedMount.m_AllFeatures.AppendToArray(Animal);
            PaladinDivineMountSelection.m_AllFeatures = PaladinDivineMountSelection.m_AllFeatures.AppendToArray(Animal);

            Main.LogHeader("Animal Added");
        }


    }
}
