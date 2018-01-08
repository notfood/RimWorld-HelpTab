using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace HelpTab
{

    public static class BiomeDef_Extensions
    {
        // TODO: This is a nasty method, please get rid of it. Reason: Poor performance.
        public static List<TerrainDef> AllTerrainDefs(this BiomeDef biome)
        {
            List<TerrainDef> ret = new List<TerrainDef>();

            // map terrain
            if (!biome.terrainsByFertility.NullOrEmpty())
            {
                ret.AddRangeUnique(biome.terrainsByFertility.Select(t => t.terrain));
            }

            // patch maker terrain
            if (!biome.terrainPatchMakers.NullOrEmpty())
            {
                foreach (TerrainPatchMaker patchMaker in biome.terrainPatchMakers)
                {
                    ret.AddRangeUnique(patchMaker.thresholds.Select(t => t.terrain));
                }
            }

            return ret;
        }

    }

}
