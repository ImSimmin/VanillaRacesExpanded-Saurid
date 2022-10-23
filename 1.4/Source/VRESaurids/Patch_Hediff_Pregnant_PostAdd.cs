﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace VRESaurids
{
    [HarmonyPatch(typeof(Hediff_Pregnant), "Tick")]
    public static class Patch_Hediff_Pregnant_PostAdd
	{
		[HarmonyPrefix]
		public static bool Prefix(Hediff_Pregnant __instance)
		{
            if ((Find.TickManager.TicksAbs % 200 == 0) && (__instance?.pawn?.genes?.HasGene(VRESauridsDefOf.VRESaurids_Oviparous) ?? false))
            {
				// Lay the egg.
				Thing egg = LayHumanEgg(__instance.mother, __instance.father, __instance.geneSet);
				GenSpawn.Spawn(egg, __instance.pawn.Position, __instance.pawn.Map);
				// Make temporarily sterile.
				Hediff hediff = HediffMaker.MakeHediff(VRESauridsDefOf.VRESaurids_EggFatigue, __instance.pawn);
				__instance.pawn.health.AddHediff(hediff);
                // Tell the player.
                LetterMaker.MakeLetter("VRESaurids.EggLaidLabel".Translate(__instance.mother.LabelShort), "VRESaurids.EggLaidDesc".Translate(__instance.mother.LabelShort), LetterDefOf.PositiveEvent, new LookTargets(egg));
				__instance.pawn.health.RemoveHediff(__instance);
				return false;
            }
			return true;
		}

		public static Thing LayHumanEgg(Pawn mother, Pawn father, GeneSet geneSet)
        {
			Thing thing;
			thing = ThingMaker.MakeThing(VRESauridsDefOf.VRESaurids_HumanEgg);
			Comp_HumanHatcher comp = thing.TryGetComp<Comp_HumanHatcher>();
			comp.mother = mother;
			if(father != null)
            {
				comp.father = father;
			}
			comp.geneSet = geneSet;

			return thing;
        }
	}
}
