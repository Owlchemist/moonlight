using Verse;
using RimWorld;
using System;
using System.Linq;
using static Moonlight.ModSettings_Moonlight;

namespace Moonlight
{
	[StaticConstructorOnStartup]
	static class MoonlightUtility
	{
        public static int ticks, day = 8;
        static float[] brightnessEdge = new float[] { 0.7f, 0.7425f, 0.785f, 0.8275f, 0.87f, 0.9125f, 0.955f, 1, 1, 0.955f, 0.9125f, 0.87f, 0.8275f, 0.785f, 0.7425f };
		public static float[] brightnessMid = new float[] { 0.35f, 0.45f, 0.55f, 0.65f, 0.75f, 0.85f, 0.95f, 1, 1, 0.95f, 0.85f, 0.75f, 0.65f, 0.55f, 0.45f };
		static WeatherDef[] weatherDefs;

		static MoonlightUtility()
		{
			weatherDefs = DefDatabase<WeatherDef>.AllDefs.Where(x => !x.HasModExtension<Weather>()).ToArray();
			RefreshCache();
		}

		public static void RefreshCache()
		{
			if (Current.ProgramState == ProgramState.Playing) LongEventHandler.QueueLongEvent(() => FindMiddle(), null, false, null);
			LongEventHandler.QueueLongEvent(() => RecalculateArray(), null, false, null);
		}

		//Sets the frame of reference (for the timezone) based on the middlemost world position when the player has multiple colonies
        public static int FindMiddle()
        {
			//Prepare list of coordinates to analyze
			if (Find.Maps.Count < 1) return 0;
			float[] maps = Find.Maps.Select(map => Find.WorldGrid.LongLatOf(map.Tile).x).ToArray();
			float longitude;

			//If we only have 1 map, the answer is simple and we can skip all the complicated math
			if (maps.Length == 1) longitude = maps[0];
			else
			{
				//Find averages for solution A (primemerdian) and B (antimerdian)
				float[] longitudes = new float[2] {maps.Average(), 0};

				//Solution B
				foreach (var mapCoord in maps)
				{
					longitudes[1] += (mapCoord < 0) ? mapCoord + 360 : mapCoord;
				}
				longitudes[1] /= maps.Length;

				//Analyze results
				float[] distances = new float[2];
				foreach (var mapCoord in maps)
				{
					distances[0] += Math.Abs(longitudes[0] - mapCoord);
					distances[1] += Math.Abs(longitudes[1] - (mapCoord < 0 ? mapCoord + 360 : mapCoord));
				}

				//Return the coordinate back to its original value that Rimworld recognizes
				if (longitudes[1] > 180) longitudes[1] -= 360;
			
				//Pick the best solution
				longitude = distances[0] < distances[1] ? longitudes[0] : longitudes[1];
			}

			long time = Find.TickManager.gameStartAbsTick + Find.TickManager.TicksGame + GenDate.LocalTicksOffsetFromLongitude(longitude);
			day = GenDate.DayOfSeason(time, longitude);

			//Debug
			if (logging && Prefs.DevMode)
			{
				Log.Message("[Moonlight] local time: " + GenMath.PositiveModRemap(time, 2500, 24) + " and day: " + day.ToString());
			}

			//Return
            return GenMath.PositiveModRemap(time, 2500, 24);
        }
        public static void RecalculateArray()
		{
			//Recalculate midnight darkness
			float diff = brightest - darkest;
			if (diff != 0f) diff /= 6;

			brightnessMid = new float[]
			{
				darkest,
				darkest + diff,
				darkest + (diff * 2),
				darkest + (diff * 3),
				darkest + (diff * 4),
				darkest + (diff * 5),
				darkest + (diff * 6),
				brightest,
				brightest,
				darkest + (diff * 6),
				darkest + (diff * 5),
				darkest + (diff * 4),
				darkest + (diff * 3),
				darkest + (diff * 2),
				darkest + diff
			};

			//Recalculate edge darkness
			float halfDarkest = brightest - (diff / 2);
			diff = brightest - halfDarkest;
			if (diff != 0f) diff /= 6;

			brightnessEdge = new float[]
			{ 
				halfDarkest,
				halfDarkest + diff,
				halfDarkest + (diff * 2),
				halfDarkest + (diff * 3),
				halfDarkest + (diff * 4),
				halfDarkest + (diff * 5),
				halfDarkest + (diff * 6),
				brightest,
				brightest,
				halfDarkest + (diff * 6),
				halfDarkest + (diff * 5),
				halfDarkest + (diff * 4),
				halfDarkest + (diff * 3),
				halfDarkest + (diff * 2),
				halfDarkest + diff
			};

			UpdateMoonlight();
		}
		public static void UpdateMoonlight()
		{
			//Debug before...
			string originalValue = "";
			if (logging && Prefs.DevMode && Current.ProgramState == ProgramState.Playing) originalValue = DefDatabase<WeatherDef>.GetNamed("Clear").workerInt.skyTargets[0].colors.sky.ToString();

			if (weatherDefs == null)
			{
				Log.Warning("[Moonlight] No weather defs found.");
				return;
			}
			for (int i = weatherDefs.Length; i-- > 0;)
			{
				var def = weatherDefs[i];
				def.Worker.skyTargets[0].colors.sky = def.skyColorsNightMid.sky * brightnessMid[day]; //0 is the midnight brightness
				def.Worker.skyTargets[1].colors.sky = def.skyColorsNightEdge.sky * brightnessEdge[day]; //1 is the brightness between dusk and midnight
			}

			//Debug after...
			if (logging && !originalValue.NullOrEmpty())
			{
				Log.Message("[Moonlight] Base sky changed from " + originalValue + " to " + (DefDatabase<WeatherDef>.GetNamed("Clear").workerInt.skyTargets[0].colors.sky).ToString() + " for day " + day);
			}
		}
    }
}