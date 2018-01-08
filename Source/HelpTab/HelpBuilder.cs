using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using RimWorld;
using Verse;

namespace HelpTab
{

    public static class HelpBuilder
    {

        //[Unsaved]

        #region Instance Data

		static readonly string HelpPostFix = "_HelpCategoryDef",

			// items
			ApparelHelp = "Apparel" + HelpPostFix,
			BodyPartHelp = "BodyPart" + HelpPostFix,
			DrugHelp = "Drug" + HelpPostFix,
			MealHelp = "Meal" + HelpPostFix,
			WeaponHelp = "Weapon" + HelpPostFix,

			// flora and fauna
			TerrainHelp = "Terrain" + HelpPostFix,
			Plants = "Plants" + HelpPostFix,
			Animals = "Animals" + HelpPostFix,
			Humanoids = "Humanoids" + HelpPostFix,
			Mechanoids = "Mechanoids" + HelpPostFix,
			Biomes = "Biomes" + HelpPostFix,

			// recipes and research
			RecipeHelp = "Recipe" + HelpPostFix,
			ResearchHelp = "Research" + HelpPostFix;

        #endregion

        #region Process State

        public static void ResolveImpliedDefs()
        {

            // Items
            ResolveApparel();
            ResolveBodyParts();
            ResolveDrugs();
            ResolveMeals();
            ResolveWeapons();

            // TODO: Add stuff categories
            // TODO: Add workTypes
            // TODO: Add capacities
            // TODO: Add skills

            // The below are low priority  (as considered by Fluffy)
            // TODO: Add needs
            // TODO: Add building resources
            // TODO: Add factions
            // TODO: Add hediffs

            // The below are really low priority (as considered by Fluffy)
            // TODO: Add traders
            // TODO: Add tradertags

            // Buildings
            ResolveBuildings();
            ResolveMinifiableOnly();

            // Terrain
            ResolveTerrain();

            // flora and fauna
            ResolvePlants();
            ResolvePawnkinds();
            ResolveBiomes();

            // Recipes
            ResolveRecipes();

            // Research
            ResolveResearch();

            // Rebuild help caches
            ResolveReferences();
        }

        static void ResolveReferences()
        {
            foreach (var helpCategory in DefDatabase<HelpCategoryDef>.AllDefsListForReading)
            {
                helpCategory.Recache();
            }
            MainTabWindow_ModHelp.Recache();
        }

        #endregion

        #region Item Resolvers

        static void ResolveApparel()
        {
            // Get list of things
            var thingDefs =
                DefDatabase<ThingDef>.AllDefsListForReading.Where(t => (
                 (t.thingClass == typeof(Apparel))
             )).ToList();

            if (thingDefs.NullOrEmpty())
            {
                return;
            }

            // Get help category
			var helpCategoryDef = HelpCategoryForKey(ApparelHelp, ResourceBank.String.AutoHelpSubCategoryApparel, ResourceBank.String.AutoHelpCategoryItems);

            // Scan through all possible buildable defs and auto-generate help
            ResolveDefList(
                thingDefs,
                helpCategoryDef
            );
        }

        static void ResolveBodyParts()
        {
            // Get list of things
            var thingDefs = (
                from thing in DefDatabase<ThingDef>.AllDefsListForReading
                where typeof(ThingWithComps).IsAssignableFrom(thing.thingClass) && thing.isBodyPartOrImplant
                select thing
            ).ToList();

            if (thingDefs.NullOrEmpty())
            {
                return;
            }

            // Get help category
			var helpCategoryDef = HelpCategoryForKey(BodyPartHelp, ResourceBank.String.AutoHelpSubCategoryBodyParts, ResourceBank.String.AutoHelpCategoryItems);

            // Scan through all possible buildable defs and auto-generate help
            ResolveDefList(
                thingDefs,
                helpCategoryDef
            );
        }

        static void ResolveDrugs()
        {
            // Get list of things
            var thingDefs = (
                from thing in DefDatabase<ThingDef>.AllDefsListForReading
                where thing.IsIngestible && thing.IsDrug
                select thing
            ).ToList();

            if (thingDefs.NullOrEmpty())
            {
                return;
            }

            // Get help category
            var helpCategoryDef = HelpCategoryForKey(DrugHelp, ResourceBank.String.AutoHelpSubCategoryDrugs, ResourceBank.String.AutoHelpCategoryItems);

            // Scan through all possible buildable defs and auto-generate help
            ResolveDefList(
                thingDefs,
                helpCategoryDef
            );
        }

        static void ResolveMeals()
        {
            // Get list of things
            var thingDefs = (
                from thing in DefDatabase<ThingDef>.AllDefsListForReading
                where thing.IsNutritionGivingIngestible
                && !thing.IsDrug && thing.category != ThingCategory.Plant
                select thing
            ).ToList();

            if (thingDefs.NullOrEmpty())
            {
                return;
            }

            // Get help category
            var helpCategoryDef = HelpCategoryForKey(MealHelp, ResourceBank.String.AutoHelpSubCategoryMeals, ResourceBank.String.AutoHelpCategoryItems);

            // Scan through all possible buildable defs and auto-generate help
            ResolveDefList(
                thingDefs,
                helpCategoryDef
            );
        }

        static void ResolveWeapons()
        {
            // Get list of things
            var thingDefs = (
                from thing in DefDatabase<ThingDef>.AllDefsListForReading
                where thing.IsWeapon
                select thing
            ).ToList();

            if (thingDefs.NullOrEmpty())
            {
                return;
            }

            // Get help category
            var helpCategoryDef = HelpCategoryForKey(WeaponHelp, ResourceBank.String.AutoHelpSubCategoryWeapons, ResourceBank.String.AutoHelpCategoryItems);

            // Scan through all possible buildable defs and auto-generate help
            ResolveDefList(
                thingDefs,
                helpCategoryDef
            );
        }

        #endregion

        #region Building Resolvers

        static void ResolveBuildings()
        {
            // Go through buildings by designation categories
            foreach (var designationCategoryDef in DefDatabase<DesignationCategoryDef>.AllDefsListForReading)
            {
                // Get list of things
                var thingDefs = (
                    from thing in DefDatabase<ThingDef>.AllDefsListForReading
                    where thing.designationCategory == designationCategoryDef
                    select thing
                ).ToList();

                if (thingDefs.NullOrEmpty())
                {
                    continue;
                }

                // Get help category
                var helpCategoryDef = HelpCategoryForKey (designationCategoryDef.defName + "_Building" + HelpPostFix, designationCategoryDef.label, ResourceBank.String.AutoHelpCategoryBuildings);

                // Scan through all possible buildable defs and auto-generate help
                ResolveDefList (
                    thingDefs,
                    helpCategoryDef
                );
            }
        }

        static void ResolveMinifiableOnly()
        {
            // Get list of things
            var thingDefs = (
                from thing in DefDatabase<ThingDef>.AllDefsListForReading
                where thing.Minifiable && thing.designationCategory == null
                select thing
            ).ToList();

            if (thingDefs.NullOrEmpty())
            {
                return;
            }

            // Get help category
            var helpCategoryDef = HelpCategoryForKey("Special_Building" + HelpPostFix, ResourceBank.String.AutoHelpSubCategorySpecial, ResourceBank.String.AutoHelpCategoryBuildings);

            // Scan through all possible buildable defs and auto-generate help
            ResolveDefList(
                thingDefs,
                helpCategoryDef
            );
        }

        #endregion

        #region Terrain Resolver

        static void ResolveTerrain()
        {
            // Get list of terrainDefs without designation category that occurs as a byproduct of mining (rocky),
            // or is listed in biomes (natural terrain). This excludes terrains that are not normally visible (e.g. Underwall).
            string[] rockySuffixes = new[] { "_Rough", "_Smooth", "_RoughHewn" };

            List<TerrainDef> terrainDefs =
                DefDatabase<TerrainDef>.AllDefsListForReading
                                       .Where(
                                            // not buildable
                                            t => (t.designationCategory == null)
                                            && (
                                                // is a type generated from rock
                                                rockySuffixes.Any(s => t.defName.EndsWith(s))

                                                // or is listed in any biome
                                                || DefDatabase<BiomeDef>.AllDefsListForReading.Any(
                                                    b => b.AllTerrainDefs().Contains(t))
                                                ))
                                       .ToList();

            if (!terrainDefs.NullOrEmpty())
            {
                // Get help category
                var helpCategoryDef = HelpCategoryForKey(TerrainHelp, ResourceBank.String.AutoHelpSubCategoryTerrain, ResourceBank.String.AutoHelpCategoryTerrain);

                // resolve the defs
                ResolveDefList(terrainDefs, helpCategoryDef);
            }

            // Get list of buildable floors per designation category
            foreach (var categoryDef in DefDatabase<DesignationCategoryDef>.AllDefsListForReading)
            {
                terrainDefs =
                    DefDatabase<TerrainDef>.AllDefsListForReading.Where(t => t.designationCategory == categoryDef).ToList();

                if (!terrainDefs.NullOrEmpty())
                {
                    // Get help category
                    var helpCategoryDef = HelpCategoryForKey(categoryDef.defName + HelpPostFix, categoryDef.LabelCap, ResourceBank.String.AutoHelpCategoryTerrain);

                    // resolve the defs
                    ResolveDefList(terrainDefs, helpCategoryDef);
                }
            }
        }

        #endregion

        #region Flora and Fauna resolvers

        static void ResolvePlants()
        {
            // plants
            var plants = DefDatabase<ThingDef>.AllDefsListForReading.Where(t => t.plant != null).ToList();
            var category = HelpCategoryForKey(Plants, ResourceBank.String.AutoHelpSubCategoryPlants,
                                               ResourceBank.String.AutoHelpCategoryFloraAndFauna);

            ResolveDefList(plants, category);
        }

        static void ResolvePawnkinds()
        {
            // animals
            List<PawnKindDef> pawnkinds =
                DefDatabase<PawnKindDef>.AllDefsListForReading.Where(t => t.race.race.Animal).ToList();
            HelpCategoryDef category = HelpCategoryForKey(Animals, ResourceBank.String.AutoHelpSubCategoryAnimals,
                                               ResourceBank.String.AutoHelpCategoryFloraAndFauna);
            ResolveDefList(pawnkinds, category);

            // mechanoids
            pawnkinds = DefDatabase<PawnKindDef>.AllDefsListForReading.Where(t => t.race.race.IsMechanoid).ToList();
            category = HelpCategoryForKey(Mechanoids, ResourceBank.String.AutoHelpSubCategoryMechanoids,
                                           ResourceBank.String.AutoHelpCategoryFloraAndFauna);
            ResolveDefList(pawnkinds, category);

            // humanoids
            pawnkinds = DefDatabase<PawnKindDef>.AllDefsListForReading.Where(t => !t.race.race.Animal && !t.race.race.IsMechanoid).ToList();
            category = HelpCategoryForKey(Humanoids, ResourceBank.String.AutoHelpSubCategoryHumanoids,
                                           ResourceBank.String.AutoHelpCategoryFloraAndFauna);
            ResolveDefList(pawnkinds, category);

        }

        static void ResolveBiomes()
        {
            var biomes = DefDatabase<BiomeDef>.AllDefsListForReading;
            var category = HelpCategoryForKey(Biomes, ResourceBank.String.AutoHelpSubCategoryBiomes,
                                               ResourceBank.String.AutoHelpCategoryFloraAndFauna);
            ResolveDefList(biomes, category);
        }

        #endregion

        #region Recipe Resolvers

        static void ResolveRecipes()
        {
            // Get the thing database of things which ever have recipes
            var thingDefs = (
                from thing in DefDatabase<ThingDef>.AllDefsListForReading
                where thing.EverHasRecipes ()
                && typeof (Corpse).IsAssignableFrom (thing.thingClass)
                && thing.category != ThingCategory.Pawn
                select thing
            ).ToList();

            // Get help database
            var helpDefs = DefDatabase<HelpDef>.AllDefsListForReading;

            // Scan through defs and auto-generate help
            foreach (var thingDef in thingDefs)
            {
                var recipeDefs = thingDef.GetRecipesAll();
                if (!recipeDefs.NullOrEmpty())
                {
                    // Get help category
                    var helpCategoryDef = HelpCategoryForKey(thingDef.defName + "_" + RecipeHelp, thingDef.label, ResourceBank.String.AutoHelpCategoryRecipes);

                    foreach (var recipeDef in recipeDefs)
                    {
                        // Find an existing entry
                        var helpDef = helpDefs.Find(h => (
                           (h.keyDef == recipeDef) &&
                           (h.secondaryKeyDef == thingDef)
                       ));

                        if (helpDef == null)
                        {
                            // Make a new one
                            //Log.Message( "Help System :: " + recipeDef.defName );
                            helpDef = HelpForRecipe(thingDef, recipeDef, helpCategoryDef);

                            // Inject the def
                            if (helpDef != null)
                            {
                                helpDefs.Add(helpDef);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Research Resolvers

        static void ResolveResearch()
        {
            // Get research database
            var researchProjectDefs =
                DefDatabase<ResearchProjectDef>.AllDefsListForReading.ToList();

            if (researchProjectDefs.NullOrEmpty())
            {
                return;
            }

            // Get help category
            var helpCategoryDef = HelpCategoryForKey(ResearchHelp, ResourceBank.String.AutoHelpSubCategoryProjects, ResourceBank.String.AutoHelpCategoryResearch);

            // filter duplicates and create helpdefs
            ResolveDefList(researchProjectDefs, helpCategoryDef);
        }

        #endregion

        #region Help Makers

        static void ResolveDefList<T>(IEnumerable<T> defs, HelpCategoryDef category) where T : Def
        {
            // Get help database
            HashSet<Def> processedDefs =
                new HashSet<Def>(DefDatabase<HelpDef>.AllDefsListForReading.Select(h => h.keyDef));

            // Scan through defs and auto-generate help
            foreach (T def in defs)
            {
                // Check if the def doesn't already have a help entry
                if (!processedDefs.Contains(def))
                {
					// Make a new one
					HelpDef helpDef = null;
					try {
						helpDef = HelpForDef (def, category);
					} catch (Exception e) {
						Log.Warning ("HelpTab :: Failed to build help for: " + def + "\n\t" + e);
					}

					// Inject the def
					if (helpDef != null) {
						DefDatabase<HelpDef>.Add (helpDef);
					}
                }
            }
        }

        static HelpCategoryDef HelpCategoryForKey(string key, string label, string modname)
        {
            // Get help category
            var helpCategoryDef = DefDatabase<HelpCategoryDef>.GetNamed(key, false);

            if (helpCategoryDef == null)
            {
                // Create new designation help category
                helpCategoryDef = new HelpCategoryDef();
                helpCategoryDef.defName = key;
                helpCategoryDef.keyDef = key;
                helpCategoryDef.label = label;
                helpCategoryDef.ModName = modname;

                DefDatabase<HelpCategoryDef>.Add(helpCategoryDef);
            }

            return helpCategoryDef;
        }

        static HelpDef HelpForDef<T>(T def, HelpCategoryDef category) where T : Def
        {
			// both thingdefs (buildings, items) and terraindefs (floors) are derived from buildableDef
			if (def is BuildableDef)
            {
                return HelpForBuildable(def as BuildableDef, category);
            }
            if (def is ResearchProjectDef)
            {
                return HelpForResearch(def as ResearchProjectDef, category);
            }
            if (def is PawnKindDef)
            {
                return HelpForPawnKind(def as PawnKindDef, category);
            }
            if (def is RecipeDef)
            {
                return null;
            }
            if (def is BiomeDef)
            {
                return HelpForBiome(def as BiomeDef, category);
            }
            return null;
        }

        static HelpDef HelpForBuildable(BuildableDef buildableDef, HelpCategoryDef category)
        {
            // we need the thingdef in several places
            ThingDef thingDef = buildableDef as ThingDef;

            // set up empty helpdef
            var helpDef = new HelpDef();
            helpDef.defName = buildableDef.defName + "_BuildableDef_Help";
            helpDef.keyDef = buildableDef;
            helpDef.label = buildableDef.label;
            helpDef.category = category;
            helpDef.description = buildableDef.description;

            List<HelpDetailSection> statParts = new List<HelpDetailSection>();
            List<HelpDetailSection> linkParts = new List<HelpDetailSection>();

            #region Base Stats

            if (!buildableDef.statBases.NullOrEmpty())
            {
                // Look at base stats
                HelpDetailSection baseStats = new HelpDetailSection(
                    null,
                    buildableDef.statBases.Select(sb => sb.stat).ToList().ConvertAll(def => (Def)def),
                    null,
                    buildableDef.statBases.Select(sb => sb.stat.ValueToString(sb.value, sb.stat.toStringNumberSense))
                                .ToArray());

                statParts.Add(baseStats);
            }

            #endregion

            #region required research
            // Add list of required research
            var researchDefs = buildableDef.GetResearchRequirements();
            if (!researchDefs.NullOrEmpty())
            {
                HelpDetailSection reqResearch = new HelpDetailSection(
                        ResourceBank.String.AutoHelpListResearchRequired,
                        researchDefs.ConvertAll(def => (Def)def));
                linkParts.Add(reqResearch);
            }
            #endregion

            #region Cost List
            // specific thingdef costs (terrainDefs are buildable with costlist, but do not have stuff cost (oddly)).
            if (!buildableDef.costList.NullOrEmpty())
            {
                HelpDetailSection costs = new HelpDetailSection(
                    ResourceBank.String.AutoHelpCost,
                    buildableDef.costList.Select(tc => tc.thingDef).ToList().ConvertAll(def => (Def)def),
                    buildableDef.costList.Select(tc => tc.count.ToString()).ToArray());

                linkParts.Add(costs);
            }
            #endregion

            #region ThingDef Specific
            if (thingDef != null)
            {
                #region stat offsets

                if (!thingDef.equippedStatOffsets.NullOrEmpty())
                {
                    HelpDetailSection equippedOffsets = new HelpDetailSection(
                    ResourceBank.String.AutoHelpListStatOffsets,
                    thingDef.equippedStatOffsets.Select(so => so.stat).ToList().ConvertAll(def => (Def)def),
                    null,
                    thingDef.equippedStatOffsets.Select(so => so.stat.ValueToString(so.value, so.stat.toStringNumberSense))
                                .ToArray());

                    statParts.Add(equippedOffsets);
                }

                #endregion

                #region Stuff Cost

                // What stuff can it be made from?
                if (
                    (thingDef.costStuffCount > 0) &&
                    (!thingDef.stuffCategories.NullOrEmpty())
                )
                {
                    linkParts.Add(new HelpDetailSection(
                        "AutoHelpStuffCost".Translate(thingDef.costStuffCount.ToString()),
                        thingDef.stuffCategories.ToList().ConvertAll(def => (Def)def)));
                }

                #endregion

                #region Recipes (to make thing)
                List<RecipeDef> recipeDefs = buildableDef.GetRecipeDefs();
                if (!recipeDefs.NullOrEmpty())
                {
                    HelpDetailSection recipes = new HelpDetailSection(
                        ResourceBank.String.AutoHelpListRecipes,
                        recipeDefs.ConvertAll(def => (Def)def));
                    linkParts.Add(recipes);

                    // TODO: Figure out why this fails on a few select recipes (e.g. MVP's burger recipes and Apparello's Hive Armor), but works when called directly in these recipe's helpdefs.
                    var tableDefs = recipeDefs.SelectMany(r => r.GetRecipeUsers())
                                              .ToList()
                                              .ConvertAll(def => def as Def);

                    if (!tableDefs.NullOrEmpty())
                    {
                        HelpDetailSection tables = new HelpDetailSection(
                        ResourceBank.String.AutoHelpListRecipesOnThingsUnlocked, tableDefs);
                        linkParts.Add(tables);
                    }
                }
                #endregion

                #region Ingestible Stats
                // Look at base stats
                if (thingDef.IsIngestible)
                {
                    // only show Joy if it's non-zero
                    List<Def> needDefs = new List<Def>();
                    needDefs.Add(NeedDefOf.Food);
                    if (Math.Abs(thingDef.ingestible.joy) > 1e-3)
                    {
                        needDefs.Add(NeedDefOf.Joy);
                    }

                    List<string> suffixes = new List<string>();
                    suffixes.Add(thingDef.ingestible.nutrition.ToString("0.###"));
                    if (Math.Abs(thingDef.ingestible.joy) > 1e-3)
                    {
                        suffixes.Add(thingDef.ingestible.joy.ToString("0.###"));
                    }

                    // show different label for plants to show we're talking about the actual plant, not the grown veggie/fruit/etc.
                    string statLabel = ResourceBank.String.AutoHelpListNutrition;
                    if (thingDef.plant != null)
                    {
                        statLabel = ResourceBank.String.AutoHelpListNutritionPlant;
                    }

                    statParts.Add(
                        new HelpDetailSection(statLabel, needDefs, null, suffixes.ToArray()));
                }

                #endregion

                #region Body Part Stats

                if ((!thingDef.thingCategories.NullOrEmpty()) &&
                    (thingDef.thingCategories.Contains(ThingCategoryDefOf.BodyParts)) &&
                    (thingDef.isBodyPartOrImplant))
                {
                    var hediffDef = thingDef.GetImplantHediffDef();

                    #region Efficiency

                    if (hediffDef.addedPartProps != null)
                    {
                        statParts.Add(new HelpDetailSection(ResourceBank.String.BodyPartEfficiency, new[] { hediffDef.addedPartProps.partEfficiency.ToString("P0") }, null, null));
                    }

                    #endregion

                    #region Capacities
                    if ((!hediffDef.stages.NullOrEmpty()) &&
                        (hediffDef.stages.Exists(stage => (
                          (!stage.capMods.NullOrEmpty())
                      )))
                    )
                    {
                        HelpDetailSection capacityMods = new HelpDetailSection(
                            ResourceBank.String.AutoHelpListCapacityModifiers,
                            hediffDef.stages.Where(s => !s.capMods.NullOrEmpty())
                                            .SelectMany(s => s.capMods)
                                            .Select(cm => cm.capacity)
                                            .ToList()
                                            .ConvertAll(def => (Def)def),
                            null,
                            hediffDef.stages
                                     .Where(s => !s.capMods.NullOrEmpty())
                                     .SelectMany(s => s.capMods)
                                     .Select(
                                        cm => (cm.offset > 0 ? "+" : "") + cm.offset.ToString("P0"))
                                     .ToArray());

                        statParts.Add(capacityMods);
                    }

                    #endregion

                    /*
                    #region Components (Melee attack)

                    if ((!hediffDef.comps.NullOrEmpty()) &&
                        (hediffDef.comps.Exists(p => (
                          (p.compClass == typeof(HediffComp_VerbGiver))
                      )))
                    )
                    {
                        foreach (var comp in hediffDef.comps)
                        {
                            if (comp.compClass == typeof(HediffComp_VerbGiver))
                            {
                                if (!comp.verbs.NullOrEmpty())
                                {
                                    foreach (var verb in comp.verbs)
                                    {
                                        if (verb.verbClass == typeof(Verb_MeleeAttack))
                                        {
                                            statParts.Add(new HelpDetailSection(
                                                    "MeleeAttack".Translate(verb.meleeDamageDef.label),
                                                    new[]
                                                    {
                                                        ResourceBank.String.MeleeWarmupTime,
                                                        ResourceBank.String.StatsReport_MeleeDamage
                                                    },
                                                    null,
                                                    new[]
                                                    {
                                                        verb.defaultCooldownTicks.ToString(),
                                                        verb.meleeDamageBaseAmount.ToString()
                                                    }
                                                ));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                    */
                    #region Body part fixed or replaced
                    var recipeDef = thingDef.GetImplantRecipeDef();
                    if (!recipeDef.appliedOnFixedBodyParts.NullOrEmpty())
                    {
                        linkParts.Add(new HelpDetailSection(
                            ResourceBank.String.AutoHelpSurgeryFixOrReplace,
                            recipeDef.appliedOnFixedBodyParts.ToList().ConvertAll(def => (Def)def)));
                    }

                    #endregion

                }

                #endregion

                #region Recipes & Research (on building)

                // Get list of recipes
                recipeDefs = thingDef.AllRecipes;
                if (!recipeDefs.NullOrEmpty())
                {
                    HelpDetailSection recipes = new HelpDetailSection(
                        ResourceBank.String.AutoHelpListRecipes,
                        recipeDefs.ConvertAll(def => (Def)def));
                    linkParts.Add(recipes);
                }

                // Build help for unlocked recipes associated with building
                recipeDefs = thingDef.GetRecipesUnlocked(ref researchDefs);
                if (
                    (!recipeDefs.NullOrEmpty()) &&
                    (!researchDefs.NullOrEmpty())
                )
                {
                    HelpDetailSection unlockRecipes = new HelpDetailSection(
                        ResourceBank.String.AutoHelpListRecipesUnlocked,
                        recipeDefs.ConvertAll<Def>(def => (Def)def));
                    HelpDetailSection researchBy = new HelpDetailSection(
                        ResourceBank.String.AutoHelpListResearchBy,
                        researchDefs.ConvertAll<Def>(def => (Def)def));
                    linkParts.Add(unlockRecipes);
                    linkParts.Add(researchBy);
                }

                #endregion

                #region Power

                var powerSectionList = new List<StringDescTriplet>();

                // Get power required or generated
                var compPowerTrader = thingDef.GetCompProperties<CompProperties_Power>();
                if (compPowerTrader != null)
                {
                    if (compPowerTrader.basePowerConsumption > 0)
                    {
                        var basePowerConsumption = (int)compPowerTrader.basePowerConsumption;
                        powerSectionList.Add(new StringDescTriplet(ResourceBank.String.AutoHelpRequired, null, basePowerConsumption.ToString()));

                        /*
                        var compPowerIdle = thingDef.GetCompProperties<CompProperties_LowIdleDraw>();
                        if (compPowerIdle != null)
                        {
                            int idlePower;
                            if (compPowerIdle.idlePowerFactor < 1.0f)
                            {
                                idlePower = (int)(compPowerTrader.basePowerConsumption * compPowerIdle.idlePowerFactor);
                            }
                            else
                            {
                                idlePower = (int)compPowerIdle.idlePowerFactor;
                            }
                            powerSectionList.Add(new StringDescTriplet(ResourceBank.String.AutoHelpIdlePower, null, idlePower.ToString()));
                        }
                        */
                    }
                    else if (compPowerTrader.basePowerConsumption < 0)
                    {
                        // A14 - check this!
                        if (thingDef.HasComp(typeof(CompPowerPlantWind)))
                        {
                            powerSectionList.Add(new StringDescTriplet(ResourceBank.String.AutoHelpGenerates, null, "1700"));
                        }
                        else
                        {
                            var basePowerConsumption = (int)-compPowerTrader.basePowerConsumption;
                            powerSectionList.Add(new StringDescTriplet(ResourceBank.String.AutoHelpGenerates, null, basePowerConsumption.ToString()));
                        }
                    }
                }
                var compBattery = thingDef.GetCompProperties<CompProperties_Battery>();
                if (compBattery != null)
                {
                    var stored = (int)compBattery.storedEnergyMax;
                    var efficiency = (int)(compBattery.efficiency * 100f);
                    powerSectionList.Add(new StringDescTriplet(ResourceBank.String.AutoHelpStores, null, stored.ToString()));
                    powerSectionList.Add(new StringDescTriplet(ResourceBank.String.AutoHelpEfficiency, null, efficiency.ToString() + "%"));
                }

                if (!powerSectionList.NullOrEmpty())
                {
                    HelpDetailSection powerSection = new HelpDetailSection(
                        ResourceBank.String.AutoHelpPower,
                        null,
                        powerSectionList);
                    statParts.Add(powerSection);
                }

				#endregion

				#region Facilities

				// Get list of buildings effected by it
				var facilityProperties = thingDef.GetCompProperties<CompProperties_Facility> ();
				if (facilityProperties != null)
                {
                    var effectsBuildings = DefDatabase<ThingDef>.AllDefsListForReading
					                                            .Where(f => {
						var compProps = f.GetCompProperties<CompProperties_AffectedByFacilities> ();
						return compProps != null && compProps.linkableFacilities != null && compProps.linkableFacilities.Contains (f);
					}).ToList();
                    if (!effectsBuildings.NullOrEmpty())
                    {
                        List<DefStringTriplet> facilityDefs = new List<DefStringTriplet>();
                        List<StringDescTriplet> facilityStrings = new List<StringDescTriplet>();
                        facilityStrings.Add(new StringDescTriplet(ResourceBank.String.AutoHelpMaximumAffected, null, facilityProperties.maxSimultaneous.ToString()));

						// Look at stats modifiers if there is any
						if (!facilityProperties.statOffsets.NullOrEmpty ()) {
							foreach (var stat in facilityProperties.statOffsets) {
								facilityDefs.Add (new DefStringTriplet (stat.stat, null, ": " + stat.stat.ValueToString (stat.value, stat.stat.toStringNumberSense)));
							}
						}

                        HelpDetailSection facilityDetailSection = new HelpDetailSection(
                            ResourceBank.String.AutoHelpFacilityStats,
                            facilityDefs, facilityStrings);

                        HelpDetailSection facilitiesAffected = new HelpDetailSection(
                            ResourceBank.String.AutoHelpListFacilitiesAffected,
                            effectsBuildings.ConvertAll<Def>(def => (Def)def));

                        statParts.Add(facilityDetailSection);
                        linkParts.Add(facilitiesAffected);
                    }
                }

                #endregion

                #region Joy

                // Get valid joy givers
                var joyGiverDefs = thingDef.GetJoyGiverDefsUsing();

                if (!joyGiverDefs.NullOrEmpty())
                {
                    foreach (var joyGiverDef in joyGiverDefs)
                    {
                        // Get job driver stats
                        if (joyGiverDef.jobDef != null)
                        {
                            List<DefStringTriplet> defs = new List<DefStringTriplet>();
                            List<StringDescTriplet> strings = new List<StringDescTriplet>();

                            strings.Add(new StringDescTriplet(joyGiverDef.jobDef.reportString));
                            strings.Add(new StringDescTriplet(joyGiverDef.jobDef.joyMaxParticipants.ToString(), ResourceBank.String.AutoHelpMaximumParticipants));
                            defs.Add(new DefStringTriplet(joyGiverDef.jobDef.joyKind, ResourceBank.String.AutoHelpJoyKind));
                            if (joyGiverDef.jobDef.joySkill != null)
                            {
                                defs.Add(new DefStringTriplet(joyGiverDef.jobDef.joySkill, ResourceBank.String.AutoHelpJoySkill));
                            }

                            linkParts.Add(new HelpDetailSection(
                                ResourceBank.String.AutoHelpListJoyActivities,
                                defs, strings));
                        }
                    }
                }

                #endregion

            }

            #endregion

            #region plant extras

            if (
                (thingDef != null) &&
                (thingDef.plant != null)
            )
            {
                HelpPartsForPlant(thingDef, ref statParts, ref linkParts);
            }

            #endregion

            #region Terrain Specific
            TerrainDef terrainDef = buildableDef as TerrainDef;
            if (terrainDef != null)
            {
                HelpPartsForTerrain(terrainDef, ref statParts, ref linkParts);
            }

            #endregion

            helpDef.HelpDetailSections.AddRange(statParts);
            helpDef.HelpDetailSections.AddRange(linkParts);

            return helpDef;
        }

        static HelpDef HelpForRecipe(ThingDef thingDef, RecipeDef recipeDef, HelpCategoryDef category)
        {
            var helpDef = new HelpDef();
            helpDef.keyDef = recipeDef;
            helpDef.secondaryKeyDef = thingDef;
            helpDef.defName = helpDef.keyDef + "_RecipeDef_Help";
            helpDef.label = recipeDef.label;
            helpDef.category = category;
            helpDef.description = recipeDef.description;

            #region Base Stats

            helpDef.HelpDetailSections.Add(new HelpDetailSection(null,
                new[] { recipeDef.WorkAmountTotal((ThingDef)null).ToStringWorkAmount() },
                new[] { ResourceBank.String.WorkAmount + " : " },
                null));

            #endregion

            #region Skill Requirements

            if (!recipeDef.skillRequirements.NullOrEmpty())
            {
                helpDef.HelpDetailSections.Add(new HelpDetailSection(
                    ResourceBank.String.MinimumSkills,
                    recipeDef.skillRequirements.Select(sr => sr.skill).ToList().ConvertAll(sd => (Def)sd),
                    null,
                    recipeDef.skillRequirements.Select(sr => sr.minLevel.ToString("####0")).ToArray()));
            }

            #endregion

            #region Ingredients

            // List of ingredients
            if (!recipeDef.ingredients.NullOrEmpty())
            {
                // TODO: find the actual thingDefs of ingredients so we can use defs instead of strings.
                HelpDetailSection ingredients = new HelpDetailSection(
                    ResourceBank.String.Ingredients,
                    recipeDef.ingredients.Select(ic => recipeDef.IngredientValueGetter.BillRequirementsDescription(recipeDef, ic)).ToArray(), null, null);

                helpDef.HelpDetailSections.Add(ingredients);
            }

            #endregion

            #region Products

            // List of products
            if (!recipeDef.products.NullOrEmpty())
            {
                HelpDetailSection products = new HelpDetailSection(
                    ResourceBank.String.AutoHelpListRecipeProducts,
                    recipeDef.products.Select(tc => tc.thingDef).ToList().ConvertAll(def => (Def)def),
                    recipeDef.products.Select(tc => tc.count.ToString()).ToArray());

                helpDef.HelpDetailSections.Add(products);
            }

            #endregion

            #region Things & Research

            // Add things it's on
            var thingDefs = recipeDef.GetRecipeUsers();
            if (!thingDefs.NullOrEmpty())
            {
                HelpDetailSection billgivers = new HelpDetailSection(
                    ResourceBank.String.AutoHelpListRecipesOnThings,
                    thingDefs.ConvertAll<Def>(def => (Def)def));

                helpDef.HelpDetailSections.Add(billgivers);
            }

            // Add research required
            var researchDefs = recipeDef.GetResearchRequirements();
            if (!researchDefs.NullOrEmpty())
            {
                HelpDetailSection requiredResearch = new HelpDetailSection(
                    ResourceBank.String.AutoHelpListResearchRequired,
                    researchDefs);

                helpDef.HelpDetailSections.Add(requiredResearch);
            }

            // What things is it on after research
            thingDefs = recipeDef.GetThingsUnlocked(ref researchDefs);
            if (!thingDefs.NullOrEmpty())
            {
                HelpDetailSection recipesOnThingsUnlocked = new HelpDetailSection(
                    ResourceBank.String.AutoHelpListRecipesOnThingsUnlocked,
                    thingDefs.ConvertAll<Def>(def => (Def)def));

                helpDef.HelpDetailSections.Add(recipesOnThingsUnlocked);

                if (!researchDefs.NullOrEmpty())
                {
                    HelpDetailSection researchBy = new HelpDetailSection(
                        ResourceBank.String.AutoHelpListResearchBy,
                        researchDefs.ConvertAll<Def>(def => (Def)def));

                    helpDef.HelpDetailSections.Add(researchBy);
                }
            }
            #endregion

            return helpDef;
        }

        static HelpDef HelpForResearch(ResearchProjectDef researchProjectDef, HelpCategoryDef category)
        {
            var helpDef = new HelpDef();
            helpDef.defName = researchProjectDef.defName + "_ResearchProjectDef_Help";
            helpDef.keyDef = researchProjectDef;
            helpDef.label = researchProjectDef.label;
            helpDef.category = category;
            helpDef.description = researchProjectDef.description;

            #region Base Stats
            HelpDetailSection totalCost = new HelpDetailSection(null,
                                                                new[] { researchProjectDef.baseCost.ToString() },
                                                                new[] { ResourceBank.String.AutoHelpTotalCost },
                                                                null);
            helpDef.HelpDetailSections.Add(totalCost);

            #endregion

            #region Research, Buildings, Recipes and SowTags

            // Add research required
            var researchDefs = researchProjectDef.GetResearchRequirements();
            if (!researchDefs.NullOrEmpty())
            {
                HelpDetailSection researchRequirements = new HelpDetailSection(
                    ResourceBank.String.AutoHelpListResearchRequired,
                    researchDefs.ConvertAll<Def>(def => (Def)def));

                helpDef.HelpDetailSections.Add(researchRequirements);
            }

            // Add research unlocked
            //CCL_Log.Message(researchProjectDef.label, "getting unlocked research");
            researchDefs = researchProjectDef.GetResearchUnlocked();
            if (!researchDefs.NullOrEmpty())
            {
                HelpDetailSection reseachUnlocked = new HelpDetailSection(
                    ResourceBank.String.AutoHelpListResearchLeadsTo,
                    researchDefs.ConvertAll<Def>(def => (Def)def));

                helpDef.HelpDetailSections.Add(reseachUnlocked);
            }

            // Add buildables unlocked (items, buildings and terrain)
            List<Def> buildableDefs = new List<Def>();

            // items and buildings
            buildableDefs.AddRange(researchProjectDef.GetThingsUnlocked().ConvertAll<Def>(def => (Def)def));

            // terrain
            buildableDefs.AddRange(researchProjectDef.GetTerrainUnlocked().ConvertAll<Def>(def => (Def)def));

            // create help section
            if (!buildableDefs.NullOrEmpty())
            {
                HelpDetailSection thingsUnlocked = new HelpDetailSection(
                    ResourceBank.String.AutoHelpListThingsUnlocked,
                    buildableDefs);

                helpDef.HelpDetailSections.Add(thingsUnlocked);
            }

            // filter down to thingdefs for recipes etc.
            List<ThingDef> thingDefs =
                buildableDefs.Where(def => def is ThingDef)
                             .ToList()
                             .ConvertAll<ThingDef>(def => (ThingDef)def);

            // Add recipes it unlocks
            var recipeDefs = researchProjectDef.GetRecipesUnlocked(ref thingDefs);
            if (
                (!recipeDefs.NullOrEmpty()) &&
                (!thingDefs.NullOrEmpty())
            )
            {
                HelpDetailSection recipesUnlocked = new HelpDetailSection(
                    ResourceBank.String.AutoHelpListRecipesUnlocked,
                    recipeDefs.ConvertAll<Def>(def => (Def)def));

                helpDef.HelpDetailSections.Add(recipesUnlocked);

                HelpDetailSection recipesOnThingsUnlocked = new HelpDetailSection(
                    ResourceBank.String.AutoHelpListRecipesOnThingsUnlocked,
                    thingDefs.ConvertAll<Def>(def => (Def)def));

                helpDef.HelpDetailSections.Add(recipesOnThingsUnlocked);
            }

            // Look in advanced research to add plants and sow tags it unlocks
            var sowTags = researchProjectDef.GetSowTagsUnlocked(ref thingDefs);
            if (
                (!sowTags.NullOrEmpty()) &&
                (!thingDefs.NullOrEmpty())
            )
            {
                HelpDetailSection plantsUnlocked = new HelpDetailSection(
                    ResourceBank.String.AutoHelpListPlantsUnlocked,
                    thingDefs.ConvertAll<Def>(def => (Def)def));

                helpDef.HelpDetailSections.Add(plantsUnlocked);

                HelpDetailSection plantsIn = new HelpDetailSection(
                    ResourceBank.String.AutoHelpListPlantsIn,
                    sowTags.ToArray(), null, null);

                helpDef.HelpDetailSections.Add(plantsIn);
            }

            #endregion

            return helpDef;
        }

        static HelpDef HelpForBiome(BiomeDef biomeDef, HelpCategoryDef category)
        {
            var helpDef = new HelpDef();
            helpDef.keyDef = biomeDef;
            helpDef.defName = helpDef.keyDef + "_RecipeDef_Help";
            helpDef.label = biomeDef.label;
            helpDef.category = category;
            helpDef.description = biomeDef.description;

            #region Generic (temp, rainfall, elevation)
            // we can't get to these stats. They seem to be hardcoded in RimWorld.Planet.WorldGenerator_Grid.BiomeFrom()
            // hacky solution would be to reverse-engineer them by taking a loaded world and 5th and 95th percentiles from worldsquares with this biome.
            // however, that requires a world to be loaded.
            #endregion

            #region Diseases

            var diseases = (
                from incident in DefDatabase<IncidentDef>.AllDefsListForReading
                where incident.diseaseBiomeRecords != null
                from record in incident.diseaseBiomeRecords
                where record.biome == biomeDef && record.commonality > 0
                select incident
            ).ToList();

            if (diseases.Count > 0)
            {

                var defs = new List<Def> (diseases.Count);
                var chances = new List<string> (diseases.Count);

                foreach (var disease in diseases)
                {
                    var diseaseCommonality = biomeDef.CommonalityOfDisease(disease) / (biomeDef.diseaseMtbDays * GenDate.DaysPerYear);

                    chances.Add(diseaseCommonality.ToStringPercent());
                    defs.Add (disease.diseaseIncident);
                }

                helpDef.HelpDetailSections.Add(new HelpDetailSection(
                                                    ResourceBank.String.AutoHelpListBiomeDiseases,
                                                    defs, null, chances.ToArray()));
            }

            #endregion

            #region Terrain

            var terrains = biomeDef.AllTerrainDefs().ConvertAll(def => (Def)def);
            // commonalities unknown
            if (!terrains.NullOrEmpty())
            {
                helpDef.HelpDetailSections.Add(new HelpDetailSection(
                                                    ResourceBank.String.AutoHelpListBiomeTerrain,
                                                    terrains));
            }

            #endregion

            #region Plants

            var plants = (
                from thing in DefDatabase<ThingDef>.AllDefsListForReading
                where thing.plant != null && thing.plant.wildBiomes != null
                from record in thing.plant.wildBiomes
                where record.biome == biomeDef && record.commonality > 0
                select thing as Def
            ).ToList();

            if (!plants.NullOrEmpty())
            {
                helpDef.HelpDetailSections.Add(new HelpDetailSection(
                                                    ResourceBank.String.AutoHelpListBiomePlants,
                                                    plants));
            }

            #endregion

            #region Animals

            var animals = (
                from pawnKind in DefDatabase<PawnKindDef>.AllDefs
                where pawnKind.RaceProps != null && pawnKind.RaceProps.wildBiomes != null
                from record in pawnKind.RaceProps.wildBiomes
                where record.biome == biomeDef && record.commonality > 0
                select pawnKind as Def
            ).ToList ();

            if (!animals.NullOrEmpty())
            {
                helpDef.HelpDetailSections.Add(new HelpDetailSection(
                                                    ResourceBank.String.AutoHelpListBiomeAnimals,
                                                    animals));
            }

            #endregion

            return helpDef;
        }

        static HelpDef HelpForPawnKind(PawnKindDef kindDef, HelpCategoryDef category)
        {
            // we need the thingdef in several places
            ThingDef raceDef = kindDef.race;

            // set up empty helpdef
            var helpDef = new HelpDef();
            helpDef.defName = kindDef.defName + "_PawnKindDef_Help";
            helpDef.keyDef = kindDef;
            helpDef.label = kindDef.label;
            helpDef.category = category;
            helpDef.description = kindDef.description;

            List<HelpDetailSection> statParts = new List<HelpDetailSection>();
            List<HelpDetailSection> linkParts = new List<HelpDetailSection>();

            #region Base Stats

            if (!raceDef.statBases.NullOrEmpty())
            {
                // Look at base stats
                HelpDetailSection baseStats = new HelpDetailSection(
                    null,
                    raceDef.statBases.Select(sb => sb.stat).ToList().ConvertAll(def => (Def)def),
                    null,
                    raceDef.statBases.Select(sb => sb.stat.ValueToString(sb.value, sb.stat.toStringNumberSense))
                                .ToArray());

                statParts.Add(baseStats);
            }

            #endregion

            HelpPartsForAnimal(kindDef, ref statParts, ref linkParts);

            helpDef.HelpDetailSections.AddRange(statParts);
            helpDef.HelpDetailSections.AddRange(linkParts);

            return helpDef;
        }

        #endregion

        #region Help maker helpers

        static void HelpPartsForTerrain(TerrainDef terrainDef, ref List<HelpDetailSection> statParts, ref List<HelpDetailSection> linkParts)
        {
            statParts.Add(new HelpDetailSection(null,
                                                  new[]
                                                  {
                                                      terrainDef.fertility.ToStringPercent(),
                                                      terrainDef.pathCost.ToString()
                                                  },
                                                  new[]
                                                  {
                                                      ResourceBank.String.AutoHelpListFertility + ":",
                                                      ResourceBank.String.AutoHelpListPathCost + ":"
                                                  },
                                                  null));

            // wild biome tags
            var biomes = DefDatabase<BiomeDef>.AllDefsListForReading
                                              .Where(b => b.AllTerrainDefs().Contains(terrainDef))
                                              .ToList();
            if (!biomes.NullOrEmpty())
            {
                linkParts.Add(new HelpDetailSection(ResourceBank.String.AutoHelpListAppearsInBiomes,
                                                      biomes.Select(r => r as Def).ToList()));
            }

        }

        static void HelpPartsForPlant(ThingDef thingDef, ref List<HelpDetailSection> statParts, ref List<HelpDetailSection> linkParts)
        {
            var plant = thingDef.plant;

            // non-def stat part
            statParts.Add(new HelpDetailSection(null,
                                                  new[]
                                                  {
                                                      plant.growDays.ToString(),
                                                      plant.fertilityMin.ToStringPercent(),
                                                      plant.growMinGlow.ToStringPercent() + " - " + plant.growOptimalGlow.ToStringPercent()
                                                  },
                                                  new[]
                                                  {
                                                      ResourceBank.String.AutoHelpGrowDays,
                                                      ResourceBank.String.AutoHelpMinFertility,
                                                      ResourceBank.String.AutoHelpLightRange
                                                  },
                                                  null));

            if (plant.Harvestable)
            {
                // yield
                linkParts.Add(new HelpDetailSection(
                                   ResourceBank.String.AutoHelpListPlantYield,
                                   new List<Def>(new[] { plant.harvestedThingDef }),
                                   new[] { plant.harvestYield.ToString() }
                                   ));
            }

            // sowtags
            if (plant.Sowable)
            {
                linkParts.Add(new HelpDetailSection(ResourceBank.String.AutoHelpListCanBePlantedIn,
                                                      plant.sowTags.ToArray(), null, null));
            }

            // biomes
            if (!plant.wildBiomes.NullOrEmpty()) {
                var biomes = (
                    from record in plant.wildBiomes
                    where record.commonality > 0
                    select record.biome as Def
                ).ToList ();

                linkParts.Add (new HelpDetailSection (ResourceBank.String.AutoHelpListAppearsInBiomes, biomes));
            }
        }

        static void HelpPartsForAnimal(PawnKindDef kindDef, ref List<HelpDetailSection> statParts,
                                        ref List<HelpDetailSection> linkParts)
        {
            RaceProperties race = kindDef.race.race;
            float maxSize = race.lifeStageAges.Select(lsa => lsa.def.bodySizeFactor * race.baseBodySize).Max();

            // set up vars
            List<Def> defs = new List<Def>();
            List<string> stringDescs = new List<string>();
            List<string> prefixes = new List<string>();
            List<String> suffixes = new List<string>();

            #region Health, diet and intelligence

            statParts.Add(new HelpDetailSection(null,
                new[]
                {
                    ( race.baseHealthScale * race.lifeStageAges.Last().def.healthScaleFactor ).ToStringPercent(),
                    race.lifeExpectancy.ToStringApproxAge(),
                    race.foodType.ToHumanString(),
                    race.TrainableIntelligence.ToString()
                },
                new[]
                {
                    ResourceBank.String.AutoHelpHealthScale,
                    ResourceBank.String.AutoHelpLifeExpectancy,
                    ResourceBank.String.AutoHelpDiet,
                    ResourceBank.String.AutoHelpIntelligence
                },
                null));

            #endregion

            #region Training

            if (race.Animal)
            {
                List<DefStringTriplet> DST = new List<DefStringTriplet>();

                foreach (TrainableDef def in DefDatabase<TrainableDef>.AllDefsListForReading)
                {
                    // skip if explicitly disallowed
                    if (!race.untrainableTags.NullOrEmpty() &&
                         race.untrainableTags.Any(tag => def.MatchesTag(tag)))
                    {
                        continue;
                    }

                    // explicitly allowed tags.
                    if (!race.trainableTags.NullOrEmpty() &&
                         race.trainableTags.Any(tag => def.MatchesTag(tag)) &&
                         maxSize >= def.minBodySize)
                    {
                        DST.Add(new DefStringTriplet(def));
                        continue;
                    }

                    // A17 TODO: Check TrainableIntelligance
                    // normal proceedings
                    if (maxSize >= def.minBodySize
                    //   && race.TrainableIntelligence >= def.requiredTrainableIntelligence
                         && def.defaultTrainable)
                    {
                        DST.Add(new DefStringTriplet(def));
                    }
                }

                if (DST.Count > 0)
                {
                    linkParts.Add(new HelpDetailSection(
                                       ResourceBank.String.AutoHelpListTrainable,
                                       DST, null));
                }
                defs.Clear();
            }

            #endregion

            #region Lifestages

            List<float> ages = race.lifeStageAges.Select(age => age.minAge).ToList();
            for (int i = 0; i < race.lifeStageAges.Count; i++)
            {
                defs.Add(race.lifeStageAges[i].def);
                // final lifestage
                if (i == race.lifeStageAges.Count - 1)
                {
                    suffixes.Add(ages[i].ToStringApproxAge() + " - ~" +
                                  race.lifeExpectancy.ToStringApproxAge());
                }
                else
                // other lifestages
                {
                    suffixes.Add(ages[i].ToStringApproxAge() + " - " +
                                  ages[i + 1].ToStringApproxAge());
                }
            }

            // only print if interesting (i.e. more than one lifestage).
            if (defs.Count > 1)
            {
                statParts.Add(new HelpDetailSection(
                    ResourceBank.String.AutoHelpListLifestages,
                    defs,
                    null,
                    suffixes.ToArray()));
            }
            defs.Clear();
            suffixes.Clear();

            #endregion

            #region Reproduction

			var eggComp = kindDef.race.GetCompProperties<CompProperties_EggLayer> ();
            if (eggComp != null)
            {
                // egglayers
                string range;
                if (eggComp.eggCountRange.min == eggComp.eggCountRange.max)
                {
                    range = eggComp.eggCountRange.min.ToString();
                }
                else
                {
                    range = eggComp.eggCountRange.ToString();
                }
                stringDescs.Add("AutoHelpEggLayer".Translate(range,
                    (eggComp.eggLayIntervalDays * GenDate.TicksPerDay / GenDate.TicksPerYear).ToStringApproxAge()));

                statParts.Add(new HelpDetailSection(
                                   ResourceBank.String.AutoHelpListReproduction,
                                   stringDescs.ToArray(), null, null));
                stringDescs.Clear();
            }
            else if (
                (race.hasGenders) &&
                (race.lifeStageAges.Any(lsa => lsa.def.reproductive))
            )
            {
                // mammals
                List<StringDescTriplet> SDT = new List<StringDescTriplet>();
                SDT.Add(new StringDescTriplet(
                    (race.gestationPeriodDays * GenDate.TicksPerDay / GenDate.TicksPerYear).ToStringApproxAge(),
                    ResourceBank.String.AutoHelpGestationPeriod));

                if (
                    (race.litterSizeCurve != null) &&
                    (race.litterSizeCurve.PointsCount >= 3)
                )
                {
                    // if size is three, there is actually only one option (weird boundary restrictions by Tynan require a +/- .5 min/max)
                    if (race.litterSizeCurve.PointsCount == 3)
                    {
                        SDT.Add(new StringDescTriplet(
                            race.litterSizeCurve[1].x.ToString(),
                            ResourceBank.String.AutoHelpLitterSize));
                    }

                    // for the same reason, if more than one choice, indeces are second and second to last.
                    else
                    {
                        SDT.Add(new StringDescTriplet(
                            race.litterSizeCurve[1].x.ToString() + " - " +
                            race.litterSizeCurve[race.litterSizeCurve.PointsCount - 2].x.ToString(),
                            ResourceBank.String.AutoHelpLitterSize));
                        stringDescs.Add(ResourceBank.String.AutoHelpLitterSize);
                    }
                }
                else
                {
                    // if litterSize is not defined in XML, it's always 1
                    SDT.Add(new StringDescTriplet(
                        "1",
                        ResourceBank.String.AutoHelpLitterSize));
                }

                statParts.Add(new HelpDetailSection(
                                   ResourceBank.String.AutoHelpListReproduction,
                                   null, SDT));
            }

            #endregion

            #region Biomes

            var kinds = DefDatabase<PawnKindDef>.AllDefsListForReading.Where(t => t.race == kindDef.race);
            foreach (PawnKindDef kind in kinds)
            {
                foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefsListForReading)
                {
                    if (biome.AllWildAnimals.Contains(kind))
                    {
                        defs.Add(biome);
                    }
                }
            }
            defs = defs.Distinct().ToList();

            if (!defs.NullOrEmpty())
            {
                linkParts.Add(new HelpDetailSection(
                                   ResourceBank.String.AutoHelpListAppearsInBiomes,
                                   defs));
            }
            defs.Clear();

            #endregion

            #region Butcher products

            if (race.IsFlesh)
            {
                // fleshy pawns ( meat + leather )
                defs.Add(race.meatDef);
                prefixes.Add("~" + maxSize * StatDefOf.MeatAmount.defaultBaseValue);

                if (race.leatherDef != null)
                {
                    defs.Add(race.leatherDef);
                    prefixes.Add("~" + maxSize * kindDef.race.statBases.Find(sb => sb.stat == StatDefOf.LeatherAmount).value);
                }

                statParts.Add(new HelpDetailSection(
                    ResourceBank.String.AutoHelpListButcher,
                    defs,
                    prefixes.ToArray()));
            }
            else if (
                (race.IsMechanoid) &&
                (!kindDef.race.butcherProducts.NullOrEmpty())
            )
            {
                // metallic pawns ( mechanoids )
                linkParts.Add(new HelpDetailSection(
                                   ResourceBank.String.AutoHelpListDisassemble,
                                    kindDef.race.butcherProducts.Select(tc => tc.thingDef).ToList().ConvertAll(def => (Def)def),
                                    kindDef.race.butcherProducts.Select(tc => tc.count.ToString()).ToArray()));
            }
            defs.Clear();
            prefixes.Clear();

            #endregion

            #region Milking products

            // Need to handle subclasses (such as CompMilkableRenameable)
            var milkComp = kindDef.race.comps.Find(c => (
               (c.compClass == typeof(CompMilkable)) ||
               (c.compClass.IsSubclassOf(typeof(CompMilkable)))
           )) as CompProperties_Milkable;
            if (milkComp != null)
            {
                defs.Add(milkComp.milkDef);
                prefixes.Add(milkComp.milkAmount.ToString());
                suffixes.Add("AutoHelpEveryX".Translate(((float)milkComp.milkIntervalDays * GenDate.TicksPerDay / GenDate.TicksPerYear).ToStringApproxAge()));

                linkParts.Add(new HelpDetailSection(
                                   ResourceBank.String.AutoHelpListMilk,
                                   defs,
                                   prefixes.ToArray(),
                                   suffixes.ToArray()));
            }
            defs.Clear();
            prefixes.Clear();
            suffixes.Clear();

            #endregion

            #region Shearing products

            // Need to handle subclasses (such as CompShearableRenameable)
            var shearComp = kindDef.race.comps.Find(c => (
               (c.compClass == typeof(CompShearable)) ||
               (c.compClass.IsSubclassOf(typeof(CompShearable)))
           )) as CompProperties_Shearable;
            if (shearComp != null)
            {
                defs.Add(shearComp.woolDef);
                prefixes.Add(shearComp.woolAmount.ToString());
                suffixes.Add("AutoHelpEveryX".Translate(((float)shearComp.shearIntervalDays * GenDate.TicksPerDay / GenDate.TicksPerYear).ToStringApproxAge()));

                linkParts.Add(new HelpDetailSection(
                                   ResourceBank.String.AutoHelpListShear,
                                   defs,
                                   prefixes.ToArray(),
                                   suffixes.ToArray()));
            }
            defs.Clear();
            prefixes.Clear();
            suffixes.Clear();

            #endregion

        }
        #endregion
    }

}
