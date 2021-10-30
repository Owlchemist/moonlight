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
        static void Postfix(ref SkyTarget? __result)
		{  
            if (__result.HasValue) __result.value.colors.sky *= brightnessMid[day];
        }
    }

    [HarmonyPatch(typeof(World), nameof(World.WorldTick))]
	public class Patch_WorldTick
	{
        static void Postfix()
        {
            //Noon?
            if(++ticks == 2500 && FindMiddle() == 12) UpdateMoonlight();
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.LoadGame))]
	public class Patch_LoadGame
	{
        static void Postfix()
        {
            Setup();
        }
    }
}