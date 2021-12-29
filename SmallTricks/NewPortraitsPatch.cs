namespace SmallTricks
{
    public class NewPortraitsPatch
    {
        /*




        public static void CreatePortrait(string minppath, string majppath, BlueprintUnit Unit)
        {
            HalfPortraitInjecotr.Replacements[Unit.PortraitSafe.Data] = Utilities.AssetLoader.IMG2Sprite.LoadNewSprite(Config.ModSettings.modpath + majppath);
            SmallPortraitInjecotr.Replacements[Unit.PortraitSafe.Data] = Utilities.AssetLoader.IMG2Sprite.LoadNewSprite(Config.ModSettings.modpath + minppath);
        }




        

        [HarmonyPatch(typeof(PortraitData), "get_HalfLengthPortrait")]
        public static class HalfPortraitInjecotr
        {
            public static Dictionary<PortraitData, Sprite> Replacements = new();
            public static bool Prefix(PortraitData __instance, ref Sprite __result)
            {
                if (Replacements.TryGetValue(__instance, out __result))
                    return false;
                return true;
            }
        }
        [HarmonyPatch(typeof(PortraitData), "get_SmallPortrait")]
        public static class SmallPortraitInjecotr
        {
            public static Dictionary<PortraitData, Sprite> Replacements = new();
            public static bool Prefix(PortraitData __instance, ref Sprite __result)
            {
                if (Replacements.TryGetValue(__instance, out __result))
                    return false;
                return true;
            }
        }
        */
    }
}
