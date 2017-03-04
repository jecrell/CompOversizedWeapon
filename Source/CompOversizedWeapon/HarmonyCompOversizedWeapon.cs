﻿using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using System.Reflection;
using UnityEngine;

namespace CompOversizedWeapon
{
    [StaticConstructorOnStartup]
    static class HarmonyCompOversizedWeapon
    {
        static HarmonyCompOversizedWeapon()
        {

            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.jecrell.comps.oversized");
            harmony.Patch(typeof(PawnRenderer).GetMethod("DrawEquipmentAiming"), new HarmonyMethod(typeof(HarmonyCompOversizedWeapon).GetMethod("DrawEquipmentAimingPreFix")), null);
            harmony.Patch(AccessTools.Method(typeof(Thing), "get_Graphic"), null, new HarmonyMethod(typeof(HarmonyCompOversizedWeapon).GetMethod("get_Graphic_PostFix")));


        }


        /// <summary>
        /// Adds another "layer" to the equipment aiming if they have a
        /// weapon with a CompActivatableEffect.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="eq"></param>
        /// <param name="drawLoc"></param>
        /// <param name="aimAngle"></param>
        public static bool DrawEquipmentAimingPreFix(PawnRenderer __instance, Thing eq, Vector3 drawLoc, float aimAngle)
        {
            Pawn pawn = (Pawn)AccessTools.Field(typeof(PawnRenderer), "pawn").GetValue(__instance);

            Pawn_EquipmentTracker pawn_EquipmentTracker = pawn.equipment;
            if (pawn_EquipmentTracker != null)
            {
                ThingWithComps thingWithComps = (ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);
                if (thingWithComps != null)
                {
                    CompOversizedWeapon compOversizedWeapon = thingWithComps.TryGetComp<CompOversizedWeapon>();
                    if (compOversizedWeapon != null)
                    {
                        float num = aimAngle - 90f;
                        Mesh mesh;
                        if (aimAngle > 20f && aimAngle < 160f)
                        {
                            mesh = MeshPool.plane10;
                            num += eq.def.equippedAngleOffset;
                        }
                        else if (aimAngle > 200f && aimAngle < 340f)
                        {
                            mesh = MeshPool.plane10Flip;
                            num -= 180f;
                            num -= eq.def.equippedAngleOffset;
                        }
                        else
                        {
                            mesh = MeshPool.plane10;
                            num += eq.def.equippedAngleOffset;
                        }
                        num %= 360f;
                        Graphic_StackCount graphic_StackCount = eq.Graphic as Graphic_StackCount;
                        Material matSingle;
                        if (graphic_StackCount != null)
                        {
                            matSingle = graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle;
                        }
                        else
                        {
                            matSingle = eq.Graphic.MatSingle;
                        }
                        mesh = MeshPool.GridPlane(thingWithComps.def.graphicData.drawSize);
                        Graphics.DrawMesh(mesh, drawLoc, Quaternion.AngleAxis(num, Vector3.up), matSingle, 0);
                        return false;
                    }
                }
            }
            return true;
        }


        public static void get_Graphic_PostFix(Thing __instance, ref Graphic __result)
        {
            ThingWithComps thingWithComps = __instance as ThingWithComps;
            if (thingWithComps != null)
            {
                CompOversizedWeapon compOversizedWeapon = thingWithComps.TryGetComp<CompOversizedWeapon>();
                if (compOversizedWeapon != null)
                { 
                    Graphic tempGraphic = (Graphic)AccessTools.Field(typeof(Thing), "graphicInt").GetValue(__instance);
                    tempGraphic.drawSize = __instance.def.graphicData.drawSize;
                    __result = tempGraphic;
                }
            }

        }


    }
}
