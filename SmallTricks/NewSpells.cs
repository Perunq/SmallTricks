using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using System;
using static Kingmaker.UnitLogic.Commands.Base.UnitCommand;

namespace SmallTricks
{
    class NewSpells
    {


        public static BlueprintAbility RitualLvl1;
        public static BlueprintAbility SumRit1Lvl1;
        public static BlueprintSummonPoolReference monsterpoolref;
        public static BlueprintBuff SumRitBuff;
        public static ContextActionSpawnMonster comp1;
        public static ContextActionSpawnMonster comp2;
        public static BlueprintAbility SumRit2Lvl1;
        public static BlueprintAbility SumRit3Lvl1;
        public static BlueprintAbility SummonLeopard;

        public BlueprintAbility SummonSpell { get; private set; }

        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool Initialized;

            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;

                Main.LogHeader("Patching Spells");
                try
                {

                    CreateRitualLvl1();

                }
                catch (Exception e)
                {

                    Main.Error(e, "Blah");
                }

                Main.LogHeader("Feats Patched");
            }


        }
        /*

        [HarmonyPatch(typeof(AbilityEffectRunAction), "Init")]
        static class SpellComponent_Init_Patch
        {
            

            AbilityEffectRunAction static void Postfix( AbilityEffectRunAction __instance, ref bool __result)
            {
                
                

                Main.LogHeader("Patching Summon Spells");
                try
                {       
                    
                    if(__instance.)

                    

                }
                catch (Exception e)
                {

                    Main.Error(e, "Blah");
                }

                Main.LogHeader("Feats Patched");
            }


        }*/


        private static void CreateRitualLvl1()
        {
            var genericsummonability = Resources.GetBlueprint<BlueprintAbility>("46ef10fe0290367448a037b9d533ce0a"); //inqui
            var monsterpool = Resources.GetBlueprint<BlueprintSummonPool>("d94c93e7240f10e41ae41db4c83d1cbe");
            SummonLeopard = Resources.GetBlueprint<BlueprintAbility>("b1326a7a72fae4c4996339e14715c08d");
            var Monster1 = Resources.GetBlueprint<BlueprintUnit>("2d643696b0653b846bf95c62be792e85");
            var Monster2 = Resources.GetBlueprint<BlueprintUnit>("9125fbea5b02f4744a169091dec729b2");
            monsterpoolref = monsterpool.ToReference<BlueprintSummonPoolReference>();
            RitualLvl1 = Helpers.CreateBlueprint<BlueprintAbility>("RitualLvl1", Bp =>
            {

                Bp.name = "SummonBachors - Perunq";



            });
            SumRitBuff = Helpers.CreateBlueprint<BlueprintBuff>("SumRitBuff", Bp =>
            {

                Bp.name = "SummonBachorsBuff - Perunq";



            });




            createDemonicRitualSpell11();
            createDemonicRitualSpell12();
        }

        static void createDemonicRitualSpell11()
        {
            var component = SummonLeopard.GetComponent<AbilityEffectRunAction>();
            comp1 = new ContextActionSpawnMonster();
            comp1 = (ContextActionSpawnMonster)component.Actions.Actions[0];
            comp1.DurationValue = new ContextDurationValue()
            {
                Rate = DurationRate.Minutes,
                BonusValue = new ContextValue()
                {
                    ValueType = ContextValueType.Rank
                },
                DiceCountValue = new ContextValue(),
                DiceType = DiceType.One
            };

            var Monster = Resources.GetBlueprint<BlueprintUnit>("2d643696b0653b846bf95c62be792e85");
            comp1.m_Blueprint = Monster.ToReference<BlueprintUnitReference>();

            SumRit1Lvl1 = Helpers.CreateBlueprint<BlueprintAbility>("SumRit1Lvl1", Bp =>
            {

                Bp.SetName("Summon Dretch - Perunq");
                Bp.SetDescription("You summon a Dretch.");
                Bp.LocalizedDuration = Helpers.CreateString("SumRit1Lvl1.Duration", "1 minute/level");
                Bp.Parent = RitualLvl1;
                Bp.m_IsFullRoundAction = true;
                Bp.Range = AbilityRange.Close;
                Bp.CanTargetPoint = true;
                Bp.CanTargetSelf = true;
                Bp.ActionType = CommandType.Standard;
                Bp.m_Description = Helpers.CreateString("RitualDescription", "RitualSpell");
                Bp.AddComponent(Helpers.Create<AbilityEffectRunAction>(a => { a.Actions = Helpers.CreateActionList(comp1); }));
                Bp.AddComponent(Helpers.Create<ContextRankConfig>(z => { z.m_Max = 20; }));
            });
        }

        static void createDemonicRitualSpell12()
        {
            var component = SummonLeopard.GetComponent<AbilityEffectRunAction>();
            comp2 = new ContextActionSpawnMonster();
            comp2 = (ContextActionSpawnMonster)component.Actions.Actions[0];
            comp2.DurationValue = new ContextDurationValue()
            {
                Rate = DurationRate.Minutes,
                BonusValue = new ContextValue()
                {
                    ValueType = ContextValueType.Rank
                },
                DiceCountValue = new ContextValue(),
                DiceType = DiceType.One
            };

            var Monster = Resources.GetBlueprint<BlueprintUnit>("9125fbea5b02f4744a169091dec729b2");
            comp2.m_Blueprint = Monster.ToReference<BlueprintUnitReference>();

            SumRit2Lvl1 = Helpers.CreateBlueprint<BlueprintAbility>("SumRit2Lvl1", Bp =>
            {

                Bp.SetName("Summon Cambion - Perunq");
                Bp.SetDescription("You summon a Cambion. A very cambion cambion. Uuu.");
                Bp.LocalizedDuration = Helpers.CreateString("SumRit1Lvl1.Duration", "1 minute/level");
                Bp.Parent = RitualLvl1;
                Bp.m_IsFullRoundAction = true;
                Bp.Range = AbilityRange.Close;
                Bp.CanTargetPoint = true;
                Bp.CanTargetSelf = true;
                Bp.ActionType = CommandType.Standard;
                Bp.m_Description = Helpers.CreateString("RitualDescription", "RitualSpell");
                Bp.AddComponent(Helpers.Create<AbilityEffectRunAction>(a => { a.Actions = Helpers.CreateActionList(comp2); }));
                Bp.AddComponent(Helpers.Create<ContextRankConfig>(z => { z.m_Max = 20; }));
            });
        }
    }
}    