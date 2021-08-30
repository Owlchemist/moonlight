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
            static public float[] brightnessEdge = new float[] { 0.7f, 0.7425f, 0.785f, 0.8275f, 0.87f, 0.9125f, 0.955f, 1, 1, 0.955f, 0.9125f, 0.87f, 0.8275f, 0.785f, 0.7425f };
		static public float[] brightnessMid = new float[] { 0.35f, 0.45f, 0.55f, 0.65f, 0.75f, 0.85f, 0.95f, 1, 1, 0.95f, 0.85f, 0.75f, 0.65f, 0.55f, 0.45f };
            public static void Postfix(WeatherDef def, SkyThreshold[] ___skyTargets)
		{
                  int day = 8; //Fetch a default
                  if (Find.CurrentMap != null) day = GenLocalDate.DayOfSeason(Find.CurrentMap);
                  
                  //Debug
                  if (Prefs.DevMode && def.defName == "Clear") Log.Message("[Moonlight]: Clear Sky changing from "  + ___skyTargets[0].colors.sky.ToString() + " to " + (___skyTargets[0].colors.sky * brightnessMid[day]).ToString());

                  ___skyTargets[0].colors.sky *= brightnessMid[day]; //0 is the midnight brightness
                  ___skyTargets[1].colors.sky *= brightnessEdge[day]; //1 is the brightness between dusk and midnight
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