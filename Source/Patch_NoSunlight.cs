using HarmonyLib;
using RimWorld;
using Verse;

namespace Moonlight
{
    
    [HarmonyPatch(typeof(GameCondition_NoSunlight), "SkyTarget")]
	public class Patch_NoSunlight
	{
        static SkyTarget? Postfix(SkyTarget? __result)
		{
            int day = (Find.CurrentMap != null) ? GenLocalDate.DayOfSeason(Find.CurrentMap) : 8;
            
            if (__result.HasValue)
            {
                SkyTarget newSky = __result.Value;
                newSky.colors.sky *= MoonlightMod.brightnessMid[day];
                return newSky;
            }
            return new SkyTarget();
        }
    }
}