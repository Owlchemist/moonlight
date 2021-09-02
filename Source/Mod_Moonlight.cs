using HarmonyLib;
using Verse;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using UnityEngine;
using RimWorld;

namespace Moonlight
{
	public class MoonlightMod : Mod
	{	
		ModSettings_Moonlight settings;
		public static float[] brightnessEdge = new float[] { 0.7f, 0.7425f, 0.785f, 0.8275f, 0.87f, 0.9125f, 0.955f, 1, 1, 0.955f, 0.9125f, 0.87f, 0.8275f, 0.785f, 0.7425f };
		public static float[] brightnessMid = new float[] { 0.35f, 0.45f, 0.55f, 0.65f, 0.75f, 0.85f, 0.95f, 1, 1, 0.95f, 0.85f, 0.75f, 0.65f, 0.55f, 0.45f };
		public MoonlightMod(ModContentPack content) : base(content)
		{
			this.settings = base.GetSettings<ModSettings_Moonlight>();
			RecalculateArray();
			new Harmony("owlchemist.moonlight").PatchAll();
		}

		 public override void DoSettingsWindowContents(Rect inRect)
		{
			Listing_Standard options = new Listing_Standard();
			options.Begin(inRect);
			options.Label("Darkest nights (Mod default: 0.35, Min: 0.25, Max: 0.5): " + Math.Round(this.settings.darkest, 2), -1f, null);
			this.settings.darkest = options.Slider(this.settings.darkest, 0.25f, 0.5f);
			options.Label("Brightest nights (Mod default: 0.35, Min: 0.5, Max: 1): " + Math.Round(this.settings.brightest, 2), -1f, null);
			this.settings.brightest = options.Slider(this.settings.brightest, 0.5f, 1f);
			options.End();
			base.DoSettingsWindowContents(inRect);
		}
		public override string SettingsCategory()
		{
			return "Moonlight";
		}
		public override void WriteSettings()
		{
			if (this.settings != null)
			{
				this.settings.Write();
			}
			RecalculateArray();
		}

		public void RecalculateArray()
		{
			//Recalculate midnight darkness
			float diff = settings.brightest - settings.darkest;
			if (diff != 0f) diff /= 6;

			brightnessMid = new float[] { 
				settings.darkest,
				settings.darkest + diff,
				settings.darkest + (diff * 2),
				settings.darkest + (diff * 3),
				settings.darkest + (diff * 4),
				settings.darkest + (diff * 5),
				settings.darkest + (diff * 6),
				settings.brightest,
				settings.brightest,
				settings.darkest + (diff * 6),
				settings.darkest + (diff * 5),
				settings.darkest + (diff * 4),
				settings.darkest + (diff * 3),
				settings.darkest + (diff * 2),
				settings.darkest + diff};

			//Recalculate edge darkness
			float halfDarkest = settings.brightest - (diff / 2);
			diff = settings.brightest - halfDarkest;
			if (diff != 0f) diff /= 6;

			brightnessEdge = new float[] { 
				halfDarkest,
				halfDarkest + diff,
				halfDarkest + (diff * 2),
				halfDarkest + (diff * 3),
				halfDarkest + (diff * 4),
				halfDarkest + (diff * 5),
				halfDarkest + (diff * 6),
				settings.brightest,
				settings.brightest,
				halfDarkest + (diff * 6),
				halfDarkest + (diff * 5),
				halfDarkest + (diff * 4),
				halfDarkest + (diff * 3),
				halfDarkest + (diff * 2),
				halfDarkest + diff};

			UpdateMoonlight();
		}

		public static void UpdateMoonlight()
		{
			var dd = DefDatabase<WeatherDef>.AllDefs.Where(x => x.defName != "OuterSpaceWeather").ToList(); //Skip SoS2 weather. We should improve how we handle this sooner or later.
            foreach (var weather in dd)
            {
                weather.PostLoad();
            }
		}
    }
}


