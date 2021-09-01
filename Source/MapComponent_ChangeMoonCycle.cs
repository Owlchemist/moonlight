using Verse;
using RimWorld;
using System.Linq;

namespace Moonlight
{
	public class MapComponent_ChangeMoonCycle : MapComponent
	{
		
        bool updatedThisDay = false;
        public MapComponent_ChangeMoonCycle(Map map) : base(map)
        {
        }

        //Rebuild the weatherdef's workers every day at noon. This may have some weird consequences for games where multiple colonies are sepreated by over 6 hours of timezone? Something to look into improving as time allows...
        public override void MapComponentUpdate()
        {
            if(!updatedThisDay && GenLocalDate.HourOfDay(map) == 12)
            {
                updatedThisDay = true;
                MoonlightMod.UpdateMoonlight();
            }

            else if(updatedThisDay && GenLocalDate.HourOfDay(map) != 12)
            {
                updatedThisDay = false;
            }
        }

        //Rebuild the weatherdef's workers on map load
        public override void FinalizeInit()
        {
            MoonlightMod.UpdateMoonlight();
        }
    }
}