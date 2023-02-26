using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using static Moonlight.MoonlightUtility;

namespace Moonlight
{
    //This is for events such as eclipse and the sunblocker
    [HarmonyPatch(typeof(GameCondition_NoSunlight), nameof(GameCondition_NoSunlight.SkyTarget))]
	public class Patch_NoSunlight
	{
        static SkyTarget? Postfix(SkyTarget? __result)
		{  
            if (__result.HasValue)
            {
                __result.value.colors.sky *= brightnessMid[day];
                return __result;
            }
            else return null;
        }
    }

    [HarmonyPatch(typeof(TickManager), nameof(TickManager.DoSingleTick))]
	public class Patch_DoSingleTick
	{
        static void Postfix()
        {
            if(ticks-- == 0)
            {
			    ticks = 2500;
                if (FindMiddle() == 12) UpdateMoonlight(); //Noon?
            }
        }
    }

    [HarmonyPatch(typeof(World), nameof(World.FinalizeInit))]
	public class Patch_FinalizeInit
	{
        static void Postfix()
        {
            RefreshCache();
        }
    }
}