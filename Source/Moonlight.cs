using HarmonyLib;
using Verse;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Moonlight
{
	public class MoonlightMod : Mod
	{	
		public MoonlightMod(ModContentPack content) : base(content)
		{
			new Harmony("owlchemist.moonlight").PatchAll();
		}
    }
}