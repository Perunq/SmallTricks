using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem;
using Kingmaker.Enums;
using Kingmaker.Items;
using Kingmaker.Items.Slots;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.Kineticist;
using Kingmaker.UnitLogic.Class.Kineticist.ActivatableAbility;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Utility;
using Kingmaker.View.Animation;
using Kingmaker.View.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using static Kingmaker.Blueprints.Area.FactHolder;

namespace SmallTricks
{
    partial class HandHolding
    {
        [HarmonyPatch(typeof(ItemEntityWeapon))]
        [HarmonyPatch("HoldInTwoHands", HarmonyLib.MethodType.Getter)]
        class ItemEntityWeapon__HoldInTwoHands__Patch
        {
            static void Postfix(ItemEntityWeapon __instance, ref bool __result)
            {
                bool spell_combat = false;
                UnitPartMagus unit_part_magus = __instance.Wielder?.Get<UnitPartMagus>();
                if ((bool)(unit_part_magus) && unit_part_magus.SpellCombat.Active)
                {
                    spell_combat = true;
                }

                if (__instance.Blueprint.IsTwoHanded
                    || (__instance.Blueprint.IsOneHandedWhichCanBeUsedWithTwoHands && __result == false))
                {
                    var unit_part = __instance.Wielder?.Get<UnitPartCanHold2hWeaponIn1h>();
                    if (!spell_combat)
                    {//check if we can hold the 2h weapon in 1h
                        if (unit_part == null)
                        {
                            return;
                        }

                        __result = unit_part.CanBeUsedAs2h(__instance);
                    }
                    else
                    {
                        if (unit_part != null && unit_part.CanBeUsedOn(__instance))
                        {//weapon is being held as one-handed
                            __result = false;
                            return;
                        }
                        //normally we can not 2h with spell combat, so we check only magus specific feature that would allow us
                        var use_spell_combat_part = __instance.Wielder?.Get<UnitPartCanUseSpellCombat>();
                        if (use_spell_combat_part == null)
                        {
                            return;
                        }

                        var pair_slot = (__instance.HoldingSlot as HandSlot)?.PairSlot;
                        __result = use_spell_combat_part.CanBeUsedOn(__instance.HoldingSlot as HandSlot, pair_slot, true);
                    }
                }
            }
        }



        [HarmonyPatch(typeof(HandSlot))]
        [HarmonyPatch("OnItemInserted", HarmonyLib.MethodType.Normal)]
        class HandSlot__OnItemInserted__Patch
        {
            static bool Prefix(HandSlot __instance)
            {

                var unit_part = __instance.Owner?.Get<UnitPartCanHold2hWeaponIn1h>();
                if (unit_part == null)
                {
                    return true;
                }
                HandSlot primaryHand = __instance.HandsEquipmentSet.PrimaryHand;
                ItemEntityWeapon primaryWeapon = primaryHand.MaybeItem as ItemEntityWeapon;
                ItemEntityWeapon shield = __instance.MaybeItem as ItemEntityWeapon;
                if (!__instance.IsPrimaryHand)
                {
                    if (primaryHand.MaybeWeapon.Blueprint.IsTwoHanded && unit_part.CanBeUsedOn(primaryWeapon) && unit_part.IsShield(shield))
                    {
                        __instance.RemoveItem();
                        __instance.PairSlot.InsertItem((ItemEntity)shield);
                    }
                    else
                    {

                    }


                }


                __instance.IsDirty = true;
                /*

                if (!__instance.IsPrimaryHand)
                {
                    HandSlot primaryHand = __instance.HandsEquipmentSet.PrimaryHand;
                    ItemEntityWeapon maybeItem = primaryHand.MaybeItem as ItemEntityWeapon;
                    if (maybeItem != null && ((maybeItem.Blueprint.IsTwoHanded && !unit_part.CanBeUsedOn(maybeItem)) || maybeItem.Blueprint.Double))
                    {
                        primaryHand.RemoveItem();
                    }
                }
                ItemEntityWeapon maybeItem1 = __instance.MaybeItem as ItemEntityWeapon;
                if (maybeItem1 != null && ((maybeItem1.Blueprint.IsTwoHanded && !unit_part.CanBeUsedOn(maybeItem1)) || maybeItem1.Blueprint.Double))
                {
                    if (__instance.IsPrimaryHand)
                    {
                        __instance.PairSlot.RemoveItem();
                    }
                    else
                    {
                        __instance.RemoveItem();
                        __instance.PairSlot.InsertItem((ItemEntity)maybeItem1);
                    }
                }
                __instance.IsDirty = true;
                */
                return false;
            }
        }



        public class UnitPartCanHold2hWeaponIn1h : AdditiveUnitPart
        {
            public bool CanBeUsedOn(ItemEntityWeapon weapon)
            {
                if (buffs.Empty())
                {
                    return false;
                }

                foreach (var b in buffs)
                {
                    bool result = false;
                    b.CallComponents<CanUse2hWeaponAs1hBase>(c => result = c.CanBeUsedOn(weapon));
                    if (result)
                    {
                        return true;
                    }
                }

                return false;
            }


            public bool CanBeUsedAs2h(ItemEntityWeapon weapon)
            {
                bool can_use_at_all = false;
                foreach (var b in buffs)
                {
                    bool can_use = false;
                    bool can_2h = false;
                    b.CallComponents<CanUse2hWeaponAs1hBase>(c => { can_2h = c.CanBeUsedAs2h(weapon); can_use = c.CanBeUsedOn(weapon); });
                    if (can_use && can_2h)
                    {
                        return true;
                    }
                    can_use_at_all = can_use_at_all || can_use;
                }

                if (!can_use_at_all)
                {
                    HandSlot pair_slot = (weapon?.HoldingSlot as HandSlot)?.PairSlot;
                    if (pair_slot != null)
                        return !pair_slot.HasItem;
                    return false;
                }
                else
                {
                    return false;
                }
            }

            internal bool IsShield(ItemEntityWeapon shield)
            {
                HandSlot holding_slot = shield?.HoldingSlot as HandSlot;
                return holding_slot.PairSlot.MaybeShield.ArmorComponent.Blueprint.ProficiencyGroup == Kingmaker.Blueprints.Items.Armors.ArmorProficiencyGroup.Buckler ||
                    holding_slot.PairSlot.MaybeShield.ArmorComponent.Blueprint.ProficiencyGroup == Kingmaker.Blueprints.Items.Armors.ArmorProficiencyGroup.LightShield ||
                    holding_slot.PairSlot.MaybeShield.ArmorComponent.Blueprint.ProficiencyGroup == Kingmaker.Blueprints.Items.Armors.ArmorProficiencyGroup.HeavyShield ||
                    holding_slot.PairSlot.MaybeShield.ArmorComponent.Blueprint.ProficiencyGroup == Kingmaker.Blueprints.Items.Armors.ArmorProficiencyGroup.TowerShield;
            }
        }





        public abstract class CanUse2hWeaponAs1hBase : UnitFactComponentDelegate
        {


            abstract public bool CanBeUsedOn(ItemEntityWeapon weapon);

            abstract public bool CanBeUsedAs2h(ItemEntityWeapon weapon);

            public override void OnActivate()
            {
                this.Owner.Ensure<UnitPartCanHold2hWeaponIn1h>().AddBuff((Fact)(EntityFact)this.Fact);
            }

            public override void OnDeactivate()
            {
                this.Owner.Ensure<UnitPartCanHold2hWeaponIn1h>().RemoveBuff((Fact)(EntityFact)this.Fact);
            }
        }


        [AllowMultipleComponents]
        public class ShieldBrace : CanUse2hWeaponAs1hBase, IInitiatorRulebookHandler<RuleCalculateAttackBonusWithoutTarget>
        {
            public override bool CanBeUsedAs2h(ItemEntityWeapon weapon)
            {
                return false;
            }

            public override bool CanBeUsedOn(ItemEntityWeapon weapon)
            {
                if (!weapon.Blueprint.IsTwoHanded)
                {
                    return false;
                }
                HandSlot holding_slot = weapon?.HoldingSlot as HandSlot;
                if (holding_slot == null)
                {
                    return false;
                }

                if (!holding_slot.PairSlot.HasItem)
                {
                    return false;
                }

                if (holding_slot.PairSlot.MaybeShield == null)
                {
                    return false;
                }

                var shield_proficiency = holding_slot.PairSlot.MaybeShield.ArmorComponent.Blueprint.ProficiencyGroup;

                if (shield_proficiency == Kingmaker.Blueprints.Items.Armors.ArmorProficiencyGroup.Buckler)
                {
                    return false;
                }
                return weapon.Blueprint.FighterGroup == WeaponFighterGroupFlags.Spears || weapon.Blueprint.FighterGroup == WeaponFighterGroupFlags.Polearms;
            }

            public void OnEventAboutToTrigger(RuleCalculateAttackBonusWithoutTarget evt)
            {
                if (evt.Weapon == null)
                {
                    return;
                }

                if (!CanBeUsedOn(evt.Weapon))
                {
                    return;
                }

                var shield = (evt.Weapon?.HoldingSlot as HandSlot)?.PairSlot.MaybeShield;
                int penalty = Rulebook.Trigger<RuleCalculateArmorCheckPenalty>(new RuleCalculateArmorCheckPenalty(evt.Initiator, shield.ArmorComponent)).Result;


                evt.AddModifier(penalty, this.Fact);
            }

            public void OnEventDidTrigger(RuleCalculateAttackBonusWithoutTarget evt)
            {

            }
        }

        public class UnitPartCanUseSpellCombat : AdditiveUnitPart
        {
            public bool CanBeUsedOn(HandSlot primary_hand, HandSlot secondary_hand, bool use_two_handed)
            {
                if (buffs.Empty())
                {
                    return false;
                }

                foreach (var b in buffs)
                {
                    bool result = false;
                    b.CallComponents<CanUseSpellCombatBase>(c => result = c.CanBeUsedOn(primary_hand, secondary_hand, use_two_handed));
                    if (result)
                    {
                        return true;
                    }
                }

                return false;
            }
        }


        public abstract class CanUseSpellCombatBase : UnitFactComponentDelegate
        {
            abstract public bool CanBeUsedOn(HandSlot primary_hand_slot, HandSlot secondary_hand_slot, bool use_two_handed);

            public override void OnActivate()
            {
                this.Owner.Ensure<UnitPartCanUseSpellCombat>().AddBuff(this.Fact);
            }


            public override void OnDeactivate()
            {
                this.Owner.Ensure<UnitPartCanUseSpellCombat>().RemoveBuff(this.Fact);
            }
        }






        [AllowMultipleComponents]
        public class UnhinderingShield : CanUse2hWeaponAs1hBase
        {
            public override bool CanBeUsedAs2h(ItemEntityWeapon weapon)
            {
                return true;
            }


            public override bool CanBeUsedOn(ItemEntityWeapon weapon)
            {
                HandSlot holding_slot = weapon?.HoldingSlot as HandSlot;
                if (holding_slot == null)
                {
                    return false;
                }

                if (!holding_slot.PairSlot.HasItem)
                {
                    return false;
                }

                if (holding_slot.PairSlot.MaybeShield == null)
                {
                    return false;
                }

                return holding_slot.PairSlot.MaybeShield.ArmorComponent.Blueprint.ProficiencyGroup == Kingmaker.Blueprints.Items.Armors.ArmorProficiencyGroup.Buckler;
            }




        }



        public class UseSpellCombatWithShield : CanUseSpellCombatBase
        {
            public override bool CanBeUsedOn(HandSlot primary_hand_slot, HandSlot secondary_hand_slot, bool use_two_handed)
            {
                if (use_two_handed)
                {
                    return false;
                }

                var shield = secondary_hand_slot?.MaybeShield;

                return shield != null;
            }

            public override void OnActivate()
            {
                this.Owner.Ensure<UnitPartCanUseSpellCombat>().AddBuff(this.Fact);
            }


            public override void OnDeactivate()
            {
                this.Owner.Ensure<UnitPartCanUseSpellCombat>().RemoveBuff(this.Fact);
            }
        }


        public class UseSpellCombatWith2ManifestedWeapons : CanUseSpellCombatBase
        {
            public BlueprintWeaponEnchantment required_enchant;
            public override bool CanBeUsedOn(HandSlot primary_hand_slot, HandSlot secondary_hand_slot, bool use_two_handed)
            {
                if (use_two_handed)
                {
                    return false;
                }

                var weapon = primary_hand_slot?.MaybeWeapon;
                var weapon2 = secondary_hand_slot?.MaybeWeapon;
                if (weapon == null || weapon.EnchantmentsCollection == null)
                {
                    return false;
                }

                if (weapon.Blueprint.Double && weapon.EnchantmentsCollection.HasFact(required_enchant))
                {
                    return true;
                }

                if (weapon2 == null || weapon2.EnchantmentsCollection == null)
                {
                    return false;
                }

                return weapon.EnchantmentsCollection.HasFact(required_enchant) && weapon2.EnchantmentsCollection.HasFact(required_enchant);
            }

            public override void OnActivate()
            {
                this.Owner.Ensure<UnitPartCanUseSpellCombat>().AddBuff(this.Fact);
            }


            public override void OnDeactivate()
            {
                this.Owner.Ensure<UnitPartCanUseSpellCombat>().RemoveBuff(this.Fact);
            }
        }


        public class UseSpellCombatWith2hManifestedWeapon : CanUseSpellCombatBase
        {
            public BlueprintWeaponEnchantment required_enchant;
            public override bool CanBeUsedOn(HandSlot primary_hand_slot, HandSlot secondary_hand_slot, bool use_two_handed)
            {
                var weapon = primary_hand_slot?.MaybeWeapon;
                if (weapon == null || weapon.EnchantmentsCollection == null)
                {
                    return false;
                }

                if (secondary_hand_slot != null && !Helpers.hasFreeHand(secondary_hand_slot))
                {
                    return false;
                }

                return weapon.EnchantmentsCollection.HasFact(required_enchant);
            }

            public override void OnActivate()
            {
                this.Owner.Ensure<UnitPartCanUseSpellCombat>().AddBuff(this.Fact);
            }


            public override void OnDeactivate()
            {
                this.Owner.Ensure<UnitPartCanUseSpellCombat>().RemoveBuff(this.Fact);
            }
        }





        [AllowMultipleComponents]
        public class Jouster : CanUse2hWeaponAs1hBase
        {
            public override bool CanBeUsedAs2h(ItemEntityWeapon weapon)
            {
                return false;
            }

            public override bool CanBeUsedOn(ItemEntityWeapon weapon)
            {
                if (!weapon.Blueprint.IsTwoHanded)
                {
                    return false;
                }
                HandSlot holding_slot = weapon?.HoldingSlot as HandSlot;
                if (holding_slot == null)
                {
                    return false;
                }

                if (!holding_slot.PairSlot.HasItem)
                {
                    return false;
                }

                if (holding_slot.PairSlot.MaybeShield == null)
                {
                    return false;
                }

                var shield_proficiency = holding_slot.PairSlot.MaybeShield.ArmorComponent.Blueprint.ProficiencyGroup;

                if (shield_proficiency == Kingmaker.Blueprints.Items.Armors.ArmorProficiencyGroup.Buckler)
                {
                    return false;
                }
                return weapon.Blueprint.Type.m_TypeNameText == "Longspear";
            }
        }



        [AllowMultipleComponents]
        public class CanHoldIn1Hand : CanUse2hWeaponAs1hBase
        {
            public WeaponCategory category;
            public BlueprintWeaponType[] except_types = new BlueprintWeaponType[0];
            public bool require_full_proficiency = false;
            public override bool CanBeUsedAs2h(ItemEntityWeapon weapon)
            {
                HandSlot holding_slot = weapon?.HoldingSlot as HandSlot;
                if (holding_slot == null)
                {
                    return false;
                }

                return Helpers.hasFreeHand(holding_slot.PairSlot);
            }


            public override bool CanBeUsedOn(ItemEntityWeapon weapon)
            {
                if (require_full_proficiency &&
                    !(weapon.Owner?.Get<WeaponsFix.UnitPartFullProficiency>()?.hasFullProficiency(category)).GetValueOrDefault())
                {
                    return false;
                }

                return (weapon.Blueprint?.Category).GetValueOrDefault() == category
                        && !except_types.Contains(weapon?.Blueprint?.Type);
            }
        }





        internal class Helpers
        {
            internal static bool hasFreeHand(HandSlot hand_slot)
            {
                if (hand_slot == null)
                {
                    return true;
                }
                if (!hand_slot.HasItem)
                {
                    return true;
                }

                var shield = hand_slot.MaybeShield;
                if (shield == null)
                {
                    return false;
                }
                return shield.ArmorComponent.Blueprint.ProficiencyGroup == Kingmaker.Blueprints.Items.Armors.ArmorProficiencyGroup.Buckler && hand_slot.Owner.HasFact(NewFeats.unhindering_shield);
            }


            internal static bool hasFreeHandOrBuckler(HandSlot hand_slot)
            {
                if (hand_slot == null)
                {
                    return true;
                }
                if (!hand_slot.HasItem)
                {
                    return true;
                }

                var shield = hand_slot.MaybeShield;
                if (shield == null)
                {
                    return false;
                }
                return shield.ArmorComponent.Blueprint.ProficiencyGroup == Kingmaker.Blueprints.Items.Armors.ArmorProficiencyGroup.Buckler;
            }


            internal static bool hasFreeHandOr1hWeapon(HandSlot hand_slot)
            {
                if (hasFreeHand(hand_slot))
                {
                    return true;
                }

                var weapon = hand_slot.Weapon;
                if (weapon == null)
                {
                    return true;
                }
                return !weapon.Blueprint.IsTwoHanded && (!weapon.Blueprint.Double);
            }


            internal static bool has2hWeapon(HandSlot hand_slot)
            {
                if (hand_slot == null)
                {
                    return false;
                }
                if (!hand_slot.HasItem)
                {
                    return false;
                }

                var weapon = hand_slot.MaybeWeapon;
                if (weapon == null)
                {
                    return false;
                }

                if (weapon.Blueprint.IsTwoHanded)
                {
                    return true;
                }

                return weapon.Blueprint.Double;
            }

            internal static bool hasShield2(HandSlot hand_slot)
            {
                if (!hand_slot.HasItem)
                {
                    return false;
                }

                var shield = hand_slot.MaybeShield;
                if (shield == null)
                {
                    return false;
                }

                return !(shield.ArmorComponent.Blueprint.ProficiencyGroup == Kingmaker.Blueprints.Items.Armors.ArmorProficiencyGroup.Buckler && hand_slot.Owner.HasFact(NewFeats.unhindering_shield));
            }
        }







        [HarmonyPatch(typeof(UnitPartMagus))]
        [HarmonyPatch("HasOneHandedMeleeWeaponAndFreehand", HarmonyLib.MethodType.Normal)]
        class UnitPartMagus__HasOneHandedMeleeWeaponAndFreehand__Patch
        {
            static void Postfix(UnitDescriptor unit, ref bool __result)
            {
                if (__result == false && unit.Body.PrimaryHand.MaybeWeapon != null)
                {
                    __result = !unit.Body.PrimaryHand.MaybeWeapon.HoldInTwoHands && Helpers.hasFreeHand(unit.Body.SecondaryHand);
                }
                var use_spell_combat_part = unit.Get<UnitPartCanUseSpellCombat>();
                if (__result == false && use_spell_combat_part != null)
                {
                    __result = use_spell_combat_part.CanBeUsedOn(unit.Body.PrimaryHand, unit.Body.SecondaryHand, false);
                }
            }
        }


        [HarmonyPatch(typeof(MonkNoArmorAndMonkWeaponFeatureUnlock))]
        [HarmonyPatch("CheckEligibility", HarmonyLib.MethodType.Normal)]
        class MonkNoArmorAndMonkWeaponFeatureUnlock__CheckEligibility__Patch
        {
            static IEnumerable<HarmonyLib.CodeInstruction> Transpiler(IEnumerable<HarmonyLib.CodeInstruction> instructions)
            {
                var codes = instructions.ToList();
                var check_shield = codes.FindIndex(x => x.opcode == System.Reflection.Emit.OpCodes.Callvirt && x.operand.ToString().Contains("HasShield"));

                codes[check_shield] = new HarmonyLib.CodeInstruction(System.Reflection.Emit.OpCodes.Call, new Func<HandSlot, bool>(Helpers.hasShield2).Method);
                return codes.AsEnumerable();
            }
        }


        [HarmonyPatch(typeof(MonkNoArmorFeatureUnlock))]
        [HarmonyPatch("CheckEligibility", HarmonyLib.MethodType.Normal)]
        class MonkNoArmorFeatureUnlock__CheckEligibility__Patch
        {
            static bool Prefix(MonkNoArmorFeatureUnlock __instance)
            {
                if (!Helpers.hasShield2(__instance.Owner.Body.SecondaryHand) && (!__instance.Owner.Body.Armor.HasArmor || !__instance.Owner.Body.Armor.Armor.Blueprint.IsArmor))
                {
                    __instance.AddFact();
                }
                else
                    __instance.RemoveFact();

                return false;
            }

            /*static IEnumerable<Harmony12.CodeInstruction> Transpiler(IEnumerable<Harmony12.CodeInstruction> instructions)
            {
                var codes = instructions.ToList();
                var check_shield = codes.FindIndex(x => x.opcode == System.Reflection.Emit.OpCodes.Callvirt && x.operand.ToString().Contains("HasShield"));
                codes[check_shield] = new Harmony12.CodeInstruction(System.Reflection.Emit.OpCodes.Call, new Func<HandSlot, bool>(Helpers.hasShield2).Method);
                return codes.AsEnumerable();
            }*/
        }


        [HarmonyPatch(typeof(CannyDefensePermanent))]
        [HarmonyPatch("CheckArmor", HarmonyLib.MethodType.Normal)]
        class CannyDefensePermanent__CheckArmor__Patch
        {
            static IEnumerable<HarmonyLib.CodeInstruction> Transpiler(IEnumerable<HarmonyLib.CodeInstruction> instructions)
            {
                var codes = instructions.ToList();
                var check_shield = codes.FindIndex(x => x.opcode == System.Reflection.Emit.OpCodes.Callvirt && x.operand.ToString().Contains("HasShield"));

                codes[check_shield] = new HarmonyLib.CodeInstruction(System.Reflection.Emit.OpCodes.Call, new Func<HandSlot, bool>(Helpers.hasShield2).Method);
                return codes.AsEnumerable();
            }
        }


        [HarmonyPatch(typeof(ACBonusAgainstAttacks))]
        [HarmonyPatch("Check", HarmonyLib.MethodType.Normal)]
        class ACBonusAgainstAttacks__Check__Patch
        {
            static IEnumerable<HarmonyLib.CodeInstruction> Transpiler(IEnumerable<HarmonyLib.CodeInstruction> instructions)
            {
                var codes = instructions.ToList();
                var check_shield = codes.FindIndex(x => x.opcode == System.Reflection.Emit.OpCodes.Callvirt && x.operand.ToString().Contains("HasShield"));

                codes[check_shield] = new HarmonyLib.CodeInstruction(System.Reflection.Emit.OpCodes.Call, new Func<HandSlot, bool>(Helpers.hasShield2).Method);
                return codes.AsEnumerable();
            }
        }


        [HarmonyPatch(typeof(DuelistPreciseStrike))]
        [HarmonyPatch("OnEventAboutToTrigger", HarmonyLib.MethodType.Normal)]
        class DuelistPreciseStrike__OnEventAboutToTrigger__Patch
        {
            static IEnumerable<HarmonyLib.CodeInstruction> Transpiler(IEnumerable<HarmonyLib.CodeInstruction> instructions)
            {
                var codes = instructions.ToList();
                var check_shield = codes.FindIndex(x => x.opcode == System.Reflection.Emit.OpCodes.Callvirt && x.operand.ToString().Contains("HasShield"));

                codes[check_shield] = new HarmonyLib.CodeInstruction(System.Reflection.Emit.OpCodes.Call, new Func<HandSlot, bool>(Helpers.hasShield2).Method);
                return codes.AsEnumerable();
            }
        }


        [HarmonyPatch(typeof(DeflectArrows))]
        [HarmonyPatch("CheckRestriction", HarmonyLib.MethodType.Normal)]
        class DeflectArrows__CheckRestriction__Patch
        {
            static IEnumerable<HarmonyLib.CodeInstruction> Transpiler(IEnumerable<HarmonyLib.CodeInstruction> instructions)
            {
                var codes = instructions.ToList();

                var check_shield = codes.FindIndex(x => x.opcode == System.Reflection.Emit.OpCodes.Callvirt && x.operand.ToString().Contains("HasShield"));
                codes[check_shield] = new HarmonyLib.CodeInstruction(System.Reflection.Emit.OpCodes.Call, new Func<HandSlot, bool>(Helpers.hasShield2).Method);
                check_shield = codes.FindIndex(x => x.opcode == System.Reflection.Emit.OpCodes.Callvirt && x.operand.ToString().Contains("HasShield"));
                codes[check_shield] = new HarmonyLib.CodeInstruction(System.Reflection.Emit.OpCodes.Call, new Func<HandSlot, bool>(Helpers.hasShield2).Method);
                return codes.AsEnumerable();
            }
        }


        [HarmonyPatch(typeof(RestrictionCanGatherPower))]
        [HarmonyPatch("IsAvailable", HarmonyLib.MethodType.Normal)]
        class RestrictionCanGatherPower__IsAvailable__Patch
        {
            static bool Prefix(RestrictionCanGatherPower __instance, ref bool __result)
            {
                __result = getResult(__instance);
                return false;
            }


            static bool getResult(RestrictionCanGatherPower __instance)
            {
                UnitPartKineticist unitPartKineticist = __instance.Owner.Get<UnitPartKineticist>();
                if (unitPartKineticist == null)
                    return false;
                UnitBody body = __instance.Owner.Body;
                if (body.IsPolymorphed)
                    return false;
                ItemEntity maybeItem1 = body.PrimaryHand.MaybeItem;
                WeaponCategory? category = (maybeItem1 as ItemEntityWeapon)?.Blueprint.Category;
                bool flag = category.GetValueOrDefault() == WeaponCategory.KineticBlast && category.HasValue;
                if (maybeItem1 != null && !flag)
                    return false;

                ItemEntity maybeItem2 = body.SecondaryHand.MaybeItem;
                if (maybeItem2 == null)
                    return true;
                ArmorProficiencyGroup? proficiencyGroup = body.SecondaryHand.MaybeShield?.Blueprint.Type.ProficiencyGroup;

                if (proficiencyGroup.HasValue && (proficiencyGroup.GetValueOrDefault() != ArmorProficiencyGroup.TowerShield))
                    return unitPartKineticist.CanGatherPowerWithShield || Helpers.hasFreeHand(body.SecondaryHand);
                return false;
            }
        }



        [HarmonyPatch(typeof(ItemEntityWeapon))]
        [HarmonyPatch("GetAnimationStyle", HarmonyLib.MethodType.Normal)]
        class ItemEntityWeapon__GetAnimationStyle__Patch
        {
            static void Postfix(ItemEntityWeapon __instance, bool forDollRoom, ref WeaponAnimationStyle __result)
            {
                if (__instance == null)
                {
                    return;
                }
                if ((__instance.Blueprint.IsTwoHanded && !__instance.HoldInTwoHands) //2h that is held as 1h
                    || ((__instance.Blueprint.IsTwoHanded || __instance.Blueprint.IsOneHandedWhichCanBeUsedWithTwoHands) && forDollRoom)) // make weapon look 1h in the doll room to see the shield if possible
                {
                    var pair_slot = (__instance.HoldingSlot as HandSlot)?.PairSlot;
                    if (!(pair_slot?.HasItem).GetValueOrDefault())
                    {
                        return;
                    }

                    switch (__result)
                    {
                        case WeaponAnimationStyle.AxeTwoHanded:
                            __result = WeaponAnimationStyle.SlashingOneHanded;
                            break;
                        case WeaponAnimationStyle.PiercingTwoHanded:
                            __result = WeaponAnimationStyle.PiercingOneHanded;
                            break;
                        case WeaponAnimationStyle.SlashingTwoHanded:
                            __result = WeaponAnimationStyle.SlashingOneHanded;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        //EmptyHandWeaponOverrideNotification
        [HarmonyPatch(typeof(UnitBody))]
        [HarmonyPatch("SetEmptyHandWeapon", HarmonyLib.MethodType.Normal)]
        class UnitBody__SetEmptyHandWeapon__Patch
        {
            public static bool no_animation_action = false;
            //signal weapon set change to notify that weapons could have been changed
            static void Postfix(UnitBody __instance, BlueprintItemWeapon weapon)
            {
                no_animation_action = true;
                EventBus.RaiseEvent<IUnitActiveEquipmentSetHandler>((Action<IUnitActiveEquipmentSetHandler>)(h => h.HandleUnitChangeActiveEquipmentSet(__instance.Owner)));
                no_animation_action = false;
            }
        }

        //make signal from created weapons no longer consume move action
        [HarmonyPatch(typeof(UnitViewHandsEquipment))]
        [HarmonyPatch("HandleEquipmentSetChanged", HarmonyLib.MethodType.Normal)]
        class UnitViewHandsEquipment_HandleEquipmentSetChanged_Patch
        {
            static bool Prefix(UnitViewHandsEquipment __instance)
            {
                var tr = HarmonyLib.Traverse.Create(__instance);

                if (UnitBody__SetEmptyHandWeapon__Patch.no_animation_action)
                {
                    tr.Method("ChangeEquipmentWithoutAnimation").GetValue();
                    return false;
                }

                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(UnitBody))]
        [HarmonyLib.HarmonyPatch("RemoveEmptyHandWeapon", HarmonyLib.MethodType.Normal)]
        class UnitBody__RemoveEmptyHandWeaponEmptyHandWeapon__Patch
        {
            //signal weapon set change to notify that weapons could have been changed
            static void Postfix(UnitBody __instance, BlueprintItemWeapon weapon)
            {
                UnitBody__SetEmptyHandWeapon__Patch.no_animation_action = true;
                EventBus.RaiseEvent<IUnitActiveEquipmentSetHandler>((Action<IUnitActiveEquipmentSetHandler>)(h => h.HandleUnitChangeActiveEquipmentSet(__instance.Owner)));
                UnitBody__SetEmptyHandWeapon__Patch.no_animation_action = false;
            }
        }




        public class UnitPartConsiderAsLightWeapon : AdditiveUnitPart
        {
            public bool canBeUsedOn(ItemEntityWeapon weapon)
            {
                if (buffs.Empty())
                {
                    return false;
                }

                foreach (var b in buffs)
                {
                    bool result = false;
                    b.CallComponents<ConsiderAsLightWeaponBase>(c => result = c.CanBeUsedOn(weapon));
                    if (result)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public abstract class ConsiderAsLightWeaponBase : UnitFactComponentDelegate
        {
            abstract public bool CanBeUsedOn(ItemEntityWeapon weapon);

            public override void OnActivate()
            {
                this.Owner.Ensure<UnitPartConsiderAsLightWeapon>().AddBuff(this.Fact);
            }


            public override void OnDeactivate()
            {
                this.Owner.Ensure<UnitPartConsiderAsLightWeapon>().RemoveBuff(this.Fact);
            }
        }

        public class ConsiderWeaponCategoriesAsLightWeapon : ConsiderAsLightWeaponBase
        {
            public WeaponCategory[] categories;

            public override void OnActivate()
            {
                base.OnActivate();
                foreach (var c in categories)
                {
                    //this.Owner.Ensure<DamageGracePart>().AddEntry(c, this.Fact);
                }
            }

            public override void OnDeactivate()
            {
                base.OnDeactivate();
                foreach (var c in categories)
                {
                    //this.Owner.Ensure<DamageGracePart>().RemoveEntry(this.Fact);
                }
            }

            public override bool CanBeUsedOn(ItemEntityWeapon weapon)
            {
                if (weapon == null)
                {
                    return false;
                }

                return categories.Contains(weapon.Blueprint.Category);
            }
        }



    }


}