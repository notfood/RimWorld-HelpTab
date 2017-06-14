using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;
using UnityEngine;

namespace HelpTab
{

    [StaticConstructorOnStartup]
    public static class RecipeDef_Extensions
    {
        #region Availability

        public static bool HasResearchRequirement(this RecipeDef recipeDef)
        {
            // Can't entirely rely on this one check as it's state may change mid-game
            if (recipeDef.researchPrerequisite != null)
            {
                // Easiest check, do it first
                return true;
            }

            // Get list of things referencing
            var thingsOn = DefDatabase<ThingDef>.AllDefsListForReading.Where(t => (
               (t.recipes != null) &&
               (t.recipes.Contains(recipeDef))
           )).ToList();

            if (thingsOn == null)
            {
                thingsOn = new List<ThingDef>();
            }
            else
            {
                thingsOn.AddRangeUnique(recipeDef.recipeUsers);
            }

            // Now check for an absolute requirement
            return (thingsOn.All(t => t.HasResearchRequirement()));
        }

        #endregion

        #region Lists of affected data

        public static List<Def> GetResearchRequirements(this RecipeDef recipeDef)
        {
            var researchDefs = new List<Def>();

            if (recipeDef.researchPrerequisite != null)
            {
                // Basic requirement
                researchDefs.AddUnique(recipeDef.researchPrerequisite);
            }

            // Get list of things recipe is used on
            var thingsOn = new List<ThingDef>();
            var recipeThings = DefDatabase<ThingDef>.AllDefsListForReading.Where(t => (
             (t.recipes != null) &&
             (t.recipes.Contains(recipeDef))
         )).ToList();

            if (!recipeThings.NullOrEmpty())
            {
                thingsOn.AddRangeUnique(recipeThings);
            }

            // Add those linked via the recipe
            if (!recipeDef.recipeUsers.NullOrEmpty())
            {
                thingsOn.AddRangeUnique(recipeDef.recipeUsers);
            }

            // Make sure they all have hard requirements
            if (
                (!thingsOn.NullOrEmpty()) &&
                (thingsOn.All(t => t.HasResearchRequirement()))
            )
            {
                foreach (var t in thingsOn)
                {
                    researchDefs.AddRangeUnique(t.GetResearchRequirements());
                }
            }

            // Return the list of research required
            return researchDefs;
        }

        public static List<ThingDef> GetRecipeUsers(this RecipeDef recipeDef)
        {
            var thingDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where(t =>
                   !t.AllRecipes.NullOrEmpty() &&
                   t.AllRecipes.Contains(recipeDef)
            ).ToList();

            if (!thingDefs.NullOrEmpty())
            {
                return thingDefs;
            }
            return new List<ThingDef>();
        }

        public static List<ThingDef> GetThingsUnlocked(this RecipeDef recipeDef, ref List<Def> researchDefs)
        {
            // Things it is unlocked on with research
            var thingDefs = new List<ThingDef>();
            if (researchDefs != null)
            {
                researchDefs.Clear();
            }

            if (recipeDef.researchPrerequisite != null)
            {
                thingDefs.AddRangeUnique(recipeDef.recipeUsers);
                if (researchDefs != null)
                {
                    researchDefs.AddUnique(recipeDef.researchPrerequisite);
                }
            }

            return thingDefs;
        }

		public static bool TryFindBestRecipeIngredientsInSet_NoMix(this RecipeDef recipeDef, List<Thing> availableThings, List<ThingAmount> chosen)
        {
            chosen.Clear();
            List<IngredientCount> ingredientsOrdered = new List<IngredientCount>();
            HashSet<Thing> assignedThings = new HashSet<Thing>();
            DefCountList availableCounts = new DefCountList();
            availableCounts.GenerateFrom(availableThings);

            for (int ingredientIndex = 0; ingredientIndex < recipeDef.ingredients.Count; ++ingredientIndex)
            {
                IngredientCount ingredientCount = recipeDef.ingredients[ingredientIndex];
                if (ingredientCount.filter.AllowedDefCount == 1)
                {
                    ingredientsOrdered.Add(ingredientCount);
                }
            }
            for (int ingredientIndex = 0; ingredientIndex < recipeDef.ingredients.Count; ++ingredientIndex)
            {
                IngredientCount ingredientCount = recipeDef.ingredients[ingredientIndex];
                if (!ingredientsOrdered.Contains(ingredientCount))
                {
                    ingredientsOrdered.Add(ingredientCount);
                }
            }

            for (int orderedIndex = 0; orderedIndex < ingredientsOrdered.Count; ++orderedIndex)
            {
                IngredientCount ingredientCount = recipeDef.ingredients[orderedIndex];
                bool hasAllRequired = false;
                for (int countsIndex = 0; countsIndex < availableCounts.Count; ++countsIndex)
                {
                    float countRequiredFor = (float)ingredientCount.CountRequiredOfFor(availableCounts.GetDef(countsIndex), recipeDef);
                    if (
                        ((double)countRequiredFor <= (double)availableCounts.GetCount(countsIndex)) &&
                        (ingredientCount.filter.Allows(availableCounts.GetDef(countsIndex)))
                    )
                    {
                        for (int thingsIndex = 0; thingsIndex < availableThings.Count; ++thingsIndex)
                        {
                            if (
                                (availableThings[thingsIndex].def == availableCounts.GetDef(countsIndex)) &&
                                (!assignedThings.Contains(availableThings[thingsIndex]))
                            )
                            {
                                int countToAdd = Mathf.Min(Mathf.FloorToInt(countRequiredFor), availableThings[thingsIndex].stackCount);
                                ThingAmount.AddToList(chosen, availableThings[thingsIndex], countToAdd);
                                countRequiredFor -= (float)countToAdd;
                                assignedThings.Add(availableThings[thingsIndex]);
                                if ((double)countRequiredFor < 1.0 / 1000.0)
                                {
                                    hasAllRequired = true;
                                    float val = availableCounts.GetCount(countsIndex) - ingredientCount.GetBaseCount();
                                    availableCounts.SetCount(countsIndex, val);
                                    break;
                                }
                            }
                        }
                        if (hasAllRequired)
                        {
                            break;
                        }
                    }
                }
                if (!hasAllRequired)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool TryFindBestRecipeIngredientsInSet_AllowMix(this RecipeDef recipeDef, List<Thing> availableThings, List<ThingAmount> chosen)
        {
            chosen.Clear();
            for (int ingredientIndex = 0; ingredientIndex < recipeDef.ingredients.Count; ++ingredientIndex)
            {
                IngredientCount ingredientCount = recipeDef.ingredients[ingredientIndex];
                float baseCount = ingredientCount.GetBaseCount();
                for (int thingIndex = 0; thingIndex < availableThings.Count; ++thingIndex)
                {
                    Thing thing = availableThings[thingIndex];
                    if (ingredientCount.filter.Allows(thing))
                    {
                        float ingredientValue = recipeDef.IngredientValueGetter.ValuePerUnitOf(thing.def);
                        int countToAdd = Mathf.Min(Mathf.CeilToInt(baseCount / ingredientValue), thing.stackCount);
                        ThingAmount.AddToList(chosen, thing, countToAdd);
                        baseCount -= (float)countToAdd * ingredientValue;
                        if ((double)baseCount <= 9.99999974737875E-05)
                        {
                            break;
                        }
                    }
                }
                if ((double)baseCount > 9.99999974737875E-05)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Helper class for TryFindBestRecipeIngredientsInSet_NoMix

        private class DefCountList
        {
            private List<ThingDef> defs;
            private List<float> counts;

            public int Count
            {
                get
                {
                    return defs.Count;
                }
            }

            public float this[ThingDef def]
            {
                get
                {
                    int index = defs.IndexOf(def);
                    if (index < 0)
                    {
                        return 0.0f;
                    }
                    return counts[index];
                }
                set
                {
                    int index = defs.IndexOf(def);
                    if (index < 0)
                    {
                        defs.Add(def);
                        counts.Add(value);
                        index = defs.Count - 1;
                    }
                    else
                    {
                        counts[index] = value;
                    }
                    CheckRemove(index);
                }
            }

            public DefCountList()
            {
                defs = new List<ThingDef>();
                counts = new List<float>();
            }

            public float GetCount(int index)
            {
                return counts[index];
            }

            public void SetCount(int index, float val)
            {
                counts[index] = val;
                CheckRemove(index);
            }

            public ThingDef GetDef(int index)
            {
                return defs[index];
            }

            private void CheckRemove(int index)
            {
                if (counts[index] > 0.0)
                {
                    return;
                }
                counts.RemoveAt(index);
                defs.RemoveAt(index);
            }

            public void Clear()
            {
                defs.Clear();
                counts.Clear();
            }

            public void GenerateFrom(List<Thing> things)
            {
                Clear();
                for (int index = 0; index < things.Count; ++index)
                {
                    this[things[index].def] += (float)things[index].stackCount;
                }
            }

        }

        #endregion

    }

}