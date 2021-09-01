using System;
using Verse;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace Moonlight
{
	internal class ModSettings_Moonlight : ModSettings
	{
		public override void ExposeData()
		{
			Scribe_Values.Look<float>(ref this.darkest, "darkest", 0.35f, false);
            Scribe_Values.Look<float>(ref this.brightest, "brightest", 1f, false);
			base.ExposeData();
		}

		public float darkest = 0.35f;
        public float brightest = 1f;
	}
}
