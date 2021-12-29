/*
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using System;

namespace SmallTricks
{
    class NewPoisons
    {
        static public BlueprintFeature corrosivepoisonfeature;
        static public BlueprintFeature potentpoison;
        static public BlueprintItemEquipmentUsable weakpoison1;


        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool Initialized;

            static void Postfix()
            {
                if (Initialized) return;
                Initialized = true;

                Main.LogHeader("Patching Poisons");
                try
                {
                    //var ability;

                    //weakpoison1 =CreateConsumableOnHitPoison(ability);
                }
                catch (Exception e)
                {

                    Main.Error(e, "Blah");
                }

                Main.LogHeader("Poisons Patched");
            }

            static BlueprintItemEquipmentUsable CreateConsumableOnHitPoison(BlueprintAbility ability)
            {
                var potion = Resources.GetBlueprint<BlueprintItemEquipmentUsable>("115c4dcc899ce9747a6e97335955092a"); 
             
                
                var consumable = Helpers.Create<BlueprintItemEquipmentUsable>(a=>
                {
                    a = potion;
                    a.name = "Weak Poison";
                    a.m_DisplayNameText = Helpers.CreateString("WeakPoisonName", "Weak Poison");
                    a.m_Ability =  ability.ToReference<BlueprintAbilityReference>();


                });
                consumable.name ="Weak poison";
                return consumable;


            }






        }





    }
}
*/