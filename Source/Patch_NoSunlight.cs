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
            int day = 8; //Fetch a default
            if (Find.CurrentMap != null) day = GenLocalDate.DayOfSeason(Find.CurrentMap);
            if (__result.HasValue)
            {
                SkyTarget newSky = __result.Value;
                newSky.colors.sky *= WeatherWorker_Moonlight.brightnessMid[day];
                return newSky;
            }
            return new SkyTarget();
        }
    }
}