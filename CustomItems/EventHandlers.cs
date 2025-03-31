using HarmonyLib;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Firearms;
using InventorySystem.Items;
using PlayerRoles.FirstPersonControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MEC;
using LabApi.Features.Console;

using Logger = LabApi.Features.Console.Logger;
using LabApi.Events;

namespace CustomItems
{
    internal class EventHandlers
    {
        //patch used for triggering custom items
        [HarmonyPatch(typeof(HitscanHitregModuleBase), nameof(HitscanHitregModuleBase.RandomizeRay))]
        public static class HitscanModifier
        {

            public static void Postfix(ref Ray __result, HitscanHitregModuleBase __instance)
            {
                // CUSTOM ITEM LOGIC BELOW

                ItemBase firearm = __instance.Firearm;

                // CUSTOM ITEM LOGIC ABOVE
            }
        }
    }
}
