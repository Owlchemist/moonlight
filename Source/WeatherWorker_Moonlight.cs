using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Moonlight
{
      [HarmonyPatch(typeof(WeatherWorker), MethodType.Constructor, typeof(WeatherDef))]
      [StaticConstructorOnStartup]
	public class WeatherWorker_Moonlight
	{
            public static void Postfix(WeatherDef def, SkyThreshold[] ___skyTargets)
		{
                  int day = 8; //Fetch a default
                  if (Find.CurrentMap != null) day = GenLocalDate.DayOfSeason(Find.CurrentMap);
                  
                  //Debug
                  if (Prefs.DevMode && Find.CurrentMap != null && def.defName == "Clear") Log.Message("[Moonlight]: Clear Sky changing from "  + ___skyTargets[0].colors.sky.ToString() + " to " + (___skyTargets[0].colors.sky * MoonlightMod.brightnessMid[day]).ToString());

                  ___skyTargets[0].colors.sky *= MoonlightMod.brightnessMid[day]; //0 is the midnight brightness
                  ___skyTargets[1].colors.sky *= MoonlightMod.brightnessEdge[day]; //1 is the brightness between dusk and midnight
            }

            public struct SkyThreshold
		{
			public SkyThreshold(SkyColorSet colors, float celGlowThreshold)
			{
				this.colors = colors;
				this.celGlowThreshold = celGlowThreshold;
			}
			public SkyColorSet colors;
			public float celGlowThreshold;
		}
      }
}