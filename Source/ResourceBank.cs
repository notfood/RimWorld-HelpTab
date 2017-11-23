using UnityEngine;
using Verse;

namespace HelpTab
{
    public static class ResourceBank
    {
        [StaticConstructorOnStartup]
		public static class Icon {
			/*
			public static readonly Texture2D GrowZone = ContentFinder<Texture2D>.Get ("UI/Designators/ZoneCreate_Growing");
			public static readonly Texture2D ShareSowTag = ContentFinder<Texture2D>.Get ("UI/Icons/Commands/ShareSowTag");
			public static readonly Texture2D ShareLightColor = ContentFinder<Texture2D>.Get ("UI/Icons/Commands/ShareLightColor");
			public static readonly Texture2D SelectLightColor = ContentFinder<Texture2D>.Get ("UI/Icons/Commands/SelectLightColor");
			*/

			// Help tab
			public static readonly Texture2D HelpMenuArrowUp = ContentFinder<Texture2D>.Get ("UI/HelpMenu/ArrowUp");
			public static readonly Texture2D HelpMenuArrowDown = ContentFinder<Texture2D>.Get ("UI/HelpMenu/ArrowDown");
			public static readonly Texture2D HelpMenuArrowRight = ContentFinder<Texture2D>.Get ("UI/HelpMenu/ArrowRight");
		}

        [StaticConstructorOnStartup]
		public static class String {
			public static readonly string Finished = "Finished".Translate ();
			public static readonly string InProgress = "InProgress".Translate ();
			public static readonly string Locked = "Locked".Translate ();
			public static readonly string Research = "Research".Translate ();

			public static readonly string JumpToTopic = "JumpToTopic".Translate ();

			public static readonly string BodyPartEfficiency = "BodyPartEfficiency".Translate ();
			public static readonly string AutoHelpListCapacityModifiers = "AutoHelpListCapacityModifiers".Translate ();
			public static readonly string MeleeWarmupTime = "MeleeWarmupTime".Translate ();
			public static readonly string StatsReport_MeleeDamage = "StatsReport_MeleeDamage".Translate ();

			public static readonly string WorkAmount = "WorkAmount".Translate ();
			public static readonly string MinimumSkills = "MinimumSkills".Translate ();
			public static readonly string Ingredients = "Ingredients".Translate ();

			public static readonly string AutoHelpCategoryItems = "AutoHelpCategoryItems".Translate ();
			public static readonly string AutoHelpSubCategoryApparel = "AutoHelpSubCategoryApparel".Translate ();
			public static readonly string AutoHelpSubCategoryBodyParts = "AutoHelpSubCategoryBodyParts".Translate ();
			public static readonly string AutoHelpSubCategoryDrugs = "AutoHelpSubCategoryDrugs".Translate ();
			public static readonly string AutoHelpSubCategoryMeals = "AutoHelpSubCategoryMeals".Translate ();
			public static readonly string AutoHelpSubCategoryWeapons = "AutoHelpSubCategoryWeapons".Translate ();

			public static readonly string AutoHelpCategoryBuildings = "AutoHelpCategoryBuildings".Translate ();
			public static readonly string AutoHelpSubCategorySpecial = "AutoHelpSubCategorySpecial".Translate ();

			public static readonly string AutoHelpCategoryTerrain = "AutoHelpCategoryTerrain".Translate ();
			public static readonly string AutoHelpSubCategoryTerrain = "AutoHelpSubCategoryTerrain".Translate ();
			public static readonly string AutoHelpSubCategoryPlants = "AutoHelpSubCategoryPlants".Translate ();

			public static readonly string AutoHelpCategoryFloraAndFauna = "AutoHelpCategoryFloraAndFauna".Translate ();
			public static readonly string AutoHelpSubCategoryAnimals = "AutoHelpSubCategoryAnimals".Translate ();
			public static readonly string AutoHelpSubCategoryMechanoids = "AutoHelpSubCategoryMechanoids".Translate ();
			public static readonly string AutoHelpSubCategoryHumanoids = "AutoHelpSubCategoryHumanoids".Translate ();
			public static readonly string AutoHelpSubCategoryBiomes = "AutoHelpSubCategoryBiomes".Translate ();

			public static readonly string AutoHelpCategoryRecipes = "AutoHelpCategoryRecipes".Translate ();

			public static readonly string AutoHelpCategoryResearch = "AutoHelpCategoryResearch".Translate ();
			public static readonly string AutoHelpSubCategoryProjects = "AutoHelpSubCategoryProjects".Translate ();

			public static readonly string AutoHelpLightRange = "AutoHelpLightRange".Translate ();
			public static readonly string AutoHelpListAppearsInBiomes = "AutoHelpListAppearsInBiomes".Translate ();
			public static readonly string AutoHelpListBiomeAnimals = "AutoHelpListBiomeAnimals".Translate ();
			public static readonly string AutoHelpListBiomeDiseases = "AutoHelpListBiomeDiseases".Translate ();
			public static readonly string AutoHelpListBiomePlants = "AutoHelpListBiomePlants".Translate ();
			public static readonly string AutoHelpListBiomeTerrain = "AutoHelpListBiomeTerrain".Translate ();
			public static readonly string AutoHelpListButcher = "AutoHelpListButcher".Translate ();
			public static readonly string AutoHelpListCanBePlantedIn = "AutoHelpListCanBePlantedIn".Translate ();
			public static readonly string AutoHelpListDisassemble = "AutoHelpListDisassemble".Translate ();
			public static readonly string AutoHelpListFacilitiesAffected = "AutoHelpListFacilitiesAffected".Translate ();
			public static readonly string AutoHelpListFertility = "AutoHelpListFertility".Translate ();
			public static readonly string AutoHelpListJoyActivities = "AutoHelpListJoyActivities".Translate ();
			public static readonly string AutoHelpListLifestages = "AutoHelpListLifestages".Translate ();
			public static readonly string AutoHelpListMilk = "AutoHelpListMilk".Translate ();
			public static readonly string AutoHelpListNutrition = "AutoHelpListNutrition".Translate ();
			public static readonly string AutoHelpListNutritionPlant = "AutoHelpListNutritionPlant".Translate ();
			public static readonly string AutoHelpListPathCost = "AutoHelpListPathCost".Translate ();
			public static readonly string AutoHelpListPlantsIn = "AutoHelpListPlantsIn".Translate ();
			public static readonly string AutoHelpListPlantsUnlocked = "AutoHelpListPlantsUnlocked".Translate ();
			public static readonly string AutoHelpListPlantYield = "AutoHelpListPlantYield".Translate ();
			public static readonly string AutoHelpListRecipeProducts = "AutoHelpListRecipeProducts".Translate ();
			public static readonly string AutoHelpListRecipes = "AutoHelpListRecipes".Translate ();
			public static readonly string AutoHelpListRecipesOnThings = "AutoHelpListRecipesOnThings".Translate ();
			public static readonly string AutoHelpListRecipesOnThingsUnlocked = "AutoHelpListRecipesOnThingsUnlocked".Translate ();
			public static readonly string AutoHelpListRecipesUnlocked = "AutoHelpListRecipesUnlocked".Translate ();
			public static readonly string AutoHelpListReproduction = "AutoHelpListReproduction".Translate ();
			public static readonly string AutoHelpListResearchBy = "AutoHelpListResearchBy".Translate ();
			public static readonly string AutoHelpListResearchLeadsTo = "AutoHelpListResearchLeadsTo".Translate ();
			public static readonly string AutoHelpListResearchRequired = "AutoHelpListResearchRequired".Translate ();
			public static readonly string AutoHelpListShear = "AutoHelpListShear".Translate ();
			public static readonly string AutoHelpListStatOffsets = "AutoHelpListStatOffsets".Translate ();
			public static readonly string AutoHelpListThingsUnlocked = "AutoHelpListThingsUnlocked".Translate ();
			public static readonly string AutoHelpListTrainable = "AutoHelpListTrainable".Translate ();

			public static readonly string AutoHelpCost = "AutoHelpCost".Translate ();
			public static readonly string AutoHelpDiet = "AutoHelpDiet".Translate ();
			public static readonly string AutoHelpEfficiency = "AutoHelpEfficiency".Translate ();
			public static readonly string AutoHelpFacilityStats = "AutoHelpFacilityStats".Translate ();
			public static readonly string AutoHelpGenerates = "AutoHelpGenerates".Translate ();
			public static readonly string AutoHelpGestationPeriod = "AutoHelpGestationPeriod".Translate ();
			public static readonly string AutoHelpGrowDays = "AutoHelpGrowDays".Translate ();
			public static readonly string AutoHelpHealthScale = "AutoHelpHealthScale".Translate ();
			public static readonly string AutoHelpIdlePower = "AutoHelpIdlePower".Translate ();
			public static readonly string AutoHelpIntelligence = "AutoHelpIntelligence".Translate ();
			public static readonly string AutoHelpJoyKind = "AutoHelpJoyKind".Translate ();
			public static readonly string AutoHelpJoySkill = "AutoHelpJoySkill".Translate ();
			public static readonly string AutoHelpLifeExpectancy = "AutoHelpLifeExpectancy".Translate ();
			public static readonly string AutoHelpLitterSize = "AutoHelpLitterSize".Translate ();
			public static readonly string AutoHelpMaximumAffected = "AutoHelpMaximumAffected".Translate ();
			public static readonly string AutoHelpMaximumParticipants = "AutoHelpMaximumParticipants".Translate ();
			public static readonly string AutoHelpMinFertility = "AutoHelpMinFertility".Translate ();
			public static readonly string AutoHelpPower = "AutoHelpPower".Translate ();
			public static readonly string AutoHelpRequired = "AutoHelpRequired".Translate ();
			public static readonly string AutoHelpStores = "AutoHelpStores".Translate ();
			public static readonly string AutoHelpSurgeryFixOrReplace = "AutoHelpSurgeryFixOrReplace".Translate ();
			public static readonly string AutoHelpTotalCost = "AutoHelpTotalCost".Translate ();

		}
    }

}